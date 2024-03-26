using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace db.bll
{
    public class NN_apply
    {
        public static List<SelectListItem> bind类型(bool has请选择 = false, string selectedValues = "")
        {
            dbEntities dc = new dbEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            rui.listHelper.add请选择(list, has请选择);
            list.Add(new SelectListItem { Text = "注销账户", Value = " 3"});
            list.Add(new SelectListItem { Text = "申请成为内容创作者", Value = " 0" });
            list.Add(new SelectListItem { Text = "变更新闻机构", Value = " 1" });
            list.Add(new SelectListItem { Text = "注销内容创作者身份", Value = " 2" });
            return list;
        }

        public static List<SelectListItem> bind类型__(bool has请选择 = false, string selectedValues = "")
        {
            dbEntities dc = new dbEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            rui.listHelper.add请选择(list, has请选择);
            list.Add(new SelectListItem { Text = "注销账户", Value = "3" });
            list.Add(new SelectListItem { Text = "申请成为内容创作者", Value = "0" });
            list.Add(new SelectListItem { Text = "变更新闻机构", Value = "1" });
            list.Add(new SelectListItem { Text = "注销内容创作者身份", Value = "2" });
            return list;
        }
        public static List<SelectListItem> bind处理状态(bool has请选择 = false, string selectedValues = "")
        {
            dbEntities dc = new dbEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            rui.listHelper.add请选择(list, has请选择);
            list.Add(new SelectListItem { Text = "未处理", Value = "0" });
            list.Add(new SelectListItem { Text = "已通过", Value = "1" });
            list.Add(new SelectListItem { Text = "已拒绝", Value = "2" });
            return list;
        }

            public static string Create_ID(dbEntities dc)
            {
                string code = "NNA";
                string result = (from a in dc.NN_apply where a.applyID.StartsWith(code) select a.applyID).Max();
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


        /// <summary>
        /// 0 申请成为内容创作者 
        /// 1 变更新闻机构
        /// 2 注销自己的内容创作者身份
        /// 3 注销自己的账户
        /// </summary>
        /// <param name="keyFieldValues"></param>
        /// <param name="dc"></param>
        /// <returns>string</returns>
        public static string batchAgree(string keyFieldValues, dbEntities dc)//批量同意
        {
            efHelper ef = new efHelper(ref dc);
            List<string> KeyFieldList = rui.dbTools.getList(keyFieldValues);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            string sqlCheck = "select applyID, userID, Type, newsAgencyID, status  from NN_apply where applyID in @applyID";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { applyID = KeyFieldList });
            foreach (string applyID in KeyFieldList)
            {
                try
                {
                    var disposeTime = DateTime.Now;
                    string sql = "UPDATE NN_apply SET status = 1, disposeTime = @disposeTime WHERE applyID=@applyID "; //标记处理状态和处理时间 
                    DataRow[] rows = table.Select("applyID='" + applyID + "'");
                    rui.dbTools.checkRowFieldNotValue(rows, "status", "0", "已经被处理"); //在这里我新加了一个函数，如果不为0，则throw error
                    if (ef.Execute(sql, new {disposeTime, applyID }) == 0)
                    {
                        errorDic.Add(applyID, "发生错误，未成功运行");
                    }
                    string userID = dc.NN_apply.Where(a => a.applyID == applyID).SingleOrDefault().userID;
                    string type = rows.SingleOrDefault()["Type"].ToString();
                    if (type == "0" || type == "1") //说明是修改内容创作者的相关
                    {
                        string newsAgencyID = rows.SingleOrDefault().Field<string>("newsAgencyID");
                        string T_Sql = "UPDATE NN_user SET newsAgencyID = @newsAgencyID, type = 1 WHERE userID = @userID ";
                        
                        if (ef.Execute(T_Sql, new { newsAgencyID, userID }) == 0)
                        {
                            errorDic.Add(applyID, "发生错误，未成功运行");
                        }
                    }
                    else if(type == "2") //注销自己内容创作者的身份
                    {
                        string T_Sql = "UPDATE NN_user SET newsAgencyID = NULL, type = 0 WHERE userID = @userID ";
                        if (ef.Execute(T_Sql, new { userID }) == 0)
                        {
                            errorDic.Add(applyID, "发生错误，未成功运行");
                        }
                    }
                    else if(type == "3") //注销自己账户,并不真正删除，只是变为封禁状态
                    {
                        string T_Sql = "UPDATE NN_user SET newsAgencyID = null, type = 0， status = 1 WHERE userID = @userID ";
                        if (ef.Execute(T_Sql, new { userID }) == 0)
                        {
                            errorDic.Add(applyID, "发生错误，未成功运行");
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorDic.Add(applyID, "发生错误：" + rui.excptHelper.getExMsg(ex));
                }
            }
            dc.SaveChanges();
            return rui.dbTools.getBatchMsg("批量通过", KeyFieldList.Count, errorDic);
        }

        /// <summary>
        ///批量不同意 
        /// </summary>
        /// <param name="keyFieldValues"></param>
        /// <param name="dc"></param>
        /// <returns>string</returns>
        public static string batchDelete(string keyFieldValues, dbEntities dc)//批量不同意
        {
            efHelper ef = new efHelper(ref dc);
            List<string> KeyFieldList = rui.dbTools.getList(keyFieldValues);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            string sqlCheck = "select applyID, userID, Type, newsAgencyID, status  from NN_apply where applyID in @applyID";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { applyID = KeyFieldList });
            foreach (string applyID in KeyFieldList)
            {
                try
                {
                    var disposeTime = DateTime.Now;
                    string sql = "UPDATE NN_apply SET status = 2, disposeTime = @disposeTime WHERE applyID=@applyID "; //标记处理状态
                    DataRow[] rows = table.Select("applyID='" + applyID + "'");
                    rui.dbTools.checkRowFieldNotValue(rows, "status", "0", "已经被处理"); //在这里我新加了一个函数，如果不为0，则throw error
                    if (ef.Execute(sql, new {disposeTime, applyID }) == 0)
                    {
                        errorDic.Add(applyID, "发生错误，未成功运行");
                    }
                }
                catch(Exception ex)
                {
                    errorDic.Add(applyID, "发生错误：" + rui.excptHelper.getExMsg(ex));
                }
            }
            dc.SaveChanges();
            return rui.dbTools.getBatchMsg("批量拒绝", KeyFieldList.Count, errorDic);
        }

        public static db.NN_apply GetModelByuserIDAndType(string NNAID, dbEntities dc)
        {
            return dc.NN_apply.Where(a => a.applyID == NNAID).SingleOrDefault();
        }



        public static void insert_tobeofficial(string userID, string newsAgency, string content, dbEntities dc)
        {
            if(dc.NN_apply.Where(a => a.userID == userID && a.newsAgencyID == newsAgency && a.status == 0).Count() > 0)
            {
                rui.excptHelper.throwEx("已经申请过了");
            }
            db.NN_apply model = new db.NN_apply();
            efHelper ef = new efHelper(ref dc);
            model.rowID = ef.newGuid();
            model.userID = userID;
            model.applyID = Create_ID(dc);
            model.newsAgencyID = newsAgency;
            model.data = content;
            model.type = 1;
            model.status = 0;
            model.applicationDateTime = DateTime.Now;
            try
            {
                dc.NN_apply.Add(model);
                dc.SaveChanges();
            }
            catch(Exception ex)
            {
                rui.excptHelper.throwEx(ex.Message);
            }

        }

        public static void insert_tobeCreater(string userID, string newsAgency, string content, dbEntities dc)
        {
            if (dc.NN_apply.Where(a => a.userID == userID && a.newsAgencyID == newsAgency && a.status == 0).Count() > 0)
            {
                rui.excptHelper.throwEx("已经申请过了");
            }
            db.NN_apply model = new db.NN_apply();
            efHelper ef = new efHelper(ref dc);
            model.rowID = ef.newGuid();
            model.userID = userID;
            model.applyID = Create_ID(dc);
            model.newsAgencyID = newsAgency;
            model.data = content;
            model.type = 0;
            model.status = 0;
            model.applicationDateTime = DateTime.Now;
            try
            {
                dc.NN_apply.Add(model);
                dc.SaveChanges();
            }
            catch (Exception ex)
            {
                rui.excptHelper.throwEx(ex.Message);
            }

        }
    }
}
