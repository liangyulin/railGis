using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Xml;
using System.IO;

namespace ModelInfo
{
    public class ProjectConfig
    {
        public string projectName;
        public string projectUrl;
        public string userName;
        public string userPSD;
        public string projectLocalPath;
    }

    public class CGisDataSettings
    {
        public static string gDataPath = @"D:\GISData\";
        public static List<ProjectConfig> gProjectList = new List<ProjectConfig>();
        public static ProjectConfig gCurrentProject;

        public static void initGisDataSettings(string path = @"D:\GISData\"){
            if (string.IsNullOrEmpty(path))
                path = @"D:\GISData\";
            gDataPath = path;
            LoadConfig();
        }

        private static void LoadConfig()
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                //gConfigFile = Application.StartupPath + @"\application.config" ;
                string sConfigFile =gDataPath+ "application.config" ;
                if (!File.Exists(sConfigFile))
                {
                    // FIXME 如果配置文件不存在，使用缺省设置，缺省设置如何合理？
                    throw new Exception("路径：" + sConfigFile + "的配置文件不存在");                    
                }
                xmlDoc.Load(sConfigFile);
                
                XmlNodeList nodeList = xmlDoc.SelectNodes("/Configuration/Project");

                foreach (XmlNode xxNode in nodeList)
                {
                    //XmlNodeList childList = xxNode.ChildNodes; //取得row下的子节点集合
                    ProjectConfig pc = new ProjectConfig();
                    pc.projectName = xxNode.Attributes["ProjectName"].Value;
                    pc.projectUrl = xxNode.Attributes["ProjectUrl"].Value;
                    pc.userName = xxNode.Attributes["UserName"].Value;
                    pc.projectLocalPath = xxNode.Attributes["ProjectLocalPath"].Value;
                    gProjectList.Add(pc);

                }
                //FIXME 取第一个工程，如果一个工程也没有如何处理
                gCurrentProject = gProjectList.First();

                //gConnectStr = xmlDoc.DocumentElement["ConnectionString"].InnerText.Trim();
                //gConnectStrLocal = xmlDoc.DocumentElement["ConnectionStringLocal"].InnerText.Trim();
                //gDataPath = xmlDoc.DocumentElement["DataPath"].InnerText.Trim();
                //gUserIP = xmlDoc.DocumentElement["UserIP"].InnerText.Trim();
                //gUserName = xmlDoc.DocumentElement["UserName"].InnerText.Trim();
                //gProjectPath = xmlDoc.DocumentElement["ProjectPath"].InnerText.Trim();


            }
            catch (Exception ex)
            {
                Console.WriteLine("CConfig.LoadConfig() fail,error:" + ex.Message);
                Environment.Exit(-1);
            }
        }

        //FIXME 程序退出时保存用户状态
        public static void UpdateConfigInfo()
        {

            //xml.Load(gConfigFile);

            //xmlDoc.DocumentElement["UserName"].InnerText = gUserName;
            //xmlDoc.DocumentElement["UserIP"].InnerText = gUserIP;

            //xmlDoc.Save(gConfigFile);

        }
    }
}
