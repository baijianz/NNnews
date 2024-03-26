using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Controllers;

namespace web.Areas.employee.Controllers
{
    public class employeeHomesController : baseController
    {
        // GET: employee/employeeHomes
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult homes()
        {
            return View();
        }
    }
}