using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.areas.admin.reports;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class ReportController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        public ReportController(UnitOfWork unitOfWork)
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
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminGroupBasedSalesReport");
            var model = new GroupBasedSalesReportGetVm
            {
                AllGroups = _unitOfWork.EducationGroup.GetAllGroupsDictionary()
            };
            return View(model);
        }

        [Route("raporlar/grup-bazli-satis-raporu2")]
        public IActionResult GroupBasedSalesReportByGroupId(Guid groupId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminGroupBasedSalesReport");
            ViewData["GroupId"] = groupId;
            return View();
        }

        [Route("raporlar/genel-satis-raporu")]
        public IActionResult SalesReport()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminGeneralSalesReport");
            return View();
        }

        [Route("raporlar/grup-bazli-satis-raporu-excel-export")]
        public IActionResult GroupBasedSalesReportExport(Guid groupId)
        {
            var generalInformation = _unitOfWork.EducationGroup.GetDetailByGroupId(groupId);
            var studentList = _unitOfWork.Report.GetGroupBasedSalesReportStudents(groupId).ToList();
            var cancellationList = _unitOfWork.Report.GetGroupBasedCancellationSalesReport(groupId).ToList();
            var groupExpenses = _unitOfWork.EducationGroup.GetExpensesByGroupId(groupId);
            var groupExpenseAndIncome = _unitOfWork.EducationGroup.CalculateGroupExpenseAndIncome(groupId);
            string baseUrl = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Request.PathBase);

            //Excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var pck = new ExcelPackage())
            {
                #region Genel Bilgiler ve Öğrenci Listesi
                var worksheet = pck.Workbook.Worksheets.Add($"Özet");
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
                using (var rng = worksheet.Cells[1, 1, 1, 2])
                {
                    rng.Style.Font.Name = "Calibri";
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Size = 14;
                    rng.Merge = true;
                    rng.Value = "Grup Genel Bilgileri";
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                using (var rng = worksheet.Cells[1, 4, 1, 5])
                {
                    rng.Style.Font.Name = "Calibri";
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Size = 14;
                    rng.Merge = true;
                    rng.Value = "Bütçe";
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                using (var rng = worksheet.Cells[2, 1, 12, 1])
                {
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Top.Color.SetColor(Color.Black);
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Color.SetColor(Color.Black);
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Color.SetColor(Color.Black);
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.Red);
                    rng.Style.Font.Size = 12;
                }
                using (var rng = worksheet.Cells[2, 4, 9, 4])
                {
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Top.Color.SetColor(Color.Black);
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Color.SetColor(Color.Black);
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Color.SetColor(Color.Black);
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.Red);
                    rng.Style.Font.Size = 12;
                }
                using (var rng = worksheet.Cells[1, 1, 12, 2])
                {
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }
                using (var rng = worksheet.Cells[1, 4, 9, 5])
                {
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }


                //Genel Bilgiler
                worksheet.Cells[2, 1].Value = "Grup Adı";
                worksheet.Cells[3, 1].Value = "Eğitim Yeri";
                worksheet.Cells[4, 1].Value = "Eğitim";
                worksheet.Cells[5, 1].Value = "Sınıf";
                worksheet.Cells[6, 1].Value = "Eğitim Günleri";
                worksheet.Cells[7, 1].Value = "Eğitmen";
                worksheet.Cells[8, 1].Value = "Başlangıç Tarihi";
                worksheet.Cells[9, 1].Value = "Bitiş Tarihi";
                worksheet.Cells[10, 1].Value = "Kontenjan";
                worksheet.Cells[11, 1].Value = "Eğitim Gün/Saat";
                worksheet.Cells[12, 1].Value = "Satış Fiyatı";

                worksheet.Cells[2, 2].Formula = "HYPERLINK(\"" + baseUrl + "/admin/grup-detay/" + groupId + "\",\"" + generalInformation.GroupName + "\")";
                worksheet.Cells[3, 2].Value = generalInformation.Host.HostName;
                worksheet.Cells[4, 2].Value = generalInformation.Education.Name;
                worksheet.Cells[5, 2].Value = generalInformation.ClassRoomName;
                worksheet.Cells[6, 2].Value = String.Join(", ", generalInformation.WeekdayNames);
                worksheet.Cells[7, 2].Value = generalInformation.EducatorName;
                worksheet.Cells[8, 2].Value = generalInformation.StartDate;
                worksheet.Cells[9, 2].Value = generalInformation.EndDate;
                worksheet.Cells[10, 2].Value = generalInformation.Quota.ToString();
                worksheet.Cells[11, 2].Value = $"{generalInformation.EducationDays} gün / günde {generalInformation.EducationHoursPerDay} saat";
                worksheet.Cells[12, 2].Value = generalInformation.NewPrice.GetValueOrDefault().ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));




                worksheet.Cells[2, 4].Value = "Ciro";
                worksheet.Cells[3, 4].Value = "Grup Giderleri";
                worksheet.Cells[4, 4].Value = "Eğitmen Ücreti Toplamı";
                worksheet.Cells[5, 4].Value = "Pos Komisyonu";
                worksheet.Cells[6, 4].Value = "K.D.V.";
                worksheet.Cells[7, 4].Value = "Toplam Gider";
                worksheet.Cells[8, 4].Value = "Genel Toplam";
                worksheet.Cells[9, 4].Value = "Kâr Oranı";

                worksheet.Cells[2, 5].Value = groupExpenseAndIncome.TotalStudentIncomes;
                worksheet.Cells[3, 5].Value = groupExpenseAndIncome.GroupExpenses;
                worksheet.Cells[4, 5].Value = groupExpenseAndIncome.EducatorExpenses;
                worksheet.Cells[5, 5].Value = groupExpenseAndIncome.TotalPosCommissionAmount;
                worksheet.Cells[6, 5].Value = groupExpenseAndIncome.KDV;
                worksheet.Cells[7, 5].Value = groupExpenseAndIncome.TotalExpenses;
                worksheet.Cells[8, 5].Value = groupExpenseAndIncome.GrandTotal;
                worksheet.Cells[9, 5].Value = $"%{groupExpenseAndIncome.ProfitRate}";

                worksheet.Column(1).Width = 25;
                worksheet.Column(2).Width = 35;
                worksheet.Column(3).Width = 5;
                worksheet.Column(4).Width = 25;
                worksheet.Column(5).Width = 13;
                #endregion

                #region Öğrenci Listesi
                var worksheetStudentList = pck.Workbook.Worksheets.Add($"Öğrenci Listesi");
                int studentListCount = studentList.Count();
                using (var rng = worksheetStudentList.Cells[studentListCount + 3, 3, studentListCount + 3, 7])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Size = 12;
                }

                using (var rng = worksheetStudentList.Cells[1, 1, 1, 7])
                {
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Top.Color.SetColor(Color.Black);
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Color.SetColor(Color.Black);
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Color.SetColor(Color.Black);
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Size = 12;
                }

                worksheetStudentList.Cells[1, 1].Value = "Satış Tarihi";
                worksheetStudentList.Cells[1, 2].Value = "Adı Soyadı";
                worksheetStudentList.Cells[1, 3].Value = "Liste Fiyatı";
                worksheetStudentList.Cells[1, 4].Value = "Ödenen";
                worksheetStudentList.Cells[1, 5].Value = "İşlem Ücreti";
                worksheetStudentList.Cells[1, 6].Value = "Komisyon";
                worksheetStudentList.Cells[1, 7].Value = "Kalan";
                for (int i = 0; i < studentList.Count; i++)
                {
                    int rowIndex = i + 2;
                    var current = studentList[i];
                    worksheetStudentList.Cells[rowIndex, 1].Value = current.PaymentDate.ToShortDateString();
                    worksheetStudentList.Cells[rowIndex, 2].Formula = "HYPERLINK(\"" + baseUrl + "/admin/ogrenci-detay?studentId=" + current.Id + "\",\"" + $"{current.Name} {current.Surname}" + "\")";
                    worksheetStudentList.Cells[rowIndex, 3].Value = current.ListPrice.Value.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));
                    worksheetStudentList.Cells[rowIndex, 4].Value = current.PaidPrice.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));
                    worksheetStudentList.Cells[rowIndex, 5].Value = current.CommissionFee.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));
                    worksheetStudentList.Cells[rowIndex, 6].Value = current.CommissionRate.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));
                    worksheetStudentList.Cells[rowIndex, 7].Value = current.MerchantPayout.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));
                }
                //TOPLAM SATIRI
                worksheetStudentList.Cells[studentListCount + 3, 3].Value = studentList.Sum(x => x.ListPrice).GetValueOrDefault().ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));
                worksheetStudentList.Cells[studentListCount + 3, 4].Value = studentList.Sum(x => x.PaidPrice).ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));
                worksheetStudentList.Cells[studentListCount + 3, 5].Value = studentList.Sum(x => x.CommissionFee).ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));
                worksheetStudentList.Cells[studentListCount + 3, 6].Value = studentList.Sum(x => x.CommissionRate).ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));
                worksheetStudentList.Cells[studentListCount + 3, 7].Value = studentList.Sum(x => x.MerchantPayout).ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));


                worksheetStudentList.Column(1).Width = 18;
                worksheetStudentList.Column(2).Width = 45;
                worksheetStudentList.Column(3).Width = 25;
                worksheetStudentList.Column(4).Width = 18;
                worksheetStudentList.Column(5).Width = 18;
                worksheetStudentList.Column(6).Width = 15;
                worksheetStudentList.Column(7).Width = 20;

                #endregion
                #region İptal İade Listesi
                if (cancellationList.Count > 0)
                {

                    var worksheetCancellationList = pck.Workbook.Worksheets.Add($"İptal/İade Listesi");
                    using (var rng = worksheetCancellationList.Cells[1, 1, 1, 5])
                    {
                        rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Top.Color.SetColor(Color.Black);
                        rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Left.Color.SetColor(Color.Black);
                        rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Right.Color.SetColor(Color.Black);
                        rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                        rng.Style.Font.Bold = true;
                        rng.Style.Font.Size = 12;
                    }
                    using (var rng = worksheetCancellationList.Cells[cancellationList.Count + 3, 3, cancellationList.Count + 3, 5])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Font.Size = 12;
                    }

                    worksheetCancellationList.Cells[1, 1].Value = "İptal Tarihi";
                    worksheetCancellationList.Cells[1, 2].Value = "Adı Soyadı";
                    worksheetCancellationList.Cells[1, 3].Value = "Liste Fiyatı";
                    worksheetCancellationList.Cells[1, 4].Value = "Satış Fiyatı";
                    worksheetCancellationList.Cells[1, 5].Value = "İade Tutarı";
                    for (int i = 0; i < cancellationList.Count; i++)
                    {
                        int rowIndex = i + 2;
                        var current = cancellationList[i];
                        worksheetCancellationList.Cells[rowIndex, 1].Value = current.CancellationDate.Value.ToShortDateString();
                        worksheetCancellationList.Cells[rowIndex, 2].Formula = "HYPERLINK(\"" + baseUrl + "/admin/ogrenci-detay?studentId=" + current.Id + "\",\"" + $"{current.Name} {current.Surname}" + "\")";
                        worksheetCancellationList.Cells[rowIndex, 3].Value = current.ListPrice.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));
                        worksheetCancellationList.Cells[rowIndex, 4].Value = current.PaidPrice.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));
                        worksheetCancellationList.Cells[rowIndex, 5].Value = current.RefundPrice.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));

                    }

                    //TOPLAM SATIRI
                    worksheetCancellationList.Cells[cancellationList.Count + 3, 3].Value = cancellationList.Sum(x => x.ListPrice).ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));
                    worksheetCancellationList.Cells[cancellationList.Count + 3, 4].Value = cancellationList.Sum(x => x.PaidPrice).ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));
                    worksheetCancellationList.Cells[cancellationList.Count + 3, 5].Value = cancellationList.Sum(x => x.RefundPrice).ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));


                    worksheetCancellationList.Column(1).Width = 18;
                    worksheetCancellationList.Column(2).Width = 45;
                    worksheetCancellationList.Column(3).Width = 25;
                    worksheetCancellationList.Column(4).Width = 18;
                    worksheetCancellationList.Column(5).Width = 18;

                }
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
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Top.Color.SetColor(Color.Black);
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Color.SetColor(Color.Black);
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Color.SetColor(Color.Black);
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Color.SetColor(Color.Black);
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Size = 12;
                }
                using (var rng = worksheet2.Cells[groupExpenses.Count + 3, 6, groupExpenses.Count + 3, 6])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Size = 12;
                }
                worksheet2.Cells[1, 1].Value = "Tarih";
                worksheet2.Cells[1, 2].Value = "Gider Tipi";
                worksheet2.Cells[1, 3].Value = "Açıklama";
                worksheet2.Cells[1, 4].Value = "Adet";
                worksheet2.Cells[1, 5].Value = "Fiyat";
                worksheet2.Cells[1, 6].Value = "Toplam Tutar";

                for (int i = 0; i < groupExpenses.Count; i++)
                {
                    int rowIndex = i + 2;
                    var current = groupExpenses[i];
                    worksheet2.Cells[rowIndex, 1].Value = current.CreatedDate.ToShortDateString();
                    worksheet2.Cells[rowIndex, 2].Value = current.ExpenseTypeName;
                    worksheet2.Cells[rowIndex, 3].Value = current.Description;
                    worksheet2.Cells[rowIndex, 4].Value = current.Count;
                    worksheet2.Cells[rowIndex, 5].Value = current.Price.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));
                    worksheet2.Cells[rowIndex, 6].Value = current.TotalPrice.ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));
                }

                //TOPLAM SATIRI
                worksheet2.Cells[groupExpenses.Count + 3, 6].Value = groupExpenses.Sum(x => x.TotalPrice).ToString("C", CultureInfo.CreateSpecificCulture("tr-TR"));

                worksheet2.Column(1).Width = 18;
                worksheet2.Column(2).Width = 28;
                worksheet2.Column(3).Width = 28;
                worksheet2.Column(4).Width = 7;
                worksheet2.Column(5).Width = 15;
                worksheet2.Column(6).Width = 15;

                #endregion


                var fileBytes = pck.GetAsByteArray();
                var fileName = generalInformation.GroupName + "-GrupBazliSatisRaporu" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10) + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
        }

        [Route("raporlar/grup-bazli-satis-raporu-egitmen-ucret-tablosu")]
        public IActionResult GetGroupBasedSalesReportEducatorSalaryTable(Guid groupId)
        {
            try
            {
                var model = _unitOfWork.Report.GetGroupBasedSalesReportEducatorSalaryTable(groupId);
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
                    errors = new List<string> { $"Hata : {ex.Message}" }
                });
            }


        }

        [Route("raporlar/grup-takip-raporu")]
        public IActionResult GroupFollowUpReport()
        {
            return View();
        }


        public IActionResult NonGroupEducations()
        {
            var educations = _unitOfWork.Education.GetNonGroupEducations();
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = educations
            });
        }
        public IActionResult GetGroupFollowUpReportData() {

            var data = _unitOfWork.Report.GetGroupFollowUpReportData();
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = data
            });
        }

    }
}
