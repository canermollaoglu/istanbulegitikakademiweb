using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.areas.admin.reports;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Drawing;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class EarningsReportController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public EarningsReportController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Route("raporlar/satis")]
        public IActionResult IncomeReport()
        {
            return View();
        }

        [Route("fetch-income-report/{year?}/{month?}")]
        public IActionResult FetchIncomeReport(int? year, int? month)
        {
            if (year == null || month == null)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });
            var model = _unitOfWork.Report.FetchIncomeReport(year.Value, month.Value);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }

        [Route("raporlar/grup-bazli-satis-raporu")]
        public IActionResult GroupBasedSalesReport()
        {
            var model = new GroupBasedSalesReportGetVm
            {
                AllGroups = _unitOfWork.EducationGroup.GetAllGroupsDictionary()
            };
            return View(model);
        }
        [Route("raporlar/grup-bazli-satis-raporu-excel-export")]
        public IActionResult GroupBasedSalesReportExport(Guid groupId)
        {
            var studentList = _unitOfWork.EducationGroup.GetGroupBasedSalesReportStudentsToList(groupId);
            var generalInformation = _unitOfWork.EducationGroup.GetGroupGeneralInformation(groupId);
            var groupExpenses = _unitOfWork.EducationGroup.GetExpensesByGroupId(groupId);
            var groupExpenseAndIncome = _unitOfWork.EducationGroup.CalculateGroupExpenseAndIncome(groupId);
            //Excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var pck = new ExcelPackage())
            {
                #region Genel Bilgiler ve Öğrenci Listesi
                var worksheet = pck.Workbook.Worksheets.Add($"Öğrenci Listesi");
                worksheet.PrinterSettings.Orientation = eOrientation.Portrait;
                worksheet.PrinterSettings.PaperSize = ePaperSize.A4;
                worksheet.PrinterSettings.FitToPage = true;
                worksheet.PrinterSettings.HeaderMargin = 0;
                worksheet.PrinterSettings.FooterMargin = 0;
                worksheet.PrinterSettings.BottomMargin = 0;
                worksheet.PrinterSettings.TopMargin = 0.65m;
                worksheet.PrinterSettings.RightMargin = 0;
                worksheet.PrinterSettings.LeftMargin = 0;
                //Stil
                using (var rng = worksheet.Cells[1, 1, 1, 6])
                {
                    rng.Style.Font.Name = "Calibri";
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Size = 14;
                    rng.Merge = true;
                    rng.Value = "Grup Genel Bilgileri";
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Top.Color.SetColor(Color.Black);
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Color.SetColor(Color.Black);
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Color.SetColor(Color.Black);
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                }

                using (var rng = worksheet.Cells[1, 9, 1, 10])
                {
                    rng.Style.Font.Name = "Calibri";
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Size = 14;
                    rng.Merge = true;
                    rng.Value = "Bütçe";
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Top.Color.SetColor(Color.Black);
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Color.SetColor(Color.Black);
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Color.SetColor(Color.Black);
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                }
                using (var rng = worksheet.Cells[5, 1, 5, 6])
                {
                    rng.Style.Font.Name = "Calibri";
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Size = 14;
                    rng.Merge = true;
                    rng.Value = "Kayıtlı Öğrenci Ödeme Listesi";
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Top.Color.SetColor(Color.Black);
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Color.SetColor(Color.Black);
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Color.SetColor(Color.Black);
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                }
                using (var rng = worksheet.Cells[2, 1, 2, 6])
                {
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    rng.Style.Border.Top.Color.SetColor(Color.Black);
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    rng.Style.Border.Left.Color.SetColor(Color.Black);
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    rng.Style.Border.Right.Color.SetColor(Color.Black);
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Size = 12;
                }
                using (var rng = worksheet.Cells[2, 9, 5, 10])
                {
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    rng.Style.Border.Top.Color.SetColor(Color.Black);
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    rng.Style.Border.Left.Color.SetColor(Color.Black);
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    rng.Style.Border.Right.Color.SetColor(Color.Black);
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Size = 12;
                }
                using (var rng = worksheet.Cells[6, 1, 6, 6])
                {
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    rng.Style.Border.Top.Color.SetColor(Color.Black);
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    rng.Style.Border.Left.Color.SetColor(Color.Black);
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    rng.Style.Border.Right.Color.SetColor(Color.Black);
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Size = 12;
                }

                //Genel Bilgiler
                worksheet.Cells[2, 1].Value = "Grup Adı";
                worksheet.Cells[2, 2].Value = "Eğitim";
                worksheet.Cells[2, 3].Value = "Eğitim Yeri";
                worksheet.Cells[2, 4].Value = "Başlangıç Tarihi";
                worksheet.Cells[2, 5].Value = "Eğitmen";
                worksheet.Cells[2, 6].Value = "Kontenjan";
                worksheet.Cells[3, 1].Value = generalInformation.GroupName;
                worksheet.Cells[3, 2].Value = generalInformation.EducationName;
                worksheet.Cells[3, 3].Value = $"{generalInformation.EducationHost} - {generalInformation.Classroom}";
                worksheet.Cells[3, 4].Value = generalInformation.StartDate;
                worksheet.Cells[3, 5].Value = generalInformation.EducatorName;
                worksheet.Cells[3, 6].Value = $"{generalInformation.AssignedStudentsCount}/{generalInformation.Quota}";


                worksheet.Cells[2, 9].Value = "Grup Giderleri";
                worksheet.Cells[3, 9].Value = "Eğitmen Ücreti";
                worksheet.Cells[4, 9].Value = "Toplam Gelir(Ciro)";
                worksheet.Cells[5, 9].Value = "Genel Toplam";
                worksheet.Cells[2, 10].Value = groupExpenseAndIncome.GroupExpenses;
                worksheet.Cells[3, 10].Value = groupExpenseAndIncome.EducatorExpenses;
                worksheet.Cells[4, 10].Value = groupExpenseAndIncome.TotalStudentIncomes;
                worksheet.Cells[5, 10].Value = groupExpenseAndIncome.GrandTotal;


                //Öğrenci Listesi Başlıklar
                worksheet.Cells[6, 1].Value = "Kayıt Tarihi";
                worksheet.Cells[6, 2].Value = "Adı Soyadı";
                worksheet.Cells[6, 3].Value = "Ödenen";
                worksheet.Cells[6, 4].Value = "İşlem Ücreti";
                worksheet.Cells[6, 5].Value = "Komisyon";
                worksheet.Cells[6, 6].Value = "Kalan";
                for (int i = 0; i < studentList.Count; i++)
                {
                    int rowIndex = i + 7;
                    var current = studentList[i];
                    worksheet.Cells[rowIndex, 1].Value = current.RegistrationDate.ToShortDateString();
                    worksheet.Cells[rowIndex, 2].Value = $"{current.Name} {current.Surname}";
                    worksheet.Cells[rowIndex, 3].Value = current.PaidPrice;
                    worksheet.Cells[rowIndex, 4].Value = current.CommissionFee;
                    worksheet.Cells[rowIndex, 5].Value = current.CommissionRate;
                    worksheet.Cells[rowIndex, 6].Value = current.MerchantPayout;
                }
                worksheet.Column(1).Width = 18;
                worksheet.Column(2).Width = 45;
                worksheet.Column(3).Width = 25;
                worksheet.Column(4).Width = 18;
                worksheet.Column(5).Width = 18;
                worksheet.Column(6).Width = 15;
                worksheet.Column(9).Width = 19;
                worksheet.Column(10).Width = 8;

                #endregion

                #region Grup Giderleri
                var worksheet2 = pck.Workbook.Worksheets.Add($"Grup Giderleri");
                worksheet2.PrinterSettings.Orientation = eOrientation.Portrait;
                worksheet2.PrinterSettings.PaperSize = ePaperSize.A4;
                worksheet2.PrinterSettings.FitToPage = true;
                worksheet2.PrinterSettings.HeaderMargin = 0;
                worksheet2.PrinterSettings.FooterMargin = 0;
                worksheet2.PrinterSettings.BottomMargin = 0;
                worksheet2.PrinterSettings.TopMargin = 0.65m;
                worksheet2.PrinterSettings.RightMargin = 0;
                worksheet2.PrinterSettings.LeftMargin = 0;
                //Stil
                using (var rng = worksheet2.Cells[1, 1, 1, 6])
                {
                    rng.Style.Font.Name = "Calibri";
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Size = 14;
                    rng.Merge = true;
                    rng.Value = "Grup Giderleri";
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Top.Color.SetColor(Color.Black);
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Color.SetColor(Color.Black);
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Color.SetColor(Color.Black);
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                }
                using (var rng = worksheet2.Cells[2, 1, 2, 6])
                {
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    rng.Style.Border.Top.Color.SetColor(Color.Black);
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    rng.Style.Border.Left.Color.SetColor(Color.Black);
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    rng.Style.Border.Right.Color.SetColor(Color.Black);
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Size = 12;
                }
                worksheet2.Cells[2, 1].Value = "Tarih";
                worksheet2.Cells[2, 2].Value = "Gider Tipi";
                worksheet2.Cells[2, 3].Value = "Açıklama";
                worksheet2.Cells[2, 4].Value = "Adet";
                worksheet2.Cells[2, 5].Value = "Fiyat";
                worksheet2.Cells[2, 6].Value = "Toplam Tutar";

                for (int i = 0; i < groupExpenses.Count; i++)
                {
                    int rowIndex = i + 3;
                    var current = groupExpenses[i];
                    worksheet2.Cells[rowIndex, 1].Value = current.CreatedDate.ToShortDateString();
                    worksheet2.Cells[rowIndex, 2].Value = current.ExpenseTypeName;
                    worksheet2.Cells[rowIndex, 3].Value = current.Description;
                    worksheet2.Cells[rowIndex, 4].Value = current.Count;
                    worksheet2.Cells[rowIndex, 5].Value = current.Price;
                    worksheet2.Cells[rowIndex, 6].Value = current.TotalPrice;
                }
                worksheet2.Column(1).Width = 18;
                worksheet2.Column(2).Width = 28;
                worksheet2.Column(3).Width = 28;
                worksheet2.Column(4).Width = 7;
                worksheet2.Column(5).Width = 7;
                worksheet2.Column(6).Width = 15;

                #endregion


                var fileBytes = pck.GetAsByteArray();
                var fileName = generalInformation.GroupName+"-GrupBazliSatisRaporu" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10) + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
        }
    }
}
