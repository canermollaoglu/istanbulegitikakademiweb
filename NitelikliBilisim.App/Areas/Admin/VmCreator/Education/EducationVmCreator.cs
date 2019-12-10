﻿using NitelikliBilisim.App.Areas.Admin.Models.Education;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.VmCreator.Education
{
    public class EducationVmCreator
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationVmCreator(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public AddGetVm CreateAddGetVm()
        {
            var categories = _unitOfWork.EducationCategory.Get(null, x => x.OrderBy(o => o.Name));
            var levels = EnumSupport.ToKeyValuePair<EducationLevel>();
            return new AddGetVm
            {
                Categories = categories,
                Levels = levels
            };
        }
    }
}
