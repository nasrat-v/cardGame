using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCardgame
{
    class Game
    {
        private int                                 _maxAnnounce;
        private int                                 _announceToInt;
        private string                              _announce;
        private string                              _colorAnnounce;
        private bool                                _announceDefied;
        private List<Player>                        _players;
        private Deck                                _gameDeck;
        private Deck                                _tableDeck;
        private Deck                                _plisDeck;
        private Rules                               _rules;
        private Server                              _server;
        private Dictionary<Cart.cartNumber, string> _cartNumberMap;
        private Dictionary<Cart.cartColor, string>  _cartColorMap;
        private Dictionary<Player.teamColor, int>   _teamsPointsMap;
        private Dictionary<Player.teamColor, int>   _teamsFinalPointsMap;
        private Player                              _bestAnnoncer;
        private Player.teamColor                    _dixDeDer;
        private Player.teamColor                    _teamBestAnnonce;
        private Player.teamColor                    _rebelotteTeam;

        public Game(Server server)
        {
            _players = new List<Player>();
            _gameDeck = new Deck();
            _tableDeck = new Deck();
            _plisDeck = new Deck();
            _rules = new Rules(_plisDeck, _players, server);
            initCartMap();
            initPointsMap();
            _maxAnnounce = 0;
            _server = server;
            _rebelotteTeam = Player.teamColor.NO_COLOR;
        }

        private void initCartMap()
        {
            // Number
            _cartNumberMap = new Dictionary<Cart.cartNumber, string>();
            _cartNumberMap.Add(Cart.cartNumber.SEPT, "7");
            _cartNumberMap.Add(Cart.cartNumber.HUIT, "8");
            _cartNumberMap.Add(Cart.cartNumber.NEUF, "9");
            _cartNumberMap.Add(Cart.cartNumber.DIX, "10");
            _cartNumberMap.Add(Cart.cartNumber.VALET, "VALET");
            _cartNumberMap.Add(Cart.cartNumber.DAME, "DAME");
            _cartNumberMap.Add(Cart.cartNumber.ROI, "ROI");
            _cartNumberMap.Add(Cart.cartNumber.AS, "AS");
            // Color
            _cartColorMap = new Dictionary<Cart.cartColor, string>();
            _cartColorMap.Add(Cart.cartColor.CARREAU, "CARREAU");
            _cartColorMap.Add(Cart.cartColor.COEUR, "COEUR");
            _cartColorMap.Add(Cart.cartColor.PIQUE, "PIQUE");
            _cartColorMap.Add(Cart.cartColor.TREFLE, "TREFLE");
        }

        private void initPointsMap()
        {
            _teamsPointsMap = new Dictionary<Player.teamColor, int>();
            _teamsPointsMap.Add(Player.teamColor.BLEUE, 0);
            _teamsPointsMap.Add(Player.teamColor.ROUGE, 0);
            _teamsFinalPointsMap = new Dictionary<Player.teamColor, int>();
            _teamsFinalPointsMap.Add(Player.teamColor.BLEUE, 0);
            _teamsFinalPointsMap.Add(Player.teamColor.ROUGE, 0);
        }

        private Player getPlayerById(int id)
        {
            foreach (Player player in _players)
                if (player.getId() == id)
                    return (player);
            return (null);
        }

        private Player getPlayerByIndex(int index)
        {
            int i = 0;

            foreach (Player player in _players)
            {
                if (i == index)
                    return (player);
                i += 1;
            }
            return (null);
        }

        private void printAndSendPlayerDeck()
        {
            string cartNumber;
            string cartColor;
            Cart.cartColor cartAtout;
            int id;

            foreach (Player player in _players)
            {
                id = player.getId();
                if (!(player.getDeck().getAllCarts().Any()))
                    _server.sendToClient("\nVous n'avez plus de cartes", id, false);
                else
                    _server.sendToClient("\nVoici vos cartes:", id, false);
                foreach (Cart cart in player.getDeck().getAllCarts())
                {
                    cartNumber = _cartNumberMap[cart.getNumber()];
                    cartColor = _cartColorMap[cart.getColor()];
                    _server.sendToClient(cartNumber + ' ' + cartColor, id, false);
                }
                if ((cartAtout = player.getDeck().getAtout()) != Cart.cartColor.NO_COLOR)
                    _server.sendToClient("\nL'atout est " + cartAtout, id, false);
            }
        }

        private void printAndSendPlisDeck()
        {
            string cartNumber;
            string cartColor;

            if (!(_plisDeck.getAllCarts().Any()))
                _server.sendToAllClients("\nIl n'y pas de carte sur la table\n");
            else
                _server.sendToAllClients("\nVoici les cartes sur la table:\n");
            foreach (Cart cart in _plisDeck.getAllCarts())
            {
                cartNumber = _cartNumberMap[cart.getNumber()];
                cartColor = _cartColorMap[cart.getColor()];
                _server.sendToAllClients(cartNumber + ' ' + cartColor);
            }
        }

        private int printError(string msg)
        {
            System.Console.WriteLine(msg);
            return (Macro.ERROR);
        }

        private Cart.cartColor getCartColorFromStringColor(string color)
        {
            try {
                return (_cartColorMap.Where(p => p.Value == color).Select(p => p.Key).ElementAt(0));
            }
            catch (ArgumentNullException) {
                return (Cart.cartColor.NO_COLOR);
            }
            catch (ArgumentOutOfRangeException) {
                return (Cart.cartColor.NO_COLOR);
            }
        }

        private Cart.cartNumber getCartNumberFromStringNumber(string number)
        {
            try {
                return (_cartNumberMap.Where(p => p.Value == number).Select(p => p.Key).ElementAt(0));
            }
            catch (ArgumentNullException) {
                return (Cart.cartNumber.NO_NUMBER);
            }
            catch (ArgumentOutOfRangeException) {
                return (Cart.cartNumber.NO_NUMBER);
            }
        }

        private void distributeCart()
        {
            _server.sendToAllClients("\nDistribution des cartes en cours...\n");
            foreach (Player player in _players)
                player.addMultipleCartsToDeck(_gameDeck.getAndRemoveCarts(3));
            foreach (Player player in _players)
                player.addMultipleCartsToDeck(_gameDeck.getAndRemoveCarts(2));
            foreach (Player player in _players)
                player.addMultipleCartsToDeck(_gameDeck.getAndRemoveCarts(3));
        }

        private String doAnnouncement()
        {
            bool isFinish = false;
            bool firstTime = true;

            _bestAnnoncer = _players[Macro.INDEX_FIRST_PLAYER];
            while (!(isFinish))
            {
                _announceDefied = false;
                foreach (Player player in _players)
                {
                    if ((player != _bestAnnoncer) || (firstTime))
                    {
                        if (_maxAnnounce != 0)
                            _server.sendToAllClients("\nLa meilleure annonce provient du joueur " +
                                    _bestAnnoncer.getId() + ". Elle est de " + _maxAnnounce + ' ' + _colorAnnounce);
                        _server.sendToClient("\nCombien annoncez-vous ? [" + Macro.PASS + " ou nombre + COEUR/TREFLE/PIQUE/CARREAU]\n",
                                player.getId(), true);
                        while (playerProposeAnnonce(player)) ;
                        if (_maxAnnounce == Macro.MAX_ANNOUNCE)
                            return (_colorAnnounce);
                    }
                    else
                    {
                        _server.sendToAllClients("\nLe joueur " + player.getId() +
                                " a remporté l'annonce. Elle est de " + _maxAnnounce + ' ' + _colorAnnounce + '\n');
                        _server.sendToClient("Bravo, vous avez remporté l'annonce.\n", player.getId(), false);
                        _teamBestAnnonce = player.getTeamColor();
                        return (_colorAnnounce);
                    }
                    firstTime = false;
                }
                if (!(_announceDefied))
                    isFinish = true;
                Thread.Sleep(Macro.UTIME_TO_WAIT);
            }
            return (Macro.STRING_EMPTY);
        }

        private bool playerProposeAnnonce(Player player)
        {
            Thread.Sleep(Macro.UTIME_TO_WAIT);
            if (!((_announce = _server.getMsg()).Any()))
                return (true);
            if (!(_announce.Equals(Macro.PASS)))
            {
                if (parsAnnounce())
                {
                    _announceDefied = true;
                    _maxAnnounce = _announceToInt;
                    _colorAnnounce = _announce;
                    _bestAnnoncer = player;
                    return (false);
                }
                _server.sendToClient("Votre annonce doit-être compris entre 80 et 160, " +
                        "et supérieur à la meilleure annonce.\nCombien annoncez-vous ? ["
                        + Macro.PASS + " ou nombre + COEUR/TREFLE/PIQUE/CARREAU]\n", player.getId(), true);
                return (true);
            }
            return (false);
        }

        private int findTeamMate(Player.teamColor team, int id)
        {
            foreach (Player player in _players)
                if (player.getTeamColor() == team && player.getId() != id)
                    return (player.getId());
            return (-1);
        }

        private bool parsAnnounce()
        {
            String integerString;
            int pos;

            if ((pos = _announce.IndexOf(' ')) != -1)
            {
                try
                {
                    try
                    {
                        integerString = _announce.Substring(0, pos);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        return (false);
                    }
                    _announceToInt = int.Parse(integerString);
                }
                catch (FormatException)
                {
                    return (false);
                }
                return (parsAnnounceMore(pos));
            }
            return (false);
        }

        private bool parsAnnounceMore(int pos)
        {
            try
            {
                _announce = _announce.Substring(pos + 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                return (false);
            }
            if ((_announceToInt <= _maxAnnounce) || (_announceToInt < Macro.MIN_ANNOUNCE) ||
                    (_announceToInt > Macro.MAX_ANNOUNCE) || ((!_announce.Equals("COEUR")) &&
                    (!_announce.Equals("PIQUE")) && (!_announce.Equals("TREFLE")) &&
                    (!_announce.Equals("CARREAU"))))
            {
                _announce = Macro.STRING_EMPTY;
                return (false);
            }
            return (true);
        }


        private Cart findCartFromString(List<Cart> carts, String cartString)
        {
            int pos;
            Cart.cartColor color;
            Cart.cartNumber number;

            if ((pos = cartString.IndexOf(' ')) == -1)
                return (null);
            try
            {
                if ((color = getCartColorFromStringColor(cartString.Substring(pos + 1))) == Cart.cartColor.NO_COLOR)
                    return (null);
                if ((number = getCartNumberFromStringNumber(cartString.Substring(0, pos))) == Cart.cartNumber.NO_NUMBER)
                    return (null);
            }
            catch (ArgumentOutOfRangeException) {
                return (null);
            }
            foreach (Cart cart in carts)
                if (cart.getNumber().Equals(number) && cart.getColor().Equals(color))
                    return (cart);
            return (null);
        }

        private void sendWelcomeToPlayers()
        {
            int id;
            Player.teamColor team;

            foreach (Player player in _players)
            {
                id = player.getId();
                team = player.getTeamColor();
                _server.sendToClient("\nBienvenu sur le serveur de jeu Cardgame 2017 !\n", id, false);
                _server.sendToClient("Vous faites partie de l'équipe " + team, id, false);
                _server.sendToClient("Votre coéquipier est le joueur " + findTeamMate(team, id), id, false);
            }
        }

        public void createPlayer(int id)
        {
            int i;
            Player.teamColor team;

            i = -1;
            while (++i < 4)
            {
                if ((i % 2) == 0)
                    team = Player.teamColor.BLEUE;
                else
                    team = Player.teamColor.ROUGE;
                Player player = new Player(i + 1 + id, team);
                _players.Add(player);
            }
        }

        private int setAtout(String atout)
        {
            Cart.cartColor color;

            if ((color = getCartColorFromStringColor(atout)) != Cart.cartColor.NO_COLOR)
            {
                foreach (Player player in _players)
                    player.getDeck().setAtout(color);
                return (Macro.SUCCESS);
            }
            return (Macro.ERROR);
        }

        private int cartCanBePlayed(Cart cartChoosen, Player player)
        {
            return (_rules.checkCart(cartChoosen, player));
        }

        private bool belottePossible(Player player, Cart.cartColor atout)
        {
            Cart dameBelotte = new Cart();
            Cart roiBelotte = new Cart();

            dameBelotte.setColor(atout);
            dameBelotte.setNumber(Cart.cartNumber.DAME);
            roiBelotte.setColor(atout);
            roiBelotte.setNumber(Cart.cartNumber.ROI);
            foreach (Cart cart in player.getDeck().getAllCarts())
            {
                if (((cart.getNumber() == Cart.cartNumber.DAME && cart.getColor() == atout) &&
                        (cartCanBePlayed(dameBelotte, player) != Macro.BAD_CART)) ||
                    ((cart.getNumber() == Cart.cartNumber.ROI && cart.getColor() == atout)
                            && (cartCanBePlayed(roiBelotte, player) != Macro.BAD_CART)))
                {
                    player.setBelotte(true);
                    return (true);
                }
            }
            return (false);
        }

        private bool rebelottePossible(Player player, Cart.cartColor atout)
        {
            if (!(player.getBelotte()))
            {
                _server.sendToClient("Vous devez annoncer BELOTTE avant d'annoncer REBELOTTE.\n", player.getId(), false);
                return (false);
            }
            if (!(belottePossible(player, atout)))
            {
                _server.sendToClient("Vous ne pouvez pas annoncer REBELOTTE avec vos cartes.\n", player.getId(), false);
                return (false);
            }
            return (true);
        }

        private Cart getCartBelotteRebelotte(Player player, String belotteRebelotte)
        {
            Cart.cartColor atout;
            Cart cartChoosen;

            atout = player.getDeck().getAtout();
            _server.sendToClient(belotteRebelotte + " acceptée. Veuillez choisir entre la DAME " +
                    atout + " et le ROI " + atout + ".\n", player.getId(), true);
            while (!((_announce = _server.getMsg()).Any()))
                Thread.Sleep(Macro.UTIME_TO_WAIT);
            if ((((cartChoosen = findCartFromString(player.getDeck().getAllCarts(), _announce)).getNumber() == Cart.cartNumber.DAME) ||
                    (cartChoosen.getNumber() == Cart.cartNumber.ROI)) && cartChoosen.getColor() == atout)
                return (cartChoosen);
            _server.sendToClient("Vous ne pouvez pas choisir cette carte car vous avez annoncé " +
                    belotteRebelotte + ".\n", player.getId(), false);
            return (null);
        }

        private Cart checkBelotte(Player player)
        {
            Cart cartChoosen;

            if (!(belottePossible(player, player.getDeck().getAtout())))
            {
                _server.sendToClient("Vous ne pouvez pas annoncer BELOTTE avec vos cartes.\n", player.getId(), false);
                return (null);
            }
            if ((cartChoosen = getCartBelotteRebelotte(player, "BELOTTE")) == null)
                return (checkBelotte(player));
            return (cartChoosen);
        }

        private Cart checkRebelotte(Player player)
        {
            Cart cartChoosen;

            if (!(rebelottePossible(player, player.getDeck().getAtout())))
                return (null);
            if ((cartChoosen = getCartBelotteRebelotte(player, "REBELOTTE")) == null)
                return (checkRebelotte(player));
            _rebelotteTeam = player.getTeamColor();
            return (cartChoosen);
        }

        private int chooseCartToPlay(Player player, int id)
        {
            Cart cartChoosen;

            _server.sendToClient("\nChoisissez une carte à jouer [nombre + COEUR/TREFLE/PIQUE/CARREAU ou BELOTTE ou REBELOTTE] :",
                    id, true);
            while (!((_announce = _server.getMsg()).Any()))
                Thread.Sleep(Macro.UTIME_TO_WAIT);
            if (_announce.Equals("BELOTTE"))
            {
                if ((cartChoosen = checkBelotte(player)) == null)
                    return (chooseCartToPlay(player, id));
            }
            else if (_announce.Equals("REBELOTTE"))
            {
                if ((cartChoosen = checkRebelotte(player)) == null)
                    return (chooseCartToPlay(player, id));
            }
            else if ((cartChoosen = findCartFromString(player.getDeck().getAllCarts(), _announce)) == null)
            {
                _server.sendToClient("Cette carte n'existe pas", id, false);
                return (chooseCartToPlay(player, id));
            }
            else if (cartCanBePlayed(cartChoosen, player) == Macro.BAD_CART)
            {
                _server.sendToClient("Vous ne pouvez pas jouer cette carte.", id, false);
                return (chooseCartToPlay(player, id));
            }
            _server.sendToAllClients("\nLe joueur " + id + " a joué " + _announce + '\n');
            _tableDeck.addOneCart(cartChoosen);
            _plisDeck.addOneCart(cartChoosen);
            player.removeOneCart(cartChoosen);
            return (Macro.SUCCESS);
        }

        private void reSetOrderPlayer(Player player)
        {
            List<Player> tmpList = new List<Player>();

            tmpList.Add(player);
            if (player.getId() == (Macro.INDEX_FIRST_PLAYER + 1))
            {
                tmpList.Add(getPlayerById(Macro.INDEX_SECOND_PLAYER + 1));
                tmpList.Add(getPlayerById(Macro.INDEX_THIRD_PLAYER + 1));
                tmpList.Add(getPlayerById(Macro.INDEX_LAST_PLAYER + 1));
            }
            else if (player.getId() == (Macro.INDEX_SECOND_PLAYER + 1))
            {
                tmpList.Add(getPlayerById(Macro.INDEX_THIRD_PLAYER + 1));
                tmpList.Add(getPlayerById(Macro.INDEX_LAST_PLAYER + 1));
                tmpList.Add(getPlayerById(Macro.INDEX_FIRST_PLAYER + 1));
            }
            else if (player.getId() == (Macro.INDEX_THIRD_PLAYER + 1))
            {
                tmpList.Add(getPlayerById(Macro.INDEX_LAST_PLAYER + 1));
                tmpList.Add(getPlayerById(Macro.INDEX_FIRST_PLAYER + 1));
                tmpList.Add(getPlayerById(Macro.INDEX_SECOND_PLAYER + 1));
            }
            else if (player.getId() == (Macro.INDEX_LAST_PLAYER + 1))
            {
                tmpList.Add(getPlayerById(Macro.INDEX_FIRST_PLAYER + 1));
                tmpList.Add(getPlayerById(Macro.INDEX_SECOND_PLAYER + 1));
                tmpList.Add(getPlayerById(Macro.INDEX_THIRD_PLAYER + 1));
            }
            _players = tmpList;
        }

        private void setPointToTeams()
        {
            int i = 0;
            int cartWithMaxPoint = 0;
            int indexPlayerWithBestCart = 0;
            int addition = 0;

            foreach (Cart cart in _plisDeck.getAllCarts())
            {
                if ((_plisDeck.getAllCarts()[Macro.INDEX_FIRST_CART].getColor() != cart.getAtout()
                        && cart.getPointsCart() > cartWithMaxPoint &&
                        cart.getColor() == _plisDeck.getAllCarts()[Macro.INDEX_FIRST_CART].getColor())
                                || (_plisDeck.getAllCarts()[Macro.INDEX_FIRST_CART].getColor() != cart.getAtout()
                        && cart.getPointsCart() > cartWithMaxPoint && cart.getColor() == cart.getAtout()) ||
                        (_plisDeck.getAllCarts()[Macro.INDEX_FIRST_CART].getColor() == cart.getAtout()
                                && cart.getPointsCart() > cartWithMaxPoint && cart.getColor() == cart.getAtout()))
                {
                    cartWithMaxPoint = cart.getPointsCart();
                    indexPlayerWithBestCart = i;
                }
                addition += cart.getPointsCart();
                i++;
            }
            if ((getPlayerByIndex(indexPlayerWithBestCart).getId() == (Macro.INDEX_FIRST_PLAYER + 1)) ||
                    (getPlayerByIndex(indexPlayerWithBestCart).getId() == (Macro.INDEX_THIRD_PLAYER + 1)))
            {
                _teamsPointsMap[Player.teamColor.BLEUE] = _teamsPointsMap[Player.teamColor.BLEUE] + addition;
                _server.sendToAllClients("\nL'équipe " + Player.teamColor.BLEUE + " a remporté le plis.\n");
                _dixDeDer = Player.teamColor.BLEUE;
            }
            else
            {
                _teamsPointsMap[Player.teamColor.ROUGE] = _teamsPointsMap[Player.teamColor.ROUGE] + addition;
                _server.sendToAllClients("\nL'équipe " + Player.teamColor.ROUGE + " a remporté le plis.\n");
                _dixDeDer = Player.teamColor.ROUGE;
            }
            reSetOrderPlayer(getPlayerByIndex(indexPlayerWithBestCart));
            System.Console.Write('\n');
        }

        private void sendPointsDonne()
        {
            int pointsBlue;
            int pointsRed;

            if (_dixDeDer == Player.teamColor.BLEUE)
            {
                pointsBlue = Macro.BONUS_DIX_DE_DER + _teamsPointsMap[Player.teamColor.BLEUE];
                pointsRed = _teamsPointsMap[Player.teamColor.ROUGE];
                _server.sendToAllClients("Le bonus Dix-de-der est pour l'équipe " + Player.teamColor.BLEUE + ".\n");
            }
            else
            {
                pointsRed = Macro.BONUS_DIX_DE_DER + _teamsPointsMap[Player.teamColor.ROUGE];
                pointsBlue = _teamsPointsMap[Player.teamColor.BLEUE];
                _server.sendToAllClients("Le bonus Dix-de-der est pour l'équipe " + Player.teamColor.ROUGE + ".\n");
            }
            if (_rebelotteTeam == Player.teamColor.BLEUE)
            {
                pointsBlue += Macro.BONUS_BELOTTE_REBELOTTE;
                _server.sendToAllClients("Le bonus Belotte/Rebelotte est pour l'équipe " + Player.teamColor.BLEUE + ".\n");
            }
            else if (_rebelotteTeam == Player.teamColor.ROUGE)
            {
                pointsRed += Macro.BONUS_BELOTTE_REBELOTTE;
                _server.sendToAllClients("Le bonus Belotte/Rebelotte est pour l'équipe " + Player.teamColor.ROUGE + ".\n");
            }
            _server.sendToAllClients("L'équipe " + Player.teamColor.BLEUE + " a marqué " + pointsBlue + " points.\n");
            _server.sendToAllClients("L'équipe " + Player.teamColor.ROUGE + " a marqué " + pointsRed + " points.\n");
            if (_teamBestAnnonce == Player.teamColor.BLEUE)
            {
                _server.sendToAllClients("L'équipe " + Player.teamColor.BLEUE + " a annoncé " + _announceToInt + " points.\n");
                if (pointsBlue >= _announceToInt)
                {
                    _server.sendToAllClients("L'équipe " + Player.teamColor.BLEUE + " a donc remporté la donne.\n");
                    _teamsFinalPointsMap[Player.teamColor.BLEUE] = _teamsFinalPointsMap[Player.teamColor.BLEUE] + pointsBlue;
                }
                else
                {
                    _server.sendToAllClients("L'équipe " + Player.teamColor.BLEUE + " a donc perdu la donne.\n");
                    _teamsFinalPointsMap[Player.teamColor.ROUGE] = _teamsFinalPointsMap[Player.teamColor.ROUGE] + Macro.MAX_ANNOUNCE;
                }
            }
            else
            {
                _server.sendToAllClients("L'équipe " + Player.teamColor.ROUGE + " a annoncé " + _announceToInt + " points.\n");
                if (pointsRed >= _announceToInt)
                {
                    _server.sendToAllClients("L'équipe " + Player.teamColor.ROUGE + " a donc remporté la donne.\n");
                    _teamsFinalPointsMap[Player.teamColor.ROUGE] = _teamsFinalPointsMap[Player.teamColor.ROUGE] + pointsRed;
                }
                else
                {
                    _server.sendToAllClients("L'équipe " + Player.teamColor.ROUGE + " a donc perdu la donne.\n");
                    _teamsFinalPointsMap[Player.teamColor.BLEUE] = _teamsFinalPointsMap[Player.teamColor.BLEUE] + Macro.MAX_ANNOUNCE;
                }
            }
            _teamsPointsMap[Player.teamColor.BLEUE] = 0;
            _teamsPointsMap[Player.teamColor.ROUGE] = 0;
        }

        private void sendPointsGame()
        {
            int pointsBlue;
            int pointsRed;

            pointsBlue = _teamsFinalPointsMap[Player.teamColor.BLEUE];
            pointsRed = _teamsFinalPointsMap[Player.teamColor.ROUGE];
            _server.sendToAllClients("L'équipe " + Player.teamColor.BLEUE + " a marqué au total " + pointsBlue + ".\n");
            _server.sendToAllClients("L'équipe " + Player.teamColor.ROUGE + " a marqué au total " + pointsRed + ".\n");
            if (pointsBlue > pointsRed)
                _server.sendToAllClients("L'équipe " + Player.teamColor.BLEUE + " a donc remporté la partie.\n");
            else if (pointsBlue < pointsRed)
                _server.sendToAllClients("L'équipe " + Player.teamColor.ROUGE + " a donc remporté la partie.\n");
            else
                _server.sendToAllClients("L'équipe " + Player.teamColor.ROUGE + " et l'équipe " +
                        Player.teamColor.BLEUE + " sont donc a égalité.\n");
        }

        private void unsetBonusBelottePlayers()
        {
            foreach (Player player in _players)
                player.setBelotte(false);
            _rebelotteTeam = Player.teamColor.NO_COLOR;
        }

        private void playGame()
        {
            int nbDonne = -1;
            bool firstTime = true;

            while (++nbDonne < Macro.MAX_DONNE)
            {
                if (!(firstTime))
                    distributeCart();
                else
                    firstTime = false;
                while (!(_players[Macro.INDEX_LAST_PLAYER].deckIsEmpty()))
                {
                    while (_plisDeck.getAllCarts().Count < Macro.NB_CART_PLIS)
                    {
                        foreach (Player player in _players)
                        {
                            printAndSendPlayerDeck();
                            chooseCartToPlay(player, player.getId());
                            printAndSendPlisDeck();
                        }
                        Thread.Sleep(Macro.UTIME_TO_WAIT);
                    }
                    setPointToTeams();
                    _plisDeck.removeAllCart();
                    Thread.Sleep(Macro.UTIME_TO_WAIT);
                }
                sendPointsDonne();
                unsetBonusBelottePlayers();
                _gameDeck.addMultipleCarts(_tableDeck.getAllCarts());
                _tableDeck.removeAllCart();
                Thread.Sleep(Macro.UTIME_TO_WAIT);
            }
            sendPointsGame();
            _server.sendToAllClients("Partie terminée\n");
        }

        public int runGame()
        {
            _gameDeck.initGameDeck();
            _gameDeck.mixDeck();
            sendWelcomeToPlayers();
            distributeCart();
            printAndSendPlayerDeck();
            if (setAtout(doAnnouncement()) == Macro.ERROR)
                return (printError("Erreur: durant la séléction de l'atout\n"));
            playGame();
            return (Macro.SUCCESS);
        }
    }
}
