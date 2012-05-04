using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using theSouthtown.Models;

namespace theSouthtown.Repositories
{ 
    public class ArticleRepository : IArticleRepository
    {
        theSouthtownContext context = new theSouthtownContext();

        public IQueryable<Article> All
        {
            get { return context.Articles; }
        }

        public IQueryable<Article> AllIncluding(params Expression<Func<Article, object>>[] includeProperties)
        {
            IQueryable<Article> query = context.Articles;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Article Find(int id)
        {
            return context.Articles.Find(id);
        }

        public void InsertOrUpdate(Article article)
        {
            if (article.Id == default(int)) {
                // New entity
                context.Articles.Add(article);
            } else {
                // Existing entity
                context.Entry(article).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var article = context.Articles.Find(id);
            context.Articles.Remove(article);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IArticleRepository
    {
        IQueryable<Article> All { get; }
        IQueryable<Article> AllIncluding(params Expression<Func<Article, object>>[] includeProperties);
        Article Find(int id);
        void InsertOrUpdate(Article article);
        void Delete(int id);
        void Save();
    }
}