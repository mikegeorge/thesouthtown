using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using theSouthtown.Models;
using theSouthtown.Repositories;

namespace theSouthtown.Areas.Admin.Controllers
{
  [Authorize]
  public class ArticlesController : Controller
    {
		private readonly IArticleRepository articleRepository;

		// If you are using Dependency Injection, you can delete the following constructor
        public ArticlesController() : this(new ArticleRepository())
        {
        }

        public ArticlesController(IArticleRepository articleRepository)
        {
			this.articleRepository = articleRepository;
        }

        //
        // GET: /Articles/

        public ViewResult Index()
        {
            return View(articleRepository.All);
        }

        //
        // GET: /Articles/Details/5

        public ViewResult Details(int id)
        {
            return View(articleRepository.Find(id));
        }

        //
        // GET: /Articles/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Articles/Create

        [HttpPost]
        public ActionResult Create(Article article)
        {
            if (ModelState.IsValid) {
                articleRepository.InsertOrUpdate(article);
                articleRepository.Save();
                return RedirectToAction("Index");
            } else {
				return View();
			}
        }
        
        //
        // GET: /Articles/Edit/5
 
        public ActionResult Edit(int id)
        {
             return View(articleRepository.Find(id));
        }

        //
        // POST: /Articles/Edit/5

        [HttpPost]
        public ActionResult Edit(Article article)
        {
            if (ModelState.IsValid) {
                articleRepository.InsertOrUpdate(article);
                articleRepository.Save();
                return RedirectToAction("Index");
            } else {
				return View();
			}
        }

        //
        // GET: /Articles/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View(articleRepository.Find(id));
        }

        //
        // POST: /Articles/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            articleRepository.Delete(id);
            articleRepository.Save();

            return RedirectToAction("Index");
        }
    }
}

