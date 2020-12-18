﻿using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Enums.user_details;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_comments;
using NitelikliBilisim.Core.ViewModels.Main.EducationComment;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Business.Repositories
{
    public class EducationCommentRepository : BaseRepository<EducationComment, Guid>
    {
        private readonly NbDataContext _context;
        public EducationCommentRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public EducationCommentsVm GetEducationComments(Guid? categoryId, int? sortingType, int? pageIndex)
        {
            var sType = sortingType.HasValue ? (EducationCommentSortingTypes)sortingType : EducationCommentSortingTypes.Date;
            var retVal = new EducationCommentsVm();


            var rawdata = (from comment in _context.EducationComments
                           join education in _context.Educations on comment.EducationId equals education.Id
                           join category in _context.EducationCategories on education.CategoryId equals category.Id
                           join user in _context.Users on comment.CommentatorId equals user.Id
                           join student in _context.Customers on user.Id equals student.Id
                           where !comment.IsEducatorComment
                           && comment.ApprovalDate != null
                           && categoryId.HasValue ? category.Id == categoryId : true
                           select new EducationCommentListVm
                           {
                               CreatedDate = comment.CreatedDate,
                               Date = comment.CreatedDate.ToString("dd MMMM yyyy"),
                               Category = category.Name,
                               Content = comment.Content,
                               Point = comment.Points,
                               UserName = $"{user.Name} {user.Surname}",
                               AvatarPath = user.AvatarPath,
                               Job = student.Job
                           }).AsQueryable();
            switch (sType)
            {
                case EducationCommentSortingTypes.Date:
                    rawdata = rawdata.OrderByDescending(x => x.CreatedDate);
                    break;
                case EducationCommentSortingTypes.PointDescending:
                    rawdata = rawdata.OrderByDescending(x => x.Point);
                    break;
                case EducationCommentSortingTypes.Point:
                    rawdata = rawdata.OrderBy(x => x.Point);
                    break;
                default:
                    rawdata = rawdata.OrderByDescending(x => x.CreatedDate);
                    break;
            }
            pageIndex = pageIndex ?? 1;
            rawdata = rawdata.Skip((pageIndex.Value - 1) * 6).Take(6);
            retVal.TotalCount = rawdata.Count();
            retVal.PageIndex = pageIndex.Value;
            retVal.Comments = rawdata.ToList();

            return retVal;
        }

        public void ToggleHighlight(Guid commentId)
        {
            var comment = _context.EducationComments.First(x => x.Id == commentId);
            comment.IsHighLight = comment.IsHighLight?false:true;
            _context.SaveChanges();
        }

        public void SetCommentStatus(Guid commentId, CommentApprovalStatus status)
        {
           var comment = _context.EducationComments.First(x => x.Id == commentId);
            comment.ApprovalStatus = status;
            _context.SaveChanges();
        }

        public IQueryable<AdminEducationCommentListVm> GetEducationCommentsQueryable()
        {
            var data = from comment in _context.EducationComments
                       join user in _context.Users on comment.CommentatorId equals user.Id
                       join education in _context.Educations on comment.EducationId equals education.Id
                       select new AdminEducationCommentListVm
                       {
                           Id = comment.Id,
                           CreatedDate = comment.CreatedDate,
                           Point = comment.Points,
                           Content = comment.Content,
                           CommenterName = user.Name,
                           CommenterSurname = user.Surname,
                           CommenterId = user.Id,
                           CommenterEmail = user.Email,
                           EducationName = education.Name,
                           EducationId = education.Id,
                           ApprovalStatus = comment.ApprovalStatus,
                           ApprovalDate = comment.ApprovalDate,
                           IsHighlight = comment.IsHighLight
                       };
            return data;
        }
    }
}
