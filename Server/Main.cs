using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCardgame
{
    class main
    {
        static short checkParam(string[] args)
        {
            short port = Macro.ERROR_PARAM;

            if (args.Length == 0)
            {
                System.Console.WriteLine("Usage: ./Server PORT");
                return (Macro.ERROR_PARAM);
            }
            try {
                port = short.Parse(args[0]);
            }
            catch (FormatException) {
                System.Console.WriteLine("Usage: ./Server PORT");
            }
            catch (OverflowException) {
                System.Console.WriteLine("Usage: ./Server PORT");
            }
            return (port);
        }

        static void Main(string[] args)
        {
            short   port;

            if ((port = checkParam(args)) == Macro.ERROR_PARAM)
                return ;
            Room room = new Room(port);
            room.init();
            if (room.waitForConnection() == Macro.ERROR)
                return ;
        }
    }
}
