using Engine.Cards;
using Engine.Player;

namespace Mahjong
{
	/// <summary>
	/// Contains information about one player's course of action during a trick of Mahjong.
	/// Four of these collectively constitute a single trick and all need to be collected before play advances.
	/// </summary>
	public class MahjongMove
	{
		/// <summary>
		/// Creates a new move with default values on every property.
		/// </summary>
		public MahjongMove()
		{
			Tile = null;

			Meld = false;
			ConcealedMeld = false;
			Discard = false;
			Pass = false;
			Chow = false;
			Pung = false;
			Kong = false;
			Mahjong = false;

			return;
		}

		/// <summary>
		/// Clones this move.
		/// </summary>
		public MahjongMove Clone()
		{
			MahjongMove ret = new MahjongMove();

			ret.Tile = Tile.Clone();

			ret.Meld = Meld;
			ret.ConcealedMeld = ConcealedMeld;
			ret.Discard = Discard;
			ret.Pass = Pass;
			ret.Chow = Chow;
			ret.Pung = Pung;
			ret.Kong = Kong;
			ret.Mahjong = Mahjong;

			return null;
		}
		
		/// <summary>
		/// The tile being discarded, added to a pung to form a kong, or a representative of a melded concealed kong.
		/// This is null if this is not a move of the active player.
		/// </summary>
		public Card Tile
		{get; set;}

		/// <summary>
		/// True if this move represents an attempt to meld.
		/// This is the case if the player wants to steal a tile, meld a concealed kong, or if they want to turn a pung into a kong with a tile drawn from the wall.
		/// Concealed melding and pung->kong from the wall does need to pass through regular moves as robbing a kong is a thing.
		/// </summary>
		public bool Meld
		{get; set;}

		/// <summary>
		/// True if this move represents an attempt to meld from the hand.
		/// </summary>
		public bool ConcealedMeld
		{get; set;}

		/// <summary>
		/// True is this is a discarding move as opposed to a meld.
		/// </summary>
		public bool Discard
		{get; set;}

		/// <summary>
		/// True if this move represents a pass on the current tile.
		/// </summary>
		public bool Pass
		{get; set;}

		/// <summary>
		/// True if this move represents an attempt to make a chow on the current tile.
		/// </summary>
		public bool Chow
		{get; set;}

		/// <summary>
		/// True if this move represents an attempt to make a pung on the current tile.
		/// </summary>
		public bool Pung
		{get; set;}

		/// <summary>
		/// True if this move represents an attempt to make a kong on the current tile.
		/// </summary>
		public bool Kong
		{get; set;}

		/// <summary>
		/// True if this move represents an attempt to declare Mahjong on the current tile.
		/// </summary>
		public bool Mahjong
		{get; set;}
	}
}
