using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace db.bll.NN
{
    public class onLineQuery
    {
        public static string exScaleQuery(string sql) //有一项返回值
        {
            string connectionString = "data source=DUNE;initial catalog=delicacyDataBase;persist security info=True;user id=sa;password=123456;MultipleActiveResultSets=True;App=EntityFramework&quot;";
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open(); //打开数据链接
            SqlCommand commd = new SqlCommand(sql, conn);
            string result = commd.ExecuteScalar().ToString();
            conn.Close();
            return result;
        }
    }
}
