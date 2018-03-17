using System;
using System.Collections.Generic;
using System.ComponentModel; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.OleDb;
using System.Data;

//using Microsoft.Office.Interop.Excel;

//using TerraExplorerX;

using System.Drawing;


namespace ModelInfo
{
    public class CRWPosition
    {
        public double meter;
        public double latitude;
        public double longitude;
        public double altitude;
        public double heading;
        public double pitching;
        public double utmX;
        public double utmY;
    }


    //public class CRailwayStation
    //{
    //    public string mStationName;
    //    public double mX;
    //    public double mY;
    //    public double mZ;
    //    public double mR = 0.001;
    //    public double mHeight = 100;
    //    //public ITerrainBuilding66 mModel;
    //    public CRailwayStation(string name, double x, double y, double z)
    //    {
    //        mStationName = name;
    //        mX = x;
    //        mY = y;
    //        mZ = z;
    //    }
    //}

    /// <summary>
    /// 分项工程
    /// </summary>
    public class CFXProj{
        public int fxID;
        public string fxName;
        public double totalAmount;
        public Dictionary<string,double> progress;
        public override string ToString()
        {
            return fxName + "\t" + totalAmount + "\t" + progress.Keys.First() + "\t" + progress.Values.First()  ;
        }

    }


    // 工点
    public class CRailwayProject :IHotSpot{

        public CRailwayScene mScene;

        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("02工点类型")] 
        public String ProfessionalName { get; set; }  //工点类型  :桥，路基，涵洞，站点，其他
        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("显示信息"), Browsable(false)]
        public string ShowMessage{ get; set; }
        
        private string mProjectName;  // 工点名称
        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("01工点名称")] 
        public string ProjectName
        {
            get { return mProjectName; }
            set { mProjectName = value; }
        }

        private string mSegmentName;  // 所属标段
        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("03所属标段")] 
        public string SegmentName
        {
            get { return mSegmentName; }
            set { mSegmentName = value; }
        }


        private DateTime mUpdateTime; // 更新时间
        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("04更新时间"), Browsable(false)] 
        public DateTime UpdateTime
        {
            get { return mUpdateTime; }
            set { mUpdateTime = value; }
        }

        private double mAvgProgress; // 进度      
        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("08整体进度")] 
        public double AvgProgress
        {
            get { return mAvgProgress; }
            set { mAvgProgress = value; }
        }
        
        // 空间位置信息
        private double mLongitude_Mid;

        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("中心点经度"), Browsable(false)]
        public double CenterLongitude
        {
            get { return mLongitude_Mid; }
            set { mLongitude_Mid = value; }
        }


        private double mLatitude_Mid;
        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("中心点纬度"), Browsable(false)]
        public double CenterLatitude
        {
            get { return mLatitude_Mid; }
            set { mLatitude_Mid = value; }
        }
        public double mAltitude_Mid;  // 中点经纬度，高度
        public double mHeading_Mid;

        public double mLongitude_Start;
        public double mLatitude_Start;
        public double mAltitude_Start;  // 起点经纬度，高度
        public double mHeading_Start;

        public double mLongitude_End;
        public double mLatitude_End;
        public double mAltitude_End;  // 终点经纬度，高度
        public double mHeading_End;


        private string Mileage_Start_Ds;
        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("05起始里程")] 
        public string Mileage_Start_Discription
        {
            get { return Mileage_Start_Ds; }
            set { Mileage_Start_Ds = value; }
        }
        //public string Mileage_Mid_Ds;

        private string Mileage_End_Ds;

        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("06终止里程")] 
        public string Mileage_End_Discription
        {
            get { return Mileage_End_Ds; }
            set { Mileage_End_Ds = value; }
        }

        private string mDKCode;

        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("里程代码"), Browsable(false)] 
        public string DKCode
        {
            get { return mDKCode; }
            set { mDKCode = value; }
        }


        public double mMileage_Start;
        private double mMileage_Mid;

        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("中心里程"), Browsable(false)] 
        public double CenterMileage
        {
            get { return mMileage_Mid; }
            set { mMileage_Mid = value; }
        }    
        public double mMileage_End;  // 起止里程，中点里程

        //public string mDKCode_Mid;
        //public string mDKCode_End;

        private double mLength;  // 工点长度
        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("07工点长度")]
        public double Length
        {
            get { return mLength; }
            set { mLength = value; }
        }

        public bool mIsValid;
        
        //public string mMileage_Start_Des;
        //public string mMileage_Mid_Des;
        //public string mMileage_End_Des;  // 起始里程文字描述

        public double mDirection;  // 朝向


        //public int mProgressCount;

        
        public string mLabelImage;  //文件名
        
        //public ITerrainLabel66 mLabel; //标签
        //public ITerrainPolyline66 mPolyline; //中线        
        //public CTEProject mTEProject; // 模型，目前没有关联

        public string mSerialNo;  // 序列号编码
        public int mProjectID;

        public List<CRailwayDWProj> mDWProjList = new List<CRailwayDWProj>();

        public List<CFXProj> mfx = new List<CFXProj>();

        public DataSet mds = null;
        private double mGlobalMileage = 0;

        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("全局里程"), Browsable(false)] 
        public double GlobalMileage
        {
            get { return mGlobalMileage; }
            set { mGlobalMileage = value; }
        }
        

        /// <summary>
        /// 利用dkcode 与 里程初始化，潜在问题，要求在同一个链中，dkcode一致，目前数据库中的里程描述description不正确
        /// </summary>
        /// <param name="s"></param>
        /// <param name="SerialNo"></param>
        /// <param name="projID"></param>
        /// <param name="profName"></param>
        /// <param name="ProjectName"></param>
        /// <param name="SegmentName"></param>
        /// <param name="DKCode"></param>
        /// <param name="Mileage_Start"></param>
        /// <param name="Mileage_Mid"></param>
        /// <param name="Mileage_End"></param>
        /// <param name="dt"></param>
        /// <param name="AvgProgress"></param>
        /// <param name="dir"></param>
        /// <param name="labelFile"></param>
        /// <param name="length"></param>
        public CRailwayProject(CRailwayScene s, string SerialNo, int projID, string profName, string ProjectName, string SegmentName,string DKCode,
            double Mileage_Start, double Mileage_Mid, double Mileage_End, 
            //string Mileage_Start_Des, string Mileage_Mid_Des, string Mileage_End_Des, 
            DateTime dt, double AvgProgress, double dir, string labelFile)
        {
            
            mScene = s;
            ProfessionalName = profName;
            mProjectID = projID;
            mProjectName = ProjectName;
            mSegmentName = SegmentName;
            mSerialNo = SerialNo;
            mDKCode = DKCode;

            Mileage_Start_Ds = CRailwayLineList.CombiDKCode(DKCode,Mileage_Start);
            //Mileage_Mid_Ds = Mileage_Mid_Des;
            Mileage_End_Ds = CRailwayLineList.CombiDKCode(DKCode, Mileage_End); ;

            ///Doing
            //DataSet ds = CServerWrapper.findProjHistory(mSerialNo);

            //if (ds != null)
            //{
            //    PrintDataTable(ds.Tables[0]);
            //    PrintDataTable(ds.Tables[1]);
            //}
            //else
            //{
            //    Console.WriteLine("None Data " + ProjectName);
            //}

            ///FIXME, 描述在数据库中不准确，待修正
            double tmp;

            //mScene.mMiddleLines.parseDKCode(Mileage_Start_Des, out mDKCode_Start, out tmp);
            //mScene.mMiddleLines.parseDKCode(Mileage_Mid_Des, out mDKCode_Mid, out tmp);
            //mScene.mMiddleLines.parseDKCode(Mileage_End_Des, out mDKCode_End, out tmp);

            mMileage_Start = Mileage_Start;
            mMileage_Mid = Mileage_Mid;
            mMileage_End = Mileage_End;

            mIsValid = mScene.mMiddleLines.getPosbAndLengthByDKCode(mDKCode, mMileage_Mid, out mLongitude_Mid, out mLatitude_Mid, out mAltitude_Mid, out mHeading_Mid, out tmp);
            mIsValid &= mScene.mMiddleLines.getPosbAndLengthByDKCode(mDKCode, mMileage_Start, out mLongitude_Start, out mLatitude_Start, out mAltitude_Start, out mHeading_Start, out mGlobalMileage);
            mIsValid &= mScene.mMiddleLines.getPosbAndLengthByDKCode(mDKCode, mMileage_End, out mLongitude_End, out mLatitude_End, out mAltitude_End, out mHeading_End, out tmp);

            mUpdateTime = dt;
          
            mDirection = dir;
            mLabelImage = labelFile;
            mLength = Math.Abs(mMileage_End - mMileage_Start); //FIXME 目前是导入，不同DKCode的线路如何求解
            mds = CServerWrapper.findProjHistory(mSerialNo);
            getFXProgress();
        }

        /// <summary>
        /// 获取分项进度
        /// </summary>
        /// <returns>返回值：分项名称，分项进度 的队列</returns>
        public Dictionary<string, double> getFXProgress()
        {
            Dictionary<string, double> fxlist = new Dictionary<string, double>();
            int fid;
            int n = 0;
            double avgProgress = 0;
            foreach (DataRow dr in mds.Tables[0].Rows)
            {
                fid = Convert.ToInt32(dr["ProjectDictID"]);
                DataRow[] drs = mds.Tables[1].Select("ProjectDictID = " + fid ,  "ReportDate desc");
                if (drs.Count() > 0) {
                    fxlist.Add(dr["ProjectDictName"].ToString(),Convert.ToDouble(drs[0]["DictRate"]));
                    avgProgress += Convert.ToDouble(drs[0]["DictRate"]);
                }
                else{
                    fxlist.Add(dr["ProjectDictName"].ToString(),0);
                }
                n++;

            }
            if (n > 0)
            {
                avgProgress /= n;
                mAvgProgress = avgProgress;
            }
            else
                mAvgProgress = 0;

            return fxlist;
        }


/// <summary>
/// 获取截至某日的分项进度
/// </summary>
        /// <param name="date">日期格式 20160511, 典型的使用方法 getFXProgressByDate(DateTime.Now.AddDays(-1).ToString("yyyyMMdd"));</param>
/// <returns>返回值：分项名称，分项进度 的队列</returns>
        public Dictionary<string, double> getFXProgressByDate(DateTime toDate)
        {
            Dictionary<string, double> fxlist = new Dictionary<string, double>();
            int fid;
            int n = 0;
            double avgProgress = 0;

            foreach (DataRow dr in mds.Tables[0].Rows)
            {
                fid = Convert.ToInt32(dr["ProjectDictID"]);
                DataRow[] drs = mds.Tables[1].Select(@"ProjectDictID = " + fid + @" AND ReportDate < '" + toDate.ToString("yyyyMMdd") + @"'" , "ReportDate desc");
                if (drs.Count() > 0)
                {
                    fxlist.Add(dr["ProjectDictName"].ToString(), Convert.ToDouble(drs[0]["DictRate"]));
                    avgProgress += Convert.ToDouble(drs[0]["DictRate"]);
                    //Console.WriteLine(dr["ProjectDictName"].ToString() + "\t" + Convert.ToDouble(drs[0]["DictRate"]));
                }
                else
                {
                    fxlist.Add(dr["ProjectDictName"].ToString(), 0);
                }
                n++;
                
            }
            if (n > 0)
            {
                avgProgress /= n;
                mAvgProgress = avgProgress;
            }
            else
                mAvgProgress = 0;
            return fxlist;
        }

        /// <summary>
        /// 工点的获取line方法
        /// </summary>
        /// <param name="stepm"></param>
        /// <param name="cVerticesArray"></param>
        /// <returns></returns>
        public int getSubLine(double stepm, out double[] cVerticesArray)
        {
            cVerticesArray = null;
            double[] x;
            double[] y;
            double[] z;
            double[] d;
            //CRailwayLineList oo=null;
            int pointNum = mScene.mMiddleLines.getSubLineByDKCode(Mileage_Start_Ds, Mileage_End_Ds, 1, out x, out y, out z, out d);
            if (pointNum > 0)
            {
                cVerticesArray = new double[pointNum * 3];

                for (int i = 0; i < pointNum; i++)
                {
                    cVerticesArray[3 * i] = x[i];
                    cVerticesArray[3 * i + 1] = y[i];
                    cVerticesArray[3 * i + 2] = z[i] + 10;
                }
            }
            return pointNum;
        }

        /// <summary>
        /// 工点的获取偏移方法
        /// </summary>
        /// <param name="stepm"></param>
        /// <param name="angleOff"></param>
        /// <param name="disOff"></param>
        /// <param name="cVerticesArray"></param>
        /// <returns></returns>
        public int getOffsetLine(double stepm, double angleOff, double disOff, out double[] cVerticesArray)
        {
            cVerticesArray = null;
            double[] latout;
            double[] lonout;
            double[] height;

            int pointNum = mScene.mMiddleLines.getOffsetLine(Mileage_Start_Ds, Mileage_End_Ds, stepm, angleOff, disOff, out  latout, out  lonout, out height);
            if (pointNum > 0)
            {
                cVerticesArray = new double[pointNum * 3];

                for (int i = 0; i < pointNum; i++)
                {
                    cVerticesArray[3 * i] = lonout[i];
                    cVerticesArray[3 * i + 1] = latout[i];
                    cVerticesArray[3 * i + 2] = height[i] + 10;
                }
            }
            return pointNum;
        }


        public override string ToString()
        {
            return ProfessionalName + "\t" + mProjectName + "\t" + mMileage_Start + "\t" + mMileage_End + "\t" + mLongitude_Start + "\t" + mLatitude_Start + "\t" + mAltitude_Start;
        }



    }

    public class CRailwayBridge : CRailwayProject
    {
        public List<CRailwayPier> mPier = new List<CRailwayPier>();
        public List<CRailwayBeam> mBeam = new List<CRailwayBeam>();
        public CRailwayBridge(CRailwayScene s, string SerialNo, int projID, string ProfessionalName, string ProjectName, string SegmentName,string DKCode,
            double Mileage_Start, double Mileage_Mid, double Mileage_End,             
            DateTime dt, double AvgProgress, double dir, string labelFile)
            :base( s,  SerialNo,  projID,  ProfessionalName,  ProjectName,  SegmentName,DKCode,
             Mileage_Start,  Mileage_Mid,  Mileage_End,               
             dt,  AvgProgress,  dir,  labelFile  )
        {
            /// FIXME Doing
            ///

            //CFXProj fx;
            //foreach (DataRow dr in mds.Tables[0].Rows)
            //{
            //    fx = new CFXProj();
            //    fx.fxID = Convert.ToInt32(dr["ProjectDictID"]);
            //    fx.fxName = dr["ProjectDictName"].ToString();
            //    fx.totalAmount = Convert.ToDouble(dr["DesignNum"]);
            //    fx.progress = new Dictionary<string, double>();
            //    mfx.Add(fx);
            //    DataRow[] drs = mds.Tables[1].Select("ProjectDictID = " + fx.fxID, "ReportDate desc");
            //    foreach (DataRow dr2 in drs){
            //        try
            //        {
            //            fx.progress.Add(dr2["ReportDate"].ToString(), Convert.ToDouble(dr2["DictRate"]));
            //        }
            //        catch (Exception e) {
            //            Console.WriteLine(e.Message);
            //            Console.WriteLine(dr2["ReportDate"].ToString());
            //        }

            //    }
            //    //Console.WriteLine(fx.ToString());

            //}

            //if (ds != null)
            //{
            //    DatabaseWrapper.PrintDataTable(ds.Tables[0]);
            //    DatabaseWrapper.PrintDataTable(ds.Tables[1]);
            //}
            //else
            //{
            //    Console.WriteLine("None Data " + ProjectName);
            //}
        }
    }

    public class CRailwayRoad : CRailwayProject
    {
        public CRailwayRoad(CRailwayScene s, string SerialNo, int projID, string ProfessionalName, string ProjectName, string SegmentName,string DKCode,
            double Mileage_Start, double Mileage_Mid, double Mileage_End, 
            DateTime dt, double AvgProgress, double dir, string labelFile)
            : base(s, SerialNo, projID, ProfessionalName, ProjectName, SegmentName, DKCode,
             Mileage_Start, Mileage_Mid, Mileage_End,
             dt, AvgProgress, dir, labelFile)
        {

        }
    }

    public class CRailwayTunnel : CRailwayProject
    {
        public CRailwayTunnel(CRailwayScene s, string SerialNo, int projID, string ProfessionalName, string ProjectName, string SegmentName,string DKCode,
            double Mileage_Start, double Mileage_Mid, double Mileage_End, 
            DateTime dt, double AvgProgress, double dir, string labelFile)
            : base(s, SerialNo, projID, ProfessionalName, ProjectName, SegmentName, DKCode,
             Mileage_Start, Mileage_Mid, Mileage_End,
             dt, AvgProgress, dir, labelFile)
        {

        }

    }


    public class CContBeam : CRailwayProject
    {
        public CContBeam(CRailwayScene s, string SerialNo, int projID, string ProfessionalName, string ProjectName, string SegmentName, string DKCode,
            double Mileage_Start, double Mileage_Mid, double Mileage_End,
            DateTime dt, double AvgProgress, double dir, string labelFile)
            : base(s, SerialNo, projID, ProfessionalName, ProjectName, SegmentName, DKCode,
             Mileage_Start, Mileage_Mid, Mileage_End,
             dt, AvgProgress, dir, labelFile)
        {

        }

    }



}


