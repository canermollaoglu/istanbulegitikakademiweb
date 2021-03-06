using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using Newtonsoft.Json;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.promotion;
using NitelikliBilisim.Core.Enums.promotion;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_promotion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.Promotion
{
    public class EducationPromotionController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationPromotionController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Route("admin/promosyonlar")]
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationPromotionList");
            return View();
        }
        [Route("admin/sepet-bazli-promosyonlar")]
        public IActionResult BasketBasedPromotionList()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationPromotionList");
            return View();
        }

        [HttpGet]
        [Route("admin/promosyon-olustur")]
        public IActionResult Add()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationPromotionAdd");
            ViewData["PromotionTypes"] = EnumHelpers.ToKeyValuePair<PromotionType>();
            return View();
        }


        [HttpGet]
        [Route("admin/promosyon-guncelle")]
        public IActionResult Update(Guid promotionId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationPromotionUpdate");
            var data = _unitOfWork.EducationPromotionCode.GetById(promotionId);
            var model = new EducationPromotionUpdateGetVm
            {
                Id = data.Id,
                StartDate = data.StartDate,
                EndDate = data.EndDate,
                Description = data.Description,
                DiscountAmount = data.DiscountAmount,
                UserBasedUsageLimit = data.UserBasedUsageLimit,
                MinBasketAmount = data.MinBasketAmount,
                PromotionCode = data.PromotionCode,
                MaxUsageLimit = data.MaxUsageLimit,
                Name = data.Name,
                PromotionType = data.PromotionType
            };
            return View(model);
        }


        [HttpPost]
        [Route("admin/promosyon-olustur")]
        public IActionResult Add(EducationPromotionAddVm data)
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
                var promotionId = _unitOfWork.EducationPromotionCode.Insert(new EducationPromotionCode
                {
                    Name = data.Name,
                    Description = data.Description,
                    DiscountAmount = data.DiscountAmount,
                    StartDate = data.StartDate,
                    EndDate = data.EndDate,
                    UserBasedUsageLimit = data.UserBasedUsageLimit,
                    MaxUsageLimit = data.MaxUsageLimit,
                    MinBasketAmount = data.MinBasketAmount,
                    PromotionCode = data.PromotionCode,
                    PromotionType =data.PromotionType
                });

                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = promotionId
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata " + ex.Message }

                });
            }

        }

        [HttpPost]
        [Route("admin/promosyon-guncelle")]
        public IActionResult Update(EducationPromotionUpdatePostVm data)
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
                var promotion = _unitOfWork.EducationPromotionCode.GetById(data.Id);
                promotion.Name = data.Name;
                promotion.Description = data.Description;
                promotion.StartDate = data.StartDate;
                promotion.EndDate = data.EndDate;
                promotion.DiscountAmount = data.DiscountAmount;
                promotion.MaxUsageLimit = data.MaxUsageLimit;
                promotion.MinBasketAmount = data.MinBasketAmount;
                promotion.UserBasedUsageLimit = data.UserBasedUsageLimit;
                promotion.PromotionCode = data.PromotionCode;
                _unitOfWork.EducationPromotionCode.Update(promotion);

                return Json(new ResponseModel
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata " + ex.Message }

                });
            }

        }


        [Route("admin/promosyon-sil")]
        public IActionResult Delete(Guid promotionId)
        {
            try
            {
                var isItemContains = _unitOfWork.EducationPromotionCode.CheckThePromotionItem(promotionId);
                if (isItemContains)
                {
                    return Json(new ResponseModel
                    {
                        isSuccess = false,
                        errors = new List<string> { "Promosyon kullanımları olduğu için silinemiyor." }

                    });
                }
                _unitOfWork.EducationPromotionCode.Delete(promotionId);
                return Json(new ResponseModel
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata " + ex.Message }

                });
            }


        }

        [Route("admin/promosyon-kodu-olustur")]
        public IActionResult CreatePromotionCode()
        {
            bool isUnique = false;
            var code = string.Empty;
            while (!isUnique)
            {
                code = StringHelpers.GenerateUpperCode(7);
                isUnique = IsUniquePromotionCode(code);
            }

            return Json(new ResponseModel
            {
                isSuccess = true,
                data = code
            });
        }


        [Route("admin/promosyon-kod-dogrula")]
        public IActionResult CheckPromotionCode(string promotionCode)
        {
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = IsUniquePromotionCode(promotionCode)
            });

        }


        [Route("admin/promosyon-kosul")]
        public IActionResult PromotionConditionManagement(Guid promotionId,int pT)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationPromotionConditionManagement");
            ViewData["PromotionId"] = promotionId;
            ViewData["PromotionType"] = pT;
            ViewData["ConditionTypes"] = EnumHelpers.ToKeyValuePair<ConditionType>();
            return View();
        }

        public IActionResult GetPromotionConditions(Guid promotionId)
        {
            try
            {
                List<EducationPromotionGetListVm> model = new List<EducationPromotionGetListVm>();
                var conditions = _unitOfWork.EducationPromotionCondition.Get(x => x.EducationPromotionCodeId == promotionId).ToList();
                foreach (var condition in conditions)
                {
                    List<string> values = new List<string>();
                    if (condition.ConditionType == ConditionType.Category || condition.ConditionType == ConditionType.PurchasedCategory)
                    {
                        var categories = _unitOfWork.EducationCategory.Get().ToList();
                        var ids = JsonConvert.DeserializeObject<Guid[]>(condition.ConditionValue);
                        foreach (var id in ids)
                        {
                            values.Add(categories.First(x => x.Id == id).Name);
                        }
                        model.Add(new EducationPromotionGetListVm()
                        {
                            Id = condition.Id,
                            ConditionType = EnumHelpers.GetDescription(condition.ConditionType),
                            ConditionValues = values
                        });
                    }
                    else if (condition.ConditionType == ConditionType.Education || condition.ConditionType == ConditionType.PurchasedEducation)
                    {
                        var educations = _unitOfWork.Education.Get().ToList();
                        var ids = JsonConvert.DeserializeObject<Guid[]>(condition.ConditionValue);
                        foreach (var id in ids)
                        {
                            values.Add(educations.First(x => x.Id == id).Name);
                        }
                        model.Add(new EducationPromotionGetListVm()
                        {
                            Id = condition.Id,
                            ConditionType = EnumHelpers.GetDescription(condition.ConditionType),
                            ConditionValues = values
                        });
                    }
                    else if (condition.ConditionType == ConditionType.User)
                    {
                        var users = _unitOfWork.Customer.GetCustomerListQueryable().ToList();
                        var ids = JsonConvert.DeserializeObject<string[]>(condition.ConditionValue);
                        foreach (var id in ids)
                        {
                            var user = users.FirstOrDefault(x => x.Id == id);
                            if (user != null)
                                values.Add($"{user.Name} {user.Surname}");
                        }
                        model.Add(new EducationPromotionGetListVm()
                        {
                            Id = condition.Id,
                            ConditionType = EnumHelpers.GetDescription(condition.ConditionType),
                            ConditionValues = values
                        });
                    }
                }
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = model
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata :" + ex.Message }
                });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPromotionCondition(EducationPromotionConditionAddVm data)
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
                _unitOfWork.EducationPromotionCondition.Insert(new EducationPromotionCondition
                {
                    ConditionType = data.ConditionType,
                    ConditionValue = JsonConvert.SerializeObject(data.MultipleValue),
                    EducationPromotionCodeId = data.PromotionId
                });

                return Json(new ResponseModel
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata " + ex.Message }

                });
            }

        }

        [HttpGet]
        [Route("admin/promosyon-kosul-sil")]
        public IActionResult DeletePromotionCondition(Guid promotionConditionId)
        {
            try
            {
                _unitOfWork.EducationPromotionCondition.Delete(promotionConditionId);
                return Json(new ResponseModel
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata " + ex.Message }

                });
            }
        }

        /// <summary>
        /// Promosyon kodu Db ye daha önce kaydolmamış ise true döner
        /// </summary>
        /// <param name="promotionCode"></param>
        /// <returns></returns>
        public bool IsUniquePromotionCode(string promotionCode)
        {
            var code = _unitOfWork.EducationPromotionCode.GetPromotionbyPromotionCode(promotionCode);
            if (code == null)
                return true;
            else
                return false;
        }
    }
}
