using db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Controllers;

namespace web.Areas.employee.Controllers
{
    public class employeeNewsCategoryNameController : baseController
    {
        // GET: employee/employeeNewsCategoryName
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult select(db.view.NN_newsCategory model)
        {
            model.Search();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ShowData", model);
            }
            return View(model);
        }

        public ActionResult Detail(string rowID)
        {
            db.NN_newsCategory model = db.bll.NN_newsCategory.getModelByRowID(rowID, dc);
            return View(model);
        }

        public ActionResult Update(string rowID)
        {
            db.NN_newsCategory model = db.bll.NN_newsCategory.getModelByRowID(rowID, dc);
            return View(model);
        }

        [HttpPost]
        public ActionResult Update(db.NN_newsCategory model)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                if (ModelState.IsValid)
                {
                    string rowID = db.bll.NN_newsCategory.update(model, dc);
                    result.data = rui.jsonResult.getAJAXResult("更新成功", true);
                }
                else
                {
                    result.data = rui.jsonResult.getAJAXResult("输入的信息有误", false);
                }
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("更新失败", false);
            }
            return Json(result.data);
        }


        public ActionResult Insert()
        {
            db.NN_newsCategory model = new db.NN_newsCategory();
            return View(model);
        }

        [HttpPost]
        public ActionResult Insert(db.NN_newsCategory model)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                if (ModelState.IsValid)
                {
                    string rowID = db.bll.NN_newsCategory.insert(model, dc);
                    result.data = rui.jsonResult.getAJAXResult("新增成功", true,
                        rui.jsonResult.getDicByRowID(rowID));
                }
                else
                {
                    result.data = rui.jsonResult.getAJAXResult("输入不合法", false);
                }
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return Json(result.data);
        }

        public ActionResult Delete(string rowID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                db.bll.NN_newsCategory.deleteNewsCategory(rowID, dc);
                result.data = rui.jsonResult.getAJAXResult("删除成功", true);
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult(rui.excptHelper.getExMsg(ex), false);
            }
            return Json(result.data);
        }


        public ActionResult ChangeState(string rowID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                string msg = db.bll.NN_newsCategory.ChangeState(rowID, dc);
                result.data = rui.jsonResult.getAJAXResult(msg, true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return Json(result.data);
        }

        public ActionResult batchDelete(string keyFieldValues)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                string msg = db.bll.NN_newsCategory.batchDelete(keyFieldValues, dc);
                result.data = rui.jsonResult.getAJAXResult(msg, true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return Json(result.data);
        }

        public ActionResult batchBan(string keyFieldValues)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                string msg = db.bll.NN_newsCategory.batchBan(keyFieldValues, dc);
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
                string msg = db.bll.NN_newsCategory.batchRecovey(keyFieldValues, dc);
                result.data = rui.jsonResult.getAJAXResult(msg, true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return Json(result.data);
        }


    }
}