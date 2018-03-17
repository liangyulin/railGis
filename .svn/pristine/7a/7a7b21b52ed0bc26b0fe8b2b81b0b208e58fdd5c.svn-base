using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using Microsoft.Office.Interop.Excel;

namespace ModelInfo
{
    public class ExcelWrapper
    {
        public static System.Data.DataTable LoadDataTableFromExcel(string fileName, string tableName, string sql)
        {
            //OleDbConnection conn = null;
            //OleDbDataAdapter myCommand = null;
            //DataSet ds = new DataSet();

            string strConn;
            if (System.IO.Path.GetExtension(fileName) == ".xls")
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + fileName + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
            else
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + fileName + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";

            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
               // SqlCommand cmd = new SqlCommand(sql, conn);
                using (OleDbDataAdapter da = new OleDbDataAdapter(sql, conn))
                {
                    System.Data.DataTable dt = new System.Data.DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }

        }

        // 必须要求存在对应的Excel文件
        public static void SaveDataTableToExcel(DataSet ds, string tableName, string fileName, int sheetNum = 1)
        {

            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.ApplicationClass();
            try
            {
                app.Visible = false;
                //Workbook wBook = app.Workbooks.Add(true);
                Workbook wBook = app.Workbooks.Open(fileName);
                Worksheet wSheet = wBook.Worksheets[sheetNum] as Worksheet;
                if (ds != null)
                {
                    System.Data.DataTable excelTable = ds.Tables[tableName];
                    //System.Data.DataTable chainTable = ds.Tables["chain"];

                    int col = excelTable.Columns.Count;
                    for (int i = 0; i < col; i++)
                    {
                        wSheet.Cells[1, 1 + i] = excelTable.Columns[i].ColumnName;
                    }
                    int row = excelTable.Rows.Count;
                    for (int i = 1; i <= row; i++)
                    {
                        for (int j = 1; j <= col; j++)
                        {
                            wSheet.Cells[i + 1, j] = excelTable.Rows[i][j].ToString();
                        }
                    }

                    //设置禁止弹出保存和覆盖的询问提示框 
                    app.DisplayAlerts = false;
                    app.AlertBeforeOverwriting = false;
                    //保存工作簿 

                    //wBook.SaveAs(@"D:\result.xlsx");
                    wBook.Save();
                    //_Excel.Workbook book = app.Workbooks.Open(savePath, System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing);
                    //_Excel.Worksheet sheet = (_Excel.Worksheet)book.ActiveSheet;  
                    //保存excel文件 
                    //app.Save(@"D:\test1.xlsx");
                    //app.SaveWorkspace(@"D:\test1.xlsx");
                    app.Quit();
                }
                app = null;
                return;
            }
            catch (Exception err)
            {
                Console.WriteLine("导出Excel出错！错误原因：" + err.Message);
                return;
            }
            finally
            {
            }
        }

        // 存储内存自定义数据到Excel表中
        public static void SaveArrayListToExcel(DataSet ds, string tableName, string fileName, int sheetNum = 1)
        {

        }
    }
}
