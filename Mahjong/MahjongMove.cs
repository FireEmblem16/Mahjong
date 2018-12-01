using System.Collections.Generic;
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
		/// Creates a new move with negative values on every property.
		/// This corresponds to a pass.
		/// </summary>
		public MahjongMove()
		{
			DiscardedTile = null;
			MeldTiles = null;

			Discard = false;
			Mahjong = false;

			return;
		}

		/// <summary>
		/// Creates a new discard move.
		/// </summary>
		/// <param name="discard">The tile being discarded.</param>
		public MahjongMove(Card discard)
		{
			DiscardedTile = discard.Clone();
			MeldTiles = null;

			Discard = true;
			Mahjong = false;

			return;
		}

		/// <summary>
		/// Creates a new melding move.
		/// </summary>
		/// <param name="meld">The meld being attempted.</param>
		/// <param name="new_tile">The tile taken from the discard, robbed from a kong, or a representative of a melded concealed kong.</param>
		/// <param name="mahjong">If true, this meld is to go out on.</param>
		public MahjongMove(MahjongMeld meld, Card new_tile, bool mahjong = false)
		{
			DiscardedTile = new_tile.Clone();
			MeldTiles = meld.Clone();

			Discard = false;
			Mahjong = mahjong;

			return;
		}

		/// <summary>
		/// Clones this move.
		/// </summary>
		public MahjongMove Clone()
		{
			MahjongMove ret = new MahjongMove();

			ret.DiscardedTile = DiscardedTile == null ? null : DiscardedTile.Clone();
			ret.MeldTiles = MeldTiles == null ? null : MeldTiles.Clone();
			
			ret.Discard = Discard;
			ret.Mahjong = Mahjong;

			return null;
		}

		/// <summary>
		/// True is this is a discarding move as opposed to a meld.
		/// </summary>
		public bool Discard
		{get; set;}
		
		/// <summary>
		/// The tile being discarded, added to a pung to form a kong, stolen from the discard, or a representative of a melded concealed kong.
		/// </summary>
		public Card DiscardedTile
		{get; set;}

		/// <summary>
		/// True if this move represents a pass on the current tile.
		/// </summary>
		public bool Pass
		{
			get
			{return !Meld && !Discard && !Mahjong;} // You can go out on your own draw, so we need to check that we're also not declaring Mahjong
		}

		/// <summary>
		/// True if this move represents an attempt to meld.
		/// This is the case if the player wants to steal a tile, meld a concealed kong, or if they want to turn a pung into a kong with a tile drawn from the wall.
		/// Concealed melding and pung->kong from the wall does need to pass through regular moves as robbing a kong is a thing.
		/// </summary>
		public bool Meld
		{
			get
			{return MeldTiles != null && MeldTiles.Valid;}
		}

		/// <summary>
		/// True if this move represents an attempt to meld from the hand.
		/// Note that if this is true, Meld must also be true.
		/// </summary>
		public bool ConcealedMeld
		{
			get
			{return Meld && MeldTiles.Concealed;}
		}
		
		/// <summary>
		/// The intended meld to be created or null if no meld is intended.
		/// </summary>
		public MahjongMeld MeldTiles
		{get; set;}

		/// <summary>
		/// True if this move represents an attempt to make a chow on the current tile.
		/// </summary>
		public bool Chow
		{
			get
			{return Meld && MeldTiles.Chow;}
		}

		/// <summary>
		/// True if this move represents an attempt to make a pung on the current tile.
		/// </summary>
		public bool Pung
		{
			get
			{return Meld && MeldTiles.Pung;}
		}

		/// <summary>
		/// True if this move represents an attempt to make a kong on the current tile.
		/// </summary>
		public bool Kong
		{
			get
			{return Meld && MeldTiles.Kong;}
		}

		/// <summary>
		/// True if this move represents an attempt to declare Mahjong on the current tile.
		/// </summary>
		public bool Mahjong
		{get; set;}
	}
}
