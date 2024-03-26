using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace db.bll
{
    public class NN_newsItem
    {
        public static List<SelectListItem> bind排序(bool has请选择 = false, string selectedValues = "")
        {
            dbEntities dc = new dbEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            rui.listHelper.add请选择(list, has请选择);
            list.Add(new SelectListItem { Text = "阅读量高到低", Value = "1" });
            list.Add(new SelectListItem { Text = "阅读量低到高", Value = "0" });
            return list;
        }

        public static List<SelectListItem> bindNN官方新闻(bool has请选择 = false, string selectedValues = "")
        {
            dbEntities dc = new dbEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            rui.listHelper.add请选择(list, has请选择);
            list.Add(new SelectListItem { Text = "NN新闻官方", Value = "NNU000000" });
            return list;
        }


        public static List<SelectListItem> bind新闻状态(bool has请选择 = false, string selectedValues = "")
        {
            dbEntities dc = new dbEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            rui.listHelper.add请选择(list, has请选择);
            list.Add(new SelectListItem { Text = "正常", Value = "0" });
            list.Add(new SelectListItem { Text = "封禁", Value = "1" });
            return list;
        }

        private static string _createID(dbEntities dc)
        {
            string code = "NNN";
            string result = (from a in dc.NN_newsItem where a.newsID.StartsWith(code) select a.newsID).Max();
            if (result != null)
            {
                code = rui.stringHelper.codeNext(result, 6);
            }
            else
            {
                code = code + "000001";
            }
            return code;

        }
        //对字段的相关合法性进行检查
        private static void _checkINput(db.NN_newsItem model)
        {
            efHelper.checkNotNull(model.newsID, "标签ID");
        }

        public static string insert(db.NN_newsItem model, string list, HttpPostedFileBase file, dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            try
            {
                if (rui.typeHelper.isNullOrEmpty(model.newsID))
                {
                    model.newsID = _createID(dc);
                }
                List<string> list1 = rui.dbTools.getList(list);
                foreach(var item in list1)
                {
                    db.NN_newsCategroryItem entry= new db.NN_newsCategroryItem();
                    entry.newsID = model.newsID;
                    entry.newsCateGoryID = item;
                    entry.rowID = ef.newGuid();
                    dc.NN_newsCategroryItem.Add(entry);
                }
                _checkINput(model);
                model.rowID = ef.newGuid();
                model.publishDateTime = DateTime.Now;
                model.readQuantity = 0;
                model.status = 0;
                byte[] imageByte;
                if (file == null)
                {
                    imageByte = File.ReadAllBytes(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "..\\dbLibrary\\pic\\defaultCover.png");
                }
                else //有选择封面
                {
                    var binaryReader = new BinaryReader(file.InputStream);
                    imageByte = binaryReader.ReadBytes(file.ContentLength);
                }
                model.coverPic = imageByte;
                dc.NN_newsItem.Add(model);
                dc.SaveChanges();
                return model.rowID;
            }
            catch(DbEntityValidationException ex)
            {
                rui.excptHelper.throwEx(ex.Message);
            }
            return "-1";
        }


        public static db.NN_newsItem getModelByRowID(string rowID, dbEntities dc)
        {
            return dc.NN_newsItem.Where(a => a.rowID == rowID).SingleOrDefault();
        }

        public static db.NN_newsItem getModelByNewsID(string newsID, dbEntities dc)
        {
            return dc.NN_newsItem.Where(a => a.newsID == newsID).SingleOrDefault();
        }


        public static db.NN_newsItem getModelByNewsID_NonDc(string newsID)
        {
            dbEntities dc = new dbEntities();
            return dc.NN_newsItem.Where(a => a.newsID == newsID).SingleOrDefault();
        }
        public static int getLikeByNewsID(string newsID)
        {
            dbEntities dc = new dbEntities();
            return dc.NN_like.Where(a => a.newsID == newsID).Count();
        }

        public static int getStarByNewsID(string newsID)
        {
            dbEntities dc = new dbEntities();
            return dc.NN_star.Where(a => a.newsID == newsID).Count();
        }

        public static int getOutlookByNewsID(string newsID)
        {
            dbEntities dc = new dbEntities();
            return (int)dc.NN_newsItem.Where(a => a.newsID == newsID).SingleOrDefault().readQuantity;
        }
        public static int getCommitByNewsID(string newsID)
        {
            dbEntities dc = new dbEntities();
            return dc.NN_comment.Where(a => a.newsID == newsID).Count();
        }
            
        public static string getPublishDateByNewsID(string newsID)
        {
            dbEntities dc = new dbEntities();
            return dc.NN_newsItem.Where(a => a.newsID == newsID).SingleOrDefault().publishDateTime.ToString("yyy-MM-dd:HH:mm:ss");
        }

        public static string getNewsTitleBynewsID(string newsID)
        {
            dbEntities dc = new dbEntities();
            if (newsID.Length < 1) return "";
            return dc.NN_newsItem.Where(a => a.newsID == newsID).SingleOrDefault().newsTitle;
        }

        public static string getUserName(string useID)
        {
            dbEntities dc = new dbEntities();
            return dc.NN_user.Where(a => a.userID == useID).SingleOrDefault().userName;
        }

        public static string getnewID(string rowID)
        {
            dbEntities dc = new dbEntities();
            return dc.NN_newsItem.Where(a => a.rowID == rowID).SingleOrDefault().newsID;
        }

        public static string batchDelete(string keyFieldValues, dbEntities dc)//批量删除
        {
            efHelper ef = new efHelper(ref dc);
            List<string> KeyFieldList = rui.dbTools.getList(keyFieldValues);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            string sqlCheck = "select newsID  from NN_newsItem where newsID in @newsID";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { newsID = KeyFieldList });
            foreach(string newsID in KeyFieldList)
            {
                try
                {

                    dc.NN_newsItem.Remove(dc.NN_newsItem.Where(a => a.newsID == newsID).SingleOrDefault());                   
                }
                catch
                {
                    errorDic.Add(newsID, "该条目有关联项目，目前不允许删除，后面会设置一个练级删除");
                }
            }
            dc.SaveChanges();
            return rui.dbTools.getBatchMsg("批量删除", KeyFieldList.Count, errorDic);
        }




        public static string batchBan(string keyFieldValues, dbEntities dc)//批量禁止
        {
            efHelper ef = new efHelper(ref dc);
            List<string> KeyFieldList = rui.dbTools.getList(keyFieldValues);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            string sqlCheck = "select newsID, status  from NN_newsItem where newsID in @newsID";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { newsID = KeyFieldList });
            foreach (string newsID in KeyFieldList)
            {
                try
                {
                    string sql = " UPDATE NN_newsItem SET status = 1 WHERE newsID=@newsID ";
                    DataRow[] rows = table.Select("newsID='" + newsID + "'");
                    rui.dbTools.checkRowFieldValue(rows, "status", "1", "已处于封禁状态");
                    if (ef.Execute(sql, new { newsID }) == 0)
                    {
                        errorDic.Add(newsID, "发生错误，未成功设置");
                    }
                }
                catch(Exception ex)
                {
                    errorDic.Add(newsID, "发生错误" + rui.excptHelper.getExMsg(ex));
                }
            }
            dc.SaveChanges();
            return rui.dbTools.getBatchMsg("批量封禁", KeyFieldList.Count, errorDic);
        }


        

        public static string batchRecovey(string keyFieldValues, dbEntities dc)//批量解封
        {
            efHelper ef = new efHelper(ref dc);
            List<string> KeyFieldList = rui.dbTools.getList(keyFieldValues);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            string sqlCheck = "select newsID, status  from NN_newsItem where newsID in @newsID";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { newsID = KeyFieldList });
            foreach (string newsID in KeyFieldList)
            {
                try
                {
                    string sql = " UPDATE NN_newsItem SET status = 0 WHERE newsID=@newsID ";
                    DataRow[] rows = table.Select("newsID='" + newsID + "'");
                    rui.dbTools.checkRowFieldValue(rows, "status", "0", "状态正常，不需要解封");
                    if (ef.Execute(sql, new { newsID }) == 0)
                    {
                        errorDic.Add(newsID, "发生错误，未成功设置");
                    }
                }
                catch (Exception ex)
                {
                    errorDic.Add(newsID, "发生错误" + rui.excptHelper.getExMsg(ex));
                }
            }
            dc.SaveChanges();
            return rui.dbTools.getBatchMsg("批量解封", KeyFieldList.Count, errorDic);
        }



        public static string ChangeState(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            try
            {
                string sqlCheck = " SELECT newsID,status FROM NN_newsItem WHERE rowID = '" + rowID + "'";
                DataRow item = ef.ExecuteDataRow(sqlCheck);
                if (item["status"].ToString() == "0")
                {
                    string sql = " UPDATE NN_newsItem SET status=1 WHERE rowID= '" + rowID + "'";
                    if (ef.Execute(sql, new { rowID }) == 0)
                        rui.excptHelper.throwEx("数据未变更");
                }
                else if (item["status"].ToString() == "1")
                {
                    string sql = " UPDATE NN_newsItem SET status=0 WHERE rowID= '" + rowID + "'";
                    if (ef.Execute(sql, new { rowID }) == 0)
                        rui.excptHelper.throwEx("数据未变更");
                }
            }
            catch (Exception ex)
            {
                errorDic.Add(rowID, rui.excptHelper.getExMsg(ex));
                rui.logHelper.log(ex);
            }
            return rui.dbTools.getBatchMsg("完成操作", 1, errorDic);
        }

        public static void deleteNN_newsItem(string rowID, dbEntities dc)
        {
            try
            {
                db.NN_newsItem model = dc.NN_newsItem.Where(a => a.rowID == rowID).SingleOrDefault();
                dc.NN_newsItem.Remove(model);
                dc.SaveChanges();
            }
            catch (Exception ex)
            {
                rui.excptHelper.throwEx("有关联信息，不允许删除");
            }
        }

        public static string update(db.NN_newsItem model, string list, HttpPostedFileBase newProfile,  dbEntities dc)
        {
            try
            {
                string rowID = model.rowID;
                efHelper.entryUpdate(model, dc);
                foreach(var i in dc.NN_newsCategroryItem.Where(a => a.newsID == model.newsID).ToList())
                {
                    dc.NN_newsCategroryItem.Remove(i);
                }
                foreach(var i in rui.dbTools.getList(list))
                {
                    db.NN_newsCategroryItem entry = new db.NN_newsCategroryItem();
                    entry.newsID = model.newsID;
                    entry.newsCateGoryID = i;
                    entry.rowID = new efHelper().newGuid();
                    dc.NN_newsCategroryItem.Add(entry);
                }
                if (newProfile != null)
                    model.coverPic = new BinaryReader(newProfile.InputStream).ReadBytes(newProfile.ContentLength);
                dc.SaveChanges();
                return rowID;
            }
            catch(DbEntityValidationException ex)
            {
                rui.excptHelper.throwEx(ex.Message);
            }
            return "-1";
         
        }


        /// <summary>
        /// 随机返回新闻内容，实现随机推荐的功能
        /// </summary>
        /// <param name="count">返回list的长度</param>
        /// <returns></returns>

        public static List<db.NN_newsItem> getRandomRecommendNewsItem(int count) 
        {
            List<db.NN_newsItem> ans = new List<db.NN_newsItem>();
            dbEntities dc = new dbEntities();
            efHelper ef = new efHelper(ref dc);
            string sql = "SELECT TOP (@Count) * FROM NN_newsItem where status = 0  ORDER BY NEWID()";
            DataTable table = ef.ExecuteDataTable(sql, new {Count = count});
            List<string> col = new List<string>() {"rowNum","rowID", "newsID","newsTitle", "userID","readQuantity","publishDateTime","content","status","coverPic","describe" };
            foreach (DataRow item in table.Rows)
            {
                db.NN_newsItem model = new db.NN_newsItem();
                int idx = 0;
                
                model.rowNum = (long)item[col[idx++]];
                model.rowID = (string)item[col[idx++]];
                model.newsID = (string)item[col[idx++]];
                model.newsTitle = (string)item[col[idx++]];
                model.userID = (string)item[col[idx++]];
                model.readQuantity = (long)item[col[idx++]];
                model.publishDateTime = (DateTime)item[col[idx++]];
                model.content = (string)item[col[idx++]];
                model.status = (int)item[col[idx++]];
                model.coverPic = (byte[])item[col[idx++]];
                model.describe = (string)item[col[idx++]];
                ans.Add(model);
            }
            return ans;
        }

        //按阅读量排序返回新闻
        public static List<db.NN_newsItem> getModelListReadQuanByCount(int count)
        {
            List<db.NN_newsItem> ans = new List<db.NN_newsItem>();
            dbEntities dc = new dbEntities();
            efHelper ef = new efHelper(ref dc);
            string sql = "SELECT TOP (@Count) * FROM NN_newsItem where status = 0 ORDER BY readQuantity desc";
            DataTable table = ef.ExecuteDataTable(sql, new { Count = count });
            List<string> col = new List<string>() { "rowNum", "rowID", "newsID", "newsTitle", "userID", "readQuantity", "publishDateTime", "content", "status", "coverPic", "describe" };
            foreach (DataRow item in table.Rows)
            {
                db.NN_newsItem model = new db.NN_newsItem();
                int idx = 0;

                model.rowNum = (long)item[col[idx++]];
                model.rowID = (string)item[col[idx++]];
                model.newsID = (string)item[col[idx++]];
                model.newsTitle = (string)item[col[idx++]];
                model.userID = (string)item[col[idx++]];
                model.readQuantity = (long)item[col[idx++]];
                model.publishDateTime = (DateTime)item[col[idx++]];
                model.content = (string)item[col[idx++]];
                model.status = (int)item[col[idx++]];
                model.coverPic = (byte[])item[col[idx++]];
                model.describe = (string)item[col[idx++]];
                ans.Add(model);
            }
            return ans;
        }
        /// <summary>
        /// 按照发布时间
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<db.NN_newsItem> getModelListPublishTimeByCount(int count)
        {
            List<db.NN_newsItem> ans = new List<db.NN_newsItem>();
            dbEntities dc = new dbEntities();
            efHelper ef = new efHelper(ref dc);
            string sql = "SELECT TOP (@Count) * FROM NN_newsItem where status = 0 ORDER BY publishDateTime desc";
            DataTable table = ef.ExecuteDataTable(sql, new { Count = count });
            List<string> col = new List<string>() { "rowNum", "rowID", "newsID", "newsTitle", "userID", "readQuantity", "publishDateTime", "content", "status", "coverPic", "describe" };
            foreach (DataRow item in table.Rows)
            {
                db.NN_newsItem model = new db.NN_newsItem();
                int idx = 0;

                model.rowNum = (long)item[col[idx++]];
                model.rowID = (string)item[col[idx++]];
                model.newsID = (string)item[col[idx++]];
                model.newsTitle = (string)item[col[idx++]];
                model.userID = (string)item[col[idx++]];
                model.readQuantity = (long)item[col[idx++]];
                model.publishDateTime = (DateTime)item[col[idx++]];
                model.content = (string)item[col[idx++]];
                model.status = (int)item[col[idx++]];
                model.coverPic = (byte[])item[col[idx++]];
                model.describe = (string)item[col[idx++]];
                ans.Add(model);
            }
            return ans;
        }
        /// <summary>
        /// 根据类别名字返回新闻
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<db.NN_newsItem> getModelListCategoryByCount(string name, int count)
        {
            List<db.NN_newsItem> ans = new List<db.NN_newsItem>();
            dbEntities dc = new dbEntities();
            efHelper ef = new efHelper(ref dc);
            string catrgoryID = dc.NN_newsCategory.Where(a => a.newsCategoryName == name).SingleOrDefault().newsCategoryID;
            var newsIDModel = dc.NN_newsCategroryItem.Where(a => a.newsCateGoryID == catrgoryID).ToList<db.NN_newsCategroryItem>();
            List<string> list = new List<string>();
            foreach(var x in newsIDModel)
            {
                list.Add(x.newsID);
            }
            string sql = "SELECT TOP (@Count) * FROM NN_newsItem where newsID in @list and  status = 0 order by publishDateTime desc";
            DataTable table = ef.ExecuteDataTable(sql, new { Count = count, list = list });
            List<string> col = new List<string>() { "rowNum", "rowID", "newsID", "newsTitle", "userID", "readQuantity", "publishDateTime", "content", "status", "coverPic", "describe" };
            foreach (DataRow item in table.Rows)
            {
                db.NN_newsItem model = new db.NN_newsItem();
                int idx = 0;
                model.rowNum = (long)item[col[idx++]];
                model.rowID = (string)item[col[idx++]];
                model.newsID = (string)item[col[idx++]];
                model.newsTitle = (string)item[col[idx++]];
                model.userID = (string)item[col[idx++]];
                model.readQuantity = (long)item[col[idx++]];
                model.publishDateTime = (DateTime)item[col[idx++]];
                model.content = (string)item[col[idx++]];
                model.status = (int)item[col[idx++]];
                model.coverPic = (byte[])item[col[idx++]];
                model.describe = (string)item[col[idx++]];
                ans.Add(model);
            }
            return ans;
        }
        

        public static List<db.NN_newsItem> getModelLIstByCateGoryID(string categoryID, dbEntities dc)
        {
            List<db.NN_newsItem> list = new List<db.NN_newsItem>();
            efHelper ef = new efHelper(ref dc);
            var tmp = dc.NN_newsCategroryItem.Where(a => a.newsCateGoryID == categoryID).OrderBy(a => a.newsCateGoryID).ToList<db.NN_newsCategroryItem>();
            List<string> newsIDList = new List<string>();
            foreach(var v in tmp)
            {
                newsIDList.Add(v.newsID);
            }
            string sql = "SELECT * FROM NN_newsItem where newsID in @newsIDList and  status = 0 order by newsID desc";
            DataTable table = ef.ExecuteDataTable(sql, new { newsIDList = newsIDList });
            List<string> col = new List<string>() { "rowNum", "rowID", "newsID", "newsTitle", "userID", "readQuantity", "publishDateTime", "content", "status", "coverPic", "describe" };
            foreach (DataRow item in table.Rows)
            {
                db.NN_newsItem model = new db.NN_newsItem();
                int idx = 0;
                model.rowNum = (long)item[col[idx++]];
                model.rowID = (string)item[col[idx++]];
                model.newsID = (string)item[col[idx++]];
                model.newsTitle = (string)item[col[idx++]];
                model.userID = (string)item[col[idx++]];
                model.readQuantity = (long)item[col[idx++]];
                model.publishDateTime = (DateTime)item[col[idx++]];
                model.content = (string)item[col[idx++]];
                model.status = (int)item[col[idx++]];
                model.coverPic = (byte[])item[col[idx++]];
                model.describe = (string)item[col[idx++]];
                list.Add(model);
            }
            return list;
        }
        public static void readQuantityPP(string newsID, dbEntities dc)
        {
            var model = dc.NN_newsItem.Where(a => a.newsID == newsID).SingleOrDefault();
            model.readQuantity++;
            dc.SaveChanges();
        }


        public static List<db.NN_newsItem> NNsearch(string content, dbEntities dc)
        {
            List<db.NN_newsItem> list = new List<db.NN_newsItem>();
            efHelper ef = new efHelper(ref dc);
            List<string> col = new List<string>() { "rowNum", "rowID", "newsID", "newsTitle", "userID", "readQuantity", "publishDateTime", "content", "status", "coverPic", "describe" };
            //按标题搜索
            string sql = "SELECT * FROM NN_newsItem where newsTitle like @newsTitle and  status = 0 order by newsID";
            string newsTitle = "%" + content + "%";
            DataTable tableOne = ef.ExecuteDataTable(sql, new { newsTitle = newsTitle });
            foreach (DataRow item in tableOne.Rows)
            {
                db.NN_newsItem model = new db.NN_newsItem();
                int idx = 0;
                model.rowNum = (long)item[col[idx++]];
                model.rowID = (string)item[col[idx++]];
                model.newsID = (string)item[col[idx++]];
                model.newsTitle = (string)item[col[idx++]];
                model.userID = (string)item[col[idx++]];
                model.readQuantity = (long)item[col[idx++]];
                model.publishDateTime = (DateTime)item[col[idx++]];
                model.content = (string)item[col[idx++]];
                model.status = (int)item[col[idx++]];
                model.coverPic = (byte[])item[col[idx++]];
                model.describe = (string)item[col[idx++]];
                list.Add(model);
            }

            //按作者名称搜索
            List<string> authorIDList = new List<string>();
            string sql1 = "SELECT * FROM NN_user where userName like @userName and  status = 0 order by userID"; //先拿到名字所对应的ID再按ID搜
            string userName = "%" + content + "%";
            DataTable tabletwo = ef.ExecuteDataTable(sql1, new { userName = userName});
            foreach(DataRow item in tabletwo.Rows)
            {
                authorIDList.Add((string)item["userID"]);
            }
            string sql2 = "SELECT * FROM NN_newsItem where userID in @authorIDList and status = 0 order by newsID"; //用作者ID搜索
            DataTable tablwThree = ef.ExecuteDataTable(sql2, new { authorIDList = authorIDList });
            foreach (DataRow item in tablwThree.Rows)
            {
                db.NN_newsItem model = new db.NN_newsItem();
                int idx = 0;
                model.rowNum = (long)item[col[idx++]];
                model.rowID = (string)item[col[idx++]];
                model.newsID = (string)item[col[idx++]];
                model.newsTitle = (string)item[col[idx++]];
                model.userID = (string)item[col[idx++]];
                model.readQuantity = (long)item[col[idx++]];
                model.publishDateTime = (DateTime)item[col[idx++]];
                model.content = (string)item[col[idx++]];
                model.status = (int)item[col[idx++]];
                model.coverPic = (byte[])item[col[idx++]];
                model.describe = (string)item[col[idx++]];
                list.Add(model);
            }

            //按新闻标签名字搜索
            string sql3 = "SELECT * FROM NN_newsCategory where newsCategoryName like @newsCategoryName and status = '0'";//先找到标签的ID
            List<string> tagIDList = new List<string>();
            string newsCategoryName = content;
            DataTable tablethree = ef.ExecuteDataTable(sql3, new { newsCategoryName = newsCategoryName });
            foreach (DataRow item in tablethree.Rows)
            {
                tagIDList.Add((string)item["newsCategoryID"]);
            }
            string sql4 = "SELECT * FROM NN_newsCategroryItem where newsCateGoryID in @newsCateGoryID";
            DataTable tablefour = ef.ExecuteDataTable(sql4, new { newsCateGoryID = tagIDList });  //这里是装的ID
            List<string> newsIDList = new List<string>();
            foreach(DataRow item in tablefour.Rows)
            {
                newsIDList.Add((string)item["newsID"]);
            }
            string sql5 = "SELECT * FROM NN_newsItem where newsID in @newsIDList and status = 0 order by newsID";
            DataTable tablwfive = ef.ExecuteDataTable(sql5, new { newsIDList = newsIDList });
            foreach (DataRow item in tablwfive.Rows)
            {
                db.NN_newsItem model = new db.NN_newsItem();
                int idx = 0;
                model.rowNum = (long)item[col[idx++]];
                model.rowID = (string)item[col[idx++]];
                model.newsID = (string)item[col[idx++]];
                model.newsTitle = (string)item[col[idx++]];
                model.userID = (string)item[col[idx++]];
                model.readQuantity = (long)item[col[idx++]];
                model.publishDateTime = (DateTime)item[col[idx++]];
                model.content = (string)item[col[idx++]];
                model.status = (int)item[col[idx++]];
                model.coverPic = (byte[])item[col[idx++]];
                model.describe = (string)item[col[idx++]];
                list.Add(model);
            }
            return list;

        }
        public static void insert_LeadingEnd(string title, string describe, HttpPostedFileBase file, string list, string richtext, dbEntities dc)
        {
            try
            {
                efHelper ef = new efHelper(ref dc);
                db.NN_newsItem model = new db.NN_newsItem();
                model.newsID = _createID(dc);
                model.rowID = ef.newGuid();
                model.newsTitle = title;
                model.describe = describe;
                model.content = richtext;
                byte[] imageByte;
                if (file == null)
                {
                    imageByte = File.ReadAllBytes(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "..\\dbLibrary\\pic\\defaultCover.png");
                }
                else //有选择封面
                {
                    var binaryReader = new BinaryReader(file.InputStream);
                    imageByte = binaryReader.ReadBytes(file.ContentLength);
                }
                model.coverPic = imageByte;
                model.status = 0;
                model.publishDateTime = DateTime.Now;
                model.readQuantity = 0;
                model.userID = rui.sessionHelper.getValue("userID");
                List<string> list1 = rui.dbTools.getList(list, ' ');
                foreach (var item in list1)
                {
                    db.NN_newsCategroryItem entry = new db.NN_newsCategroryItem();
                    entry.newsID = model.newsID;
                    entry.newsCateGoryID = item;
                    entry.rowID = ef.newGuid();
                    dc.NN_newsCategroryItem.Add(entry);
                }
                dc.NN_newsItem.Add(model);
                dc.SaveChanges();
            }
            catch(Exception ex)
            {
                rui.excptHelper.throwEx(ex.Message);
            }
        }


    }

    
}
