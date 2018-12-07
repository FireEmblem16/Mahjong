using System;
using System.Collections.Generic;
using System.IO;
using Engine.Cards;
using Engine.Cards.CardTypes;
using Engine.Cards.CardTypes.Suits.Mahjong;
using Engine.Cards.CardTypes.Values;
using Engine.Cards.Hands;
using Engine.Game;
using Engine.Player;
using Engine.Player.AI;
using Engine.Player.AI.ABP;
using Engine.Player.AI.MonteCarlo;

namespace Mahjong
{
	public class Program
    {
		public static void Main(string[] args)
        {
			// Setup output information
			string fout = "Game " + DEPTH + "-" + SAMPLES + ".txt";
			string header = "Minimax Search Depth: " + DEPTH + "\nMonte Carlo Sample Size: " + SAMPLES + "\n\nFinal Scores\n------------\n";
			string results = "";
			string game_text = "";

            // Create the game
            MahjongGameState state = new MahjongGameState();

            // Add the AI
			state.PlayerLeft(0,new NaiveAI(0));
            state.PlayerLeft(1,new GreedyAI(1));
			state.PlayerLeft(2,new MiniMaxAI(2,DEPTH));
			state.PlayerLeft(3,new MonteCarloAI(3,SAMPLES));

			int i = 4;

            // Run the game to completion
            while(!state.GameFinished)
            {
                MahjongAIPlayer ai = state.GetPlayer(state.SubActivePlayer) as MahjongAIPlayer;
                MahjongMove move = ai.AI.GetNextMove(state);

				Log(PlayerToName(state.SubActivePlayer) + " " + move + ".\n",ref game_text);

				if(--i == 0)
				{
					Log("\n",ref game_text);
					i = 4;
				}

                if(!state.ApplyMove(move))
                    Log("An invalid move has been provided.\n",ref game_text);

				if(state.HandFinished && !state.GameFinished)
				{
					Print(state,ref game_text);
					state.StartHand();
				}
            }

			// Output the final scores
			PrintLight(state,ref results);
			Print(state,ref game_text,true);

			File.WriteAllText(fout,header + results + "\nGame Log\n--------\n" + game_text);
            return;
        }

		private static void Log(string str, ref string append_to)
		{
			if(VERBOSE)
				Console.Write(str);

			append_to += str;
			return;
		}

        private static void Print(MahjongGameState state, ref string append_to, bool done = false)
        {
			if(done)
				Log("Game Finished!\n--------------\n",ref append_to);
			else
				Log("Hand Finished!\n--------------\n",ref append_to);

			PrintLight(state,ref append_to);

			if(!done)
				Log("\n",ref append_to);
			else
				append_to = append_to.Substring(0,append_to.Length - 1); // Chop off the extra newline

            return;
        }

		private static string PlayerToName(int i)
		{
			switch(i)
			{
			case 0:
				return "NaiveAI";
			case 1:
				return "GreedyAI";
			case 2:
				return "MinimaxAI";
			case 3:
				return "MonteCarloAI";
			}

			return "Error Player";
		}

		private static void PrintLight(MahjongGameState state, ref string append_to)
		{
			Log(PlayerToName(0) + "'s Score: " + (state.GetPlayer(0) as MahjongPlayer).Score + "\n",ref append_to);
            Log(PlayerToName(1) + "'s Score: " + (state.GetPlayer(1) as MahjongPlayer).Score + "\n",ref append_to);
            Log(PlayerToName(2) + "'s Score: " + (state.GetPlayer(2) as MahjongPlayer).Score + "\n",ref append_to);
            Log(PlayerToName(3) + "'s Score: " + (state.GetPlayer(3) as MahjongPlayer).Score + "\n",ref append_to);

			return;
		}
		
        private static readonly bool VERBOSE = true;

		private static readonly int DEPTH = 12;
		private static readonly int SAMPLES = 20;
    }
	
    /// <summary>
    /// An AI for Mahjong that declares mahjong when it can (with only tiles drawn from the wall) and otherwise performs random discards and passes.
    /// This AI will almost certainly score 0 every hand.
    /// </summary>
    public class NaiveAI : AIBehavior<MahjongMove>
    {
        public NaiveAI(int p)
        {
            seed = DateTime.Now.Millisecond;
            rand = new Random(seed);
            rc = 0;

            Player = p;
            return;
        }

        protected NaiveAI(int p, int s, uint c)
        {
            seed = s;
            rand = new Random(seed);
            rc = c;

            while (c-- > 0)
                rand.Next();

            Player = p;
            return;
        }

        /// <summary>
        /// Determines the next move for this AI to make.
        /// </summary>
        /// <param name="state">The state of the game. This will be cloned so that the AI does not affect the actual game state.</param>
        /// <returns>Returns the next move to make based on the current game state.</returns>
        public MahjongMove GetNextMove(GameState<MahjongMove> state)
        {
            MahjongGameState s = state as MahjongGameState;
            MahjongAIPlayer ai = state.GetPlayer(Player) as MahjongAIPlayer;
            MahjongPlayer mp = ai as MahjongPlayer;

            // Pass if it's not this AI's turn
            if (s.ActivePlayer != Player)
                return new MahjongMove();

            if (MahjongStaticFunctions.HasMahjong(ai.CardsInHand, mp.Melds))
                return new MahjongMove(true);

            // Pick a random tile to discard
            int r = rand.Next(0, (int)ai.CardsInHand.CardsInHand);
            rc++;

            return new MahjongMove(ai.CardsInHand.Cards[r]);
        }

        /// <summary>
        /// Creates a deep copy of this AIBehavior.
        /// </summary>
        /// <returns>Returns a copy of this AIBehavior.</returns>
        public AIBehavior<MahjongMove> Clone()
        { return new NaiveAI(Player, seed, rc); }

        /// <summary>
        /// Which player this AI is. This is discoverable implicitly, but this is easier.
        /// </summary>
        public int Player
        { get; protected set; }

        protected Random rand;
        protected uint rc;
        protected int seed;
    }

    /// <summary>
    /// An AI for Mahjong that melds pungs and kongs whenever possible and otherwise randomly discards singleton tiles whenever it can't.
    /// </summary>
    public class GreedyAI : AIBehavior<MahjongMove>
    {
        public GreedyAI(int p)
        {
            seed = DateTime.Now.Millisecond;
            rand = new Random(seed);
            rc = 0;

            Player = p;
            return;
        }

        protected GreedyAI(int p, int s, uint c)
        {
            seed = s;
            rand = new Random(seed);
            rc = c;

            while (c-- > 0)
                rand.Next();

            Player = p;
            return;
        }

        /// <summary>
        /// Determines the next move for this AI to make.
        /// </summary>
        /// <param name="state">The state of the game. This will be cloned so that the AI does not affect the actual game state.</param>
        /// <returns>Returns the next move to make based on the current game state.</returns>
        public MahjongMove GetNextMove(GameState<MahjongMove> state)
        {
            MahjongGameState s = state as MahjongGameState;
            MahjongAIPlayer ai = state.GetPlayer(Player) as MahjongAIPlayer;
            MahjongPlayer mp = ai as MahjongPlayer;

            // Pass if it's not this AI's turn and it can't meld a pung or kong
            if (s.ActivePlayer != Player)
                if (s.AvailableTile == null)
                    return new MahjongMove(); // The active player is going out, so there's nothing we can do
                else
                    switch (ai.CardsInHand.CountCard(s.AvailableTile))
                    {
                        case 1:
                            // If this is the last tile we needed to declare mahjong, go for it
                            if (ai.CardsInHand.CardsInHand == 1)
                            {
                                List<Card> eye = new List<Card>();
                                eye.Add(s.AvailableTile.Clone());
                                eye.Add(s.AvailableTile.Clone());

                                return new MahjongMove(new MahjongMeld(eye, false), s.AvailableTile, true);
                            }

                            // Pass otherwise
                            return new MahjongMove();
                        case 2:
                            List<Card> p2 = new List<Card>();
                            p2.Add(s.AvailableTile.Clone());
                            p2.Add(s.AvailableTile.Clone());
                            p2.Add(s.AvailableTile.Clone());

                            Hand htemp2 = new StandardHand(ai.CardsInHand.Cards);
                            htemp2.PlayCard(s.AvailableTile);
                            htemp2.PlayCard(s.AvailableTile);

                            MahjongMeld m2 = new MahjongMeld(p2, false);
                            List<MahjongMeld> melds2 = new List<MahjongMeld>();

                            foreach (MahjongMeld m22 in mp.Melds)
                                melds2.Add(m22.Clone());

                            return new MahjongMove(m2, s.AvailableTile, MahjongStaticFunctions.HasMahjong(htemp2, melds2));
                        case 3:
                            List<Card> p3 = new List<Card>();
                            p3.Add(s.AvailableTile.Clone());
                            p3.Add(s.AvailableTile.Clone());
                            p3.Add(s.AvailableTile.Clone());
                            p3.Add(s.AvailableTile.Clone());

                            Hand htemp3 = new StandardHand(ai.CardsInHand.Cards);
                            htemp3.PlayCard(s.AvailableTile);
                            htemp3.PlayCard(s.AvailableTile);
                            htemp3.PlayCard(s.AvailableTile);

                            MahjongMeld m3 = new MahjongMeld(p3, false);
                            List<MahjongMeld> melds3 = new List<MahjongMeld>();

                            foreach (MahjongMeld m33 in mp.Melds)
                                melds3.Add(m33.Clone());

                            return new MahjongMove(m3, s.AvailableTile, MahjongStaticFunctions.HasMahjong(htemp3, melds3));
                        default:
                            return new MahjongMove(); // If we naturally drew a kong, it'll stick around until we're forced to discard one of the tiles or just lose
                    }

            // If it's our turn, check if we have mahjong
            if (MahjongStaticFunctions.HasMahjong(ai.CardsInHand, mp.Melds))
                return new MahjongMove(true);

            // Find all the singletons
            List<int> singletons = new List<int>();

            for (int i = 0; i < ai.CardsInHand.CardsInHand; i++)
                if (ai.CardsInHand.CountCard(ai.CardsInHand.Cards[i]) == 1)
                    singletons.Add(i);

            // If we don't have a singleton tile, we'll have to part with another
            if (singletons.Count == 0)
            {
                singletons.Add(rand.Next(0, (int)ai.CardsInHand.CardsInHand));
                rc++;
            }

            // Pick a random singleton tile to discard
            int r = rand.Next(0, singletons.Count);
            rc++;

            return new MahjongMove(ai.CardsInHand.Cards[r]);
        }

        /// <summary>
        /// Creates a deep copy of this AIBehavior.
        /// </summary>
        /// <returns>Returns a copy of this AIBehavior.</returns>
        public AIBehavior<MahjongMove> Clone()
        { return new GreedyAI(Player, seed, rc); }

        /// <summary>
        /// Which player this AI is. This is discoverable implicitly, but this is easier.
        /// </summary>
        public int Player
        { get; protected set; }

        protected Random rand;
        protected uint rc;
        protected int seed;
    }

	/// <summary>
	/// An AI for Mahjong that uses the minimax algorithm to compute its choice of move.
	/// </summary>
	public class MiniMaxAI : AIBehavior<MahjongMove>
	{
		public MiniMaxAI(int p, int depth)
        {
            seed = DateTime.Now.Millisecond;
            rand = new Random(seed);
            rc = 0;

            Player = p;
			Depth = depth;

            return;
        }

        protected MiniMaxAI(int p, int depth, int s, uint c)
        {
            seed = s;
            rand = new Random(seed);
            rc = c;

            while (c-- > 0)
                rand.Next();

            Player = p;
			Depth = depth;

            return;
        }

		static MiniMaxAI()
		{
			string[] value_names = {"One","Two","Three","Four","Five","Six","Seven","Eight","Nine"};

			Bamboo = new List<Card>();
			Characters = new List<Card>();
			Dots = new List<Card>();

			for(int i = 1;i < 10;i++)
			{
				Bamboo.Add(new StandardCard(new SimpleSuit(SuitIdentifier.BAMBOO),new ValueN(i,value_names[i-1])));
				Characters.Add(new StandardCard(new SimpleSuit(SuitIdentifier.CHARACTER),new ValueN(i,value_names[i-1])));
				Dots.Add(new StandardCard(new SimpleSuit(SuitIdentifier.DOT),new ValueN(i,value_names[i-1])));
			}
			
			return;
		}

        /// <summary>
        /// Determines the next move for this AI to make.
        /// </summary>
        /// <param name="state">The state of the game. This will be cloned so that the AI does not affect the actual game state.</param>
        /// <returns>Returns the next move to make based on the current game state.</returns>
        public MahjongMove GetNextMove(GameState<MahjongMove> state)
        {
			MahjongGameState ms = state as MahjongGameState;
			return AlphaBetaPruner<MahjongGameState,MahjongMove>.Search(ms,Depth,StateGoodness(ms),GetMoveEnumerator,StateUpdater,Maximising);
		}

		// This is not a great evaluation function, but I'm almost out of time to program it, so meh
		private Engine.Player.AI.ABP.StateEvaluator<MahjongGameState> StateGoodness(MahjongGameState original)
		{
			return state =>
			{
				// If the hand is finished, just check the difference in score
				// A low difference is lame, so if it gets overpowered by other moves that might lead to lesser outcomes, so be it
				if(state.HandFinished)
					return 100 * ((state.GetPlayer(Player) as MahjongPlayer).Score - (original.GetPlayer(Player) as MahjongPlayer).Score);

				int ret = 0;

				// Melds are usually good to get, and some are better than others
				foreach(MahjongMeld m in (state.GetPlayer(Player) as MahjongPlayer).Melds)
					if(m.Chow)
						ret++;
					else if(m.Pung)
						ret += 3;
					else if(m.Kong)
						ret += 6;

				Player<MahjongMove> p = state.GetPlayer(Player);

				// Concealed melds are even better
				foreach(Card c in p.CardsInHand.Cards)
				{
					int count = p.CardsInHand.CountCard(c);

					if(count == 3)
						ret += 3; // This will count three times
					else if(count == 4)
						ret += 5; // This will count four times
					else
					{
						ret += GetChows(p.CardsInHand,c).Count; // Being able to form more than one chow with a tile is good strategy; each tile will get counted thirce per chow it's in

						if(count == 2)
							ret++; // Having eyes available is also good
					}
				}
				
				return ret;
			};
		}

		private bool Maximising(MahjongGameState state)
		{return state.SubActivePlayer == Player;}

		private MahjongGameState StateUpdater(MahjongGameState state, MahjongMove m)
		{
			MahjongGameState s = state.Clone() as MahjongGameState;
			s.ApplyMove(m);

			return s;
		}

		/// <summary>
        /// Creates a deep copy of this AIBehavior.
        /// </summary>
        /// <returns>Returns a copy of this AIBehavior.</returns>
        public AIBehavior<MahjongMove> Clone()
        {return new MiniMaxAI(Player,Depth,seed,rc);}

		/// <summary>
		/// Returns an enumerator of the possible moves at the given state.
		/// </summary>
		/// <param name="state">The state to enumerate the moves of.</param>
		private IEnumerator<MahjongMove> GetMoveEnumerator(MahjongGameState state)
		{
			List<MahjongMove> moves = new List<MahjongMove>();

			// If the hand is done (or the game), then there's nothing left to enumerate.
			if(state.HandFinished || state.GameFinished)
				return moves.GetEnumerator();

			// Get the player we're considering
			MahjongAIPlayer ai = state.GetPlayer(state.SubActivePlayer) as MahjongAIPlayer;
			MahjongPlayer mp = ai as MahjongPlayer;

			// The active player has different choices available than the responding players
			if(state.ActivePlayer == state.SubActivePlayer)
			{
				// Add the kongs first
				foreach(Card c in ai.CardsInHand.Cards)
					if(!ContainsKong(moves,c) && ai.CardsInHand.CountCard(c) == 4)
					{
						List<Card> meld = new List<Card>();

						for(int i = 0;i < 4;i++)
							meld.Add(c.Clone());

						moves.Add(new MahjongMove(new MahjongMeld(meld,true),c)); // We're melding a kong, so mahjong is definitely false here
					}

				// We can also meld a fourth tile into an existing pung
				foreach(MahjongMeld meld in mp.Melds)
					if(meld.Pung && ai.CardsInHand.Cards.Contains(meld.Cards[0]))
					{
						List<Card> n_meld = new List<Card>();

						for(int i = 0;i < 4;i++)
							n_meld.Add(meld.Cards[0].Clone());

						moves.Add(new MahjongMove(new MahjongMeld(n_meld,false),meld.Cards[0])); // We're melding a kong, so mahjong is definitely false here
					}

				// Add all of the discards next
				foreach(Card c in ai.CardsInHand.Cards)
					moves.Add(new MahjongMove(c));

				// Lastly, we'll add the ability to declare mahjong
				if(MahjongStaticFunctions.HasMahjong(ai.CardsInHand,mp.Melds))
					moves.Add(new MahjongMove(true));
			}
			else
			{
				// We need a tile available to do anything but pass (there are a few cases were this would not be available)
				if(state.AvailableTile != null)
				{
					// First, add chow moves if we can
					if(state.SubActivePlayer == state.NextPlayer)
					{
						List<MahjongMeld> chows = GetChows(ai.CardsInHand,state.AvailableTile);

						foreach(MahjongMeld m in chows)
						{
							Hand h; List<MahjongMeld> melds;
							CreateDuplicateMinusMeld(ai.CardsInHand,mp.Melds,m,state.AvailableTile,out h,out melds);
							
							moves.Add(new MahjongMove(m,state.AvailableTile,MahjongStaticFunctions.HasMahjong(h,melds)));
						}
					}
					
					// Add the ability to meld pungs and kongs
					int count = ai.CardsInHand.CountCard(state.AvailableTile); // We'll use this again after pung/kong checks, so this is extra fine to keep around

					if(count > 1)
					{
						// First, pungs are always possible
						List<Card> cards = new List<Card>();

						cards.Add(state.AvailableTile.Clone());
						cards.Add(state.AvailableTile.Clone());
						cards.Add(state.AvailableTile.Clone());

						MahjongMeld m = new MahjongMeld(cards);

						Hand h; List<MahjongMeld> melds;
						CreateDuplicateMinusMeld(ai.CardsInHand,mp.Melds,m,state.AvailableTile,out h,out melds);

						moves.Add(new MahjongMove(m,state.AvailableTile,MahjongStaticFunctions.HasMahjong(h,melds)));

						// Now create a kong move if possible
						if(count > 2)
						{
							cards.Add(state.AvailableTile.Clone());

							m = new MahjongMeld(cards);
							CreateDuplicateMinusMeld(ai.CardsInHand,mp.Melds,m,state.AvailableTile,out h,out melds);

							moves.Add(new MahjongMove(m,state.AvailableTile,MahjongStaticFunctions.HasMahjong(h,melds)));
						}
					}

					// Now add the ability to declare mahjong through non-standard means
					// Nine gates can only be won on a self pick, so let's deal with thirteen orphans and seven pair first
					Hand htemp = new StandardHand(ai.CardsInHand.Cards);
					htemp.DrawCard(state.AvailableTile.Clone());

					MahjongMeld mtemp = new MahjongMeld(htemp.Cards);

					if(mtemp.SevenPair || mtemp.ThirteenOrphans)
						moves.Add(new MahjongMove(mtemp,state.AvailableTile,true));

					// The last mahjong we need to check now is if we can form an eye and win
					if(count > 0)
					{
						List<Card> cards = new List<Card>();

						cards.Add(state.AvailableTile.Clone());
						cards.Add(state.AvailableTile.Clone());

						MahjongMeld m = new MahjongMeld(cards);
						
						Hand h; List<MahjongMeld> melds;
						CreateDuplicateMinusMeld(ai.CardsInHand,mp.Melds,m,state.AvailableTile,out h,out melds);

						if(MahjongStaticFunctions.HasMahjong(h,melds))
							moves.Add(new MahjongMove(m,state.AvailableTile,true));
					}
				}

				// Lastly, add the simple pass option
				moves.Add(new MahjongMove());
			}

			return moves.GetEnumerator();
		}

		/// <summary>
		/// Copies the data in hand and melds into new objects assigned to h and m. It then adds meld to m, add to h, and discards from h every tile in meld.
		/// Add can be null.
		/// </summary>
		private void CreateDuplicateMinusMeld(Hand hand, List<MahjongMeld> melds, MahjongMeld meld, Card add, out Hand h, out List<MahjongMeld> m)
		{
			MahjongStaticFunctions.CopyData(hand,melds,out h,out m);
			m.Add(meld);
			
			if(add != null)
				h.DrawCard(add.Clone());

			foreach(Card c in meld.Cards)
				h.PlayCard(c);

			return;
		}

		/// <summary>
		/// Gets a list of all the chows a hand can form with the given card.
		/// </summary>
		private List<MahjongMeld> GetChows(Hand h, Card c)
		{
			List<MahjongMeld> ret = new List<MahjongMeld>();

			// If we don't have a simple tile, we can't form a chow
			if(c.Suit.Color != SuitColor.SIMPLE)
				return ret;

			// Get the list of simples to work with
			List<Card> l = null;

			switch(c.Suit.ID)
			{
			case SuitIdentifier.BAMBOO:
				l = Bamboo;
				break;
			case SuitIdentifier.CHARACTER:
				l = Characters;
				break;
			case SuitIdentifier.DOT:
				l = Dots;
				break;
			}

			// Find out what simple we have
			int index = 0;

			for(;index < l.Count;index++)
				if(l[index].Equals(c))
					break;

			// Now that we have the right index, form the possible chows
			// a b c
			if(index > 1 && h.Cards.Contains(l[index - 2]) && h.Cards.Contains(l[index - 1]))
			{
				List<Card> l2 = new List<Card>();

				l2.Add(l[index - 2].Clone());
				l2.Add(l[index - 1].Clone());
				l2.Add(c.Clone());

				ret.Add(new MahjongMeld(l2));
			}

			// a c b
			if(index > 0 && index < l.Count - 1 && h.Cards.Contains(l[index - 1]) && h.Cards.Contains(l[index + 1]))
			{
				List<Card> l2 = new List<Card>();

				l2.Add(l[index - 1].Clone());
				l2.Add(c.Clone());
				l2.Add(l[index + 1].Clone());

				ret.Add(new MahjongMeld(l2));
			}

			// c a b
			if(index < l.Count - 2 && h.Cards.Contains(l[index + 1]) && h.Cards.Contains(l[index + 2]))
			{
				List<Card> l2 = new List<Card>();
				
				l2.Add(c.Clone());
				l2.Add(l[index + 1].Clone());
				l2.Add(l[index + 2].Clone());

				ret.Add(new MahjongMeld(l2));
			}

			return ret;
		}
		
		private bool ApproxEqual(double d1, double d2, double epsilon = 0.0001)
		{return Math.Abs(d1 - d2) < epsilon;}

		/// <summary>
		/// Determines if a kong of the given card is already in the moveset.
		/// </summary>
		private bool ContainsKong(List<MahjongMove> moves, Card c)
		{
			foreach(MahjongMove m in moves)
				if(IsSameKong(m,c))
					return true;

			return false;
		}

		/// <summary>
		/// Determines if a card is in the move given and the move is a kong.
		/// </summary>
		private bool IsSameKong(MahjongMove m, Card c)
		{return m.Kong && c.Equals(m.MeldTiles.Cards[0]);}

        /// <summary>
        /// Which player this AI is. This is discoverable implicitly, but this is easier.
        /// </summary>
        public int Player
        {get; protected set;}

		/// <summary>
		/// The number of search depth available move.
		/// </summary>
		public int Depth
		{get; protected set;}

        protected Random rand;
        protected uint rc;
        protected int seed;
		
		private static List<Card> Bamboo;
		private static List<Card> Characters;
		private static List<Card> Dots;
	}

	/// <summary>
	/// An AI for Mahjong that uses Monte Carlo simulations to compute its choice of move.
	/// </summary>
    public class MonteCarloAI : AIBehavior<MahjongMove>
    {
        public MonteCarloAI(int p, int samples)
        {
            seed = DateTime.Now.Millisecond;
            rand = new Random(seed);
            rc = 0;

            Player = p;
			SamplesPerMove = samples;

            return;
        }

        protected MonteCarloAI(int p, int samples, int s, uint c)
        {
            seed = s;
            rand = new Random(seed);
            rc = c;

            while (c-- > 0)
                rand.Next();

            Player = p;
			SamplesPerMove = samples;

            return;
        }

		static MonteCarloAI()
		{
			string[] value_names = {"One","Two","Three","Four","Five","Six","Seven","Eight","Nine"};

			Bamboo = new List<Card>();
			Characters = new List<Card>();
			Dots = new List<Card>();

			for(int i = 1;i < 10;i++)
			{
				Bamboo.Add(new StandardCard(new SimpleSuit(SuitIdentifier.BAMBOO),new ValueN(i,value_names[i-1])));
				Characters.Add(new StandardCard(new SimpleSuit(SuitIdentifier.CHARACTER),new ValueN(i,value_names[i-1])));
				Dots.Add(new StandardCard(new SimpleSuit(SuitIdentifier.DOT),new ValueN(i,value_names[i-1])));
			}
			
			return;
		}

        /// <summary>
        /// Determines the next move for this AI to make.
        /// </summary>
        /// <param name="state">The state of the game. This will be cloned so that the AI does not affect the actual game state.</param>
        /// <returns>Returns the next move to make based on the current game state.</returns>
        public MahjongMove GetNextMove(GameState<MahjongMove> state)
        {
			MahjongGameState ms = state as MahjongGameState;
			return UniformMonteCarloSearch<MahjongGameState,MahjongMove>.Search(ms,SamplesPerMove,CloneState,GetMoveEnumerator,StateUpdater,ScoreDiff((ms.GetPlayer(Player) as MahjongPlayer).Score));
		}

		private MahjongGameState CloneState(MahjongGameState state)
		{return state.Clone() as MahjongGameState;}

		private MahjongGameState StateUpdater(MahjongGameState state, MahjongMove m)
		{
			state.ApplyMove(m);
			return state;
		}

		private Engine.Player.AI.MonteCarlo.StateEvaluator<MahjongGameState> ScoreDiff(int original)
		{return s => (s.GetPlayer(Player) as MahjongPlayer).Score - original;}

        /// <summary>
        /// Creates a deep copy of this AIBehavior.
        /// </summary>
        /// <returns>Returns a copy of this AIBehavior.</returns>
        public AIBehavior<MahjongMove> Clone()
        {return new MonteCarloAI(Player,SamplesPerMove,seed,rc);}

		/// <summary>
		/// Returns an enumerator of the possible moves at the given state.
		/// </summary>
		/// <param name="state">The state to enumerate the moves of.</param>
		private IEnumerator<MahjongMove> GetMoveEnumerator(MahjongGameState state)
		{
			List<MahjongMove> moves = new List<MahjongMove>();

			// If the hand is done (or the game), then there's nothing left to enumerate.
			if(state.HandFinished || state.GameFinished)
				return moves.GetEnumerator();

			// Get the player we're considering
			MahjongAIPlayer ai = state.GetPlayer(state.SubActivePlayer) as MahjongAIPlayer;
			MahjongPlayer mp = ai as MahjongPlayer;

			// The active player has different choices available than the responding players
			if(state.ActivePlayer == state.SubActivePlayer)
			{
				// Add the kongs first
				foreach(Card c in ai.CardsInHand.Cards)
					if(!ContainsKong(moves,c) && ai.CardsInHand.CountCard(c) == 4)
					{
						List<Card> meld = new List<Card>();

						for(int i = 0;i < 4;i++)
							meld.Add(c.Clone());

						moves.Add(new MahjongMove(new MahjongMeld(meld,true),c)); // We're melding a kong, so mahjong is definitely false here
					}

				// We can also meld a fourth tile into an existing pung
				foreach(MahjongMeld meld in mp.Melds)
					if(meld.Pung && ai.CardsInHand.Cards.Contains(meld.Cards[0]))
					{
						List<Card> n_meld = new List<Card>();

						for(int i = 0;i < 4;i++)
							n_meld.Add(meld.Cards[0].Clone());

						moves.Add(new MahjongMove(new MahjongMeld(n_meld,false),meld.Cards[0])); // We're melding a kong, so mahjong is definitely false here
					}

				// Add all of the discards next
				foreach(Card c in ai.CardsInHand.Cards)
					moves.Add(new MahjongMove(c));

				// Lastly, we'll add the ability to declare mahjong
				if(MahjongStaticFunctions.HasMahjong(ai.CardsInHand,mp.Melds))
					moves.Add(new MahjongMove(true));
			}
			else
			{
				// We need a tile available to do anything but pass (there are a few cases were this would not be available)
				if(state.AvailableTile != null)
				{
					// First, add chow moves if we can
					if(state.SubActivePlayer == state.NextPlayer)
					{
						List<MahjongMeld> chows = GetChows(ai.CardsInHand,state.AvailableTile);

						foreach(MahjongMeld m in chows)
						{
							Hand h; List<MahjongMeld> melds;
							CreateDuplicateMinusMeld(ai.CardsInHand,mp.Melds,m,state.AvailableTile,out h,out melds);
							
							moves.Add(new MahjongMove(m,state.AvailableTile,MahjongStaticFunctions.HasMahjong(h,melds)));
						}
					}
					
					// Add the ability to meld pungs and kongs
					int count = ai.CardsInHand.CountCard(state.AvailableTile); // We'll use this again after pung/kong checks, so this is extra fine to keep around

					if(count > 1)
					{
						// First, pungs are always possible
						List<Card> cards = new List<Card>();

						cards.Add(state.AvailableTile.Clone());
						cards.Add(state.AvailableTile.Clone());
						cards.Add(state.AvailableTile.Clone());

						MahjongMeld m = new MahjongMeld(cards);

						Hand h; List<MahjongMeld> melds;
						CreateDuplicateMinusMeld(ai.CardsInHand,mp.Melds,m,state.AvailableTile,out h,out melds);

						moves.Add(new MahjongMove(m,state.AvailableTile,MahjongStaticFunctions.HasMahjong(h,melds)));

						// Now create a kong move if possible
						if(count > 2)
						{
							cards.Add(state.AvailableTile.Clone());

							m = new MahjongMeld(cards);
							CreateDuplicateMinusMeld(ai.CardsInHand,mp.Melds,m,state.AvailableTile,out h,out melds);

							moves.Add(new MahjongMove(m,state.AvailableTile,MahjongStaticFunctions.HasMahjong(h,melds)));
						}
					}

					// Now add the ability to declare mahjong through non-standard means
					// Nine gates can only be won on a self pick, so let's deal with thirteen orphans and seven pair first
					Hand htemp = new StandardHand(ai.CardsInHand.Cards);
					htemp.DrawCard(state.AvailableTile.Clone());

					MahjongMeld mtemp = new MahjongMeld(htemp.Cards);

					if(mtemp.SevenPair || mtemp.ThirteenOrphans)
						moves.Add(new MahjongMove(mtemp,state.AvailableTile,true));

					// The last mahjong we need to check now is if we can form an eye and win
					if(count > 0)
					{
						List<Card> cards = new List<Card>();

						cards.Add(state.AvailableTile.Clone());
						cards.Add(state.AvailableTile.Clone());

						MahjongMeld m = new MahjongMeld(cards);
						
						Hand h; List<MahjongMeld> melds;
						CreateDuplicateMinusMeld(ai.CardsInHand,mp.Melds,m,state.AvailableTile,out h,out melds);

						if(MahjongStaticFunctions.HasMahjong(h,melds))
							moves.Add(new MahjongMove(m,state.AvailableTile,true));
					}
				}

				// Lastly, add the simple pass option
				moves.Add(new MahjongMove());
			}

			return moves.GetEnumerator();
		}

		/// <summary>
		/// Copies the data in hand and melds into new objects assigned to h and m. It then adds meld to m, add to h, and discards from h every tile in meld.
		/// Add can be null.
		/// </summary>
		private void CreateDuplicateMinusMeld(Hand hand, List<MahjongMeld> melds, MahjongMeld meld, Card add, out Hand h, out List<MahjongMeld> m)
		{
			MahjongStaticFunctions.CopyData(hand,melds,out h,out m);
			m.Add(meld);
			
			if(add != null)
				h.DrawCard(add.Clone());

			foreach(Card c in meld.Cards)
				h.PlayCard(c);

			return;
		}

		/// <summary>
		/// Gets a list of all the chows a hand can form with the given card.
		/// </summary>
		private List<MahjongMeld> GetChows(Hand h, Card c)
		{
			List<MahjongMeld> ret = new List<MahjongMeld>();

			// If we don't have a simple tile, we can't form a chow
			if(c.Suit.Color != SuitColor.SIMPLE)
				return ret;

			// Get the list of simples to work with
			List<Card> l = null;

			switch(c.Suit.ID)
			{
			case SuitIdentifier.BAMBOO:
				l = Bamboo;
				break;
			case SuitIdentifier.CHARACTER:
				l = Characters;
				break;
			case SuitIdentifier.DOT:
				l = Dots;
				break;
			}

			// Find out what simple we have
			int index = 0;

			for(;index < l.Count;index++)
				if(l[index].Equals(c))
					break;

			// Now that we have the right index, form the possible chows
			// a b c
			if(index > 1 && h.Cards.Contains(l[index - 2]) && h.Cards.Contains(l[index - 1]))
			{
				List<Card> l2 = new List<Card>();

				l2.Add(l[index - 2].Clone());
				l2.Add(l[index - 1].Clone());
				l2.Add(c.Clone());

				ret.Add(new MahjongMeld(l2));
			}

			// a c b
			if(index > 0 && index < l.Count - 1 && h.Cards.Contains(l[index - 1]) && h.Cards.Contains(l[index + 1]))
			{
				List<Card> l2 = new List<Card>();

				l2.Add(l[index - 1].Clone());
				l2.Add(c.Clone());
				l2.Add(l[index + 1].Clone());

				ret.Add(new MahjongMeld(l2));
			}

			// c a b
			if(index < l.Count - 2 && h.Cards.Contains(l[index + 1]) && h.Cards.Contains(l[index + 2]))
			{
				List<Card> l2 = new List<Card>();
				
				l2.Add(c.Clone());
				l2.Add(l[index + 1].Clone());
				l2.Add(l[index + 2].Clone());

				ret.Add(new MahjongMeld(l2));
			}

			return ret;
		}
		
		private bool ApproxEqual(double d1, double d2, double epsilon = 0.0001)
		{return Math.Abs(d1 - d2) < epsilon;}

		/// <summary>
		/// Determines if a kong of the given card is already in the moveset.
		/// </summary>
		private bool ContainsKong(List<MahjongMove> moves, Card c)
		{
			foreach(MahjongMove m in moves)
				if(IsSameKong(m,c))
					return true;

			return false;
		}

		/// <summary>
		/// Determines if a card is in the move given and the move is a kong.
		/// </summary>
		private bool IsSameKong(MahjongMove m, Card c)
		{return m.Kong && c.Equals(m.MeldTiles.Cards[0]);}

        /// <summary>
        /// Which player this AI is. This is discoverable implicitly, but this is easier.
        /// </summary>
        public int Player
        {get; protected set;}

		/// <summary>
		/// The number of terminal states examined per available move.
		/// </summary>
		public int SamplesPerMove
		{get; protected set;}

        protected Random rand;
        protected uint rc;
        protected int seed;
		
		private static List<Card> Bamboo;
		private static List<Card> Characters;
		private static List<Card> Dots;
    }
}
