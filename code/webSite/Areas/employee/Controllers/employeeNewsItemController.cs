using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Controllers;

namespace web.Areas.employee.Controllers
{
    public class employeeNewsItemController : baseController
    {
        // GET: employee/employeeNewsItem
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult select(db.view.NN_newsItem model)
        {
            model.Search();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ShowData", model);
            }
            return View(model);
        }


        public ActionResult Insert()
        {
            db.NN_newsItem model = new db.NN_newsItem();
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Insert(db.NN_newsItem model, FormCollection form, HttpPostedFileBase newCoverPic)
        {
            rui.jsonResult result = new rui.jsonResult();
            string selectChkBox = form["selectedItems"];
            try
            {
                if (ModelState.IsValid)
                {
                    string rowID = db.bll.NN_newsItem.insert(model, selectChkBox, newCoverPic, dc);
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


        public  ActionResult Delete(string rowID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                db.bll.NN_newsItem.deleteNN_newsItem(rowID, dc);
                result.data = rui.jsonResult.getAJAXResult("删除成功", true);
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("有关联项目不允许删除", true);
            }
            return Json(result.data);
        }
        public ActionResult Detail(string rowID)
        {
            db.NN_newsItem model = db.bll.NN_newsItem.getModelByRowID(rowID, dc);
            return View(model);
        }

        [HttpGet]
        public ActionResult Update(string rowID)
        {
            db.NN_newsItem model = db.bll.NN_newsItem.getModelByRowID(rowID, dc);
            return View(model);
        }
        
        [HttpPost]
        [ValidateInput(false)]
        public  ActionResult Update(db.NN_newsItem model, HttpPostedFileBase newCoverPic, FormCollection form)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                string selectChkBox = form["selectedItems"];
                if (ModelState.IsValid)
                {
                    string rowID = db.bll.NN_newsItem.update(model, selectChkBox,newCoverPic, dc);
                    result.data = rui.jsonResult.getAJAXResult("更新成功", true);
                }
                else
                {
                    result.data = rui.jsonResult.getAJAXResult("出错", false);
                }
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("出错", false);
            }
            return Json(result.data);
        }

        public ActionResult batchDelete(string keyFieldValues)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                string msg = db.bll.NN_newsItem.batchDelete(keyFieldValues, dc);
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
                string msg = db.bll.NN_newsItem.batchBan(keyFieldValues, dc);
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
                string msg = db.bll.NN_newsItem.batchRecovey(keyFieldValues, dc);
                result.data = rui.jsonResult.getAJAXResult(msg, true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return Json(result.data);
        }

        public ActionResult ChangeState(string rowID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                 string msg =  db.bll.NN_newsItem.ChangeState(rowID, dc);
                result.data = rui.jsonResult.getAJAXResult(msg, true);
            }
            catch(Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return Json(result.data);
        }
    }
}