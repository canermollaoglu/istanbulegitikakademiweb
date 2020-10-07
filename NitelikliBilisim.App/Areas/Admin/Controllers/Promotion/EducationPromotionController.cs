﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_promotion;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.Promotion
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class EducationPromotionController : Controller
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
        [HttpGet]
        [Route("admin/promosyon-olustur")]
        public IActionResult Add()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationPromotionAdd");
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
                Id=data.Id,
                StartDate = data.StartDate,
                EndDate = data.EndDate,
                Description = data.Description,
                DiscountAmount = data.DiscountAmount,
                UserBasedUsageLimit = data.UserBasedUsageLimit,
                MinBasketAmount = data.MinBasketAmount,
                PromotionCode = data.PromotionCode,
                MaxUsageLimit = data.MaxUsageLimit,
                Name = data.Name
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
                _unitOfWork.EducationPromotionCode.Insert(new EducationPromotionCode
                {
                    Name = data.Name,
                    Description = data.Description,
                    DiscountAmount = data.DiscountAmount,
                    StartDate = data.StartDate,
                    EndDate = data.EndDate,
                    UserBasedUsageLimit = data.UserBasedUsageLimit,
                    MaxUsageLimit = data.MaxUsageLimit,
                    MinBasketAmount = data.MinBasketAmount,
                    PromotionCode = data.PromotionCode 
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
                        errors = new List<string> {"Promosyon kullanımları olduğu için silinemiyor."}

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


    }
}
