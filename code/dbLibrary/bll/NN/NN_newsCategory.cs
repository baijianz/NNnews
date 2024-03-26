using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace db.bll
{
    public class NN_newsCategory
    {
        public static List<SelectListItem> bind账号状态(bool has请选择 = false, string selectedValues = "")
        {
            dbEntities dc = new dbEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            rui.listHelper.add请选择(list, has请选择);
            list.Add(new SelectListItem { Text = "正常", Value = "0" });
            list.Add(new SelectListItem { Text = "封禁", Value = "1" });
            return list;
        }

        public static string getIDByName(string Name)
        {
            dbEntities dc = new dbEntities();
            return dc.NN_newsCategory.Where(a => a.newsCategoryName == Name).SingleOrDefault().newsCategoryID;
        }
        public static db.NN_newsCategory getModelByRowID(string rowID, dbEntities dc)
        {
            return dc.NN_newsCategory.Where(a => a.rowID == rowID).FirstOrDefault();
        }


        public static List<KeyValuePair<string, string>> getWords()
        {
            dbEntities dc = new dbEntities();
            List<KeyValuePair<string, string>> ans = new List<KeyValuePair<string, string>>();
            foreach (var v in dc.NN_newsCategory)
            {
                var name = v.newsCategoryName;
                var val = v.newsCategoryID;
                ans.Add(new KeyValuePair<string ,string>(name, val));
            }
            return ans;
        }
        public static string update(db.NN_newsCategory model, dbEntities dc)
        {
            if (dc.NN_newsCategory.Count(a => a.newsCategoryName == model.newsCategoryName) > 0)
            {
                rui.excptHelper.throwEx("已存在相同的标签");
            }
            string rowID = model.rowID;
            efHelper.entryUpdate(model, dc);
            dc.SaveChanges();
            return rowID;
        }

        private static string _createID(dbEntities dc)
        {
            string code = "NNS";
            string result = (from a in dc.NN_newsCategory where a.newsCategoryID.StartsWith(code) select a.newsCategoryID).Max();
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
        private static void _checkINput(db.NN_newsCategory model)
        {
            efHelper.checkNotNull(model.newsCategoryID, "标签ID");
        }

        public static string insert(db.NN_newsCategory model, dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            if (rui.typeHelper.isNullOrEmpty(model.newsCategoryID))
            {
                model.newsCategoryID = _createID(dc);
            }
            if (dc.NN_newsCategory.Count(a => a.newsCategoryName == model.newsCategoryName) > 0)
            {
                //return rui.dbTools.getBatchMsg("新增", 1, new Dictionary<string, string>(){ { "", "该机构已经存在" } });
                rui.excptHelper.throwEx("该机构已存在");
            }
            _checkINput(model);
            model.rowID = ef.newGuid();
            dc.NN_newsCategory.Add(model);
            dc.SaveChanges();
            return model.rowID;
        }

        public static void deleteNewsCategory(string rowID, dbEntities dc)
        {
            try
            {
                dc.NN_newsCategory.Remove(dc.NN_newsCategory.Where(a => a.rowID == rowID).SingleOrDefault());
                dc.SaveChanges();
            }
            catch
            {
                rui.excptHelper.throwEx("删除失败");
            }
        }

        public static string ChangeState(string rowID, dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            try
            {
                string sql = string.Format("select newsCategoryID as code, status from NN_newsCategory where rowID = '{0}'", rowID);
                DataRow item = ef.ExecuteDataRow(sql);
                if (item["status"].ToString() == "0") //正常
                {
                    string updateSql = string.Format("update NN_newsCategory set status=1 where rowID = '{0}'", rowID);
                    if (ef.Execute(updateSql, new { rowID }) == 0)
                    {
                        rui.excptHelper.throwEx("数据未变更");
                    }
                }
                else
                {
                    string updateSql = string.Format("update NN_newsCategory set status=0 where rowID = '{0}'", rowID);
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
            string sqlCheck = "select newsCategoryID  from NN_newsCategory where newsCategoryID in @newsCategoryID";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { newsCategoryID = KeyFieldList });
            foreach (string newsCategoryID in KeyFieldList)
            {
                try
                {

                    dc.NN_newsCategory.Remove(dc.NN_newsCategory.Where(a => a.newsCategoryID == newsCategoryID).SingleOrDefault());
                }
                catch
                {
                    errorDic.Add(newsCategoryID, "该条目有关联项目，目前不允许删除，后面会设置一个练级删除");
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
            string sqlCheck = "select newsCategoryID, status  from NN_newsCategory where newsCategoryID in @newsCategoryID";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { newsCategoryID = KeyFieldList });
            foreach (string newsCategoryID in KeyFieldList)
            {
                try
                {
                    string sql = " UPDATE NN_newsCategory SET status = 1 WHERE newsCategoryID=@newsCategoryID ";
                    DataRow[] rows = table.Select("newsCategoryID='" + newsCategoryID + "'");
                    rui.dbTools.checkRowFieldValue(rows, "status", "1", "已处于封禁状态");
                    if (ef.Execute(sql, new { newsCategoryID }) == 0)
                    {
                        errorDic.Add(newsCategoryID, "发生错误，未成功设置");
                    }
                }
                catch (Exception ex)
                {
                    errorDic.Add(newsCategoryID, "发生错误:" + rui.excptHelper.getExMsg(ex));
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
            string sqlCheck = "select newsCategoryID, status from NN_newsCategory where newsCategoryID in @newsCategoryID";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { newsCategoryID = KeyFieldList });
            foreach (string newsCategoryID in KeyFieldList)
            {
                try
                {
                    string sql = " UPDATE NN_newsCategory SET status = 0 WHERE newsCategoryID=@newsCategoryID ";
                    DataRow[] rows = table.Select("newsCategoryID='" + newsCategoryID + "'");
                    rui.dbTools.checkRowFieldValue(rows, "status", "0", "状态正常，不需要解封");
                    if (ef.Execute(sql, new { newsCategoryID }) == 0)
                    {
                        errorDic.Add(newsCategoryID, "发生错误，未成功设置");
                    }
                }
                catch (Exception ex)
                {
                    errorDic.Add(newsCategoryID, "发生错误:" + rui.excptHelper.getExMsg(ex));
                }
            }
            dc.SaveChanges();
            return rui.dbTools.getBatchMsg("批量解封", KeyFieldList.Count, errorDic);
        }

        public static List<string> getWordList(int count)
        {
            dbEntities dc = new dbEntities();
            var list = dc.NN_newsCategory.Where(a => a.status == "0").OrderBy(a => a.newsCategoryID).Take(3).ToList<db.NN_newsCategory>();
            List<string> ans = new List<string>();
            foreach(var x in list)
            {
                ans.Add(x.newsCategoryName);
            }
            return ans;
        }

        public static List<db.NN_newsCategory> getWordListAll()
        {
            dbEntities dc = new dbEntities();
            var list = dc.NN_newsCategory.Where(a => a.status == "0").OrderBy(a => a.newsCategoryID).ToList<db.NN_newsCategory>();
            return list;
        }

        public static List<db.NN_newsCategory> getWordListAll_ef()
        {
            dbEntities dc = new dbEntities();
            efHelper ef = new efHelper(ref dc);
            string sql = "select * from  NN_newsCategory where status='0'";
            DataTable table = ef.ExecuteDataTable(sql);
            List<db.NN_newsCategory> list = new List<db.NN_newsCategory>();
            List<string> col = new List<string>() { "rowNum", "rowID", "newsCategoryID", "newsCategoryName", "status", "remark" };
            foreach(DataRow item in table.Rows)
            {
                int index = 0;
                db.NN_newsCategory model = new db.NN_newsCategory();
                model.rowNum = (long)item[col[index++]];
                model.rowID = (string)item[col[index++]];
                model.newsCategoryID = (string)item[col[index++]];
                model.newsCategoryName = (string)item[col[index++]];
                model.status = (string)item[col[index++]];
                model.remark = item[col[index++]].ToString();
                list.Add(model);
            }
            return list;
        }

        public static string getNameByID(string ID, dbEntities dc)
        {
            return dc.NN_newsCategory.Where(a => a.newsCategoryID == ID).SingleOrDefault().newsCategoryName;
        }

    }
}
