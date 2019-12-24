using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Business.PagedEntity;
using NitelikliBilisim.Core.DTO;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.education;
using NitelikliBilisim.Data;
using NitelikliBilisim.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationRepository : BaseRepository<Education, Guid>, IPageableEntity<Education>
    {
        public EducationRepository(NbDataContext context) : base(context)
        {
        }

        public PagedEntity<Education> GetPagedEntity(int page = 0, Expression<Func<Education, bool>> filter = null, int shownRecords = 15)
        {
            IQueryable<Education> dbSet = base._table;
            if (filter != null)
                dbSet = dbSet.Where(filter);

            return new PagedEntity<Education>
            {
                Data = dbSet.OrderBy(o => o.Name)
                    .Skip(page * shownRecords)
                    .Take(shownRecords)
                    .AsNoTracking()
                    .ToList(),
                Count = dbSet.Count()
            };
        }

        public ListGetVm GetPagedEducations(int page = 0, int shownRecords = 15)
        {
            var educations = _table
                .OrderBy(o => o.Name)
                .Skip(page * shownRecords)
                .Take(shownRecords);

            var mediaCount = educations.Join(_context.EducationMedias, l => l.Id, r => r.EducationId, (x, y) => new
            {
                EducationId = x.Id,
                MediaId = y.Id
            })
            .GroupBy(g => g.EducationId)
            .Select(x => new _EducationSub
            {
                EducationId = x.Key,
                Count = x.Count()
            }).ToList();
            var partCount = educations.Join(_context.EducationParts, l => l.Id, r => r.EducationId, (x, y) => new
            {
                EducationId = x.Id,
                PartId = y.Id
            })
            .GroupBy(g => g.EducationId)
            .Select(x => new _EducationSub
            {
                EducationId = x.Key,
                Count = x.Count()
            }).ToList();
            var gainCount = educations.Join(_context.EducationGains, l => l.Id, r => r.EducationId, (x, y) => new
            {
                EducationId = x.Id,
                GainId = y.Id
            })
            .GroupBy(x => x.EducationId)
            .Select(x => new _EducationSub
            {
                EducationId = x.Key,
                Count = x.Count()
            }).ToList();

            var categories = educations.Join(_context.Bridge_EducationTags, l => l.Id, r => r.Id2, (x, y) => new
            {
                EducationId = x.Id,
                CategoryId = y.Id
            }).Join(_context.EducationTags, l => l.CategoryId, r => r.Id, (x, y) => new
            {
                EducationId = x.EducationId,
                Category = y
            })
            .GroupBy(g => g.EducationId)
            .Select(x => new
            {
                EducationId = x.Key,
                Data = x.ToList()
            });

            var educationCategories = new List<_EducationCategory>();
            foreach (var item in categories)
            {
                var concattedCategories = item.Data.Count != 0 ? "" : null;
                for (int i = 0; i < item.Data.Count; i++)
                {
                    if (i != item.Data.Count - 1)
                        concattedCategories += $"{item.Data[i].Category.Name}, ";
                    else
                        concattedCategories += item.Data[i].Category.Name;
                }

                educationCategories.Add(new _EducationCategory { EducationId = item.EducationId, ConcattedCategories = concattedCategories });
            }

            var educationDtos = new List<EducationDto>();
            //var mapper = new Mapper.Mapper<Education, EducationDto>();
            //foreach (var item in educations)
            //    educationDtos.Add(mapper.Map(item));

            foreach (var item in educations)
                educationDtos.Add(new EducationDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Days = item.Days,
                    HoursPerDay = item.HoursPerDay,
                    Level = EnumSupport.GetDescription(item.Level),
                    NewPrice = item.NewPrice,
                    IsActive = item.IsActive
                });

            return new ListGetVm
            {
                Educations = educationDtos,
                Medias = mediaCount,
                Parts = partCount,
                Gains = gainCount,
                EducationCategories = educationCategories
            };
        }

        public Guid? Insert(Education entity, List<Guid> categoryIds, List<EducationMedia> medias, bool isSaveLater = false)
        {
            if (categoryIds == null || categoryIds.Count == 0)
                return null;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    entity.IsActive = false;
                    var educationId = base.Insert(entity);

                    var bridge = new List<Bridge_EducationTag>();
                    foreach (var categoryId in categoryIds)
                        bridge.Add(new Bridge_EducationTag
                        {
                            Id = categoryId,
                            Id2 = educationId
                        });
                    foreach (var item in medias)
                        item.EducationId = educationId;

                    _context.EducationMedias.AddRange(medias);
                    _context.Bridge_EducationTags.AddRange(bridge);
                    _context.SaveChanges();
                    transaction.Commit();
                    return educationId;
                }
                catch
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }

        public List<EducationTag> GetTags(Guid educationId)
        {
            var data = _context.Bridge_EducationTags.Where(x => x.Id2 == educationId)
                .Join(_context.EducationTags, l => l.Id, r => r.Id, (x, y) => new
                {
                    Category = y
                }).ToList();
            var categories = new List<EducationTag>();
            foreach (var item in data)
                categories.Add(new EducationTag
                {
                    Id = item.Category.Id,
                    BaseTagId = item.Category.BaseTagId,
                    Description = item.Category.Description,
                    Name = item.Category.Name
                });

            return categories;
        }

        public bool CheckEducationState(Guid educationId)
        {
            var mediaCount = _context.EducationMedias.Count(x => x.EducationId == educationId &&
            (x.MediaType == Core.Enums.EducationMediaType.Banner ||
            x.MediaType == Core.Enums.EducationMediaType.PreviewPhoto ||
            x.MediaType == Core.Enums.EducationMediaType.PreviewVideo));

            var partCount = _context.EducationParts.Count(x => x.EducationId == educationId);

            var gainCount = _context.EducationGains.Count(x => x.EducationId == educationId);

            var categoryCount = _context.Bridge_EducationTags.Count(x => x.Id2 == educationId);

            var education = _context.Educations.First(x => x.Id == educationId);

            education.IsActive = mediaCount >= 2 && partCount > 0 && gainCount > 0 && categoryCount > 0;

            _context.SaveChanges();

            return education.IsActive;
        }

        public int Update(Education entity, List<Guid> categoryIds, bool isSaveLater = false)
        {
            var currentCategories = _context.Bridge_EducationTags.Where(x => x.Id2 == entity.Id).ToList();

            _context.Bridge_EducationTags.RemoveRange(currentCategories);

            var newItems = new List<Bridge_EducationTag>();
            foreach (var categoryId in categoryIds)
                newItems.Add(new Bridge_EducationTag
                {
                    Id = categoryId,
                    Id2 = entity.Id
                });

            _context.Bridge_EducationTags.AddRange(newItems);

            return base.Update(entity, isSaveLater);
        }
    }
}
