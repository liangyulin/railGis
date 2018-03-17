using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    public class CRWTEStandard
    {
        public ILabelStyle66 mLabelStyleL1;  // 全线 12KM分辨率 ，应用于建设单位，施工单位，监理单位， 车站
        public ILabelStyle66 mLabelStyleL2; //标段 3KM 分辨率， 制梁场，项目部，
        public ILabelStyle66 mLabelStyleL3; // 工点 600M 分辨率 工点，千米标
        public ILabelStyle66 mLabelStyleL4; // 单位 20M 分辨率 墩，梁，百米标


        public CRWTEStandard(){
            SGWorld66 sgworld = new SGWorld66();

            mLabelStyleL1= sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
            {
                mLabelStyleL1.FontName = "Arial";                         // Set font name to Arial
                mLabelStyleL1.Italic = false;                              // Set label style font to italic
                mLabelStyleL1.Scale = 200;                                // Set label style scale
                mLabelStyleL1.TextOnImage = false;
                mLabelStyleL1.TextColor = sgworld.Creator.CreateColor(255, 255, 255, 255);
                mLabelStyleL1.SmallestVisibleSize = 7;
                mLabelStyleL1.FontSize = 12;                
            }

            mLabelStyleL2 = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
            {
                mLabelStyleL2.FontName = "Arial";                         // Set font name to Arial
                mLabelStyleL2.Italic = false;                              // Set label style font to italic
                mLabelStyleL2.Scale = 50;                                // Set label style scale
                mLabelStyleL2.TextOnImage = false;
                mLabelStyleL2.TextColor = sgworld.Creator.CreateColor(0, 255, 0, 255);
                mLabelStyleL2.SmallestVisibleSize = 7;
                mLabelStyleL2.FontSize = 12;
            }

            mLabelStyleL3 = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
            {
                mLabelStyleL3.FontName = "Arial";                         // Set font name to Arial
                mLabelStyleL3.Italic = false;                              // Set label style font to italic
                mLabelStyleL3.Scale = 10;                                // Set label style scale
                mLabelStyleL3.TextOnImage = false;
                mLabelStyleL3.TextColor = sgworld.Creator.CreateColor(0, 255, 255, 255);
                mLabelStyleL3.SmallestVisibleSize = 7;
                mLabelStyleL3.FontSize = 12;
            }

            mLabelStyleL4 = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
            {
                mLabelStyleL4.FontName = "Arial";                         // Set font name to Arial
                mLabelStyleL4.Italic = false;                              // Set label style font to italic
                mLabelStyleL4.Scale = 1;                                // Set label style scale
                mLabelStyleL4.TextOnImage = false;
                mLabelStyleL4.TextColor = sgworld.Creator.CreateColor(0, 0, 255, 255);
                mLabelStyleL4.SmallestVisibleSize = 7;
                mLabelStyleL4.FontSize = 12;
            }

        }
        

    }
}
