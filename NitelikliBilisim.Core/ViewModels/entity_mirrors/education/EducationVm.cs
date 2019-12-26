﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels
{
    public class EducationVm
    {
        public EducationBaseVm Base { get; set; }
        public List<EducationMediaVm> Medias { get; set; }
        public List<EducationPartVm> Parts { get; set; }
        public List<EducationGainVm> Gains { get; set; }
    }
    public class EducationBaseVm
    {
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public decimal PriceNumeric { get; set; }
        public string PriceText { get; set; }
        public byte DaysNumeric { get; set; }
        public string DaysText { get; set; }
        public byte HoursPerDayNumeric { get; set; }
        public string HoursPerDayText { get; set; }
        public string Level { get; set; }
    }
    public class EducationMediaVm
    {
        public Guid EducationId { get; set; }
        public string FileUrl { get; set; }
    }
    public class EducationPartVm
    {
        public Guid EducationId { get; set; }
        public string Title { get; set; }
        public byte Duration { get; set; }
        public byte Order { get; set; }
    }
    public class EducationGainVm
    {
        public Guid EducationId { get; set; }
        public string Gain { get; set; }
    }
}
