using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Controllers;

namespace web.Areas.employee.Controllers
{
    public class employeeApplyController : baseController
    {
        // GET: employee/employeeApply
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult select(db.view.NN_apply model)
        {
            model.Search();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ShowData", model);
            }
            return View(model);
        }

        public ActionResult batchAgree(string keyFieldValues)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                string msg = db.bll.NN_apply.batchAgree(keyFieldValues, dc);
                result.data = rui.jsonResult.getAJAXResult(msg, true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return Json(result.data);
        }

        public ActionResult batchReject(string keyFieldValues)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                string msg = db.bll.NN_apply.batchDelete(keyFieldValues, dc);
                result.data = rui.jsonResult.getAJAXResult(msg, true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return Json(result.data);
        }


        public ActionResult detail(string NNAID)
        {
            db.NN_apply model = db.bll.NN_apply.GetModelByuserIDAndType(NNAID, dc);
            return View(model);
        }


        public ActionResult agree(string applyID)
        {
            return batchAgree(applyID);
            
        }

        public ActionResult reject(string applyID)
        {
            return batchReject(applyID);
        }
    }
}