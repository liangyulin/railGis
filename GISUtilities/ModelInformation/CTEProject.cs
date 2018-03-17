using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Threading.Tasks;
//using TerraExplorerX;

namespace GISUtilities.ModelInformation
{
    public class CTEProject //: SKRailwayItem
    {
        public double sX, sY, sZ, mX,mY,mZ, eX,eY,eZ;  // 经纬度坐标
        //double sgsX, sgsY, sgsZ;
        public string mProjectType;
        public string mProjectName;
        public double mStartMeter, mEndMeter, mMidMeter;

        public string mModelName;
        //IColor66 mTintColor;
        //ITerrainModel66 mTEModel;
        double mOpacity;
        public List<CTEPier> mPierList = new List<CTEPier>();
         
        public static List<CTEProject> mTEProjectList= new List<CTEProject>();

        public CTEProject(string proType, string proName, string proFile, 
            double x1, double y1, double z1, double x2, double y2, double z2)
        {
            mProjectType = proType;
            mProjectName = proName;
            //mStartMeter = ms;
            //mEndMeter = me;
            //mMidMeter = mm;
            mModelName = proFile;
            //mModelName = GlobalVar.gDataPath + @"mn\Models\" + proFile + ".x";
            //mMileageMid = mileageM;
            sX = x1;
            sY = y1;
            sZ = z1;
            mX = x2;
            mY = y2;
            mZ = z2;
            mStartMeter = CScene.mMiddleLine.findMeterbyCoor(sX, sY);
            mMidMeter = CScene.mMiddleLine.findMeterbyCoor(mX, mY);
            mEndMeter = mMidMeter * 2 - mStartMeter;
            double dir;
            CScene.mMiddleLine.findPosbyMeter(mEndMeter, out eX, out eY, out eZ, out dir);


             //mOpacity = 1;
            
        }

        public static void LoadTEProFromDB()
        {
            OleDbConnection conn = null;
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            CTEProject cItem = null;


                try
                {
                    string fileName = @"D:\gisData\xlsx\MNTEProj.xlsx";
                    string strConn;
                    if (System.IO.Path.GetExtension(fileName) == ".xls")
                        strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + fileName + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
                    else
                        strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + fileName + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";

                    conn = new OleDbConnection(strConn);
                    conn.Open();
                    myCommand = new OleDbDataAdapter(@"select * from [TEProject$]; ", conn);//order by Mileage asc
                    ds = new DataSet();
                    myCommand.Fill(ds, "table1");

                    foreach (DataRow dr in ds.Tables["table1"].Rows)
                    {
                        cItem = new CTEProject((string)dr["ProjectType"], (string)dr["ProjectName"],
                                (string)dr["ProjectModel"], (double)dr["LongitudeS"], (double)dr["LatitudeS"], (double)dr["AltitudeS"],
                                (double)dr["LongitudeM"], (double)dr["LatitudeM"], (double)dr["AltitudeM"]);

                        mTEProjectList.Add(cItem);
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                }
                finally
                {
                    conn.Close();
                }                
                

        }

        //public static void LoadTEProModel()
        //{
        //    var sgworld = new SGWorld66();
        //    //IColor66 bgcolor = sgworld.Creator.CreateColor(255, 0, 0, 255);
        //    var branch = sgworld.ProjectTree.CreateGroup("TEModels");
        //    foreach (CTEProject cItem in mTEProjectList)
        //    {
        //        var cPos = sgworld.Creator.CreatePosition(cItem.sgX,cItem.sgY,cItem.sgZ,AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);
        //        cItem.mTEModel = sgworld.Creator.CreateModel(cPos, cItem.mModelName, 1, ModelTypeCode.MT_NORMAL, branch, cItem.mProjectName);
        //        cItem.mTEModel.Terrain.Tint.abgrColor = 0xFF00;
        //        cItem.mTEModel.Terrain.Tint.SetAlpha(0.20); // FIXME;
        //    }
        //}

    }

    public class CTEPier
    {
        public double sgX, sgY, sgZ;
        public double sgYaw, sgRoll;

        public string mPierName;
        static string mModelName ;
        public double mMeter;
        //IColor66 mTintColor;
        //public ITerrainModel66 mTEModel;
        //double mOpacity;
        public CTEProject mParentBridge;
        public DateTime dts, dte;

        public static List<CTEPier> mPierList= new List<CTEPier>();

        public CTEPier(string pName, CTEProject parent, double x, double y, double z, double roll,double yaw)
        {
            mPierName = pName;
            mParentBridge = parent;
            parent.mPierList.Add(this);
            sgX = x;
            sgY = y;
            sgZ = z;
            sgYaw = yaw;
            sgRoll = roll;
            mMeter = CScene.mMiddleLine.findMeterbyCoor(sgX, sgY);
        }

        public static void LoadPiersFromDB()
        {
            OleDbConnection conn = null;
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;

            CTEPier cItem = null;
            CTEProject pbridge;
            string bridgeName;

            try
            {
                string fileName = @"D:\gisData\xlsx\MNTEPier.xlsx";
                string strConn;
                if (System.IO.Path.GetExtension(fileName) == ".xls")
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + fileName + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
                else
                    strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + fileName + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";

                conn = new OleDbConnection(strConn);
                conn.Open();
                myCommand = new OleDbDataAdapter(@"select * from [TEPier$]; ", conn);//order by Mileage asc
                ds = new DataSet();
                myCommand.Fill(ds, "table1");

                foreach (DataRow dr in ds.Tables["table1"].Rows)
                {
                    bridgeName = (string)dr["BridgeName"];
                    pbridge = null;
                    foreach (CTEProject b in CTEProject.mTEProjectList)
                    {
                        if (b.mProjectName.Equals(bridgeName))
                        {
                            pbridge = b;
                            break;
                        }
                    }
                    if (pbridge == null) continue;

                    cItem = new CTEPier((string)dr["PierName"], pbridge,
                                (double)dr["Longitude"], (double)dr["Latitude"], (double)dr["Altitude"],
                                (double)dr["RollOffset"], (double)dr["YawOffset"]);

                    mPierList.Add(cItem);

                   
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(cItem.mPierName + " invalid");
            }
            finally
            {
                conn.Close();
            }

        }

        //public static void LoadPiersModels()
        //{
        //    var sgworld = new SGWorld66();
        //    //string bridgeItem;
        //    mModelName = GlobalVar.gDataPath + @"mn\Models\qiaodun.xpl2";
        //    var branch = sgworld.ProjectTree.CreateGroup("Pier");
        //    double alpha = 0.01;
        //    //IColor66 bgcolor = sgworld.Creator.CreateColor(255, 0, 0, 255);
        //    int i = 0;
        //    foreach (CTEPier cItem in mPierList )
        //    {
        //        //var branch = sgworld.ProjectTree.FindItem("Bridge\\"+cItem.mParentBridge.mProjectName);
        //        var cPos = sgworld.Creator.CreatePosition(cItem.sgX, cItem.sgY, cItem.sgZ - 4.5, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE,
        //            cItem.sgYaw + 90, 0, cItem.sgRoll );
        //        cItem.mTEModel = sgworld.Creator.CreateModel(cPos, mModelName, 1, ModelTypeCode.MT_NORMAL, branch, cItem.mPierName);
        //        cItem.mTEModel.ScaleX = 0.01;
        //        cItem.mTEModel.ScaleY = 0.01;
        //        cItem.mTEModel.ScaleZ = 0.015;
        //        cItem.mTEModel.BestLOD = 1000;
                
        //        cItem.dte = DateTime.Now.AddDays(100);;
        //        DateTime ds = Convert.ToDateTime("2015-06-01");
        //        cItem.dts = ds.AddDays(i);
        //        cItem.mTEModel.TimeSpan.Start = cItem.dts;
        //        cItem.mTEModel.TimeSpan.End = cItem.dte;
        //        i += 10;
        //        if (i > 180) { i = 0; }
        //        cItem.mTEModel.Terrain.Tint.abgrColor = 0xFF;
        //        cItem.mTEModel.Terrain.Tint.SetAlpha(alpha); // FIXME;
                
        //        alpha += 0.05;
        //        if (alpha > 1) alpha = 0.02;
        //    }
        //}


    }
}
