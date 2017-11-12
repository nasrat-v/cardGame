using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCardgame
{
    class Cart
    {
        public enum cartColor
        {
            CARREAU,
            COEUR,
            TREFLE,
            PIQUE,
            NO_COLOR
        }

        public enum cartNumber
        {
            SEPT,
            HUIT,
            NEUF,
            DIX,
            VALET,
            DAME,
            ROI,
            AS,
            NO_NUMBER
        }

        private cartColor                   _color;
        private cartColor                   _atout;
        private cartNumber                  _number;
        private Dictionary<cartNumber, int> _pointsAtous;
        private Dictionary<cartNumber, int> _pointsNonAtous;

        public Cart()
        {
            initMap();
        }

        public Cart(cartColor color, cartNumber number)
        {
            initMap();
            _color = color;
            _number = number;
            _atout = cartColor.NO_COLOR;
        }

        private void initMap()
        {
            // Non Atout
            _pointsNonAtous = new Dictionary<cartNumber, int>();
            _pointsNonAtous.Add(cartNumber.SEPT, 0);
            _pointsNonAtous.Add(cartNumber.HUIT, 0);
            _pointsNonAtous.Add(cartNumber.NEUF, 0);
            _pointsNonAtous.Add(cartNumber.VALET, 2);
            _pointsNonAtous.Add(cartNumber.DAME, 3);
            _pointsNonAtous.Add(cartNumber.ROI, 4);
            _pointsNonAtous.Add(cartNumber.DIX, 10);
            _pointsNonAtous.Add(cartNumber.AS, 11);
            // Atout
            _pointsAtous = new Dictionary<cartNumber, int>();
            _pointsAtous.Add(cartNumber.SEPT, 0);
            _pointsAtous.Add(cartNumber.HUIT, 0);
            _pointsAtous.Add(cartNumber.DAME, 3);
            _pointsAtous.Add(cartNumber.ROI, 4);
            _pointsAtous.Add(cartNumber.DIX, 10);
            _pointsAtous.Add(cartNumber.AS, 11);
            _pointsAtous.Add(cartNumber.NEUF, 14);
            _pointsAtous.Add(cartNumber.VALET, 20);
        }

        public cartColor getColor()
        {
            return (_color);
        }

        public void setColor(cartColor color)
        {
            _color = color;
        }

        public cartNumber getNumber()
        {
            return (_number);
        }

        public void setNumber(cartNumber number)
        {
            _number = number;
        }

        public cartColor getAtout()
        {
            return (_atout);
        }

        public void setAtout(cartColor atout)
        {
            _atout = atout;
        }

        public int getPointsCart()
        {
            if (_color.Equals(_atout))
                return (_pointsAtous[_number]);
            return (_pointsNonAtous[_number]);
        }
    }
}
