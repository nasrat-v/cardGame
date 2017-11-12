using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using System.Threading;

namespace ServerCardgame
{
    class Server
    {
        private string                  _msg;
        private int                     _cnt;
        private short                   _port;
        private List<ClientConnected>   _allClients;

        public              Server(short port)
        {
            _port = port;
            _msg = Macro.STRING_EMPTY;
            _cnt = 0;
            _allClients = new List<ClientConnected>();
        }

        public void        start()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(Macro.OBJECT_TYPE_MESSAGE, received);
            Connection.StartListening(ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Any, _port));
        }

        public string       getMsg()
        {
            string          tmpMsg;

            tmpMsg = _msg;
            _msg = Macro.STRING_EMPTY;
            return (tmpMsg);
        }

        public void        clearMsgBuffer()
        {
            _msg = Macro.STRING_EMPTY;
        }

        private void        waitResponse(string msg, Connection connection)
        {
            bool            end = false;

            while (!(end))
            {
                while (msgIsEmpty()) ;
                if (getMsg() != Macro.MESSAGE_RECEIVED)
                    connection.SendObject(Macro.OBJECT_TYPE_MESSAGE, msg);
                else
                    end = true;
            }
        }

        public void         sendToClient(string msg, int clientId, bool responseExpected)
        {
            foreach (ClientConnected client in _allClients)
            {
                if (client.getPlayerId() == clientId)
                {
                    if (responseExpected)
                        msg = Macro.RESPONSE_EXPECTED_QUERY + msg;
                    client.getConnection().SendObject(Macro.OBJECT_TYPE_MESSAGE, msg);
                    if (!(msg.Equals(Macro.MESSAGE_RECEIVED)))
                        waitResponse(msg, client.getConnection());
                }
            }
        }

        public void         sendToClient(string msg, Connection connection, bool responseExpected)
        {
            if (responseExpected)
                msg = Macro.RESPONSE_EXPECTED_QUERY + msg;
            connection.SendObject(Macro.OBJECT_TYPE_MESSAGE, msg);
            if (!(msg.Equals(Macro.MESSAGE_RECEIVED)))
                waitResponse(msg, connection);
        }

        public void         sendToAllClients(string msg)
        {
            foreach (ClientConnected client in _allClients)
            {
                client.getConnection().SendObject(Macro.OBJECT_TYPE_MESSAGE, msg);
                if (!(msg.Equals(Macro.MESSAGE_RECEIVED)))
                    waitResponse(msg, client.getConnection());
            }
        }
        private bool msgIsEmpty()
        {
            return (!(_msg.Any()));
        }

        public int          getNbConnection()
        {
            return (_cnt);
        }

        private ClientConnected findPlayerFromNetworkId(string networkId)
        {
            foreach (ClientConnected client in _allClients)
            {
                if (networkId.Equals(client.getNetworkId()))
                    return (client);
            }
            return (null);
        }

        private void        received(PacketHeader header, Connection connection, string message)
        {
            ClientConnected client;

            if (message.IndexOf(Macro.CLIENT_CONNECT_QUERY) != -1)
                connected(connection);
            else
            {
                _msg = message;
                if ((client = findPlayerFromNetworkId(connection.ConnectionInfo.NetworkIdentifier)) == null)
                    System.Console.WriteLine("Impossible d'identifier le propriétaire du message reçu. " +
                        "Le message suivant peut contenir des erreurs\n" + message);
                else
                    System.Console.WriteLine(message + " => from " +  client.getPlayerId());
            }   
            if (!(message.Equals(Macro.MESSAGE_RECEIVED)))
                sendToClient(Macro.MESSAGE_RECEIVED, connection, false);
        }

        private void        connected(Connection connection)
        {
            if (_cnt == Macro.NB_PLAYERS)
                sendToClient(Macro.CONNECTION_REFUSED_QUERY, connection, false);
            else
            {
                ClientConnected client = new ClientConnected(++_cnt, connection);
                _allClients.Add(client);
                System.Console.WriteLine("New client connected " + client.getPlayerId() + " NetID = " + client.getNetworkId());
            }
        }

        private void        disconnected(ClientConnected client)
        {
            _cnt -= 1;
            _allClients.Remove(client);
            System.Console.WriteLine("Client disconnected " + client.getPlayerId() + " NetID = "  + client.getNetworkId());
        }
    }
}