using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Cards;
using Engine.Cards.CardTypes;
using Engine.Cards.CardTypes.Suits.Mahjong;
using Engine.Cards.CardTypes.Values;
using Engine.Cards.Hands;
using Engine.Game;
using Engine.Player.AI;

namespace Mahjong
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create the game
            MahjongGameState state = new MahjongGameState();

            // Add the AI
            for (int i = 0; i < 4; i++)
                /*if(i % 2 == 1)
					state.PlayerLeft(i,new NaiveAI(i));
				else*/
                state.PlayerLeft(i, new GreedyAI(i));

            // Run the game to completion
            while (!state.GameFinished)
            {
                MahjongAIPlayer ai = state.GetPlayer(state.SubActivePlayer) as MahjongAIPlayer;
                MahjongMove move = ai.AI.GetNextMove(state);

                if (!state.ApplyMove(move))
                    Console.WriteLine("An invalid move has been provided.");

                if (state.HandFinished && VERBOSE)
                    Print(state);
            }

            // Output the final scores
            Print(state);

            return;
        }

        private static void Print(MahjongGameState state)
        {
            Console.WriteLine("Player One Score: " + (state.GetPlayer(0) as MahjongPlayer).Score);
            Console.WriteLine("Player Two Score: " + (state.GetPlayer(1) as MahjongPlayer).Score);
            Console.WriteLine("Player Three Score: " + (state.GetPlayer(2) as MahjongPlayer).Score);
            Console.WriteLine("Player Four Score: " + (state.GetPlayer(3) as MahjongPlayer).Score);
            Console.WriteLine();

            return;
        }

        private static readonly bool VERBOSE = true;
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

        public NaiveAI(int p, int s, uint c)
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

        public GreedyAI(int p, int s, uint c)
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

    public class MonteCarloAI : AIBehavior<MahjongMove>
    {
        public MonteCarloAI(int p)
        {
            seed = DateTime.Now.Millisecond;
            rand = new Random(seed);
            rc = 0;

            Player = p;
            return;
        }

        protected MonteCarloAI(int p, int s, uint c)
        {
            seed = s;
            rand = new Random(seed);
            rc = c;

            while (c-- > 0)
                rand.Next();

            Player = p;
            return;
        }

        public AIBehavior<MahjongMove> Clone()
        { return new MonteCarloAI(Player, seed, rc); }

        public MahjongMove GetNextMove(GameState<MahjongMove> state)
        {
            MahjongGameState s = state as MahjongGameState;
            MahjongAIPlayer ai = state.GetPlayer(Player) as MahjongAIPlayer;
            MahjongPlayer mp = ai as MahjongPlayer;

            Moves possibleMoves = new Moves(s);
            foreach (MahjongMove move in possibleMoves)
            {
                for (int x = 0; x < 200; x++)
                { // simulate x many hands
                    MahjongGameState sim_state = s.Clone() as MahjongGameState;
                    while (!sim_state.HandFinished)
                    {
                        MahjongMove moveToTake = (new Moves(sim_state)).GetRandomMove();
                        if (!s.ApplyMove(moveToTake))
                            Console.WriteLine("An invalid move has been provided.");
                    }
                    // After each simulated game finishes, save Player's final score
                    // scoresToAverage <-- (sim_state.GetPlayer(Player) as MahjongPlayer).Score;
                }
            }

            return /* argmax avgScores */;
        }

        public int Player
        { get; protected set; }
        protected Random rand;
        protected uint rc;
        protected int seed;
    }

    public class Moves : IEnumerable<MahjongMove>
    {
        private List<MahjongMove> _moves;
        public Moves(MahjongGameState state)
        {
            MahjongAIPlayer currentAi = state.GetPlayer(state.SubActivePlayer) as MahjongAIPlayer;
            MahjongPlayer currentMp = currentAi as MahjongPlayer;

            if (state.SubActivePlayer == state.ActivePlayer)
            {
                // add kong move
                // add discard move

                // add go-out move:
                if (MahjongStaticFunctions.HasMahjong(currentAi.CardsInHand, currentMp.Melds))
                    _moves.Add(new MahjongMove(true));
            }
            else
            {
                if (state.SubActivePlayer == state.NextPlayer)
                {
                    // add chaw move
                }
                // add pung move
                // add kong move
                // add pass move

                // add go-out move:
                if (MahjongStaticFunctions.HasMahjong(currentAi.CardsInHand, currentMp.Melds))
                    _moves.Add(new MahjongMove(true));
            }
        }

        // Implementation for the GetEnumerator method.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator<MahjongMove>)GetEnumerator();
        }

        private IEnumerator<MahjongMove> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public MahjongMove GetRandomMove()
        {
            Random rnd = new Random();
            int r = rnd.Next(_moves.Count);
            return _moves[r];
        }

        IEnumerator<MahjongMove> IEnumerable<MahjongMove>.GetEnumerator()
        {
            return new MovesEnum(_moves);
        }
    }

    public class MovesEnum : IEnumerator<MahjongMove>
    {
        public List<MahjongMove> _moves;

        int position = -1;

        public MovesEnum(List<MahjongMove> list)
        {
            _moves = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < _moves.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public MahjongMove Current
        {
            get
            {
                try
                {
                    return _moves[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
