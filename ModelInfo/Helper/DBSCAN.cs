using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo.Helper;

namespace ModelInfo
{

    /// <summary>
    /// dym
    /// </summary>
    public struct point
    {//定义点对象，包含经纬度坐标、是否核心点、是否已分配聚类ID属性,ID，是否噪声
        public double longitude;
        public double latitude;
        public Boolean is_core;//标记是否为核心点
        public Boolean is_clusterID;//标记是否已分配聚类ID
        public int cluster_ID;//属于那一簇;
        public Boolean is_noise;//标记是否噪声
    }

    public class DBSCAN
    {
        //DBSCAN所用全局变量；
        public static List<point> setOfPoints = new List<point>();//点集用于存放所有点
        public static double Eps;//点的半径
        public static int minPts;//点的邻居临界值(领域内最少点的数量)
        public static int clustered_points_count;//已分类点的数量
        public List<point> neighbour = new List<point>();
        public static int[] p_count;//每一簇内的点数
        public static List<point> center;//聚类中心集合


        public static void Cluster()
        {
            //DBSCAN所用主方法语句
            Eps = 2000;
            minPts = 10;
            clustered_points_count = 0;
            string[] usrName;
            string[] projName;
            string[] projDWName;
            double[] x;
            double[] y;

            int num;
            num = CConsLog.findLast7Cons(out usrName, out projName, out projDWName, out x, out y);

            //for (int i = 0; i < num; i++)
            //{
            //    Console.WriteLine("{0} #\t: User {1}\t Project {2}\t  x {3}\t y  {4}\t  Date {5}", i, usrName[i], projName[i], x[i], y[i], consDate[i]);
            //}

            setOfPoints = getPointsSet(x, y);//构造点集
            Console.WriteLine("setOfPoints has already been generated!!");

            setOfPoints = judge_core();//预处理，标记核心点,初始化聚类标记为false
            Console.WriteLine("pre_process has been done!!");
            DBSCAN_algorithm();//聚类

        }

        // DBSCAN 所用方法
        public static void DBSCAN_algorithm()
        {
            int i = 0;
            int clusters_count = 0;//聚类簇数;
            center = new List<point>();

            for (i = 0; i < setOfPoints.Count; i++)
            {
                if (setOfPoints[i].is_core == true && setOfPoints[i].is_clusterID == false)//
                {//选择一个尚未分类的核心点作为种子开始聚类
                    clusters_count++;
                    if (clusters_count % 53 == 0)
                    {
                        //Console.WriteLine(clusters_count + " clusters generated!!");
                        int clustered = checkClustered();
                        Console.WriteLine(clustered + " points has been clustered!");
                    }
                    expand_Cluster((point)setOfPoints[i], clusters_count);//给所有与此点density-connect的点标记聚类ID
                }
                Console.WriteLine(i + "  points finished!!");

            }

            p_count = new int[clusters_count];//每一簇内的点数
            center = centroid_calculate(clusters_count);
            /* double pre_long = 0;
             double pre_lati = 0;
             foreach (point perPoint in center)
             {
                 //sm.WriteLine(perPoint.longitude + "," + perPoint.latitude + "," + perPoint.cluster_ID);
                 pre_long = perPoint.longitude + 115;
                 pre_lati = perPoint.latitude + 39;

              }*/

            for (int q = 0; q < center.Count; q++)
                Console.WriteLine(q + "("+ center[q].longitude + ","+ center[q].latitude + ")"+ center[q].cluster_ID);

        }
        ////检测点集聚类情况
        public static int checkClustered()
        {
            int count = 0;
            foreach (point points in setOfPoints)
            {
                if (points.is_clusterID)
                {
                    count++;
                }
            }
            return count;
        }
        ////计算各类的图心坐标
        public static List<point> centroid_calculate(int clusters_count)
        {
            double long_sum = 0;
            double lati_sum = 0;
            double cen_long = 0;
            double cen_lati = 0;
            int points_count = 0;
            
            //every_count[0] = 0;
            List<point> centroid_list = new List<point>();

            for (int i = 1; i < clusters_count+1; i++)
            {
                for (int j = 0; j < setOfPoints.Count; j++)
                {
                    if (setOfPoints[j].cluster_ID == i)
                    {
                        points_count++;
                        long_sum = long_sum + setOfPoints[j].longitude;
                        lati_sum = lati_sum + setOfPoints[j].latitude;
                    }
                }
                p_count[i-1] = points_count;

                cen_long = long_sum / points_count;
                cen_lati = lati_sum / points_count;
                point centroid = new point();
                centroid.longitude = cen_long;
                centroid.latitude = cen_lati;
                centroid.cluster_ID = i;
                centroid_list.Add(centroid);

                long_sum = 0;
                lati_sum = 0;
                points_count = 0;
            }

            return centroid_list;
        }
        ////读取数据，构造对象点集
        public static List<point> getPointsSet(double[] x, double[] y)
        {
            List<point> setOfPoints = new List<point>();

            double to_long = 0;
            double to_lati = 0;
 
            for (int i = 0; i < x.Length; i++)
            {
                to_long = x[i];
                to_lati = y[i];

                point thepoint = new point();
                thepoint.longitude = to_long;
                thepoint.latitude = to_lati;
                thepoint.is_core = false;
                thepoint.is_clusterID = false;
                thepoint.is_noise = false;
                setOfPoints.Add(thepoint);

            }



            return setOfPoints;
        }
        ////计算两点间的距离
        public static double dist(point a, point b)
        {
            #region
            double deta_long = 0;
            double deta_lat = 0;//计算经纬度度数差
            // double deta_x = 0;
            //double deta_y = 0;//经纬度换算成公里
            double distance = 0;//计算两点间距离

            deta_long = Math.Abs(a.longitude - b.longitude);
            deta_lat = Math.Abs(a.latitude - b.latitude);
            // deta_x = 85.276 * deta_long;
            // deta_y = 110.94 * deta_lat;
            distance = Math.Sqrt(deta_long * deta_long + deta_lat * deta_lat);

            return distance;
            #endregion
        }
        ////获取特定点的邻居数列
        public static List<point> get_Neighbour(point thePoint)
        {
            double distance;
            List<point> neighbour = new List<point>();

            foreach (point points in setOfPoints)
            {
                //distance = dist(thePoint, points);
                distance = CoordinateConverter.getUTMDistance(thePoint.longitude, thePoint.latitude, points.longitude, points.latitude );
                if (distance <= Eps)
                {
                    neighbour.Add(points);
                }
            }
            neighbour.Remove(thePoint);
            return neighbour;
        }
        ////遍历所有点，确定核心点
        public static List<point> judge_core()
        {
            int count = 0;
            point thepoint = new point();
            List<point> neighbour = new List<point>();
            for (int i = 0; i < setOfPoints.Count; i++)
            {
                neighbour = get_Neighbour((point)setOfPoints[i]);
                thepoint = (point)setOfPoints[i];

                if (neighbour.Count >= minPts)
                {
                    thepoint.is_core = true;
                    setOfPoints[i] = thepoint;
                    count++;
                }
                else
                {
                    //默认非核心点
                }
            }
            Console.WriteLine("There are " + count + "  core_points in total!!");
            return setOfPoints;
        }
        ////改变全局点集特定点的分类ID
        public static void change_ID(point thePoint, int current_ID)
        {
            //  StreamWriter sw = File.AppendText(@"D:\singleDay\clustered_point.txt");
            for (int i = 0; i < setOfPoints.Count; i++)
            {
                if (setOfPoints[i].longitude == thePoint.longitude && setOfPoints[i].latitude == thePoint.latitude && !setOfPoints[i].is_clusterID)
                {
                    point indexPoint = new point();
                    indexPoint = setOfPoints[i];
                    indexPoint.is_clusterID = true;
                    indexPoint.cluster_ID = current_ID;
                    setOfPoints[i] = indexPoint;
                    //   sw.WriteLine(i);
                    //   sw.Close();
                    break;
                }
            }
        }

        public static void expand_Cluster(point thePoint, int current_clusterID)
        {
            List<point> seeds = get_Neighbour(thePoint);//直接密度可达点;
            point ano_Point = new point();//neighbour中的第一个点;

            change_ID(thePoint, current_clusterID);


            foreach (point samPoint in seeds)//
            {//则其邻居全部属于此类
                if (samPoint.is_clusterID == false)
                {
                    change_ID(samPoint, current_clusterID);
                }

            }
            seeds.Remove(thePoint);//好用

            while (seeds.Count > 0)
            {
                ano_Point = (point)seeds.First();//取第一个点，好用

                if (ano_Point.is_core == true)
                {
                    List<point> neighbour = get_Neighbour(ano_Point);
                    for (int k = 0; k < neighbour.Count; k++)//
                    {
                        neighbour = get_Neighbour(ano_Point);//更新邻居中已被分类的
                        if (neighbour[k].is_clusterID == false)
                        {
                            seeds.Add(neighbour[k]);
                            change_ID(neighbour[k], current_clusterID);
                        }

                    }
                }
                //seeds.Remove(seeds[0]);
                seeds.Remove(ano_Point);
            }

        }





    }
}








