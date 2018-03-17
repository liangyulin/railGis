using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    public abstract class CTERWItem
    {
        public CRailwayScene mSceneData;
        
        public CRWTEStandard mTEStandard;

        public CTEScene mTEScene;

        public string mGroupIDStatic = null;
        public string mGroupIDDynamic = null;

        // .ToString("u")
        //public DateTime fromDate = DateTime.Now.AddDays(-7);
        //public DateTime toDate = DateTime.Now;
        //private CRailwayScene s;
        //private CRWTEStandard t;

        public CTERWItem(CRailwayScene s, CTEScene ss, CRWTEStandard t )
        {
            mSceneData = s;
            mTEScene = ss;
            mTEStandard = t;
        }

        public abstract void TECreate();

        public abstract void TEUpdate();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">中心点经度</param>
        /// <param name="y">中心点纬度</param>
        /// <param name="h">中心点高度</param>
        /// <param name="r">半径，以米为单位</param>
        /// <param name="angle">角度，圆周360度</param>
        /// <param name="n">点的个数</param>
        /// <returns>圆周点集(经纬度，高度)</returns>
        public static double[] TEDrawProgressCircle( double x, double y, double h, double r, int angle, out int count)
        {
            int step = 5;

            count = angle / step + 1 + (angle % step > 0 ? 1 : 0);
            if (count == 1)
            {
                count = 0;
                return null;
            }
            //double[] ps = new double[] { -l / 2, w / 2, h, l / 2, w / 2, h, l / 2, -w / 2, h, -l / 2, -w / 2, h };
            double[] p = new double[count * 3];
            double[] pd = new double[count * 3];
            double a = 0;
            double astep = Math.PI / 180 * step;
            double ux, uy;
            int i = 0;            
            CoordinateConverter.LatLonToUTMXY(y, x, out ux, out uy);

            // 计算圆弧点utm坐标
            for (i = 0; i < 3*(count-1); a+= astep, i+=3)
            {
                
                p[i] = r * Math.Cos(a) + ux;
                p[i + 1] = r * Math.Sin(a) + uy;
                pd[i + 2] = h;
                
            }
            //if (i == count){
            //    i --;
                p[i] = r * Math.Cos(angle * Math.PI / 180) + ux;
                p[i+ 1] = r * Math.Sin(angle * Math.PI / 180) + uy;
                pd[i + 2] = h;
            //}

            for (i = 0; i < count * 3; i+=3)
            {
                CoordinateConverter.UTMXYToLatLon(p[i], p[i + 1], out pd[i + 1], out pd[i],  false);
            }

            return pd;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">中心点经度</param>
        /// <param name="y">中心点纬度</param>
        /// <param name="h">中心点高度</param>
        /// <param name="l">矩形宽度</param>
        /// <param name="w">矩形高度</param>
        /// <param name="n">输出点的个数</param>
        /// <returns>矩形点集</returns>
        public static double[] TEDrawStationRect(double x, double y, double h, double l, double w, double dir)
        {
            //n = 4;
            double cosa = Math.Cos(dir * Math.PI / 180);
            double sina = Math.Sin(dir * Math.PI / 180);
            double[] ps = new double[] { -l/2, w/2, h, l/2, w/2, h, l/2, -w/2, h, -l/2, -w/2, h };
            double[] p = new double[12];
            double ux, uy;
            int zone;

            // 计算中心点 utm坐标
            CoordinateConverter.LatLonToUTMXY(y, x, out ux, out uy);

            // 计算四个顶点utm坐标
            for (int i = 0; i < 12; i += 3)
            {
                p[i] = cosa * ps[i] + sina * ps[i+1] + ux;
                p[i+1] = -sina * ps[i] + cosa * ps[i+1] + uy;
                p[i + 2] = h;
            }

            // 顶点坐标转换为经纬度坐标
            for (int i = 0; i < 12; i += 3)
                CoordinateConverter.UTMXYToLatLon(p[i], p[i + 1], out ps[i + 1], out ps[i],  false);
 
            return ps;

        } 
    }
    
}
