using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.view
{
    public class NN_user : rui.pagerBase
    {
        public string userID { get; set; }  //用户ID

        public string userName { get; set; }//用户昵称
        public string gender { get; set; } //性别

        public string typeID { get; set; } //账户类型

        public string newsAgencyID { get; set; } //机构ID，当账户类型为创作者时有效

        public string status { get; set; } //用户状态

        public override void Search()
        {
            this.ResourceCode = "NN_user";
            this.keyField = "userID";
            
            this.getPageConfig();


            string querySql = @"SELECT * FROM NN_user WHERE  1=1";
            querySql += rui.dbTools.searchTbx("userID", this.userID, this.cmdPara);
            querySql += rui.dbTools.searchTbx("userName", this.userName, this.cmdPara);
            querySql += rui.dbTools.searchDdl("gender", this.gender, this.cmdPara);
            querySql += rui.dbTools.searchDdl("type", this.typeID, this.cmdPara);
            querySql += rui.dbTools.searchDdl("newsAgencyID", this.newsAgencyID, this.cmdPara);
            querySql += rui.dbTools.searchDdl("status", this.status, this.cmdPara);

            //利用搜索语句获取数据
            this.getPageConfig();
            rui.pagerHelper ph = new rui.pagerHelper(querySql, this.getOrderSql("userID", "asc"), this);
            ph.Execute(this.PageSize, this.PageIndex, this);
            this.dataTable = ph.Result;
            //获取要展示的列配置
            this.showColumn = this.getShowColumn();

        }
    }
}
