using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using web.Controllers;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace web.Areas.client.Controllers
{
    public class clientHomeController : baseController
    {
        // GET: client/clientHome

        /// <summary>
        /// 主页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult about()
        {
            return View();
        }

        public ActionResult cateGory()
        {
            return View();
        }

        public ActionResult CateGoryDetail(string categoryID)
        {
            //List<db.NN_newsItem> list = db.bll.NN_newsItem.getModelLIstByCateGoryID(categoryID, dc);
            ViewBag.Title = db.bll.NN_newsCategory.getNameByID(categoryID, dc);
            ViewBag.categoryID = categoryID;
            return View();
        }

        /// <summary>
        /// 获取通过分类筛选出来的新闻条目
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public ActionResult CateGoryNewsItemDataTable(string categoryID)
        {

            var data = db.bll.NN_newsItem.getModelLIstByCateGoryID(categoryID, dc);
            string jsonString = JsonConvert.SerializeObject(data);
            return new JsonResult
            {
                Data = jsonString,
                MaxJsonLength = int.MaxValue // 设置为你想要的最大 JSON 字符串长度
            };

        }

        /// <summary>
        /// 获取本机的IP地址
        /// </summary>
        /// <returns></returns>
        public string GetLocalIp()
        {
            //淘宝网获取ID，但是现在用不了了好像
            /*          string AddressIP = string.Empty;
                        WebRequest wrt = null;
                        WebResponse wrp = null;
                        wrt = WebRequest.Create("http://www.taobao.com/help/getip.ph");
                        wrp = wrt.GetResponse();
                        StreamReader sr = new StreamReader(wrp.GetResponseStream(), Encoding.Default);
                        string html = sr.ReadToEnd();
                        JsonResult data = Json(html);
                        return data;*/

            return "114.221.1.119";
        }

        /// <summary>
        /// 通过api 获取省份+城市
        /// </summary>
        /// <returns></returns>
        public string getProvinceAndCity()
        {
            string url = "https://api.map.baidu.com/location/ip?ip=" + GetLocalIp() + "&coor=bd09ll&ak=" + "9tXRXGNHomHxlPZnR3tZ1Pod6Lmt03VO";
            WebRequest wrt = null;
            WebResponse wrp = null;
            try
            {
                wrt = WebRequest.Create(url);
                wrt.Credentials = CredentialCache.DefaultCredentials;
                wrp = wrt.GetResponse();
                StreamReader sr = new StreamReader(wrp.GetResponseStream(), Encoding.Default);
                string jsonString = sr.ReadToEnd();
                string provincePattern = "\"province\":\"(.*?)\"";
                Match provinceMatch = Regex.Match(jsonString, provincePattern);

                string ans = "";
                if (provinceMatch.Success)
                {
                    string province = provinceMatch.Groups[1].Value;
                    province = Regex.Unescape(province);
                    ans += province;
                }

                // 匹配"city"字段
                string cityPattern = "\"city\":\"(.*?)\"";
                Match cityMatch = Regex.Match(jsonString, cityPattern);

                if (cityMatch.Success)
                {
                    string city = cityMatch.Groups[1].Value;
                    // 解码Unicode转义字符
                    city = Regex.Unescape(city);
                    ans += ("." + city);
                }
                return ans;
            }
            catch (Exception ex) {
                return ex.Message;
            }
            finally
            {
                if (wrp != null)
                    wrp.Close();
                if (wrt != null)
                    wrt.Abort();
            }
        }
        /// <summary>
        /// 获取天气信息
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> getWeatherAsync()
        {
            string url = "https://api.map.baidu.com/weather/v1/";
            string ak = "9tXRXGNHomHxlPZnR3tZ1Pod6Lmt03VO";

            // 构建HttpClient
            using (HttpClient httpClient = new HttpClient())
            {
                // 设置请求参数
                var queryParams = new
                {

                    district_id = "320100",
                    data_type = "all",
                    ak
                };

                string queryString = $"?district_id={queryParams.district_id}&data_type={queryParams.data_type}&ak={queryParams.ak}";

                // 构建请求Url
                string requestUrl = $"{url}{queryString}";

                // 发送请求
                HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

                try
                {
                    // 处理响应
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(rui.jsonResult.getAJAXResult("无可用信息，请检查网络", false));
                    }
                }
                catch (Exception ex)
                {
                    return Json(rui.jsonResult.getAJAXResult(ex.Message, false));
                }
            }
        }


        public ActionResult contact(int flag = 0)
        {
            if (flag == 1)
            {
                ViewBag.Flag = 1;
            }
            return View();
        }

        public ActionResult _404NotFound()
        {
            return View();
        }

        public ActionResult postDetail(string newsID)
        {
            db.NN_newsItem model = db.bll.NN_newsItem.getModelByNewsID(newsID, dc);
            return View(model);
        }

        /// <summary>
        /// 将要展示的评论以"a, b , c"的形式返回
        /// </summary>
        /// <param name="newsID"></param>
        /// <returns></returns>
        public ActionResult CommentInfo(string newsID)
        {
            string list = "";
            var model = db.bll.NN_comment.getCommentListByNewsID(newsID, dc);
            foreach (var item in model)
            {
                list += (item.commentID + ",");
            }
            if (list.Count() > 1)
            {
                list.Remove(list.Count() - 1);
            }
            rui.jsonResult result = new rui.jsonResult();
            Dictionary<string, string> dict = new Dictionary<string, string>() { { "key", list } };
            result.data = rui.jsonResult.getAJAXResult("获取成功", true, dict);
            return Json(result.data);
        }

        /// <summary>
        /// 获取评论的内容
        /// </summary>
        /// <param name="newsID"></param>
        /// <returns></returns>
        public ActionResult commentDataTable(string newsID)
        {
            var data = db.bll.NN_comment.GetComment_UserliesByNewsID_NonDc(newsID);
            string jsonString = JsonConvert.SerializeObject(data);
            return new JsonResult
            {
                Data = jsonString,
                MaxJsonLength = int.MaxValue // 设置为你想要的最大 JSON 字符串长度
            };
        }

        /// <summary>
        /// 多条件搜索，新闻标题，作者名字，标签名字
        /// /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        /// 
        public ActionResult NNsearch(string content)
        {
            ViewBag.content = content;
            return View();
        }

        public ActionResult GetTagBynewsID(string newsID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                string ans = db.bll.NN_newsCategroryItem.getTagListByNewsID(newsID);
                result.data = rui.jsonResult.getAJAXResult(ans, true);
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("error:" + ex.Message, false);
            }
            return Json(result.data);
        }
        public ActionResult NNsearchAjax(string content)
        {
            List<db.NN_newsItem> list = db.bll.NN_newsItem.NNsearch(content, dc);
            string jsonString = JsonConvert.SerializeObject(list);
            return new JsonResult
            {
                Data = jsonString,
                MaxJsonLength = int.MaxValue // 设置为你想要的最大 JSON 字符串长度
            };
        }
        public ActionResult recommentDataTable(int count)
        {
            var data = db.bll.NN_newsItem.getRandomRecommendNewsItem(count);
            string jsonString = JsonConvert.SerializeObject(data);
            return new JsonResult
            {
                Data = jsonString,
                MaxJsonLength = int.MaxValue // 设置为你想要的最大 JSON 字符串长度
            };
        }

        /// <summary>
        /// 接收刚刚提交的评价信息
        /// </summary>
        /// <returns></returns>
        public ActionResult recCommentSubmit(string content, string newsID, string userID, string fatherCommentID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                db.bll.NN_comment.insertComment(content, newsID, userID, fatherCommentID, dc);
                result.data = rui.jsonResult.getAJAXResult("评论成功", true);
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("评论失败:" + ex.Message, false);
            }
            return Json(result.data);
        }
        /// <summary>
        /// 接收发起的举报请求
        /// </summary>
        /// <returns></returns>
        public ActionResult report(string userID, string newsID = "-1", string commentId = "-1")
        {
            ViewBag.reporterUserID = userID;
            ViewBag.reporterUserName = db.bll.NN_user.getUserNameByUserID(userID);
            ViewBag.Type = (newsID == "-1" ? "评论" : "新闻");
            ViewBag.newsID = newsID;
            ViewBag.commentId = commentId;
            return View();
        }

        public ActionResult recReport(string userID, string category, string newsID, string commentID, string reason)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                db.bll.NN_report.insert(userID, category, newsID, commentID, reason, dc);
                result.data = rui.jsonResult.getAJAXResult("提交成功", true);
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("提交失败:" + ex.Message, false);
            }

            return Json(result.data);
        }

        public ActionResult readQuanlity_pp(string newsID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                db.bll.NN_newsItem.readQuantityPP(newsID, dc);
                result.data = rui.jsonResult.getAJAXResult("success", true);
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("错误:" + ex.Message, false);
            }
            return Json(result.data);
        }

        public ActionResult Like(string userID, string newsID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                var count = dc.NN_like.Where(a => a.userID == userID && a.newsID == newsID).Count();
                if (count == 0) //说明现在是点赞
                {
                    db.bll.NN_like.insert(userID, newsID, dc);
                    result.data = rui.jsonResult.getAJAXResult("已点赞", true);
                }
                else
                {
                    db.bll.NN_like.delete(userID, newsID, dc);
                    result.data = rui.jsonResult.getAJAXResult("已取消点赞", true);
                }
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("错误:" + ex.Message, false);
            }
            return Json(result.data);
        }

        public ActionResult Star(string userID, string newsID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                var count = dc.NN_star.Where(a => a.userID == userID && a.newsID == newsID).Count();
                if (count == 0) //说明现在是点赞
                {
                    db.bll.NN_star.insert(userID, newsID, dc);
                    result.data = rui.jsonResult.getAJAXResult("已收藏", true);
                }
                else
                {
                    db.bll.NN_star.delete(userID, newsID, dc);
                    result.data = rui.jsonResult.getAJAXResult("已取消收藏", true);
                }
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("错误:" + ex.Message, false);
            }
            return Json(result.data);
        }

        public ActionResult checkStar(string userID, string newsID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                var count = dc.NN_star.Where(a => a.userID == userID && a.newsID == newsID).Count();
                if (count > 0)
                {
                    result.data = rui.jsonResult.getAJAXResult("star", true);
                }
                else
                {
                    result.data = rui.jsonResult.getAJAXResult("Non", true);
                }
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("error" + ex.Message, true);
            }
            return Json(result.data);
        }

        public ActionResult checkLike(string userID, string newsID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                var count = dc.NN_like.Where(a => a.userID == userID && a.newsID == newsID).Count();
                if (count > 0)
                {
                    result.data = rui.jsonResult.getAJAXResult("like", true);
                }
                else
                {
                    result.data = rui.jsonResult.getAJAXResult("Non", true);
                }
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("error" + ex.Message, true);
            }
            return Json(result.data);
        }

        public ActionResult personalCenter()
        {
            return View();
        }

        public ActionResult fakeContact()
        {
            rui.jsonResult result = new rui.jsonResult();
            result.data = rui.jsonResult.getAJAXResult("ok", true);
            return RedirectToAction("contact", new { flag = 1 });
        }


        public ActionResult amendPersonInfo()
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                HttpPostedFileBase avatarUpload = Request.Files["file"];
                string nickName = Request.Form["name"].ToString();
                string gender = Request.Form["gender"].ToString() != "male" ? "True" : "False";
                DateTime birthdate = DateTime.Parse(Request.Form["birth"].ToString());
                string userID = Request.Form["userID"].ToString();
                var model = dc.NN_user.Where(a => a.userID == userID).SingleOrDefault();
                model.userName = nickName;
                model.gender = bool.Parse(gender);
                model.birth = birthdate;
                dc.SaveChanges();
                db.bll.NN_user.update(dc.NN_user.Where(a => a.userID == userID).SingleOrDefault(), avatarUpload, dc);
                var updatedModel = dc.NN_user.Where(a => a.userID == userID).SingleOrDefault();
                db.bll.NN_loginCustomer.login(userID, updatedModel.password, dc);
                result.data = rui.jsonResult.getAJAXResult("更新个人信息成功", true);
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("error:" + ex.Message, false);
            }
            return Json(result.data);
        }

        public ActionResult NNLikeNewsAjax(string userID)
        {
            var list = db.bll.NN_like.getNewsModelList(userID, dc);
            string jsonString = JsonConvert.SerializeObject(list);
            return new JsonResult
            {
                Data = jsonString,
                MaxJsonLength = int.MaxValue // 设置为你想要的最大 JSON 字符串长度
            };
        }


        public ActionResult NNStarNewsAjax(string userID)
        {
            var list = db.bll.NN_star.getNewsModelList(userID, dc);
            string jsonString = JsonConvert.SerializeObject(list);
            return new JsonResult
            {
                Data = jsonString,
                MaxJsonLength = int.MaxValue // 设置为你想要的最大 JSON 字符串长度
            };
        }

        public ActionResult ToBeOfficial(string userID)
        {
            ViewBag.userID = userID;
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RecToBeOfficial()
        {
            rui.jsonResult result = new rui.jsonResult();
            string userID = Request.Form["userID"];
            string newsAgence = Request.Form["newsAgence"];
            try
            {
                var content = Request.Form["content"];
                db.bll.NN_apply.insert_tobeofficial(userID, newsAgence, content, dc);
                result.data = rui.jsonResult.getAJAXResult("申请成功", true);

            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("error" + ex.Message, false);
            }

            return Json(result.data);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RectobeCreater()
        {
            rui.jsonResult result = new rui.jsonResult();
            string userID = Request.Form["userID"];
            string newsAgence = Request.Form["newsAgence"];
            try
            {
                var content = Request.Form["content"];
                db.bll.NN_apply.insert_tobeCreater(userID, newsAgence, content, dc);
                result.data = rui.jsonResult.getAJAXResult("申请成功", true);

            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("error" + ex.Message, false);
            }

            return Json(result.data);
        }

        public ActionResult createNews(string userID)
        {
            return View();
        }

        public ActionResult amendPassword(string userID)
        {
            ViewBag.userID = userID;
            return View();
        }

        //originPassword: currentPassword, confirmPassword: confirmPassword, userID
        public ActionResult recAmendPassword(string originPassword, string confirmPassword, string userID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                db.bll.NN_user.amendPassword(originPassword, confirmPassword, userID, dc);
                result.data = rui.jsonResult.getAJAXResult("success", true);
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("error" + ex.Message, false);
            }
            return Json(result.data);
        }


        public ActionResult logoutCreater()
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                db.bll.NN_user.logoutCreater(rui.sessionHelper.getValue("userID"), dc);
                result.data = rui.jsonResult.getAJAXResult("success", true);
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("error" + ex.Message, false);
            }
            return Json(result.data);
        }

        public ActionResult beComeCreater(string userID)
        {
            ViewBag.userID = userID;
            return View();
        }


        public ActionResult getWordsAll()
        {
            var data = db.bll.NN_newsCategory.getWordListAll_ef();
            string jsonString = JsonConvert.SerializeObject(data);
            return new JsonResult
            {
                Data = jsonString,
                MaxJsonLength = int.MaxValue // 设置为你想要的最大 JSON 字符串长度
            };
        }
        public ActionResult rejectWall()
        {
            return View();
        }
        [HttpPost]
        public ActionResult createNewsItem()
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                string title = Request.Form["title"];
                string describe = Request.Form["describe"];
                HttpPostedFileBase file = Request.Files["file"];
                string richtext = Request.Form["RichText"];
                string TagList = Request.Form["TagList"];
                db.bll.NN_newsItem.insert_LeadingEnd(title, describe, file, TagList, richtext, dc);
                result.data = rui.jsonResult.getAJAXResult("发布成功", true);
            }
            catch (Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("error:" + ex.Message, false);
            }
            return Json(result.data);
        }

        public ActionResult PermanentCancellation(string userID)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                db.bll.NN_user.PermanentCancellation(userID, dc);
                result.data = rui.jsonResult.getAJAXResult("success", true);
            }
            catch(Exception ex)
            {
                result.data = rui.jsonResult.getAJAXResult("error" + ex.Message, true);
            }
            return Json(result.data);
        }

        
        public ActionResult CreateNewsItem()
        {
            string userID = rui.sessionHelper.getValue("userID");
            var model = db.bll.NN_user.getModelByUserID_NonDc(userID);
            if(model.type == 0) //普通用户，不允许进
            {
                return RedirectToAction("rejectWall");
            }
            return View();
        }
    }
}