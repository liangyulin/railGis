using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using lib_GIS.Service;
using Microsoft.Office.Interop.Excel;

namespace ModelInfo
{
    /// <summary>
    /// 多链铁路线，FIXME，目前多链首尾相连，后面需要处理中间有分叉的情况。
    /// </summary>
    public class CRailwayLineList
    {
        public List<CRailwayLine> mLineList = new List<CRailwayLine>();

        /// <summary>
        /// 由数据库或远程服务器读入三维中线的数据
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadLineListFromDB()
        {
            double[] m, x, y, z;
            double fromMeter, toMeter;
            int fromID, toID;
            string dkcode;
            int count;

            System.Data.DataTable dt1, dt2;
            dt1 = dt2 = null;

            //System.Data.DataTable dt3;
            //double[] lgt, lat;
            //dt3 = ExcelWrapper.LoadDataTableFromExcel(@"D:\GISData\jiqing\中线测试.xlsx", "MiddleLine", @"select Mileage, Longitude, Latitude from [MiddleLine$];");
            //count = 93;
            //m = new double[count];
            //x = new double[count];
            //y = new double[count];
            //z = new double[count];
            
            //string dk = "DK";
            //double meter = 0;
            //double startm = 1800;
            //int ii = 0;
            //foreach (DataRow dr3 in dt3.Rows)
            //{
            //    //dr2 = dt2.Rows[i];
            //    parseDKCode(dr3["Mileage"].ToString(), out dk, out meter);
            //    m[ii] = meter - startm;
            //    x[ii] = Convert.ToDouble(dr3["Longitude"]);
            //    y[ii] = Convert.ToDouble(dr3["Latitude"]);
            //    ii++;
            //    //z[j] = Convert.ToDouble(dr3["Altitude"]);
            //}
            //CoordinateConverter.UTMXYToLatLonList(count,y,x,out lgt, out lat,50,false);
            ////UTM2GPS.GaussProjInvCal(y[0], x[0], out lgt[1], out lat[1]);
            //for (int i = 0; i < count; i++)
            //{
            //    Console.WriteLine("longi: {0}\t lati {1}", lgt[i], lat[i]);
            //}
            //Console.WriteLine();

                if (!CServerWrapper.isConnected)
                {
                    string fileName = CGisDataSettings.gDataPath + @"jiqing\MiddleLine0525.xls";
                    dt1 = DatabaseWrapper.LoadDataTableFromExcel(fileName, "Chain", @"select fromMeter, toMeter,fromID,toID,DKCode from [Chain$]; ");
                    dt2 = DatabaseWrapper.LoadDataTableFromExcel(fileName, "MiddleLine", @"select Mileage, Longitude,Latitude,Altitude from [MiddleLine$]; ");
                    ////dt1 = ds.Tables["Chain"];
                    ////dt2 = ds2.Tables["MiddleLine"];
                }
                else
                {
                    //  ProjectService.ProjectServiceSoapClient ws = new ProjectService.ProjectServiceSoapClient();

                    //WS_GISServerData.GisDataWebServiceSoapClient ws = new WS_GISServerData.GisDataWebServiceSoapClient();
                    //dt1 = ws.ws_FindChainInfo();
                    //dt2 = ws.ws_FindMileageInfo();
//                    double mm = 100000;
//                    double mmin  ,xx,yy,zz;
//                    double mmax, xx2,yy2,zz2;
//                    double mx, my, mz;
//                    string dd = "DK";
//                    mmin = xx = yy = zz = 0; //Mileage, Longitude, Latitude, Altitude
//                    mx = my = mz = 0;
//                    System.Data.DataTable dt = CServerWrapper.execSqlQuery(@"SELECT   Mileage, Longitude, Latitude, Altitude
//FROM      MileageInfo
//WHERE   (MileagePrefix = '" +
//  dd + @"') AND ( Mileage > " + (mm - 500) + @" ) AND ( Mileage <" + (mm + 500) + @" ) ORDER BY Mileage");

//                    DatabaseWrapper.PrintDataTable(dt);
//                    bool isfind = false;
//                    foreach (DataRow dr in dt.Rows)
//                    {
//                        if (Convert.ToDouble(dr["Mileage"]) < mm)
//                        {
//                            mmin = Convert.ToDouble(dr["Mileage"]);
//                            xx = Convert.ToDouble(dr["Longitude"]);
//                            yy = Convert.ToDouble(dr["Latitude"]);
//                            zz = Convert.ToDouble(dr["Altitude"]);

//                        }
//                        else {
//                            isfind = true;
//                            mmax = Convert.ToDouble(dr["Mileage"]);
//                            xx2 = Convert.ToDouble(dr["Longitude"]);
//                            yy2 = Convert.ToDouble(dr["Latitude"]);
//                            zz2 = Convert.ToDouble(dr["Altitude"]);
//                            double a = (mm - mmin) / (mmax - mmin);
//                            mx = xx + (xx2 - xx) * a;
//                            my = yy + (yy2 - yy) * a;
//                            mz = zz + (zz2 - zz) * a;
                       
//                        }
//                    }
//                    Console.WriteLine(mx + "\t" + my + "\t" +mz + "\t"+ isfind);

                    dt1 = CServerWrapper.findChainInfo();
                    dt2 = CServerWrapper.findMileageInfo();
                    //dt1 = ProjectFromServer.FindChainInfo();
                    //dt2 = ProjectFromServer.FindMileageInfo();
                }


            DataRow dr2;
            foreach (DataRow dr in dt1.Rows)
            {

                fromMeter = (double)dr["fromMeter"];
                toMeter = (double)dr["toMeter"];
                if (!CServerWrapper.isConnected)
                {
                    fromID = (int)((double)dr["fromID"] + 0.1);
                    toID = (int)((double)dr["toID"] + 0.1);
                }
                else
                {
                    fromID = (int)dr["fromID"];
                    toID = (int)dr["toID"];
                }
                dkcode = (string)dr["DKCode"];
                count = toID - fromID + 1;

                if (count < 2) continue;

                m = new double[count];
                x = new double[count];
                y = new double[count];
                z = new double[count];
                int j = 0;
                for (int i = fromID - 1; i < toID; i++, j++)
                {
                    dr2 = dt2.Rows[i];
                    m[j] = Math.Abs((double)dr2["Mileage"] - fromMeter);
                    x[j] = Convert.ToDouble(dr2["Longitude"]);
                    y[j] = Convert.ToDouble(dr2["Latitude"]);
                    z[j] = Convert.ToDouble(dr2["Altitude"]);
                }
                mLineList.Add(new CRailwayLine(dkcode, fromMeter, toMeter, false, count, m, x, y, z));

            }

        }



        public bool getPosbyMeter(string dkcode,  out double x, out double y, out double z, out double dir )
        {
            x = y = z = dir = 0;
            string dk;
            double meter;
            parseDKCode(dkcode, out dk, out meter);
            return getPosbyDKCode(dk, meter, out x, out y, out z, out dir);

        }

        /// <summary>
        ///  根据里程（DK123+45），计算半径radius距离内的所有单位，按距离排序输出
        /// </summary>
        /// <param name="dkCode"></param>
        /// <param name="radius"></param>
        /// <param name="firmList"></param>
        /// <returns></returns>
        public Dictionary<CRailwayFirm, double> getNearFirms(string dkCode, double radius, List<CRailwayFirm> firmList)
        {
            //double[] lenList = new double[firmList.Count];
            Dictionary<CRailwayFirm,double> dict = new Dictionary<CRailwayFirm,double>(firmList.Count);
            double xx,yy,zz,dir,len;
            //double xx1,yy1,zz1,dir1;
            getPosbyMeter(dkCode,out xx, out yy,out zz, out dir);
            for (int i = 0; i < firmList.Count; i++)
            {
                len = CoordinateConverter.getUTMDistance(firmList[i].CenterLongitude, firmList[i].CenterLatitude, xx, yy);
                if (len < radius)
                    dict.Add(firmList[i],len);
                
            }

            var dictSort = from d in dict orderby d.Value ascending select d;
            
            return dict;



                
        }


        /// <summary>
        ///  根据里程（DK123+45），计算半径radius距离内的所有单位工程，按距离排序输出
        /// </summary>
        /// <param name="dkCode"></param>
        /// <param name="radius"></param>
        /// <param name="firmList"></param>
        /// <returns></returns>
        public Dictionary<CRailwayDWProj, double> getNearDWProj(string dkCode, double radius, List<CRailwayDWProj> dwProjectList)
        {
            Dictionary<CRailwayDWProj, double> dict = new Dictionary<CRailwayDWProj, double>(dwProjectList.Count);
            double xx, yy, zz, dir, len;
            getPosbyMeter(dkCode, out xx, out yy, out zz, out dir);
            for (int i = 0; i < dwProjectList.Count; i++)
            {
                len = CoordinateConverter.getUTMDistance(dwProjectList[i].mLongitude_Mid, dwProjectList[i].mLatitude_Mid, xx, yy);
                if (len < radius)
                    dict.Add(dwProjectList[i], len);

            }

            var dictSort = from d in dict orderby d.Value ascending select d;

            return dict;

        }

        /// <summary>
        ///  根据经纬度，计算半径radius距离内的所有单位工程，按距离排序输出
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="firmList"></param>
        /// <returns></returns>
        public Dictionary<CRailwayDWProj, double> getNearDWProj(double x, double y, double radius, List<CRailwayDWProj> dwProjectList)
        {
            Dictionary<CRailwayDWProj, double> dict = new Dictionary<CRailwayDWProj, double>(dwProjectList.Count);
            double len;
            //getNearPos(x, y, out mm, out xx, out yy, out zz, out dir);
            for (int i = 0; i < dwProjectList.Count; i++)
            {
                len = CoordinateConverter.getUTMDistance(dwProjectList[i].mLongitude_Mid, dwProjectList[i].mLatitude_Mid, x, y);
                if (len < radius)
                    dict.Add(dwProjectList[i], len);

            }

            var dictSort = from d in dict orderby d.Value ascending select d;

            return dict;

        }

        /// <summary>
        ///  根据里程（DK123+45），计算半径radius距离内的所有工点，按距离排序输出
        /// </summary>
        /// <param name="dkCode"></param>
        /// <param name="radius"></param>
        /// <param name="firmList"></param>
        /// <returns></returns>
        public Dictionary<CRailwayProject, double> getNearProject(string dkCode, double radius, List<CRailwayProject> projectList)
        {
            Dictionary<CRailwayProject, double> dict = new Dictionary<CRailwayProject, double>(projectList.Count);
            double xx, yy, zz, dir, len_start, len_mid, len_end, len; 
            getPosbyMeter(dkCode, out xx, out yy, out zz, out dir);
            for (int i = 0; i < projectList.Count; i++)
            {
                len_start = CoordinateConverter.getUTMDistance(projectList[i].mLongitude_Start, projectList[i].mLatitude_Start, xx, yy);
                len_mid = CoordinateConverter.getUTMDistance(projectList[i].CenterLongitude, projectList[i].CenterLatitude, xx, yy);
                len_end = CoordinateConverter.getUTMDistance(projectList[i].mLongitude_End, projectList[i].mLatitude_End, xx, yy);
                len = getMinLen(len_start, len_mid, len_end);

                if (len_start < radius || len_mid < radius || len_end < radius)
                    dict.Add(projectList[i], len);

            }

            var dictSort = from d in dict orderby d.Value ascending select d;

            return dict;

        }

        /// <summary>
        ///  根据经纬度，计算半径radius距离内的所有工点，按距离排序输出
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="firmList"></param>
        /// <returns></returns>
        public Dictionary<CRailwayProject, double> getNearProject(double x, double y, double radius, List<CRailwayProject> projectList)
        {
            Dictionary<CRailwayProject, double> dict = new Dictionary<CRailwayProject, double>(projectList.Count);
            double len, len_start, len_mid, len_end; 
            //getNearPos(x, y, out mm, out xx, out yy, out zz, out dir);
            for (int i = 0; i < projectList.Count; i++)
            {
                len_start = CoordinateConverter.getUTMDistance(projectList[i].mLongitude_Start, projectList[i].mLatitude_Start, x, y);
                len_mid = CoordinateConverter.getUTMDistance(projectList[i].CenterLongitude, projectList[i].CenterLatitude, x, y);
                len_end = CoordinateConverter.getUTMDistance(projectList[i].mLongitude_End, projectList[i].mLatitude_End, x, y);
                len = getMinLen(len_start, len_mid, len_end);

                if (len < radius)
                    dict.Add(projectList[i], len);

            }

            var dictSort = from d in dict orderby d.Value ascending select d;

            return dict;

        }



        /// <summary>
        /// 获取最小len
        /// </summary>
        public double getMinLen(double len_start, double len_mid, double len_end)
        {
            double len = Math.Min(Math.Min(len_start, len_mid), len_end);
            return len;

        }

                /// <summary>
        /// 根据dk码以及里程，例如DIK12+23.4，获取经纬度高度、朝向坐标
        /// </summary>
        /// <param name="dk"></param>
        /// <param name="meter"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public bool getPosbAndLengthByDKCode(string dk, double meter, out double x, out double y, out double z, out double dir, out double globalmeter)
        {
            x = y = z = dir = 0;
            double dis;
            globalmeter = 0;
            foreach (CRailwayLine rw in mLineList)
            {
                if (rw.isInSide(dk, meter, out dis))
                {
                    rw.getPosbyMeter(meter, out x, out y, out z, out dir);
                    globalmeter += dis;
                    return true;

                }
                globalmeter += rw.mLength;
            }
            //Console.WriteLine(dk + " " + meter + "is not valid !");
            return false;
        }

        /// <summary>
        /// 根据dk码以及里程，例如DIK12+23.4，获取经纬度高度、朝向坐标
        /// </summary>
        /// <param name="dk"></param>
        /// <param name="meter"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public bool getPosbyDKCode(string dk, double meter, out double x, out double y, out double z, out double dir )
        {
            x = y = z = dir = 0;
            double dis;
            
            foreach (CRailwayLine rw in mLineList)
            {
                if (rw.isInSide(dk, meter, out dis))
                {
                    rw.getPosbyMeter(meter, out x, out y, out z, out dir);
                    
                    return true;

                }
                
            }
            //Console.WriteLine(dk + " " + meter + "is not valid !");
            return false;
        }

        /// <summary>
        /// 根据经纬度，获取里程，如果经纬度不在铁路线上，定位铁路线上最近的一个点，返回该段铁路、该点里程以及距离
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dis"></param>
        /// <returns></returns>
        public CRailwayLine getMeterbyPos(double x, double y, out double mileage, out double dis)
        {
            dis = -1;
            //double mi;
            mileage = -1;
            double mi;
            double min = 100;
            CRailwayLine mrw = null;
            foreach (CRailwayLine rw in mLineList)
            {
                rw.getMeterbyPos(x, y,out mi, out dis);
                if (mrw == null)
                {
                    min = dis;
                    mileage = mi;
                    mrw = rw;
                }
                else if (dis < min)
                {
                    min = dis;
                    mileage = mi;
                    mrw = rw;
                }


            }
            dis = min;
            //mileage 
            return mrw;
        }

        ///// <summary>
        ///// 根据输入GPS坐标，获取铁路线上最近点的GPS坐标以及该段铁路
        ///// </summary>
        ///// <param name="x"></param>
        ///// <param name="y"></param>
        ///// <param name="om"></param>
        ///// <param name="ox"></param>
        ///// <param name="oy"></param>
        ///// <param name="oz"></param>
        ///// <param name="odir"></param>
        ///// <returns></returns>
        //public CRailwayLine getNearPos(double x, double y, out double om, out double ox, out double oy, out double oz, out double odir)
        //{
        //    om = ox = oy = oz = odir = 0;            
        //    CRailwayLine mrw = null;
        //    double dis;
        //    mrw = getMeterbyPos(x, y, out om, out dis);
        //    mrw.getPosbyMeter(om, out ox, out oy, out oz, out odir);
        //    return mrw;
        //}

        
        /// <summary>
        /// 解析dkcode为code与里程，输入串规则“**K123 + 456.78”，根据输入铁路链，同时验证该输入语法及数值是否合法。
        /// </summary>
        /// <param name="inputCode"></param>
        /// <param name="dkCode"></param>
        /// <param name="meter"></param>
        /// <param name="validateMeter"></param>
        /// <returns></returns>
        public bool parseDKCode(string inputCode, out string dkCode, out double meter, double validateMeter = -1)
        {
            bool isValid = true;
            dkCode = "DK";
            meter = 0;
            try
            {
                int indexK = inputCode.LastIndexOf('K');
                if (indexK >= 0)
                    dkCode = inputCode.Substring(0, indexK + 1);

                string ts;
                if (indexK >= 0)
                    ts = inputCode.Substring(indexK + 1);
                else
                    ts = inputCode;

                string[] ss = ts.Split(new char[1] { '+' });
                meter = Double.Parse(ss[0].Trim()) * 1000 + Double.Parse(ss[1].Trim()) ;
                if (validateMeter >= 0 && Math.Abs(meter - validateMeter) > 0.1)
                    isValid = false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Format error" + inputCode);
                isValid = false;
            }

            return isValid;
        }


        /// <summary>
        ///  合并code与里程为dkcode
        /// </summary>
        public static string CombiDKCode(string dkCode, double meter)
        {
            string outputCode = "DK0+0";
            try
            {
                int con = (int)meter;
                double ss1 = con / 1000;
                double ss2 = con % 1000;

                outputCode = dkCode + ss1 + "+" + ss2;
            }
            catch (Exception e)
            {
                Console.WriteLine("Format error" + dkCode + meter);
            }
            return outputCode;

        }

        /// <summary>
        /// 计算偏移一定角度和距离之后的经纬度
        /// </summary>
        /// <param name="startDKM"></param>
        /// <param name="endDKM"></param>
        /// <param name="stepm"></param>
        /// <param name="angleOff"></param>
        /// <param name="disOff"></param>
        /// <param name="latout"></param>
        /// <param name="lonout"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public int getOffsetLine(string startDKM, string endDKM, double stepm, double angleOff, double disOff, out double[] latout, out double[] lonout, out double[] z)
        {
            lonout = latout = z = null; 
            double[] x = null;
            double[] y = null;
            //double[] z = null;
            double[] d = null;

            int pointNum = getSubLineByDKCode(startDKM, endDKM, stepm, out x, out y, out z, out d);
            if (pointNum > 0)
            {
                latout = new double[pointNum];
                lonout = new double[pointNum];
                for (int i = 0; i < pointNum; i++)
                {
                    CoordinateConverter.LatLonOffest(y[i], x[i], d[i], angleOff, disOff, out latout[i], out lonout[i]);  //lat, lon
                }

            }
            
            return pointNum;
        }


        /// <summary>
        /// TODO： 计算一段可能跨链的铁路，用于展示一个工点（一段路基），拟在CTEProject中调用，展示三维中线，呈现工程进度。
        /// </summary>
        /// <param name="startDKM">起始里程</param>
        /// <param name="endDKM">终止历程</param>
        /// <param name="stepm">每两个采样点的间隔距离</param>
        /// <param name="x">采样点经度</param>
        /// <param name="y">采样点纬度</param>
        /// <param name="z">采样点高度</param>
        /// <param name="dir">采样点方位角（朝向）</param>
        /// <returns>采样点个数</returns>
        public int getSubLineByDKCode(string startDKM, string endDKM, double stepm, out double[] x, out double[] y, out double[] z, out double[] dir)
        {
            x = y = z = dir = null;

            string startDKCode, endDKCode;
            double startMileage, endMileage;

            //拆分code
            parseDKCode(startDKM, out startDKCode, out startMileage, -1);
            parseDKCode(endDKM, out endDKCode, out endMileage, -1);

            return getSubLineByDKCode(startDKCode, startMileage, endDKCode, endMileage,stepm, out x, out y, out z, out dir);


        }


        public int getSubLineByDKCode(string startDKCode, double startMileage, string endDKCode, double endMileage, double stepm, out double[] x, out double[] y, out double[] z, out double[] dir)
        {
            x = y = z = dir = null;

            //获取两点所在的链
            int startNumber = 0, endNumber = 0;
            double startDis, endDis;
            CRailwayLine startChain = null, endChain = null;
            for (int count = 0; count < mLineList.Count; count++)
            {
                if (mLineList[count].isInSide(startDKCode, startMileage, out startDis))
                {
                    startChain = mLineList[count];
                    startNumber = count;
                }

                if (mLineList[count].isInSide(endDKCode, endMileage, out endDis))
                {
                    endChain = mLineList[count];
                    endNumber = count;
                }
            }

            if (startChain == null || endChain == null)
            {
                return 0;
            }

            //保证startChain小于等于endChain
            if (startNumber > endNumber)
            {
                int temp = startNumber;
                double tempMileage = startMileage;
                CRailwayLine tempChain = startChain;

                startNumber = endNumber;
                startMileage = endMileage;
                startChain = endChain;

                endNumber = temp;
                endMileage = tempMileage;
                endChain = tempChain;
            }

            int[] ccount = new int[endNumber - startNumber + 1];
            double[][] xx = new double[endNumber - startNumber + 1][];
            double[][] yy = new double[endNumber - startNumber + 1][];
            double[][] zz = new double[endNumber - startNumber + 1][];
            double[][] dd = new double[endNumber - startNumber + 1][];//double[][] XX :交错数组 instead of 二维数组

            if (startNumber == endNumber)  //start和end是同一条链
            {
                int pointNumber = startChain.getSubLine(startMileage, endMileage, stepm, out x, out y, out z, out dir);
                return pointNumber;
            }
            else //两点不在同一条链
            {
                //首链
                ccount[0] = startChain.getSubLine(startMileage, startChain.mEnd, stepm, out xx[0], out yy[0], out zz[0], out dd[0]);

                //中间链
                int account = ccount[0];
                if (Math.Abs(endNumber - startNumber) > 1)
                {
                    int i = 1;
                    for (int ii = startNumber + 1; ii < endNumber; ii++)
                    {
                        ccount[i] = mLineList[ii].getSubLine(mLineList[ii].mStart, mLineList[ii].mEnd, stepm, out xx[i], out yy[i], out zz[i], out dd[i]);

                        account += ccount[i];
                        i++;
                    }
                }

                //末链
                ccount[endNumber - startNumber] = endChain.getSubLine(endChain.mStart, endMileage, stepm, out xx[endNumber - startNumber], out yy[endNumber - startNumber], out zz[endNumber - startNumber], out dd[endNumber - startNumber]);

                account += ccount[endNumber - startNumber];

                //声明输出数组
                x = new double[account];
                y = new double[account];
                z = new double[account];
                dir = new double[account];

                //首链
                for (int num = 0; num < ccount[0]; num++)
                {
                    x[num] = xx[0][num];
                    y[num] = yy[0][num];
                    z[num] = zz[0][num];
                    dir[num] = dd[0][num];
                }
                int account0 = ccount[0];

                //中间链
                if (Math.Abs(endNumber - startNumber) > 1)
                {
                    int i = 1;
                    for (int ii = startNumber + 1; ii < endNumber; ii++)
                    {
                        for (int num = 0; num < ccount[i]; num++)
                        {
                            x[account0 + num] = xx[i][num];
                            y[account0 + num] = yy[i][num];
                            z[account0 + num] = zz[i][num];
                            dir[account0 + num] = dd[i][num];
                        }
                        account0 += ccount[i];
                        i++;
                    }
                }

                //末链
                for (int num = 0; num < ccount[endNumber - startNumber]; num++)
                {
                    x[account0 + num] = xx[endNumber - startNumber][num];
                    y[account0 + num] = yy[endNumber - startNumber][num];
                    z[account0 + num] = zz[endNumber - startNumber][num];
                    dir[account0 + num] = dd[endNumber - startNumber][num];
                }

                return account;
            }

        }

        /// <summary>
        /// 获取总里程
        /// </summary>
        /// <param name="dkcode"></param>
        /// <param name="dkmeter"></param>
        /// <returns></returns>
        public double getGlobalMeter(string dkcode, double dkmeter) {
            double gm = 0;
            double dis = 0;
            bool isfind = false;
            foreach (CRailwayLine rw in mLineList)
            {
                if (rw.isInSide(dkcode, dkmeter, out dis))
                {
                    
                    gm += dis;
                    isfind = true;
                    break;

                }
                gm += rw.mLength;
            }
            if (isfind)
                return gm;
            else
                return -1;
        }
        /// <summary>
        /// TODO：计算一段可能跨链的铁路，用于生成火车漫游路径，由startDKM开始，长度为disFromStart的一段路
        /// </summary>
        /// <param name="startDKM">起始里程</param>
        /// <param name="disFromStart">起点里程偏移,为正时，小里程至大里程，为负数时，大里程至小里程</param>
        /// <param name="stepm">每两个采样点的间隔距离</param>
        /// <param name="x">采样点经度</param>
        /// <param name="y">采样点纬度</param>
        /// <param name="z">采样点高度</param>
        /// <param name="dir">采样点方位角（朝向）</param>
        /// <returns>采样点个数</returns>
        public int getSubLine(string startDKM, double disFromStart, double stepm, out double[] x, out double[] y, out double[] z, out double[] dir)
        {
            x = y = z = dir = null;

            string startDKCode, endDKCode, endDKM;
            double startMileage, endMileage;

            //拆分code
            parseDKCode(startDKM, out startDKCode, out startMileage, -1);

            //获取起点所在的链
            int startNumber = 0;
            double startDis;
            CRailwayLine startChain = null;
            for (int count = 0; count < mLineList.Count; count++)
            {
                if (mLineList[count].isInSide(startDKCode, startMileage, out startDis))
                {
                    startChain = mLineList[count];
                    startNumber = count;
                }
            }

            if (startChain == null)
            {
                return 0;
            }

            double desMileage = startMileage + disFromStart;

            if ((!startChain.mIsReverse && desMileage <= startChain.mEnd && desMileage >= startChain.mStart) || (startChain.mIsReverse && desMileage <= startChain.mStart && desMileage >= startChain.mEnd))
            {
                int pointNumber = startChain.getSubLine(startMileage, desMileage, stepm, out x, out y, out z, out dir);

                return pointNumber;
            }
            else {
                int pointNum = 0;
                if (disFromStart >= 0)  //小里程至大里程
                {
                    double remain = disFromStart - Math.Abs(startChain.mEnd - startMileage);

                    double chainsLength = 0;

                    for (int i = startNumber+1; i < mLineList.Count; i++)
                    {
                        chainsLength += mLineList[i].mLength;

                        if (chainsLength >= remain)
                        {
                            if (mLineList[i].mIsReverse)
                                endMileage = mLineList[i].mEnd + (chainsLength - remain);
                            else 
                                endMileage = mLineList[i].mEnd-(chainsLength - remain);

                            endDKCode = mLineList[i].mDKCode;

                            endDKM = CombiDKCode(endDKCode, endMileage );

                            pointNum = getSubLineByDKCode(startDKM, endDKM, stepm, out x, out y, out z, out dir);

                            break;
                        }

                    }
                }
                else  //大里程至小里程
                {
                    double remain = -disFromStart - Math.Abs(startMileage - startChain.mStart); 

                    double chainsLength = 0;

                    for (int i = startNumber - 1; i >= 0; i--)
                    {
                        chainsLength += mLineList[i].mLength;

                        if (chainsLength >= remain)
                        {
                            if (mLineList[i].mIsReverse)
                                endMileage = mLineList[i].mStart - (chainsLength - remain);
                            else 
                                endMileage = mLineList[i].mStart + (chainsLength - remain);

                            endDKCode = mLineList[i].mDKCode;

                            endDKM = CombiDKCode(endDKCode, endMileage );

                            pointNum = getSubLineByDKCode(startDKM, endDKM, stepm, out x, out y, out z, out dir);

                            break;
                        }

                    }
                }

                return pointNum;
            }
        }


    }


    // 规则，规范化里程从0开始，查找时利用里程的变换公式
    public class CRailwayLine
    {
        // 10米中线
        public int mPointNum;
        public double[] meter;
        public double[] latitude;
        public double[] longitude;
        public double[] altitude;
        public double[] heading;  // 控制火车及箭头方向
        public double[] pitching;
        public double[] utmX;  // 保留
        public double[] utmY;  // 保留
        public double mStart;
        public double mEnd;
        //public int mFromID;
        //public int mToID;
        public double mLength;
        public bool mIsAuxiliary; // 保留
        public bool mIsReverse;
        public string mDKCode; // "DK" "DIK"
        //        public double[] 
        public CRailwayLine(string dkcode, double fromM, double toM, bool isA, int num, double[] m, double[] x, double[] y, double[] z)
        {
            mStart = fromM;
            mEnd = toM;
            if (mStart < mEnd)
                mIsReverse = false;
            else
                mIsReverse = true;
            mDKCode = dkcode;
            mIsAuxiliary = isA;
            //mFromID = fromID;
            //mToID = toID;

            mLength = m[num - 1] - m[0];
            mPointNum = (int)((mLength - 0.05) / 10) + 2;  //FIXME 误差
            meter = new double[mPointNum];

            meter[0] = m[0];
            meter[mPointNum - 1] = m[num - 1];
            //for (int j = 0; j < num - 1; j++)
            //    if (m[j] >= m[j+ 1])
            //        Console.WriteLine("mileage error " + m[j] + "\t" + m[j+1]); 
                
            for (int i = 1; i < mPointNum - 1; i++)
                    meter[i] = meter[i - 1] + 10;

            longitude = CubicSpline.Compute(num, m, x, meter);
            latitude = CubicSpline.Compute(num, m, y, meter);
            altitude = CubicSpline.Compute(num, m, z, meter);

            CoordinateConverter.LatLonToYawList(mPointNum, latitude, longitude, out utmX, out utmY, out heading);
            

        }

        /// <summary>
        /// 根据输入的DK代码以及里程，判断是否在该线路内，返回距离起点的里程
        /// </summary>
        /// <param name="dkcode"></param>
        /// <param name="meter"></param>
        /// <param name="dis"></param>
        /// <returns></returns>
        public bool isInSide(string dkcode, double meter, out double dis)
        {
            dis = 0;
            if (dkcode.Equals(mDKCode, StringComparison.CurrentCultureIgnoreCase))
            {
                if (mIsReverse)
                {
                    if (meter >= mEnd && meter <= mStart)
                    {
                        dis = mStart - meter;
                        return true;
                    }
                }
                else
                {
                    if (meter >= mStart && meter <= mEnd)
                    {
                        dis = meter - mStart;
                        return true;
                    }
                }

            }
            return false;
        }

        //private void validateHeading()
        //{
        //    for (int i = 0; i < mPointNum - 1; i++)
        //    {
        //        if (Math.Abs(heading[i] - heading[i + 1]) > 5 && Math.Abs(heading[i] - heading[i + 1])< 355)
        //        {
        //            Console.WriteLine(meter[i] + "\t" + heading[i] +"\n" + meter[i+1] + "\t" + heading[i+1] + "\n");
        //        }
        //    }
        //}

        //获取千米标与百米标，如果isKML为true则为千米标，否则为百米标
        public int getKML(bool isKML, out double[] x, out double[] y, out double[] z, out double[] dir, out int[] meterFlag)
        {
            double startm, deltam;
            int count = 0;
            int s, e;
            double[] m;
            x = y = z = dir = null;
            meterFlag = null;
            int stepm = 100;
            if (isKML) stepm = 1000;


            s = (int)(mStart / stepm);
            e = (int)(mEnd / stepm);

            count = Math.Abs(e - s);
            m = new double[count];
            meterFlag = new int[count];

            if (mIsReverse)
                deltam = -stepm;
            else
                deltam = stepm;

            if (Math.Abs(s * stepm - mStart) < 0.1)
                startm = mStart;
            else
                startm = s * stepm + deltam;

            for (int i = 0; i < count; i++)
            {
                m[i] = startm;
                meterFlag[i] = (int)(m[i] / stepm);
                startm += deltam;

            }
            getSubLine(count, m, out x, out y, out z, out dir);


            return count;
        }

        //
        // 获取线路的一部分，
        //输入参数:里程采样点meterSample及数目count
        //输出参数：里程采样点的经纬度、高度坐标以及朝向        
        //
        public void getSubLine(int count, double[] meterSample, out double[] x, out double[] y, out double[] z, out double[] dir)
        {
            x = y = z = dir = null;
            for (int i = 0; i < count; i++)
                meterSample[i] = Math.Abs(meterSample[i] - mStart);
            x = CubicSpline.Compute(mPointNum, meter, longitude, meterSample);
            y = CubicSpline.Compute(mPointNum, meter, latitude, meterSample);
            z = CubicSpline.Compute(mPointNum, meter, altitude, meterSample);
            dir = CubicSpline.Compute(mPointNum, meter, heading, meterSample);

        }


        //
        // 获取线路的一部分，输入参数，起始终止里程与采样间隔， 输出参数，线路上的经纬高度坐标数组        
        //
        public int getSubLine(double startm, double endm, double stepm, out double[] x, out double[] y, out double[] z, out double[] dir)
        {
            x = y = z = dir = null;
            startm = startm - mStart;
            endm = endm - mStart;
            if (mIsReverse)
            {
                startm = -startm;
                endm = -endm;
            }

            if (startm < 0) startm = 0;
            if (endm > mLength) endm = mLength;

            if (startm >= endm)
            {
                double temp = startm;
                startm = endm;
                endm = temp;
                //return 0;
            }

            double dis = endm - startm;
            //double dir;
            stepm = Math.Abs(stepm);


            int count = (int)((dis - 0.05) / stepm) + 2;
            double m;

            x = new double[count];
            y = new double[count];
            z = new double[count];
            dir = new double[count];


            m = startm;
            for (int i = 0; i < count - 1; i++)
            {
                getPosbyLocalMeter(m, out x[i], out y[i], out z[i], out dir[i]);
                m += stepm;
            }
            getPosbyLocalMeter(endm, out x[count - 1], out y[count - 1], out z[count - 1], out dir[count - 1]);

            return count;
        }


        // 给定里程，计算经纬度朝向坐标
        private bool getPosbyLocalMeter(double m, out double x, out double y, out double z, out double dir)
        {
            bool flag = true;
            x = y = z = 0; dir = 0;

            if (m < 0)
            {
                flag = false;
                x = longitude[0];
                y = latitude[0];
                z = altitude[0];
                if (heading != null)
                    dir = heading[0];
            }
            else if (m > mLength)
            {
                flag = false;
                x = longitude[mPointNum - 1];
                y = latitude[mPointNum - 1];
                z = altitude[mPointNum - 1];
                if (heading != null)
                    dir = heading[mPointNum - 1];
            }

            else
            {
                flag = true;
                int index = (int)(m / 10);


                if (index == mPointNum - 1)
                {
                    x = longitude[index];
                    y = latitude[index];
                    z = altitude[index];
                    if (heading != null)
                        dir = heading[index];
                }
                else
                {
                    double t = m / 10 - index;
                    x = longitude[index] * (1 - t) + longitude[index + 1] * t;
                    y = latitude[index] * (1 - t) + latitude[index + 1] * t;
                    z = altitude[index] * (1 - t) + altitude[index + 1] * t;
                    if (heading != null)
                        dir = heading[index] * (1 - t) + heading[index + 1] * t;
                }
            }
            return flag;
        }


        // 给定里程，计算经纬度朝向坐标
        public void getPosbyMeter(double m, out double x, out double y, out double z, out double dir)
        {
            x = y = z = 0; dir = 0;

            bool isFind;
            double dis = m - mStart;
            if (mIsReverse)
                dis = -dis;

            isFind = getPosbyLocalMeter(dis, out x, out y, out z, out dir);

        }

        //// 给定里程，计算经纬度朝向坐标
        //public CRWPosition getPosbyMeter(double m)
        //{
        //    CRWPosition pos = new CRWPosition();
        //    int index = 0;
        //    //x = y = z = 0; dir = 0;
        //    if (m < start)
        //    {
        //        pos.longitude = longitude[0];
        //        pos.latitude = latitude[0];
        //        pos.altitude = altitude[0];
        //        if (heading != null)
        //            pos.heading = heading[0];
        //        return pos;
        //    }
        //    if (m >= end)
        //    {
        //        pos.longitude = longitude[mPointNum - 1];
        //        pos.latitude = latitude[mPointNum - 1];
        //        pos.altitude = altitude[mPointNum - 1];
        //        if (heading != null)
        //            pos.heading = heading[mPointNum - 1];
        //        return pos;
        //    }
        //    int nm = (int)m;
        //    if (nm % 10 == 9)
        //    {
        //        index = nm / 10 + 1;
        //        pos.longitude = longitude[index];
        //        pos.latitude = latitude[index];
        //        pos.altitude = altitude[index];
        //        if (heading != null)
        //            pos.heading = heading[index];
        //    }
        //    else
        //    {
        //        index = nm / 10;
        //        if (nm % 10 == 0)
        //        {
        //            pos.longitude = longitude[index];
        //            pos.latitude = latitude[index];
        //            pos.altitude = altitude[index];
        //            if (heading != null)
        //                pos.heading = heading[index];
        //        }
        //        else
        //        {
        //            double t = (m - nm) / 10;
        //            pos.longitude = longitude[index] * (1 - t) + longitude[index + 1] * t;
        //            pos.latitude = latitude[index] * (1 - t) + latitude[index + 1] * t;
        //            pos.altitude = altitude[index] * (1 - t) + altitude[index + 1] * t;
        //            if (heading != null)
        //                pos.heading = heading[index] * (1 - t) + heading[index + 1] * t;
        //        }
        //    }
        //    return pos;
        //}

        /// <summary>
        /// 给定经纬度，输出里程与距离
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mileage"></param>
        /// <param name="distance"></param>
        public void getMeterbyPos(double x, double y, out double mileage, out double distance)
        {
            int errorNum = 0;
            double ux, uy;
            
            CoordinateConverter.LatLonToUTMXY(y, x, out ux, out uy);
            distance = 0;
            mileage = 0;
            try
            {
                mileage = 0;

                int count = mPointNum;
                double mindist = (utmX[0] - ux) * (utmX[0] - ux) +
                    (utmY[0] - uy) * (utmY[0] - uy);
                double dist = 10;
                int step = (int)Math.Sqrt(count) + 1;
                //int i;
                int index = 0;
                for (int i = step; i < count; i += step)
                {
                    dist = (utmX[i] - ux) * (utmX[i] - ux) + (utmY[i] - uy) * (utmY[i] - uy);
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
                for (; j < index + step && j < count; j++)
                {
                    dist = (utmX[j] - ux) * (utmX[j] - ux) + (utmY[j] - uy) * (utmY[j] - uy);
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
                    d1 = Math.Sqrt(Math.Pow(utmX[index2 - 1] - ux, 2) + Math.Pow(utmY[index2 - 1] - uy, 2));
                    d2 = Math.Sqrt(Math.Pow(utmX[index2 + 1] - ux, 2) + Math.Pow(utmY[index2 + 1] - uy, 2));
                    mileage = meter[index2 - 1] + (meter[index2 + 1] - meter[index2 - 1]) * d1 / (d1 + d2);
                }
                else if (index2 == 0)
                {
                    double d1, d2;
                    d1 = Math.Sqrt(Math.Pow(utmX[0] - ux, 2) + Math.Pow(utmY[0] - uy, 2));
                    d2 = Math.Sqrt(Math.Pow(utmX[1] - ux, 2) + Math.Pow(utmY[1] - uy, 2));
                    mileage = (meter[1] - meter[0]) * d1 / (d1 + d2);
                }
                else
                {
                    double d1, d2;
                    d1 = Math.Sqrt(Math.Pow(utmX[count - 2] - ux, 2) + Math.Pow(utmY[count - 2] - uy, 2));
                    d2 = Math.Sqrt(Math.Pow(utmX[count - 1] - ux, 2) + Math.Pow(utmY[count - 1] - uy, 2));
                    mileage = meter[count - 2] + (meter[count - 1] - meter[count - 2]) * d1 / (d1 + d2);
                }
                double mx,my,mz,md;
                getPosbyLocalMeter(mileage, out mx, out my, out mz, out md);
                if (mIsReverse)
                    mileage = mStart - mileage;
                else
                    mileage = mStart + mileage;
                distance = CoordinateConverter.getUTMDistance(mx, my, x, y);
                //return mileage;
            }
            catch (Exception e)
            {
                Console.WriteLine("error num" + errorNum);
                //return -1;
            }
        }
    }
}
