using db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Controllers;

namespace web.Areas.employee.Controllers
{
    public class employeeUserController : baseController
    {
        // GET: employee/employeeUser
        public ActionResult Index()
        {
            return View();
        }


        /*
         * 查找
         */
        public ActionResult select(db.view.NN_user model)
        {
            model.Search();
            if(Request.IsAjaxRequest())
            {
                return PartialView("_ShowData", model);
            }
            return View(model);
        }

        /*
         * 新增
         */
        public ActionResult Insert()
        {
            db.NN_user model = new db.NN_user();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Insert(db.NN_user model, HttpPostedFileBase file)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {

                if (ModelState.IsValid)
                {
                    string rowID = db.bll.NN_user.insert(model, dc, file);
                    result.data = rui.jsonResult.getAJAXResult("新增成功", true,
                                            rui.jsonResult.getDicByRowID(rowID));
                }
                else
                {
                    List<string> sb = new List<string>();
                    //获取所有错误的Key
                    List<string> Keys = ModelState.Keys.ToList();
                    //获取每一个key对应的ModelStateDictionary
                    foreach (var key in Keys)
                    {
                        var errors = ModelState[key].Errors.ToList();
                        //将错误描述添加到sb中
                        foreach (var error in errors)
                        {
                            sb.Add(error.ErrorMessage);
                        }
                    }
                    result.data = rui.jsonResult.getAJAXResult("输入不合法", false);
                }
            }
            catch(DbEntityValidationException ex)
            {
                rui.logHelper.log(ex);
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
             return Json(result.data);
        }

        [HttpGet]
        public  ActionResult Detail(string rowID)
        {
            db.NN_user model = db.bll.NN_user.getModelByRowID(rowID, dc);
            return View(model);
        }

        [HttpPost]

        public  ActionResult Detail_(string s)
        {
            return View();
        }


        /*
         * 更新
         */
        [HttpGet]
        public ActionResult Update(string rowID)
        {
            db.NN_user model = db.bll.NN_user.getModelByRowID(rowID, dc);
            return View(model);
        }

        [HttpPost]
        public ActionResult Update(db.NN_user model, HttpPostedFileBase newProfile, string status)
        {
            rui.jsonResult result = new rui.jsonResult();
            if (status == "0") model.status = false;
            else model.status = true;
            ModelState.Remove("status");
            try
            {
                if(ModelState.IsValid)
                {
                    string rowID = db.bll.NN_user.update(model, newProfile, dc);
                    result.data = rui.jsonResult.getAJAXResult("更新成功", true,
                        rui.jsonResult.getDicByRowID(rowID));
                }
                else
                {
                    List<string> sb = new List<string>();
                    //获取所有错误的Key
                    List<string> Keys = ModelState.Keys.ToList();
                    //获取每一个key对应的ModelStateDictionary
                    foreach (var key in Keys)
                    {
                        var errors = ModelState[key].Errors.ToList();
                        //将错误描述添加到sb中
                        foreach (var error in errors)
                        {
                            sb.Add(error.ErrorMessage);
                        }
                    }
                    result.data = rui.jsonResult.getAJAXResult("输入不合法", false);
                }
            }
            catch(Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
             return Json(result.data);
        }


        /*
         * 获取联动的下拉框的值
         */
        public JsonResult getNewsAgencyDdl(string type, string rowID = "")
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("newAgencysList", db.bll.NN_user.getJsonAgencyDdl(type, rowID, dc));
                result.data = rui.jsonResult.getAJAXResult("获取成功", true, dic);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return Json(result.data);
        }


        public ActionResult Delete(string rowID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                db.bll.NN_user.deleteNN_userEntry(rowID, dc);
                result.data = rui.jsonResult.getAJAXResult("删除成功", true);
            }
            catch(Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult(rui.excptHelper.getExMsg(ex), true);
            }
            return Json(result.data);
        }


        public ActionResult ChangeState(string rowID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                string msg = db.bll.NN_user.ChangeState(rowID, dc);
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
                string msg = db.bll.NN_user.batchDelete(keyFieldValues, dc);
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
                string msg = db.bll.NN_user.batchBan(keyFieldValues, dc);
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
                string msg = db.bll.NN_user.batchRecovey(keyFieldValues, dc);
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