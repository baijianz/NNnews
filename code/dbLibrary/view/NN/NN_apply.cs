using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.view
{
    public class NN_apply : rui.pagerBase
    {
        
        public string userID { get; set; }  //用户ID

        public string newsAgencyID { get; set; } //想要加入的机构ID，如果有的话

        public string Type { get; set; } //申请的表单类型

        public string status { get; set; } //处理的状态

        public string applyDateTimeStart { get; set; } //提出申请的时间
        public string applyDateTimeEND { get; set; }


        public override void Search()
        {
            this.ResourceCode = "NN_apply";
            this.keyField = "applyID";
            this.getPageConfig();

            string querySql = @"SELECT * FROM NN_apply WHERE  1=1";
            querySql += rui.dbTools.searchDdl("userID", this.userID, cmdPara);                   //用户ID搜索
            querySql += rui.dbTools.searchDdl("newsAgencyID", this.newsAgencyID, cmdPara);       //新闻机构ID搜索
            querySql += rui.dbTools.searchDdl("Type", this.Type, cmdPara);            //申请类型搜索
            querySql += rui.dbTools.searchDdl("status", this.status, cmdPara);        //状态搜索
            querySql += rui.dbTools.searchDate("applicationDateTime", applyDateTimeStart, applyDateTimeEND, cmdPara);

            this.getPageConfig();
            rui.pagerHelper ph = new rui.pagerHelper(querySql, this.getOrderSql("rowID", "asc"), this);
            ph.Execute(this.PageSize, this.PageIndex, this);
            this.dataTable = ph.Result;
            //获取要展示的列配置
            this.showColumn = this.getShowColumn();

        }
    }
}
