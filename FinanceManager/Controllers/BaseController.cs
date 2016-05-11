using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanceManager.Controllers
{
    public class BaseController : Controller
    {
		protected ModelContext context = new ModelContext();
    }
}