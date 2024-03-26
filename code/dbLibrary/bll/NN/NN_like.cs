using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.bll
{
    public class NN_like
    {
        public static void insert(string userID, string newsID, dbEntities dc)
        {
            db.NN_like model = new db.NN_like();
            model.newsID = newsID;
            model.userID = userID;
            model.rowID = new efHelper(ref dc).newGuid();
            try
            {
                dc.NN_like.Add(model);
                dc.SaveChanges();
            }
            catch(Exception ex)
            {
                rui.excptHelper.throwEx(ex.Message);
            }
        }

        public static void delete(string userID, string newsID, dbEntities dc)
        {
            var model = dc.NN_like.Where(a => a.userID == userID && a.newsID == newsID).SingleOrDefault();
            try
            {
                dc.NN_like.Remove(model);
                dc.SaveChanges();
            }
            catch(Exception ex)
            {
                rui.excptHelper.throwEx(ex.Message);
            }
        }

        public static List<db.NN_newsItem> getNewsModelList(string userID, dbEntities dc)
        {
            List<string> newsIDLIst = new List<string>();
            efHelper ef = new efHelper(ref dc);
            var tmp = dc.NN_like.Where(a => a.userID == userID ).ToList<db.NN_like>();
            foreach(var i in tmp)
            {
                newsIDLIst.Add(i.newsID);
            }
            List<db.NN_newsItem> list = new List<db.NN_newsItem>();
            string sql = "select *  from NN_newsItem where newsID in @newsIDLIst";
            DataTable table = ef.ExecuteDataTable(sql, new { newsIDLIst = newsIDLIst });
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
    }
}
