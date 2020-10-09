using Newtonsoft.Json;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.PaymentModels;
using NitelikliBilisim.Data;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class TempSaleDataRepository
    {
        private readonly NbDataContext _context;
        public TempSaleDataRepository(NbDataContext context)
        {
            _context = context;
        }

        public void Create(string conversationId, PaymentModelSuccess successModel,string promotionId,string userId)
        {
            var data = JsonConvert.SerializeObject(new
            {
                invoiceId = successModel.InvoiceId,
                invoiceDetailIds = successModel.InvoiceDetailIds,
                promotionId,
                userId
            });

            // sepette çok fazla ürün olursa exception fırlabilir
            if (data.Length > 450)
                return;

            _context.TempSaleData.Add(new TempSaleData
            {
                Id = conversationId,
                Data = data
            });

            _context.SaveChanges();
        }
        public PaymentModelSuccess Get(string conversationId)
        {
            var data = _context.TempSaleData.First(x => x.Id == conversationId);
            return JsonConvert.DeserializeObject<PaymentModelSuccess>(data.Data);
        }
        public void Remove(string conversationId)
        {
            var data = _context.TempSaleData.FirstOrDefault(x => x.Id == conversationId);
            if (data == null)
                return;
            _context.TempSaleData.Remove(data);
            _context.SaveChanges();
        }
    }
}
