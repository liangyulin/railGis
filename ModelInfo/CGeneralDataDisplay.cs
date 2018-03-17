using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;

namespace ModelInfo
{
    public class CGeneralDataDisplay
    {
        public static DataTable findGeneralInfo(string sql)
        {
            int num = 0;


            //DataTable dt = CServerWrapper.findConsInfo(DateTime.Now.AddDays(-30).Date.ToString("u"));
            DataTable dt = CServerWrapper.execSqlQuery(sql);
            num = dt.Rows.Count;

            if (num == 0) return null;

            return dt;
        }
    }
}
