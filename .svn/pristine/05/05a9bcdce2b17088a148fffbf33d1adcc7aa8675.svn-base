using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    public class CTENavTrain 
    {
        CRailwayScene mSceneData;
        public ITerrainDynamicObject66 mDynamicTrain = null;
        string mGroupID;
        
        string mModelName=  CGisDataSettings.gDataPath  + @"Common\Models\Train\train.3ds";
        int mCount;
        SGWorld66 sgworld;

        //public event EventHandler OnFlyTo;
        public delegate void FlyToEnd();

        public void GotoNext(FlyToEnd f)  //引发事件方法
        {
            //EventHandler temp = OnFlyTo;  

            mCount--;
            Console.WriteLine("CountDown " + mCount);
            if (mCount <= 0 && mDynamicTrain != null)
            {
                //Console.WriteLine("clear train for restart");
                //sgworld.ProjectTree.DeleteItem(mGroupID);
                //mGroupID = sgworld.ProjectTree.CreateGroup("Train");
                mDynamicTrain.Pause = true;
                f();
                //if(temp != null)
                //    temp(this,new EventArgs());

            }

        }

        //public void mNavTrain_OnFlyTo()
        //{
        //    mNavIndex++;
        //    if (mNavIndex < gRWScene.mHotSpotList.Count)
        //    {
        //        sgworld.Navigate.FlyTo(sgworld.Creator.CreatePosition(gRWScene.mHotSpotList[mNavIndex].CenterLongitude, gRWScene.mHotSpotList[mNavIndex].CenterLatitude, 1000, AltitudeTypeCode.ATC_TERRAIN_RELATIVE), ActionCode.AC_FLYTO);
        //        timerNav.Start();
        //    }
        //    else
        //        timerNav.Stop();
        //}

        //public void GotoNext()  //引发事件方法
        //{
        //    //EventHandler temp = OnFlyTo;  

        //    mCount--;

        //    if (mCount <= 0 && mDynamicTrain != null)
        //    {
        //        sgworld.Creator.DeleteObject(mDynamicTrain.ID);
        //        mDynamicTrain = null;
        //        f();
        //        //if(temp != null)
        //        //    temp(this,new EventArgs());

        //    }

        //}

        public CTENavTrain(CRailwayScene s )            
        {
            mSceneData = s;
            sgworld = new SGWorld66();
            //sgworld.ProjectTree.CreateGroup("Train");
     
        }



        public void Starting(int hotSpotIndex)
        {
            if (hotSpotIndex < 0 || hotSpotIndex + 1 >= mSceneData.mHotSpotList.Count)
                return;
            double[] mx, my, mz, md;
            if (string.IsNullOrEmpty(mGroupID))
                mGroupID = sgworld.ProjectTree.CreateGroup("Train");
            else {
                Console.WriteLine("clear train for restart");
                sgworld.ProjectTree.DeleteItem(mGroupID);
                mGroupID = sgworld.ProjectTree.CreateGroup("Train");
            }
            mDynamicTrain = sgworld.Creator.CreateDynamicObject(0, DynamicMotionStyle.MOTION_MANUAL, DynamicObjectType.DYNAMIC_3D_MODEL,
                mModelName, 0.25, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, mGroupID, "Train");
            mCount = mSceneData.mMiddleLines.getSubLineByDKCode(mSceneData.mHotSpotList[hotSpotIndex].DKCode, mSceneData.mHotSpotList[hotSpotIndex].CenterMileage,
                        mSceneData.mHotSpotList[hotSpotIndex + 1].DKCode, mSceneData.mHotSpotList[hotSpotIndex+1].CenterMileage, 70, out mx, out my, out mz, out md);
            for (int i = 0; i < mCount; i++)
            {
                mDynamicTrain.Waypoints.AddWaypoint(sgworld.Creator.CreateRouteWaypoint(mx[i], my[i], mz[i], 100, md[i]));
            }
            Console.WriteLine("starting new journey from " + hotSpotIndex + "\t" + mSceneData.mHotSpotList[hotSpotIndex].DKCode + mSceneData.mHotSpotList[hotSpotIndex].CenterMileage + " to "
                + mSceneData.mHotSpotList[hotSpotIndex+1].DKCode + mSceneData.mHotSpotList[hotSpotIndex+1].CenterMileage);
            mDynamicTrain.Pause = false;
            sgworld.Navigate.FlyTo(mDynamicTrain);

            //mDynamicTrain.PlayRouteOnStartup = false;

        }


    }
}
