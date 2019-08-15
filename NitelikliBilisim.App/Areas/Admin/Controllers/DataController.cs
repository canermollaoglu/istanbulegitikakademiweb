using System;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Repositories;
using System.Linq;
using DevExtreme.AspNet.Data.ResponseModel;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DataController : Controller
    {
        private readonly IRepository<Kategori, Guid> _kategoryRepo;

        public DataController(IRepository<Kategori, Guid> kategoryRepo)
        {
            _kategoryRepo = kategoryRepo;
            if (_kategoryRepo.GetAll().Count()<100)
            {
                _kategoryRepo.Add(new Kategori()
                {
                    Ad = "Test Kategori"+new Random().Next(),
                    Aciklama = "Test açıklama"+new Random().Next()
                });
            }
        }

        public LoadResult GetKategoriData(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] {"Id"};

            var data = _kategoryRepo.GetAll().OrderBy(x=>x.CreatedDate);
            return DataSourceLoader.Load(data, loadOptions);
        }
    }
}