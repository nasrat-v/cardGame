using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCardgame
{
    class Macro
    {
        public static int       UTIME_TO_WAIT = 250;
        public static string    RESPONSE_EXPECTED_QUERY = "[RESPONSE]";
        public static string    CLIENT_DISCONNECT_QUERY = "[DISCONNECT]";
        public static string    CLIENT_CONNECT_QUERY = "[CONNECT]";
        public static string    OBJECT_TYPE_MESSAGE = "Message";
        public static string    MESSAGE_RECEIVED = "[OK]";
        public static string    STRING_EMPTY = "";
        public static string    CONNECTION_REFUSED_QUERY = "[REFUSED]";
        public static short     ERROR_PARAM = 0;
    }
}
