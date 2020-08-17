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
    }
}
