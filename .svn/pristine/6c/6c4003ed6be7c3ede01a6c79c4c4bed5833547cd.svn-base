using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInfo.Helper
{
    public class GPSAdjust
    {
        static double pi = 3.14159265358979324;
	static double a = 6378245.0;
	static double ee = 0.00669342162296594323;
	static  double x_pi = 3.14159265358979324 * 3000.0 / 180.0;

	static bool outOfChina(double lat, double lon)
	{
		if (lon < 72.004 || lon > 137.8347)
			return true;
		if (lat < 0.8293 || lat > 55.8271)
			return true;
		return false;
	}

	static double transformLat(double x, double y)
	{
         
		double ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y + 0.2 * Math.Sqrt(Math.Abs(x));
		ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
		ret += (20.0 * Math.Sin(y * pi) + 40.0 * Math.Sin(y / 3.0 * pi)) * 2.0 / 3.0;
		ret += (160.0 * Math.Sin(y / 12.0 * pi) + 320 * Math.Sin(y * pi / 30.0)) * 2.0 / 3.0;
             
		return ret;
	}

	 static double transformLon(double x, double y)
	{
		double ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * Math.Sqrt(Math.Abs(x));
		ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
		ret += (20.0 * Math.Sin(x * pi) + 40.0 * Math.Sin(x / 3.0 * pi)) * 2.0 / 3.0;
		ret += (150.0 * Math.Sin(x / 12.0 * pi) + 300.0 * Math.Sin(x / 30.0 * pi)) * 2.0 / 3.0;
		return ret;
	}

    /**
     * 地球坐标转换为火星坐标
     * World Geodetic System ==> Mars Geodetic System
     *
     * @param wgLat  地球坐标
     * @param wgLon
     *
     * mglat,mglon 火星坐标
     */
     public static void transform2Mars(double wgLat, double wgLon,out double mgLat,out double mgLon)
    {
        if (outOfChina(wgLat, wgLon))
        {
            mgLat  = wgLat;
            mgLon = wgLon;
            return ;
        }
        double dLat = transformLat(wgLon - 105.0, wgLat - 35.0);
        double dLon = transformLon(wgLon - 105.0, wgLat - 35.0);
        double radLat = wgLat / 180.0 * pi;
        double magic = Math.Sin(radLat);
        magic = 1 - ee * magic * magic;
        double sqrtMagic = Math.Sqrt(magic);
        dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * pi);
        dLon = (dLon * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * pi);
        mgLat = wgLat + dLat;
        mgLon = wgLon + dLon;

    }

    /**
     * 火星坐标转换为百度坐标
     * @param gg_lat
     * @param gg_lon
     */
    public static void bd_encrypt(double gg_lat, double gg_lon,out double bd_lat,out double  bd_lon)
    {
        double x = gg_lon, y = gg_lat;
        double z = Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * x_pi);
        double theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * x_pi);
        bd_lon = z * Math.Cos(theta) + 0.0065;
        bd_lat = z * Math.Sin(theta) + 0.006;

    }

    /**
     * 百度转火星
     * @param bd_lat
     * @param bd_lon
     */
     public static void bd_decrypt(double bd_lat, double bd_lon,out double gg_lat, out double gg_lon)
    {
        double x = bd_lon - 0.0065, y = bd_lat - 0.006;
        double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * x_pi);
        double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * x_pi);
        gg_lon = z * Math.Cos(theta);
        gg_lat = z * Math.Sin(theta);

    }



//static void main() {
//    double lat = 30.227607;
//    double lon = 120.036565;

//    //真实的经纬度转化为百度地图上的经纬度，便于计算百度POI
//    double marsLat = 0;
//    double marsLon = 0;
//    double resultLat = 0;
//    double resultLon = 0;
//    transform2Mars(lat,lon,out marsLat,out marsLon);
//    bd_encrypt(marsLat,marsLon,out resultLat,out resultLon);

//    //30.2193456 120.0348264
//    //cout<<setprecision(10)<<resultLat<<" "<<setprecision(10)<<resultLon<<endl;

//}

    }
}
