
using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Core.Enums.user_details;
using NitelikliBilisim.Core.ViewModels.Main.InvoiceInfo;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class AddressRepository : BaseRepository<Address, int>
    {
        private readonly NbDataContext _context;
        public AddressRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public List<InvoiceInfoAddressGetVm> GetInvoiceAddressesByUserId(string userId)
        {
            return _context.Addresses
                .Include(x => x.State)
                .Include(x => x.City)
                .Where(x => x.CustomerId == userId).Select(x => new InvoiceInfoAddressGetVm
                {
                    Id = x.Id,
                    Title = x.Title,
                    Content = x.Content,
                    City = x.City.Name,
                    State = x.State.Name,
                    IsDefault = x.IsDefaultAddress,
                    AddressType = x.AddressType
                }).ToList();
        }

        public void AddCorporateAddress(AddCorporateAddressPostVm model)
        {
            var otherAddresses = _context.Addresses.Where(x => x.CustomerId == model.CustomerId);
            foreach (var otherAddress in otherAddresses)
            {
                otherAddress.IsDefaultAddress = false;
            }
            var address = new Address
            {
                Title = model.Title,
                Content = model.Content,
                PhoneNumber = model.PhoneNumber,
                CityId = model.CityId,
                StateId = model.StateId,
                CompanyName = model.CompanyName,
                TaxOffice = model.TaxOffice,
                TaxNumber = model.TaxNumber,
                CustomerId = model.CustomerId,
                IsDefaultAddress = model.IsDefaultAddress,
                AddressType = AddressTypes.Corporate
            };
            _context.Addresses.Add(address);
            _context.SaveChanges();

        }
        public void AddIndividualAddress(AddIndividualAddressPostVm model)
        {
            var otherAddresses = _context.Addresses.Where(x => x.CustomerId == model.CustomerId);
            foreach (var otherAddress in otherAddresses)
            {
                otherAddress.IsDefaultAddress = false;
            }
            var address = new Address
            {
                Title = model.Title,
                Content = model.Content,
                PhoneNumber = model.PhoneNumber,
                NameSurname = model.NameSurname,
                CityId = model.CityId,
                StateId = model.StateId,
                IdentityNumber = model.IdentityNumber,
                CustomerId = model.CustomerId,
                IsDefaultAddress = model.IsDefaultAddress,
                AddressType = AddressTypes.Individual
            };
            _context.Addresses.Add(address);
            _context.SaveChanges();
        }

        public Address GetDefaultAddress(string userId)
        {
            var retVal = _context.Addresses.FirstOrDefault(x => x.IsDefaultAddress && x.CustomerId == userId);
            return retVal;
        }

        public Address GetFullAddressById(int id)
        {
          return _context.Addresses.Include(x=>x.City).Include(x=>x.State).FirstOrDefault(x => x.Id == id);
        }
    }
}
