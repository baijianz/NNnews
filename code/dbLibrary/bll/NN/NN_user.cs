using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace db.bll
{
    public class NN_user
    {
        public static List<SelectListItem> bind自定义(string ans, bool has请选择 = false, string selectedValues = "")
        {
            return rui.listHelper.bindValues(ans, has请选择, selectedValues);
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

        public static List<SelectListItem> bind用户类型(bool has请选择 = false, string selectedValues = "")
        {
            dbEntities dc = new dbEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            rui.listHelper.add请选择(list, has请选择);
            list.Add(new SelectListItem { Text = "普通用户", Value = "0" });
            list.Add(new SelectListItem { Text = "内容创作者", Value = "1" });
            return list;
        }

        /// <summary>
        /// false 是满足要求
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool checkSameUserName(string userName) 
        {
            dbEntities dc = new dbEntities();
            var result = dc.NN_user.Count(a => a.userName == userName);
            return result != 0;  
        }
        public static List<SelectListItem> bind性别(bool has请选择 = false, string selectedValues = "")
        {
            dbEntities dc = new dbEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            rui.listHelper.add请选择(list, has请选择);
            list.Add(new SelectListItem { Text = "男", Value = "0" });
            list.Add(new SelectListItem { Text = "女", Value = "1" });
            return list;
        }
        public static List<SelectListItem> bind机构(bool has请选择 = false, string selectedValues = "")
        {
            dbEntities dc = new dbEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            rui.listHelper.add请选择(list, has请选择);
            foreach(var v in dc.NN_NewsAgency)
            {
                list.Add(new SelectListItem { Text = v.newsAgencyName, Value = v.newsAgencyID });
            }
            return list;
        }

        public static string getUserNameByUserID(string userID)
        {
            dbEntities dc = new dbEntities();
            return dc.NN_user.Where(a => a.userID == userID).SingleOrDefault().userName;
        }

        private static string _createID(dbEntities dc)
        {
            string code = "NNU";
            string result = (from a in dc.NN_user where a.userID.StartsWith(code) select a.userID).Max();
            if(result != null)
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
        private static void _checkINput(db.NN_user model)
        {
            efHelper.checkNotNull(model.userID, "用户ID");
        }


        public static string insert(db.NN_user model, dbEntities dc, HttpPostedFileBase file)
        {
            efHelper ef = new efHelper(ref dc);
            if(rui.typeHelper.isNullOrEmpty(model.userID))
            {
                model.userID = _createID(dc);
            }
            else if(dc.NN_user.Count(a => a.userID == model.userID) > 0)
            {
                rui.excptHelper.throwEx("编号已存在");
            }

            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    byte[] byteArray;
                    using (var binaryReader = new BinaryReader(file.InputStream))  //将文件转为BYTE流
                    {
                        byteArray = binaryReader.ReadBytes(file.ContentLength);
                        model.profile =  byteArray;
                    }
                }
                catch
                {
                    rui.excptHelper.throwEx("头像文件出现错误");
                }
            }
            else
            {
                byte[] imageByte;
                if (model.gender == false) //男 在没有选择头像的情况
                {
                    imageByte =  File.ReadAllBytes(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "..\\dbLibrary\\pic\\man.jpg");
                }
                else
                {
                    imageByte = File.ReadAllBytes(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "..\\dbLibrary\\pic\\woman.jpg");
                }
                model.profile = imageByte;
            }
            _checkINput(model);
            model.password = rui.encryptHelper.toMD5(model.password);
            model.rowID = ef.newGuid();
            model.registerDateTime = DateTime.Now;
            model.status = false;   //默认状态为正常
            if (model.type == 0) model.newsAgencyID = null;
            try
            {
                dc.NN_user.Add(model);
                dc.SaveChanges();
            }
            catch (DbEntityValidationException ex) //验证模型
            {
                rui.excptHelper.throwEx(ex.Message);
            }
            return model.rowID;

        }


        public static int getAge(DateTime date)
        {
            DateTime nowDate = DateTime.Now;
            TimeSpan differ = nowDate - date;
            return (differ.Days + 364) / 365;
        }

        public static string getNewsAgencyByAgencyID(string newsAgencyID)
        {
            dbEntities dc = new dbEntities();
            return (dc.NN_NewsAgency.Where(a => a.newsAgencyID == newsAgencyID).SingleOrDefault()).newsAgencyName.ToString();
        }

        public static db.NN_user getModelByRowID(string rowID, dbEntities dc)
        {
            return dc.NN_user.Where(a => a.rowID == rowID).FirstOrDefault();
        }

        public static db.NN_user getModelByuserID_NonDc(string userID)
        {
            dbEntities dc = new dbEntities();
            return dc.NN_user.Where(a => a.userID == userID).FirstOrDefault();
        }
        public static string update(db.NN_user model, HttpPostedFileBase newProfile, dbEntities dc)
        {
            efHelper.entryUpdate(model, dc);
            //start 在这里使用连线查询，处理部分特殊数据
            string connectionString = "data source=DUNE;initial catalog=delicacyDataBase;persist security info=True;user id=sa;password=123456;MultipleActiveResultSets=True;App=EntityFramework&quot;" ;
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open(); //打开数据链接
            string sql = @"select password from NN_user where rowID = @rowID";
            SqlCommand commd = new SqlCommand(sql , conn);
            SqlParameter para = new SqlParameter("@rowID", model.rowID.ToString());
            para.Direction = System.Data.ParameterDirection.Input;
            commd.Parameters.Add(para);
            string passWord = commd.ExecuteScalar().ToString();
            model.password = passWord;
            conn.Close();

            string connectionStringProfile = "data source=DUNE;initial catalog=delicacyDataBase;persist security info=True;user id=sa;password=123456;MultipleActiveResultSets=True;App=EntityFramework&quot;";
            SqlConnection connProfile = new SqlConnection(connectionStringProfile);
            connProfile.Open();
            string sqlProfile = @"select profile from NN_user where rowID = @rowID";
            SqlCommand commdProfile = new SqlCommand(sqlProfile, connProfile);
            SqlParameter paraProFile = new SqlParameter("@rowID", model.rowID.ToString());
            paraProFile.Direction = System.Data.ParameterDirection.Input;
            commdProfile.Parameters.Add(paraProFile);                        //添加参数
            byte[] profile = (byte[])(commdProfile.ExecuteScalar());  //返回第一条结果的第一项
            model.profile = profile;
            connProfile.Close();
            //end
            if (newProfile != null)
                model.profile = new BinaryReader(newProfile.InputStream).ReadBytes(newProfile.ContentLength);
            if(model.type == 0)
            {
                model.newsAgencyID = null;
            }
            dc.SaveChanges();

            
            return model.rowID;
        }


        /// <summary>
        /// 实现下拉框的联动
        /// </summary>
        /// <param name="type"></param>
        /// <returns>返回一个selectitem列表</returns>
        public static string getJsonAgencyDdl(string type, string rowID, dbEntities dc)
        {
            
            if(type == "0") //表示不需要
            {
                return rui.jsonResult.SelectListToJsonStr(bindDdl(false, "", type));
            }
            else
            {
                string v;
                try
                {
                    v = dc.NN_user.Where(a => a.rowID == rowID).SingleOrDefault().newsAgencyID.ToString();
                }
                catch
                {
                    v = dc.NN_NewsAgency.First().newsAgencyID.ToString();
                }
                
                return rui.jsonResult.SelectListToJsonStr(bindDdl(false, v, type));
            }
        }

        public static List<System.Web.Mvc.SelectListItem> bindDdl(bool has请选择 = false, string selectedValue = "", string type = "")
        {
            if(type == "0")
            {
                List<SelectListItem> list1 = new List<SelectListItem>();
                rui.listHelper.add请选择(list1, has请选择);
                list1.Add(new SelectListItem() { Text = "无", Value = "-1" });
                return list1;
            }
            efHelper ef = new efHelper();
            string sql = " SELECT newsAgencyName, newsAgencyID FROM NN_NewsAgency where 1=1 ";
            sql += " ORDER BY newsAgencyID ASC ";
            DataTable table = ef.ExecuteDataTable(sql);
            List<System.Web.Mvc.SelectListItem> list = dataTableToDdlList_newAgencys(table, has请选择, selectedValue);
            return list;
        }

        public static List<SelectListItem> dataTableToDdlList_newAgencys(DataTable table, bool has请选择, string selectedValue)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            rui.listHelper.add请选择(list, has请选择);
            List<string> selectedList = rui.dbTools.getList(selectedValue);
            foreach (DataRow row in table.Rows)
            {
                if (selectedList.Contains(row["newsAgencyID"].ToString()))
                    list.Add(new SelectListItem() { Text = row["newsAgencyName"].ToString(), Value = row["newsAgencyID"].ToString(), Selected = true });
                else
                    list.Add(new SelectListItem() { Text = row["newsAgencyName"].ToString(), Value = row["newsAgencyID"].ToString() });
            }
            return list;
        }


        public static void deleteNN_userEntry(string rowID, dbEntities dc)
        {
            try
            {
                db.NN_user model = dc.NN_user.Where(a => a.rowID == rowID).SingleOrDefault();
                if(model.userID == "NNU000000")
                {
                    rui.excptHelper.throwEx("官方账户不允许删除");
                }
                dc.NN_user.Remove(model);
                dc.SaveChanges();
            }
            catch (Exception ex)
            {
                rui.excptHelper.throwEx(ex.Message);
            }
        }

        public static string ChangeState(string rowID, dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            try
            {
                string sql = string.Format("select userID as code, status from NN_user where rowID = '{0}'", rowID);
                DataRow item = ef.ExecuteDataRow(sql);
                if(item["status"].ToString() == "False") //正常
                {
                    string updateSql = string.Format("update NN_user set status=1 where rowID = '{0}'", rowID);
                    if(ef.Execute(updateSql, new { rowID}) == 0)
                    {
                        rui.excptHelper.throwEx("数据未变更");
                    }
                }
                else
                {
                    string updateSql = string.Format("update NN_user set status=0 where rowID = '{0}'", rowID);
                    if (ef.Execute(updateSql, new { rowID }) == 0)
                    {
                        rui.excptHelper.throwEx("数据未变更");
                    }
                }
                ef.commit();
            }
            catch(Exception ex)
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
            string sqlCheck = "select userID  from NN_user where userID in @userID";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { userID = KeyFieldList });
            foreach (string userID in KeyFieldList)
            {
                try
                {
                    var model = dc.NN_user.Where(a => a.userID == userID).SingleOrDefault();
                    if(model.userID == "NNU000000")
                    {
                        errorDic.Add(userID, "官方账户不允许删除");
                        continue;
                    }
                    dc.NN_user.Remove(model);
                }
                catch
                {
                    errorDic.Add(userID, "该条目有关联项目，不允许删除");
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
            string sqlCheck = "select userID, status  from NN_user where userID in @userID";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { userID = KeyFieldList });
            foreach (string userID in KeyFieldList)
            {
                try
                {
                    string sql = " UPDATE NN_user SET status = 1 WHERE userID=@userID ";
                    DataRow[] rows = table.Select("userID='" + userID + "'");
                    rui.dbTools.checkRowFieldValue(rows, "status", "1", "已处于封禁状态");
                    if (ef.Execute(sql, new { userID }) == 0)
                    {
                        errorDic.Add(userID, "发生错误，未成功设置");
                    }
                }
                catch (Exception ex)
                {
                    errorDic.Add(userID, "发生错误:" + rui.excptHelper.getExMsg(ex));
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
            string sqlCheck = "select userID, status from NN_user where userID in @userID";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { userID = KeyFieldList });
            foreach (string userID in KeyFieldList)
            {
                try
                {
                    string sql = " UPDATE NN_user SET status = 0 WHERE userID=@userID ";
                    DataRow[] rows = table.Select("userID='" + userID + "'");
                    rui.dbTools.checkRowFieldValue(rows, "status", "0", "状态正常，不需要解封");
                    if (ef.Execute(sql, new { userID }) == 0)
                    {
                        errorDic.Add(userID, "发生错误，未成功设置");
                    }
                }
                catch (Exception ex)
                {
                    errorDic.Add(userID, "发生错误:" + rui.excptHelper.getExMsg(ex));
                }
            }
            dc.SaveChanges();
            return rui.dbTools.getBatchMsg("批量解封", KeyFieldList.Count, errorDic);
        }

        public static db.NN_user getModelByUserID_NonDc(string userID)
        {
            dbEntities dc = new dbEntities();
            return dc.NN_user.Where(a => a.userID == userID).SingleOrDefault();
        }


        public static void PermanentCancellation(string userID, dbEntities dc)        
        {
            var model = dc.NN_user.Where(a => a.userID == userID).SingleOrDefault();
            model.status = false;
            dc.SaveChanges();

        }

        public static void amendPassword(string origin, string newPass, string userID, dbEntities dc)
        {
            var model = dc.NN_user.Where(a => a.userID == userID).SingleOrDefault();
            if (rui.encryptHelper.toMD5(origin) != model.password)
            {
                rui.excptHelper.throwEx("原密码错误");
            }
            else
            {
                model.password = rui.encryptHelper.toMD5(newPass);
            }
            try
            {
                dc.SaveChanges();
            }
            catch(Exception ex)
            {
                rui.excptHelper.throwEx("error: " + ex.Message);
            }

        }

        public static void logoutCreater(string userID, dbEntities dc)
        {
            if(dc.NN_apply.Where(a => a.userID == userID && a.type == 2 && a.status == 0).Count() > 0)
            {
                rui.excptHelper.throwEx("已经申请过了");
            }

            efHelper ef = new efHelper(ref dc);
            db.NN_apply model = new db.NN_apply();
            model.rowID = ef.newGuid();
            model.userID = userID;
            model.applyID = db.bll.NN_apply.Create_ID(dc);
            model.type = 2;
            model.status = 0;
            model.applicationDateTime = DateTime.Now;
            dc.NN_apply.Add(model);
            dc.SaveChanges();
        }


    }
}
