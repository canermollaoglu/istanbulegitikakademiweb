using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NitelikliBilisim.Core.Entities.user_details;
using NitelikliBilisim.Core.ViewModels.Main.Profile;
using NitelikliBilisim.Data;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Barcode;
using Syncfusion.Pdf.Graphics;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories
{
    public class CustomerCertificateRepository:BaseRepository<CustomerCertificate,Guid>
    {
        private readonly NbDataContext _context;
       public CustomerCertificateRepository(NbDataContext context):base(context)
        {
            _context = context;
        }

        
    }

}
