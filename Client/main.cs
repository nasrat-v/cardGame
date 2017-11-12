using NetworkCommsDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCardgame
{
    class main
    {
        static short    checkParam(string[] args)
        {
            short       port = Macro.ERROR_PARAM;

            if (args.Length == 0)
            {
                System.Console.WriteLine("Usage: ./Client ADDRESS_IP PORT");
                return (Macro.ERROR_PARAM);
            }
            try {
                port = short.Parse(args[1]);
            }
            catch (FormatException) {
                System.Console.WriteLine("Usage: ./Client ADDRESS_IP PORT");
            }
            catch (OverflowException) {
                System.Console.WriteLine("Usage: ./Client ADDRESS_IP PORT");
            }
            catch (ArgumentNullException) {
                System.Console.WriteLine("Usage: ./Client ADDRESS_IP PORT");
            }
            catch (IndexOutOfRangeException) {
                System.Console.WriteLine("Usage: ./Client ADDRESS_IP PORT");
            }
            return (port);
        }

        static void     Main(string[] args)
        {
            short       port;
            Client      client = new Client();
            
            if ((port = checkParam(args)) == Macro.ERROR_PARAM)
                return ;
            client.initClient(args[0], port);
            client.launchClient();
        }
    }
}
