using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.Web.Services.Description;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;

using System.Data.OleDb;
using System.Data;

using Microsoft.CSharp;
using ModelInfo.Helper;

namespace ModelInfo
{
    public class CServerWrapper
    {
        public static string mWebUrl;
        public static bool isConnected = false;
        public static WebServiceAgent mLoginAgent = null;
        public static WebServiceAgent mProjectAgent = null;
        public static WebServiceAgent mGisAgent = null;
        
        //www.railmis.cn
        public static bool ConnectToServer(string surl = @"124.128.9.254:8888/jqmis")
        {
            string url;
            mWebUrl = surl;
            isConnected = false;
            try { 
                url = "http://" + mWebUrl + "/webservice/usrlogin.asmx";
                mLoginAgent = new WebServiceAgent(url);
                url = "http://" + mWebUrl + "/webservice/ProjectService.asmx";
                mProjectAgent = new WebServiceAgent(url);
                url = "http://" + mWebUrl + "/webservice/GisDataWebService.asmx";
                mGisAgent = new WebServiceAgent(url);


            }
            catch (Exception e)
            {
                Console.WriteLine("连接服务器失败");
                return false;
            }
            isConnected = true;
            return true;
        }
        
        
        public static string webLogin(string usrName, string usrPWD)
        {
            string resultStr = "NotConnected";
            if (isConnected)
            {
                resultStr = (string)mLoginAgent.Invoke("CheckUsrLogin", usrName, usrPWD);

            }
            return resultStr;
        }

        //public static bool get
        //    ws_GetUTMDistance (string dkpre, double mileage, out double x, out double y, out double z)
        /// <summary>
        /// ToDO 
        /// </summary>
        /// <param name="projSNo"></param>
        /// <returns></returns>
        public static DataSet findProjHistory(string projSNo)
        {
            DataSet ds = null;
            if (isConnected)
            {
                ds = (DataSet)mProjectAgent.Invoke("ws_Bind_ProjectProgress_HistoryRate_DataSet",projSNo);
            }
            return ds;
        }
    ////        [WebMethod(Description = "增量获取工点信息")]
    ////[System.Web.Services.Protocols.SoapHeader("AuthHeader")]
    //    public static List<CRailwayProject> findChangedProj(string updateTime)
    //    {
    //        List<CRailwayProject> proList = null;
    //        if (isConnected)
    //        {

    //            proList = (List<CRailwayProject>)mProjectAgent.Invoke("FindChangedProject", updateTime);
    //        }
    //        return proList;
    //    }

        public static string findProjectCode(string projStr) {
            return mGisAgent.Invoke("ws_GetSysConfigation", projStr).ToString();
        }

        public static DataTable findProjectInfo()
        {
            DataTable dt = null;
            if (isConnected)
            {
                dt = (DataTable)mGisAgent.Invoke("ws_FindProjectInfo",null);
            }
            return dt;
        }

        public static DataTable findDWProjectInfo()
        {
            DataTable dt = null;
            if (isConnected)
            {
                //FIX ME 365临时的，应该在服务端支持空串
                dt = (DataTable)mGisAgent.Invoke("ws_FindDWProjectInfo", DateTime.Now.AddDays(-365).Date.ToString("u")); 
            }
            return dt;
        }

        public static DataTable findChainInfo()
        {
            DataTable dt = null;
            if (isConnected)
            {
                dt = (DataTable)mGisAgent.Invoke("ws_FindChainInfo");
            }
            return dt;
        }

        public static DataTable findMileageInfo()
        {
            DataTable dt = null;
            if (isConnected)
            {
                dt = (DataTable)mGisAgent.Invoke("ws_FindMileageInfo");
            }
            return dt;
        }

        /// <summary>
        /// 由服务器获取实名制信息,取得实名制某日之后的人员工作情况,日期格式yyyy-mm-dd hh:mi:ss
        /// </summary>
        /// <param name="updateTime"></param>
        /// <returns></returns>
        public static DataTable findConsInfo(string fromTime = null, string toTime = null) {
            DataTable dt = null;
            if (isConnected)
            {
                dt = (DataTable)mGisAgent.Invoke("ws_FindConsInfo", fromTime,toTime);
            }
            return dt;
        }

        public static DataTable findClusterConsByProj(string fromTime = null, string toTime = null)
        {
            DataTable dt = null;
            if (isConnected)
            {
                dt = (DataTable)mGisAgent.Invoke("ws_FindClusterdConsbyProj", fromTime, toTime);
            }
            return dt;
        }


        public static DataTable findClusterConsByPDW(string fromTime = null, string toTime = null)
        {
            DataTable dt = null;
            if (isConnected)
            {
                dt = (DataTable)mGisAgent.Invoke("ws_FindClusterdConsbyDW", fromTime, toTime);
            }
            return dt;
        }
        /// <summary>
        /// 由服务器执行查询sql语句，调试用
        /// </summary>
        /// <param name="sql"></param>
        /// select * from (select * from FirmInfo)a, (select FirmTypeID,FirmTypeCategoryName from FirmTypeInfo)b where a.FirmTypeID=b.FirmTypeID and FirmTypeCategoryName='单位' or FirmTypeCategoryName='分支机构'
        /// <returns></returns>
        public static DataTable execSqlQuery(string sql)
        {


            DataTable dt = null;
            if (isConnected)
            {
                dt = (DataTable)mGisAgent.Invoke("ws_FindGeneralInfo", sql);
            }
            return dt;
        }

        /// <summary>
        /// 由服务器获取最近的topName个施工影像信息
        /// </summary>
        /// <param name="updateTime"></param> 
        /// <returns>
        /// <xs:sequence>
        //<xs:element name="PhotoName" type="xs:string" minOccurs="0"/>
        //<xs:element name="FileName" type="xs:string" minOccurs="0"/>
        //<xs:element name="FileSize" type="xs:double" minOccurs="0"/>
        //<xs:element name="PhotoTime" type="xs:dateTime" minOccurs="0"/>
        //<xs:element name="Longitude" type="xs:double" minOccurs="0"/>
        //<xs:element name="Latitude" type="xs:double" minOccurs="0"/>
        //<xs:element name="ReferenceSerialNo" type="xs:string" minOccurs="0"/>
        //<xs:element name="Person" type="xs:string" minOccurs="0"/>
        //<xs:element name="Remark" type="xs:string" minOccurs="0"/>
        ///</xs:sequence>
        /// </returns>
        public static DataTable findLatestImage(int topNum = 5)
        {
            DataTable dt = null;
            if (isConnected)
            {
                dt = (DataTable)mGisAgent.Invoke("ws_FindImageInfoTop", topNum);
            }
            return dt;
        }

    }


}
