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
    /// 处理桥墩
    /// </summary>
    public class CTEPier: CTERWItem
    {
        public CTEPier(CRailwayScene s, CTEScene ss, CRWTEStandard t)
            : base(s, ss, t)
        {


        }

        public override void TECreate()
        {
            var sgworld = new SGWorld66();
            if (string.IsNullOrEmpty(mGroupIDStatic))
                mGroupIDStatic = sgworld.ProjectTree.CreateGroup("Pier");
            string modelName = CGisDataSettings.gDataPath  + @"Common\Models\Pier\qiaodun.xpl2";
            ITerrainModel66 m = null;
            
            //ILabelStyle66 cLabelStyle = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);

            //{
            //    uint nBGRValue = 0xFF0000;  // Blue
            //    double dAlpha = 0.5;        // 50% opacity
            //    var cBackgroundColor = cLabelStyle.BackgroundColor; // Get label style background color
            //    cBackgroundColor.FromBGRColor(nBGRValue);               // Set background to blue
            //    cBackgroundColor.SetAlpha(dAlpha);                      // Set transparency to 50%
            //    cLabelStyle.BackgroundColor = cBackgroundColor;         // Set label style background color
            //    cLabelStyle.FontName = "Arial";                         // Set font name to Arial
            //    cLabelStyle.Italic = true;                              // Set label style font to italic
            //    cLabelStyle.Scale = 3;                                  // Set label style scale
            //    cLabelStyle.TextOnImage = false;
            //}

            foreach (CRailwayProject proj in mSceneData.mBridgeList)
            {
                //var branch = sgworld.ProjectTree.FindItem("Bridge\\"+cItem.mParentBridge.mProjectName);
                foreach (CRailwayPier cItem in proj.mDWProjList) {
                    var cPos = sgworld.Creator.CreatePosition(cItem.mLongitude_Mid, cItem.mLatitude_Mid, cItem.mAltitude_Mid - 0.5, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE,
                        cItem.mHeading_Mid , 0, 0);
                    m = sgworld.Creator.CreateModel(cPos, modelName, 1, ModelTypeCode.MT_NORMAL, mGroupIDStatic, cItem.DWName);
                    m.ScaleX = 0.01;
                    m.ScaleY = 0.01;
                    m.ScaleZ = 0.015;
                    m.BestLOD = 1000;
                    cPos.Altitude += 4;
                    sgworld.Creator.CreateTextLabel(cPos, cItem.DWName, mTEStandard.mLabelStyleL4, mGroupIDStatic, cItem.DWName);

                    //FIXME 添加属性随时间变动功能
                    //cItem.dte = DateTime.Now.AddDays(100); ;
                    //DateTime ds = Convert.ToDateTime("2015-06-01");
                    //cItem.dts = ds.AddDays(i);
                    //cItem.mTEModel.TimeSpan.Start = cItem.dts;
                    //cItem.mTEModel.TimeSpan.End = cItem.dte;
                    //i += 10;
                    //if (i > 180) { i = 0; }
                    //cItem.mTEModel.Terrain.Tint.abgrColor = 0xFF;
                    //cItem.mTEModel.Terrain.Tint.SetAlpha(alpha); // FIXME;

                    //alpha += 0.05;
                    //if (alpha > 1) alpha = 0.02;
                }
            }
        }

        public override void TEUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
