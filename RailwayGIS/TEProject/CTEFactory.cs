using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    public class CTEFactory : CTERWItem
    {
        public CTEFactory(CRailwayScene s, CTEScene ss, CRWTEStandard t)
            : base(s, ss, t)
        {

        }

        /// <summary>
        /// 以图片的方式显示梁厂，FIXME 应该定位于铁路一侧，不应该位于铁路上，参考车站CTEStation的实现
        /// </summary>
        /// <param name="groupID"></param>
        public override void TECreate()
        {
            var sgworld = new SGWorld66();
            if (string.IsNullOrEmpty(mGroupIDStatic))
                mGroupIDStatic = sgworld.ProjectTree.CreateGroup("Factory");
            double xs, ys, zs;
            //double[] p = new double[] { -1, 1, 0, 1, 1, 0, 1, -1, 0, -1, -1, 0 };
            IPosition66 cp;
            ITerrainVideo66 iVideo;
            //int count = 0;

            //var branch = sgworld.ProjectTree.CreateGroup("CCTV");
            //var vb = sgworld.ProjectTree.CreateGroup("Video");
            try
            {
                //string surl = CGisDataSettings.gDataPath; 
                //foreach (CRailwayProject cVideo in mSceneData.mOtherList)
                //{
                //    xs = cVideo.mLongitude_Mid;
                //    ys = cVideo.mLatitude_Mid;
                //    zs = 250;
                //    cp = sgworld.Creator.CreatePosition(xs, ys, zs, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -80, 0, 2000);

                //    iVideo = sgworld.Creator.CreateVideoOnTerrain(surl + @"Common\Textures\liangchang.jpg", cp, mGroupIDStatic, cVideo.ProjectName);
                //    //iVideo.PlayVideoOnStartup = false;
                //    iVideo.ProjectionFieldOfView = 45;
                //    //if (count < 2)
                //    //{
                //    //    iVideo.VideoFileName = @"D:\GISData\Common\Textures\x1.avi";
                //    //    count++;
                //    //}

                //    //  iVideo.pl

                //}

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                Console.WriteLine("CCTV Creation Error");
            }
        }

        public override void TEUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
