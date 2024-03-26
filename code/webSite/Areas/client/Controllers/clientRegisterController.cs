using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Controllers;

namespace web.Areas.client.Controllers
{
    public class clientRegisterController : baseController
    {
        // GET: client/clientRegister
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult register()
        {
            return View();
        }

        public ActionResult registerRec()
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {

            }
            catch(Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return Json(result.data);
        }

        public ActionResult checkSameUserName(string nickname)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                if(db.bll.NN_user.checkSameUserName(nickname)) //True  已经有相同的用户名
                {

                    result.data = rui.jsonResult.getAJAXResult("被占用", false);
                }
                else
                {
                    result.data = rui.jsonResult.getAJAXResult("可用", true);
                }
            }
            catch(Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("错误:" + rui.excptHelper.getExMsg(ex), false);
            }
            return Json(result.data);
        }
        public ActionResult register_exec()
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                HttpPostedFileBase avatarUpload = Request.Files["avatarUpload"];
                string nickName = Request.Form["nickName"].ToString();
                string password = Request.Form["password"].ToString();
                string gender = Request.Form["gender"].ToString() == "male" ? "True" : "False";
                DateTime birthdate = DateTime.Parse(Request.Form["birthdate"].ToString());
                db.NN_user model = new db.NN_user();
                model.userName = nickName;
                model.password = password; //加密工作在bll里面进行
                model.gender = bool.Parse(gender);
                model.birth = birthdate;
                model.type = 0; // 新用户首次注册默认为普通用户
                string rowID = db.bll.NN_user.insert(model, dc, avatarUpload);
                string NN_account = db.bll.NN_user.getModelByRowID(rowID, dc).userID;
                result.data = rui.jsonResult.getAJAXResult(NN_account, true);
            }
            catch(Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return Json(result.data);
        }
    }
}