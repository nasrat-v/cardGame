using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCardgame
{
    class Macro
    {
        public static string        OBJECT_TYPE_MESSAGE = "Message";
        public static string        STRING_EMPTY = "";
        public static string        PASS = "PASS";
        public static string        RESPONSE_EXPECTED_QUERY = "[RESPONSE]";
        public static string        CLIENT_DISCONNECT_QUERY = "[DISCONNECT]";
        public static string        CLIENT_CONNECT_QUERY = "[CONNECT]";
        public static string        MESSAGE_RECEIVED = "[OK]";
        public static string        CONNECTION_REFUSED_QUERY = "[REFUSED]";
        public static int           MIN_ANNOUNCE = 80;
        public static int           MAX_ANNOUNCE = 160;
        public static int           MAX_DONNE = 1;
        public static int           ERROR = 0;
        public static int           SUCCESS = 1;
        public static int           BAD_CART = 0;
        public static int           INDEX_FIRST_PLAYER = 0;
        public static int           INDEX_SECOND_PLAYER = 1;
        public static int           INDEX_THIRD_PLAYER = 2;
        public static int           INDEX_LAST_PLAYER = 3;
        public static int           INDEX_FIRST_CART = 0;
        public static int           UTIME_TO_WAIT = 250;
        public static int           NB_CART_PLIS = 4;
        public static int           NB_PLAYERS = 4;
        public static int           BONUS_DIX_DE_DER = 10;
        public static int           BONUS_BELOTTE_REBELOTTE = 20;
        public static short         ERROR_PARAM = 0;
    }
}
