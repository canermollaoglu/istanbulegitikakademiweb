using System;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Repositories;
using System.Linq;
using DevExtreme.AspNet.Data.ResponseModel;
using Newtonsoft.Json;

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
        }

        [HttpGet]
        public LoadResult GetKategoriData(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] {"Id"};

            var data = _kategoryRepo.GetAll().OrderBy(x => x.CreatedDate);
            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpPost]
        public IActionResult InsertKategori([FromForm]string values)
        {
            var item = new Kategori();
            JsonConvert.PopulateObject(values,item);
            if(!TryValidateModel(item))
                return BadRequest(ModelState.ToFullErrorString());

            _kategoryRepo.Add(item);
            return Json(item.Id);
        }

        [HttpDelete]
        public IActionResult DeleteKategori([FromForm]Guid key)
        {
            var item = _kategoryRepo.GetById(key);
            if(item == null) 
                return StatusCode(409, "Kayit bulunamadi");

            _kategoryRepo.Delete(item);
            return Ok();
        }

        [HttpPut]
        public IActionResult UpdateKategori([FromForm] Guid key, [FromForm]string values)
        {
            var item = _kategoryRepo.GetById(key);
            if(item == null)
                return StatusCode(409, "Kayit bulunamadi");
            JsonConvert.PopulateObject(values,item);
            if(!TryValidateModel(item))
                return BadRequest(ModelState.ToFullErrorString());
            _kategoryRepo.Update(item);
            return Ok("Güncelleştirme işlemi başarılı");
        }
    }
}