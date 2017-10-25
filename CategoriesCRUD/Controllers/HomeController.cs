using CategoriesCRUD.Models;
using jsTree3.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CategoriesCRUD.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("CategoryTree", "Home");
        }

        private List<JsTree3Node> GetChild(ICollection<Category> childs)
        {
            List<JsTree3Node> result = new List<JsTree3Node>();
            foreach (Category item in childs)
            {
                var node = JsTree3Node.NewNode(item.Id, item.Name);
                if (item.Childs.Count > 0)
                {
                    node.children = GetChild(item.Childs);
                }
                node.state = new State(false, false, false);
                result.Add(node);
            }
            return result;
        }

        public JsonResult GetCategory()
        {

            var root = new JsTree3Node()
            {
                id = 0,
                text = "Categories",
                state = new State(true, false, false)
            };

            ICollection<Category> childs = new Collection<Category>();
            foreach (Category item in db.Category.Where(a => !a.ParentId.HasValue))
            {
                childs.Add(item);
            }
            root.children = GetChild(childs);
            return Json(root, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CategoryTree()
        {
            return View();
        }

        public ActionResult AddEditCategory(int id)
        {
            Category category = new Category();
            if (id == 0)
            {
                ViewBag.Title = "Add";
            }
            else
            {
                ViewBag.Title = "Edit";
                category = db.Category.Where(x => x.Id == id).SingleOrDefault();
            }
            ViewData["ParentList"] = from item in db.Category
                                     select new SelectListItem {Text = item.Name, Value = item.Id.ToString() } ;
            ViewData["Action"] = ViewBag.Title + "Category";
            return View(category);
        }

        [HttpPost]
        public ActionResult AddCategory(string name, int? parentId=null)
        {
            Category category = new Category();
            category.Name = name;
            category.ParentId = parentId;
            try
            {
                db.Category.Add(category);
                int count = db.SaveChanges();
                if (count != 0)
                {
                    log.Info("Category " + category.Name + " was successfully added.");
                }
                else
                {
                    log.Error("Some problem occured while adding.");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.ToString());
            }
            return RedirectToAction("CategoryTree", "Home");
        }

        [HttpPost]
        public ActionResult EditCategory(Category updateCategory)
        {
            Category category = db.Category.Find(updateCategory.Id);
            if (category.Id == updateCategory.Id)
            {
                try
                {
                    db.Entry(category).CurrentValues.SetValues(updateCategory);
                    db.Entry(category).State = EntityState.Modified;
                    int count = db.SaveChanges();
                    if (count != 0)
                    {
                        log.Info("Category " + category.Name + " was successfully edited.");
                    }
                    else
                    {
                        log.Error("Some problem occured while editing.");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.InnerException.ToString());
                }
            }
            else
            {
                log.Error("Record does not exist.");
            }
            return RedirectToAction("CategoryTree", "Home");
        }

        private void DeleteChild(ICollection<Category> childs)
        {
            Queue<Category> deleteQueue = ConvertToQueue(childs);
            while (deleteQueue.Count > 0)
            {
                try
                {
                    Category item = deleteQueue.Dequeue();
                    if (item.Childs.Count > 0)
                    {
                        DeleteChild(item.Childs);
                    }
                    db.Category.Remove(item);
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    log.Error(ex.InnerException.ToString());
                }
            }
        }

        [HttpPost]
        public ActionResult DeleteCategory(int id)
        {
            Category category = db.Category.Find(id);
            if (category != null)
            {
                try
                {
                    if (category.Childs.Count > 0)
                    {
                        DeleteChild(category.Childs);
                    }
                    db.Category.Remove(category);
                    int count=db.SaveChanges();
                    if (count != 0)
                    {
                        log.Info("Node " + category.Name + " was successfully deleted.");
                    }
                    else
                    {
                        log.Error("Some problem occured while deleting.");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.InnerException.ToString());
                }
            }
            else
            {
                log.Error("Record does not exist.");
            }
            return RedirectToAction("CategoryTree", "Home");
        }
    }
}