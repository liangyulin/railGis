using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    //public class CTEStation : CTERWItem
    //{
    //    //CRailwayScene mSceneData;
    //    //CRailwayProject st = null;
    //    List<CRailwayProject> mStList = null;

    //    public CTEStation(CRailwayScene s, CTEScene ss, CRWTEStandard t)
    //        : base(s, ss, t)
    //    {
    //        //mSceneData = s;
    //        mStList = s.mStationList;
    //    }
    //    public override void TECreate()
    //    {
    //        var sgworld = new SGWorld66();
    //        if (string.IsNullOrEmpty(mGroupIDStatic))
    //            mGroupIDStatic = sgworld.ProjectTree.CreateGroup("Station");
    //        double xs, ys, zs, ds, cx, cy;
            
    //        //double[] p = new double[] { -1, 1, 0, 1, 1, 0, 1, -1, 0, -1, -1, 0 };
    //        IPosition66 cp;
    //        //       ITerrainImageLabel66 iLabel;
    //        ITerrainLabel66 iLabel;
    //        //double t[10];

    //        //ILabelStyle66 cLabelStyle= sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
    //        //{
    //        //    //cLabelStyle.BackgroundColor = bgcolor;         // Set label style background color
    //        //    cLabelStyle.TextColor = sgworld.Creator.CreateColor(0, 0, 255, 255);
    //        //    cLabelStyle.FontName = "Arial";                         // Set font name to Arial
    //        //    cLabelStyle.FontSize = 10;
    //        //    cLabelStyle.SmallestVisibleSize = 7;
    //        //    cLabelStyle.Italic = true;                              // Set label style font to italic
    //        //    cLabelStyle.Scale = 700;                                  // Set label style scale
    //        //    //cLabelStyle.TextOnImage = false;
    //        //}

    //        //var branch = sgworld.ProjectTree.CreateGroup("站房工程");
    //        foreach (CRailwayProject st in mStList)
    //        {

    //                    //x = station.mLongitude_Mid;
    //                    //y = station.mLatitude_Mid;
    //                    //z = 10;
    //            //mStation.mDirection = 90;
    //            // 中点坐标
    //            if (!mSceneData.mMiddleLines.getPosbyDKCode(st.mDKCode_Mid, st.mMileage_Mid, out xs, out ys, out zs, out ds)) { 
    //                Console.WriteLine(st.ProjectName + "没有坐标");
    //                continue;
    //            }
    //            // 中点偏移50米坐标
    //            CoordinateConverter.LatLonOffest(ys, xs, ds, st.mDirection, 50, out cy, out cx);
    //            cp = sgworld.Creator.CreatePosition(cx, cy, 40, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -75, 0);



    //            //mSceneData.mMiddleLines.getPosbyDKCode(st.mDKCode_Mid, st.mMileage_Mid - 50, out xs, out ys, out zs, out ds);

    //            //CoordinateConverter.LatLonOffest(ys, xs, ds, st.mDirection, 25, out p[1], out p[0]);  //lat, lon
    //            //CoordinateConverter.LatLonOffest(ys, xs, ds, st.mDirection, 75, out p[4], out p[3]);

    //            //mSceneData.mMiddleLines.getPosbyDKCode(st.mDKCode_Mid, st.mMileage_Mid + 50, out xs, out ys, out zs, out ds);
    //            //CoordinateConverter.LatLonOffest(ys, xs, ds, st.mDirection, 25, out p[10], out p[9]);
    //            //CoordinateConverter.LatLonOffest(ys, xs, ds, st.mDirection, 75, out p[7], out p[6]);
    //            double[] poly;
    //            poly = CTERWItem.TEDrawStationRect(cx, cy, zs, 200, 50,  ds - 90 );
    //            //cp = sgworld.Creator.CreatePosition((p[0] + p[3] + p[6] + p[9]) / 4, (p[1] + p[4] + p[7] + p[10]) / 4, 40, AltitudeTypeCode.ATC_TERRAIN_RELATIVE);

    //            //iLabel = sgworld.Creator.CreateLabel(cp, st.ProjectName, st.mLabelImage, mTEStandard.mLabelStyleL1, mGroupIDStatic, st.ProjectName);
    //            //iLabel.Message.MessageID = sgworld.Creator.CreateMessage(MsgTargetPosition.MTP_POPUP, st.ProjectName, MsgType.TYPE_TEXT, true).ID;
    //            //iLabel.Visibility.MaxVisibilityDistance = 1000000;

    //            IGeometry buildingShp = null;
    //            ILinearRing cSQRing = sgworld.Creator.GeometryCreator.CreateLinearRingGeometry(poly);
    //            buildingShp = sgworld.Creator.GeometryCreator.CreatePolygonGeometry(cSQRing, null);
    //            var model = sgworld.Creator.CreateBuilding(buildingShp, 15, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, mGroupIDStatic, st.ProjectName);

    //            string fileName = CGisDataSettings.gDataPath + @"\Data\Textures\Sky_w_tan.jpg";
    //            model.Sides.Texture.FileName = CGisDataSettings.gDataPath + @"Common\Textures\Sky_w_tan.jpg";
    //            model.Roof.Texture.FileName = CGisDataSettings.gDataPath + @"Common\Textures\roof.png";



    //        }
    //        //catch (Exception ex)
    //        //{
    //        //    Console.WriteLine(ex.Message.ToString());
    //        //    Console.WriteLine("Station Creation Error");
    //        //}

    //    }

    //    public override void TEUpdate()
    //    {

    //    }
    //}
}
