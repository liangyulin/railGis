using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInfo
{

    public class CoordinateConverter
    {
        /* Ellipsoid model constants (actual values here are for WGS84) */
        static double sm_a = 6378137.0;
        static double sm_b = 6356752.314;
        static double sm_EccSquared = 6.69437999013e-03;
        static double UTMScaleFactor = 0.9996;
        static int zoneJQ = 50;
        //static double x, y, log, lat;

        /*
        *
        * Converts between degrees and radians.
        *
        */
        static double DegToRad(double deg)
        {
            return (deg / 180.0 * Math.PI);
        }
        static double RadToDeg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        /*
        * ArcLengthOfMeridian 经线长度
        *
        * Computes the ellipsoidal distance from the equator to a point at a
        * given latitude.
        *
        * Reference: Hoffmann-Wellenhof, B., Lichtenegger, H., and Collins, J.,
        * GPS: Theory and Practice, 3rd ed.  New York: Springer-Verlag Wien, 1994.
        *
        * Inputs:
        *     phi - Latitude of the point, in radians.
        *
        * Globals:
        *     sm_a - Ellipsoid model major axis.
        *     sm_b - Ellipsoid model minor axis.
        *
        * Returns:
        *     The ellipsoidal distance of the point from the equator, in meters.
        *
        */
        static double ArcLengthOfMeridian(double phi)
        {
            double alpha, beta, gamma, delta, epsilon, n;
            double result;

            /* Precalculate n */
            n = (sm_a - sm_b) / (sm_a + sm_b);

            /* Precalculate alpha */
            alpha = ((sm_a + sm_b) / 2.0) * (1.0 + (Math.Pow(n, 2.0) / 4.0) + (Math.Pow(n, 4.0) / 64.0));

            /* Precalculate beta */
            beta = (-3.0 * n / 2.0) + (9.0 * Math.Pow(n, 3.0) / 16.0) + (-3.0 * Math.Pow(n, 5.0) / 32.0);

            /* Precalculate gamma */
            gamma = (15.0 * Math.Pow(n, 2.0) / 16.0) + (-15.0 * Math.Pow(n, 4.0) / 32.0);

            /* Precalculate delta */
            delta = (-35.0 * Math.Pow(n, 3.0) / 48.0) + (105.0 * Math.Pow(n, 5.0) / 256.0);

            /* Precalculate epsilon */
            epsilon = (315.0 * Math.Pow(n, 4.0) / 512.0);

            /* Now calculate the sum of the series and return */
            result = alpha * (phi + (beta * Math.Sin(2.0 * phi)) + (gamma * Math.Sin(4.0 * phi)) + (delta * Math.Sin(6.0 * phi)) + (epsilon * Math.Sin(8.0 * phi)));

            return result;
        }

        /*
        * UTMCentralMeridian
        *
        * Determines the central meridian for the given UTM zone. 时区zone的中心经线
        *
        * Inputs:
        *     zone - An integer value designating the UTM zone, range [1,60].
        *
        * Returns:
        *   The central meridian for the given UTM zone, in radians, or zero
        *   if the UTM zone parameter is outside the range [1,60].
        *   Range of the central meridian is the radian equivalent of [-177,+177].
        *
        */
        static double UTMCentralMeridian(int zone)
        {
            return DegToRad(-183.0 + (zone * 6.0));
        }

        /*
        * FootpointLatitude
        *
        * Computes the footpoint latitude for use in converting transverse
        * Mercator coordinates to ellipsoidal coordinates.
        *
        * Reference: Hoffmann-Wellenhof, B., Lichtenegger, H., and Collins, J.,
        *   GPS: Theory and Practice, 3rd ed.  New York: Springer-Verlag Wien, 1994.
        *
        * Inputs:
        *   y - The UTM northing coordinate, in meters.
        *
        * Returns:
        *   The footpoint latitude, in radians.
        *
        */
        static double FootpointLatitude(double y)
        {
            double y_, alpha_, beta_, gamma_, delta_, epsilon_, n;
            double result;

            /* Precalculate n (Eq. 10.18) */
            n = (sm_a - sm_b) / (sm_a + sm_b);

            /* Precalculate alpha_ (Eq. 10.22) */
            /* (Same as alpha in Eq. 10.17) */
            alpha_ = ((sm_a + sm_b) / 2.0) * (1 + (Math.Pow(n, 2.0) / 4) + (Math.Pow(n, 4.0) / 64));

            /* Precalculate y_ (Eq. 10.23) */
            y_ = y / alpha_;

            /* Precalculate beta_ (Eq. 10.22) */
            beta_ = (3.0 * n / 2.0) + (-27.0 * Math.Pow(n, 3.0) / 32.0) + (269.0 * Math.Pow(n, 5.0) / 512.0);

            /* Precalculate gamma_ (Eq. 10.22) */
            gamma_ = (21.0 * Math.Pow(n, 2.0) / 16.0) + (-55.0 * Math.Pow(n, 4.0) / 32.0);

            /* Precalculate delta_ (Eq. 10.22) */
            delta_ = (151.0 * Math.Pow(n, 3.0) / 96.0) + (-417.0 * Math.Pow(n, 5.0) / 128.0);

            /* Precalculate epsilon_ (Eq. 10.22) */
            epsilon_ = (1097.0 * Math.Pow(n, 4.0) / 512.0);

            /* Now calculate the sum of the series (Eq. 10.21) */
            result = y_ + (beta_ * Math.Sin(2.0 * y_)) + (gamma_ * Math.Sin(4.0 * y_)) + (delta_ * Math.Sin(6.0 * y_)) + (epsilon_ * Math.Sin(8.0 * y_));

            return result;
        }

        /*
        * MapLatLonToXY
        *
        * Converts a latitude/longitude pair to x and y coordinates in the
        * Transverse Mercator projection.  Note that Transverse Mercator is not
        * the same as UTM; a scale factor is required to convert between them.
        *
        * Reference: Hoffmann-Wellenhof, B., Lichtenegger, H., and Collins, J.,
        * GPS: Theory and Practice, 3rd ed.  New York: Springer-Verlag Wien, 1994.
        *
        * Inputs:
        *    phi - Latitude of the point, in radians.
        *    lambda - Longitude of the point, in radians.
        *    lambda0 - Longitude of the central meridian to be used, in radians.
        *
        * Outputs:
        *    xy - A 2-element array containing the x and y coordinates
        *         of the computed point.
        *
        * Returns:
        *    The function does not return a value.
        *
        */
        static void MapLatLonToXY(double phi, double lambda, double lambda0, out double x, out double y)
        {
            double N, nu2, ep2, t, t2, l;
            double l3coef, l4coef, l5coef, l6coef, l7coef, l8coef;
            double tmp;

            /* Precalculate ep2 */
            ep2 = (Math.Pow(sm_a, 2.0) - Math.Pow(sm_b, 2.0)) / Math.Pow(sm_b, 2.0);

            /* Precalculate nu2 */
            nu2 = ep2 * Math.Pow(Math.Cos(phi), 2.0);

            /* Precalculate N */
            N = Math.Pow(sm_a, 2.0) / (sm_b * Math.Sqrt(1 + nu2));

            /* Precalculate t */
            t = Math.Tan(phi);
            t2 = t * t;
            tmp = (t2 * t2 * t2) - Math.Pow(t, 6.0);

            /* Precalculate l */
            l = lambda - lambda0;

            /* Precalculate coefficients for l**n in the equations below
            so a normal human being can read the expressions for easting
            and northing
            -- l**1 and l**2 have coefficients of 1.0 */
            l3coef = 1.0 - t2 + nu2;

            l4coef = 5.0 - t2 + 9 * nu2 + 4.0 * (nu2 * nu2);

            l5coef = 5.0 - 18.0 * t2 + (t2 * t2) + 14.0 * nu2 - 58.0 * t2 * nu2;

            l6coef = 61.0 - 58.0 * t2 + (t2 * t2) + 270.0 * nu2 - 330.0 * t2 * nu2;

            l7coef = 61.0 - 479.0 * t2 + 179.0 * (t2 * t2) - (t2 * t2 * t2);

            l8coef = 1385.0 - 3111.0 * t2 + 543.0 * (t2 * t2) - (t2 * t2 * t2);

            /* Calculate easting (x) */
            x = N * Math.Cos(phi) * l + (N / 6.0 * Math.Pow(Math.Cos(phi), 3.0) * l3coef * Math.Pow(l, 3.0))
                + (N / 120.0 * Math.Pow(Math.Cos(phi), 5.0) * l5coef * Math.Pow(l, 5.0))
                + (N / 5040.0 * Math.Pow(Math.Cos(phi), 7.0) * l7coef * Math.Pow(l, 7.0));

            /* Calculate northing (y) */
            y = ArcLengthOfMeridian(phi)
                + (t / 2.0 * N * Math.Pow(Math.Cos(phi), 2.0) * Math.Pow(l, 2.0))
                + (t / 24.0 * N * Math.Pow(Math.Cos(phi), 4.0) * l4coef * Math.Pow(l, 4.0))
                + (t / 720.0 * N * Math.Pow(Math.Cos(phi), 6.0) * l6coef * Math.Pow(l, 6.0))
                + (t / 40320.0 * N * Math.Pow(Math.Cos(phi), 8.0) * l8coef * Math.Pow(l, 8.0));
        }



        /*
        * MapXYToLatLon
        *
        * Converts x and y coordinates in the Transverse Mercator projection to
        * a latitude/longitude pair.  Note that Transverse Mercator is not
        * the same as UTM; a scale factor is required to convert between them.
        *
        * Reference: Hoffmann-Wellenhof, B., Lichtenegger, H., and Collins, J.,
        *   GPS: Theory and Practice, 3rd ed.  New York: Springer-Verlag Wien, 1994.
        *
        * Inputs:
        *   x - The easting of the point, in meters.
        *   y - The northing of the point, in meters.
        *   lambda0 - Longitude of the central meridian to be used, in radians.
        *
        * Outputs:
        *   philambda - A 2-element containing the latitude and longitude
        *               in radians.
        *
        * Returns:
        *   The function does not return a value.
        *
        * Remarks:
        *   The local variables Nf, nuf2, tf, and tf2 serve the same purpose as
        *   N, nu2, t, and t2 in MapLatLonToXY, but they are computed with respect
        *   to the footpoint latitude phif.
        *
        *   x1frac, x2frac, x2poly, x3poly, etc. are to enhance readability and
        *   to optimize computations.
        *
        */
        static void MapXYToLatLon(double x, double y, double lambda0, out double lat, out double lgt)
        {
            double phif, Nf, Nfpow, nuf2, ep2, tf, tf2, tf4, cf;
            double x1frac, x2frac, x3frac, x4frac, x5frac, x6frac, x7frac, x8frac;
            double x2poly, x3poly, x4poly, x5poly, x6poly, x7poly, x8poly;

            /* Get the value of phif, the footpoint latitude. */
            phif = FootpointLatitude(y);

            /* Precalculate ep2 */
            ep2 = (Math.Pow(sm_a, 2.0) - Math.Pow(sm_b, 2.0)) / Math.Pow(sm_b, 2.0);

            /* Precalculate cos (phif) */
            cf = Math.Cos(phif);

            /* Precalculate nuf2 */
            nuf2 = ep2 * Math.Pow(cf, 2.0);

            /* Precalculate Nf and initialize NfMath.Pow */
            Nf = Math.Pow(sm_a, 2.0) / (sm_b * Math.Sqrt(1 + nuf2));
            Nfpow = Nf;

            /* Precalculate tf */
            tf = Math.Tan(phif);
            tf2 = tf * tf;
            tf4 = tf2 * tf2;

            /* Precalculate fractional coefficients for x**n in the equations
            below to simplify the expressions for latitude and longitude. */
            x1frac = 1.0 / (Nfpow * cf);

            Nfpow *= Nf;   /* now equals Nf**2) */
            x2frac = tf / (2.0 * Nfpow);

            Nfpow *= Nf;   /* now equals Nf**3) */
            x3frac = 1.0 / (6.0 * Nfpow * cf);

            Nfpow *= Nf;   /* now equals Nf**4) */
            x4frac = tf / (24.0 * Nfpow);

            Nfpow *= Nf;   /* now equals Nf**5) */
            x5frac = 1.0 / (120.0 * Nfpow * cf);

            Nfpow *= Nf;   /* now equals Nf**6) */
            x6frac = tf / (720.0 * Nfpow);

            Nfpow *= Nf;   /* now equals Nf**7) */
            x7frac = 1.0 / (5040.0 * Nfpow * cf);

            Nfpow *= Nf;   /* now equals Nf**8) */
            x8frac = tf / (40320.0 * Nfpow);

            /* Precalculate polynomial coefficients for x**n.
            -- x**1 does not have a polynomial coefficient. */
            x2poly = -1.0 - nuf2;

            x3poly = -1.0 - 2 * tf2 - nuf2;

            x4poly = 5.0 + 3.0 * tf2 + 6.0 * nuf2 - 6.0 * tf2 * nuf2 - 3.0 * (nuf2 * nuf2) - 9.0 * tf2 * (nuf2 * nuf2);

            x5poly = 5.0 + 28.0 * tf2 + 24.0 * tf4 + 6.0 * nuf2 + 8.0 * tf2 * nuf2;

            x6poly = -61.0 - 90.0 * tf2 - 45.0 * tf4 - 107.0 * nuf2 + 162.0 * tf2 * nuf2;

            x7poly = -61.0 - 662.0 * tf2 - 1320.0 * tf4 - 720.0 * (tf4 * tf2);

            x8poly = 1385.0 + 3633.0 * tf2 + 4095.0 * tf4 + 1575 * (tf4 * tf2);

            /* Calculate latitude */
            lat = phif + x2frac * x2poly * (x * x) + x4frac * x4poly * Math.Pow(x, 4.0) + x6frac * x6poly * Math.Pow(x, 6.0) + x8frac * x8poly * Math.Pow(x, 8.0);

            /* Calculate longitude */
            lgt = lambda0 + x1frac * x + x3frac * x3poly * Math.Pow(x, 3.0) + x5frac * x5poly * Math.Pow(x, 5.0) + x7frac * x7poly * Math.Pow(x, 7.0);
        }

        /*
        * UTMXYToLatLon
        *
        * Converts x and y coordinates in the Universal Transverse Mercator
        * projection to a latitude/longitude pair.
        *
        * Inputs:
        *	x - The easting of the point, in meters.
        *	y - The northing of the point, in meters.
        *	zone - The UTM zone in which the point lies.
        *	southhemi - True if the point is in the southern hemisphere;
        *               false otherwise.
        *
        * Outputs:
        *	latlon - A 2-element array containing the latitude and
        *            longitude of the point, in radians.
        *
        * Returns:
        *	The function does not return a value.
        *
        */
        public static void UTMXYToLatLon(double x, double y, out double lat , out double lgt, bool southhemi = false )
        {
            double cmeridian;

            x -= 500000.0;
            x /= UTMScaleFactor;

            /* If in southern hemisphere, adjust y accordingly. */
            if (southhemi)
                y -= 10000000.0;

            y /= UTMScaleFactor;

            cmeridian = UTMCentralMeridian(zoneJQ);
            MapXYToLatLon(x, y, cmeridian, out lat, out lgt);
            lat = lat * 180.0 / Math.PI;
            lgt = lgt * 180.0 / Math.PI;
        }

        public static void UTMXYToLatLonList(int count, double[] x, double[] y, out double[] lgt, out double[] lat,  bool southhemi = false  )
        {

            double cmeridian = UTMCentralMeridian(zoneJQ);
            double xx, yy, t = 180 / Math.PI;
            lgt = new double[count];
            lat = new double[count];
            for (int i = 0; i < count; i++)
            {
                xx = x[i];
                yy = y[i];
                xx -= 500000.0;
                xx /= UTMScaleFactor;

                /* If in southern hemisphere, adjust y accordingly. */
                if (southhemi)
                    yy -= 10000000.0;

                yy /= UTMScaleFactor;
                MapXYToLatLon(xx, yy, cmeridian, out lat[i], out lgt[i]);
                lat[i] *= t;
                lgt[i] *= t;
            }      

        }



        public static void LatLonToUTMXY(double lat, double lon, out double x, out double y)
        {
            //zone = (int)((lon + 180.0) / 6) + 1;
            MapLatLonToXY(lat / 180.0 * Math.PI, lon / 180.0 * Math.PI, UTMCentralMeridian(zoneJQ), out x, out y);

            /* Adjust easting and northing for UTM system. */
            x = x * UTMScaleFactor + 500000.0;
            y = y * UTMScaleFactor;
            if (y < 0.0)
                y += 10000000.0;
        }


        public static void LatLonToUTMXYList(int count, double[] lat, double[] lon, out double[] x, out double[] y)
        {
            //int zone = (int)((lon[0] + 180.0) / 6) + 1;
            double t = UTMCentralMeridian(zoneJQ);
            double a = Math.PI / 180;
            x = new double[count];
            y = new double[count];
            for (int i = 0; i < count; i++)
            {
                MapLatLonToXY(lat[i] * a, lon[i] * a, t, out x[i], out y[i]);

                /* Adjust easting and northing for UTM system. */
                x[i] = x[i] * UTMScaleFactor + 500000.0;
                y[i] = y[i] * UTMScaleFactor;
                if (y[i] < 0.0)
                    y[i] += 10000000.0;
            }
        }

        public static double[] LatLonToYawList(int count, double[] lat, double[] lon, out double[] x, out double[] y, out double[] heading)
        {
            //int zone = (int)((lon[0] + 180.0) / 6) + 1;
            double t = UTMCentralMeridian(zoneJQ);
            x = new double[count];
            y = new double[count];
            heading = new double[count];
            double dx, dy, l, a = Math.PI / 180;
            for (int i = 0; i < count; i++)
            {
                MapLatLonToXY(lat[i] * a, lon[i] * a, t, out x[i], out y[i]);

                /* Adjust easting and northing for UTM system. */
                x[i] = x[i] * UTMScaleFactor + 500000.0;
                y[i] = y[i] * UTMScaleFactor;
                if (y[i] < 0.0)
                    y[i] += 10000000.0;
   //             Console.WriteLine(x[i] + "\t" + y[i]);
            }
            if (count == 2){
                dx = x[1] - x[0];
                dy = y[1] - y[0];
                l = Math.Sqrt(dx * dx + dy * dy);
                heading[0] = 180 * Math.Acos(dy / l) / Math.PI;
                if (dx < 0)
                    heading[0] = 360 - heading[0];
                heading[1] = heading[0];
            }                
            else
            {
                for (int i = 1; i < count - 1; i++)
                {
                    dx = x[i + 1] - x[i - 1];
                    dy = y[i + 1] - y[i - 1];
                    l = Math.Sqrt(dx * dx + dy * dy);
                    heading[i] = 180 * Math.Acos(dy / l) / Math.PI;
                    if (dx < 0)
                        heading[i] = 360 - heading[i];

                }
                heading[0] = heading[1];
                heading[count - 1] = heading[count - 2];
            }
            return heading;
        }

        /* (latin,lonin) 沿着yaw方向 偏移 angleOffset角度，disOff（米）距离得到的点（latout, lonout）
         */
        public static void LatLonOffest(double latin, double lonin, double yaw, double angleOff, double disOff, out double latout, out double lonout)
        {
            double x,y,xd,yd, a;
            //int zone;
            a = (360 - yaw - angleOff + 90) * Math.PI / 180;
            LatLonToUTMXY(latin, lonin, out x, out y);
            xd = x + disOff * Math.Cos(a);
            yd = y + disOff * Math.Sin(a);
            UTMXYToLatLon(xd, yd, out latout, out lonout,  latin<0);
        }

        /// <summary>
        /// 计算两GPS点距离，返回以米为单位
        /// </summary>
        /// <param name="longitude1"></param>
        /// <param name="latitude1"></param>
        /// <param name="longitude2"></param>
        /// <param name="latitude2"></param>
        /// <returns></returns>
        public static double getUTMDistance(double longitude1, double latitude1, double longitude2, double latitude2)
        {
            double len = 0;
            double xx1, yy1, xx2, yy2;
            //int z1, z2;
            //CoordinateConverter.UTMXYToLatLon(517670.66484, 4032205.24460, out xx, out yy, 50, false);
            CoordinateConverter.LatLonToUTMXY(latitude1, longitude1, out xx1, out yy1);
            CoordinateConverter.LatLonToUTMXY(latitude2, longitude2, out xx2, out yy2);
            len = Math.Sqrt((xx1 - xx2) * (xx1 - xx2) + (yy1 - yy2) * (yy1 - yy2));
            return len;

        }

    }

    // 另一种简易计算方法，代码中没有使用
    public class Coordinate
    {


        /// <summary>
        /// GPS经纬度换算成x,y坐标
        /// </summary>

        public static void LngLa2XY(double l, double B, out double xc, out double yc)
        {
            try
            {
                l = l * Math.PI / 180;
                B = B * Math.PI / 180;


                double Pi = Math.PI;
                double SinB = Math.Sin(B);
                double e;
                double K = T(out e);

                double tan = Math.Tan(Pi / 4 + B / 2);
                double E2 = Math.Pow((1 - e * SinB) / (1 + e * SinB), e / 2);
                double yy = tan * E2;

                yc = K * Math.Log(yy);
                xc = K * l;
                return;
            }
            catch (Exception ErrInfo)
            {
            }
            xc = -1;
            yc = -1;
        }

        public static void LngLa2XYArray(int count, double[] l, double[] B, out double[] xc, out double[] yc)
        {

            double e;
            double K = T(out e);
            xc = new double[count];
            yc = new double[count];
            double li, bi;
            for (int i = 0; i < count; i++)
            {
                li = l[i] * Math.PI / 180;
                bi = B[i] * Math.PI / 180;


                //double Pi = Math.PI;
                double SinB = Math.Sin(B[i]);


                double tan = Math.Tan(Math.PI / 4 + bi / 2);
                double E2 = Math.Pow((1 - e * SinB) / (1 + e * SinB), e / 2);
                double yy = tan * E2;

                yc[i] = K * Math.Log(yy);
                xc[i] = K * li;
            }


        }

        private static double T(out double e)
        {
            double B0 = 30 * Math.PI / 180;

            double N = 0, a = 0, b = 0, e2 = 0, K = 0;
            a = 6378137;
            b = 6356752.3142;
            e = Math.Sqrt(1 - (b / a) * (b / a));
            e2 = Math.Sqrt((a / b) * (a / b) - 1);
            double CosB0 = Math.Cos(B0);
            N = (a * a / b) / Math.Sqrt(1 + e2 * e2 * CosB0 * CosB0);
            K = N * CosB0;
            return K;
        }

        // 根据经纬度求heading
        public static double yaw(double x1, double y1, double x2, double y2)
        {
            double heading = 0; ;
            double xc1, yc1, xc2, yc2;
            double dx, dy, l;
            LngLa2XY(x1, y1, out  xc1, out  yc1);
            LngLa2XY(x2, y2, out  xc2, out  yc2);
            dx = xc2 - xc1;
            dy = yc2 - yc1;
            l = Math.Sqrt(dx * dx + dy * dy);
            heading = 180 * Math.Acos(dy / l) / Math.PI;
            if (dx < 0)
                heading = 360 - heading;
            return heading;
        }


        // 根据中线经纬度计算每个点的heading
        public static double[] yawArray(int count, double[] x, double[] y)
        {
            double[] heading = new double[count];
            double[] xc, yc;
            double dx, dy, l;

            if (count == 2)
                heading[0] = heading[1] = yaw(x[0], y[0], x[1], y[1]);
            else
            {
                LngLa2XYArray(count, x, y, out xc, out yc);

                for (int i = 1; i < count - 1; i++)
                {
                    dx = xc[i + 1] - xc[i - 1];
                    dy = yc[i + 1] - yc[i - 1];
                    l = Math.Sqrt(dx * dx + dy * dy);
                    heading[i] = 180 * Math.Acos(dy / l) / Math.PI;
                    if (dx < 0)
                        heading[i] = 360 - heading[i];

                }
                heading[0] = heading[1];
                heading[count - 1] = heading[count - 2];
            }


            return heading;
        }

    }

}

public class UTM2GPS
{

    /// <summary>

    /// 高斯投影由经纬度(Unit:DD)反算大地坐标(含带号，Unit:Metres)

    /// </summary>

    /// <param name="longitude">经度,以度为单位</param>

    /// <param name="latitude">纬度,以度为单位</param>

    /// <returns></returns>
    private static double DmsToRad(double x) {
        return x * Math.PI / 180.0;
    }

    public static void GaussProjInvCal(double X, double Y, out double  longitude, out double latitude)
    {
        int ProjNo; int ZoneWide; ////带宽
        double longitude1, latitude1, longitude0, latitude0, X0, Y0, xval, yval;
        double e1, e2, f, a, ee, NN, T, C, M, D, R, u, fai, iPI;
        iPI = 0.0174532925199433; ////3.1415926535898/180.0;
        a = 6378245.0; f = 1.0 / 298.3; //54年北京坐标系参数
        ////a=6378140.0; f=1/298.257; //80年西安坐标系参数
        ZoneWide = 6; ////6度带宽
        ProjNo =  (int)(X / 1000000L); //查找带号
        longitude0 = (ProjNo - 1) * ZoneWide + ZoneWide / 2;
        longitude0 = longitude0 * iPI; //中央经线
        X0 = ProjNo * 1000000L + 500000L;
        Y0 = 0;
        xval = X - X0; yval = Y - Y0; //带内大地坐标
        e2 = 2 * f - f * f;
        e1 = (1.0 - Math.Sqrt(1 - e2)) / (1.0 + Math.Sqrt(1 - e2));
        ee = e2 / (1 - e2);
        M = yval;
        u = M / (a * (1 - e2 / 4 - 3 * e2 * e2 / 64 - 5 * e2 * e2 * e2 / 256));
        fai = u + (3 * e1 / 2 - 27 * e1 * e1 * e1 / 32) * Math.Sin(2 * u) + (21 * e1 * e1 / 16 - 55 * e1 * e1 * e1 * e1 / 32) * Math.Sin(
        4 * u)
        + (151 * e1 * e1 * e1 / 96) * Math.Sin(6 * u) + (1097 * e1 * e1 * e1 * e1 / 512) * Math.Sin(8 * u);
        C = ee * Math.Cos(fai) * Math.Cos(fai);
        T = Math.Tan(fai) * Math.Tan(fai);
        NN = a / Math.Sqrt(1.0 - e2 * Math.Sin(fai) * Math.Sin(fai));
        R = a * (1 - e2) / Math.Sqrt((1 - e2 * Math.Sin(fai) * Math.Sin(fai)) * (1 - e2 * Math.Sin(fai) * Math.Sin(fai)) * (1 - e2 * Math.Sin
        (fai) * Math.Sin(fai)));
        D = xval / NN;
        //计算经度(Longitude) 纬度(Latitude)
        longitude1 = longitude0 + (D - (1 + 2 * T + C) * D * D * D / 6 + (5 - 2 * C + 28 * T - 3 * C * C + 8 * ee + 24 * T * T) * D
        * D * D * D * D / 120) / Math.Cos(fai);
        latitude1 = fai - (NN * Math.Tan(fai) / R) * (D * D / 2 - (5 + 3 * T + 10 * C - 4 * C * C - 9 * ee) * D * D * D * D / 24
        + (61 + 90 * T + 298 * C + 45 * T * T - 256 * ee - 3 * C * C) * D * D * D * D * D * D / 720);
        //转换为度 DD
        longitude = longitude1 / iPI;
        latitude = latitude1 / iPI;
    }
    //public Point3D GaussProjCal(double longitude, double latitude)
    //{

    //    Point3D pnt = new Point3D();

    //    int ProjNo = 0; int ZoneWide; ////带宽

    //    double longitude1, latitude1, longitude0, X0, Y0, xval, yval;

    //    double e2, ee, NN, T, C, A, M;

    //    ZoneWide = paraDegree;

    //    ProjNo = (int)(longitude / ZoneWide);

    //    longitude0 = originLongitude;

    //    longitude1 = DmsToRad(longitude);//经度转换为弧度

    //    latitude1 = DmsToRad(latitude);//纬度转换为弧度

    //    e2 = 2 * base.flat - flat * flat;

    //    ee = e2 * (1.0 - e2);

    //    double sinB = Math.Sin(latitude1);

    //    double tanB = Math.Tan(latitude1);

    //    double cosB = Math.Cos(latitude1);

    //    NN = longRadius / Math.Sqrt(1.0 - e2 * sinB * sinB);

    //    T = tanB * tanB;

    //    C = ee * cosB * cosB;

    //    A = (longitude1 - longitude0) * cosB;

    //    double a0 = 1 - e2 / 4 - 3 * e2 * e2 / 64 - 5 * e2 * e2 * e2 / 256;

    //    double a2 = 3 * e2 / 8 + 3 * e2 * e2 / 32 + 45 * e2 * e2 * e2 / 1024;

    //    double a4 = 15 * e2 * e2 / 256 + 45 * e2 * e2 * e2 / 1024;

    //    double a6 = 35 * e2 * e2 * e2 / 3072;

    //    M = longRadius * (a0 * latitude1 - a2 * Math.Sin(2 * latitude1) + a4 * Math.Sin(4 * latitude1) - a6 * Math.Sin(6 * latitude1));

    //    xval = NN * (A + (1 - T + C) * A * A * A / 6 + (5 - 18 * T + T * T + 72 * C - 58 * ee) * A * A * A * A * A / 120);

    //    yval = M + NN * Math.Tan(latitude1) * (A * A / 2 + (5 - T + 9 * C + 4 * C * C) * A * A * A * A / 24

    //    + (61 - 58 * T + T * T + 600 * C - 330 * ee) * A * A * A * A * A * A / 720);

    //    //X0 = 1000000L * ProjNo + 500000L;

    //    X0 = 500000L;

    //    Y0 = 0;

    //    xval = xval + X0;

    //    yval = yval + Y0;

    //    pnt.X = xval;

    //    pnt.Y = yval;

    //    return pnt;

    //}

    ///// <summary>

    ///// 高斯投影由经纬度(Unit:DD)反算大地坐标(含带号，Unit:Metres)

    ///// </summary>

    ///// <param name="lola"></param>

    ///// <returns></returns>

    //public Point3D GaussProjCal(LoLatude lola)
    //{

    //    double lo = lola.ToLongitude();

    //    double la = lola.ToLatitude();

    //    return GaussProjCal(lo, la);

    //}

    ///// <summary>

    ///// 高斯投影由经纬度(Unit:DD)反算大地坐标(含带号，Unit:Metres)

    ///// </summary>

    ///// <param name="longitude">经度,以度为单位</param>

    ///// <param name="latitude">纬度,以度为单位</param>

    ///// <param name="x">X坐标</param>

    ///// <param name="y">Y坐标</param>

    //public void GaussProjCal(double longitude, double latitude, out double x, out double y)
    //{

    //    Point3D pnt = GaussProjCal(longitude, latitude);

    //    x = pnt.X;

    //    y = pnt.Y;

    //}

    ///// <summary>

    ///// 高斯投影由大地坐标(Unit:Metres)反算经纬度(Unit:DD)

    ///// </summary>

    ///// <param name="X">X坐标</param>

    ///// <param name="Y">Y坐标</param>

    ///// <param name="longitude">经度</param>

    ///// <param name="latitude">纬度</param>

    //public void GaussProjInvCal(double X, double Y, out double longitude, out double latitude)
    //{

    //    int ProjNo; int ZoneWide; ////带宽

    //    double longitude1, latitude1, longitude0, X0, Y0, xval, yval;

    //    double e1, e2, ee, NN, T, C, M, D, R, u, fai;

    //    ZoneWide = paraDegree; ////6度带宽

    //    ProjNo = (int)(X / 1000000L); //查找带号

    //    longitude0 = originLongitude; //中央经线

    //    //X0 = ProjNo * 1000000L + 500000L;

    //    X0 = 500000L;

    //    Y0 = 0;

    //    xval = X - X0;

    //    yval = Y - Y0; //带内大地坐标

    //    e2 = 2 * flat - flat * flat;

    //    double sqrtE2 = Math.Sqrt(1 - e2);

    //    e1 = (1.0 - sqrtE2) / (1.0 + sqrtE2);

    //    ee = e2 / (1 - e2);

    //    M = yval;

    //    double q0 = 1 - e2 / 4 - 3 * e2 * e2 / 64 - 5 * e2 * e2 * e2 / 256;

    //    double q2 = 3 * e1 / 2 - 27 * e1 * e1 * e1 / 32;

    //    double q4 = 21 * e1 * e1 / 16 - 55 * e1 * e1 * e1 * e1 / 32;

    //    double q6 = 151 * e1 * e1 * e1 / 96;

    //    double q8 = 1097 * e1 * e1 * e1 * e1 / 512;

    //    u = M / (longRadius * q0);

    //    fai = u + q2 * Math.Sin(2 * u) + q4 * Math.Sin(4 * u)

    //    + q6 * Math.Sin(6 * u) + q8 * Math.Sin(8 * u);

    //    double cosB = Math.Cos(fai);

    //    double tanB = Math.Tan(fai);

    //    double sinB = Math.Sin(fai);

    //    C = ee * cosB * cosB;

    //    T = tanB * tanB;

    //    NN = longRadius / Math.Sqrt(1.0 - e2 * sinB * sinB);

    //    R = longRadius * (1 - e2) / Math.Sqrt((1 - e2 * sinB * sinB) * (1 - e2 * sinB * sinB) * (1 - e2 * sinB * sinB));

    //    D = xval / NN;

    //    //计算经度(Longitude) 纬度(Latitude)

    //    longitude1 = longitude0 + (D - (1 + 2 * T + C) * D * D * D / 6 + (5 - 2 * C + 28 * T - 3 * C * C + 8 * ee + 24 * T * T) * D

    //    * D * D * D * D / 120) / cosB;

    //    latitude1 = fai - (NN * tanB / R) * (D * D / 2 - (5 + 3 * T + 10 * C - 4 * C * C - 9 * ee) * D * D * D * D / 24

    //    + (61 + 90 * T + 298 * C + 45 * T * T - 256 * ee - 3 * C * C) * D * D * D * D * D * D / 720);

    //    //转换为度 DD

    //    longitude = longitude1 / Pi2;

    //    latitude = latitude1 / Pi2;

    //}

    ///// <summary>

    ///// 大地坐标反算经纬度坐标

    ///// </summary>

    ///// <param name="x">X坐标</param>

    ///// <param name="y">Y坐标</param>

    ///// <returns></returns>

    //public LoLatude GaussProjInvCal(double x, double y)
    //{

    //    double lo, la;

    //    GaussProjInvCal(x, y, out lo, out la);

    //    lo = lo * Pi2;

    //    la = la * Pi2;

    //    LoLatude lola = new LoLatude();

    //    lola.Longitude = Angle.RadToDms(lo);

    //    lola.Latitude = Angle.RadToDms(la);

    //    return lola;

    //}

    ///// <summary>

    ///// 大地坐标反算经纬度坐标

    ///// </summary>

    ///// <param name="pnt">坐标点</param>

    ///// <returns></returns>

    //public LoLatude GaussProjInvCal(Point3D pnt)
    //{

    //    double lo, la;

    //    GaussProjInvCal(pnt.X, pnt.Y, out lo, out la);

    //    lo = lo * Pi2;

    //    la = la * Pi2;

    //    LoLatude lola = new LoLatude();

    //    lola.Longitude = Angle.RadToDms(lo);

    //    lola.Latitude = Angle.RadToDms(la);

    //    return lola;

    //}

}
//测试方法：

//static void Main(string[] args)
//       {
//           double a = CoordDispose.GetDistance(new Degree(116.412007, 39.947545), new Degree(116.412924, 39.947918));//116.416984,39.944959
//           double b = CoordDispose.GetDistanceGoogle(new Degree(116.412007, 39.947545), new Degree(116.412924, 39.947918));
//           Degree[] dd = CoordDispose.GetDegreeCoordinates(new Degree(116.412007, 39.947545), 102);
//           Console.WriteLine(a+" "+b);
//           Console.WriteLine(dd[0].X + "," + dd[0].Y );
//           Console.WriteLine(dd[3].X + "," + dd[3].Y);
//           Console.ReadLine();
//      }

