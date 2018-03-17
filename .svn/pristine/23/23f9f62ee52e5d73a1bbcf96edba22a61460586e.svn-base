using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using ModelInfo;

namespace RailwayGIS
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createNew;
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out createNew))
            {
                if (createNew)
                {
                    Application.EnableVisualStyles(); 
                    Application.SetCompatibleTextRenderingDefault(false);
                    //GlobalSettings.gDataPath = System.Configuration.ConfigurationManager.AppSettings["DataPath"];
                    CGisDataSettings.initGisDataSettings(System.Configuration.ConfigurationManager.AppSettings["DataPath"]);

                    Application.Run(new GISForm());

                }
                else
                {
                    MessageBox.Show("应用程序已经运行一个实例了");
                    System.Threading.Thread.Sleep(1000);
                    System.Environment.Exit(1);
                }
            }
        }

    }
}
