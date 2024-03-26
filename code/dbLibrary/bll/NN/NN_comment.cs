using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.bll
{
    public class NN_comment
    {

        public static string getNewsTitleByCommentID(string CommentID)
        {
            dbEntities dc = new dbEntities();
            string newsID = dc.NN_comment.Where(a => a.commentID == CommentID).SingleOrDefault().newsID;
            return dc.NN_newsItem.Where(a => a.newsID == newsID).SingleOrDefault().newsTitle;
        }

        public static string getContentByID(string ID)
        {
            dbEntities dc = new dbEntities();
            if (ID == null) return "";
            return dc.NN_comment.Where(a => a.commentID == ID).FirstOrDefault().commentContent;
        }

        public static string getModelstatusByID(string ID)
        {
            dbEntities dc = new dbEntities();
            return dc.NN_comment.Where(a => a.commentID == ID).SingleOrDefault().status.ToString();
        }

        public static string batchBan(string keyFieldValues, dbEntities dc)//批量禁止
        {
            efHelper ef = new efHelper(ref dc);
            List<string> KeyFieldList = rui.dbTools.getList(keyFieldValues);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            string sqlCheck = "select commentID, status  from  NN_comment where commentID in @commentID";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { commentID = KeyFieldList });
            foreach (string commentID in KeyFieldList)
            {
                try
                {
                    string sql = " UPDATE NN_comment SET status = 1 WHERE commentID=@commentID ";
                    DataRow[] rows = table.Select("commentID='" + commentID + "'");
                    rui.dbTools.checkRowFieldValue(rows, "status", "1", "已处于封禁状态");
                    if (ef.Execute(sql, new { commentID }) == 0)
                    {
                        errorDic.Add(commentID, "发生错误，未成功设置");
                    }
                }
                catch (Exception ex)
                {
                    errorDic.Add(commentID, "发生错误" + rui.excptHelper.getExMsg(ex));
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
            string sqlCheck = "select commentID, status  from  NN_comment where commentID in @commentID";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { commentID = KeyFieldList });
            foreach (string commentID in KeyFieldList)
            {
                try
                {
                    string sql = " UPDATE NN_comment SET status = 0 WHERE commentID=@commentID ";
                    DataRow[] rows = table.Select("commentID='" + commentID + "'");
                    rui.dbTools.checkRowFieldValue(rows, "status", "0", "已处于解封状态");
                    if (ef.Execute(sql, new { commentID }) == 0)
                    {
                        errorDic.Add(commentID, "发生错误，未成功设置");
                    }
                }
                catch (Exception ex)
                {
                    errorDic.Add(commentID, "发生错误" + rui.excptHelper.getExMsg(ex));
                }
            }
            dc.SaveChanges();
            return rui.dbTools.getBatchMsg("批量封禁", KeyFieldList.Count, errorDic);
        }

        public static int getCountBynewsID(string newsID)
        {
            dbEntities dc = new dbEntities();
            return dc.NN_comment.Where(a => a.newsID == newsID && a.status == 0).Count();
        }

        public static List<db.NN_comment> getCommentListByNewsID(string newsID, dbEntities dc)
        {
            return dc.NN_comment.Where(a => a.newsID == newsID && a.status == 0).OrderByDescending(a => a.commentTime).ToList<db.NN_comment>();
        }

        public static List<db.view.NN_comment_userly> GetComment_UserliesByNewsID_NonDc(string newsID)
        {
            List<db.view.NN_comment_userly> list = new List<view.NN_comment_userly>();
            dbEntities dc = new dbEntities();
            var temp = dc.NN_comment.Where(a => a.newsID == newsID && a.status == 0).OrderByDescending(a => a.commentTime).ToList<db.NN_comment>();
            int index = 0;
            foreach(var item in temp)
            {
                string userID = item.userID;
                db.view.NN_comment_userly model = new db.view.NN_comment_userly();
                model.index = index++;
                model.commentID = item.commentID;
                model.Name = dc.NN_user.Where(a => a.userID == userID).SingleOrDefault().userName;
                model.proFile = dc.NN_user.Where(a => a.userID == userID).SingleOrDefault().profile;
                model.rowID = item.rowID;
                model.rowNum = item.rowNum;
                model.commentContent = item.commentContent;
                model.commentTime = item.commentTime;
                model.fatherCommentID = item.fatherCommentID;
                model.newsID = item.newsID;
                model.userID = item.userID;
                model.status = item.status;
                if (model.fatherCommentID != null)
                {
                    var tmpModel = dc.NN_comment.Where(a => a.commentID == model.fatherCommentID).SingleOrDefault();
                    model.fatherCommentUserName = dc.NN_user.Where(a => a.userID == tmpModel.userID).SingleOrDefault().userName;
                }
                list.Add(model);
            }
            return list;
        }

        private static string Create_ID(dbEntities dc)
        {
            string code = "NNC";
            string result = (from a in dc.NN_comment where a.commentID.StartsWith(code) select a.commentID).Max();
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
        public static void insertComment(string content, string newsID, string userID, string fatherCommentID, dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            db.NN_comment model = new db.NN_comment();
            model.commentID = Create_ID(dc);
            model.rowID = ef.newGuid();
            model.commentTime = DateTime.Now;
            model.commentContent = content;
            model.newsID = newsID;
            model.userID = userID;
            if(fatherCommentID != "-1")
            {
                model.fatherCommentID = fatherCommentID;
            }
            model.status = 0;
            try
            {
                dc.NN_comment.Add(model);
                dc.SaveChanges();
            }
            catch(Exception ex)
            {
                rui.excptHelper.throwEx(ex.Message);
            }
        }
    }


}
