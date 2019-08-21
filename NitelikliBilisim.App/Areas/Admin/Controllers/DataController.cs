using System;
using System.IO;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Repositories;
using System.Linq;
using DevExtreme.AspNet.Data.ResponseModel;
using Newtonsoft.Json;
using NitelikliBilisim.Core.Services;

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

            var data = _kategoryRepo.Get().OrderBy(x => x.CreatedDate);
            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpPost]
        public IActionResult InsertKategori([FromForm] string values)
        {
            var item = new Kategori();
            JsonConvert.PopulateObject(values, item);
            if (!TryValidateModel(item))
                return BadRequest(ModelState.ToFullErrorString());

            _kategoryRepo.Add(item);
            return Json(item.Id);
        }

        [HttpDelete]
        public IActionResult DeleteKategori([FromForm] Guid key)
        {
            var item = _kategoryRepo.GetById(key);
            if (item == null)
                return StatusCode(409, "Kayit bulunamadi");

            _kategoryRepo.Delete(item);
            if (!string.IsNullOrEmpty(item.KategoriFoto))
            {
                System.IO.File.Delete(Path.Combine("wwwroot/"+item.KategoriFoto));
            }
            return Ok();
        }

        [HttpPut]
        public IActionResult UpdateKategori([FromForm] Guid key, [FromForm] string values)
        {
            var item = _kategoryRepo.GetById(key);
            if (item == null)
                return StatusCode(409, "Kayit bulunamadi");
            JsonConvert.PopulateObject(values, item);
            if (!TryValidateModel(item))
                return BadRequest(ModelState.ToFullErrorString());
            _kategoryRepo.Update(item);
            return Ok("Güncelleştirme işlemi başarılı");
        }

        [HttpPost]
        public IActionResult UploadKategoriFoto(Guid id)
        {
            if (this.Request.Form.Files.Any() && this.Request.ContentLength > 0)
            {
                try
                {
                    var data = _kategoryRepo.GetById(id);
                    if (data == null)
                        return BadRequest("Kategori bulunamadı");

                    var file = this.Request.Form.Files.First();
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string extName = Path.GetExtension(file.FileName);
                    fileName = StringHelper.UrlFormatConverter(fileName) + StringHelper.GenerateUniqueCode();
                    var klasoryolu = Path.Combine("wwwroot/upload/");
                    var dosyayolu = Path.Combine("wwwroot/upload/") + fileName + extName;
                    if (!Directory.Exists(klasoryolu))
                        Directory.CreateDirectory(klasoryolu);
                    var eskiFotoYolu = data.KategoriFoto;
                    data.KategoriFoto = dosyayolu.Replace("wwwroot/","");
                    _kategoryRepo.Update(data, true);
                    using (var fileStream = new FileStream(dosyayolu, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    if (!string.IsNullOrEmpty(eskiFotoYolu))
                    {
                        System.IO.File.Delete(Path.Combine("wwwroot/"+eskiFotoYolu));
                    }
                    _kategoryRepo.Save();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return Ok("İşlem başarılı");
        }
    }
}