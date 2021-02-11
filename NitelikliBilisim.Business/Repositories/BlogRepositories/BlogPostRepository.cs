using Microsoft.EntityFrameworkCore;
using MUsefulMethods;
using Nest;
using NitelikliBilisim.Core.Entities.blog;
using NitelikliBilisim.Core.ESOptions.ESEntities;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.Main.Blog;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories.BlogRepositories
{
    public class BlogPostRepository : BaseRepository<BlogPost, Guid>
    {
        NbDataContext _context;
        private readonly IElasticClient _elasticClient;
        public BlogPostRepository(NbDataContext context,IElasticClient elasticClient) : base(context)
        {
            _context = context;
            _elasticClient = elasticClient;
        }

        public BlogPost GetByIdWithCategory(Guid postId)
        {
            return _context.BlogPosts.Include(x => x.Category).First(x => x.Id == postId);
        }

        public List<RecommendedBlogPostVm> GetUserRecommendedBlogPosts(Guid currentBlogPostId)
        {
            var data = (from blog in _context.BlogPosts
                        join blogCategory in _context.BlogCategories on blog.CategoryId equals blogCategory.Id
                        where blog.Id != currentBlogPostId
                        select new RecommendedBlogPostVm
                        {
                            Id = blog.Id,
                            Title = blog.Title,
                            SeoUrl = blog.SeoUrl,
                            CategorySeoUrl = blogCategory.SeoUrl,
                            Content = blog.SummaryContent,
                            CreatedDate = blog.CreatedDate.ToShortDateString(),
                            Date = blog.CreatedDate,
                            Category = blogCategory.Name,
                            FeaturedImageUrl = blog.FeaturedImageUrl,
                            ReadingTime = blog.ReadingTime.ToString()
                        }).OrderByDescending(x=>x.Date).Take(5).ToList();
            foreach (var post in data)
            {
                post.ViewCount = GetBlogPostViewCount(post.SeoUrl, post.CategorySeoUrl);
            }

            return data;
        }

        public bool CheckBlogBySeoUrl(string seoUrl)
        {
            return _context.BlogPosts.Any(x => x.SeoUrl == seoUrl);
        }

        public BlogsVm GetPosts(string catSeoUrl, int pageIndex,string searchKey)
        {
            var retVal = new BlogsVm();
            var data = _context.BlogPosts.Include(x=>x.Category).OrderByDescending(x=>x.CreatedDate).AsQueryable();
            if (!string.IsNullOrEmpty(catSeoUrl))
            {
                data = data.Where(x => x.Category.SeoUrl == catSeoUrl);
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                var ids = data.Where(x => x.Title == searchKey || x.Title.Contains(searchKey)).Select(x => x.Id).ToList();
                searchKey = searchKey.FormatForTag();
                var tags = Context.Bridge_BlogPostTags
                                    .Join(Context.BlogTags, l => l.Id2, r => r.Id, (x, y) => new
                                    {
                                        TagId = x.Id2,
                                        BlogPostId = x.Id,
                                        TagName = y.Name
                                    })
                                    .ToList();
                var blogPostIds = tags
                     .Where(x => x.TagName.Contains(searchKey))
                     .Select(x => x.BlogPostId)
                     .ToList();
                blogPostIds.AddRange(ids);
                data = data.Where(x => blogPostIds.Contains(x.Id));
            }

            var totalCount = data.Count();

            data = data.Skip((pageIndex-1) * 8).Take(8);

            var list = data.Include(x => x.Category).Select(x => new BlogPostListVm
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.SummaryContent,
                CreatedDate = x.CreatedDate.ToShortDateString(),
                Category = x.Category.Name,
                FeaturedImageUrl = x.FeaturedImageUrl,
                ReadingTime = x.ReadingTime.ToString(),
                SeoUrl = x.SeoUrl,
                CategorySeoUrl = x.Category.SeoUrl
            }).ToList();
            foreach (var post in list)
            {
                post.ViewCount = GetBlogPostViewCount(post.SeoUrl, post.CategorySeoUrl);
            }

            retVal.Posts = list;
            retVal.TotalCount = totalCount;
            retVal.TotalPageCount = (int)Math.Ceiling(totalCount/ (double)6);
            retVal.PageIndex = pageIndex;

            return retVal;
        }

        public int TotalBlogPostCount()
        {
            return _context.BlogPosts.Count();
        }

        public List<LastBlogPostVm> LastBlogPosts(int t)
        {
            var data = (from blog in _context.BlogPosts
                        join blogCategory in _context.BlogCategories on blog.CategoryId equals blogCategory.Id
                        orderby blog.CreatedDate descending
                        where blog.IsHighLight
                        select new LastBlogPostVm
                        {
                            Id = blog.Id,
                            Title = blog.Title,
                            Date = blog.CreatedDate,
                            CreatedDate = blog.CreatedDate.ToShortDateString(),
                            Category = blogCategory.Name,
                            FeaturedImageUrl = blog.FeaturedImageUrl,
                            ReadingTime = blog.ReadingTime.ToString(),
                            SeoUrl = blog.SeoUrl,
                            CategorySeoUrl = blogCategory.SeoUrl
                        }).OrderByDescending(x=>x.Date).Take(t).ToList();
            
            return data;
        }

        public List<BlogTag> GetTagsByBlogPostId(Guid postId)
        {
            var data = (from b in Context.Bridge_BlogPostTags
                        join c in Context.BlogTags on b.Id2 equals c.Id
                        where b.Id == postId
                        select c).ToList();
            return data;
        }

        public BlogPostGetDetailVm GetPostById(Guid id)
        {
            var post = _context.BlogPosts.First(x => x.Id == id);
            var model = new BlogPostGetDetailVm()
            {
                Id = post.Id,
                Content = post.Content,
                FeaturedImageUrl = post.FeaturedImageUrl,
                Title = post.Title
            };
            return model;
        }
        public BlogPostGetDetailVm GetPostBySeoUrl(string seoUrl)
        {
            var post = _context.BlogPosts.First(x => x.SeoUrl == seoUrl);
            var tags = _context.Bridge_BlogPostTags.Include(x => x.BlogTag).Where(x => x.Id == post.Id).Select(x => x.BlogTag.Name).ToList();
            var model = new BlogPostGetDetailVm()
            {
                Id = post.Id,
                Content = post.Content,
                FeaturedImageUrl = post.FeaturedImageUrl,
                Title = post.Title,
                Summary = post.SummaryContent,
                Tags = string.Join(',', tags)
            };
            return model;
        }

        public Guid? Insert(BlogPost blogPost, string[] tags)
        {
            if (tags == null || tags.Length == 0)
                return null;
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    List<Guid> tagIds = new List<Guid>();
                    var blogPostId = base.Insert(blogPost);
                    var bridge = new List<Bridge_BlogPostTag>();
                    var dbTags = _context.BlogTags.ToList();
                    foreach (var tag in tags)
                    {
                        if (!dbTags.Any(x => x.Name == tag))
                        {
                            var blogTag = new BlogTag { Name = tag };
                            var model = _context.BlogTags.Add(blogTag);
                            _context.SaveChanges();
                            bridge.Add(new Bridge_BlogPostTag
                            {
                                Id = blogPostId,
                                Id2 = blogTag.Id

                            });
                        }
                        else
                        {
                            bridge.Add(new Bridge_BlogPostTag
                            {
                                Id = blogPostId,
                                Id2 = dbTags.First(x => x.Name == tag).Id
                            });
                        }
                    }
                    _context.Bridge_BlogPostTags.AddRange(bridge);
                    _context.SaveChanges();
                    transaction.Commit();
                    return blogPostId;

                }
                catch
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }

        public int Update(BlogPost post, string[] tags)
        {
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    //Eski kayıtlar siliniyor.
                    var currentPostTags = _context.Bridge_BlogPostTags.Where(x => x.Id == post.Id).ToList();
                    _context.Bridge_BlogPostTags.RemoveRange(currentPostTags);
                    _context.SaveChanges();
                    List<Guid> tagIds = new List<Guid>();
                    var blogPostId = base.Update(post);
                    _context.SaveChanges();
                    var bridge = new List<Bridge_BlogPostTag>();
                    var dbTags = _context.BlogTags.ToList();
                    foreach (var tag in tags)
                    {
                        if (!dbTags.Any(x => x.Name == tag))
                        {
                            var blogTag = new BlogTag { Name = tag };
                            var model = _context.BlogTags.Add(blogTag);
                            _context.SaveChanges();
                            bridge.Add(new Bridge_BlogPostTag
                            {
                                Id = post.Id,
                                Id2 = blogTag.Id

                            });
                        }
                        else
                        {
                            bridge.Add(new Bridge_BlogPostTag
                            {
                                Id = post.Id,
                                Id2 = dbTags.First(x => x.Name == tag).Id
                            });
                        }
                    }
                    _context.Bridge_BlogPostTags.AddRange(bridge);
                    _context.SaveChanges();
                    transaction.Commit();
                    return blogPostId;

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }
        public void Delete(Guid id)
        {

            try
            {
                base.Delete(id);
                var bridges = _context.Bridge_BlogPostTags.Where(x => x.Id == id).ToList();
                _context.Bridge_BlogPostTags.RemoveRange(bridges);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int GetBlogPostViewCount(string seoUrl, string catSeoUrl)
        {
            int count = 0;
            var counts = _elasticClient.Count<BlogViewLog>(s =>
            s.Query(
                q =>
                q.Term(t => t.CatSeoUrl, catSeoUrl) &&
                q.Term(t => t.SeoUrl, seoUrl)));
            if (counts.IsValid)
            {
                count = (int)counts.Count;
            }
            return count;
        }

    }
}