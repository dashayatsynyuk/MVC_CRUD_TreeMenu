using System.Collections.Generic;
using System.Web.Mvc;

namespace CategoriesCRUD.Controllers
{
    public abstract class BaseController : Controller
    {
        protected CategoriesContext db { get; set; }

        public BaseController()
        {
            db = new CategoriesContext();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(HomeController));

        protected Queue<T> ConvertToQueue<T>(ICollection<T> childs)
        {
            Queue<T> result = new Queue<T>();
            foreach (T item in childs)
            {
                result.Enqueue(item);
            }
            return result;
        }
    }
}