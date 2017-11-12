using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkCommsDotNet.Connections;

namespace ServerCardgame
{
    class ClientConnected
    {
        private int         _playerId;
        private Connection _connection;

        public ClientConnected(int playerId, Connection connection)
        {
            _playerId = playerId;
            _connection = connection;
        }

        public Connection   getConnection()
        {
            return (_connection);
        }

        public int          getPlayerId()
        {
            return (_playerId);
        }

        public string       getNetworkId()
        {
            return (_connection.ConnectionInfo.NetworkIdentifier);
        }

        public void         setPlayerId(int id)
        {
            _playerId = id;
        }
    }
}
