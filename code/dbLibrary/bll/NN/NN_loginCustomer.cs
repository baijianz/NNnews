using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.bll
{
    public class NN_loginCustomer
    {
        public static void login(string userID, string password, dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            string sql = "select * from NN_user where userID = @userID and password = @password";
            DataRow row = ef.ExecuteDataRow(sql, new { userID, password });
            if(row == null)
            {
                rui.excptHelper.throwEx("账户或者密码输入错误");
            }
            else
            {
                //保存可能需要的信息
                rui.sessionHelper.saveValue<string>("userName", row.Field<string>("userName").ToString());
                rui.sessionHelper.saveValue<string>("userID", row.Field<string>("userID").ToString());
                rui.sessionHelper.saveValue<string>("type", row.Field<int>("type").ToString());
                rui.sessionHelper.saveValue<byte[]>("profile", row.Field<byte[]>("profile"));
                if (db.bll.NN_user.getModelByRowID(row["rowID"].ToString(), dc).newsAgencyID != null)
                {
                    rui.sessionHelper.saveValue<string>("newsAgencyID", row.Field<string>("newsAgencyID").ToString());
                }
            }
        }
    }
}
