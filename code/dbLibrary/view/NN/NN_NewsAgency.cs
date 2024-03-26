using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.view
{
    public class NN_NewsAgency : rui.pagerBase
    {
        public string newsAgencyID { get; set; } //账号ID

        public string newsAgencyName { get; set; }//账号名称

        public string status { get; set; }//账号状态


        public override void Search()
        {
            this.ResourceCode = "NN_NewsAgency";
            this.keyField = "newsAgencyID";
            this.getPageConfig();
            string querySql = @"SELECT * FROM NN_NewsAgency WHERE  1=1";
            querySql += rui.dbTools.searchTbx("newsAgencyID", this.newsAgencyID, this.cmdPara);
            querySql += rui.dbTools.searchTbx("newsAgencyName", this.newsAgencyName, this.cmdPara);
            querySql += rui.dbTools.searchDdl("status", this.status, this.cmdPara);
            //利用搜索语句获取数据
            this.getPageConfig();
            rui.pagerHelper ph = new rui.pagerHelper(querySql, this.getOrderSql("newsAgencyID", "asc"), this);
            ph.Execute(this.PageSize, this.PageIndex, this);
            this.dataTable = ph.Result;
            //获取要展示的列配置
            this.showColumn = this.getShowColumn();
        }
    }
}
