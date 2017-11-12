using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCardgame
{
    class Rules
    {
        private Deck            _plisDeck;
        private List<Player>    _players;
        private Cart            _cartChoosen;
        private Cart            _firstCartPlis;
        private Cart.cartColor  _atoutColor;
        private Player          _currentPlayer;
        private Server          _server;

        public Rules(Deck plisDeck, List<Player> players, Server server)
        {
            _plisDeck = plisDeck;
            _players = players;
            _server = server;
        }

        private Cart getFirstCart()
        {
            if (_plisDeck.isEmpty())
                return (null);
            return (_plisDeck.getAllCarts()[Macro.INDEX_FIRST_CART]);
        }

        private int sendBadCart(String msg)
        {
            _server.sendToClient(msg, _currentPlayer.getId(), false);
            _server.sendToAllClients("\nLe joueur " + _currentPlayer.getId() + " a tenté de jouer " +
                    _cartChoosen.getNumber() + ' ' + _cartChoosen.getColor() + ". Il n'a pas le droit.\n");
            return (Macro.BAD_CART);
        }

        private Cart getBestCart()
        {
            int maxPoints = 0;
            Cart bestCart = null;

            foreach (Cart cart in _plisDeck.getAllCarts())
            {
                if (cart.getPointsCart() > maxPoints)
                {
                    maxPoints = cart.getPointsCart();
                    bestCart = cart;
                }
            }
            return (bestCart);
        }

        private int checkIfThereIsBetterAtout()
        {
            foreach (Cart cart in _currentPlayer.getDeck().getAllCarts())
            {
                if (cart != _cartChoosen && cart.getColor() == _atoutColor && cart.getPointsCart() > _cartChoosen.getPointsCart())
                    return (sendBadCart("Vous avez un atout supérieur dans votre deck."));
            }
            return (Macro.SUCCESS);
        }

        private int checkIfThereIsAnotherAtout()
        {
            foreach (Cart cart in _currentPlayer.getDeck().getAllCarts())
            {
                if (cart != _cartChoosen && cart.getColor() == _atoutColor)
                    return (sendBadCart("Vous avez un atout dans votre deck."));
            }
            return (Macro.SUCCESS);
        }

        private int checkIfThereIsAnotherColor()
        {
            foreach (Cart cart in _currentPlayer.getDeck().getAllCarts())
            {
                if (cart.getColor() == _firstCartPlis.getColor())
                    return (sendBadCart("Vous avez une carte de la même couleur que la première carte jouée."));
            }
            return (Macro.SUCCESS);
        }

        private Player findTeamMate(Player.teamColor team, int id)
        {
            foreach (Player player in _players)
                if (player.getTeamColor() == team && player.getId() != id)
                    return (player);
            return (null);
        }

        private bool teamMateIsWinner()
        {
            Player teamMate;

            if ((teamMate = findTeamMate(_currentPlayer.getTeamColor(), _currentPlayer.getId())) == null)
            {
                sendBadCart("Une erreur est survenue, impossible d'identifier votre coéquipier.\n");
                return (false);
            }
            foreach (Cart cart in teamMate.getDeck().getAllCarts())
            {
                if (cart == getBestCart())
                    return (true);
            }
            return (false);
        }

        private int checkForNonAtout()
        {
            if (_cartChoosen.getColor() != _firstCartPlis.getColor())
            {
                if (teamMateIsWinner())
                    return (Macro.SUCCESS);
                else
                {
                    if (checkIfThereIsAnotherColor() == Macro.SUCCESS)
                    {
                        if (_cartChoosen.getColor() == _atoutColor)
                            return (Macro.SUCCESS);
                        return (checkIfThereIsAnotherAtout());
                    }
                    return (Macro.BAD_CART);
                }
            }
            return (Macro.SUCCESS);
        }

        private int checkForAtout()
        {
            if (_cartChoosen.getColor() == _atoutColor)
            {
                if (_cartChoosen.getPointsCart() >= _firstCartPlis.getPointsCart())
                    return (Macro.SUCCESS);
                else if (_cartChoosen.getPointsCart() < _firstCartPlis.getPointsCart())
                {
                    if (teamMateIsWinner())
                        return (Macro.SUCCESS);
                    return (checkIfThereIsBetterAtout());
                }
            }
            else
                return (checkIfThereIsAnotherAtout());
            return (Macro.BAD_CART);
        }

        public int checkCart(Cart cartChoosen, Player currentPlayer)
        {
            _cartChoosen = cartChoosen;
            _atoutColor = _players[Macro.INDEX_FIRST_PLAYER].getDeck().getAtout();
            _currentPlayer = currentPlayer;
            if ((_firstCartPlis = getFirstCart()) == null)
                return (Macro.SUCCESS);
            if (_firstCartPlis.getColor() == _atoutColor)
                return (checkForAtout());
            return (checkForNonAtout());
        }
    }
}
