using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCardgame
{
    class Room
    {
        private Server  _server;
        private Game    _game;
        private Thread  _serverThread;

        public Room(short port)
        {
            _server = new Server(port);
            _serverThread = new Thread(_server.start);
        }

        public void init()
        {
            _serverThread.Start();
        }

    public int  waitForConnection()
    {
        int     nbGameLaunched = 0;

        while (true)
        {
            if (_server.getNbConnection() == Macro.NB_PLAYERS)
            {
                _game = new Game(_server);
                _server.clearMsgBuffer();
                _game.createPlayer(nbGameLaunched * Macro.NB_PLAYERS);
                if (_game.runGame() == Macro.ERROR)
                    return (Macro.ERROR);
                nbGameLaunched += 1;
            }
            Thread.Sleep(Macro.UTIME_TO_WAIT);
        }
    }
}
}
