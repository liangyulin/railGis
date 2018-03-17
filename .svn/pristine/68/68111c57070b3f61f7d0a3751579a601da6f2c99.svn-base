using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    public class CTEFirm : CTERWItem
    {
        public CTEFirm(CRailwayScene s, CTEScene ss, CRWTEStandard t)
            : base(s, ss, t)
        {
        }

        public override void TECreate()
        {
            SGWorld66 sgworld = new SGWorld66();
            if (string.IsNullOrEmpty(mGroupIDStatic))
                mGroupIDStatic = sgworld.ProjectTree.CreateGroup("Firms");
            ITerrainLabel66 iLabel;
            ILabelStyle66 cLabelStyle;

            //ILabelStyle66 cLabelStyle = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_STREET);
            //ILabelStyle66 cLabelStyle = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_STATE);

            //{
            //   //uint nBGRValue = 0xff0000;                              // Blue
            //   //double dAlpha = 0.5;                                    // 50% opacity
            //   //var cBackgroundColor = cLabelStyle.BackgroundColor;     // Get label style background color
            //   //cBackgroundColor.FromBGRColor(nBGRValue);               // Set background to blue
            //   //cBackgroundColor.SetAlpha(dAlpha);                      // Set transparency to 50%
            //   //cLabelStyle.BackgroundColor = cBackgroundColor;         // Set label style background color
            //    cLabelStyle.FontName = "Arial";                         // Set font name to Arial
            //    cLabelStyle.Italic = true;                              // Set label style font to italic
            //    cLabelStyle.Scale = 200;                                // Set label style scale
            //    cLabelStyle.TextOnImage = false;
            //    cLabelStyle.TextColor = sgworld.Creator.CreateColor(0, 255, 255, 255);
            //    //cLabelStyle.SmallestVisibleSize = 7;
            //}


            foreach (CRailwayFirm rw in mSceneData.mFirmList)
            {
                IPosition66 cp = sgworld.Creator.CreatePosition(rw.CenterLongitude, rw.CenterLatitude, 10, AltitudeTypeCode.ATC_TERRAIN_RELATIVE);
                ITerrainRegularPolygon66 circle = sgworld.Creator.CreateCircle(cp, rw.NumStaff * 2 + 200, 0xFFFFFFFF, 0x00FF00FF, mGroupIDStatic, rw.FirmName + " " + rw.NumStaff);
                circle.LineStyle.Width = -5.0;

                if (rw.FirmType.Equals("制梁场") || rw.FirmType.Equals("项目部") || rw.FirmType.Equals("监理单位"))
                    cLabelStyle = mTEStandard.mLabelStyleL2;
                else
                    cLabelStyle = mTEStandard.mLabelStyleL1;
                iLabel = sgworld.Creator.CreateLabel(cp, rw.FirmName, CGisDataSettings.gDataPath + @"Common\地标图片\" + rw.mLabelImage, cLabelStyle, mGroupIDStatic, rw.FirmName);
                iLabel.Message.MessageID = sgworld.Creator.CreateMessage(MsgTargetPosition.MTP_POPUP, rw.ToString(), MsgType.TYPE_TEXT, true).ID;
            }
            //
            // F. Add Message to created circle
            //
            //{
            //    ITerraExplorerMessage66 cMessage = null;
            //    // F1. Set message input parameters
            //    MsgTargetPosition eMsgTarget = MsgTargetPosition.MTP_POPUP;
            //    string tMessage = "Hello Circle";
            //    MsgType eMsgType = MsgType.TYPE_TEXT;
            //    bool bIsBringToFront = true;

            //    // F2. Create message and add to circle
            //    cMessage = sgworld.Creator.CreateMessage(eMsgTarget, tMessage, eMsgType, bIsBringToFront);
            //    //iLabel.Message.MessageID = cMessage.ID;
            //}
            //foreach (CRailwayStaff rs in mSceneData.mStaffList)
            //{
            //    IPosition66 cp = sgworld.Creator.CreatePosition(rs.longitude, rs.latitude, 10, AltitudeTypeCode.ATC_TERRAIN_RELATIVE);
            //    ITerrainRegularPolygon66 circle = sgworld.Creator.CreateCircle(cp, rs.numStaff * 2 + 200, 0xFFFF00FF, 0x00FF00FF, branch, rs.shorName + " " + rs.numStaff);
            //    circle.LineStyle.Width = -5.0;
            //    //circle.Tooltip = rs.shorName + " " + rs.numStaff;

            //}
        }

        public override void TEUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
