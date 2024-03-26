using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.view
{
    public class NN_report_userly
    {
        public db.NN_newsItem news { get; set; }
        public db.NN_report report { get; set; }

        public NN_report_userly()
        {
        }

        public NN_report_userly(db.NN_newsItem news, db.NN_report report)
        {
            this.news = news;
            this.report = report;
        }

        ~NN_report_userly() { }

    }
}
