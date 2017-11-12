using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkCommsDotNet;
using System;
using System.Threading;
using NetworkCommsDotNet.Connections;

namespace ClientCardgame
{
    public class Client
    {
        private bool        _end;
        private bool        _responseExpected;
        private short       _port;
        private string      _ipAddress;
        private string      _msg;

        public          Client()
        {
            _end = false;
            _responseExpected = false;
            _msg = Macro.STRING_EMPTY;
        }

        public void     initClient(string ipAddress, short port)
        {
            _port = port;
            _ipAddress = ipAddress;
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(Macro.OBJECT_TYPE_MESSAGE, received);
            sendToServer(Macro.CLIENT_CONNECT_QUERY);
        }

        public string   getMsg()
        {
            string      tmpMsg;

            tmpMsg = _msg;
            _msg = Macro.STRING_EMPTY;
            return (tmpMsg);
        }

        public void     received(PacketHeader header, Connection connection, string message)
        {
            if (message.Equals(Macro.CONNECTION_REFUSED_QUERY))
            {
                System.Console.WriteLine("A game is already launched\nConnection refused");
                Environment.Exit(0);
            }
            _msg = message;
            if (!(message.Equals(Macro.MESSAGE_RECEIVED)))
                sendToServer(Macro.MESSAGE_RECEIVED);
            parsMessage(message);
        }

        private void    parsMessage(String msg)
        {
            if (msg.Contains(Macro.RESPONSE_EXPECTED_QUERY))
            {
                _responseExpected = true;
                try {
                    System.Console.WriteLine(msg.Substring(Macro.RESPONSE_EXPECTED_QUERY.Count()));
                }
                catch (ArgumentOutOfRangeException)
                {
                    System.Console.WriteLine("Une erreur est survenue. Le message suivant provenant du serveur peut contenir des erreurs");
                    System.Console.WriteLine(msg);
                }
            }
            else if (msg.Contains(Macro.CLIENT_DISCONNECT_QUERY))
            {
                NetworkComms.Shutdown();
                _end = true;
            }
            else
                System.Console.WriteLine(msg);
        }

        private void    sendToServer(String msg)
        {
            bool        end = false;
            
            NetworkComms.SendObject(Macro.OBJECT_TYPE_MESSAGE, _ipAddress, _port, msg);
            if (!(msg.Equals(Macro.MESSAGE_RECEIVED)))
            {
                while (!(end))
                {
                    while (msgIsEmpty()) ;
                    if (getMsg() != Macro.MESSAGE_RECEIVED)
                        NetworkComms.SendObject(Macro.OBJECT_TYPE_MESSAGE, _ipAddress, _port, msg);
                    else
                        end = true;
                }
            }
        }

        private bool    msgIsEmpty()
        {
            return (!(_msg.Any()));
        }

        public void     launchClient()
        {
            String      msg;

            while (!(_end))
            {
                if (_responseExpected)
                {
                    msg = Console.ReadLine();
                    if (msg.Any())
                    {
                        sendToServer(msg);
                        _responseExpected = false;
                    }
                    else
                        System.Console.WriteLine("Vous devez entrer une valeur correct");
                }
                Thread.Sleep(Macro.UTIME_TO_WAIT);
            }
        }
    }
}