using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel; 

namespace ModelInfo
{
    public interface IHotSpot
    {
        ////[CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("02工点类型")]
        //String ProfessionalName { get;  }  //工点类型  :桥，路基，涵洞，站点，其他


        ////[CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("01工点名称")]
        //string ProjectName
        //{ get;  } 
       
        ////[CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("03所属标段")]
        //string SegmentName
        //{ get;  } 

 
        ////[CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("08整体进度")]
        //double AvgProgress
        //{ get;  }

        string DKCode
        {
            get;
        }

        double CenterMileage
        {
            get;
        }
        double CenterLongitude
        { get; }
        double CenterLatitude
        {
            get;
        }
        double GlobalMileage
        {
            get;
        }
        string ShowMessage
        { get; }
        

    }
}
