using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.Core.ViewModels
{
    public class SuggestedEducationVm : EducationVm
    {
        /// <summary>
        /// Uygun kriter adedi
        /// </summary>
        public int AppropriateCriterionCount { get; set; }
    }
    public class EducationVm
    {
        public EducationBaseVm Base { get; set; }
        public List<EducationMediaVm> Medias { get; set; }
        public List<EducationPartVm> Parts { get; set; }
        public List<EducationGainVm> Gains { get; set; }
        public int TotalPartCount { get; set; }
        public int TotalDuration { get; set; }
    }
    public class EducationBaseVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public decimal PriceNumeric { get; set; }
        public string PriceText { get; set; }
        public byte DaysNumeric { get; set; }
        public string DaysText { get; set; }
        public byte HoursPerDayNumeric { get; set; }
        public string HoursPerDayText { get; set; }
        public string Level { get; set; }
        public string StartDateText { get; set; }
    }

    public class EducationListVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
        public byte HoursPerDay { get; set; }
        public byte Days { get; set; }
        public bool isActive { get; set; }

    }
    public class EducationMediaVm
    {
        public Guid EducationId { get; set; }
        public string FileUrl { get; set; }
        public EducationMediaType MediaType { get; set; }
    }
    public class EducationPartVm
    {
        public Guid Id { get; set; }
        public Guid EducationId { get; set; }
        public Guid? BasePartId { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public byte Order { get; set; }
        public List<EducationPartVm> SubParts { get; set; }
    }
    public class EducationGainVm
    {
        public Guid EducationId { get; set; }
        public string Gain { get; set; }
    }
}
