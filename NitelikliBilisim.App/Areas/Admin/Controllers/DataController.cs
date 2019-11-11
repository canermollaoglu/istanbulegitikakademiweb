//using System;
//using System.IO;
//using DevExtreme.AspNet.Data;
//using Microsoft.AspNetCore.Mvc;
//using NitelikliBilisim.App.Extensions;
//using NitelikliBilisim.Core.Entities;
//using NitelikliBilisim.Core.Repositories;
//using System.Linq;
//using System.Threading.Tasks;
//using DevExtreme.AspNet.Data.ResponseModel;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using NitelikliBilisim.Core.ComplexTypes;
//using NitelikliBilisim.Core.Entities.Identity;
//using NitelikliBilisim.Core.Enums;
//using NitelikliBilisim.Core.Services;

//namespace NitelikliBilisim.App.Areas.Admin.Controllers
//{
//    [Route("api/[controller]/[action]")]
//    [ApiController]
//    public class DataController : Controller
//    {
//        private readonly IRepository<Kategori, Guid> _kategoryRepo;
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly RoleManager<ApplicationRole> _roleManager;

//        public DataController(IRepository<Kategori, Guid> kategoryRepo, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
//        {
//            _kategoryRepo = kategoryRepo;
//            _userManager = userManager;
//            _roleManager = roleManager;
//        }

//        [HttpGet]
//        public LoadResult GetKategoriData(DataSourceLoadOptions loadOptions)
//        {
//            loadOptions.PrimaryKey = new[] { "Id" };

//            var data = _kategoryRepo.Get().OrderBy(x => x.CreatedDate);
//            return DataSourceLoader.Load(data, loadOptions);
//        }

//        [HttpPost]
//        public IActionResult InsertKategori([FromForm] string values)
//        {
//            var item = new Kategori();
//            JsonConvert.PopulateObject(values, item);
//            if (!TryValidateModel(item))
//                return BadRequest(ModelState.ToFullErrorString());

//            _kategoryRepo.Add(item);
//            return Json(item.Id);
//        }

//        [HttpDelete]
//        public IActionResult DeleteKategori([FromForm] Guid key)
//        {
//            var item = _kategoryRepo.GetById(key);
//            if (item == null)
//                return StatusCode(409, "Kayit bulunamadi");

//            _kategoryRepo.Delete(item);
//            if (!string.IsNullOrEmpty(item.KategoriFoto))
//            {
//                System.IO.File.Delete(Path.Combine("wwwroot/" + item.KategoriFoto));
//            }
//            return Ok();
//        }

//        [HttpPut]
//        public IActionResult UpdateKategori([FromForm] Guid key, [FromForm] string values)
//        {
//            var item = _kategoryRepo.GetById(key);
//            if (item == null)
//                return StatusCode(409, "Kayit bulunamadi");
//            JsonConvert.PopulateObject(values, item);
//            if (!TryValidateModel(item))
//                return BadRequest(ModelState.ToFullErrorString());
//            _kategoryRepo.Update(item);
//            return Ok("Güncelleştirme işlemi başarılı");
//        }

//        [HttpPost]
//        public IActionResult UploadKategoriFoto(Guid id)
//        {
//            if (this.Request.Form.Files.Any() && this.Request.ContentLength > 0)
//            {
//                try
//                {
//                    var data = _kategoryRepo.GetById(id);
//                    if (data == null)
//                        return BadRequest("Kategori bulunamadı");

//                    var file = this.Request.Form.Files.First();
//                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
//                    string extName = Path.GetExtension(file.FileName);
//                    fileName = StringHelper.UrlFormatConverter(fileName) + StringHelper.GenerateUniqueCode();
//                    var klasoryolu = Path.Combine("wwwroot/upload/");
//                    var dosyayolu = Path.Combine("wwwroot/upload/") + fileName + extName;
//                    if (!Directory.Exists(klasoryolu))
//                        Directory.CreateDirectory(klasoryolu);
//                    var eskiFotoYolu = data.KategoriFoto;
//                    data.KategoriFoto = dosyayolu.Replace("wwwroot/", "");
//                    _kategoryRepo.Update(data, true);
//                    using (var fileStream = new FileStream(dosyayolu, FileMode.Create))
//                    {
//                        file.CopyTo(fileStream);
//                    }

//                    if (!string.IsNullOrEmpty(eskiFotoYolu))
//                    {
//                        System.IO.File.Delete(Path.Combine("wwwroot/" + eskiFotoYolu));
//                    }
//                    _kategoryRepo.Save();
//                }
//                catch (Exception ex)
//                {
//                    return BadRequest(ex.Message);
//                }
//            }

//            return Ok("İşlem başarılı");
//        }

//        [HttpGet]
//        public LoadResult GetUserData(DataSourceLoadOptions loadOptions)
//        {
//            loadOptions.PrimaryKey = new[] { "Id" };

//            var user = _userManager.FindByNameAsync("mesut").Result;
//            if (!_userManager.IsInRoleAsync(user, IdentityRoleList.User.ToString()).Result)
//            {
//                var result = _userManager.AddToRoleAsync(user, IdentityRoleList.User.ToString()).Result;
//            }

//            var data = _userManager.Users
//                .Include(x => x.Egitici)
//                .Include(x => x.UserRoles)
//                .ThenInclude(x => x.Role)
//                .OrderBy(x => x.Name)
//                .Select(x => new UserViewModel
//                {
//                    Id = x.Id,
//                    Name = x.Name,
//                    Surname = x.Surname,
//                    UserName = x.UserName,
//                    Roles = x.UserRoles.Select(r => new RoleModel { Id = r.Role.Id, Name = r.Role.Name }),
//                    Title = x.Egitici != null ? x.Egitici.Title : "",
//                    FotoUrl = x.FotoUrl
//                });

//            return DataSourceLoader.Load(data.Include(x=>x.Roles), loadOptions);
//        }
//        [HttpGet]
//        public LoadResult RolesLookup(DataSourceLoadOptions options)
//        {
//            return DataSourceLoader.Load(
//                from r in _roleManager.Roles
//                orderby r.Name
//                select new
//                {
//                    Id = r.Id,
//                    Name = r.Name
//                },
//                options
//            );
//        }
//        [HttpPut]
//        public IActionResult UpdateUser([FromForm] string key, [FromForm] string values)
//        {
//            var item = _userManager.FindByIdAsync(key).Result;
//            if (item == null)
//                return StatusCode(409, "Kayit bulunamadi");
//            JsonConvert.PopulateObject(values, item);
//            if (!TryValidateModel(item))
//                return BadRequest(ModelState.ToFullErrorString());

//            return Ok("Güncelleştirme işlemi başarılı");
//        }


//    }
//}