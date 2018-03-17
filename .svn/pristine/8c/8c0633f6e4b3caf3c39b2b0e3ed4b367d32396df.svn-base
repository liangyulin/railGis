using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;
using System.Data.OleDb;
//using TerraExplorerX;


namespace GISUtilities.ModelInformation
{
    public class CScene
    {
        public static CRailWayMLine mMiddleLine = null;
        public static void loadMiddleLineFromDB(string fileName)
        {
            if (mMiddleLine != null) return;
            const int MAX_MID_P = 10100;
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
                string strConn;
                if (System.IO.Path.GetExtension(fileName) == ".xls")
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + fileName + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
                else
                    strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + fileName + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";

                conn = new OleDbConnection(strConn);
                conn.Open();
                myCommand = new OleDbDataAdapter(@"select Mileage,Longitude,Latitude,Altitude,ID from [SHEET1$] order by ID asc; ", conn);
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
    }

    public class CRailWayMLine
    {
        // 50米中线
        public int mPointNum;
        public double[] meter;
        public double[] latitude;
        public double[] longitude;
        public double[] altitude;
        public double[] heading;  // 控制火车及箭头方向
//        public double[] 
        public CRailWayMLine(int num, double[] m, double[] x, double[] y, double[] z)
        {

            mPointNum = (int)(m[num - 1] / 10)+2 ;
            meter = new double[mPointNum];
            //latitude = new double[count];
            //longitude = new double[count];
            //altitude = new double[count];
            meter[0] = 0; meter[mPointNum - 1] = m[num-1];
            for (int i = 1; i < mPointNum -1; i++)
                meter[i] = meter[i - 1] + 10;
            longitude = CubicSpline.Compute(num, m, x, meter);
            latitude = CubicSpline.Compute(num, m, y, meter);
            altitude = CubicSpline.Compute(num, m, z, meter);
            //heading = Coordinate.yawArray(mPointNum, longitude, latitude);
            heading = CoordinateConverter.LatLonToYawList(mPointNum, latitude, longitude);
            //validateHeading();

        }

        private void validateHeading()
        {
            for (int i = 0; i < mPointNum - 1; i++)
            {
                if (Math.Abs(heading[i] - heading[i + 1]) > 5 && Math.Abs(heading[i] - heading[i + 1])< 355)
                {
                    Console.WriteLine(meter[i] + "\t" + heading[i] +"\n" + meter[i+1] + "\t" + heading[i+1] + "\n");
                }
            }
        }



        // 获取线路的一部分，
        //startm, endm， multi 输入参数，起始终止里程与10米的multi倍， 输出参数，线路上的经纬高度坐标数组
        public int getSubLine(double startm, double endm, int multi,out double[] x,out double[] y,out double[] z)
        {
            double dis = Math.Abs(endm - startm);
            double dir;
            x = y = z = null; 
            if (dis < 1) return 0;
            else if (dis <= 10)
            {
                x = new double[2];
                y = new double[2];
                z = new double[2];
                findPosbyMeter(startm, out x[0], out y[0], out z[0], out dir);
                findPosbyMeter(endm, out x[1], out y[1], out z[1], out dir);
                return 2;
            }
            int count = (int)(dis / 10 /multi) + 1 ;
            double mstep = dis / (count-1);
            double m;

            x = new double[count];
            y = new double[count];
            z = new double[count];
            if (endm > startm)
            {
                m = startm;
                for (int i = 0; i < count; i++)
                {
                    findPosbyMeter(m, out x[i], out y[i], out z[i],out dir);
                    m += mstep;
                }
                 
            }
            else
            {
                m = endm;
                for (int i = 0; i < count; i++){
                    findPosbyMeter(m, out x[i], out y[i], out z[i], out dir);
                    m -= mstep;
                }
            }
            return count;
        }


        // 给定里程，计算经纬度朝向坐标
        public void findPosbyMeter(double m, out double x, out double y, out double z, out double dir)
        {
            int index = 0;
            x = y = z = 0; dir = 0;
            if (m <= 0)
            {
                x = longitude[0];
                y = latitude[0];
                z = altitude[0];
                if (heading != null)
                    dir = heading[0];
                return;
            }
            if (m >= meter[mPointNum - 1])
            {
                x = longitude[mPointNum - 1];
                y = latitude[mPointNum - 1];
                z = altitude[mPointNum - 1];
                if (heading != null)
                    dir = heading[mPointNum - 1];
                return;
            }
            int nm = (int)m;
            if (nm % 10 == 9)
            {
                index = nm / 10 + 1;
                x = longitude[index];
                y = latitude[index];
                z = altitude[index];
                if (heading != null)
                    dir = heading[index];
            }
            else
            {
                index = nm / 10;
                if (nm % 10 == 0)
                {
                    x = longitude[index];
                    y = latitude[index];
                    z = altitude[index];
                    if (heading != null)
                        dir = heading[index];
                }
                else
                {
                    double t = (m - nm) / 10;
                    x = longitude[index]* (1 - t) + longitude[index + 1]*t;
                    y = latitude[index] * (1 - t) + latitude[index + 1] * t;
                    z = altitude[index] * (1 - t) + altitude[index + 1] * t;
                    if (heading != null)
                        dir = heading[index] * (1 - t) + heading[index + 1] * t;
                }
            }
            return ;
        }

        //public IPosition66 findPositionbyIndex(int index){
        //    if (index < 0 || index >= mPointNum) return null;
        //    var sgworld = new SGWorld66();
        //    IPosition66 iPos = null;
        //    if (heading != null)
        //        iPos= sgworld.Creator.CreatePosition(longitude[index], latitude[index], altitude[index], AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, heading[index]);
        //    else
        //        iPos = sgworld.Creator.CreatePosition(longitude[index], latitude[index], altitude[index], AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);
        //    return iPos;

        //}

        public double findMeterbyCoor(double x, double y) {
            int errorNum = 0;
            try
            {
                double mileage = 0;

                int count = mPointNum;
                double mindist = (longitude[0] - x) * (longitude[0] - x) +
                    (latitude[0] - y) * (latitude[0] - y);
                double dist = 10;
                int step = (int)Math.Sqrt(count) + 1;
                //int i;
                int index = 0;
                for (int i = step; i < count; i += step)
                {
                    dist = (longitude[i] - x) * (longitude[i] - x) + (latitude[i] - y) * (latitude[i] - y);
                    if (dist < mindist)
                    {
                        mindist = dist;
                        index = i;
                    }
                    errorNum = i;

                }

                int index2 = index;
                int j = index - step;
                if (j < 0) j = 0;
                for ( ; j < index + step && j < count; j++)
                {
                    dist = (longitude[j] - x) * (longitude[j] - x) + (latitude[j] - y) * (latitude[j] - y);
                    if (dist < mindist)
                    {
                        mindist = dist;
                        index2 = j;
                    }
                    errorNum = -j;
                }
                if (index2 > 0 && index2 < count - 1)
                {
                    double d1, d2;
                    d1 = Math.Sqrt(Math.Pow(longitude[index2 - 1] - x, 2) + Math.Pow(latitude[index2 - 1] - y, 2));
                    d2 = Math.Sqrt(Math.Pow(longitude[index2 + 1] - x, 2) + Math.Pow(latitude[index2 + 1] - y, 2));
                    mileage = meter[index2 - 1] + 20 * d1 / (d1 + d2);
                }
                else if (index2 == 0)
                {
                    double d1, d2;
                    d1 = Math.Sqrt(Math.Pow(longitude[0] - x, 2) + Math.Pow(latitude[0] - y, 2));
                    d2 = Math.Sqrt(Math.Pow(longitude[1] - x, 2) + Math.Pow(latitude[1] - y, 2));
                    mileage = 10 * d1 / (d1 + d2);
                }
                else
                {
                    double d1, d2;
                    d1 = Math.Sqrt(Math.Pow(longitude[count - 2] - x, 2) + Math.Pow(latitude[count - 2] - y, 2));
                    d2 = Math.Sqrt(Math.Pow(longitude[count - 1] - x, 2) + Math.Pow(latitude[count - 1] - y, 2));
                    mileage = meter[count - 2] + 10 * d1 / (d1 + d2);
                }


                return mileage;
            }
            catch (Exception e)
            {
                Console.WriteLine("error num" + errorNum);
                return -1;
            }
        }


    }
 

}




