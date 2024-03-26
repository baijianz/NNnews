using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.bll
{
    public class NN_sqlHelper
    {
        /// <summary>
        /// 获取前10个最受欢迎的类别
        /// </summary>
        /// <returns></returns>
        public static DataTable getMostPopularCategory()
        {
            string sql = "select TOP 10 NN_newsCategory.newsCateGoryID as ID, NN_newsCategory.newsCategoryName as Name, COUNT(*) as count from NN_newsCategory join NN_newsCategroryItem on NN_newsCategroryItem.newsCateGoryID = NN_newsCategory.newsCategoryID group by NN_newsCategory.newsCategoryID,NN_newsCategory.newsCategoryName order by count(newsID) desc ";
            dbEntities dc = new dbEntities();
            efHelper ef = new efHelper(ref dc);
            return ef.ExecuteDataTable(sql);
        }


    }
}
