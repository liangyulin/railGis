using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DevComponents.DotNetBar;
using ModelInfo;

namespace RailwayGIS
{
    public partial class LoginForm : DevComponents.DotNetBar.RibbonForm
    {
        string loginFile = ModelInfo.CGisDataSettings.gDataPath + @".usr";//GlobalSettings.gDataPath
        public LoginForm()
        {
            InitializeComponent();

            foreach (ProjectConfig pc in CGisDataSettings.gProjectList)
            {
                comboBoxEx1.Items.Add(pc.projectName);                
            }
            comboBoxEx1.SelectedIndex = 0;
            textBoxX1.Text = CGisDataSettings.gProjectList[comboBoxEx1.SelectedIndex].userName;   
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (textBoxX1.Text.Length == 0 || textBoxX2.Text.Length == 0)
            {
                btnOK.Enabled = false;
                return;
            }


            CGisDataSettings.gCurrentProject.projectUrl = CGisDataSettings.gProjectList[comboBoxEx1.SelectedIndex].projectUrl;//ipAddressInput1.Value;
            CGisDataSettings.gCurrentProject.userName = textBoxX1.Text.Trim();
            CGisDataSettings.gCurrentProject.userPSD = textBoxX2.Text.Trim();
            CGisDataSettings.gCurrentProject.projectLocalPath = CGisDataSettings.gProjectList[comboBoxEx1.SelectedIndex].projectLocalPath;

            CServerWrapper.ConnectToServer(CGisDataSettings.gCurrentProject.projectUrl);

            //string url = "http://" + GlobalSettings.gProjectUrl + "/webservice/usrlogin.asmx";

            string resultStr = null;
            if (CServerWrapper.isConnected)
            {
                resultStr = CServerWrapper.webLogin(CGisDataSettings.gCurrentProject.userName, CGisDataSettings.gCurrentProject.userPSD);
            }
            
            //try
            //{
            //    WebServiceAgent ag = new WebServiceAgent(url);
            //    resultStr = (string)ag.Invoke("CheckUsrLogin", GlobalSettings.gUserName, GlobalSettings.gUserPWD);
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show(ex.ToString().Split('\n')[0]);
            //}
            //finally
            //{
                if (resultStr == "µÇÂ¼³É¹¦")
                {
                    CGisDataSettings.UpdateConfigInfo();
                    this.DialogResult = DialogResult.OK; // Setting the DialogResult, will close the dialog, and the ShowDialog call will return.
                }
                else if (resultStr != null)
                {
                    MessageBox.Show("Wrong User Name or Password");
                    Environment.Exit(-1);
                    //this.DialogResult 
                }
                else
                {
                    MessageBox.Show("No response from remote server, using local database instead");                    
                    this.DialogResult = DialogResult.OK;
                }
                this.btnOK.Enabled = true;
            //}           
            //GlobalVar.useLocalDB = true;
            //this.DialogResult = DialogResult.OK;

          
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


    }
}