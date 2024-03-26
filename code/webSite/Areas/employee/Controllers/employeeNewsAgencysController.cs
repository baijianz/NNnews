using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Controllers;

namespace web.Areas.employee.Controllers
{
    public class employeeNewsAgencysController: baseController
    {
        public ActionResult select(db.view.NN_NewsAgency model)
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
            var model = db.bll.NN_NewsAgency.getModelbyRowID(rowID, dc);
            return View(model);
        }

        public ActionResult Update(string rowID)
        {
            var model = db.bll.NN_NewsAgency.getModelbyRowID(rowID, dc);
            return View(model);
        }

        [HttpPost]
        public ActionResult Update(db.NN_NewsAgency model)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                if(ModelState.IsValid)
                {
                    string rowID = db.bll.NN_NewsAgency.update(model, dc);
                    result.data = rui.jsonResult.getAJAXResult("更新成功", true);
                }
                else
                {
                    rui.excptHelper.throwEx("输入的信息有误");
                }
            }
            catch(Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("更新失败", true);
            }
            return Json(result.data);
        }


        public ActionResult Delete(string rowID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                db.bll.NN_NewsAgency.deleteNewsAgencys(rowID, dc);
                result.data = rui.jsonResult.getAJAXResult("删除成功", true);
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("有关联项目不允许删除", true);
            }
            return Json(result.data);
        }

        public ActionResult Insert()
        {
            var model = new db.NN_NewsAgency();
            return View(model);
        }

        [HttpPost]
        public ActionResult Insert(db.NN_NewsAgency model)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                if(ModelState.IsValid)
                {
                    string rowID = db.bll.NN_NewsAgency.insert(model, dc);
                    result.data = rui.jsonResult.getAJAXResult("新增成功", true,
                               rui.jsonResult.getDicByRowID(rowID));
                }
                else
                {
                    result.data = rui.jsonResult.getAJAXResult("输入不合法", false);  //在这里我改了js代码，false不再是ajax失败的意思。
                                                                                      //而是单纯的添加失败，我们默认ajax都是成功的
                }
            }
            catch(Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return Json(result.data);
        }

        public ActionResult ChangeState(string rowID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                string msg = db.bll.NN_NewsAgency.ChangeState(rowID, dc);
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
                string msg = db.bll.NN_NewsAgency.batchDelete(keyFieldValues, dc);
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
                string msg = db.bll.NN_NewsAgency.batchBan(keyFieldValues, dc);
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
                string msg = db.bll.NN_NewsAgency.batchRecovey(keyFieldValues, dc);
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