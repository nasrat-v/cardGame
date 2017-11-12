using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCardgame
{
    class Deck
    {
        private List<Cart>      _carts;
        private Cart.cartColor  _atout;

        public Deck()
        {
            _carts = new List<Cart>();
            _atout = Cart.cartColor.NO_COLOR;
        }

        public void initGameDeck()
        {
            _carts.Add(new Cart(Cart.cartColor.CARREAU, Cart.cartNumber.SEPT));
            _carts.Add(new Cart(Cart.cartColor.CARREAU, Cart.cartNumber.HUIT));
            _carts.Add(new Cart(Cart.cartColor.CARREAU, Cart.cartNumber.NEUF));
            _carts.Add(new Cart(Cart.cartColor.CARREAU, Cart.cartNumber.DIX));
            _carts.Add(new Cart(Cart.cartColor.CARREAU, Cart.cartNumber.VALET));
            _carts.Add(new Cart(Cart.cartColor.CARREAU, Cart.cartNumber.DAME));
            _carts.Add(new Cart(Cart.cartColor.CARREAU, Cart.cartNumber.ROI));
            _carts.Add(new Cart(Cart.cartColor.CARREAU, Cart.cartNumber.AS));
            _carts.Add(new Cart(Cart.cartColor.COEUR, Cart.cartNumber.SEPT));
            _carts.Add(new Cart(Cart.cartColor.COEUR, Cart.cartNumber.HUIT));
            _carts.Add(new Cart(Cart.cartColor.COEUR, Cart.cartNumber.NEUF));
            _carts.Add(new Cart(Cart.cartColor.COEUR, Cart.cartNumber.DIX));
            _carts.Add(new Cart(Cart.cartColor.COEUR, Cart.cartNumber.VALET));
            _carts.Add(new Cart(Cart.cartColor.COEUR, Cart.cartNumber.DAME));
            _carts.Add(new Cart(Cart.cartColor.COEUR, Cart.cartNumber.ROI));
            _carts.Add(new Cart(Cart.cartColor.COEUR, Cart.cartNumber.AS));
            _carts.Add(new Cart(Cart.cartColor.TREFLE, Cart.cartNumber.SEPT));
            _carts.Add(new Cart(Cart.cartColor.TREFLE, Cart.cartNumber.HUIT));
            _carts.Add(new Cart(Cart.cartColor.TREFLE, Cart.cartNumber.NEUF));
            _carts.Add(new Cart(Cart.cartColor.TREFLE, Cart.cartNumber.DIX));
            _carts.Add(new Cart(Cart.cartColor.TREFLE, Cart.cartNumber.VALET));
            _carts.Add(new Cart(Cart.cartColor.TREFLE, Cart.cartNumber.DAME));
            _carts.Add(new Cart(Cart.cartColor.TREFLE, Cart.cartNumber.ROI));
            _carts.Add(new Cart(Cart.cartColor.TREFLE, Cart.cartNumber.AS));
            _carts.Add(new Cart(Cart.cartColor.PIQUE, Cart.cartNumber.SEPT));
            _carts.Add(new Cart(Cart.cartColor.PIQUE, Cart.cartNumber.HUIT));
            _carts.Add(new Cart(Cart.cartColor.PIQUE, Cart.cartNumber.NEUF));
            _carts.Add(new Cart(Cart.cartColor.PIQUE, Cart.cartNumber.DIX));
            _carts.Add(new Cart(Cart.cartColor.PIQUE, Cart.cartNumber.VALET));
            _carts.Add(new Cart(Cart.cartColor.PIQUE, Cart.cartNumber.DAME));
            _carts.Add(new Cart(Cart.cartColor.PIQUE, Cart.cartNumber.ROI));
            _carts.Add(new Cart(Cart.cartColor.PIQUE, Cart.cartNumber.AS));
        }

        public void setAtout(Cart.cartColor atout)
        {
            foreach (Cart cart in _carts)
                cart.setAtout(atout);
            _atout = atout;
        }

        public Cart.cartColor getAtout()
        {
            return (_atout);
        }

        public void mixDeck()
        {
            Random rand = new Random();

            _carts = _carts.OrderBy(x => rand.Next()).ToList();
        }

        public List<Cart> getAndRemoveCarts(int number)
        {
            int i;
            List<Cart> tmpDeck = new List<Cart>();

            i = -1;
            while (++i < number)
            {
                tmpDeck.Add(_carts[Macro.INDEX_FIRST_CART]);
                _carts.Remove(_carts[Macro.INDEX_FIRST_CART]);
            }
            return (tmpDeck);
        }

        public List<Cart> getAllCarts()
        {
            return (_carts);
        }

        public void addMultipleCarts(List<Cart> newDeck)
        {
            _carts.AddRange(newDeck);
        }

        public void addOneCart(Cart cart)
        {
            _carts.Add(cart);
        }

        public void removeOneCart(Cart cart)
        {
            _carts.Remove(cart);
        }

        public void removeAllCart()
        {
            _carts.RemoveRange(0, _carts.Count);
        }

        public bool isEmpty()
        {
            return (!(_carts.Any()));
        }
    }
}
