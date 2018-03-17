using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    /// <summary>
    /// 工点信息可视化，目前仅仅是语法正确了，逻辑很多待修正，例如不支持多链。
    /// </summary>
    public class CTEProject: CTERWItem
    {
        
        public CTEProject(CRailwayScene s, CTEScene ss, CRWTEStandard t)
            : base(s, ss, t)
        {

        }

        public override void TECreate()
        {
            createProjectLabel();
            //createProjectLineAndLabel();
            //createTest();
            //createMiddles();
            createCircle();
            //createRibbon();
        }

        public override void TEUpdate()
        {
            var sgworld = new SGWorld66();
            //var branch = sgworld.ProjectTree.FindItem(groupID);
            //if (!string.IsNullOrEmpty(branch))
            //    sgworld.ProjectTree.DeleteItem(branch);
            sgworld.ProjectTree.DeleteItem(mGroupIDDynamic);
            mGroupIDDynamic = null; 
            createCircle();
            createRibbon();
        }



        private void polylineMiddleLine(CRailwayProject rw, SGWorld66 sgworld, string branch, Dictionary<string,double> m)
        {
            ITerrainPolyline66 polyline1;
            ITerrainPolyline66 polyline2;
            ITerrainPolyline66 polyline3;
            ITerrainPolyline66 polyline4;
            IColor66 lineColor1 = sgworld.Creator.CreateColor(242, 174, 17, 255);
            IColor66 lineColor2 = sgworld.Creator.CreateColor(32, 183, 81, 255);
            IColor66 lineColor3 = sgworld.Creator.CreateColor(183, 9, 9, 255);
            IColor66 lineColor4 = sgworld.Creator.CreateColor(50, 50, 50, 255);
            double[] x, y, z, dir;
            int count = mSceneData.mMiddleLines.getSubLineByDKCode(rw.Mileage_Start_Discription, rw.Mileage_End_Discription, 10, out x, out y, out z, out dir);
            double[] mArray1 = new double[count * 3];
            double[] mArray2 = new double[count * 3];
            double[] mArray3 = new double[count * 3];
            double[] mArray4 = new double[count * 3];
            for (int i = 0; i < count; i++)
            {
                mArray1[3 * i] = x[i];
                mArray1[3 * i + 1] = y[i];
                mArray1[3 * i + 2] = z[i] + 10;

                mArray2[3 * i] = x[i];
                mArray2[3 * i + 1] = y[i];
                mArray2[3 * i + 2] = z[i] + 20;

                mArray3[3 * i] = x[i];
                mArray3[3 * i + 1] = y[i];
                mArray3[3 * i + 2] = z[i] + 30;

                mArray4[3 * i] = x[i];
                mArray4[3 * i + 1] = y[i];
                mArray4[3 * i + 2] = z[i] + 40;
            }
            if (m.Count >= 4)
            {
                KeyValuePair<string,double>kvp = m.ElementAt(0); 
                polyline1 = sgworld.Creator.CreatePolylineFromArray(mArray1, lineColor1, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, branch, rw.ProjectName + "-" + kvp.Key);
                polyline1.Spline = true;
                polyline1.Visibility.MaxVisibilityDistance = 10000000;
                polyline1.Visibility.MinVisibilityDistance = 150;
                polyline1.LineStyle.Width = 50.0;
                polyline1.Visibility.Show = true;
                polyline1.LineStyle.Color.SetAlpha(kvp.Value);

kvp = m.ElementAt(1);
                polyline2 = sgworld.Creator.CreatePolylineFromArray(mArray2, lineColor2, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, branch, rw.ProjectName + "-" + kvp.Key);
                polyline2.Spline = true;
                polyline2.Visibility.MaxVisibilityDistance = 10000000;
                polyline2.Visibility.MinVisibilityDistance = 150;
                polyline2.LineStyle.Width = 30.0;
                polyline2.Visibility.Show = true;
                polyline2.LineStyle.Color.SetAlpha(kvp.Value);
                kvp = m.ElementAt(2);
                polyline3 = sgworld.Creator.CreatePolylineFromArray(mArray3, lineColor3, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, branch, rw.ProjectName + "-" + kvp.Key);
                polyline3.Spline = true;
                polyline3.Visibility.MaxVisibilityDistance = 10000000;
                polyline3.Visibility.MinVisibilityDistance = 150;
                polyline3.LineStyle.Width = 15.0;
                polyline3.Visibility.Show = true;
                polyline3.LineStyle.Color.SetAlpha(kvp.Value);
                kvp = m.ElementAt(3);
                polyline4 = sgworld.Creator.CreatePolylineFromArray(mArray4, lineColor4, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, branch, rw.ProjectName + "-" + kvp.Key);
                polyline4.Spline = true;
                polyline4.Visibility.MaxVisibilityDistance = 10000000;
                polyline4.Visibility.MinVisibilityDistance = 150;
                polyline4.LineStyle.Width = 5;
                polyline4.Visibility.Show = true;
                polyline4.LineStyle.Color.SetAlpha(kvp.Value);
            }
        }

        /// <summary>
        /// 测试铁路链
        /// </summary>
        private void createMiddles()
        {
            var sgworld = new SGWorld66();
            var branch = sgworld.ProjectTree.CreateGroup("进度");
            ITerrainPolyline66 polyline;
            ILabelStyle66 cLabelStyle = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
            {
                cLabelStyle.FontName = "Arial";                         // Set font name to Arial
                cLabelStyle.Italic = true;                              // Set label style font to italic
                cLabelStyle.Scale = 3;                                  // Set label style scale
                cLabelStyle.TextOnImage = false;
            }
            //CRailwayProject item = mSceneData.mProjectList[0];
            foreach (CRailwayLine item in mSceneData.mMiddleLines.mLineList)
            {
            double[] x;
            double[] y;
            double[] z;
            double[] d;
            //CRailwayLineList oo=null;
            int pointNum = item.getSubLine(item.mStart,item.mEnd, 1, out x, out y, out z, out d);
            if (pointNum > 0)
            {
                double[] cVerticesArray = new double[pointNum * 3];

                for (int i = 0; i < pointNum; i++)
                {
                    cVerticesArray[3 * i] = x[i];
                    cVerticesArray[3 * i + 1] = y[i];
                    cVerticesArray[3 * i + 2] = z[i] + 10;
                }
            

                    try
                    {

                        polyline = sgworld.Creator.CreatePolylineFromArray(cVerticesArray, sgworld.Creator.CreateColor(255, 215, 0, 255),
                               AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, branch, item.mDKCode + "里程");
                        //polyline.Visibility.MinVisibilityDistance = 1250;
                        //polyline.Spline = true;
                        //polyline.LineStyle.Width = -5;
                        polyline.Visibility.MaxVisibilityDistance = 10000000;
                        polyline.LineStyle.Width = -3.0;
                        polyline.Visibility.Show = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Creation Failed" + item.mDKCode);
                    }
                }
            }
        }

        private void createRibbon()
        {
            var sgworld = new SGWorld66();
            if (string.IsNullOrEmpty(mGroupIDDynamic))
                mGroupIDDynamic = sgworld.ProjectTree.CreateGroup("ProjectProgress");

            foreach (CRailwayProject item in mSceneData.mBridgeList) // FIXME xyn
            {
                double[] p = new double[20];       // 工点分项进度，色带透明度
                String[] FXName = new String[20];  // 工点分项名字
                Dictionary<string,double> mFXProj = item.getFXProgressByDate(mTEScene.toDate);
                //if (mFXProj.Count != 0)
                //{
                //    Dictionary<string, double>.ValueCollection v0 = mFXProj[0].progress.Values;
                //    FXName[0] = mFXProj[0].fxName;
                //    if (v0.Count > 0)
                //    {
                //        p[0] = v0.First<double>();
                //    }
                //    else { p[0] = 0; }
                //}
                //else { p[0] = 0; }
                //if (mFXProj.Count >= 2)
                //{
                //    Dictionary<string, double>.ValueCollection v1 = mFXProj[1].progress.Values;
                //    FXName[1] = mFXProj[1].fxName;
                //    if (v1.Count > 0)
                //    {
                //        p[1] = v1.First<double>();
                //    }
                //    else { p[1] = 0; }
                //}
                //else { p[1] = 0; }
                //if (mFXProj.Count >= 4)
                //{
                //    Dictionary<string, double>.ValueCollection v3 = mFXProj[3].progress.Values;
                //    FXName[3] = mFXProj[3].fxName;
                //    if (v3.Count > 0)
                //    {
                //        p[3] = v3.First<double>();
                //    }
                //    else { p[3] = 0; }
                //}
                //else { p[3] = 0; }
                //if (mFXProj.Count >= 7)
                //{
                //    Dictionary<string, double>.ValueCollection v6 = mFXProj[6].progress.Values;
                //    FXName[6] = mFXProj[6].fxName;
                //    if (v6.Count > 0)
                //    {
                //        p[6] = v6.First<double>();
                //    }
                //    else { p[6] = 0; }
                //}
                //else { p[6] = 0; }
                polylineMiddleLine(item, sgworld, mGroupIDDynamic, mFXProj);
            }
        }
        /// <summary>
        /// 测试进度circle
        /// </summary>
        private void createCircle()
        {
            var sgworld = new SGWorld66();
            if (string.IsNullOrEmpty(mGroupIDDynamic))
                mGroupIDDynamic = sgworld.ProjectTree.CreateGroup("ProjectProgress");
            ITerrainPolyline66 polyline;
           
            //ILabelStyle66 cLabelStyle = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
            //{
            //    cLabelStyle.FontName = "Arial";                         // Set font name to Arial
            //    cLabelStyle.Italic = true;                              // Set label style font to italic
            //    cLabelStyle.Scale = 3;                                  // Set label style scale
            //    cLabelStyle.TextOnImage = false;
            //}
            //CRailwayProject item = mSceneData.mProjectList[0];
            foreach (CRailwayProject item in mSceneData.mBridgeList) // FIXME xyn
            {
                //if (item.mMileage_End - item.mMileage_Start > 15)
                //{
                item.getFXProgressByDate(mTEScene.toDate);
                double[] cVerticesArray=null;

                //double latcen;
                //double loncen;
                double lat;
                double lon;
                double z;
                double dir;
                //int zone;
                //double x, y, xd, yd;
                double cx, cy;
                double globalmeter;
                //bool isValue = mSceneData.mMiddleLines.getPosbyMeter(item.Mileage_Start_Ds, out lat, out lon, out z, out dir);
                //CoordinateConverter.LatLonOffest(lat, lon, z, 270, 300, out latcen, out loncen);
                //CoordinateConverter.LatLonToUTMXY(latcen, loncen, out x, out y, out zone);


                //int ccount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(item.AvgProgress * 180.0)));

                if (!mSceneData.mMiddleLines.getPosbyDKCode(item.DKCode,item.CenterMileage,out lon, out lat, out z, out dir )){
                    Console.WriteLine(item.ProjectName + " 工点 坐标错误");
                    continue;
                }
                CoordinateConverter.LatLonOffest(lat, lon, dir, 90 , 150, out cy, out cx);
                int angle = (int)(item.AvgProgress * 360);
                int count = 0;
                cVerticesArray = CTERWItem.TEDrawProgressCircle(cx, cy, z, 75, angle, out count);
                //int ccount = 140;


                //if (isValue)
                //{
                //    cVerticesArray = new double[2*ccount*3];
                //    for (int i = 0; i < ccount; i++)
                //    {
                //        xd = x + 40 * Math.Cos(2 * i * Math.PI / 180);
                //        yd = y + 109 * Math.Sin(2 * i * Math.PI / 180);
                //        CoordinateConverter.UTMXYToLatLon(xd, yd, out cVerticesArray[3 * i], out cVerticesArray[3 * i + 1], zone, latcen < 0);
                //        cVerticesArray[3 * i + 2] = z + 10;
                //    }
                //    for (int i = ccount; i < 2 * ccount; i++)
                //    {
                //        xd = x + 0.4 * 40 * Math.Cos(2 * (2 * ccount - i) * Math.PI / 180);
                //        yd = y + 0.4 * 109 * Math.Sin(2 * (2 * ccount - i) * Math.PI / 180);
                //        CoordinateConverter.UTMXYToLatLon(xd, yd, out cVerticesArray[3 * i], out cVerticesArray[3 * i + 1], zone, latcen < 0);
                //        cVerticesArray[3 * i + 2] = z + 10;
                //    }
                //}

                try
                {
                    //sgworld.Creator.createpol
                    //polyline = sgworld.Creator.CreatePolygonFromArray(cVerticesArray, sgworld.Creator.CreateColor(255, 215, 0, 255), sgworld.Creator.CreateColor(255, 215, 0, 255),
                           //AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, mTEID, item.ProjectName + "工点");
                    //polyline.Visibility.MinVisibilityDistance = 1250;
                    //polyline.Spline = true;
                    //polyline.LineStyle.Width = -5;
                    polyline = sgworld.Creator.CreatePolylineFromArray(cVerticesArray, sgworld.Creator.CreateColor(255, 215, 0, 255), AltitudeTypeCode.ATC_TERRAIN_RELATIVE, mGroupIDDynamic, "工点" + item.ProjectName);
                    polyline.Visibility.MaxVisibilityDistance = 10000000;
                    polyline.FillStyle.Color = sgworld.Creator.CreateColor(255, 215, 0, 255);
                    polyline.LineStyle.Width = 130.0;
                    polyline.Visibility.Show = true;
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Creation Failed" + item.ProjectName);
                }

                //double[] inArray = new double[180 * 3]; 
                //double[] outArray = new double[180 * 3];
                //for (int i = 0; i < 180; i++)
                //{
                //    xd = x + 0.4 * 40 * Math.Cos(2 * i * Math.PI / 180);
                //    yd = y + 0.4 * 109 * Math.Sin(2 * i * Math.PI / 180);
                //    CoordinateConverter.UTMXYToLatLon(xd, yd, out inArray[3 * i], out inArray[3 * i + 1], zone, latcen < 0);
                //    inArray[3 * i + 2] = z + 10;
                //}
                //for (int i = 0; i < 180; i++)
                //{
                //    xd = x + 40 * Math.Cos(2 * i * Math.PI / 180);
                //    yd = y + 109 * Math.Sin(2 * i * Math.PI / 180);
                //    CoordinateConverter.UTMXYToLatLon(xd, yd, out outArray[3 * i], out outArray[3 * i + 1], zone, latcen < 0);
                //    outArray[3 * i + 2] = z + 10;
                //}
                //sgworld.Creator.CreatePolylineFromArray(inArray, sgworld.Creator.CreateColor(255, 215, 0, 255),
                //       AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);
                //sgworld.Creator.CreatePolylineFromArray(outArray, sgworld.Creator.CreateColor(255, 215, 0, 255),
                //   AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);

                var cPos = sgworld.Creator.CreatePosition(cx, cy, 30, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -75, 0, 2000);
                sgworld.Creator.CreateTextLabel(cPos, Math.Round((item.AvgProgress * 100),2).ToString() + "%", mTEStandard.mLabelStyleL3, mGroupIDDynamic, "进度" + item.ProjectName);
            }
        }


        /// <summary>
        /// 添加桥梁、路基、隧道模型与标签，FIXME目前模型与工点表ProjectInof不一致（有些工点没有模型，有些工点应该对应多个模型），模型生成程序有一个单独的Excel表TEProj，不通过程序添加
        /// </summary>
        /// <param name="groupID"></param>
        private void createProjectLabel()
        {
            var sgworld = new SGWorld66();
            if (string.IsNullOrEmpty(mGroupIDStatic))
                mGroupIDStatic = sgworld.ProjectTree.CreateGroup("Project");
            double lat, lon, z, dir;
            double cx, cy;

            //IColor66 bgcolor = sgworld.Creator.CreateColor(255, 255, 255, 155);
            //IColor66 forecolor = sgworld.Creator.CreateColor(0, 0, 0, 255);
            ILabelStyle66 cLabelStyle = mTEStandard.mLabelStyleL3;
                //sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
            //{
            //    //uint nBGRValue = 0x00000000;  // White               
            //    //var cBackgroundColor = cLabelStyle.BackgroundColor; // Get label style background color
            //    //cBackgroundColor.FromBGRColor(nBGRValue);               // Set background to blue
            //    //cBackgroundColor.SetAlpha(0.8);                      // Set transparency to 50%
            //    cLabelStyle.BackgroundColor = bgcolor;         // Set label style background color
            //    cLabelStyle.TextColor = forecolor;
            //    cLabelStyle.FontName = "Arial";                         // Set font name to Arial
            //    cLabelStyle.Italic = true;                              // Set label style font to italic
            //    cLabelStyle.Scale = 3;                                  // Set label style scale
            //    cLabelStyle.TextOnImage = false;
            //}
            foreach (CRailwayProject item in mSceneData.mBridgeList)
            {
 
                var cPos = sgworld.Creator.CreatePosition(item.CenterLongitude, item.CenterLatitude, 10, AltitudeTypeCode.ATC_TERRAIN_RELATIVE);
                sgworld.Creator.CreateLabel(cPos, item.ProjectName, CGisDataSettings.gDataPath + @"\Data\Textures\桥梁.png", cLabelStyle, mGroupIDStatic, item.ProjectName);

                if (!mSceneData.mMiddleLines.getPosbyDKCode(item.DKCode,item.CenterMileage,out lon, out lat, out z, out dir ))
                {
                    //Console.WriteLine(item.ProjectName + " 工点 坐标错误");
                    continue;
                }
                CoordinateConverter.LatLonOffest(lat, lon, dir, 90, 150, out cy, out cx);
                var cPos2 = sgworld.Creator.CreatePosition(cx, cy, 30, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -75, 0, 2000);
                sgworld.Creator.CreateCircle(cPos2, 140, sgworld.Creator.CreateColor(127, 127, 127, 127), sgworld.Creator.CreateColor(127, 127, 127, 127), mGroupIDStatic);

            }

            foreach (CRailwayProject item in mSceneData.mRoadList)
            {

                var cPos = sgworld.Creator.CreatePosition(item.CenterLongitude, item.CenterLatitude, 10, AltitudeTypeCode.ATC_TERRAIN_RELATIVE);
                sgworld.Creator.CreateLabel(cPos, item.ProjectName, CGisDataSettings.gDataPath + @"\Data\Textures\路基.png", cLabelStyle, mGroupIDStatic, item.ProjectName);
                if (!mSceneData.mMiddleLines.getPosbyDKCode(item.DKCode, item.CenterMileage, out lon, out lat, out z, out dir))
                {
                    //Console.WriteLine(item.ProjectName + " 工点 坐标错误");
                    continue;
                }
                CoordinateConverter.LatLonOffest(lat, lon, dir, 90, 150, out cy, out cx);
                var cPos2 = sgworld.Creator.CreatePosition(cx, cy, 30, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -75, 0, 2000);
                sgworld.Creator.CreateCircle(cPos2, 140, sgworld.Creator.CreateColor(127, 127, 127, 127), sgworld.Creator.CreateColor(127, 127, 127, 127), mGroupIDStatic);

            }

            foreach (CRailwayProject item in mSceneData.mTunnelList)
            {
                var cPos = sgworld.Creator.CreatePosition(item.CenterLongitude, item.CenterLatitude, 10, AltitudeTypeCode.ATC_TERRAIN_RELATIVE);
                sgworld.Creator.CreateLabel(cPos, item.ProjectName, CGisDataSettings.gDataPath + @"\Data\Textures\涵洞.png", cLabelStyle, mGroupIDStatic, item.ProjectName);
                if (!mSceneData.mMiddleLines.getPosbyDKCode(item.DKCode, item.CenterMileage, out lon, out lat, out z, out dir))
                {
                    //Console.WriteLine(item.ProjectName + " 工点 坐标错误");
                    continue;
                }
                CoordinateConverter.LatLonOffest(lat, lon, dir, 90, 150, out cy, out cx);
                var cPos2 = sgworld.Creator.CreatePosition(cx, cy, 30, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -75, 0, 2000);
                sgworld.Creator.CreateCircle(cPos2, 140, sgworld.Creator.CreateColor(127, 127, 127, 127), sgworld.Creator.CreateColor(127, 127, 127, 127), mGroupIDStatic);
            }



            //bool isValue = mSceneData.mMiddleLines.getPosbyMeter(item.Mileage_Start_Ds, out lat, out lon, out z, out dir);
            //CoordinateConverter.LatLonOffest(lat, lon, z, 270, 300, out latcen, out loncen);
            //CoordinateConverter.LatLonToUTMXY(latcen, loncen, out x, out y, out zone);


            //int ccount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(item.AvgProgress * 180.0)));


        }

        #region No In Use Now  xyn

        //private void createProjectLineAndLabel()
        //{
        //    var sgworld = new SGWorld66();
        //    var branch = sgworld.ProjectTree.CreateGroup("里程");
        //    ITerrainPolyline66 polyline;
        //    ILabelStyle66 cLabelStyle = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
        //    {
        //        cLabelStyle.FontName = "Arial";                         // Set font name to Arial
        //        cLabelStyle.Italic = true;                              // Set label style font to italic
        //        cLabelStyle.Scale = 3;                                  // Set label style scale
        //        cLabelStyle.TextOnImage = false;
        //    }
        //    CRailwayProject item = mSceneData.mProjectList[0];
        //    //foreach (CRailwayProject item0 in mSceneData.mProjectList)
        //    //{
        //    //if (item.mMileage_End - item.mMileage_Start > 15)
        //    //{
        //    double[] cVerticesArray;

        //    int pointNum = item.getSubLine(1, out cVerticesArray);//三维中线
        //    //int pointNum = item.getOffsetLine(1, 90, 1000, out cVerticesArray); //偏移

        //    try
        //    {

        //        polyline = sgworld.Creator.CreatePolylineFromArray(cVerticesArray, sgworld.Creator.CreateColor(0, 0, 0, 255),
        //               AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, branch, item.ProjectName + "里程");
        //        //polyline.Visibility.MinVisibilityDistance = 1250;
        //        //polyline.Spline = true;
        //        //polyline.LineStyle.Width = -5;
        //        polyline.Visibility.MaxVisibilityDistance = 10000000;
        //        polyline.LineStyle.Width = -3.0;
        //        polyline.Visibility.Show = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Creation Failed" + item.ProjectName);
        //    }



        //    branch = sgworld.ProjectTree.CreateGroup("工点");
        //    foreach (CRailwayProject item0 in mSceneData.mProjectList)
        //    {
        //        var cPos = sgworld.Creator.CreatePosition(item0.mLongitude_Mid, item0.mLatitude_Mid, 10, AltitudeTypeCode.ATC_TERRAIN_RELATIVE);
        //        sgworld.Creator.CreateTextLabel(cPos, item0.ProjectName, cLabelStyle, branch, item0.ProjectName);
        //        //item.mLabel = sgworld.Creator.CreateTextLabel(cPos, item.ProjectName, cLabelStyle, branch, item.ProjectName);
        //        //item.mLabel.ImageFileName = item.mLabelImage;
        //    }
        //}

        //private void createProjectLabelNotUse()
        //{
        //    string[] sProjType = new string[] { "路基", "涵洞", "特大桥", "大桥", "中桥", "框构桥", "空心板桥梁", "预制", "铺架" };//, "大临", "站房工程"
        //    var sgworld = new SGWorld66();
        //    IColor66 bgcolor = sgworld.Creator.CreateColor(255, 255, 255, 155);
        //    IColor66 forecolor = sgworld.Creator.CreateColor(0, 0, 0, 255);
        //    ILabelStyle66 cLabelStyle = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
        //    {
        //        //uint nBGRValue = 0x00000000;  // White               
        //        //var cBackgroundColor = cLabelStyle.BackgroundColor; // Get label style background color
        //        //cBackgroundColor.FromBGRColor(nBGRValue);               // Set background to blue
        //        //cBackgroundColor.SetAlpha(0.8);                      // Set transparency to 50%
        //        cLabelStyle.BackgroundColor = bgcolor;         // Set label style background color
        //        cLabelStyle.TextColor = forecolor;
        //        cLabelStyle.FontName = "Arial";                         // Set font name to Arial
        //        cLabelStyle.Italic = true;                              // Set label style font to italic
        //        cLabelStyle.Scale = 3;                                  // Set label style scale
        //        cLabelStyle.TextOnImage = false;
        //    }
        //    var branch = sgworld.ProjectTree.CreateGroup("桥梁");
        //    foreach (string str in sProjType)
        //    {
        //        createOneProjectBranch(str, cLabelStyle);
        //    }


        //}
        //private void createOneProjectBranch(string projType, ILabelStyle66 cLabelStyle)
        //{
        //    //SKRailwayItem item;
        //    var sgworld = new SGWorld66();
        //    //ITerraExplorerMessage66 cMessage = null;

        //    var parentID = sgworld.ProjectTree.FindItem("桥梁");
        //    if (!(projType.Equals("特大桥") || projType.Equals("中桥") || projType.Equals("大桥") || projType.Equals("框构桥")))
        //        parentID = string.Empty;
        //    var branch = sgworld.ProjectTree.CreateGroup(projType, parentID);

        //    foreach (CRailwayProject item in mSceneData.mProjectList)
        //    {
        //        //    item = (SKRailwayItem)CRailwayScene.mProjectList.mProjectList.ElementAt(j);
        //        if (item.ProfessionalName.Equals(projType, StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            var cPos = sgworld.Creator.CreatePosition(item.mLongitude_Mid, item.mLatitude_Mid, 10, AltitudeTypeCode.ATC_TERRAIN_RELATIVE);
        //            sgworld.Creator.CreateTextLabel(cPos, item.ProjectName, cLabelStyle, branch, item.ProjectName);
        //         //   item.mLabel = sgworld.Creator.CreateTextLabel(cPos, item.ProjectName, cLabelStyle, branch, item.ProjectName);
        //            //item.mLabel.ImageFileName = item.mLabelImage;
        //            //string urlStr = "http://" + GlobalVar.gSTAT_SERVERIP + "/MNMIS/APP/ProjectMng.aspx?f=detail&sn=" + item.mSerialNo +
        //            //    "&uacc=" + GlobalVar.gSTAT_USERACC + "&upwd=" + GlobalVar.gSTAT_USRPWD;
        //            //cMessage = sgworld.Creator.CreateMessage(MsgTargetPosition.MTP_POPUP, urlStr, MsgType.TYPE_URL, true);
        //            ////cMessage.Height = 100;
        //            ////MsgTargetPosition mtp = 
        //            //item.mLabel.Message.MessageID = cMessage.ID;

        //        }


        //    }

        //}
        //This method converts the values to RGB
        private void HslToRgb(int Hue, double Saturation, double Lightness, out int r, out int g, out int b)
        {
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            double num = ((double)Hue) % 360.0;
            double num2 = Saturation;
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
            r = (int)(num4 * 255);
            g = (int)(num5 * 255);
            b = (int)(num6 * 255);

            //return (uint)((num4 * 255.0)* 65536 + (num5 * 255.0) * 256 + (num6 * 255.0) )* 256+ 255;
        }


        /// <summary>
        /// FIXME 目前只支持DK，且待调试修正
        /// </summary>
        private void createProjectProgress()
        {
            //double[] x;
            //double[] y;
            //double[] z;
            int pointNum = mSceneData.mMiddleLines.mLineList[1].mPointNum; //CRailwayScene.mMiddleLine.mPointNum;
            //x = new double[pointNum];
            //y = new double[pointNum];
            //z = new double[pointNum];
            if (pointNum > 1)
            {
                double[] cVerticesArray = new double[pointNum * 3];

                int i;
                for (i = 0; i < pointNum; i += 10)
                {
                    cVerticesArray[3 * i] = mSceneData.mMiddleLines.mLineList[1].longitude[i];
                    cVerticesArray[3 * i + 1] = mSceneData.mMiddleLines.mLineList[1].latitude[i];
                    cVerticesArray[3 * i + 2] = mSceneData.mMiddleLines.mLineList[1].altitude[i] + 5; 

                }
                try
                {
                    var sgworld = new SGWorld66();
                    var branch = sgworld.ProjectTree.FindItem("MiddleLine");
                    //int r, g, b;
                    //HslToRgb(10, 1 - item.AvgProgress, 1, out r, out g, out b);

                    sgworld.Creator.CreatePolylineFromArray(cVerticesArray, sgworld.Creator.CreateColor(255, 0, 0, 255),
                        AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, string.Empty, "Middle Line");
                    //item.mPolyline.Visibility.MinVisibilityDistance = 1250;
                    //item.mPolyline.Spline = true;
                    //item.mPolyline.LineStyle.Width = -5;
                    //item.mPolyline.

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Creation Middle Line Failed");
                }

            }

            //var sgworld = new SGWorld66();
            //var branch = sgworld.ProjectTree.CreateGroup("MiddleLine"); 
            ////Console.WriteLine(CRailwayScene.mProjectList.Count + "");
            //foreach (CRailwayProject item in CRailwayScene.mBridgeList)
            //    showProjectProgress(item);
            //foreach (CRailwayProject item in CRailwayScene.mRoadList)
            //    showProjectProgress(item);
            //foreach (CRailwayProject item in CRailwayScene.mTunnelList)
            //    showProjectProgress(item);
        }

 
        #endregion
    }
}
