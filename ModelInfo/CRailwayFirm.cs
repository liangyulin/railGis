using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInfo
{
    public class CRailwayFirm : IHotSpot
    {
        public CRailwayScene mScene;
        public CRailwayLine mLine= null;

        //public String mFirmType { get; set; } 
        
        private string mFirmType; //单位类型  :

        public string FirmType
        {
            get { return mFirmType; }
            set { mFirmType = value; }
        }

        public int mFirmID; // 单位编号
        private string mFirmName;  // 单位名称

        public string FirmName
        {
            get { return mFirmName; }
            set { mFirmName = value; }
        }

        private int mNumStaff; // 实名制人数

        public int NumStaff
        {
            get { return mNumStaff; }
            set { mNumStaff = value; }
        }

        //public string mSegmentName;  // 所属标段

        //public DateTime mUpdateTime; // 更新时间
        //public double mAvgProgress; // 进度      

        // 空间位置信息
        private double mLongitude_Mid;

        public double CenterLongitude
        {
            get { return mLongitude_Mid; }
            set { mLongitude_Mid = value; }
        }
        private double mLatitude_Mid;

        public double CenterLatitude
        {
            get { return mLatitude_Mid; }
            set { mLatitude_Mid = value; }
        }
        //public double mAltitude_Mid;  // 中点经纬度，高度

        //public string mSNo;

        public string mLabelImage;  //文件名
        private string mDKCode;

        public string DKCode
        {
            get { return mDKCode; }
            set { mDKCode = value; }
        }
        private double mMileage_Mid;

        public double CenterMileage
        {
            get { return mMileage_Mid; }
            set { mMileage_Mid = value; }
        }

        private double mGlobalMileage;
        public double GlobalMileage
        {
            get { return mGlobalMileage; }
            set { mGlobalMileage = value; }
        }

        public double mDis;

        public string ShowMessage { get; set; }

        //public string mSerialNo;  // 序列号编码

        //            string sqlstr = @"select FirmName ,a.FirmTypeID, CategoryCode, SerialNo, UpdateTime, Longitude, Latitude from (select * from FirmInfo)a, (select FirmTypeID,FirmTypeCategoryName from FirmTypeInfo)b where a.FirmTypeID=b.FirmTypeID and( FirmTypeCategoryName='单位' or FirmTypeCategoryName='分支机构') and Longitude > 10 AND Latitude > 10 order by a.FirmTypeID asc ;"; 

        //this, Convert.ToInt32(dataReader["Num"]), Convert.ToInt32(dataReader["firmid"]), dataReader["ShorName"].ToString(),
        //                Convert.ToDouble(dataReader["Longitude"]), Convert.ToDouble(dataReader["Latitude"]), firmType, fileName

        public CRailwayFirm(CRailwayScene s, int num, int fid, string fname,  double x, double y,string firmType, string fileName)
        {
            mScene = s;
            mFirmType = firmType;
            mFirmName = fname;

            mNumStaff = num;
            mFirmID = fid;
            //mSNo = sNo;
            //mUpdateTime = dt;

            mLongitude_Mid = x;
            mLatitude_Mid = y;

            if (fname.StartsWith("11标"))
                Console.WriteLine(fname + x + "\t" + y);
            mLine = mScene.mMiddleLines.getMeterbyPos(x, y, out mMileage_Mid, out mDis  );
            mDKCode = mLine.mDKCode;
            mGlobalMileage = mScene.mMiddleLines.getGlobalMeter(mLine.mDKCode, mMileage_Mid);
       //     mScene.mMiddleLines.get

            mLabelImage = fileName;


        }

        public override string ToString()
        {
            string res = this.mFirmName + "\n实名制注册人数" + this.mNumStaff;
            return res;
            //return base.ToString();
        }


    }
}
