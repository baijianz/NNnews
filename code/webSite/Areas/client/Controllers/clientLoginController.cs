using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Controllers;

namespace web.Areas.client.Controllers
{
    public class clientLoginController : baseController
    {
        // GET: client/clientLogin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult login(int type = 0)
        {
            ViewBag.type = type;
            return View();
        }

        public ActionResult login_(string userID, string password)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                db.bll.NN_loginCustomer.login(userID, password, dc);
                result.data = rui.jsonResult.getAJAXResult("登录成功", true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.data = rui.jsonResult.getAJAXResult(rui.excptHelper.getExMsg(ex), false);
            }
            return Json(result.data);
        }
    }
}