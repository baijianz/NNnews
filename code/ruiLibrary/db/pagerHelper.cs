using System.Data;
using System.Globalization;

namespace rui
{
    /// <summary>
    /// ���ݷ�ҳ�ĸ�����
    /// </summary>
    public class pagerHelper
    {
        private string querySql = "";       //����Ĳ�ѯ���
        private string countSql = "";       //ͨ����ѯ��乹��
        private string orderSql = "";       //��������������
        private string pagerSql = "";       //��ŷ�ҳ��ȡ�����
        private pagerBase pagerPara;        //��ҳ����
        private DataTable pagerResult;      //��ҳ����

        /// <summary>
        /// �����ҳ��ѯ���
        /// </summary>
        public DataTable Result { get { return pagerResult; } set { pagerResult = value; } }

        /// <summary>
        /// Ĭ�ϲ��������Ĺ��캯��
        /// </summary>
        public pagerHelper() { }

        /// <summary>
        /// �������Ĺ��캯��
        /// </summary>
        /// <param name="querySql">��ѯ�ַ���</param>
        /// <param name="orderSql">��ѯ�������ַ���</param>
        /// <param name="pagerPara">��ҳ����</param>
        public pagerHelper(string querySql, string orderSql, pagerBase pagerPara)
        {
            this.querySql = querySql;
            this.orderSql = orderSql;
            this.pagerPara = pagerPara;
            if (this.pagerPara.PageSize == 0)
                this.pagerPara.PageSize = rui.configHelper.pageSize;
            this.processSql();
        }

        /// <summary>
        /// �Դ���Ĳ�ѯsql���д���
        /// �������֮���ȡ��getField��countSql���
        /// </summary>
        private void processSql()
        {
            CompareInfo compare = CultureInfo.InvariantCulture.CompareInfo;
            int selectEndPosition = compare.IndexOf(this.querySql, "select ", CompareOptions.IgnoreCase) + 6;
            int fromStartPosition = compare.IndexOf(this.querySql, " from ", CompareOptions.IgnoreCase);

            if (selectEndPosition < 0)
                rui.excptHelper.throwEx("select�ؼ��ֺ����Ҫ�пո�");
            if (fromStartPosition < 0)
                rui.excptHelper.throwEx("from�ؼ���ǰ����Ҫ�пո�");

            this.countSql = string.Format("select count(*) {0}", querySql.Substring(fromStartPosition));

            //�����ҳ���
            int offset = (this.pagerPara.PageIndex - 1) * this.pagerPara.PageSize;
            int next = this.pagerPara.PageSize;
            this.pagerSql = string.Format(" offset {0}  ROWS FETCH NEXT {1} ROWS only ", offset, next);

            //ƴ�����������
            this.querySql += " " + this.orderSql;
        }

        /// <summary>
        /// ���ݲ�ѯ����
        /// ��ѯ��ϣ�����pageCount,rowCount
        /// </summary>
        /// <param name="pageSize">ҳ���С</param>
        /// <param name="pageIndex">��ǰҳ</param>
        /// <param name="pager">��ҳ����</param>
        /// <param name="exceptDateFieldName">��Ҫ�ų�����</param>
        public void Execute(int pageSize, int pageIndex, rui.pagerBase pager, string exceptDateFieldName = "")
        {
            using (rui.dbHelper dbHelper = rui.dbHelper.createHelper())
            {
                pager.LongDateField = exceptDateFieldName;

                //�����������ߵ�����ҳʱ�����ر�ҳ������
                if (pager.ExportRange == null || pager.ExportRange == rui.dataRange.page.ToString())
                {
                    //��ȡ��ҳ����
                    this.pagerResult = dbHelper.ExecuteDataTable(this.querySql + this.pagerSql, pager.cmdPara);
                    rui.dbTools.insert���(this.pagerResult, pager.PageSize * (pager.PageIndex - 1));
                    this.Result = rui.dbTools.dateFormat(this.pagerResult, exceptDateFieldName);

                    //ִ�л������ʱ��ȡ��¼����
                    if (pager.ExeCountSql)
                    {
                        this.pagerPara.RowCount = rui.typeHelper.toInt(dbHelper.ExecuteScalar(this.countSql, pager.cmdPara));
                        this.pagerPara.PageCount = rui.typeHelper.toInt(pager.RowCount / pager.PageSize) + (pager.RowCount % pager.PageSize == 0 ? 0 : 1);
                    }
                }
                //������ѯ����������
                if (pager.ExportRange == rui.dataRange.all.ToString())
                {
                    //��ȡ��ѯ���������� - ����Excel���ݵ���
                    this.pagerResult = dbHelper.ExecuteDataTable(this.querySql, pager.cmdPara);
                    rui.dbTools.insert���(this.pagerResult, pager.PageSize * (pager.PageIndex - 1));
                    this.Result = rui.dbTools.dateFormat(this.pagerResult, exceptDateFieldName);
                }

                //�������в�ѯ���
                if (pager.sumRange == rui.dataRange.all.ToString() && rui.typeHelper.isNotNullOrEmpty(pager.sumSql))
                {
                    string sumSql = pager.sumSql + rui.dbTools.getWhereExpr(querySql);
                    DataRow sumRow = dbHelper.ExecuteDataRow(sumSql, pager.cmdPara);
                    foreach (DataColumn col in sumRow.Table.Columns)
                    {
                        pager.sumRow.Add(col.ColumnName, rui.typeHelper.toDecimal(sumRow[col.ColumnName]));
                    }
                }
                //���ܱ�ҳ������
                if (pager.sumRange == rui.dataRange.page.ToString() && rui.typeHelper.isNotNullOrEmpty(pager.sumSql))
                {
                    string sumSql = pager.sumSql + $" where {pager.keyField} in ({rui.dbTools.getInExpression(this.pagerResult, pager.keyField)}) ";
                    DataRow sumRow = dbHelper.ExecuteDataRow(sumSql);
                    foreach (DataColumn col in sumRow.Table.Columns)
                    {
                        pager.sumRow.Add(col.ColumnName, rui.typeHelper.toDecimal(sumRow[col.ColumnName]));
                    }
                }
            }
        }

        /// <summary>
        /// ���ݲ�ѯ����
        /// ��ѯ��ϣ�����pageCount,rowCount
        /// </summary>
        /// <param name="pageSize">ҳ���С</param>
        /// <param name="pageIndex">��ǰҳ</param>
        /// <param name="pager">��ҳ����</param>
        public void ExecuteI6(int pageSize, int pageIndex, rui.pagerBase pager)
        {
            using (rui.dbHelper dbHelperI6 = rui.dbHelper.createI6Helper())
            {
                //�����������ߵ�����ҳʱ�����ر�ҳ������
                if (pager.ExportRange == null || pager.ExportRange == dataRange.page.ToString())
                {
                    //��ȡ��ҳ����
                    this.pagerResult = dbHelperI6.ExecuteDataTable(this.querySql + this.pagerSql, pager.cmdPara);
                    rui.dbTools.insert���(this.pagerResult, pager.PageSize * (pager.PageIndex - 1));
                    this.Result = rui.dbTools.dateFormat(this.pagerResult);

                    //ִ�л������ʱ��ȡ��¼����
                    if (pager.ExeCountSql)
                    {
                        this.pagerPara.RowCount = rui.typeHelper.toInt(dbHelperI6.ExecuteScalar(this.countSql, pager.cmdPara));
                        this.pagerPara.PageCount = rui.typeHelper.toInt(pager.RowCount / pager.PageSize) + (pager.RowCount % pager.PageSize == 0 ? 0 : 1);
                    }
                }

                if (pager.ExportRange == dataRange.all.ToString())
                {
                    //��ȡ�������� - ����Excel���ݵ���
                    this.pagerResult = dbHelperI6.ExecuteDataTable(this.querySql, pager.cmdPara);
                    rui.dbTools.insert���(this.pagerResult, pager.PageSize * (pager.PageIndex - 1));
                    this.Result = rui.dbTools.dateFormat(this.pagerResult);
                }

                //�������в�ѯ���
                if (pager.sumRange == rui.dataRange.all.ToString() && rui.typeHelper.isNotNullOrEmpty(pager.sumSql))
                {
                    string sumSql = pager.sumSql + rui.dbTools.getWhereExpr(querySql);
                    DataRow sumRow = dbHelperI6.ExecuteDataRow(sumSql, pager.cmdPara);
                    foreach (DataColumn col in sumRow.Table.Columns)
                    {
                        pager.sumRow.Add(col.ColumnName, rui.typeHelper.toDecimal(sumRow[col.ColumnName]));
                    }
                }
                //���ܱ�ҳ������
                if (pager.sumRange == rui.dataRange.page.ToString() && rui.typeHelper.isNotNullOrEmpty(pager.sumSql))
                {
                    string sumSql = pager.sumSql + $" and {pager.keyField} in ({rui.dbTools.getInExpression(this.pagerResult, pager.keyField)}) ";
                    DataRow sumRow = dbHelperI6.ExecuteDataRow(sumSql);
                    foreach (DataColumn col in sumRow.Table.Columns)
                    {
                        pager.sumRow.Add(col.ColumnName, rui.typeHelper.toDecimal(sumRow[col.ColumnName]));
                    }
                }
            }
        }
    }

    /// <summary>
    /// �����ͻ������ݷ�Χ
    /// </summary>
    public enum dataRange
    {
        /// <summary>
        /// ���в�ѯ���
        /// </summary>
        all,
        /// <summary>
        /// ��ҳ������
        /// </summary>
        page
    }
}

