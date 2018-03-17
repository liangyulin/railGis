using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using ModelInfo;
using RailwayGIS.TEProject;
using System.Data.OleDb; // <- for database methods

using TerraExplorerX;

namespace RailwayGIS
{
    public partial class GISForm : DevComponents.DotNetBar.OfficeForm
    {
        private DevComponents.DotNetBar.RadialMenuItem itemAround;
        private DevComponents.DotNetBar.RadialMenuItem itemPano;
        private DevComponents.DotNetBar.RadialMenuItem itemNextPro;
        private DevComponents.DotNetBar.RadialMenuItem itemPrevPro;

        CRailwayScene gRWScene;

        CTEScene mTEScene;

        MaxForm f = new MaxForm();

        public SGWorld66 sgworld;

        GMap.NET.WindowsForms.Markers.GMarkerGoogle marker;
        GMap.NET.WindowsForms.Markers.GMarkerGoogle markerTrain;
        int mflycode = 0;
        //int flyingTick = 0;
        //bool isFlying = false;
        bool _bFullScreenMode = false;

        private DateTime sTime = Convert.ToDateTime("2016-01-01");
        private DateTime eTime = DateTime.Now;
        private DateTime curTime = DateTime.Now;
        private bool isTimeChanged = false;

        public IPosition66 mCurPos;

        private int SH = Screen.PrimaryScreen.Bounds.Height;
        private int SW = Screen.PrimaryScreen.Bounds.Width;

        bool trainFly = false;
        bool trainBack = false;
        ITerrainDynamicObject66 mDynamicTrain;
        string fileName = CGisDataSettings.gDataPath + @"Common\Models\Train\train.3ds";
        int count = 0;

        CTENavTrain mNavTrain;
        int mNavIndex = 0;

        private void createMenuSG(){

            this.itemAround = new DevComponents.DotNetBar.RadialMenuItem();
            this.itemPano = new DevComponents.DotNetBar.RadialMenuItem();
            this.itemNextPro = new DevComponents.DotNetBar.RadialMenuItem();
            this.itemPrevPro = new DevComponents.DotNetBar.RadialMenuItem();

            this.menuSG.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.itemAround,
            this.itemPano,
            this.itemNextPro,
            this.itemPrevPro});
            // 
            // itemAround
            // 
            this.itemAround.Name = "itemAround";
            this.itemAround.Symbol = "";
            this.itemAround.Text = "FlyAround";
            this.itemAround.TextVisible = false;
            this.itemAround.Tooltip = "Fly Around View";
            // 
            // itemPano
            // 
            this.itemPano.Name = "itemPano";
            this.itemPano.Symbol = "57779";
            this.itemPano.SymbolSet = DevComponents.DotNetBar.eSymbolSet.Material;
            this.itemPano.Text = "Panorama";
            this.itemPano.TextVisible = false;
            this.itemPano.Tooltip = "Panorama View";
            // 
            // itemNextPro
            // 
            this.itemNextPro.Name = "itemNextPro";
            this.itemNextPro.Symbol = "";
            this.itemNextPro.Text = "NextPro";
            this.itemNextPro.TextVisible = false;
            this.itemNextPro.Tooltip = "Next Project";
            // 
            // itemPrevPro
            // 
            this.itemPrevPro.Name = "itemPrevPro";
            this.itemPrevPro.Symbol = "";
            this.itemPrevPro.Text = "PrevPro";
            this.itemPrevPro.TextVisible = false;
            this.itemPrevPro.Tooltip = "Previous Project";
        }

        private void showProject()
        {
            fillRoadGrid();
            fillBridgeGrid();
            fillTunnelGrid();
            fillStationGrid();
            fillOtherList();
        }

        private void fillBridgeGrid()
        {
            DataGridViewRow dr = new DataGridViewRow();
            foreach (CRailwayProject p in gRWScene.mBridgeList)
            {
                addItem(p.ProjectName, p.SegmentName, (int)(p.AvgProgress * 100), p, dgvBridge);
            }
        }

        private void fillRoadGrid()
        {
            DataGridViewRow dr = new DataGridViewRow();
            foreach (CRailwayProject p in gRWScene.mRoadList)
            {
                //addItem((int)(p.mMileage_Mid / 1000), p.mProjectName, p.SegmentName, p.mProfessionalName, p, dgvRoad);
                addItem( p.ProjectName, p.SegmentName, (int)(p.AvgProgress*100), p, dgvRoad);
            }
        }

        private void fillTunnelGrid()
        {
            DataGridViewRow dr = new DataGridViewRow();
            foreach (CRailwayProject p in gRWScene.mTunnelList)
            {
                //addItem((int)(p.mMileage_Mid / 1000), p.mProjectName, p.SegmentName, p.mProfessionalName, p, dgvTunnel);
                addItem(p.ProjectName, p.SegmentName, (int)(p.AvgProgress * 100), p, dgvTunnel);
            }
        }

        private void fillStationGrid()
        {
            DataGridViewRow dr = new DataGridViewRow();
            foreach (CRailwayProject p in gRWScene.mContBeamList)
            {
                //addItem((int)(p.mMileage_Mid / 1000), p.mProjectName, p.SegmentName, p.mProfessionalName, p, dgvStation);
                addItem( p.ProjectName, p.SegmentName, (int)(p.AvgProgress*100), p, dgvConsBeam);
            }
        }
        private void fillOtherList()
        {
            //DataGridViewRow dr = new DataGridViewRow();
            //foreach (CRailwayProject p in gRWScene.mOtherList)
            //{
            //    //addItem((int)(p.mMileage_Mid / 1000), p.mProjectName, p.SegmentName, p.mProfessionalName, p, dgvOther);
            //    addItem( p.ProjectName, p.SegmentName, (int)(p.AvgProgress*100), p, dgvOther);
            //}
        }

        private void addItem(string s1, string s2, int s3,  CRailwayProject p, DataGridView dgv)
        {
            DataGridViewRow dr = new DataGridViewRow();
            foreach (DataGridViewColumn c in dgv.Columns)
            {
                dr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);
            }
            dr.Cells[0].Value = s1;
            dr.Cells[1].Value = s2;
            dr.Cells[2].Value = s3;
            //dr.Cells[3].Value = s4;
            dr.Tag = p;
            dgv.Rows.Add(dr);
        }

        private void timerSyncronize_Tick(object sender, EventArgs e)
        {
            try
            {
                IPosition66 wp = sgworld.Window.CenterPixelToWorld().Position;
                GMap.NET.PointLatLng LatLng = marker.Position;
                LatLng.Lat = wp.Y;
                LatLng.Lng = wp.X;
                marker.Position = LatLng;

                //LatLng = markerTrain.Position;
                //LatLng.Lat = GlobalVar.gScene.mDynamicTrain.Position.Y;
                //LatLng.Lng = GlobalVar.gScene.mDynamicTrain.Position.X;
                //markerTrain.Position = LatLng;
                //if (isFlying)
                //{
                //    flyingTick++;
                //    if (flyingTick % 100 == 0)
                //        sgworld.Navigate.FlyTo(GlobalVar.gScene.mDynamicTrain, nextFLyCode());
                //}


            }
            catch (Exception ex)
            {
                Console.WriteLine("Train is not ready");
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    if (MessageBox.Show("是否退出系统?", "退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Application.Exit();
                    }
                    else
                        return true;
                    break;
                case Keys.F11:
                    {

                    }
                    break;
                //case Keys.F5:
                //    beginFly(); return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void setMainContainer()
        {
            mainContainer.Panel1.Controls.Add(this.axTE3DWindow1);
            _bFullScreenMode = false;
        }

        void OnProjectLoadFinished(bool bSuccess)
        {

            mCurPos = sgworld.Creator.CreatePosition(118.6, 36.6, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE,
                341, -90.0, 0, 445000);
            //Camera.initial_latitude, Camera.initial_longitude, Camera.initial_heading, Camera., Camera.initial_tilt
            sgworld.Navigate.FlyTo(mCurPos, ActionCode.AC_FLYTO);
            if (mTEScene == null)
                mTEScene = new CTEScene(gRWScene);
            mTEScene.createProjectTree();
            


           // mTEScene = new CTEScene()
            //GlobalVar.gScene.createProjectTree();
            timerSyncronize.Start();

        }

        ActionCode nextFLyCode()
        {
            ActionCode ac = ActionCode.AC_FLYTO;
            Random ran = new Random();
            int tmpcode = ran.Next(1, 5);
            while (tmpcode == mflycode)
            {
                tmpcode = ran.Next(1, 5);
            }
            mflycode = tmpcode;
            //Console.Write(mflycode + "\t");
            //if (mflycode > 4) mflycode = 1;
            switch (mflycode)
            {
                case 1:
                    ac = ActionCode.AC_FOLLOWABOVE; //ActionCode.AC_FOLLOWBEHIND;
                    break;
                case 2:
                    ac = ActionCode.AC_FOLLOWBEHINDANDABOVE;
                    break;
                case 3:
                    ac = ActionCode.AC_FOLLOWRIGHT;
                    break;
                case 4:
                    ac = ActionCode.AC_FOLLOWLEFT;
                    break;

            }
            return ac;

        }

        bool sgworld_OnRButtonUp(int Flags, int X, int Y)
        {
            //GlobalVar.gCurPos = sgworld.Window.PixelToWorld(X, Y).Position;
            //GlobalVar.gCurPos = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);
            this.menuSG.Location = new Point(X, Y);
            menuSG.Visible = true;
            return true;
        }

        bool OnLButtonUp(int Flags, int X, int Y)
        {
            mCurPos = sgworld.Window.PixelToWorld(X, Y).Position;
            //GlobalVar.gCurPos = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);
            GMap.NET.PointLatLng LatLng = marker.Position;
            LatLng.Lat = mCurPos.Y;
            LatLng.Lng = mCurPos.X;
            marker.Position = LatLng;
            //wp.Distance
            return false;
        }

        void OnProjectTreeAction(string ID, IAction66 Action)
        {
            string projectName;
            if (Action.Code.Equals(ActionCode.AC_SELCHANGED))
            {
                try
                {
                    projectName = sgworld.ProjectTree.GetItemName(ID);
                    //var branch = sgworld.ProjectTree.FindItem("MiddleLine");
                    //if (projectName.Equals("Train"))
                    //{

                    //    //sgworld.ProjectTree.SetVisibility(branch, false);
                    //    sgworld.Navigate.FlyTo(GlobalVar.gScene.mDynamicTrain, ActionCode.AC_FLYTO);
                    //    flyingTick++;
                    //    return;

                    //}

                    //sgworld.ProjectTree.SetVisibility(branch, true);
                    foreach (CRailwayProject item in gRWScene.mBridgeList)
                    //for (int j = 0; j < CRailwayScene.mProjectList.Count; j++)
                    {
                        if (item.ProjectName.Equals(projectName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            sgworld.Navigate.FlyTo(sgworld.Creator.CreatePosition(item.CenterLongitude, item.CenterLatitude, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE));
                            //textBox2.Text = "Project Name\t" + item.mProjectName + "\r\nProject Type\t" + item.mProfessionalName +
                            //    "\r\nStart Position\t" + item.mMileage_Start_Des + "\r\nEnd_Position\t" + item.mMileage_End_Des;
                            //timerFlyto.Tag = item.mSerialNo;
                            //timerFlyto.Start();
                            //System.Threading.Thread.Sleep(2000);
                            //showWebPage(item.mSerialNo);
                            break;

                        }
                    }
                    //flyingTick = 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }

            }


        }



        private void gMapControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            marker.Position = gMapControl1.FromLocalToLatLng(e.X, e.Y);
            double x, y, z, dir;
            //var cPos = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);
            double meters;
            double dis;
            CRailwayLine crwline = gRWScene.mMiddleLines.getMeterbyPos(marker.Position.Lng, marker.Position.Lat, out meters,out dis);
            gRWScene.mMiddleLines.getPosbyDKCode(crwline.mDKCode, meters, out x, out y, out z, out dir);
            IPosition66 dPos = sgworld.Creator.CreatePosition(x, y, z, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, dir, -70, 0, 1500);
            //CRailwayScene.mMiddleLine.findNearestPos(cPos);
            sgworld.Navigate.FlyTo(dPos);
        }

        private void gMapControl1_Load(object sender, EventArgs e)
        {
            GMap.NET.WindowsForms.GMapOverlay markersOverlay = new GMap.NET.WindowsForms.GMapOverlay("markers");

            marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(new GMap.NET.PointLatLng(36.6, 118.6),
              GMap.NET.WindowsForms.Markers.GMarkerGoogleType.red_dot);
            markerTrain = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(new GMap.NET.PointLatLng(36.6, 118.6),
                GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue_small);
            markersOverlay.Markers.Add(marker);
            markersOverlay.Markers.Add(markerTrain);

            marker.IsHitTestVisible = false;

            gMapControl1.Overlays.Add(markersOverlay);
        }



        private void dgvRoad_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            CRailwayProject p = null;
            if (e.RowIndex < 0) return;
            string sname = (sender as DevComponents.DotNetBar.Controls.DataGridViewX).Name;
            switch (sname)
            {
                case "dgvRoad":
                    p = dgvRoad.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                case "dgvBridge":
                    p = dgvBridge.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                case "dgvTunnel":
                    p = dgvTunnel.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                case "dgvConsBeam":
                    p = dgvConsBeam.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                case "dgvOther":
                    //p = dgvOther.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                default:
                    return;

            }
            //if (GlobalVar.gCurrentPro != null && GlobalVar.gCurrentPro.mPolyline != null)
            //{
            //    GlobalVar.gCurrentPro.mPolyline.Visibility.Show = false;
            //}
            ////if (p.mPolyline != null)
            ////    p.mPolyline.Visibility.Show = true;
            //GlobalVar.gCurrentPro = p;
            //sgworld.Navigate.FlyTo(sgworld.Creator.CreatePosition(p.mLongitude_Mid, p.mLatitude_Mid, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, p.mDirection + 90, -35.0, 0, 800));
            sgworld.Navigate.FlyTo(sgworld.Creator.CreatePosition(p.CenterLongitude, p.CenterLatitude, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -35.0, 0, 800));
            advPropertyGrid1.SelectedObject = p;

        }

        private void dgvRoad_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            CRailwayProject p = null;
            if (e.RowIndex < 0) return;
            string sname = (sender as DevComponents.DotNetBar.Controls.DataGridViewX).Name;
            switch (sname)
            {
                case "dgvRoad":
                    p = dgvRoad.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                case "dgvBridge":
                    p = dgvBridge.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                case "dgvTunnel":
                    p = dgvTunnel.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                case "dgvConsBeam":
                    p = dgvConsBeam.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                case "dgvOther":
                    p = dgvOther.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                default:
                    return;

            }
            // http://railmis.imwork.net/jqmis/webservice/ProjectService.asmx
            sgworld.Navigate.FlyTo(sgworld.Creator.CreatePosition(p.CenterLongitude, p.CenterLatitude, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -35.0, 0, 800));
            string urlstr = "http://" + CGisDataSettings.gCurrentProject.projectUrl + "/APP/ProjectMng.aspx?f=detail&shownav=1&sn=" + p.mSerialNo +
                       "&uacc=" + CGisDataSettings.gCurrentProject.userName + "&upwd=" + CGisDataSettings.gCurrentProject.userPSD;//&showtab=1
            
            //ProjectService.ProjectServiceSoapClient ws = new ProjectService.ProjectServiceSoapClient();
            //Console.WriteLine(p.mProjectName + 　ws.ws_Get_ProjectProgressRate(p.mSerialNo));
            //DataTable dt;
            //dt = ws.ws_Bind_ProjectProgress_HistoryRate_DataTable(p.mSerialNo);
            //foreach (DataRow dr in dt.Rows)
            //{
            //    Console.WriteLine((string)dr["Date"] + "\t" + (double)dr["Rate"]);
            //}
            advPropertyGrid1.SelectedObject = p;
            pnlTrain.Expanded = true;
            //mainContainer.Panel2Collapsed = false;
            //webBrowser1.Navigate(urlstr);
        }


        private void knobControl1_ValueChanged(object sender, DevComponents.Instrumentation.ValueChangedEventArgs e)
        {
            //GlobalVar.gScene.mDynamicTrain.TurnSpeed = Decimal.ToDouble(knobControl1.Value);

        }

        private void pnlTrain_ExpandedChanged(object sender, ExpandedChangeEventArgs e)
        {
            //if (pnlTrain.Expanded)
            //{
            //    isFlying = true;
            //    sgworld.Navigate.FlyTo(GlobalVar.gScene.mDynamicTrain, ActionCode.AC_FLYTO);
            //    GlobalVar.gScene.mDynamicTrain.TurnSpeed = Decimal.ToDouble(knobControl1.Value);
            //    GlobalVar.gScene.mDynamicTrain.Pause = false;
            //}
            //else
            //{
            //    isFlying = false;
            //    GlobalVar.gScene.mDynamicTrain.Pause = true;
            //}
            
        }


        private void trackZoom_ValueChanged(object sender, EventArgs e)
        {
            IPosition66 ipos = sgworld.Window.CenterPixelToWorld().Position;
            double x, y, z, dir;
            //var cPos = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);
            double meters;
            double dist;
            CRailwayLine crwline = gRWScene.mMiddleLines.getMeterbyPos(marker.Position.Lng, marker.Position.Lat, out meters,out dist);
            gRWScene.mMiddleLines.getPosbyDKCode(crwline.mDKCode, meters, out x, out y, out z, out dir);
            double dis = Math.Pow(10,trackZoom.Value+1);
            if (trackZoom.Value == 5)
            {
                dis = dis / 2;
            }
            IPosition66 dPos = sgworld.Creator.CreatePosition(x, y, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, dir, -70, 0, dis);
            //CRailwayScene.mMiddleLine.findNearestPos(cPos);
            sgworld.Navigate.FlyTo(dPos);
           
            //double t = (double)(trackZoom.Value - trackZoom.Minimum) / (trackZoom.Maximum - trackZoom.Minimum);

            //int t2 = (int)((eTime.DayOfYear - sTime.DayOfYear) * t);
            //DateTime dt = sTime.AddDays(t2);
            //trackZoom.Tooltip = dt.ToString();

            //sgworld.DateTime.Current = dt;
            //trackZoom.ShowToolTip();

        }


        private void menuSG_ItemClick(object sender, EventArgs e)
        {
            RadialMenuItem item = sender as RadialMenuItem;

            if (item != null && !string.IsNullOrEmpty(item.Text))
            {
                switch (item.Text)
                {
                    case "FlyAround":
                        ////var cPos = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);
                        //IPosition66 cPos = gRWScene.mMiddleLines.getNearPos();//CRailwayScene.mMiddleLine.findNearestPos();
                        //this.menuSG.Visible = false;
                        ////CRailwayScene.mMiddleLine.findPosbyMeter(meters, out x, out y, out z, out dir);
                        //sgworld.Navigate.FlyTo(cPos, ActionCode.AC_ARCPATTERN);
                        break;
                    case "NextPro":

                        break;

                }
                //MessageBox.Show(string.Format("{0} Menu item clicked: {1}\r\n", DateTime.Now, item.Text));
            }
        }

        private void addDVG_Event()
        {
            
            dgvRoad.CellMouseClick += dgvRoad_RowHeaderMouseClick;
            dgvBridge.CellMouseClick += dgvRoad_RowHeaderMouseClick;
            dgvTunnel.CellMouseClick += dgvRoad_RowHeaderMouseClick;
            dgvConsBeam.CellMouseClick += dgvRoad_RowHeaderMouseClick;
            dgvOther.CellMouseClick += dgvRoad_RowHeaderMouseClick;

            dgvRoad.CellMouseDoubleClick += dgvRoad_RowHeaderMouseDoubleClick;
            dgvBridge.CellMouseDoubleClick += dgvRoad_RowHeaderMouseDoubleClick;
            dgvTunnel.CellMouseDoubleClick += dgvRoad_RowHeaderMouseDoubleClick;
            dgvConsBeam.CellMouseDoubleClick += dgvRoad_RowHeaderMouseDoubleClick;
            dgvOther.CellMouseDoubleClick += dgvRoad_RowHeaderMouseDoubleClick;

            this.gMapControl1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseDoubleClick);

            this.pnlTrain.ExpandedChanged += new DevComponents.DotNetBar.ExpandChangeEventHandler(this.pnlTrain_ExpandedChanged);
            //this.btnTrain.Checked += new System.EventHandler(this.btnTrain_Checked);
         //   this.knobControl1.ValueChanged += new System.EventHandler<DevComponents.Instrumentation.ValueChangedEventArgs>(this.knobControl1_ValueChanged);
            this.menuSG.ItemClick += new System.EventHandler(this.menuSG_ItemClick);

            btnCloseDetail.Click += btnCloseDetail_Click;
            this.trackZoom.ValueChanged += new System.EventHandler(this.trackZoom_ValueChanged);
            
            this.btnRadar.Click += btnRadar_Click;
            this.btnTrain.Click += btnTrain_Click;
            this.btnTimeCtrl.Click += btnTimeCtrl_Click;
            this.btnFullScr.Click += btnFullScr_Click;
            this.btnRotate.Click += btnRotate_Click;
            

            this.btnSubgrade.Click += new System.EventHandler(this.btnProject_Click);
            this.btnBridge.Click += new System.EventHandler(this.btnProject_Click);
            this.btnTunnel.Click += new System.EventHandler(this.btnProject_Click);
            this.btnStation.Click += new System.EventHandler(this.btnProject_Click);
            this.btnOthers.Click += new System.EventHandler(this.btnProject_Click);
        }

        void btnRotate_Click(object sender, EventArgs e)
        {
            //var sgworld = new SGWorld66();
            IPosition66 resPos = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_RELATIVE); 
            resPos = sgworld.Creator.CreatePosition(resPos.X, resPos.Y, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, resPos.Yaw + 5, resPos.Pitch, resPos.Roll,1500 );//resPos.Altitude
            sgworld.Navigate.FlyTo(resPos);
        }

        void btnFullScr_Click(object sender, EventArgs e)
        {
            if (!_bFullScreenMode)
            {
                f.gf = this;
                f.Controls.Add(axTE3DWindow1);
                f.formState.Maximize(f);
                f.Show();
                _bFullScreenMode = true;
            }
        }

        void btnTimeCtrl_Click(object sender, EventArgs e)
        {
            //btnTimeCtrl.Checked = !btnTimeCtrl.Checked;
            //sgworld.Command.Execute(1065, 4);  // time slider

        }

        void btnCloseDetail_Click(object sender, EventArgs e)
        {
            mainContainer.Panel2Collapsed = true;
            bar1.Visible = false;
        }

        void btnTrain_Click(object sender, EventArgs e)
        {
            btnTrain.Checked = !btnTrain.Checked;
            
            timerNav.Start();
            //if (btnTrain.Checked)
            //{
            //    roamingAuto();
            //}
            //else
            //{
            //    sgworld.Creator.DeleteObject(mDynamicTrain.ID);
            //}

            //pnlTrain.Visible = btnTrain.Checked;
        }

//        public void roamingAuto()
//        {
//            //var sgworld = new SGWorld66();
//            mDynamicTrain = sgworld.Creator.CreateDynamicObject(0, DynamicMotionStyle.MOTION_MANUAL, DynamicObjectType.DYNAMIC_3D_MODEL,
//fileName, 0.25, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, string.Empty, "Train");//创建对象

//            double[] x,y,z,d;
//            int n;

//            count = 0;
//            //for (int count = 0; count < gRWScene.mProjectList.Count - 1; count++)//暂时以这个list代替
//            //{
//            n = gRWScene.mMiddleLines.getSubLineByDKCode(gRWScene.mProjectList[0].Mileage_Start_Discription, gRWScene.mProjectList[1].Mileage_Start_Discription, 70, out x, out y, out z, out d);
//                if (n > 0) {
//                    for (int i = 0; i < 200; i++) //CRailwayScene.mMiddleLine.mPointNum 将一个工点上所有点addWaypoint
//                    {
  
//                            if (d[i] > 180) d[i] = d[i] - 180;
//                                mDynamicTrain.Waypoints.AddWaypoint(sgworld.Creator.CreateRouteWaypoint(x[i], y[i], z[i], 300, d[i]));

//                    }
//                    //曾经加过一些动作或状态..
//                    mDynamicTrain.CircularRoute = false; 
//                    //Console.WriteLine(count + "," + DateTime.Now.ToString("HH:mm:ss"));//看下运行时的机制
//                //} 
//                    sgworld.Navigate.FlyTo(mDynamicTrain);

//            }
//    }

        private void btnRadar_Click(object sender, EventArgs e)
        {
            btnRadar.Checked = !btnRadar.Checked;
            pnlRadar.Expanded = true;
            pnlRadar.Visible = btnRadar.Checked;
        }

        private void buttonItem2_Click(object sender, EventArgs e)
        {
            mainContainer.Panel2Collapsed = true;
        }


        public GISForm()
        {
            //GlobalSettings.LoadConfig();

            LoginForm login = new LoginForm();
            if (login.ShowDialog() != DialogResult.OK) 
            {
                Application.Exit();
            }
            WelcomeFormJQ wf = new WelcomeFormJQ();
            wf.Show(this);

            //GlobalSettings.InitGlobal();

            gRWScene = new CRailwayScene();
            //GlobalVar.gScene = new ModelInformation.CRailwayScene();
            
            
            InitializeComponent();
            sgworld = new SGWorld66();
            this.axTE3DWindow1.Text = "新建济青高速铁路";
            
            //axTE3DWindow1.ProductName = "蒙内标轨铁路";
            axTE3DWindow1.Caption = "新建济青高速铁路";
            //sgworld.Project.set_Settings("RemoveSkylineCopyright", 1);
            //sgworld.Project.set_Settings("DisplaySun", 0);
            if (mTEScene == null)
                mTEScene = new CTEScene(gRWScene);
            mNavTrain = new CTENavTrain(gRWScene);
            //mNavTrain.OnFlyTo += mNavTrain_OnFlyTo;
            
            gMapControl1.MapProvider = GMap.NET.MapProviders.OpenCycleMapProvider.Instance;
            gMapControl1.Zoom = 6D;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gMapControl1.Position = new GMap.NET.PointLatLng(36.6, 118.6);
            gMapControl1.Visible = true;
            

            //GlobalSettings.gGForm = this;
            
            showProject();
            addDVG_Event();
            //bar1.Visible = false;
            //bar1.Hide();
            createMenuSG();            
            menuSG.Visible = false;
            mainContainer.Panel2Collapsed = true;
            trackTime.Text = curTime.ToLongDateString();
            
           
            wf.Close();
        }

        public void mNavTrain_OnFlyTo()
        {
            mNavIndex ++;
            if (mNavIndex < gRWScene.mHotSpotList.Count)
            {
                Console.WriteLine(gRWScene.mHotSpotList.Count + " Index " + mNavIndex);
                IPosition66 resPos = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_RELATIVE);
                resPos = sgworld.Creator.CreatePosition(gRWScene.mHotSpotList[mNavIndex].CenterLongitude, gRWScene.mHotSpotList[mNavIndex].CenterLatitude, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, resPos.Yaw + 5, resPos.Pitch, resPos.Roll, 1500);//resPos.Altitude
                sgworld.Navigate.FlyTo(resPos, ActionCode.AC_ARCPATTERN);
                //sgworld.Navigate.FlyTo(sgworld.Creator.CreatePosition(gRWScene.mHotSpotList[mNavIndex].CenterLongitude, gRWScene.mHotSpotList[mNavIndex].CenterLatitude, 800, AltitudeTypeCode.ATC_TERRAIN_RELATIVE,0,0,-90,0), ActionCode.AC_FOLLOWABOVE);
                timerNav.Start();
            }
            else { 
                timerNav.Stop();
                Console.WriteLine("end jounrey" + mNavIndex );
            
            }
        }

        private void GISForm_Load(object sender, EventArgs e)
        {
            // Register to OnLoadFinished globe event

            sgworld.OnLoadFinished += new _ISGWorld66Events_OnLoadFinishedEventHandler(OnProjectLoadFinished);
            //sgworld.OnMouseWheel += new _ISGWorld66Events_OnMouseWheelEventHandler(OnMouseWheel);
            sgworld.OnLButtonUp += new _ISGWorld66Events_OnLButtonUpEventHandler(OnLButtonUp);
            sgworld.OnRButtonUp += sgworld_OnRButtonUp;
            //sgworld.OnProjectTreeAction += new _ISGWorld66Events_OnProjectTreeActionEventHandler(OnProjectTreeAction);
            sgworld.OnDateTimeChanged += sgworld_OnDateTimeChanged;
            sgworld.OnObjectAction += sgworld_OnObjectAction;
            //sgworld.OnDrawHUD += sgworld_OnDrawHUD;


            // Open Project in asynchronous mode

            string tProjectUrl = CGisDataSettings.gDataPath + CGisDataSettings.gCurrentProject.projectLocalPath + "Default.fly";
            sgworld.Project.Open(tProjectUrl, true, null, null);//          sgworld.Project.Open(@"F:\地图\MNGis.mpt", true, null, null);
            //sgworld.DateTime.DisplaySun = false;
            sgworld.Command.Execute(1026, 0);  // sun
            sgworld.Command.Execute(1065, 4);
            

        }

        private void sgworld_OnObjectAction(string ObjectID, IAction66 Action)
        {
            //throw new NotImplementedException();
            //Console.WriteLine("ObjectID:" + ObjectID + ",action:" + Action.Code.ToString());

            if ((Action.Code == ActionCode.AC_WAYPOINT_REACHED) && (sgworld.ProjectTree.GetItemName(ObjectID) == "Train"))
                //if (sgworld.ProjectTree.GetItemName(ObjectID) == "Train" && Action.Code == ActionCode.AC_WAYPOINT_REACHED)

            {
                //mDynamicTrain.Pause = true;
                //Console.WriteLine(sgworld.ProjectTree.GetItemName(ObjectID));
                mNavTrain.GotoNext(mNavTrain_OnFlyTo);

            }

        }

        void sgworld_OnDateTimeChanged(object DateTime)
        {
            Console.WriteLine(DateTime);
            //mTEScene.mTECons.fromDate = DateTime.Now.AddDays(-30).Date.ToString("u");
            //mTEScene.mTECons.TEUpdate("Cons");
        }

        private void btnProject_Click(object sender, EventArgs e)
        {
            //this.mainContainer.Width = this.Width - 400;
            //this.axTE3DWindow1.Width = this.Width - 400;
            //this.bar1.Width =  360;
            //this.bar1.Show();
            this.bar1.Visible = true;
            bar1.SelectedDockTab = 0;
            string sname = (sender as DevComponents.DotNetBar.ButtonItem).Name;
            switch (sname)
            {
                case "btnSubgrade":
                    bar1.SelectedDockTab = 0;
                    break;
                case "btnBridge":
                    bar1.SelectedDockTab = 1;
                    break;
                case "btnTunnel":
                    bar1.SelectedDockTab = 2;
                    break;
                case "btnStation":
                    bar1.SelectedDockTab = 3;
                    break;
                case "btnOthers":
                    bar1.SelectedDockTab = 4;
                    break;
                default:
                    return;

            }
        }

        private void trackTime_ValueChanged(object sender, EventArgs e)
        {
            double t = (double)(trackTime.Value - trackTime.Minimum) / (trackTime.Maximum - trackTime.Minimum);

            int t2 = (int)((eTime.DayOfYear - sTime.DayOfYear) * t);
            DateTime dt = sTime.AddDays(t2);
            if (dt.ToShortDateString().Equals(curTime.ToShortDateString()))
                return;

            curTime = dt;
            isTimeChanged = true;

            trackTime.Tooltip = dt.ToString();
            trackTime.Text = dt.ToLongDateString();

            trackTime.ShowToolTip();
            sgworld.DateTime.Current = dt.AddHours(12);
            

            
            mTEScene.fromDate = dt.AddDays(-7);
            mTEScene.toDate = dt;

        }

        private void trackTime_MouseUp(object sender, MouseEventArgs e)
        {
            if (isTimeChanged)
            {
                isTimeChanged = false;
                mTEScene.updateProjectTree();
            }
        }

        private void timerNav_Tick(object sender, EventArgs e)
        {
            mNavTrain.Starting(mNavIndex);
            timerNav.Stop();
        }

    }
}