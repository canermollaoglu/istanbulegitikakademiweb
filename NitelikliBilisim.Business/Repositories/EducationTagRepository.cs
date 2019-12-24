﻿using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationTagRepository : BaseRepository<EducationTag, Guid>
    {
        public EducationTagRepository(NbDataContext context) : base(context)
        {
        }
    }
}