using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInfo
{
    // 单位工程, 目前要求起止里程都在一个DKCode中
    public class CRailwayDWProj
    {
        public CRailwayProject mParentProj;
        
        private string mDWName;

        public string DWName
        {
            get { return mDWName; }
            set { mDWName = value; }
        }
        private string mDWType;

        public string DWType
        {
            get { return mDWType; }
            set { mDWType = value; }
        }

        private double mMileage_Start;

        public double Mileage_Start
        {
            get { return mMileage_Start; }
            set { mMileage_Start = value; }
        }
        private string mDKCode_Start;

        public string DKCode_Start
        {
            get { return mDKCode_Start; }
            set { mDKCode_Start = value; }
        }
        //public double mMileage_End;  // 起止里程，中点里程
        //public string mDKCode_End;
        public double mLength;  // 工点长度

        public double mLongitude_Mid;
        public double mLatitude_Mid;
        public double mAltitude_Mid;  // 中点经纬度，高度
        public double mHeading_Mid;

        public bool mIsValid;

        private DateTime mUpdateTime; // 更新时间

        public DateTime UpdateTime
        {
            get { return mUpdateTime; }
            set { mUpdateTime = value; }
        }
        private double mAvgProgress; // 进度     

        public double AvgProgress
        {
            get { return mAvgProgress; }
            set { mAvgProgress = value; }
        }

        public string mSerialNo;  // 序列号编码

        public double mGlobalLength;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pp"></param>
        /// <param name="SerialNo"></param>
        /// <param name="name"></param>
        /// <param name="dwtype"></param>
        /// <param name="startM"></param>
        /// <param name="length"></param>
        /// <param name="dt"></param>
        /// <param name="AvgProgress"></param>
        /// <param name="dkcode"></param>
        public CRailwayDWProj(CRailwayProject pp, string SerialNo, string name, string dwtype, double startM, double length, DateTime dt, double AvgProgress, string dkcode = "DK")
        {
            mParentProj = pp;
            mDWName = name;
            mDWType = dwtype;
            mSerialNo = SerialNo;

            mMileage_Start = startM;
            mDKCode_Start = dkcode;

            mLength = length;

            mUpdateTime = dt;
            mAvgProgress = AvgProgress;

            mIsValid = mParentProj.mScene.mMiddleLines.getPosbAndLengthByDKCode(dkcode, mMileage_Start,
                out mLongitude_Mid, out mLatitude_Mid, out mAltitude_Mid, out mHeading_Mid, out mGlobalLength);

        }

    }

    public class CRailwayPier : CRailwayDWProj
    {
        public CRailwayPier(CRailwayProject pp, string SerialNo, string name, string dwtype, double startM, double endM, DateTime dt, double AvgProgress, string dkcode )
            : base(pp, SerialNo, name, dwtype, startM, endM, dt, AvgProgress, dkcode = "DK")
        {

        }
    }

    public class CRailwayBeam : CRailwayDWProj
    {
        public CRailwayBeam(CRailwayProject pp, string SerialNo, string name, string dwtype, double startM, double endM, DateTime dt, double AvgProgress, string dkcode)
            : base(pp, SerialNo, name, dwtype, startM, endM, dt, AvgProgress, dkcode = "DK")
        {

        }
    }
}
