using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCardgame
{
    class Player
    {
        private Deck        _deck;
        private int         _id;
        private teamColor   _team;
        private bool        _belotte;

        public enum teamColor
        {
            BLEUE,
            ROUGE,
            NO_COLOR
        }

        public Player(int id, teamColor team)
        {
            _id = id;
            _team = team;
            _deck = new Deck();
            _belotte = false;
        }

        public int getId()
        {
            return (_id);
        }

        public Deck getDeck()
        {
            return (_deck);
        }

        public void addMultipleCartsToDeck(List<Cart> newDeck)
        {
            _deck.addMultipleCarts(newDeck);
        }

        public teamColor getTeamColor()
        {
            return (_team);
        }

        public void removeOneCart(Cart cart)
        {
            _deck.removeOneCart(cart);
        }

        public bool deckIsEmpty()
        {
            return (_deck.isEmpty());
        }

        public bool getBelotte()
        {
            return (_belotte);
        }

        public void setBelotte(bool belotte)
        {
            _belotte = belotte;
        }
    }
}
