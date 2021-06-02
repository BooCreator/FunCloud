using System.Web.Mvc;

namespace FunCloud.Controllers
{
    public class DataBaseController : Controller
    {

        public ActionResult Index() 
            => this.View();

        public ActionResult Check()
        {

            this.ViewBag.Messages = Context.CheckTables();

            return this.View();
        }

    }
}