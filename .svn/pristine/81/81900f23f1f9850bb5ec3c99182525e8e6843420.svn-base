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
    /// 千米标
    /// </summary>
    public class CTEKML : CTERWItem
    {


        public CTEKML(CRailwayScene s, CTEScene ss, CRWTEStandard t)
            : base(s, ss, t)
        {
           
        }

        public override void TECreate()
        {
            var sgworld = new SGWorld66();
            if (string.IsNullOrEmpty(mGroupIDStatic))
                mGroupIDStatic = sgworld.ProjectTree.CreateGroup("KML");
            ILabelStyle66 cLabelStyle = mTEStandard.mLabelStyleL3;
            
            //= sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);

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

            foreach (CRailwayLine rl in mSceneData.mMiddleLines.mLineList)
            {
                double[] x,y,z,w;
                int[] f;
                int num;
                num = rl.getKML(true,out x, out y,out z, out w, out f);
                for (int i = 0; i < num; i++)
                {
                    IPosition66 cp = sgworld.Creator.CreatePosition(x[i],y[i],z[i]+1,AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE,w[i]);
                    sgworld.Creator.CreateTextLabel(cp, f[i] + "", cLabelStyle, mGroupIDStatic, rl.mDKCode + f[i]);

                }
            }
            //for (int i = 0; i < mSceneData.mMiddleLines.mPointNum; i += 100)
            //{
            //    
            //}

        }
        public override void TEUpdate()
        {

        }
    }
}
