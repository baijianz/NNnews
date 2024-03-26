using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.bll
{
    public class NN_newsCategroryItem
    {
        public static string getTagListByNewsID(string newsID)
        {
            dbEntities dc = new dbEntities();
            var data = dc.NN_newsCategroryItem.Where(a => a.newsID == newsID).ToList();
            List<string> list = new List<string>();
            string ans = "";
            foreach(var v in data)
            {
                string item = dc.NN_newsCategory.Where(a => a.newsCategoryID == v.newsCateGoryID).Single().newsCategoryName;
                ans += (item + ",");
            }
            if(ans.Length > 0)
            return ans.Remove(ans.Length - 1);
            return ans;
        }
    }
}
