using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace db.bll
{
    public class NN_NewsAgency
    {
        public static db.NN_NewsAgency getModelbyRowID(string rowID, dbEntities dc)
        {
            return dc.NN_NewsAgency.Where(a => a.rowID == rowID).SingleOrDefault();
        }

        public static List<SelectListItem> bind账号状态(bool has请选择 = false, string selectedValues = "")
        {
            dbEntities dc = new dbEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            rui.listHelper.add请选择(list, has请选择);
            list.Add(new SelectListItem { Text = "正常", Value = "0" });
            list.Add(new SelectListItem { Text = "封禁", Value = "1" });
            return list;
        }

        public static List<SelectListItem> bind机构(bool has请选择 = false, string selectedValues = "")
        {
            dbEntities dc = new dbEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            rui.listHelper.add请选择(list, has请选择);
            foreach(var i in dc.NN_NewsAgency.ToList())
            {
                list.Add(new SelectListItem { Text = i.newsAgencyName, Value = i.newsAgencyID });
            }
            return list;
        }

        public static List<SelectListItem> bind机构__(bool has请选择 = false, string selectedValues = "")
        {
            dbEntities dc = new dbEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            rui.listHelper.add请选择(list, has请选择);
            foreach (var i in dc.NN_NewsAgency.ToList())
            {
                if (i.newsAgencyName == "个人作者" || i.newsAgencyName == "NN新闻官方") continue;
                list.Add(new SelectListItem { Text = i.newsAgencyName, Value = i.newsAgencyID });
            }
            return list;
        }

        public static List<SelectListItem> bind机构_(bool has请选择 = false, string selectedValues = "")
        {
            dbEntities dc = new dbEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            rui.listHelper.add请选择(list, has请选择);
            foreach (var i in dc.NN_NewsAgency.ToList())
            {
                if (i.newsAgencyName == "NN新闻官方") continue;
                list.Add(new SelectListItem { Text = i.newsAgencyName, Value = i.newsAgencyID });
            }
            return list;
        }
        public static string update(db.NN_NewsAgency model, dbEntities dc)
        {
            string rowID = model.rowID;
            efHelper.entryUpdate(model, dc);
            dc.SaveChanges();
            return rowID;
        }


        public static string getNewsAgencyNameByID(string ID)
        {
            if (ID == null || ID == "") return "";
            dbEntities dc = new dbEntities();
            return dc.NN_NewsAgency.Where(a => a.newsAgencyID == ID).SingleOrDefault().newsAgencyName;
        }
        public static void deleteNewsAgencys(string rowID, dbEntities dc)
        {
            try
            {
                dc.NN_NewsAgency.Remove(dc.NN_NewsAgency.Where(a => a.rowID == rowID).SingleOrDefault());
                dc.SaveChanges();
            }
            catch
            {
                rui.excptHelper.throwEx("删除失败");
            }
        }

        private static string _createID(dbEntities dc)
        {
            string code = "NNI";
            string result = (from a in dc.NN_NewsAgency where a.newsAgencyID.StartsWith(code) select a.newsAgencyID).Max();
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
        private static void _checkINput(db.NN_NewsAgency model)
        {
            efHelper.checkNotNull(model.newsAgencyID, "机构ID");
        }

        public static string insert(db.NN_NewsAgency model, dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            if (rui.typeHelper.isNullOrEmpty(model.newsAgencyID))
            {
                model.newsAgencyID = _createID(dc);
            }
            if (dc.NN_NewsAgency.Count(a => a.newsAgencyName == model.newsAgencyName) > 0)
            {
                rui.excptHelper.throwEx("该机构已存在");
            }
            _checkINput(model);
            model.rowID = ef.newGuid();
            dc.NN_NewsAgency.Add(model);
            dc.SaveChanges();
            return model.rowID;
        }


        public static string ChangeState(string rowID, dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            try
            {
                string sql = string.Format("select newsAgencyID as code, status from NN_NewsAgency where rowID = '{0}'", rowID);
                DataRow item = ef.ExecuteDataRow(sql);
                if (item["status"].ToString() == "0") //正常
                {
                    string updateSql = string.Format("update NN_NewsAgency set status=1 where rowID = '{0}'", rowID);
                    if (ef.Execute(updateSql, new { rowID }) == 0)
                    {
                        rui.excptHelper.throwEx("数据未变更");
                    }
                }
                else
                {
                    string updateSql = string.Format("update NN_NewsAgency set status=0 where rowID = '{0}'", rowID);
                    if (ef.Execute(updateSql, new { rowID }) == 0)
                    {
                        rui.excptHelper.throwEx("数据未变更");
                    }
                }
                ef.commit();
            }
            catch (Exception ex)
            {
                errorDic.Add(rowID, rui.excptHelper.getExMsg(ex));
                rui.logHelper.log(ex);
            }
            return rui.dbTools.getBatchMsg("完成操作", 1, errorDic);
        }

        public static string batchDelete(string keyFieldValues, dbEntities dc)//批量删除
        {
            efHelper ef = new efHelper(ref dc);
            List<string> KeyFieldList = rui.dbTools.getList(keyFieldValues);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            string sqlCheck = "select newsAgencyID  from NN_NewsAgency where newsAgencyID in @newsAgencyID";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { newsAgencyID = KeyFieldList });
            foreach (string newsAgencyID in KeyFieldList)
            {
                try
                {

                    dc.NN_NewsAgency.Remove(dc.NN_NewsAgency.Where(a => a.newsAgencyID == newsAgencyID).SingleOrDefault());
                }
                catch
                {
                    errorDic.Add(newsAgencyID, "该条目有关联项目，目前不允许删除，后面会设置一个练级删除");
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
            string sqlCheck = "select newsAgencyID, status  from NN_NewsAgency where newsAgencyID in @newsAgencyID";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { newsAgencyID = KeyFieldList });
            foreach (string newsAgencyID in KeyFieldList)
            {
                try
                {
                    string sql = " UPDATE NN_NewsAgency SET status = 1 WHERE newsAgencyID=@newsAgencyID ";
                    DataRow[] rows = table.Select("newsAgencyID='" + newsAgencyID + "'");
                    rui.dbTools.checkRowFieldValue(rows, "status", "1", "已处于封禁状态");
                    if (ef.Execute(sql, new { newsAgencyID }) == 0)
                    {
                        errorDic.Add(newsAgencyID, "发生错误，未成功设置");
                    }
                }
                catch (Exception ex)
                {
                    errorDic.Add(newsAgencyID, "发生错误:" + rui.excptHelper.getExMsg(ex));
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
            string sqlCheck = "select newsAgencyID, status  from NN_NewsAgency where newsAgencyID in @newsAgencyID";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { newsAgencyID = KeyFieldList });
            foreach (string newsAgencyID in KeyFieldList)
            {
                try
                {
                    string sql = " UPDATE NN_NewsAgency SET status = 0 WHERE newsAgencyID=@newsAgencyID ";
                    DataRow[] rows = table.Select("newsAgencyID='" + newsAgencyID + "'");
                    rui.dbTools.checkRowFieldValue(rows, "status", "0", "状态正常，不需要解封");
                    if (ef.Execute(sql, new { newsAgencyID }) == 0)
                    {
                        errorDic.Add(newsAgencyID, "发生错误，未成功设置");
                    }
                }
                catch (Exception ex)
                {
                    errorDic.Add(newsAgencyID, "发生错误:" + rui.excptHelper.getExMsg(ex));
                }
            }
            dc.SaveChanges();
            return rui.dbTools.getBatchMsg("批量解封", KeyFieldList.Count, errorDic);
        }

    }
}
