using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class WishListRepository :BaseRepository<WishlistItem,string>
    {
        private readonly NbDataContext _context;
        public WishListRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }


        public void Delete(string userId, Guid educationId)
        {
            var wishListItem = _context.Wishlist.First(x => x.Id == userId && x.Id2 == educationId);
            _context.Wishlist.Remove(wishListItem);
            _context.SaveChanges();
        }

        /// <summary>
        /// Eğitim favorilere ekli ise çıkarır yoksa ekler. 
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Eklendiyse true döner.</returns>
        public bool ToggleWishListItem(WishlistItem item)
        {
            var retVal = false;
            var wlItem = _context.Wishlist.FirstOrDefault(x => x.Id == item.Id && x.Id2 == item.Id2);
            if (wlItem ==null)
            {
                _context.Wishlist.Add(item);
                retVal = true;
            }
            else
            {
                _context.Wishlist.Remove(wlItem);
            }
            _context.SaveChanges();
            return retVal;
        }

        public bool CheckWishListItem(string userId, Guid educationId)
        {
            var wlItem = _context.Wishlist.FirstOrDefault(x => x.Id == userId && x.Id2 == educationId);
            return wlItem == null ? false : true;
        }
    }
}
