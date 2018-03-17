using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;

namespace ModelInfo
{
    public class CConsPhoto
    {
//<xs:element name="PhotoName" type="xs:string" minOccurs="0"/>
//<xs:element name="FileName" type="xs:string" minOccurs="0"/>
//<xs:element name="FileSize" type="xs:double" minOccurs="0"/>
//<xs:element name="PhotoTime" type="xs:dateTime" minOccurs="0"/>
//<xs:element name="Longitude" type="xs:double" minOccurs="0"/>
//<xs:element name="Latitude" type="xs:double" minOccurs="0"/>
//<xs:element name="ReferenceSerialNo" type="xs:string" minOccurs="0"/>
//<xs:element name="Person" type="xs:string" minOccurs="0"/>
//<xs:element name="Remark" type="xs:string" minOccurs="0"/>

    //    public static 

        public static int findConsPhoto(int topN, out string[] photoName, out string[] fileName, out string[] photoTime, out double[] longitude, out double[] latitude,out string[] sNo,out string[] person, out string[] remark )
        {
            int num = 0;
            photoName = fileName = photoTime = sNo = person = remark = null;
            longitude = latitude = null;

            //DataTable dt = CServerWrapper.findConsInfo(DateTime.Now.AddDays(-30).Date.ToString("u"));
            DataTable dt = CServerWrapper.findLatestImage(topN);
            num = dt.Rows.Count;

            if (num == 0) return 0;

            photoName = new string[num];
            fileName = new string[num];
            photoTime = new string[num];
            sNo = new string[num];
            person = new string[num];
            remark = new string[num];
            longitude = new double[num];
            latitude = new double[num];

            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                photoName[i] = dr["PhotoName"].ToString();
                fileName[i] = CGisDataSettings.gCurrentProject.projectUrl + "/" +  dr["FileName"].ToString();
          //      Console.WriteLine(fileName[i]);
                photoTime[i] = dr["PhotoTime"].ToString();
                sNo[i] = dr["ReferenceSerialNo"].ToString();
                person[i] = dr["Person"].ToString();
                remark[i] = dr["Remark"].ToString();

                longitude[i] = Convert.ToDouble(dr["Longitude"]);
                latitude[i] = Convert.ToDouble(dr["Latitude"]); //FIXME 更新Web 服务
                i++;


            }
            return num;
        }

    }
}
