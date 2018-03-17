using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using TerraExplorerX;


namespace RailwayGIS.ModelInformation
{
    public class CRailwayScene
    {

        //  string sqlstr;
        public ITerrainDynamicObject66 mDynamicTrain;
        public ITerrainModel66 mTrain;
        public static CRailWayMLine mMiddleLine;

        const int MAX_MID_P = 10100;

        public static List<CRailwayProject> mProjectList = new List<CRailwayProject>();
        public static List<CRailwayProject> mBridgeList = new List<CRailwayProject>();
        public static List<CRailwayProject> mRoadList = new List<CRailwayProject>();
        public static List<CRailwayProject> mTunnelList = new List<CRailwayProject>();
        public static List<CRailwayProject> mStationList = new List<CRailwayProject>();
        public static List<CRailwayProject> mOtherList = new List<CRailwayProject>();

        public CRailwayScene()
        {
            loadMiddleLineFromDB(); 
            loadProjectsFromDB();
         }

        public static CRailwayProject findProjectByMeter(double meter)
        {
            meter /= 1000.0;
            CRailwayProject p2 = null;
            foreach (CRailwayProject p in mProjectList)
            {
                if (meter >= p.mMileage_Start & meter <= p.mMileage_End)
                {
                    if (p2 == null)
                        p2 = p;
                    else
                    {
                        p2 = Math.Abs(p2.mMileage_Mid - meter) > Math.Abs(p.mMileage_Mid - meter) ? p : p2;
                    }
                    //break;
                }
            }
            return p2;
        }
        public static CRailwayProject findProjectByCoor(double x, double y)
        {
            CRailwayProject p = null;
            double meters;
            meters = mMiddleLine.findMeterbyCoor(x, y);
            p = findProjectByMeter(meters);
            return p;
        }

        //初始化三维中线，始终访问本地数据，不会更新
        private void loadMiddleLineFromDB()
        {

            double[] m, x, y, z;
            m = new double[MAX_MID_P];
            x = new double[MAX_MID_P];
            y = new double[MAX_MID_P];
            z = new double[MAX_MID_P];


            double meter = -1;

            OleDbConnection conn = null;
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;

            int count = 0;
            try
            {
                string fileName = GlobalVar.gDataPath + @"mn\middleLine.xlsx";
                string strConn;
                if (System.IO.Path.GetExtension(fileName) == ".xls")
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + fileName + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
                else
                    strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + fileName + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";

                conn = new OleDbConnection(strConn);
                conn.Open();
                myCommand = new OleDbDataAdapter(@"select Mileage,Longitude,Latitude,Altitude,ID from [MileageInfo$] order by Mileage asc; ", conn);
                ds = new DataSet();
                myCommand.Fill(ds, "table1");


                bool flag = true;
                foreach (DataRow dr in ds.Tables["table1"].Rows)
                {
                    meter = (double)dr["Mileage"];
                    if (count > 0 && Math.Abs(meter - m[count - 1]) < 0.5)
                    {                       
                        continue;                        
                    }
                    if (count > 0 && meter < m[count - 1]) 
                    {
                        Console.WriteLine(m[count - 1] + (flag ? "DIK" : "DK") + "\t" + meter);
                        flag = !flag;
                        continue;
                    }
                    m[count] = meter;
                    x[count] = (double)dr["Longitude"];
                    y[count] = (double)dr["Latitude"];
                    z[count] = (double)dr["Altitude"];
                    //if (count > 1 && m[count] <= m[count - 1])
                    //    Console.WriteLine(count +"\t" + m[count]);
                    count++;
                }

                mMiddleLine = new CRailWayMLine(count, m, x, y, z);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("中线初始化错误");
            }
            finally
            {

                    conn.Close();

            }

        }




        private void loadProjectsFromDB()
        {
            #region init Items

            OleDbConnection conn = null;


            OleDbDataAdapter myCommand = null;
            DataSet ds = null;

            string ProjectSQL;

            string projectName, professionalName;
            string projectCatCode;
            //double meters = 0;
            CRailwayProject sItem = null;
            GlobalVar.useLocalDB = true;
            try
            {
                if (!GlobalVar.useLocalDB)
                {
                    ProjectSQL = @"SELECT SerialNo,ShorName,ProfessionalName,  "
            + @"ProjectName,ProjectName_en_us ,ProfessionalCategoryCode, Mileage_Start, Mileage_End,"
            + @"UpdateTime, avgProgress, direction FROM vw_ProjectInfo WHERE ProfessionalCategoryCode like '-1-42-%'"
            + @"OR ProfessionalCategoryCode like '-1-43-%' OR ProfessionalCategoryCode like '-1-87-%';";
                    conn = new OleDbConnection(GlobalVar.gConnectStr);                
                    conn.Open();

                    myCommand = new OleDbDataAdapter(ProjectSQL, conn);
                    ds = new DataSet();
                    myCommand.Fill(ds, "table1");

                }
            }catch (Exception ex){
                Console.WriteLine(ex.Message.ToString());
                MessageBox.Show("Can't Connect Server, use local database instead. ");
                GlobalVar.useLocalDB = true;

            }
            try {
                if (GlobalVar.useLocalDB)
                {
                    ProjectSQL = @"SELECT SerialNo,ShorName,ProfessionalName,  "
            + @"ProjectName,ProjectName_en_us ,ProfessionalCategoryCode, Mileage_Start, Mileage_End,"
            + @"UpdateTime, avgProgress, direction FROM vw_ProjectInfo WHERE ProfessionalCategoryCode like '-1-42-%'"
            + @"OR ProfessionalCategoryCode like '-1-43-%' OR ProfessionalCategoryCode like '-1-87-%';";
                    string fileName = GlobalVar.gDataPath + @"mn\vw_ProjectInfo.xlsx";
                    string strConn;
                    if (System.IO.Path.GetExtension(fileName) == ".xls")
                        strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + fileName + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
                    else
                        strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + fileName + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";

                    conn = new OleDbConnection(strConn);
                    conn.Open();
                    myCommand = new OleDbDataAdapter(ProjectSQL, conn);
                    ds = new DataSet();
                    myCommand.Fill(ds, "table1");

                }
                int count = ds.Tables["table1"].Rows.Count;
                mProjectList = new List<CRailwayProject>(count);
                mStationList = new List<CRailwayProject>();
                mBridgeList = new List<CRailwayProject>();
                mRoadList = new List<CRailwayProject>();
                mTunnelList = new List<CRailwayProject>();
                mOtherList = new List<CRailwayProject>();

                foreach (DataRow dataReader in ds.Tables["table1"].Rows)
                {
                    try
                    {
                        projectCatCode = (string)dataReader["ProfessionalCategoryCode"];
                        projectName = (string)dataReader["ProjectName_en_us"];
                        if (projectName == null || projectName == "")
                            projectName = (string)dataReader["ProjectName"];

                        professionalName = (string)dataReader["ProfessionalName"];

                        // 统一以米为单位
                        if (!GlobalVar.useLocalDB)
                        {
                            sItem = new CRailwayProject((string)dataReader["SerialNo"], professionalName, projectName, (string)dataReader["ShorName"],
                                Decimal.ToDouble((decimal)dataReader["Mileage_Start"]) * 1000,  Decimal.ToDouble((decimal)dataReader["Mileage_End"]) * 1000,
                                (DateTime)dataReader["UpdateTime"], Decimal.ToDouble((decimal)dataReader["avgProgress"]),Decimal.ToDouble((decimal)dataReader["direction"]));
                        }
                        else
                        {
                            sItem = new CRailwayProject((string)dataReader["SerialNo"], professionalName, projectName, (string)dataReader["ShorName"],
                                (double)dataReader["Mileage_Start"] * 1000, (double)dataReader["Mileage_End"] * 1000,
                                (DateTime)dataReader["UpdateTime"], (double)dataReader["avgProgress"], (double)dataReader["direction"]);
                        }
                        mProjectList.Add(sItem);
                        if (projectCatCode.StartsWith("-1-42-26-"))
                            mBridgeList.Add(sItem);
                        else if (projectCatCode.StartsWith("-1-42-28-"))
                            mRoadList.Add(sItem);
                        else if (projectCatCode.StartsWith("-1-42-74-"))
                            mTunnelList.Add(sItem);
                        else if (projectCatCode.StartsWith("-1-43-33-"))
                            mStationList.Add(sItem);
                        else
                            mOtherList.Add(sItem);
                    }//(string)dataReader["UpdateTime"],
                    catch (System.Exception ex)
                    {
                        Console.WriteLine((string)dataReader["ProjectName"] + " invalid");
                    }
                }
                //System.Console.WriteLine("readok");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                //dataReader.Close();
                conn.Close();
            }

            #endregion
        }


        public void createProjectTree()
        {

            CTEProject.LoadTEProFromDB();
            CTEProject.LoadTEProModel();
            CTEPier.LoadPiersFromDB();
            CTEPier.LoadPiersModels();

            createLabelWithProject();
            //createKML();
            createNavTrain();
            //createVideo();
            //createProjectProgress();
            
            createStation();
        }

        private void createVideo()
        {
            var sgworld = new SGWorld66();
            //var branch = sgworld.ProjectTree.FindItem("站房工程");
            double xs, ys, zs;
            //double[] p = new double[] { -1, 1, 0, 1, 1, 0, 1, -1, 0, -1, -1, 0 };
            IPosition66 cp;
            ITerrainVideo66 iVideo;
            int count = 0;

            var branch = sgworld.ProjectTree.CreateGroup("CCTV");
            //var vb = sgworld.ProjectTree.CreateGroup("Video");
            try
            {
                foreach (CRailwayProject cVideo in CRailwayScene.mOtherList)
                {
                    xs = cVideo.mLongitude_Mid;
                    ys = cVideo.mLatitude_Mid;
                    zs = 250;
                    cp = sgworld.Creator.CreatePosition(xs, ys, zs, AltitudeTypeCode.ATC_TERRAIN_RELATIVE,0,-80,0,2000);

                    iVideo = sgworld.Creator.CreateVideoOnTerrain(@"D:\GISData\Common\Textures\test.png", cp, branch, cVideo.mProjectName);
                    //iVideo.PlayVideoOnStartup = false;
                    iVideo.ProjectionFieldOfView = 45;
                    if (count < 2)
                    {
                        iVideo.VideoFileName = @"D:\GISData\Common\Textures\x1.avi";
                        count++;
                    }
                    
                  //  iVideo.pl



                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                Console.WriteLine("CCTV Creation Error");
            }
        }

        private void createNavTrain()
        {
            var sgworld = new SGWorld66();
            string fileName = GlobalVar.gDataPath + @"Common\Models\train.3ds";

            //mTrain = sgworld.Creator.CreateModel(sgworld.Creator.CreatePosition(),fileName,0.25,ModelTypeCode.MT_NORMAL,string.Empty,"Train");
            //mTrain.Position.X = 50;
            

            mDynamicTrain = sgworld.Creator.CreateDynamicObject(0, DynamicMotionStyle.MOTION_MANUAL, DynamicObjectType.DYNAMIC_3D_MODEL,
                fileName, 0.25, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, string.Empty, "Train");
            for (int i = 6000; i <36000 ; i+=5) //CRailwayScene.mMiddleLine.mPointNum
            {
                mDynamicTrain.Waypoints.AddWaypoint(sgworld.Creator.CreateRouteWaypoint(
                    CRailwayScene.mMiddleLine.longitude[i],
                    CRailwayScene.mMiddleLine.latitude[i],
                    CRailwayScene.mMiddleLine.altitude[i] ,
                    300, CRailwayScene.mMiddleLine.heading[i]));
                //Console.WriteLine(CRailwayScene.mMiddleLine.longitude[i] + "\t" + CRailwayScene.mMiddleLine.latitude[i]);
            }
            mDynamicTrain.PlayRouteOnStartup = false;
            //mDynamicTrain.Pause = true;
            //mDynamicTrain.s
            //mDynamicTrain.


        }

        private void createLabelWithProject()
        {
            string[] sProjType = new string[] { "路基", "涵洞", "特大桥", "大桥", "中桥", "框构桥", "空心板桥梁", "预制", "铺架" };//, "大临", "站房工程"
            var sgworld = new SGWorld66();
            IColor66 bgcolor = sgworld.Creator.CreateColor(255, 255, 255, 155);
            IColor66 forecolor = sgworld.Creator.CreateColor(0, 0, 0, 255);
            ILabelStyle66 cLabelStyle = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
            {
                //uint nBGRValue = 0x00000000;  // White               
                //var cBackgroundColor = cLabelStyle.BackgroundColor; // Get label style background color
                //cBackgroundColor.FromBGRColor(nBGRValue);               // Set background to blue
                //cBackgroundColor.SetAlpha(0.8);                      // Set transparency to 50%
                cLabelStyle.BackgroundColor = bgcolor;         // Set label style background color
                cLabelStyle.TextColor = forecolor;
                cLabelStyle.FontName = "Arial";                         // Set font name to Arial
                cLabelStyle.Italic = true;                              // Set label style font to italic
                cLabelStyle.Scale = 3;                                  // Set label style scale
                cLabelStyle.TextOnImage = false;
            }
            var branch = sgworld.ProjectTree.CreateGroup("桥梁");
            foreach (string str in sProjType)
            {
                createOneProjectBranch(str, cLabelStyle);
            }


        }


        private void createOneProjectBranch(string projType, ILabelStyle66 cLabelStyle)
        {
            //SKRailwayItem item;
            var sgworld = new SGWorld66();
            //ITerraExplorerMessage66 cMessage = null;

            var parentID = sgworld.ProjectTree.FindItem("桥梁");
            if (!(projType.Equals("特大桥") || projType.Equals("中桥") || projType.Equals("大桥") || projType.Equals("框构桥")))
                parentID = string.Empty;
            var branch = sgworld.ProjectTree.CreateGroup(projType, parentID);

            foreach (CRailwayProject item in CRailwayScene.mProjectList)
            {
                //    item = (SKRailwayItem)CRailwayScene.mProjectList.mProjectList.ElementAt(j);
                if (item.mProfessionalName.Equals(projType, StringComparison.InvariantCultureIgnoreCase))
                {
                    var cPos = sgworld.Creator.CreatePosition(item.mLongitude_Mid, item.mLatitude_Mid, 10, AltitudeTypeCode.ATC_TERRAIN_RELATIVE);
                    item.mLabel = sgworld.Creator.CreateTextLabel(cPos, item.mProjectName, cLabelStyle, branch, item.mProjectName);
                    item.mLabel.ImageFileName = item.mLabelImage;
                    //string urlStr = "http://" + GlobalVar.gSTAT_SERVERIP + "/MNMIS/APP/ProjectMng.aspx?f=detail&sn=" + item.mSerialNo +
                    //    "&uacc=" + GlobalVar.gSTAT_USERACC + "&upwd=" + GlobalVar.gSTAT_USRPWD;
                    //cMessage = sgworld.Creator.CreateMessage(MsgTargetPosition.MTP_POPUP, urlStr, MsgType.TYPE_URL, true);
                    ////cMessage.Height = 100;
                    ////MsgTargetPosition mtp = 
                    //item.mLabel.Message.MessageID = cMessage.ID;

                }


            }

        }

        private void createStation()
        {
            var sgworld = new SGWorld66();
            //var branch = sgworld.ProjectTree.FindItem("站房工程");
            double xs, ys, zs, ds;
            double[] p = new double[] { -1, 1, 0, 1, 1, 0, 1, -1, 0, -1, -1, 0 };
            IPosition66 cp;
            ITerrainImageLabel66 iLabel;
            //double t[10];

            ILabelStyle66 cLabelStyle = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
            {
                //cLabelStyle.BackgroundColor = bgcolor;         // Set label style background color
                //cLabelStyle.TextColor = forecolor;
                cLabelStyle.FontName = "Arial";                         // Set font name to Arial
                cLabelStyle.Italic = true;                              // Set label style font to italic
                cLabelStyle.Scale = 100;                                  // Set label style scale
                //cLabelStyle.TextOnImage = false;
            }

            var branch = sgworld.ProjectTree.CreateGroup("站房工程");
            try
            {
                foreach (CRailwayProject station in CRailwayScene.mStationList)
                {
                    //    item = (SKRailwayItem)CRailwayScene.mProjectList.mProjectList.ElementAt(j);
                    if (station.mProfessionalName.Equals("站房工程", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //x = station.mLongitude_Mid;
                        //y = station.mLatitude_Mid;
                        //z = 10;
                        CRailwayScene.mMiddleLine.findPosbyMeter(station.mMileage_Mid - 50, out xs, out ys, out zs, out ds);
                        CoordinateConverter.LatLonOffest(ys, xs, ds, station.mDirection, 25, out p[1], out p[0]);  //lat, lon
                        CoordinateConverter.LatLonOffest(ys, xs, ds, station.mDirection, 75, out p[4], out p[3]);
                        CRailwayScene.mMiddleLine.findPosbyMeter(station.mMileage_Mid + 50, out xs, out ys, out zs, out ds);
                        CoordinateConverter.LatLonOffest(ys, xs, ds, station.mDirection, 25, out p[10], out p[9]);
                        CoordinateConverter.LatLonOffest(ys, xs, ds, station.mDirection, 75, out p[7], out p[6]);

                        cp = sgworld.Creator.CreatePosition((p[0] + p[3] + p[6] + p[9])/4,(p[1] + p[4] + p[7] + p[10])/4,40,AltitudeTypeCode.ATC_TERRAIN_RELATIVE);
                        
                        iLabel = sgworld.Creator.CreateImageLabel(cp, station.mLabelImage,cLabelStyle, branch, station.mProjectName);
                        iLabel.Message.MessageID = sgworld.Creator.CreateMessage(MsgTargetPosition.MTP_POPUP, station.mProjectName, MsgType.TYPE_TEXT, true).ID;
                        //iLabel.Visibility.MaxVisibilityDistance = 1000000;
                        
                        IGeometry buildingShp = null;
                        ILinearRing cSQRing = sgworld.Creator.GeometryCreator.CreateLinearRingGeometry(p);
                        buildingShp = sgworld.Creator.GeometryCreator.CreatePolygonGeometry(cSQRing, null);
                        var model = sgworld.Creator.CreateBuilding(buildingShp, 15, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, branch, station.mProjectName);
                        //fileName = GlobalVar.gDataPath + @"\Data\Textures\Sky_w_tan.jpg";
                        model.Sides.Texture.FileName = GlobalVar.gDataPath + @"Common\Textures\Sky_w_tan.jpg";
                        model.Roof.Texture.FileName = GlobalVar.gDataPath + @"Common\Textures\roof2.jpg";

                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                Console.WriteLine("Station Creation Error");
            }

        }

        private void createKML()
        {
            var sgworld = new SGWorld66();
            var branch = sgworld.ProjectTree.CreateGroup("KML");
            ILabelStyle66 cLabelStyle = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);

            {
                uint nBGRValue = 0xFF0000;  // Blue
                double dAlpha = 0.5;        // 50% opacity
                var cBackgroundColor = cLabelStyle.BackgroundColor; // Get label style background color
                cBackgroundColor.FromBGRColor(nBGRValue);               // Set background to blue
                cBackgroundColor.SetAlpha(dAlpha);                      // Set transparency to 50%
                cLabelStyle.BackgroundColor = cBackgroundColor;         // Set label style background color
                cLabelStyle.FontName = "Arial";                         // Set font name to Arial
                cLabelStyle.Italic = true;                              // Set label style font to italic
                cLabelStyle.Scale = 3;                                  // Set label style scale
                cLabelStyle.TextOnImage = false;
            }
            for (int i = 0; i < CRailwayScene.mMiddleLine.mPointNum; i += 100)
            {
                sgworld.Creator.CreateTextLabel(CRailwayScene.mMiddleLine.findPositionbyIndex(i),
                    (int)CRailwayScene.mMiddleLine.meter[i] + "", cLabelStyle, branch, (int)CRailwayScene.mMiddleLine.meter[i] + "");
            }

        }

        //This method converts the values to RGB
        private void HslToRgb(int Hue, double Saturation, double Lightness, out int r, out int g, out int b)
        {
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            double num = ((double)Hue) % 360.0;
            double num2 = Saturation ;
            double num3 = Lightness;
            if (num2 == 0.0)
            {
                num4 = num3;
                num5 = num3;
                num6 = num3;
            }
            else
            {
                double d = num / 60.0;
                int num11 = (int)Math.Floor(d);
                double num10 = d - num11;
                double num7 = num3 * (1.0 - num2);
                double num8 = num3 * (1.0 - (num2 * num10));
                double num9 = num3 * (1.0 - (num2 * (1.0 - num10)));
                switch (num11)
                {
                    case 0:
                        num4 = num3;
                        num5 = num9;
                        num6 = num7;
                        break;
                    case 1:
                        num4 = num8;
                        num5 = num3;
                        num6 = num7;
                        break;
                    case 2:
                        num4 = num7;
                        num5 = num3;
                        num6 = num9;
                        break;
                    case 3:
                        num4 = num7;
                        num5 = num8;
                        num6 = num3;
                        break;
                    case 4:
                        num4 = num9;
                        num5 = num7;
                        num6 = num3;
                        break;
                    case 5:
                        num4 = num3;
                        num5 = num7;
                        num6 = num8;
                        break;
                }
            }
            r =(int)( num4 * 255);
            g = (int)(num5 * 255);
            b = (int)(num6 * 255);

            //return (uint)((num4 * 255.0)* 65536 + (num5 * 255.0) * 256 + (num6 * 255.0) )* 256+ 255;
        }

        private void showProjectProgress(CRailwayProject item)
        {
            if ( item.mMileage_End - item.mMileage_Start > 15)
            {
                    double[] x;
                    double[] y;
                    double[] z;
                    int pointNum = CRailwayScene.mMiddleLine.getSubLine(item.mMileage_Start , item.mMileage_End , 1, out x, out y, out z);
                    if (pointNum > 1)
                    {
                        double[] cVerticesArray = new double[pointNum * 3];

                        int i;
                        for (i = 0; i < pointNum; i++)
                        {
                            cVerticesArray[3 * i] = x[i];
                            cVerticesArray[3 * i + 1] = y[i];
                            cVerticesArray[3 * i + 2] = 20;

                        }
                        try
                        {
                            var sgworld = new SGWorld66();
                            var branch = sgworld.ProjectTree.FindItem("MiddleLine");
                            int r, g, b;
                            HslToRgb(10, 1-item.mAvgProgress, 1, out r, out g, out b);
                            
                            item.mPolyline = sgworld.Creator.CreatePolylineFromArray(cVerticesArray, sgworld.Creator.CreateColor(r,g,b,255), 
                                AltitudeTypeCode.ATC_TERRAIN_RELATIVE, branch, "Middle Line" + item.mProjectName);
                            item.mPolyline.Visibility.MinVisibilityDistance = 1250;
                            item.mPolyline.Spline = true;
                            item.mPolyline.LineStyle.Width = -5;
                            //item.mPolyline.

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Creation Failed" + item.mProjectName);
                        }

                    }
             }
        }

        //FIXME Some Projects conflict with NavTrain
        private void createProjectProgress()
        {

            var sgworld = new SGWorld66();
            var branch = sgworld.ProjectTree.CreateGroup("MiddleLine"); 
            //Console.WriteLine(CRailwayScene.mProjectList.Count + "");
            foreach (CRailwayProject item in CRailwayScene.mBridgeList)
                showProjectProgress(item);
            foreach (CRailwayProject item in CRailwayScene.mRoadList)
                showProjectProgress(item);
            foreach (CRailwayProject item in CRailwayScene.mTunnelList)
                showProjectProgress(item);
        }
    }
}
