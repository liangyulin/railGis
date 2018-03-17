using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Drawing;
//using lib_GIS.Service;
using Microsoft.Office.Interop.Excel;
using ModelInfo.Helper;
using Gavaghan.Geodesy;


namespace ModelInfo
{
    public class CRailwayScene
    {

        //  string sqlstr;
        //public ITerrainDynamicObject66 mDynamicTrain;
        //public ITerrainModel66 mTrain;
        public CRailwayLineList mMiddleLines = new CRailwayLineList();
        public string mDataPath, mProjectPath;  // 数据存放路径与工程存放路径
        public bool mUseLocalDB;
        //CServerWrapper mWSAgent;

        //const int MAX_MID_P = 28400;

        List<IHotSpot> mTotalSpotList = new List<IHotSpot>();
        public List<IHotSpot> mHotSpotList = new List<IHotSpot>(); // mHotSpotList
        public List<CRailwayBridge> mBridgeList = new List<CRailwayBridge>();
        public List<CRailwayRoad> mRoadList = new List<CRailwayRoad>();
        public List<CRailwayTunnel> mTunnelList = new List<CRailwayTunnel>();
        public List<CContBeam> mContBeamList = new List<CContBeam>();
        //public List<CRailwayProject> mOtherList = new List<CRailwayProject>();

        public List<CRailwayFirm> mFirmList = new List<CRailwayFirm>();
        //public List<CRailwayStaff> mStaffList = new List<CRailwayStaff>();

        public CRailwayScene()
        {
            //mWSAgent = new CServerWrapper();
            //Console.WriteLine(CoordinateConverter.getUTMDistance(120.142022774671, 36.248995887497, 120.142906127281, 36.2481853286171) + "\t 120");
            //Console.WriteLine(CoordinateConverter.getUTMDistance(120.142022774671, 36.248995887497, 120.150934476456, 36.2437588244801) + "\t 1000");

            
            mMiddleLines.LoadLineListFromDB();

            //double x,y,z,dir,x2,y2;
            //mMiddleLines.getPosbyDKCode("DK", 21074.9, out x, out y, out z, out dir);
            
            //// instantiate the calculator
            //GeodeticCalculator geoCalc = new GeodeticCalculator();

            //// select a reference elllipsoid
            //Ellipsoid reference = Ellipsoid.WGS84;
            //double xx, yy;
            //xx = 117.395521;
            //yy = 36.782793;
            //// set Lincoln Memorial coordinates
            //GlobalCoordinates pierGPS;
            //pierGPS = new GlobalCoordinates(
            //    new Angle(y), new Angle(x)
            //);

            //// set Eiffel Tower coordinates
            //GlobalCoordinates peopleMars;
            //peopleMars = new GlobalCoordinates(
            //    new Angle(yy), new Angle(xx)
            //);

            //// calculate the geodetic curve
            //GeodeticCurve geoCurve = geoCalc.CalculateGeodeticCurve(reference, pierGPS, peopleMars);
            //Console.WriteLine( geoCurve.EllipsoidalDistance + "\t 准确的火星");

            //Console.WriteLine(CoordinateConverter.getUTMDistance(xx, yy, x, y) + "\t 我的火星");

            //GPSAdjust.bd_decrypt(yy, xx, out y2, out x2);
            //Console.WriteLine(x + "\t" + y + "\t" + x2 + "\t" + y2);
            //GlobalCoordinates peopleEarth;
            //peopleEarth = new GlobalCoordinates(
            //    new Angle(y2), new Angle(x2)
            //);
            
            //Console.WriteLine(geoCalc.CalculateGeodeticCurve(reference, pierGPS, peopleEarth).EllipsoidalDistance+ "\t 准确的地球");
            //Console.WriteLine(CoordinateConverter.getUTMDistance(x2, y2, x, y) + "\t 我的地球");
            // 如果连接不到Server，只初始化中线，不再初始化其他信息
            if (!CServerWrapper.isConnected) return;

            //loadMiddleLineFromDB();
            //initProjects();
            initProjectsFromServer();

            initDWProjects();
            initFirmsFromServer();
            //foreach (var item in mTotalSpotList)
            //{
            //    Console.WriteLine(item.GlobalMileage);
            //}
            var dictSort = from d in mTotalSpotList orderby d.GlobalMileage ascending select d;
            //mHotSpotList.Clear();
            foreach (var item in dictSort)
            {
                if (mHotSpotList.Count > 0)
                    if (Math.Abs(item.GlobalMileage - mHotSpotList.Last().GlobalMileage) < 300)
                        continue;
                    else
                        mHotSpotList.Add(item);
                else
                    mHotSpotList.Add(item);
            }
            //initRegisteredStaffFromServer();
            //Console.WriteLine("after__________________");
            //foreach (var item in mHotSpotList)
            //{
            //    Console.WriteLine(item.GlobalMileage);
            //}

            //DatabaseWrapper.SavePierToDB(@"D:\GISData\jiqing\PierX.xls",mBridgeList);
            
            //mMiddleLines.SaveLineListToDB(@"D:\GISData\jiqing\MLTest.xls");
            //loadProjectsFromDB();
        } 

        private void initDWProjects()
        {
            #region init Items

            //DataSet ds = null;

            string ProjectSQL;

            string dwName, dwModelType, dwDKCode;
            int projID;
            double dwMileage, dwLength;
            //string Mileage_Start_Des, Mileage_End_Des, Mileage_Mid_Des, SerialNo;
            DateTime dwUpdateTime;
            int curProjIndex = 0;
            System.Data.DataTable dt = null;
            //double Direction, avgProgress;
            

            //CRailwayProject sItem = null;
            //mUseLocalDB = true;  

            //if (CServerWrapper.isConnected)
            //{
            //    ProjectSQL = @"SELECT Name, ProjectID, Mileage, ProjectLength, UpdateTime, ModelType from [Project_B_DW_Info$] where Name like '%墩' order by ProjectID asc;";

            //    string fileName = @"D:\GISData\jiqing\Project_B_dw_Info.xls";

            //    dt = DatabaseWrapper.LoadDataTableFromExcel(fileName, "Project_B_DW_Info", ProjectSQL);


            //}
            //else
            //{
                // web service模式
                //WS_GISServerData.GisDataWebServiceSoapClient ws = new WS_GISServerData.GisDataWebServiceSoapClient();
                //dt = ws.ws_FindDWProjectInfo();
                //string sql = @"SELECT Name, ProjectID, Mileage, ProjectLength, UpdateTime, ModelType from Project_B_DW_Info where Name like '%墩' ";
                
                //dt = CServerWrapper.findDWProjectInfo();

                //dt = CServerWrapper.execSqlQuery(@"SELECT  * from Project_B_DW_Info where not MileagePrefix = 'DK' and Name like '%墩' order by ProjectID asc;");
                
            //}

            dt = CServerWrapper.execSqlQuery(@"SELECT Name, ProjectID, MileagePrefix, Mileage, ProjectLength, UpdateTime, ModelType, SerialNo from Project_B_DW_Info where Name like '%墩' order by ProjectID asc");
            //DatabaseWrapper.PrintDataTable(dt);

            foreach (DataRow dataReader in dt.Rows)
            {
                try
                {
                    if (!CServerWrapper.isConnected)
                    {
                        projID = (int)((double)dataReader["ProjectID"] + 0.1);
                    }
                    else
                    {
                        projID = (int)dataReader["ProjectID"];

                    }
                    dwMileage = Convert.ToDouble(dataReader["Mileage"]);
                    dwLength = Convert.ToDouble(dataReader["ProjectLength"]);  
                    dwName = (string)dataReader["Name"];

                    dwModelType = (string)dataReader["ModelType"];
                    dwUpdateTime = (DateTime)dataReader["UpdateTime"];
                    while (curProjIndex < mBridgeList.Count && projID > mBridgeList[curProjIndex].mProjectID)
                    {
                        curProjIndex++;
                    }
                    if (curProjIndex == mBridgeList.Count)
                    {
                        Console.WriteLine("DW_input_Error" + dwName + dwMileage);
                        return;
                    }
                    mBridgeList[curProjIndex].mDWProjList.Add(new CRailwayPier(mBridgeList[curProjIndex], dataReader["Mileage"].ToString(), dwName, dwModelType, dwMileage, dwLength, dwUpdateTime, 0, dataReader["MileagePrefix"].ToString()));
   //                 Console.WriteLine(dwName +"\t"+ dwMileage);

                    //gProType.Add("-1-43-84-", "车站");
                    //gProType.Add("-1-42-26-", "桥梁");
                    //gProType.Add("-1-42-27-", "隧道");
                    //gProType.Add("-1-42-28-", "区间路基");
                    //gProType.Add("-1-42-74-", "涵洞");
                    //gProType.Add("-1-42-100-", "轨道");


                }//(string)dataReader["UpdateTime"],
                catch (System.Exception ex)
                {
                    Console.WriteLine((string)dataReader["Name"] + " invalid");
                }
            }

 
            #endregion
        }

        /// <summary>
        /// 未使用
        /// </summary>
//        private void initProjects()
//        {
//            #region init Items

//            //DataSet ds = null;
//            System.Data.DataTable dt = null;

//            string ProjectSQL;

//            string projectName, professionalName, ProfessionalCategoryCode, ShorName;
//            int projID;
//            double Mileage_Start, Mileage_End, Mileage_Mid, projLength;
//            string Mileage_Start_Des, Mileage_End_Des, Mileage_Mid_Des, SerialNo;
//            DateTime UpdateTime;
//            double Direction, avgProgress;


//            CRailwayProject sItem = null;
//            //mUseLocalDB = true;


//            try
//            {
//                if (!CServerWrapper.isConnected)
//                {
//                    ProjectSQL = @"SELECT ProjectID, ProjectName, ProfessionalName, ProfessionalCategoryCode, ShorName,Mileage_Start,Mileage_Mid, Mileage_End, 
//Mileage_Start_Des,Mileage_Mid_Des, Mileage_End_Des, ProjectLenth, SerialNo, UpdateTime, Direction , avgProgress from [vw_ProjectInfo$] order by ProjectID asc;";

//                    string fileName = CGisDataSettings.gDataPath + @"jiqing\vw_ProjectInfo.xls";

//                    dt = DatabaseWrapper.LoadDataTableFromExcel(fileName, "vw_ProjectInfo", ProjectSQL);

//                }
//                else
//                {
//                    //WS_GISServerData.GisDataWebServiceSoapClient ws = new WS_GISServerData.GisDataWebServiceSoapClient();
//                    //dt = ws.ws_FindProjectInfo();
////                    ProjectSQL = @"SELECT ProjectID, ProjectName, ProfessionalName, ProfessionalCategoryCode, ShorName,Mileage_Start,Mileage_Mid, Mileage_End, 
////Mileage_Start_Des,Mileage_Mid_Des, Mileage_End_Des, ProjectLenth, SerialNo, UpdateTime, Direction , avgProgress from vw_ProjectInfo where ProfessionalCategoryCode = '";
////                    ProjectSQL += CServerWrapper.findProjectCode("giscode_station") + @"' order by ProjectID asc;";
////                    dt = CServerWrapper.execSqlQuery(ProjectSQL);
//                    dt = CServerWrapper.findProjectInfo();
//                    //dt = ProjectFromServer.FindProjectInfo();
//                }
//                int count = dt.Rows.Count;
//                mProjectList = new List<CRailwayProject>(count);
//                mContBeamList = new List<CRailwayProject>();
//                mBridgeList = new List<CRailwayProject>();
//                mRoadList = new List<CRailwayProject>();
//                mTunnelList = new List<CRailwayProject>();
//                mOtherList = new List<CRailwayProject>();

//                foreach (DataRow dataReader in dt.Rows)
//                {
//                    try
//                    {
//                        if (!CServerWrapper.isConnected)
//                        {
//                            projID = (int)((double)dataReader["ProjectID"] + 0.1);

//                        }
//                        else
//                        {
//                            projID = (int)dataReader["ProjectID"];

//                        }

//                        Mileage_Start = Convert.ToDouble(dataReader["Mileage_Start"]);
//                        Mileage_Mid = Convert.ToDouble(dataReader["Mileage_Mid"]);
//                        Mileage_End = Convert.ToDouble(dataReader["Mileage_End"]);
//                        avgProgress = Convert.ToDouble(dataReader["avgProgress"]);
//                        Direction = Convert.ToDouble(dataReader["Direction"]);
//                        projLength = Convert.ToDouble(dataReader["ProjectLenth"]);

//                        projectName = (string)dataReader["ProjectName"];
//                        professionalName = (string)dataReader["ProfessionalName"];
//                        ProfessionalCategoryCode = (string)dataReader["ProfessionalCategoryCode"];
//                        ShorName = (string)dataReader["ShorName"];

//                        Mileage_Start_Des = (string)dataReader["Mileage_Start_Des"];
//                        Mileage_Mid_Des = (string)dataReader["Mileage_Mid_Des"];
//                        Mileage_End_Des = (string)dataReader["Mileage_End_Des"];
//                        SerialNo = (string)dataReader["SerialNo"];
//                        UpdateTime = DateTime.Now; // (DateTime)dataReader["UpdateTime"];

//                        ///没有车站表，暂时以站场路基计算，车站方向暂时位于铁路同一侧
//                        //if (ProfessionalCategoryCode.StartsWith("-1-42-28-84-")) //-1-43-84-
//                        if (ProfessionalCategoryCode.StartsWith(CServerWrapper.findProjectCode("giscode_station")))
//                        {
//                            int ix = projectName.IndexOf('站');
//                            Console.WriteLine(projectName);
//                            if (ix >= 0)
//                            {
//                                projectName = projectName.Substring(0, ix + 1);
//                                Direction = 90;
//                            }
//                            sItem = new CRailwayProject(this, SerialNo, projID, professionalName, projectName, ShorName,
//                                Mileage_Start, Mileage_Mid, Mileage_End, Mileage_Start_Des, Mileage_Mid_Des, Mileage_End_Des, UpdateTime, avgProgress, Direction, @"s2.jpg", projLength);
//                            mProjectList.Add(sItem);
//                            //mOtherList.Add(sItem);
//                            mContBeamList.Add(sItem);

//                        }
//                        else if (ProfessionalCategoryCode.StartsWith("-1-42-26-"))
//                        {
//                            sItem = new CRailwayProject(this, SerialNo, projID, professionalName, projectName, ShorName,
//                                Mileage_Start, Mileage_Mid, Mileage_End, Mileage_Start_Des, Mileage_Mid_Des, Mileage_End_Des, UpdateTime, avgProgress, Direction, @"桥梁.png", projLength);
//                            mProjectList.Add(sItem);
//                            mBridgeList.Add(sItem);
//                        }
//                        else if (ProfessionalCategoryCode.StartsWith("-1-42-28-"))
//                        {
//                            sItem = new CRailwayProject(this, SerialNo, projID, professionalName, projectName, ShorName,
//                                Mileage_Start, Mileage_Mid, Mileage_End, Mileage_Start_Des, Mileage_Mid_Des, Mileage_End_Des, UpdateTime, avgProgress, Direction, @"路基.png", projLength);
//                            mProjectList.Add(sItem);
//                            mRoadList.Add(sItem);

//                        }
//                        else if (ProfessionalCategoryCode.StartsWith("-1-42-31-")) // && projectName.EndsWith("站") // 梁厂
//                        {
//                            sItem = new CRailwayProject(this, SerialNo, projID, professionalName, projectName, ShorName,
//                                Mileage_Start, Mileage_Mid, Mileage_End, Mileage_Start_Des, Mileage_Mid_Des, Mileage_End_Des, UpdateTime, avgProgress, Direction, @"路基.png", projLength);
//                            mOtherList.Add(sItem);
//                            //mStationList.Add(sItem);
//                        } // professionalID = "27"
//                        else if (ProfessionalCategoryCode.StartsWith("-1-42-27-") || ProfessionalCategoryCode.StartsWith("-1-42-74-"))
//                        {
//                            sItem = new CRailwayProject(this, SerialNo, projID, professionalName, projectName, ShorName,
//                                Mileage_Start, Mileage_Mid, Mileage_End, Mileage_Start_Des, Mileage_Mid_Des, Mileage_End_Des, UpdateTime, avgProgress, Direction, @"涵洞.png", projLength);
//                            mProjectList.Add(sItem);
//                            mTunnelList.Add(sItem);

//                        }


//                    }//(string)dataReader["UpdateTime"],
//                    catch (System.Exception ex)
//                    {
//                        Console.WriteLine((string)dataReader["ProjectName"] + " invalid");
//                    }
//                }
//                //System.Console.WriteLine("readok");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message.ToString());
//            }

//            #endregion
//        }

        /// <summary>
        ///giscode_station	-1-42-28-84-
        ///giscode_bridge	-1-42-26-
        ///giscode_road	-1-42-28-
        ///giscode_liangchang	-1-42-31-
        ///giscode_tunnel	-1-42-27-
        ///giscode_handong	-1-42-74-
        /// </summary>
        private void initProjectsFromServerNoUse() {
            //initRegisteredStaffFromServer();

            //double xx, yy, zz;
            //CoordinateConverter.UTMXYToLatLon(517670.66484, 4032205.24460,  out xx, out yy, 50, false);
            //Console.WriteLine("xx\t {0} yy\t {1}", xx, yy);
            //CoordinateConverter.UTMXYToLatLon(4032205.24460, 517670.66484, out xx, out yy, 50, false);
            //Console.WriteLine("xx\t {0} yy\t {1}", xx, yy);
            //mProjectList = new List<CRailwayProject>();
            //mStationList = new List<CRailwayProject>();
            //mBridgeList = new List<CRailwayProject>();
            //mRoadList = new List<CRailwayProject>();
            //mTunnelList = new List<CRailwayProject>();
            //mOtherList = new List<CRailwayProject>();
            //initOneTypeProjectsFromServer("giscode_station", mStationList, @"s2.jpg");
            //initOneTypeProjectsFromServer("giscode_bridge", mBridgeList, @"桥梁.png");
            //initOneTypeProjectsFromServer("giscode_road", mRoadList, @"路基.png");
            //initOneTypeProjectsFromServer("giscode_tunnel", mTunnelList, @"涵洞.png");
            //initOneTypeProjectsFromServer("giscode_liangchang", mOtherList, @"LC.png");
            //double x, y, z, dir;
            //mMiddleLines.getPosbyDKCode("DK", 1908.95, out x, out y, out z, out dir);
            //Console.WriteLine("{0}\t{1}\t{2}  ", x, y, z);
            //initOneTypeProjectsFromServer("-1-42-28-84-", mStationList, @"s2.jpg");
            //initOneTypeProjectsFromServer("-1-42-26-", mBridgeList, @"桥梁.png");
            //initOneTypeProjectsFromServer("-1-42-28-", mRoadList, @"路基.png");
            //initOneTypeProjectsFromServer("-1-42-27-", mTunnelList, @"涵洞.png",true);
            //initOneTypeProjectsFromServer("-1-42-31-", mOtherList, @"LC.png");

            //foreach (CRailwayProject rp in mBridgeList)
            //{
            //    if (rp.mDKCode_Start.Equals("DK"))
            //        Console.WriteLine(rp);
            //}

            //Console.WriteLine();

            //foreach (CRailwayProject rp in mRoadList)
            //{
            //    if (rp.mDKCode_Start.Equals("DK"))
            //        Console.WriteLine(rp);
            //}


        }

        private void initProjectsFromServer()
        {
            #region init Items

            //DataSet ds = null;
            System.Data.DataTable dt = null;

            string ProjectSQL = @"SELECT ProjectID, ProjectName, ProfessionalName, ProfessionalCategoryCode, ShorName,Mileage_Start,Mileage_Mid, Mileage_End, 
                  MileagePrefix , SerialNo, UpdateTime, Direction , avgProgress, ParentID from vw_ProjectInfo where ProfessionalCategoryCode like '-1-42-26-%' OR
    ProfessionalCategoryCode like '-1-42-27-%' OR ProfessionalCategoryCode like '-1-42-28-%' order by ProjectID asc;";
            //string[] professionalCateCode = { "-1-42-26-", "-1-42-28-", "-1-42-27-" }; // 桥梁，路基，涵洞
           
            string projectName, professionalName, ProfessionalCategoryCode, ShorName;
            int projID,parentID;
            double Mileage_Start, Mileage_End, Mileage_Mid;
            string dkCode, SerialNo;
            DateTime UpdateTime;
            double Direction, avgProgress;


            CRailwayProject sItem = null;
            
            try
            {
//                if (isTunnel)
//                    ProjectSQL = @"SELECT ProjectID, ProjectName, ProfessionalName, ProfessionalCategoryCode, ShorName,Mileage_Start,Mileage_Mid, Mileage_End, 
//                    Mileage_Start_Des,Mileage_Mid_Des, Mileage_End_Des, ProjectLenth, SerialNo, UpdateTime, Direction , avgProgress, ParentID from vw_ProjectInfo where ProfessionalCategoryCode like '";
//                else
//                    ProjectSQL = @"SELECT ProjectID, ProjectName, ProfessionalName, ProfessionalCategoryCode, ShorName,Mileage_Start,Mileage_Mid, Mileage_End, 
//                    Mileage_Start_Des,Mileage_Mid_Des, Mileage_End_Des, ProjectLenth, SerialNo, UpdateTime, Direction , avgProgress, ParentID from vw_ProjectInfo where ParentID=0 AND ProfessionalCategoryCode like '";
//                //ProjectSQL += CServerWrapper.findProjectCode(projCode) + @"%' order by ProjectID asc;"; 
//                ProjectSQL += projCode + @"%' order by ProjectID asc;"; 
                dt = CServerWrapper.execSqlQuery(ProjectSQL);

                //int count = dt.Rows.Count;


                foreach (DataRow dataReader in dt.Rows)
                {
                        projID = (int)dataReader["ProjectID"];
                        Mileage_Start = Convert.ToDouble(dataReader["Mileage_Start"]);
                        Mileage_Mid = Convert.ToDouble(dataReader["Mileage_Mid"]);
                        Mileage_End = Convert.ToDouble(dataReader["Mileage_End"]);
                        avgProgress = Convert.ToDouble(dataReader["avgProgress"]);
                        Direction = Convert.ToDouble(dataReader["Direction"]);
                        projectName = (string)dataReader["ProjectName"];
                        professionalName = (string)dataReader["ProfessionalName"];
                        ProfessionalCategoryCode = (string)dataReader["ProfessionalCategoryCode"];
                        ShorName = (string)dataReader["ShorName"];
                        dkCode = (string)dataReader["MileagePrefix"];
                        SerialNo = (string)dataReader["SerialNo"];
                        UpdateTime = (DateTime)dataReader["UpdateTime"]; // (DateTime)dataReader["UpdateTime"];
                    parentID = (int)dataReader["ProjectID"];
                        if (ProfessionalCategoryCode.StartsWith("-1-42-26-")) // 桥梁
                        {
                            if (ProfessionalCategoryCode.StartsWith("-1-42-26-81-")) // 连续梁
                            {
                                sItem = new CContBeam(this, SerialNo, projID, professionalName, projectName, ShorName, dkCode,
                                Mileage_Start, Mileage_Mid, Mileage_End, UpdateTime, avgProgress, Direction, @"桥梁.png");
                                mContBeamList.Add((CContBeam)sItem);
                                if (sItem.GlobalMileage > 0)
                                    mTotalSpotList.Add(sItem);
                            }
                            else
                            {
                                sItem = new CRailwayBridge(this, SerialNo, projID, professionalName, projectName, ShorName, dkCode,
                                Mileage_Start, Mileage_Mid, Mileage_End, UpdateTime, avgProgress, Direction, @"桥梁.png");
                                mBridgeList.Add((CRailwayBridge)sItem);
                            }
                        }
                        else if (ProfessionalCategoryCode.StartsWith("-1-42-27-")) // 隧道，涵洞
                        {
                                sItem = new CRailwayTunnel(this, SerialNo, projID, professionalName, projectName, ShorName, dkCode,
                                Mileage_Start, Mileage_Mid, Mileage_End, UpdateTime, avgProgress, Direction, @"涵洞.png");
                                mTunnelList.Add((CRailwayTunnel)sItem);
                                if (sItem.GlobalMileage > 0)
                                    mTotalSpotList.Add(sItem);

                        } 
                        else if (ProfessionalCategoryCode.StartsWith("-1-42-28-")) // 路基
                        {
                                sItem = new CRailwayRoad(this, SerialNo, projID, professionalName, projectName, ShorName, dkCode,
                                Mileage_Start, Mileage_Mid, Mileage_End, UpdateTime, avgProgress, Direction, @"路基.png");
                                mRoadList.Add((CRailwayRoad)sItem);
                                if (sItem.GlobalMileage > 0)
                                    mTotalSpotList.Add(sItem);

                        } 

                        ///没有车站表，暂时以站场路基计算，车站方向暂时位于铁路同一侧
                        //if (ProfessionalCategoryCode.StartsWith("-1-42-28-84-")) //-1-43-84-
                        //if (ProfessionalCategoryCode.StartsWith(CServerWrapper.findProjectCode("giscode_station")))
                        //{
                        //    int ix = projectName.IndexOf('站');
                        ////Console.WriteLine(projectName);
                        //    if (ix >= 0)
                        //    {
                        //        projectName = projectName.Substring(0, ix + 1);
                        //        Direction = 90;
                        //    }
                        //    sItem = new CRailwayProject(this, SerialNo, projID, professionalName, projectName, ShorName,
                        //        Mileage_Start, Mileage_Mid, Mileage_End, Mileage_Start_Des, Mileage_Mid_Des, Mileage_End_Des, UpdateTime, avgProgress, Direction, fileName, projLength);
                        //    mProjectList.Add(sItem);
                        //    //mOtherList.Add(sItem);
                        //    projList.Add(sItem);

                        //}
                        //else if (ProfessionalCategoryCode.StartsWith("-1-42-26-"))
                        //{
                        //    sItem = new CRailwayProject(this, SerialNo, projID, professionalName, projectName, ShorName,
                        //        Mileage_Start, Mileage_Mid, Mileage_End, Mileage_Start_Des, Mileage_Mid_Des, Mileage_End_Des, UpdateTime, avgProgress, Direction, @"桥梁.png", projLength);
                        //    mProjectList.Add(sItem);
                        //    mBridgeList.Add(sItem);

                        //}
                        //else if (ProfessionalCategoryCode.StartsWith("-1-42-28-"))
                        //{
                        //    sItem = new CRailwayProject(this, SerialNo, projID, professionalName, projectName, ShorName,
                        //        Mileage_Start, Mileage_Mid, Mileage_End, Mileage_Start_Des, Mileage_Mid_Des, Mileage_End_Des, UpdateTime, avgProgress, Direction, @"路基.png", projLength);
                        //    mProjectList.Add(sItem);
                        //    mRoadList.Add(sItem);

                        //}
                        //else if (ProfessionalCategoryCode.StartsWith("-1-42-31-")) // && projectName.EndsWith("站") // 梁厂
                        //{
                        //    sItem = new CRailwayProject(this, SerialNo, projID, professionalName, projectName, ShorName,
                        //        Mileage_Start, Mileage_Mid, Mileage_End, Mileage_Start_Des, Mileage_Mid_Des, Mileage_End_Des, UpdateTime, avgProgress, Direction, @"路基.png", projLength);
                        //    mOtherList.Add(sItem);
                        //    //mStationList.Add(sItem);
                        //}
                        //else if (ProfessionalCategoryCode.StartsWith("-1-42-27-") || ProfessionalCategoryCode.StartsWith("-1-42-74-"))
                        //{
                        //    sItem = new CRailwayProject(this, SerialNo, projID, professionalName, projectName, ShorName,
                        //        Mileage_Start, Mileage_Mid, Mileage_End, Mileage_Start_Des, Mileage_Mid_Des, Mileage_End_Des, UpdateTime, avgProgress, Direction, @"涵洞.png", projLength);
                        //    mProjectList.Add(sItem);
                        //    mTunnelList.Add(sItem);

                        //}


                }
                //System.Console.WriteLine("readok");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            #endregion
        }

        /// <summary>
        ///   string sqlstr = @"select FirmName ,a.FirmTypeID, CategoryCode, SerialNo, UpdateTime, Longitude, Latitude from (select * from FirmInfo)a, (select FirmTypeID,FirmTypeCategoryName from FirmTypeInfo)b where a.FirmTypeID=b.FirmTypeID and( FirmTypeCategoryName='单位' or FirmTypeCategoryName='分支机构') and Longitude > 10 AND Latitude > 10 order by a.FirmTypeID asc ;"; 
        /// </summary>
        private void initFirmsFromServer() //string projCode, List<CRailwayProject> projList, string fileName
        {
            #region init Items

            System.Data.DataTable dt = null;

            string ProjectSQL;

            int firmID;
            string fileName = null;
            string firmType = null;

            CRailwayFirm sItem = null;

            try
            {
                ProjectSQL = @"select a.*,b.ShorName,b.Latitude,b.Longitude,b.FirmTypeID, c.FirmTypeCategoryName from 
                    (select count(usrid) as num,firmid from UsrInfo 
                    where  idcardno is not null group by firmid)a,
                    (select latitude,longitude,firmid,ShorName,FirmTypeID from FirmInfo
                    where Latitude!=0 and Longitude!=0)b,
                    (select FirmTypeID,FirmTypeCategoryName from FirmTypeInfo
                    where FirmTypeCategoryName='单位' or FirmTypeCategoryName='分支机构')c
                    where a.firmid=b.FirmID and b.FirmTypeID=c.FirmTypeID";
                //ProjectSQL = @"select FirmName ,a.FirmTypeID, CategoryCode, SerialNo, UpdateTime, Longitude, Latitude from (select * from FirmInfo)a, (select FirmTypeID,FirmTypeCategoryName from FirmTypeInfo)b where a.FirmTypeID=b.FirmTypeID and( FirmTypeCategoryName='单位' or FirmTypeCategoryName='分支机构') and Longitude > 10 AND Latitude > 10 order by a.FirmTypeID asc ;"; 
                //ProjectSQL += CServerWrapper.findProjectCode(projCode) + @"%' order by ProjectID asc;"; 
                //ProjectSQL += projCode + @"%' order by ProjectID asc;";
                dt = CServerWrapper.execSqlQuery(ProjectSQL);
                //DatabaseWrapper.PrintDataTable(dt);
                foreach (DataRow dataReader in dt.Rows)
                {
                    firmID = (int)dataReader["FirmTypeID"];
                    switch (firmID)
                    {
                        case 2: 
                            firmType = "施工单位";
                            fileName = @"施工单位.png";
                            break;
                        case 5:
                            firmType = "建设单位";
                            fileName = @"单位.png";
                            break;
                        case 7:
                            firmType = "制梁场";
                            fileName = @"LC.png";
                            break;
                        case 8:
                            firmType = "监理单位";
                            fileName = @"监理单位.png";
                            break;
                        case 10:
                            firmType = "项目部";
                            fileName = @"工程.png";
                            break;
                    }

                    sItem = new CRailwayFirm(this, Convert.ToInt32(dataReader["Num"]), Convert.ToInt32(dataReader["firmid"]), dataReader["ShorName"].ToString(),
                        Convert.ToDouble(dataReader["Longitude"]), Convert.ToDouble(dataReader["Latitude"]), firmType, fileName);
                    //sItem = new CRailwayFirm(this, firmType, dataReader["FirmName"].ToString(), dataReader["SerialNo"].ToString(), (DateTime)dataReader["UpdateTime"],
                    //    Convert.ToDouble(dataReader["Longitude"]), Convert.ToDouble(dataReader["Latitude"]), fileName);
                    mFirmList.Add(sItem);
                    if (sItem.GlobalMileage > 0)
                        mTotalSpotList.Add(sItem);

                }
                //System.Console.WriteLine("readok");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            #endregion
        }

 
    }
}
