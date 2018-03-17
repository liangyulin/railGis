using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using GISUtilities.ModelInformation;
using CCWin;

namespace GISUtilities
{

    public partial class Form1 : CCSkinMain
    {
        DataSet ds = new DataSet();
        List<ChainNode> chainList = new List<ChainNode>();
        
        public Form1()
        {
            InitializeComponent();
        }

        private void LoadDataTableFromExcel(string fileName,string tableName)
        {
            OleDbConnection conn = null;
            OleDbDataAdapter myCommand = null;
            
            string strConn;
            if (System.IO.Path.GetExtension(fileName) == ".xls")
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + fileName + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
            else
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + fileName + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
            try
            {
                conn = new OleDbConnection(strConn);
                conn.Open();
                myCommand = new OleDbDataAdapter(@"select * from [" +tableName + "$]; ", conn);//order by Mileage asc
                myCommand.Fill(ds, tableName);

            }
            finally
            {
                conn.Close();
            }
        }

        private void LoadChainList(string fileName )
        {
            if (chainList.Count > 0) return;
            LoadDataTableFromExcel(fileName, "chain");
            foreach (DataRow dr in ds.Tables["chain"].Rows)
            {
                ChainNode ch = new ChainNode();
                ch.fromMeter = (double)dr["fromMeter"];
                ch.toMeter = (double)dr["toMeter"];
                ch.dkCode = (string)dr["DKCode"];
                ch.fromID = (int)((double)dr["fromID"] + 0.1);
                ch.toID = (int)((double)dr["toID"] + 0.1); ;
                chainList.Add(ch);
            }
        }
        private void LoadAndSaveMileList(string fileName)
        {
            LoadDataTableFromExcel(fileName, "mile");

            Microsoft.Office.Interop.Excel.Application app =
    new Microsoft.Office.Interop.Excel.ApplicationClass();
            try
            {
                app.Visible = false;
                //Workbook wBook = app.Workbooks.Add(true);
                Workbook wBook = app.Workbooks.Open(@"D:\gisData\xlsx\MiddleLine.xlsx");
                Worksheet wSheet = wBook.Worksheets[1] as Worksheet;
                if (ds != null)
                {
                    System.Data.DataTable excelTable = ds.Tables["mile"];
                    //System.Data.DataTable chainTable = ds.Tables["chain"];
                    int chainCount = 1;
                    int rowCount=0;
                    double offset = 0;
                    if (excelTable.Rows.Count > 0)
                    {
                        int row = 0;
                        row = excelTable.Rows.Count;
                        int col = excelTable.Columns.Count;
                        for (int i = 0; i < row; i++)
                        {
                            int id = int.Parse( excelTable.Rows[i][4].ToString());
                            if (chainCount < chainList.Count && id == chainList[chainCount].fromID)
                            {
                                offset += chainList[chainCount - 1].toMeter - chainList[chainCount].fromMeter;
                                Console.WriteLine(chainCount);
                                chainCount++;
                                
                                continue;
                                //currentMeter = currentMeter + 
                            }
                            double meter = double.Parse(excelTable.Rows[i][0].ToString()) + offset;
                            wSheet.Cells[rowCount + 2, 1] = meter.ToString();
                            wSheet.Cells[rowCount + 2, 2] = excelTable.Rows[i][1].ToString();
                            wSheet.Cells[rowCount + 2, 3] = excelTable.Rows[i][2].ToString();
                            wSheet.Cells[rowCount + 2, 4] = excelTable.Rows[i][3].ToString();
                            wSheet.Cells[rowCount + 2, 5] = (rowCount + 1).ToString(); ;
                            rowCount++;
                            //for (int j = 0; j < col; j++)
                            //{
                                
                            //    string str = excelTable.Rows[i][j].ToString();
                            //    wSheet.Cells[i + 2, j + 1] = str;
                            //}
                        }
                    }

                    //int size = excelTable.Columns.Count;
                    //for (int i = 0; i < size; i++)
                    //{
                    //    wSheet.Cells[1, 1 + i] = excelTable.Columns[i].ColumnName;
                    //}
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
                return ;
            }
            catch (Exception err)
            {
                MessageBox.Show("导出Excel出错！错误原因：" + err.Message, "提示信息",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ;
            }
            finally
            {
            }

        }

        private string KMConvert(string s)
        {
            double num = 0;
            if (s.StartsWith("DMK"))
            {
                s = s.Substring(3);
                string[] ss = s.Split(new char[1] { '+' });
                num = Double.Parse(ss[0].Trim()) * 1000 + Double.Parse(ss[1].Trim());
                //double offset = 0;
                //for (int i = 1; i < chainList.Count; i++)
                //{
                //    offset += chainList[i - 1].toMeter - chainList[i].fromMeter;
                //    if (chainList[i].dkCode.Equals("DMK") && num >= chainList[i].fromMeter && num <= chainList[i].toMeter)
                //    {
                //        num += offset;
                //        break;
                //    }

                //}
                
            }
            else if (s.StartsWith("DIK"))
            {
                s = s.Substring(3);
                string[] ss = s.Split(new char[1] { '+' });
                num = Double.Parse(ss[0].Trim()) * 1000 + Double.Parse(ss[1].Trim()) + chainList[0].toMeter - chainList[1].fromMeter;
                //double offset = 0;
                //for (int i = 1; i < chainList.Count; i++)
                //{
                //    offset += chainList[i - 1].toMeter - chainList[i].fromMeter;
                //    if (chainList[i].dkCode.Equals("DIK") && num >= chainList[i].fromMeter && num <= chainList[i].toMeter)
                //    {
                //        num += offset;
                //        break;
                //    }

                //}
                
            }
            else if (s.StartsWith("DK") || s.StartsWith("CK"))
            {
                s = s.Substring(2);
                string[] ss = s.Split(new char[1] { '+' });
                num = Double.Parse(ss[0].Trim()) * 1000 + Double.Parse(ss[1].Trim());
                double offset = 0;
                for (int i = 1; i<chainList.Count; i++)
                {
                    offset += chainList[i - 1].toMeter - chainList[i].fromMeter;
                    if (chainList[i].dkCode.Equals("DK") && num >= chainList[i].fromMeter && num <= chainList[i].toMeter){
                        num += offset;
                        break;
                    } 
                }
                
            }
            else
            {
                num = 0;
            }

            return num + "";
        }

        private void ProcessMidLine(string tableName)
        {
            foreach (DataRow dr in ds.Tables[tableName].Rows)
            {
                string dks = (string)dr["Mileage_Start_Des"];
                dr["Mileage_Start"] = KMConvert(dks);
                dks = (string)dr["Mileage_End_Des"];
                dr["Mileage_End"] = KMConvert(dks);
            }
        }

        public bool SaveDataTableToExcel( string tableName)
        {
            Microsoft.Office.Interop.Excel.Application app =
                new Microsoft.Office.Interop.Excel.ApplicationClass();
            try
            {
                app.Visible = false;
                Workbook wBook = app.Workbooks.Open(@"D:\GISData\xlsx\ProjectInfo.xlsx");
                Worksheet wSheet = wBook.Worksheets[1] as Worksheet;
                if (ds != null)
                {
                    System.Data.DataTable excelTable = ds.Tables[tableName];
                    if (excelTable.Rows.Count > 0)
                    {
                        int row = 0;
                        row = excelTable.Rows.Count;
                        int col = excelTable.Columns.Count;
                        for (int i = 0; i < row; i++)
                        {
                            for (int j = 0; j < col; j++)
                            {
                                string str = excelTable.Rows[i][j].ToString();
                                wSheet.Cells[i + 2, j + 1] = str;
                            }
                        }
                    }

                    int size = excelTable.Columns.Count;
                    for (int i = 0; i < size; i++)
                    {
                        wSheet.Cells[1, 1 + i] = excelTable.Columns[i].ColumnName;
                    }
                    //设置禁止弹出保存和覆盖的询问提示框 
                    app.DisplayAlerts = false;
                    app.AlertBeforeOverwriting = false;
                    //保存工作簿 
                    wBook.Save();
                    //保存excel文件 
                    //app.Save(filePath);
                    //app.SaveWorkspace(filePath);
                    app.Quit();
                }
                app = null;
                return true;
            }
            catch (Exception err)
            {
                MessageBox.Show("导出Excel出错！错误原因：" + err.Message, "提示信息",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            finally
            {
            }
        }


        private void SaveTEProList()
        {
            //LoadDataTableFromExcel(fileName, "mile");

            Microsoft.Office.Interop.Excel.Application app =
    new Microsoft.Office.Interop.Excel.ApplicationClass();
            try
            {
                app.Visible = false;
                //Workbook wBook = app.Workbooks.Add(true);
                Workbook wBook = app.Workbooks.Open(@"D:\gisData\xlsx\TEProj.xlsx");
                Worksheet wSheet = wBook.Worksheets[1] as Worksheet;
                //if (ds != null)
                //{
                    //System.Data.DataTable excelTable = ds.Tables["mile"];
                    //System.Data.DataTable chainTable = ds.Tables["chain"];
                int rowCount = 0;
                    foreach (CTEProject p in CTEProject.mTEProjectList)
                    {
                        wSheet.Cells[rowCount + 2, 1] = p.mProjectType;
                        wSheet.Cells[rowCount + 2, 2] = p.mProjectName;
                        wSheet.Cells[rowCount + 2, 3] = p.mModelName;
                        wSheet.Cells[rowCount + 2, 4] = p.sX.ToString();
                        wSheet.Cells[rowCount + 2, 5] = p.sY.ToString();
                        wSheet.Cells[rowCount + 2, 6] = p.sZ.ToString();
                        wSheet.Cells[rowCount + 2, 7] = p.eX.ToString();
                        wSheet.Cells[rowCount + 2, 8] = p.eY.ToString();
                        wSheet.Cells[rowCount + 2, 9] = p.eZ.ToString();
                        wSheet.Cells[rowCount + 2, 10] = p.mX.ToString();
                        wSheet.Cells[rowCount + 2, 11] = p.mY.ToString();
                        wSheet.Cells[rowCount + 2, 12] = p.mZ.ToString();
                        wSheet.Cells[rowCount + 2, 13] = p.mStartMeter.ToString();
                        wSheet.Cells[rowCount + 2, 14] = p.mEndMeter.ToString();
                        wSheet.Cells[rowCount + 2, 15] = p.mMidMeter.ToString();
                        rowCount++;
                    }
                    
 

                    //int size = excelTable.Columns.Count;
                    //for (int i = 0; i < size; i++)
                    //{
                    //    wSheet.Cells[1, 1 + i] = excelTable.Columns[i].ColumnName;
                    //}
                    //设置禁止弹出保存和覆盖的询问提示框 
                    app.DisplayAlerts = false;
                    app.AlertBeforeOverwriting = false;
                    //保存工作簿 

                    wBook.Save();

                    app.Quit();
                //}
                app = null;
                
            }
            catch (Exception err)
            {
                MessageBox.Show("导出Excel出错！错误原因：" + err.Message, "提示信息",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show("TEProj Success");

        }

        private void SaveTEPier()
        {
            //LoadDataTableFromExcel(fileName, "mile");

            Microsoft.Office.Interop.Excel.Application app =
    new Microsoft.Office.Interop.Excel.ApplicationClass();
            try
            {
                app.Visible = false;
                //Workbook wBook = app.Workbooks.Add(true);
                Workbook wBook = app.Workbooks.Open(@"D:\gisData\xlsx\TEPier.xlsx");
                Worksheet wSheet = wBook.Worksheets[1] as Worksheet;
                //if (ds != null)
                //{
                //System.Data.DataTable excelTable = ds.Tables["mile"];
                //System.Data.DataTable chainTable = ds.Tables["chain"];
                int rowCount = 0;
                foreach (CTEPier p in CTEPier.mPierList)
                {
                    wSheet.Cells[rowCount + 2, 1] = p.mParentBridge.mProjectName;
                    wSheet.Cells[rowCount + 2, 2] = p.mPierName;
                    wSheet.Cells[rowCount + 2, 3] = p.sgX.ToString();
                    wSheet.Cells[rowCount + 2, 4] = p.sgY.ToString();
                    wSheet.Cells[rowCount + 2, 5] = p.sgZ.ToString();
                    wSheet.Cells[rowCount + 2, 6] = p.sgRoll.ToString();
                    wSheet.Cells[rowCount + 2, 7] = p.sgYaw.ToString();
                    wSheet.Cells[rowCount + 2, 8] = p.mMeter.ToString();

                    rowCount++;
                }



                //int size = excelTable.Columns.Count;
                //for (int i = 0; i < size; i++)
                //{
                //    wSheet.Cells[1, 1 + i] = excelTable.Columns[i].ColumnName;
                //}
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
                //}
                app = null;

            }
            catch (Exception err)
            {
                MessageBox.Show("导出Excel出错！错误原因：" + err.Message, "提示信息",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show("TETier Success");

        }

        private void btnModel_Click(object sender, EventArgs e)
        {
            string fileName = @"D:\gisData\xlsx\middleLine.xlsx";

            CScene.loadMiddleLineFromDB(fileName);
            CTEProject.LoadTEProFromDB();
            CTEPier.LoadPiersFromDB();
            SaveTEProList();
            SaveTEPier();
            MessageBox.Show("导出成功！");

        }

        private void btnVW_Click(object sender, EventArgs e)
        {
            //CScene.loadMiddleLineFromDB(@"D:\gisData\xlsx\middleLine.xlsx");
            LoadChainList(@"D:\gisData\xlsx\middleLineInput.xlsx");
            LoadDataTableFromExcel(@"D:\gisData\xlsx\vw_ProjectInfo.xlsx", "vw_ProjectInfo");
            ProcessMidLine("vw_ProjectInfo");
            SaveDataTableToExcel( "vw_ProjectInfo");
            MessageBox.Show("导出成功！");

        }

        private void btnChain_Click(object sender, EventArgs e)
        {
            string fileName = @"D:\gisData\xlsx\middleLineInput.xlsx";
            //if (openFileDialog1.ShowDialog() == DialogResult.OK)
            //{
            //    fileName = openFileDialog1.FileName;
            //    //sheetName1 = "chain";
            //    //sheetName2 = "mile";
                LoadChainList(fileName);
                LoadAndSaveMileList(fileName);
                
            //    //LoadDataTableFromExcel(openFileDialog1.FileName,"sheet1");
            //}
            //SaveDataTableToExcel(@"D:\test.xlsx")
        }

    }
}
