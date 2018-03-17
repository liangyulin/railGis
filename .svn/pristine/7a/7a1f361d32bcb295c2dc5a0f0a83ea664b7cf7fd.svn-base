using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;
using TerraExplorerX;
using System.Collections;

namespace RailwayGIS.TEProject
{
    public class CTEMiddleLine : CTERWItem
    {
        public CTEMiddleLine(CRailwayScene s, CTEScene ss, CRWTEStandard t)
            : base(s, ss, t)
        {
        }
        public override void TECreate()
        {
            SGWorld66 sgworld = new SGWorld66();
            if (string.IsNullOrEmpty(mGroupIDStatic))
                mGroupIDStatic = sgworld.ProjectTree.CreateGroup("MiddleLine");
            IColor66 lineColorGreen = sgworld.Creator.CreateColor(0, 255, 0, 255);    //工点进度前20%
            IColor66 lineColorYellow = sgworld.Creator.CreateColor(255, 255, 0, 255); //工点进度前60%
            IColor66 lineColorRed = sgworld.Creator.CreateColor(255, 0, 0, 255);      //工点进度前20%
            IColor66 lineColorGray = sgworld.Creator.CreateColor(100, 100, 100, 255); //未施工
            List<CRailwayProject> items = new List <CRailwayProject> (mSceneData.mBridgeList);
            items.AddRange(mSceneData.mRoadList);
            items.AddRange(mSceneData.mTunnelList);
            IEnumerable<CRailwayProject> itemsOrderAsc = items.OrderBy(p => p, new CRWProjectCompares());
            int number = 0;
            int start = 1;
            foreach (CRailwayProject rw in itemsOrderAsc)
            {                
                if(rw.AvgProgress == 0)
                {
                    number++;
                    polylineMiddleLine(rw, lineColorGray, sgworld, mGroupIDStatic);
                }
                else
                {
                    int end = (itemsOrderAsc.Count() - number);
                    if (start <= (int)(end * 0.2))
                    {
                        start++;
                        polylineMiddleLine(rw, lineColorRed, sgworld, mGroupIDStatic);
                    }
                    else if ((start >= (int)(end * 0.2 + 1)) & (start <= (int)(end * 0.6)))
                    {
                        start++;
                        polylineMiddleLine(rw, lineColorYellow, sgworld, mGroupIDStatic);
                    }
                    else if ((start <= (int)(end * 0.6 + 1)) & (start <= end))
                    {
                        start++;
                        polylineMiddleLine(rw, lineColorGreen, sgworld, mGroupIDStatic);
                    }
               }
           
            }
        }

        public override void TEUpdate()
        {
            throw new NotImplementedException();
        }

        private void polylineMiddleLine(CRailwayProject rw, IColor66 lineColor, SGWorld66 sgworld, string branch)
        {
            ITerrainPolyline66 polyline;
            double[] x, y, z, dir;
            int count = mSceneData.mMiddleLines.getSubLineByDKCode(rw.Mileage_Start_Discription, rw.Mileage_End_Discription, 10, out x, out y, out z, out dir);
            double[] mArray = new double[count * 3];
            for (int i = 0; i < count; i++)
            {
                 mArray[3 * i] = x[i];
                 mArray[3 * i + 1] = y[i];
                 mArray[3 * i + 2] = z[i] + 1;

            }
            polyline = sgworld.Creator.CreatePolylineFromArray(mArray, lineColor, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, branch, rw.ProjectName);
            polyline.Spline = true;
            polyline.Visibility.MaxVisibilityDistance = 10000000;
            polyline.Visibility.MinVisibilityDistance = 12;
            polyline.LineStyle.Width = -8.0;
            polyline.Visibility.Show = true;
        }
    }

    public class CRWProjectCompares : IComparer<CRailwayProject>
    {
        public int Compare(CRailwayProject p1, CRailwayProject p2)
        {
            if (p1.AvgProgress > p2.AvgProgress)
            {
                return 1;
            }
            else if (p1.AvgProgress == p2.AvgProgress)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
