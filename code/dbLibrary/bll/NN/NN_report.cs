using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace db.bll
{
    public class NN_report
    {
        public static List<SelectListItem> bind处理状态(bool has请选择 = false, string selectedValues = "")
        {
            dbEntities dc = new dbEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            rui.listHelper.add请选择(list, has请选择);
            list.Add(new SelectListItem { Text = "未处理", Value = "0" });
            list.Add(new SelectListItem { Text = "已处理", Value = "1" });
            return list;
        }

        public static List<SelectListItem> bind举报类别(bool has请选择 = false, string selectedValues = "")
        {
            dbEntities dc = new dbEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            rui.listHelper.add请选择(list, has请选择);
            list.Add(new SelectListItem { Text = "新闻", Value = "0" });
            list.Add(new SelectListItem { Text = "评论", Value = "1" });
            return list;
        }

        public static db.NN_report getModelByRowID(string rowID, dbEntities dc)
        {
            return dc.NN_report.Where(a => a.rowID == rowID).SingleOrDefault();
        }
        public static db.view.NN_report_userly getNewsItemByRowID(string rowID, dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            db.NN_report report = getModelByRowID(rowID, dc);
            string newsID = report.newsID;
            string CommentID = report.commentID;
            if (newsID == null) //只有commentID
            {
                newsID = dc.NN_comment.Where(a => a.commentID == CommentID).SingleOrDefault().newsID;
            }
            db.NN_newsItem news = dc.NN_newsItem.Where(a => a.newsID == newsID).SingleOrDefault();
            db.view.NN_report_userly model = new view.NN_report_userly(news, report);
            return model;
        }

        public static string batchBan(string keyFieldValues, dbEntities dc)//批量禁止
        {
            efHelper ef = new efHelper(ref dc);
            List<string> KeyFieldList = rui.dbTools.getList(keyFieldValues);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            string sqlCheck = "select reportID, newsID, commentID, disposeStatus  from NN_report where reportID in @reportID";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { reportID = KeyFieldList });
            foreach (string reportID in KeyFieldList)
            {
                try
                {
                    string sql = " UPDATE NN_report SET disposeStatus = 1 WHERE reportID=@reportID ";
                    DataRow[] rows = table.Select("reportID='" + reportID + "'");
                    db.bll.NN_newsItem.batchBan(rows.SingleOrDefault()["newsID"].ToString(), dc);
                    db.bll.NN_comment.batchBan(rows.SingleOrDefault()["commentID"].ToString(), dc);
                    rui.dbTools.checkRowFieldValue(rows, "disposeStatus", "1", "该条目已经处理");
                    if (ef.Execute(sql, new { reportID }) == 0)
                    {
                        errorDic.Add(reportID, "发生错误，未成功设置");
                    }
                }
                catch (Exception ex)
                {
                    errorDic.Add(reportID, "发生错误：" + rui.excptHelper.getExMsg(ex));
                }
            }
            dc.SaveChanges();
            return rui.dbTools.getBatchMsg("批量封禁：", KeyFieldList.Count, errorDic);
        }

        public static string batchRecovey(string keyFieldValues, dbEntities dc)//批量解封
        {
            efHelper ef = new efHelper(ref dc);
            List<string> KeyFieldList = rui.dbTools.getList(keyFieldValues);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            string sqlCheck = "select reportID, newsID, commentID, disposeStatus  from NN_report where reportID in @reportID";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { reportID = KeyFieldList });
            foreach (string reportID in KeyFieldList)
            {
                try
                {
                    string sql = " UPDATE NN_report SET disposeStatus = 1 WHERE reportID=@reportID ";
                    DataRow[] rows = table.Select("reportID='" + reportID + "'");
                    db.bll.NN_newsItem.batchRecovey(rows.SingleOrDefault()["newsID"].ToString(), dc);
                    db.bll.NN_comment.batchRecovey(rows.SingleOrDefault()["commentID"].ToString(), dc);
                    rui.dbTools.checkRowFieldValue(rows, "disposeStatus", "1", "该条目已经处理");
                    if (ef.Execute(sql, new { reportID }) == 0)
                    {
                        errorDic.Add(reportID, "发生错误，未成功设置");
                    }
                }
                catch (Exception ex)
                {
                    errorDic.Add(reportID, "发生错误：" + rui.excptHelper.getExMsg(ex));
                }
            }
            dc.SaveChanges();
            return rui.dbTools.getBatchMsg("批量封禁：", KeyFieldList.Count, errorDic);
        }

        private static string Create_ID(dbEntities dc)
        {
            string code = "NNR";
            string result = (from a in dc.NN_report where a.reportID.StartsWith(code) select a.reportID).Max();
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

        public static void insert(string userID, string category, string newsID, string commentID, string reason, dbEntities dc)
        {
            db.NN_report model = new db.NN_report();
            model.reportID = Create_ID(dc);
            model.userID = userID;
            model.category = int.Parse(category);
            if (model.category == 0)
            {
                model.newsID = newsID;
            }
            else
            {
                model.commentID = commentID;
            }
            model.reason = reason;
            model.reportTimeDateTime = DateTime.Now;
            model.disposeStatus = 0;
            model.rowID = new efHelper(ref dc).newGuid();
            try
            {
                dc.NN_report.Add(model);
                dc.SaveChanges();
            }
            catch(Exception ex)
            {
                rui.excptHelper.throwEx(ex.Message);
            }
        }

    }
}
