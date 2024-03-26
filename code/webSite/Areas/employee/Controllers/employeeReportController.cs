using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Controllers;

namespace web.Areas.employee.Controllers
{
    public class employeeReportController : baseController
    {
        // GET: employee/employeeReport
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult select(db.view.NN_report model)
        {
            model.Search();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ShowData", model);
            }
            return View(model);
        }

        public ActionResult batchBan(string keyFieldValues)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                string msg = db.bll.NN_report.batchBan(keyFieldValues, dc);
                result.data = rui.jsonResult.getAJAXResult(msg, true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return Json(result.data);
        }
        public ActionResult batchRecovey(string keyFieldValues)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                string msg = db.bll.NN_report.batchRecovey(keyFieldValues, dc);
                result.data = rui.jsonResult.getAJAXResult(msg, true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return Json(result.data);
        }

        public ActionResult Detail(string rowID)
        {
            db.view.NN_report_userly model = db.bll.NN_report.getNewsItemByRowID(rowID, dc);
            return View(model);
        }

        public ActionResult Update(string rowID)
        {
            db.view.NN_report_userly model = db.bll.NN_report.getNewsItemByRowID(rowID, dc);
            return View(model);
        }

        public ActionResult reportBan(string reportID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                string msg = db.bll.NN_report.batchBan(reportID, dc);
                result.data = rui.jsonResult.getAJAXResult(msg, true);
            }
            catch(Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return Json(result.data);
        }

        public ActionResult reportPass(string reportID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                string msg = db.bll.NN_report.batchRecovey(reportID, dc);
                result.data = rui.jsonResult.getAJAXResult(msg, true);
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return Json(result.data);
        }
    }
}