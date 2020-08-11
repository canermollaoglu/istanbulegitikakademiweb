using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities.blog;
using NitelikliBilisim.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.Business.Repositories.BlogRepositories
{
    public class BlogPostRepository : BaseRepository<BlogPost, Guid>
    {
        NbDataContext _context;
        public BlogPostRepository(NbDataContext context) : base(context)
        {
            _context = context;
        }

        public BlogPost GetByIdWithCategory(Guid postId)
        {
            return _context.BlogPosts.Include(x => x.Category).First(x=>x.Id == postId);
        }

        public List<BlogTag> GetTagsByBlogPostId(Guid postId)
        {
            var data = (from b in Context.Bridge_BlogPostTags
                        join c in Context.BlogTags on b.Id2 equals c.Id
                        where b.Id == postId
                        select c).ToList();
            return data;
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
    }
}