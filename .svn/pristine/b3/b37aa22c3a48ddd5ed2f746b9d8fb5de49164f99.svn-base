using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    public class CTEConsPhoto:CTERWItem
    {


        public CTEConsPhoto(CRailwayScene s, CTEScene ss, CRWTEStandard t)
            : base(s, ss, t)
        {

        }

        /// <summary>
        /// TODO 丁一明，显示聚类结果
        /// </summary>
        /// <param name="groupID"></param>
        public override void TECreate()
        {
            string[] photoName;
            string[] fileName;
            string[] photoTime;
            string[] sNo;
            string[] person;
            string[] remark;
            double[] x;
            double[] y;

            var sgworld = new SGWorld66();
            if (string.IsNullOrEmpty(mGroupIDStatic))
                mGroupIDStatic = sgworld.ProjectTree.CreateGroup("Photo");

            int num;
            num = CConsPhoto.findConsPhoto(5, out photoName, out fileName, out photoTime, out x, out y, out sNo, out person, out remark);
            //num = CConsLog.findLast365Cons(out usrName, out projName, out consDate, out x, out y);
            for (int i = 0; i < num; i++)
            {
                Console.WriteLine("{0} #\t: photoName {1}\t fileName {2}\t  x {3}\t y  {4}\t  Date {5}", i, photoName[i], fileName[i], x[i], y[i], photoTime[i]);
            }
        }

        public override void TEUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
