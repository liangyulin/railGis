using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;

namespace ModelInfo.Helper
{
    public class StaticCluster
    {
        public int numP = 10;
        //定义点集
        public double[] px ;
        public double[] py ;
        //判定是否为中心点
        public int[] iscenter ;
        //定义相似度矩阵
        public double[,] similarmatrix ;
        //定义消息
        public double[,] msga;
        public double[,] msgr  ;
        public double[,] oldmsga ;
        public double[,] oldmsgr ;
        //定义中值
        public double pk = 0;
        //定义阻尼系数
        public const double dampcv = 0.5;

        public void clusterProcess() {
            InitPoints();
            InitMsga();

            //第一步，创建相似度矩阵
            CreateSimilarMatrix();
            int i = 0;
            while (i < 2000)
            {
                //第二步，更新消息
                GetOldMsg();
                UpdateMsg();
                DampMsg();
                //第三步，判断中心点
                JudgeCenter();
                i++;
            }
            for (i = 0; i < numP; i++)
            {
                Console.Write(iscenter[i] + "\t");
            }
            //MessageBox.Show("计算结束  ");        
        }

        /// <summary>
        /// 初始化点集
        /// </summary>
        private void InitPoints()
        {
            string[] usrName;
            string[] projName;
            string[] projDWName;

            numP = CConsLog.findLast7Cons(out usrName, out projName, out projDWName, out px, out py);

                    //判定是否为中心点
        iscenter = new int[numP];
        //定义相似度矩阵
         similarmatrix = new double[numP, numP];
        //定义消息
         msga = new double[numP, numP];
         msgr = new double[numP, numP];
         oldmsga = new double[numP, numP];
         oldmsgr = new double[numP, numP];

            for (int k = 0; k < numP; k++)
                iscenter[k] = 0;
        }
        /// <summary>
        /// 初始化消息
        /// </summary>
        private void InitMsga()
        {
            for (int i = 0; i < numP; i++)
            {
                for (int j = 0; j < numP; j++)
                {
                    msga[i, j] = 0;
                    msgr[i, j] = 0;
                    oldmsga[i, j] = 0;
                    oldmsgr[i, j] = 0;
                }
            }
        }

        /// <summary>
        /// 计算欧式距离的相反数
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private double CalDistant(double x1, double x2, double y1, double y2)
        {
            return -(x1 - x2) * (x1 - x2) - (y1 - y2) * (y1 - y2);
        }

        /// <summary>
        /// 创建相似度矩阵.这里有点疑惑，是不是先按照常规计算再去算Pk？？
        /// 而且这个中值是不是要排除0？先不管它，当不排除
        /// </summary>
        private void CreateSimilarMatrix()
        {
            for (int i = 0; i < numP; i++)
            {
                for (int j = 0; j < numP; j++)
                {
                    similarmatrix[i, j] = CalDistant(px[i], px[j], py[i], py[j]);
                }
            }
            double median = ReturnMedian(similarmatrix);
            pk = median / 2;                                         //取中值作为preference的值
            for (int i = 0; i < numP; i++)
            {
                similarmatrix[i, i] = pk;
            }
        }
        /// <summary>
        /// 更新消息
        /// </summary>
        private void UpdateMsg()
        {
            //更新msgr.R(i,k)=S(i,k)- max{A(i,j)+S(i,j)}(j {1,2,……,N,但j≠k})
            for (int i = 0; i < numP; i++)
            {
                for (int k = 0; k < numP; k++)
                {
                    //首先得到max{A(i,j)+S(i,j)}(j!=k)
                    double max_tmp;
                    //赋初值
                    if (k == 0)
                    {
                        max_tmp = msga[i, 1] + similarmatrix[i, 1];
                    }
                    else
                    {
                        max_tmp = msga[i, 0] + similarmatrix[i, 0];
                    }
                    for (int j = 0; j < numP; j++)
                    {
                        if (j != k)
                        {
                            if (max_tmp < (msga[i, j] + similarmatrix[i, j]))
                            {
                                max_tmp = msga[i, j] + similarmatrix[i, j];
                            }
                        }
                    } //end get the max

                    msgr[i, k] = similarmatrix[i, k] - max_tmp;
                }
            }
            //更新msgr[k,k].R(k,k)=P(k)-max{A(k,j)+S(k,j)} (j {1,2,……,N,但j≠k})
            for (int i = 0; i < numP; i++)
            {
                double max_tmp;
                //赋初值
                if (i == 0)
                {
                    max_tmp = msga[i, 1] + similarmatrix[i, 1];
                }
                else
                {
                    max_tmp = msga[i, 0] + similarmatrix[i, 0];
                }
                for (int j = 0; j < numP; j++)
                {
                    if (j != i)
                    {
                        if ((msga[i, j] + similarmatrix[i, j]) > max_tmp)
                        {
                            max_tmp = msga[i, j] + similarmatrix[i, j];
                        }
                    }
                }
                msgr[i, i] = pk - max_tmp;
            }
            //为容易看先分开写
            //更新msga.A(i,k)=min{0,R(k,k)+  (j {1,2,……,N,但j≠i且j≠k})
            for (int i = 0; i < numP; i++)
            {
                for (int k = 0; k < numP; k++)
                {
                    //求得max部分
                    double sum_tmp = 0;
                    for (int j = 0; j < numP; j++)
                    {
                        if (j != i)
                        {
                            if (msgr[j, k] > 0)
                            {
                                sum_tmp += msgr[j, k];
                            }
                        }
                    } //end 求max部分
                    double addtmp = msgr[k, k] + sum_tmp;
                    if (addtmp < 0)
                    {
                        msga[i, k] = addtmp;
                    }
                    else
                    {
                        msga[i, k] = 0;
                    }
                }
            }//end 更新msga
        } //end updatemsg
        /// <summary>
        /// 保存到旧消息数组
        /// </summary>
        private void GetOldMsg()
        {
            for (int i = 0; i < numP; i++)
            {
                for (int k = 0; k < numP; k++)
                {
                    oldmsga[i, k] = msga[i, k];
                    oldmsgr[i, k] = msgr[i, k];
                }
            }
        }

        /// <summary>
        /// 加入阻尼系数
        /// </summary>
        private void DampMsg()
        {
            for (int i = 0; i < numP; i++)
            {
                for (int k = 0; k < numP; k++)
                {
                    msga[i, k] = (1 - dampcv) * msga[i, k] + dampcv * oldmsga[i, k]; ;
                    msgr[i, k] = (1 - dampcv) * msgr[i, k] + dampcv * oldmsgr[i, k];
                }
            }
        }

        /// <summary>
        /// 判断是否为中心点。
        /// </summary>
        private void JudgeCenter()
        {
            for (int k = 0; k < numP; k++)
            {
                if (msga[k, k] + msgr[k, k] >= 0)  //判定条件。。
                    iscenter[k] = 1;
            }
        }

        /// <summary>
        /// 返回二维数组的中值
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private double ReturnMedian(double[,] matrix)
        {
            double median = 0;
            double[] list = new double[matrix.GetLength(0) * matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    list[matrix.GetLength(0) * i + j] = matrix[i, j];
                }
            }
            Array.Sort(list);
            if (list.Length % 2 == 0)
            {
                median = (list[list.Length / 2] + list[list.Length / 2 - 1]) * 0.5;
            }
            else
            {
                median = list[Convert.ToInt32((list.Length - 1) * 0.5)];
            }
            return median;
        }

        /// <summary>
        /// 冒泡排序法
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public double[] BubbleSort(double[] list)
        {
            int i;
            double temp;
            for (int j = 0; j < list.Length; j++)
            {
                for (i = list.Length - 1; i > j; i--)
                {
                    if (list[j] < list[i])
                    {
                        temp = list[j];
                        list[j] = list[i];
                        list[i] = temp;
                    }
                }
            }
            return list;
        }
    }
}
