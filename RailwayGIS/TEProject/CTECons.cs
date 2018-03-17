using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    /// <summary>
    /// 实名制可视化，可视化施工点的人员信息
    /// </summary>
    public class CTECons : CTERWItem
    {
        Dictionary<string, ConsLocation> mPairs = new Dictionary<string, ConsLocation>();

        //List<ConsLocation> ConsList;
        //List<string> circleList;
        //List<string> labelList;


        public CTECons(CRailwayScene s, CTEScene ss, CRWTEStandard t)
            : base(s,ss, t)
        {
            
        }

        /// <summary>
        /// TODO 丁一明，显示聚类结果
        /// </summary>
        /// <param name="groupID"></param>
        public override void TECreate()
        {
            /* string[] usrName;
             string[] projName;
             string[] consDate;
             double[] x;
             double[] y;

             var sgworld = new SGWorld66();
             var branch = sgworld.ProjectTree.FindItem(groupID);

             int num;
             num = CConsLog.findLast365Cons(out usrName, out projName, out consDate, out x, out y);
             for (int i = 0; i < num; i++)
             {
                 Console.WriteLine("{0} #\t: User {1}\t Project {2}\t  x {3}\t y  {4}\t  Date {5}", i, usrName[i], projName[i], x[i], y[i], consDate[i]);
             }*/

            ///dym
            var sgworld = new SGWorld66();
            if (string.IsNullOrEmpty(mGroupIDDynamic))
                mGroupIDDynamic = sgworld.ProjectTree.CreateGroup("Cons");
            //var branch = sgworld.ProjectTree.FindItem(groupID);


            List<ConsLocation> ls;
            //ls = CConsLog.clusterConsByProj();
            ls = CConsLog.clusterConsFromWebByProj(mTEScene.fromDate.ToString("u"), mTEScene.toDate.ToString("u"));

            //int maxParticles = 100;
            //string imageName = "";
            //int rate = 2;
            //int shape = 0;

            //var branch = sgworld.ProjectTree.FindItem("Dying");
            //ITerrainEffect66 gParticleLabel = (ITerrainEffect66)sgworld.ProjectTree.GetObject(branch);
            //var xml = gParticleLabel.EffectXML;
            //Console.WriteLine(xml);

            //var particleText = "$$PARTICLE$$UserDefine: \r\n <?xml version='1.0' encoding='UTF-8'?> \r\n <Particle ID='Custom'>";
            //particleText += "<ParticleEmitter ID='Disc' NumParticles='130' Texture='d:\\Campfire.png'>\r\n";
            //particleText += "<Emitter Rate='13' Shape='Disc' SpeedShape='Disc' Scale='50,50,50' Speed='1,1,1' />\r\n";
            //particleText += "<Cycle Value='1' /><Sort Value='1' /><Rotation Speed='1' Time='2' Initial='0' />\r\n";
            //particleText += "<Render Value='Billboard' /><Gravity Value='0, 1, 0' /><Force Value='0' OverrideRotation='0' />\r\n";
            //particleText += "<Position Value='0, 0, 0' /><Life Value='3.06' /><Speed Value='1.41' />\r\n";
            //particleText += "<Color Value='0,0,255,255' /><Size Value='24,24' /><Drag Value='1' />\r\n";
            //particleText += "<Blend Type='' /><Fade FadeIn='0.47' FadeOut='0.65' MaxFade='0.28' /></ParticleEmitter>\r\n";
            //particleText += "</Particle>";

            //IPosition66 tp = sgworld.Creator.CreatePosition(118.086466892066, 36.9038495888797, 3, AltitudeTypeCode.ATC_TERRAIN_RELATIVE,
            //      0, -90.0, 0, 0);
            //ITerrainRegularPolygon66 tcircle = sgworld.Creator.CreateCircle(tp, 200, 0xFF0000FF, 0x00FF00FF, branch, "测试点");
            //tcircle.LineStyle.Width = -5.0;

            foreach (ConsLocation cl in ls) {
                IPosition66 p = sgworld.Creator.CreatePosition(cl.Longitude, cl.Latitude, 30, AltitudeTypeCode.ATC_TERRAIN_RELATIVE,
                  0, -90.0, 0, 0);
                ITerrainRegularPolygon66 circle = sgworld.Creator.CreateCircle(p, cl.number * 2 + 200, 0xFF0000FF, 0x00FF00FF, mGroupIDDynamic, cl.ProjName + cl.number);
              //  circle.de
                circle.LineStyle.Width = -5.0;
                circle.Visibility.MinVisibilityDistance = 100000;
                //circle.SetParam

                ITerrainLabel66 iLabel = sgworld.Creator.CreateLabel(p, cl.number + "", CGisDataSettings.gDataPath + @"Common\地标图片\worker.png", mTEStandard.mLabelStyleL2, mGroupIDDynamic, cl.ProjName);
                iLabel.Message.MessageID = sgworld.Creator.CreateMessage(MsgTargetPosition.MTP_POPUP, cl.ToString(), MsgType.TYPE_TEXT, true).ID;
            //    circle.Message.MessageID = sgworld.Creator.CreateMessage(MsgTargetPosition.MTP_POPUP, cl.ToString(), MsgType.TYPE_TEXT, true).ID;
                
                
                //particleText += "<ParticleEmitter ID='ring' NumParticles='" + maxParticles + "' Texture='" + imageName + "'>" + "\r\n" +
                //  "<Emitter Rate='" + rate + "' Shape='" + shape + "' SpeedShape='" + speedShape + "' Scale='" + scaleX + "," + scaleY + "," + scaleZ + "' Speed='" + speedX + "," + speedY + "," + speedZ + "' />" + "\r\n" + // shape:(Cone, Sphere, ShellCone, ShellSphere, Ring, Disc, Cube), scale:the size of the shape in meters,  speed:(X,Alt,Z) disform the shape like it is in radius=1.
                //  "<Cycle Value='1' />" + "\r\n" + // 0=one time , 1=loop
                //  "<Sort Value='1' />" + "\r\n" +
                //  rotationNodeConditional +

                //  "<Render Value='" + render + "' />" + "\r\n" +
                //  "<Gravity Value='" + gravityX + ", " + gravityY + ", " + gravityZ + "' />" + "\r\n" + // Gravity in X, Altitude and Z directions
                //  "<Force Value='" + force + "' OverrideRotation='" + overrideRotation + "' />" + "\r\n" + // Gravity in X, Altitude and Z directions
                //  "<Position Value='0, 0, 0' />" + "\r\n" + // doesn;t work
                //  "<Life Value='" + timeSpan + "' />" + "\r\n" + // life of each particle in seconds
                //  "<Speed Value='" + particleSpeed + "' />" + "\r\n" + // this value multiply the Emitter speed values (x,y,z)
                //  "<Color Value='20," + colorR + "," + colorG + "," + colorB + "' />" + "\r\n" +
                //  "<Size Value='" + sizeWithRatioX /*size*/ + "," + sizeWithRatioY /*size * sizeRatio*/ + "' />" + "\r\n" + // size of the particle image 1=original image size. Format: SizeX, SizeY
                //  "<Drag Value='" + drag + "' />" + "\r\n" + // drag force (units like graviy)
                //  "<Blend Type='" + blend + "' />" + "\r\n" +
                //    //***   "<Rotation Speed='0' Time='1.7' />" + "\r\n" + // optional: should be in a checkbox
                //  "<Fade FadeIn='" + fadeIn + "' FadeOut='" + fadeOut + "' MaxFade='" + maxFade + "' />" + "\r\n" + // fade of each particle. FadeIn/fade out in seconds. Max fade - the maximum fade value (alpha 0-1)
                //"</ParticleEmitter>" + "\r\n";
                //particleText += "</Particle>";

                //ITerrainEffect66 ite = sgworld.Creator.CreateEffect(p, particleText, branch, cl.name + cl.number);

                //ite.EffectXML = particleText;
                //ite.Terrain.BBox
                

                //Console.WriteLine("x: {0} y {1} number {2}", cl.longitude, cl.latitude, cl.number);
            }

            //DBSCAN.Cluster();

            //for (int i = 0; i < DBSCAN.center.Count; i++)
            //{
            //    IPosition66 p = sgworld.Creator.CreatePosition(DBSCAN.center[i].longitude, DBSCAN.center[i].latitude, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE,
            //      341, -90.0, 0, 445000);
            //    ITerrainRegularPolygon66 Circle = sgworld.Creator.CreateCircle(p, DBSCAN.p_count[i], 0x00FFFF, 0x00FF00, "", "");
            //    Circle.LineStyle.Width = -5.0;
            //   // sgworld.Creator.CreateCircle(p, DBSCAN.p_count[i]*100, 0x0000FF, 0x00FF00, "", "");
            //     //sgworld.Creator.CreateEllipse(p, DBSCAN.p_count[i], DBSCAN.p_count[i], 0x00FFFF, 0x00FF00, -1, "", "");

            //}
           

        }

        public override void TEUpdate()
        {
            var sgworld = new SGWorld66();
            //var branch = sgworld.ProjectTree.FindItem(groupID);
            //if (!string.IsNullOrEmpty(branch))
            //    sgworld.ProjectTree.DeleteItem(branch);
            sgworld.ProjectTree.DeleteItem(mGroupIDDynamic);
            mGroupIDDynamic = null;            
            this.TECreate();
            //sgworld.ProjectTree.
            //var current = sgworld.ProjectTree.GetNextItem(branch, ItemCode.CHILD);
            //while (string.IsNullOrEmpty(current) == false) { 

            //}
           // var current = sgworld.ProjectTree.GetNextItem(child, ItemCode.NEXT);

        }
    }


    //        $$PARTICLE$$UserDefine: 
    // <?xml version='1.0' encoding='UTF-8'?> 
    // <Particle ID='Custom'><ParticleEmitter ID='ring' NumParticles='130' Texture='fire.png'>
    //<Emitter Rate='13' Shape='Ring' SpeedShape='Ring' Scale='0,0,0' Speed='1,1,1' />
    //<Cycle Value='1' />
    //<Sort Value='1' />
    //<Rotation Speed='1' Time='2' Initial='0' />
    //<Render Value='Billboard' />
    //<Gravity Value='0, 1, 0' />
    //<Force Value='0' OverrideRotation='0' />
    //<Position Value='0, 0, 0' />
    //<Life Value='3.06' />
    //<Speed Value='1.41' />
    //<Color Value='20,0,255,255' />
    //<Size Value='2.4,2.4' />
    //<Drag Value='1' />
    //<Blend Type='' />
    //<Fade FadeIn='0.47' FadeOut='0.65' MaxFade='0.28' />
    //</ParticleEmitter>
    //<ParticleEmitter ID='ring' NumParticles='62' Texture='CampFireBrightSmall.png'>
    //<Emitter Rate='8' Shape='Disc' SpeedShape='Disc' Scale='0.6,0.7,0.6' Speed='1,1,1' />
    //<Cycle Value='1' />
    //<Sort Value='1' />
    //<Render Value='Billboard' />
    //<Gravity Value='0, 2, 0' />
    //<Force Value='0' OverrideRotation='0' />
    //<Position Value='0, 0, 0' />
    //<Life Value='1.12' />
    //<Speed Value='1' />
    //<Color Value='20,255,255,255' />
    //<Size Value='2.1,2.1' />
    //<Drag Value='0' />
    //<Blend Type='' />
    //<Fade FadeIn='0.16' FadeOut='0.15' MaxFade='0.07' />
    //</ParticleEmitter>
    //</Particle>
}
