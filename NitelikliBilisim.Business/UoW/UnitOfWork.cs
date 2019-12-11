using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Business.Repositories;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NbDataContext _context;
        private EducationCategoryRepository _educationCategory;
        private EducationRepository _education;
        private EducationMediaItemRepository _educationMediaItem;
        public UnitOfWork(NbDataContext context)
        {
            _context = context;
        }
        public int Save()
        {
            _context.EnsureAutoHistory();
            return _context.SaveChanges();
        }
        public EducationCategoryRepository EducationCategory
        {
            get
            {
                return _educationCategory ?? (_educationCategory = new EducationCategoryRepository(_context));
            }
        }
        public EducationRepository Education
        {
            get
            {
                return _education ?? (_education = new EducationRepository(_context));
            }
        }
        public EducationMediaItemRepository EducationMedia
        {
            get
            {
                return _educationMediaItem ?? (_educationMediaItem = new EducationMediaItemRepository(_context));
            }
        }
    }
}
