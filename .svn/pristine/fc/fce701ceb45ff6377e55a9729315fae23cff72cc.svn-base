using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using RailwayGIS.ModelInformation;

using System.Data.OleDb; // <- for database methods

using TerraExplorerX;

namespace RailwayGIS
{
    public partial class MainForm : DevComponents.DotNetBar.OfficeForm
    {

        public SGWorld66 sgworld;

        GMap.NET.WindowsForms.Markers.GMarkerGoogle marker;
        GMap.NET.WindowsForms.Markers.GMarkerGoogle markerTrain;
        int mflycode = 0;
        int flyingTick = 0;
        bool isFlying = false;

        private DateTime sTime = Convert.ToDateTime("2015-06-01");
        private DateTime eTime = DateTime.Now;

        public MainForm()
        {
            GlobalVar.LoadConfig();

            WelcomeFormJQ wf = new WelcomeFormJQ();
            wf.Show();

            LoginForm login = new LoginForm();
            if (login.ShowDialog() != DialogResult.OK)
            {
                Application.Exit();
            }

            GlobalVar.gScene = new ModelInformation.CRailwayScene();
            InitializeComponent();
            sgworld = new SGWorld66();
            
            this.axTE3DWindow1.Text = "蒙内标轨铁路";

            //buildProjectTree();

            gMapControl1.MapProvider = GMap.NET.MapProviders.OpenCycleMapProvider.Instance;
            gMapControl1.Zoom = 6D;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gMapControl1.Position = new GMap.NET.PointLatLng(-3.55 + 0.8, 38.5 - 0.3);
            gMapControl1.Visible = true;
            //this.globeWindow1.StatusBarReleaseInfo = "蒙内标轨铁路";

            //GlobalVar.gGForm = this;
            
            showProject();
            addDVG_Event();
            bar2.Hide();
            menuSG.Visible = false;
            mainContainer.Panel2Collapsed = true;
           
            wf.Close();

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
            foreach (CRailwayProject p in CRailwayScene.mBridgeList)
            {
                addItem((int)(p.mMileage_Mid / 1000), p.mProjectName, p.mSegmentName, p.mProfessionalName, p, dgvBridge);
            }
        }

        private void fillRoadGrid()
        {
            DataGridViewRow dr = new DataGridViewRow();
            foreach (CRailwayProject p in CRailwayScene.mRoadList)
            {
                addItem((int)(p.mMileage_Mid / 1000), p.mProjectName, p.mSegmentName, p.mProfessionalName, p, dgvRoad);
            }
        }

        private void fillTunnelGrid()
        {
            DataGridViewRow dr = new DataGridViewRow();
            foreach (CRailwayProject p in CRailwayScene.mTunnelList)
            {
                addItem((int)(p.mMileage_Mid / 1000), p.mProjectName, p.mSegmentName, p.mProfessionalName, p, dgvTunnel);
            }
        }

        private void fillStationGrid()
        {
            DataGridViewRow dr = new DataGridViewRow();
            foreach (CRailwayProject p in CRailwayScene.mStationList)
            {
                addItem((int)(p.mMileage_Mid / 1000), p.mProjectName, p.mSegmentName, p.mProfessionalName, p, dgvStation);
            }
        }
        private void fillOtherList()
        {
            DataGridViewRow dr = new DataGridViewRow();
            foreach (CRailwayProject p in CRailwayScene.mOtherList)
            {
                addItem((int)(p.mMileage_Mid / 1000), p.mProjectName, p.mSegmentName, p.mProfessionalName, p, dgvOther);
            }
        }

        private void addItem(int v1, string s2, string s3, string s4, CRailwayProject p, DataGridView dgv)
        {
            DataGridViewRow dr = new DataGridViewRow();
            foreach (DataGridViewColumn c in dgv.Columns)
            {
                dr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);
            }
            dr.Cells[0].Value = v1;
            dr.Cells[1].Value = s2;
            dr.Cells[2].Value = s3;
            dr.Cells[3].Value = s4;
            dr.Tag = p;
            dgv.Rows.Add(dr);
        }

        //private void buildProjectTree()
        //{
        //    TreeNode root = treeView1.Nodes.Add("蒙内铁路");
        //    root.Checked = true;
        //    TreeNode tn, leaf;
        //    root.Tag = null;

        //    tn = root.Nodes.Add("火车");
        //    tn.Checked = false;
        //    tn.ImageIndex = 6;
        //    if (CRailwayScene.mRoadList.Count > 0)
        //    {
        //        tn = root.Nodes.Add("路基");
        //        tn.Checked = true;
        //        tn.ImageIndex = 1;
        //        foreach (CRailwayProject p in CRailwayScene.mRoadList)
        //        {
        //            leaf = tn.Nodes.Add(p.mProjectName);
        //            leaf.Tag = p;
        //            leaf.ImageIndex = 1;
        //        }
        //        tn = root.Nodes.Add("桥梁");
        //        tn.Checked = true;
        //        tn.ImageIndex = 2;
        //        foreach (CRailwayProject p in CRailwayScene.mBridgeList)
        //        {
        //            leaf = tn.Nodes.Add(p.mProjectName);
        //            leaf.Tag = p;
        //            leaf.ImageIndex = 2;
        //        }
        //        tn = root.Nodes.Add("涵洞");
        //        tn.Checked = true;
        //        tn.ImageIndex = 3;
        //        foreach (CRailwayProject p in CRailwayScene.mTunnelList)
        //        {
        //            leaf = tn.Nodes.Add(p.mProjectName);
        //            leaf.Tag = p;
        //            leaf.ImageIndex = 3;
        //        }
        //        tn = root.Nodes.Add("车站工程");
        //        tn.Checked = true;
        //        tn.ImageIndex = 4;
        //        foreach (CRailwayProject p in CRailwayScene.mStationList)
        //        {
        //            leaf = tn.Nodes.Add(p.mProjectName);
        //            leaf.Tag = p;
        //            leaf.ImageIndex = 4;
        //        }
        //        tn = root.Nodes.Add("其他");
        //        tn.Checked = false;
        //        tn.ImageIndex = 5;
        //        foreach (CRailwayProject p in CRailwayScene.mOtherList)
        //        {
        //            leaf = tn.Nodes.Add(p.mProjectName);
        //            leaf.Tag = p;
        //            leaf.ImageIndex = 5;
        //        }


        //    }

        //}


        private void timerSyncronize_Tick(object sender, EventArgs e)
        {
            try
            {
                IPosition66 wp = sgworld.Window.CenterPixelToWorld().Position;
                GMap.NET.PointLatLng LatLng = marker.Position;
                LatLng.Lat = wp.Y;
                LatLng.Lng = wp.X;
                marker.Position = LatLng;

                LatLng = markerTrain.Position;
                LatLng.Lat = GlobalVar.gScene.mDynamicTrain.Position.Y;
                LatLng.Lng = GlobalVar.gScene.mDynamicTrain.Position.X;
                markerTrain.Position = LatLng;
                if (isFlying)
                {
                    flyingTick++;
                    if (flyingTick % 100 == 0)
                        sgworld.Navigate.FlyTo(GlobalVar.gScene.mDynamicTrain, nextFLyCode());
                }
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
                //case Keys.F5:
                //    beginFly(); return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        void OnProjectLoadFinished(bool bSuccess)
        {
            GlobalVar.gCurPos = sgworld.Creator.CreatePosition(38.5, -3.35, 180000, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE,
                0, -65.0, 0, 150000);
            //Camera.initial_latitude, Camera.initial_longitude, Camera.initial_heading, Camera., Camera.initial_tilt
            sgworld.Navigate.FlyTo(GlobalVar.gCurPos, ActionCode.AC_FLYTO);

            GlobalVar.gScene.createProjectTree();
            timerSyncronize.Start();

        }

        ActionCode nextFLyCode()
        {
            ActionCode ac = ActionCode.AC_FLYTO;
            Random ran = new Random();
            int tmpcode = ran.Next(1,5);
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
            GlobalVar.gCurPos = sgworld.Window.PixelToWorld(X, Y).Position;
            //GlobalVar.gCurPos = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);
            GMap.NET.PointLatLng LatLng = marker.Position;
            LatLng.Lat = GlobalVar.gCurPos.Y;
            LatLng.Lng = GlobalVar.gCurPos.X;
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
                    foreach (CRailwayProject item in CRailwayScene.mProjectList)
                    //for (int j = 0; j < CRailwayScene.mProjectList.Count; j++)
                    {
                        if (item.mProjectName.Equals(projectName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            sgworld.Navigate.FlyTo(sgworld.Creator.CreatePosition(item.mLongitude_Mid, item.mLatitude_Mid, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE));
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

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Register to OnLoadFinished globe event

            sgworld.OnLoadFinished += new _ISGWorld66Events_OnLoadFinishedEventHandler(OnProjectLoadFinished);
            //sgworld.OnMouseWheel += new _ISGWorld66Events_OnMouseWheelEventHandler(OnMouseWheel);
            sgworld.OnLButtonUp += new _ISGWorld66Events_OnLButtonUpEventHandler(OnLButtonUp);
            sgworld.OnRButtonUp +=sgworld_OnRButtonUp;
            sgworld.OnProjectTreeAction += new _ISGWorld66Events_OnProjectTreeActionEventHandler(OnProjectTreeAction);
            //sgworld.OnDrawHUD += sgworld_OnDrawHUD;


            // Open Project in asynchronous mode

            string tProjectUrl = GlobalVar.gDataPath + GlobalVar.gProjectPath;
            sgworld.Project.Open(tProjectUrl, true, null, null);//          sgworld.Project.Open(@"F:\地图\MNGis.mpt", true, null, null);
        }


        private void bubbleButton1_Click(object sender, ClickEventArgs e)
        {
            //expandableSplitter1.Expanded = true;
            //navigationPane1.Expanded = true;
            //navigationPane1.ActiveControl = navigationPanePanel1;
            //navigationPanePanel1.to;

        }

        private void bubbleButton2_Click(object sender, ClickEventArgs e)
        {
            //navigationPane1.Expanded = true;
            //navigationPane1.ActiveControl = navigationPanePanel4;
            //navigationPane1.se
            //navigationPane1.SelectedPanel = navigationPanePanel2;
        }



        private void gMapControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            marker.Position = gMapControl1.FromLocalToLatLng(e.X, e.Y);
            double x, y, z, dir;
            //var cPos = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);
            double meters = CRailwayScene.mMiddleLine.findMeterbyCoor(marker.Position.Lng, marker.Position.Lat);
            CRailwayScene.mMiddleLine.findPosbyMeter(meters,out x,out y,out z,out dir);
            IPosition66 dPos = sgworld.Creator.CreatePosition(x, y, z, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, dir, -70, 0, 1500);
            //CRailwayScene.mMiddleLine.findNearestPos(cPos);
            sgworld.Navigate.FlyTo(dPos);
        }

        private void gMapControl1_Load(object sender, EventArgs e)
        {
            GMap.NET.WindowsForms.GMapOverlay markersOverlay = new GMap.NET.WindowsForms.GMapOverlay("markers");

            marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(new GMap.NET.PointLatLng(-3.35, 38.5),
              GMap.NET.WindowsForms.Markers.GMarkerGoogleType.red_dot);
            markerTrain = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(new GMap.NET.PointLatLng(-3.35, 38.5),
                GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue_small);
            markersOverlay.Markers.Add(marker);
            markersOverlay.Markers.Add(markerTrain);

            marker.IsHitTestVisible = false;

            gMapControl1.Overlays.Add(markersOverlay);
        }



        private void dgvRoad_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            CRailwayProject p = null;
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
                case "dgvStation":
                    p = dgvStation.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                case "dgvOther":
                    p = dgvOther.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                default:
                    return;

            }

            sgworld.Navigate.FlyTo(sgworld.Creator.CreatePosition(p.mLongitude_Mid, p.mLatitude_Mid, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -65.0, 0, 1500));


        }

        private void dgvRoad_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            CRailwayProject p = null;
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
                case "dgvStation":
                    p = dgvStation.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                case "dgvOther":
                    p = dgvOther.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                default:
                    return;

            }
            sgworld.Navigate.FlyTo(sgworld.Creator.CreatePosition(p.mLongitude_Mid, p.mLatitude_Mid, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -65.0, 0, 1500));
            string urlstr = "http://" + GlobalVar.gUserIP + "/MNMIS/APP/ProjectMng.aspx?f=detail&sn=" + p.mSerialNo +
                       "&uacc=" + GlobalVar.gUserName + "&upwd=" + GlobalVar.gUserPWD;
            mainContainer.Panel2Collapsed = false;
            webBrowser1.Navigate(urlstr);
        }


        private void knobControl1_ValueChanged(object sender, DevComponents.Instrumentation.ValueChangedEventArgs e)
        {
            GlobalVar.gScene.mDynamicTrain.TurnSpeed = Decimal.ToDouble(knobControl1.Value);
        }

        private void expandablePanel2_ExpandedChanged(object sender, ExpandedChangeEventArgs e)
        {
            if (expandablePanel2.Expanded)
            {
                isFlying = true;
                sgworld.Navigate.FlyTo(GlobalVar.gScene.mDynamicTrain, ActionCode.AC_FLYTO);
                GlobalVar.gScene.mDynamicTrain.TurnSpeed = Decimal.ToDouble(knobControl1.Value);
                GlobalVar.gScene.mDynamicTrain.Pause = false;                
            }
            else
            {
                isFlying = false;
                GlobalVar.gScene.mDynamicTrain.Pause = true;
            }
        }

        private void buttonItem1_Click(object sender, EventArgs e)
        {
            this.bar2.Show();
        }


        private void trackTime_ValueChanged(object sender, EventArgs e)
        {
            double t = (double)(trackTime.Value - trackTime.Minimum) / (trackTime.Maximum - trackTime.Minimum);
            int t2 = (int)((eTime.DayOfYear - sTime.DayOfYear) * t);
            DateTime dt = sTime.AddDays(t2);
            trackTime.KeyTips = dt.ToString();
            Console.WriteLine(dt.ToString());
            var sgworld = new SGWorld65();
            sgworld.DateTime.Current = dt;
            trackTime.ShowToolTip();
        }

        private void buttonItem4_Click(object sender, EventArgs e)
        {
            expandablePanel1.Visible = !expandablePanel1.Visible;
        }
 

        private void menuSG_ItemClick(object sender, EventArgs e)
        {
            RadialMenuItem item = sender as RadialMenuItem;

            if (item != null && !string.IsNullOrEmpty(item.Text))
            {
                switch (item.Text)
                {
                    case "FlyAround":
                        //var cPos = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);
                        IPosition66 cPos = CRailwayScene.mMiddleLine.findNearestPos();
                        this.menuSG.Visible = false;
                        //CRailwayScene.mMiddleLine.findPosbyMeter(meters, out x, out y, out z, out dir);
                        sgworld.Navigate.FlyTo(cPos,ActionCode.AC_ARCPATTERN);
                        break;
                    case "NextPro":

                        break;

                }
                //MessageBox.Show(string.Format("{0} Menu item clicked: {1}\r\n", DateTime.Now, item.Text));
            }
        }

        private void addDVG_Event()
        {
            this.dgvRoad.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvRoad_RowHeaderMouseClick);
            this.dgvRoad.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvRoad_RowHeaderMouseDoubleClick);
            this.dgvBridge.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvRoad_RowHeaderMouseClick);
            this.dgvBridge.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvRoad_RowHeaderMouseDoubleClick);
            this.dgvTunnel.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvRoad_RowHeaderMouseClick);
            this.dgvTunnel.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvRoad_RowHeaderMouseDoubleClick);
            this.dgvStation.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvRoad_RowHeaderMouseClick);
            this.dgvStation.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvRoad_RowHeaderMouseDoubleClick);
            this.dgvOther.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvRoad_RowHeaderMouseClick);
            this.dgvOther.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvRoad_RowHeaderMouseDoubleClick);

            
            this.gMapControl1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseDoubleClick);

            this.expandablePanel2.ExpandedChanged += new DevComponents.DotNetBar.ExpandChangeEventHandler(this.expandablePanel2_ExpandedChanged);
            this.knobControl1.ValueChanged += new System.EventHandler<DevComponents.Instrumentation.ValueChangedEventArgs>(this.knobControl1_ValueChanged);
            this.menuSG.ItemClick += new System.EventHandler(this.menuSG_ItemClick);

            this.buttonItem1.Click += new System.EventHandler(this.buttonItem1_Click);
            this.buttonItem2.Click += new System.EventHandler(this.buttonItem2_Click);
            this.trackTime.ValueChanged += new System.EventHandler(this.trackTime_ValueChanged);
            this.buttonItem4.Click += new System.EventHandler(this.buttonItem4_Click);
        }

        private void buttonItem2_Click(object sender, EventArgs e)
        {
            mainContainer.Panel2Collapsed = true;
        }


    }
}