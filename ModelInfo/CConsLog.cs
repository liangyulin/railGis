using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using ModelInfo.Helper;
//using lib_GIS.Service;

namespace ModelInfo
{
    /// <summary>
    /// 施工日志，或者周志，记录最近施工情况
    /// </summary>
    /// Sample
    /// <UsrID>105</UsrID>
    //<UsrName>高世超</UsrName>
    //<ConsDate>2016-03-11T10:10:57+08:00</ConsDate>
    //<Longitude>117.12375700</Longitude>
    //<Latitude>36.68019100</Latitude>
    //<ProjectID>56</ProjectID>
    //<ProjectName>济南特大桥8#～13#墩（18+3*24+18）</ProjectName>
    //<DwName>8#墩</DwName>
    
    //public class CConsRecord{
    //    string usrName;
    //    string consDate;
    //    decimal longitude;
    //    decimal latitude;
    //    string projName;
    //}

    public class ConsLocation 
    {
        private double latitude;

        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }
        private double longitude;

        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }
        private string projName;

        public string ProjName
        {
            get { return projName; }
            set { projName = value; }
        }
        private string projDWName;

        public string ProjDWName
        {
            get { return projDWName; }
            set { projDWName = value; }
        }
        private string fromD;

        public string FromDate
        {
            get { return fromD; }
            set { fromD = value; }
        }
        private string toD;

        public string ToDate
        {
            get { return toD; }
            set { toD = value; }
        }
        //public string day;
        //public string usrName;
        public int number;
        public ConsLocation(double x,double y) {
            longitude = x;
            latitude = y;
        }
        public ConsLocation(string pn, string pdwn, double x, double y, string fd, string td, int n):this(x, y) {
            projName = pn;
            projDWName = pdwn;
            fromD = fd;
            toD = td;
            number = n;
            
        
        }
        public override string ToString()
        {
            string result = "日期：" + fromD + " 至 " + toD + "\n";
            //result += usrName + "\n";
            result += projName + "\n" + projDWName + "\n";
            result += number + " 人次 ";
            return result;
        }
    }
    /// <summary>
    /// TODO 丁一明 聚类算法建议放在Helper中，此处利用find数据，聚类结果在CTECons中显示
    /// </summary>
    public class CConsLog 
    {
        //public List<CConsRecord> consList = new List<CConsRecord>();
        /// <summary>
        /// 返回当天，最近3天，7天，30天，365天情况，注意日期格式
        /// </summary>
        /// <param name="usrName"></param>
        /// <param name="projName"></param>
        /// <param name="consDate"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        //public static int findTodayCons(out string[] usrName, out string[] projName, out string[] consDate, out double[] longitude, out double[] latitude)
        //{
        //    return findConsFromDate(DateTime.Now.Date.ToString("u"),out usrName,out projName,out consDate, out longitude, out latitude);
        //}

        //public static int findLast3Cons(out string[] usrName, out string[] projName, out string[] consDate, out double[] longitude, out double[] latitude)
        //{
        //    return findConsFromDate(DateTime.Now.AddDays(-3).Date.ToString("u"), out usrName, out projName, out consDate, out longitude, out latitude);
        //}

        public static int findLast7Cons(out string[] usrName, out string[] projName, out string[] projDWName,  out double[] longitude, out double[] latitude)
        {
            return findConsFromDate(DateTime.Now.AddDays(-7).Date.ToString("u"), out usrName, out projName, out projDWName,  out longitude, out latitude);
        }

        //public static int findLast30Cons(out string[] usrName, out string[] projName, out string[] consDate, out double[] longitude, out double[] latitude)
        //{
        //    return findConsFromDate(DateTime.Now.AddDays(-30).Date.ToString("u"), out usrName, out projName, out consDate, out longitude, out latitude);
        //}

        //public static int findLast365Cons(out string[] usrName, out string[] projName, out string[] consDate, out double[] longitude, out double[] latitude)
        //{
        //    return findConsFromDate(DateTime.Now.AddDays(-365).Date.ToString("u"), out usrName, out projName, out consDate, out longitude, out latitude);
        //}


        private static int findConsFromDate(string date, out string[] usrName, out string[] projName, out string[] projDWName, out double[] longitude, out double[] latitude)
        {
            int num = 0;
            usrName = projName = projDWName= null;
            longitude = latitude = null;

            //DataTable dt = CServerWrapper.findConsInfo(DateTime.Now.AddDays(-30).Date.ToString("u"));
            DataTable dt = CServerWrapper.findConsInfo(date);
            //DatabaseWrapper.PrintDataTable(dt);
            num = dt.Rows.Count;

            if (num == 0) return 0;

            usrName = new string[num];
            projName = new string[num];
            projDWName = new string[num];
            longitude = new double[num];
            latitude = new double[num];

            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                usrName[i] = dr["UsrName"].ToString();
                projName[i] = dr["ProjectName"].ToString();
                projDWName[i] = dr["DwName"].ToString();
                longitude[i] = Convert.ToDouble(dr["Longitude"]);
                latitude[i] = Convert.ToDouble(dr["Latitude"]);
                i++;


            }
            return num;
        }

        /// <summary>
        /// 如果绑定控件，DataTable方式可以减少数据复制，但本应用需要对数据做聚类处理，需要用上面方法做数据的初步解析
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static DataTable findConsFromDate(string date)
        {


            //DataTable dt = CServerWrapper.findConsInfo(DateTime.Now.AddDays(-30).Date.ToString("u"));
            DataTable dt = CServerWrapper.findConsInfo(date);

            return dt;
        }


        /// <summary>
        /// 按照工点聚类，单位工程显示第一个
        /// </summary>
        /// <returns></returns>
        public static List<ConsLocation> clusterConsByProj() {
            int numP;
            double[] px,py;
            string[] usrName;
            string[] projName;
            string[] projDWName;
            List<ConsLocation> ls = new List<ConsLocation>();
            int cn = 0;
            //double ax, ay;
            string cp = null;
            string fromD = DateTime.Now.AddDays(-7).ToShortDateString();
            string toD = DateTime.Now.ToShortDateString();

            //List<double> ax = new List<double>();
            //List<double> ay = new List<double>();

            numP = findLast7Cons(out usrName, out projName, out projDWName, out px, out py);
            //cp = projName[0];
            for (int i = 0; i < numP; i++) {
                if (px[i] < 100 || py[i] < 20) continue;
                if (!projName[i].Equals(cp))  // 新工点
                {
                    cp = projName[i];
                    cn = 1;
                    ls.Add(new ConsLocation(px[i],py[i]));
                    ls[ls.Count - 1].number = cn;
                    ls[ls.Count - 1].ProjName = cp;
                    ls[ls.Count - 1].FromDate = fromD;
                    ls[ls.Count - 1].ToDate = toD;
                    ls[ls.Count - 1].ProjDWName = projDWName[i];
                    //if (cp.Equals("邹平梁场"))
                    //    Console.WriteLine(ls.Last().ToString());
                    
                }
                else  // 已存在工点
                {
                    //if (cp.Equals("临淄梁场") )
                    //    Console.WriteLine(usrName[i] + "\t" + projName[i] + "\t" + projDWName[i] + "\t" + px[i] + "\t" + py[i]);
                    ls[ls.Count - 1].Longitude = (ls[ls.Count - 1].Longitude * cn + px[i]) / (cn + 1);
                    ls[ls.Count - 1].Latitude = (ls[ls.Count - 1].Latitude * cn + py[i]) / (cn + 1);
                    cn++;
                    ls[ls.Count - 1].number = cn;
                }

            }
            //for (int i = 0; i < ax.Count; i++) {
            //    Console.WriteLine("x: {0}\t y {1}", ax[i], ay[i]);
            //}

            return ls;
           
            //new StaticCluster().clusterProcess();
        }

        /// <summary>
        /// 按照工点聚类，单位工程显示第一个
        /// </summary>
        /// <returns></returns>
        public static List<ConsLocation> clusterConsFromWebByProj(string fromDate = null, string toDate = null)
        {

            List<ConsLocation> ls = new List<ConsLocation>();
            double cx, cy;

            //string fromD = DateTime.Now.AddDays(-7).ToShortDateString();
            //string toD = DateTime.Now.ToShortDateString();

        //dt2.Columns.Add("ProjectName", typeof(string));
        //dt2.Columns.Add("DwName", typeof(string));
        //dt2.Columns.Add("Longitude", typeof(double));
        //dt2.Columns.Add("Latitude", typeof(double));
        //dt2.Columns.Add("StaffNum", typeof(int));
            // 2016-05-06 00:00:00Z
            //DataTable dt = CServerWrapper.findClusterConsByPDW(DateTime.Now.AddDays(-7).Date.ToString("u"));
            //DataTable dt = CServerWrapper.findClusterConsByPDW(fromDate, toDate);
            DataTable dt = CServerWrapper.findClusterConsByProj(fromDate, toDate); 
            foreach (DataRow dr in dt.Rows) {
                GPSAdjust.bd_decrypt(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), out cy, out cx);
                ls.Add(new ConsLocation(dr["ProjectName"].ToString(), dr["DwName"].ToString(), cx, cy,
                    fromDate.Substring(0,11), toDate.Substring(0,11), Convert.ToInt32(dr["StaffNum"])));
    //            ls.Add(new ConsLocation(dr["ProjectName"].ToString(), dr["DwName"].ToString(), Convert.ToDouble(dr["Longitude"]), Convert.ToDouble(dr["Latitude"]),
    //fromDate.Substring(0, 11), toDate.Substring(0, 11), Convert.ToInt32(dr["StaffNum"])));
            }

            return ls;

            //new StaticCluster().clusterProcess();
        }
    }
}
