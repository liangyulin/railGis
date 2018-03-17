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
    public class DatabaseWrapper
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

        public static void SavePierToDB(string fileName, List<CRailwayProject> bridgeList)
        {

            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.ApplicationClass();
            try
            {
                app.Visible = false;
                //Workbook wBook = app.Workbooks.Add(true);
                Workbook wBook = app.Workbooks.Open(fileName);
                Worksheet wSheet = wBook.Worksheets["Pier"] as Worksheet;

                wSheet.Cells[1, 1] = "BridgeName";
                wSheet.Cells[1, 2] = "PierName";
                wSheet.Cells[1, 3] = "Longitude";
                wSheet.Cells[1, 4] = "Latitude";
                wSheet.Cells[1, 5] = "Altitude";
                wSheet.Cells[1, 6] = "YawOffset";
                wSheet.Cells[1, 7] = "DKCode";
                wSheet.Cells[1, 8] = "Meter";
                int count = 1;

                foreach (CRailwayProject p in bridgeList)
                {
                    foreach (CRailwayDWProj dwp in p.mDWProjList)
                    {
                        wSheet.Cells[1 + count, 1] = p.ProjectName;
                        wSheet.Cells[1 + count, 2] = dwp.DWName;
                        wSheet.Cells[1 + count, 3] = dwp.mLongitude_Mid;
                        wSheet.Cells[1 + count, 4] = dwp.mLatitude_Mid;
                        wSheet.Cells[1 + count, 5] = dwp.mAltitude_Mid;
                        wSheet.Cells[1 + count, 6] = dwp.mHeading_Mid;
                        wSheet.Cells[1 + count, 7] = dwp.DKCode_Start;
                        wSheet.Cells[1 + count, 8] = dwp.Mileage_Start;
                        count++;
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

                app = null;
                return;
            }
            catch (Exception err)
            {
                Console.WriteLine("导出Excel出错！错误原因：" + err.Message);
                return;
            }

        }

        /// <summary>
        /// 三维中线数据写入Excel表，目前仅测试用，写入时间较长
        /// </summary>
        /// <param name="fileName"></param>
        public static void SaveLineListToDB(List<CRailwayLine> mLineList, string fileName)
        {

            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.ApplicationClass();
            try
            {
                app.Visible = false;
                //Workbook wBook = app.Workbooks.Add(true);
                Workbook wBook = app.Workbooks.Open(fileName);
                Worksheet wSheet = wBook.Worksheets["Chain"] as Worksheet;
                int countID = 1;

                wSheet.Cells[1, 1] = "fromMeter";
                wSheet.Cells[1, 2] = "toMeter";
                wSheet.Cells[1, 3] = "fromID";
                wSheet.Cells[1, 4] = "toID";
                wSheet.Cells[1, 5] = "DKCode";

                for (int i = 0; i < mLineList.Count; i++)
                {
                    wSheet.Cells[2 + i, 1] = mLineList[i].mStart;
                    wSheet.Cells[2 + i, 2] = mLineList[i].mEnd;
                    wSheet.Cells[2 + i, 3] = countID;
                    countID += mLineList[i].mPointNum;
                    wSheet.Cells[2 + i, 4] = countID - 1;
                    wSheet.Cells[2 + i, 5] = mLineList[i].mDKCode;
                }

                Worksheet ws = wBook.Worksheets["MiddleLine"] as Worksheet;

                ws.Cells[1, 1] = "Mileage";
                ws.Cells[1, 2] = "Longitude";
                ws.Cells[1, 3] = "Latitude";
                ws.Cells[1, 4] = "Altitude";
                ws.Cells[1, 5] = "ID";
                ws.Cells[1, 6] = "DKCode";

                countID = 1;
                bool isR;
                for (int i = 0; i < mLineList.Count; i++)
                {
                    isR = mLineList[i].mIsReverse;
                    for (int j = 0; j < mLineList[i].mPointNum; j++)
                    {
                        if (isR)
                            ws.Cells[1 + countID, 1] = mLineList[i].mStart - mLineList[i].meter[j];
                        else
                            ws.Cells[1 + countID, 1] = mLineList[i].mStart + mLineList[i].meter[j];
                        ws.Cells[1 + countID, 2] = mLineList[i].longitude[j];
                        ws.Cells[1 + countID, 3] = mLineList[i].latitude[j];
                        ws.Cells[1 + countID, 4] = mLineList[i].altitude[j];
                        ws.Cells[1 + countID, 5] = countID;
                        ws.Cells[1 + countID, 6] = mLineList[i].mDKCode;
                        countID++;

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

        public static void PrintDataTable(System.Data.DataTable dt)
        {

            List<string> ls;
            ls = GetDataTableColumnName(dt);
            foreach (string str in ls)
            {
                Console.Write(str + "\t");
            }
            Console.WriteLine();
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < ls.Count; i++)
                {
                    Console.Write(dr[i] + "\t");
                }
                Console.WriteLine();

            }
        }

        private static List<string> GetDataTableColumnName(System.Data.DataTable dt)
        {
            List<string> al = new List<string>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                al.Add(dt.Columns[i].ColumnName);
            }
            return al;
        }
    }
}
