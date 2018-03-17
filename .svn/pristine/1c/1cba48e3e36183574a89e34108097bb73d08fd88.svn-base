using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    public class CTEScene
    {
        public CRailwayScene mSceneData;
        public CRWTEStandard mRWStandard;

        public DateTime fromDate = DateTime.Now.AddDays(-7);
        public DateTime toDate = DateTime.Now;

        public List<CTERWItem> mTEItemList = new List<CTERWItem>();

        //public CTEKML mTEKml = null;
        //public CTEStation mTEStation = null;
        //public CTEFactory mTEFact = null;
        //public CTENavTrain mTETrain = null;
        //public CTEPier mTEPier = null;
        //public CTEProject mTEProj = null;
        //public CTECons mTECons = null;
        //public CTEMiddleLine mTEMiddleLine = null;
        //public CTEConsPhoto mTEPhotos = null;
        //public CTEFirm mTEFirm = null;

        //public string mIDKml = "KML";
        //public string mIDStation = "Station";
        public CTEScene(CRailwayScene s) {
            mSceneData = s;
            mRWStandard = new CRWTEStandard();

            //mTEItemList.Add(new CTEKML(s, this, mRWStandard));
            //mTEItemList.Add(new CTEStation(s, this, mRWStandard));

            //mTEItemList.Add(new CTENavTrain(s, this, mRWStandard));

            mTEItemList.Add(new CTEProject(s, this, mRWStandard));
            mTEItemList.Add(new CTECons(s, this, mRWStandard));
            //mTEItemList.Add(new CTEMiddleLine(s, this, mRWStandard));
            mTEItemList.Add(new CTEFirm(s, this, mRWStandard));
            
            
            
            //mTEItemList.Add(new CTEConsPhoto(s, mRWStandard));

            //mTEKml = new CTEKML(s, mRWStandard);
            //mTEStation = new CTEStation(s, mRWStandard);
            //mTEFact = new CTEFactory(s, mRWStandard);
            //mTETrain = new CTENavTrain(s, this, mRWStandard); 
            //mTEPier = new CTEPier(s, mRWStandard);
            //mTEProj = new CTEProject(s, mRWStandard);
            //mTECons = new CTECons(s, mRWStandard);
            //mTEMiddleLine = new CTEMiddleLine(s, mRWStandard);
            //mTEFirm = new CTEFirm(s, mRWStandard);
            //mTEPhotos = new CTEConsPhoto(s, mRWStandard);
        }

        public void updateProjectTree()
        {
            foreach (CTERWItem rw in mTEItemList)
            {
                if (rw is CTECons || rw is CTEProject)
                    rw.TEUpdate();
            }
        }

        public void createProjectTree()
        {
            //var sgworld = new SGWorld66();
            
            //各种铁路单位，工点，标注图标
            CTEFeature.linesPointsLoad("JqPoints1");
            CTEFeature.linesPointsLoad("JqPoints2");
            foreach (CTERWItem rw in mTEItemList)
            {
                rw.TECreate();
            }

            //sgworld.Terrain.Opacity = 0.3;

            //sgworld.ProjectTree.CreateGroup("Cons");
            //mTECons.TECreate();

            ////sgworld.ProjectTree.CreateGroup("Photos");
            ////mTEPhotos.TECreate("Photos");            
            
            ////sgworld.ProjectTree.CreateGroup(mIDKml);
            //mTEKml.TECreate();
            ////sgworld.ProjectTree.CreateGroup(mIDStation);
            //mTEStation.TECreate();
            //sgworld.ProjectTree.CreateGroup("梁厂");
            //mTEFact.TECreate();

            //mTETrain.TECreate();

            
            ////sgworld.ProjectTree.CreateGroup("Pier");
            ////mTEPier.TECreate("Pier");

            //sgworld.ProjectTree.CreateGroup("Project");
            //mTEProj.TECreate();

            //sgworld.ProjectTree.CreateGroup("三维中线");
            //mTEMiddleLine.TECreate();

            //sgworld.ProjectTree.CreateGroup("Firms");
            //mTEFirm.TECreate();
            
            //createProjectLineAndLabel();
            //CTEProject.LoadTEProFromDB();
            //CTEProject.LoadTEProModel();
            //CTEPier.LoadPiersFromDB();
            //CTEPier.LoadPiersModels();

            //createLabelWithProject();
            //createKML();
            //createNavTrain();
            //createVideo();
            //createProjectProgress();

            //createStation();
        }
    }
}
