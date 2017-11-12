using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCardgame
{
    class ClientConnection
    {
        private string         _id;

        public          ClientConnection(string id)
        {
            _id = id;
        }

        public string      getId()
        {
            return (_id);
        }
    }
}
