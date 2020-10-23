using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Areas.Admin.VmCreator.Educator;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums.user_details;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.areas.admin.educator;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MUsefulMethods;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class EducatorController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly FileUploadManager _fileManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EducatorVmCreator _vmCreator;
        private readonly IStorageService _storageService;

        public EducatorController(IWebHostEnvironment hostingEnvironment, UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IStorageService storageService)
        {
            _hostingEnvironment = hostingEnvironment;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _fileManager = new FileUploadManager(_hostingEnvironment, "jpg", "jpeg");
            _vmCreator = new EducatorVmCreator(_unitOfWork);
            _storageService = storageService;
        }
        [Route("admin/egitmen-ekle")]
        public IActionResult Add()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducatorAdd");
            var model = new AddGetVm
            {
                Certificates = _unitOfWork.EducatorCertificate.Get(null, order => order.OrderBy(x => x.Name)),
                BankNames = EnumHelpers.ToKeyValuePair<BankNames>()
            };
            return View(model);
        }

        [HttpPost, Route("admin/egitmen-ekle")]
        public async Task<IActionResult> Add(AddPostVm data)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelStateUtil.GetErrors(ModelState);
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = errors
                });
            }

            try
            {
                //var dbPath = _fileManager.Upload("/uploads/educator-photos/", data.ProfilePhoto.Base64Content, data.ProfilePhoto.Extension, "profile-photo", $"{data.Name} {data.Surname}");
                var stream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.ProfilePhoto.Base64Content));
                var fileName = StringHelpers.FormatForTag($"{data.Name} {data.Surname}");
                var dbPath = await _storageService.UploadFile(stream, $"{fileName}.{data.ProfilePhoto.Extension}", "educator-photos");
                var userName = StringHelpers.ConcatForUserName(data.Name, data.Surname);

                var count = _userManager.Users.Count(x => x.UserName.StartsWith(userName));
                var countText = count > 0 ? count.ToString() : "";
                var newUser = new ApplicationUser
                {
                    Name = data.Name,
                    Surname = data.Surname,
                    AvatarPath = dbPath,
                    Email = data.Email,
                    PhoneNumber = data.Phone,
                    UserName = $"{userName}{countText}"
                };
                // TODO: belirlenen şifre mail olarak atılmalı & sabit şifre değiştirilmeli
                var pwd = StringHelpers.RandomPasswordGenerator(10);
                var res = await _userManager.CreateAsync(newUser, "qwe123");
                if (!res.Succeeded)
                {
                    _fileManager.Delete(dbPath);

                    return Json(new ResponseModel
                    {
                        isSuccess = false,
                        errors = res.Errors.Select(x => x.Description)
                    });
                }

                res = await _userManager.AddToRoleAsync(newUser, "Educator");
                if (!res.Succeeded)
                {
                    _fileManager.Delete(dbPath);

                    return Json(new ResponseModel
                    {
                        isSuccess = false,
                        errors = res.Errors.Select(x => x.Description)
                    });
                }

                var newEducator = new Educator
                {
                    Id = newUser.Id,
                    Title = data.Title,
                    Biography = data.Biography,
                    ShortDescription = data.ShortDescription,
                    Bank = data.Bank,
                    IBAN = data.IBAN
                };
                _unitOfWork.Educator.Insert(newEducator, data.CertificateIds);

                if (data.SocialMedia != null)
                    _unitOfWork.EducatorSocialMedia.Insert(newEducator.Id, data.SocialMedia.Facebook, data.SocialMedia.Linkedin, data.SocialMedia.GooglePlus, data.SocialMedia.Twitter);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }

        [Route("admin/egitmenler")]
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducatorList");
            return View();
        }

        [Route("admin/get-educators")]
        public IActionResult GetEducators()
        {
            var educators = _vmCreator.GetEducators();
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = educators
            });
        }

        [Route("admin/get-educators-list")]
        public JsonResult GetEducatorslist()
        {
            var model = _vmCreator.GetEducators();
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }

        [Route("admin/egitmen-guncelle/{educatorId}")]
        public IActionResult Update(Guid? educatorId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducatorUpdate");
            if (!educatorId.HasValue)
                return Redirect("/admin/egitmenler");
            var educator = _unitOfWork.Educator.Get(x => x.Id == educatorId.ToString(), null, x => x.User).First();
            var model = new UpdateGetVm
            {
                Id = Guid.Parse(educator.Id),
                Title = educator.Title,
                Name = educator.User.Name,
                Surname = educator.User.Surname,
                Phone = educator.User.PhoneNumber,
                Email = educator.User.Email,
                FilePath = educator.User.AvatarPath,
                Biography = educator.Biography,
                ShortDescription = educator.ShortDescription,
                Bank = educator.Bank,
                IBAN = educator.IBAN,
                Certificates = _unitOfWork.EducatorCertificate.Get(null, o => o.OrderBy(x => x.Name)),
                RelatedCertificates = _unitOfWork.Educator.GetCertificates(educator.Id),
                BankNames = EnumHelpers.ToKeyValuePair<BankNames>(),

            };
            return View(model);
        }

        [HttpPost, Route("admin/egitmen-guncelle")]
        public async Task<IActionResult> Update(UpdatePostNewVm data)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelStateUtil.GetErrors(ModelState);
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = errors
                });
            }

            var educator = _unitOfWork.Educator.Get(x => x.Id == data.EducatorId.ToString(), null, x => x.User).First();


            if (!string.IsNullOrEmpty(data.ProfilePhoto.Base64Content))
            {
                var stream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.ProfilePhoto.Base64Content));
                var fileName = StringHelpers.FormatForTag($"{data.Name} {data.Surname}");
                var dbPath = await _storageService.UploadFile(stream, $"{fileName}.{data.ProfilePhoto.Extension}", "educator-photos");
                var userName = StringHelpers.ConcatForUserName(data.Name, data.Surname);
                educator.User.AvatarPath = dbPath;
            }

            educator.Title = data.Title;
            educator.Biography = data.Biography;
            educator.ShortDescription = data.ShortDescription;
            educator.User.Name = data.Name;
            educator.User.Surname = data.Surname;
            educator.User.PhoneNumber = data.Phone;
            educator.User.Email = data.Email;
            educator.Bank = data.Bank;
            educator.IBAN = data.IBAN;
            //Test 
            _unitOfWork.Educator.Update(educator, data.CertificateIds);

            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Eğitmen başarıyla güncellenmiştir"
            });
        }

        [Route("admin/egitmen-sosyal-medya-guncelle/{educatorId}")]
        public IActionResult UpdateEducatorSocialMedia(Guid? educatorId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducatorUpdateEducatorSocialMedia");
            if (!educatorId.HasValue)
                return Redirect("/admin/egitmenler");
            var educatorSocialMedias = _unitOfWork.EducatorSocialMedia.Get(x => x.EducatorId == educatorId.ToString(), null, x => x.Educator).ToList();
            var model = new UpdateGetEducatorSocialMediaVm
            {
                Id = Guid.Parse(educatorId.ToString()),
                Facebook = educatorSocialMedias.FirstOrDefault(x => x.SocialMediaType == Core.Enums.EducatorSocialMediaType.Facebook)?.Link,
                Linkedin = educatorSocialMedias.FirstOrDefault(x => x.SocialMediaType == Core.Enums.EducatorSocialMediaType.LinkedIn)?.Link,
                GooglePlus = educatorSocialMedias.FirstOrDefault(x => x.SocialMediaType == Core.Enums.EducatorSocialMediaType.GooglePlus)?.Link,
                Twitter = educatorSocialMedias.FirstOrDefault(x => x.SocialMediaType == Core.Enums.EducatorSocialMediaType.Twitter)?.Link
            };
            return View(model);
        }

        [HttpPost, Route("admin/egitmen-sosyal-medya-guncelle")]
        public IActionResult UpdateEducatorSocialMedia(UpdatePostEducatorSocialMediaVm data)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelStateUtil.GetErrors(ModelState);
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = errors
                });
            }

            if (!string.IsNullOrEmpty(data.Facebook))
            {
                var facebook = _unitOfWork.EducatorSocialMedia.Get().FirstOrDefault(x => x.EducatorId == data.EducatorId &&
                x.SocialMediaType == Core.Enums.EducatorSocialMediaType.Facebook);

                if (facebook != null)
                {
                    facebook.Link = data.Facebook;
                    _unitOfWork.EducatorSocialMedia.Update(facebook);
                }
                else
                {
                    _unitOfWork.EducatorSocialMedia.Insert(new EducatorSocialMedia()
                    {
                        EducatorId = data.EducatorId,
                        SocialMediaType = Core.Enums.EducatorSocialMediaType.Facebook,
                        Link = data.Facebook
                    });
                }
            }
            else
            {
                var facebook = _unitOfWork.EducatorSocialMedia.Get().FirstOrDefault(x => x.EducatorId == data.EducatorId &&
                x.SocialMediaType == Core.Enums.EducatorSocialMediaType.Facebook);

                if (facebook != null)
                    _unitOfWork.EducatorSocialMedia.Delete(facebook);
            }

            if (!string.IsNullOrEmpty(data.Linkedin))
            {
                var linkedin = _unitOfWork.EducatorSocialMedia.Get().FirstOrDefault(x => x.EducatorId == data.EducatorId &&
                x.SocialMediaType == Core.Enums.EducatorSocialMediaType.LinkedIn);

                if (linkedin != null)
                {
                    linkedin.Link = data.Linkedin;
                    _unitOfWork.EducatorSocialMedia.Update(linkedin);
                }
                else
                {
                    _unitOfWork.EducatorSocialMedia.Insert(new EducatorSocialMedia()
                    {
                        EducatorId = data.EducatorId,
                        SocialMediaType = Core.Enums.EducatorSocialMediaType.LinkedIn,
                        Link = data.Linkedin
                    });
                }
            }
            else
            {
                var linkedin = _unitOfWork.EducatorSocialMedia.Get().FirstOrDefault(x => x.EducatorId == data.EducatorId &&
               x.SocialMediaType == Core.Enums.EducatorSocialMediaType.LinkedIn);

                if (linkedin != null)
                    _unitOfWork.EducatorSocialMedia.Delete(linkedin);
            }

            if (!string.IsNullOrEmpty(data.GooglePlus))
            {
                var googlePlus = _unitOfWork.EducatorSocialMedia.Get().FirstOrDefault(x => x.EducatorId == data.EducatorId &&
                x.SocialMediaType == Core.Enums.EducatorSocialMediaType.GooglePlus);

                if (googlePlus != null)
                {
                    googlePlus.Link = data.GooglePlus;
                    _unitOfWork.EducatorSocialMedia.Update(googlePlus);
                }
                else
                {
                    _unitOfWork.EducatorSocialMedia.Insert(new EducatorSocialMedia()
                    {
                        EducatorId = data.EducatorId,
                        SocialMediaType = Core.Enums.EducatorSocialMediaType.GooglePlus,
                        Link = data.GooglePlus
                    });
                }
            }
            else
            {
                var googlePlus = _unitOfWork.EducatorSocialMedia.Get().FirstOrDefault(x => x.EducatorId == data.EducatorId &&
                x.SocialMediaType == Core.Enums.EducatorSocialMediaType.GooglePlus);

                if (googlePlus != null)
                    _unitOfWork.EducatorSocialMedia.Delete(googlePlus);
            }

            if (!string.IsNullOrEmpty(data.Twitter))
            {
                var twitter = _unitOfWork.EducatorSocialMedia.Get().FirstOrDefault(x => x.EducatorId == data.EducatorId &&
                x.SocialMediaType == Core.Enums.EducatorSocialMediaType.Twitter);

                if (twitter != null)
                {
                    twitter.Link = data.Twitter;
                    _unitOfWork.EducatorSocialMedia.Update(twitter);
                }
                else
                {
                    _unitOfWork.EducatorSocialMedia.Insert(new EducatorSocialMedia()
                    {
                        EducatorId = data.EducatorId,
                        SocialMediaType = Core.Enums.EducatorSocialMediaType.Twitter,
                        Link = data.Twitter
                    });
                }
            }
            else
            {
                var twitter = _unitOfWork.EducatorSocialMedia.Get().FirstOrDefault(x => x.EducatorId == data.EducatorId &&
               x.SocialMediaType == Core.Enums.EducatorSocialMediaType.Twitter);

                if (twitter != null)
                    _unitOfWork.EducatorSocialMedia.Delete(twitter);
            }

            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Eğitmenin Sosyal Medya Profilleri başarıyla güncellenmiştir"
            });
        }

        [Route("admin/delete-educator")]
        public IActionResult Delete(Guid educatorId)
        {
            if (educatorId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    message = "Silinecek veri bulunamadı"
                });

            _unitOfWork.Educator.Delete(educatorId.ToString());

            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Silme işlemi başarılı"
            });
        }
    }
}