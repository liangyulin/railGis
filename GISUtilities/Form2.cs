using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ModelInfo;
using System.Drawing.Drawing2D;

namespace GISUtilities
{
    //public partial class Form2 : Form
    //{
    //    public Form2()
    //    {
    //        InitializeComponent();
    //    }
    //}

    public partial class Form2 : Form
    {
        public const int num = 10;
        //定义点集
        public PointF[] points = new PointF[num];
        //判定是否为中心点
        public int[] iscenter = new int[num];
        //定义相似度矩阵
        public double[,] similarmatrix = new double[num, num];
        //定义消息
        public double[,] msga = new double[num, num];
        public double[,] msgr = new double[num, num];
        public double[,] oldmsga = new double[num, num];
        public double[,] oldmsgr = new double[num, num];
        //定义中值
        public double pk = 0;
        //定义阻尼系数
        public const double dampcv = 0.5;
        public Form2()
        {
            InitializeComponent();
            //pictureBox1.ImageLocation = @"http://124.128.9.254:8888/jqmis/ProjectPhoto/066/2015/%E8%AE%B8%E4%B8%96%E6%89%AC20151231001902.jpg";
            //InitPoints();
            //InitMsga();

            ////第一步，创建相似度矩阵
            //CreateSimilarMatrix();
            //int i = 0;
            //while (i < 2000)
            //{
            //    //第二步，更新消息
            //    GetOldMsg();
            //    UpdateMsg();
            //    DampMsg();
            //    //第三步，判断中心点
            //    JudgeCenter();
            //    i++;
            //}
            //for (i = 0; i < num; i++) {
            //    Console.Write(iscenter[i] + "\t");
            //}
            //MessageBox.Show("计算结束  ");
        }

        #region useless
        private void button1_Click(object sender, EventArgs e)
        {
            ///////////////测试找中值函数//////////////
            //double[,] aa = new double[4, 4];
            //for (int i = 0; i < 4; i++)
            //{
            //    for (int j = 0; j < 4; j++)
            //    {
            //        aa[i, j] = i * 4 + j;
            //    }
            //}
            //Console.WriteLine(ReturnMedian(aa).ToString());
            /////////////////////////////////////////////
            CServerWrapper.ConnectToServer();

            //    string sqlstr = @"select FirmName, FirmTypeID,from (select * from FirmInfo)a, (select FirmTypeID,FirmTypeCategoryName from FirmTypeInfo)b where a.FirmTypeID=b.FirmTypeID and( FirmTypeCategoryName='单位' or FirmTypeCategoryName='分支机构');";  and Longitude > 10 AND Latitude > 10
            string sqlstr = @"select FirmName ,a.FirmTypeID, CategoryCode, SerialNo, UpdateTime, Longitude, Latitude from (select * from FirmInfo)a, (select FirmTypeID,FirmTypeCategoryName from FirmTypeInfo)b where a.FirmTypeID=b.FirmTypeID and( FirmTypeCategoryName='单位' or FirmTypeCategoryName='分支机构') and Longitude > 10 AND Latitude > 10 order by a.FirmTypeID asc ;";

            // + " where a.FirmTypeID=b.FirmTypeID and (FirmTypeCategoryName='单位' or FirmTypeCategoryName='分支机构') and Longitude > 10 AND latitude > 10 ;";
            dataGridView1.DataSource = CServerWrapper.execSqlQuery(sqlstr);

        }

        /// <summary>
        /// 初始化点集
        /// </summary>
        private void InitPoints()
        {
            points[0].X = (float)0.1;
            points[0].Y = (float)0.1;
            points[1].X = (float)0.3;
            points[1].Y = (float)0.1;
            points[2].X = (float)0.3;
            points[2].Y = (float)0.3;
            points[3].X = (float)0.1;
            points[3].Y = (float)0.3;
            points[4].X = (float)0.2;
            points[4].Y = (float)0.2;
            points[5].X = (float)0.5;
            points[5].Y = (float)0.5;
            points[6].X = (float)0.7;
            points[6].Y = (float)0.5;
            points[7].X = (float)0.7;
            points[7].Y = (float)0.7;
            points[8].X = (float)0.5;
            points[8].Y = (float)0.7;
            points[9].X = (float)0.6;
            points[9].Y = (float)0.6;

            for (int k = 0; k < num; k++)
                iscenter[k] = 0;
        }
        /// <summary>
        /// 初始化消息
        /// </summary>
        private void InitMsga()
        {
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num; j++)
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
        private double CalDistant(PointF p1, PointF p2)
        {
            return -(p1.X - p2.X) * (p1.X - p2.X) - (p1.Y - p2.Y) * (p1.Y - p2.Y);
        }

        /// <summary>
        /// 创建相似度矩阵.这里有点疑惑，是不是先按照常规计算再去算Pk？？
        /// 而且这个中值是不是要排除0？先不管它，当不排除
        /// </summary>
        private void CreateSimilarMatrix()
        {
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    similarmatrix[i, j] = CalDistant(points[i], points[j]);
                }
            }
            double median = ReturnMedian(similarmatrix);
            pk = median / 2;                                         //取中值作为preference的值
            for (int i = 0; i < num; i++)
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
            for (int i = 0; i < num; i++)
            {
                for (int k = 0; k < num; k++)
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
                    for (int j = 0; j < num; j++)
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
            for (int i = 0; i < num; i++)
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
                for (int j = 0; j < num; j++)
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
            for (int i = 0; i < num; i++)
            {
                for (int k = 0; k < num; k++)
                {
                    //求得max部分
                    double sum_tmp = 0;
                    for (int j = 0; j < num; j++)
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
            for (int i = 0; i < num; i++)
            {
                for (int k = 0; k < num; k++)
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
            for (int i = 0; i < num; i++)
            {
                for (int k = 0; k < num; k++)
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
            for (int k = 0; k < num; k++)
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
        #endregion



        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics myGraphics = e.Graphics;
            List<double> ld = new List<double>();
            ld.Add(0.5);
            ld.Add(0.6);
            ld.Add(0.3);
            List<string> ls = new List<string>();
            ls.Add("测试1");
            ls.Add("测试2");
            ls.Add("测试3");

            Matrix m = new Matrix();

            myGraphics.DrawString("青阳隧道", new Font("宋体", 12), Brushes.Red, new PointF(5,  5));
            for (float f = 50; f < 500; f += 50)
                myGraphics.DrawLine(Pens.Black, 0, f, 500, f);
            //myGraphics.TranslateTransform(50, 50);
            m.Translate(50, 150);
            double width = 50;
            //myGraphics.Transform = m;
            draw_SubTunnelWithoutDate(myGraphics, ls, ld, width, m, 0, 150,10, 0);
            draw_SubTunnelWithoutDate(myGraphics, ls, ld, width, m, 150, 250,10, 1);
            draw_SubTunnelWithoutDate(myGraphics, ls, ld, width, m, 250, 300, 10, 0);
            draw_SubTunnelWithoutDate(myGraphics, ls, ld, width, m, 300, 400, 10, 1);
            draw_SubTunnelWithoutDate(myGraphics, ls, ld, width, m, 400, 450, 10, 0);
            draw_SubTunnelWithoutDate(myGraphics, ls, ld, width, m, 450, 600, 10, 1);
            draw_SubTunnelWithoutDate(myGraphics, ls, ld, width, m, 250, 300, 10, 2);
            draw_SubTunnelWithoutDate(myGraphics, ls, ld, width, m, 400, 480, 10, 3);
            //m.Translate(100, 75);
            //m.Rotate(90);
            //m.Shear(0, -0.5f);
            //myGraphics.TranslateTransform(100, 0);

            //myGraphics.RotateTransform(30);
            //myGraphics.Transform = m;
            //draw_SubTunnel(myGraphics, ld, ls, 100, 150, 0);
            //Pen myPen = new Pen(Color.Red, 5);
            //myGraphics.DrawEllipse(myPen, 0, 0, 100, 50);
            //myGraphics.ScaleTransform(1, 0.5f);
            //myGraphics.TranslateTransform(50, 0);
            //myGraphics.RotateTransform(30);
            //myPen.Color = Color.Blue;
            //myGraphics.DrawEllipse(myPen, 0, 0, 100, 50);
            //myPen.Dispose();

            //GraphicsPath myGraphicsPath = new GraphicsPath();
            //Rectangle myRectangle = new Rectangle(0, 0, 60, 60);
            ////myGraphicsPath.
            //myGraphicsPath.AddRectangle(myRectangle);

            // Fill the path on the new coordinate system.
            // No local transformation
            //myGraphics.FillPath(new SolidBrush(Color.Yellow), myGraphicsPath);
            //Pen widenPen = new Pen(Color.Black, 10);
            //Matrix widenMatrix = new Matrix();
            //widenMatrix.Translate(50, 50);
            //myPath.Widen(widenPen, widenMatrix, 1.0f);
            //// Draw the widened path to the screen in red.
            //e.Graphics.FillPath(new SolidBrush(Color.Red), myPath);
        }

        private void draw_Tunnel(Graphics myGraphics){
        }
        private double bwidth = 400;
        private double bheight = 150;
        private float scale = 1.0f;
        private Color[] backColor= new Color[]{Color.Red,Color.Green,Color.Blue,Color.Yellow,Color.Purple};
        //private double 
        /// <summary>
        /// 绘制隧道进度
        /// </summary>
        /// <param name="myGraphics"></param>
        /// <param name="description">分项工程描述</param>
        /// <param name="done">分项完成百分比</param>
        /// <param name="th">绘制宽度</param>
        /// <param name="pm">隧道坐标系矩阵</param>
        /// <param name="startM">起始里程，米</param>
        /// <param name="endM">终止里程，米</param>
        /// <param name="unitM">围岩等级，米</param>
        /// <param name="mode">绘制模式，小里程-》大里程，角度</param>
        private void draw_SubTunnelWithoutDate(Graphics myGraphics, List<string> description, List<double> done, double th, Matrix pm, double startM, double endM ,double unitM, int mode)
        {
            float totalLen = (float)((endM - startM) / scale);
            float unitLen = (float)unitM / scale;
            float sx = (float)(startM / scale);
            float sy  = -(float)(th / 2);
            //int doneLen;
            float height = (float)(th / (done.Count+1));
            RectangleF areaf;
            SolidBrush notDoneB = new SolidBrush(Color.LightGray);
            SolidBrush doneB = new SolidBrush(Color.FromArgb(50, Color.Red));
            Matrix m = new Matrix();
            pm = pm.Clone();
            Pen p = new Pen(Color.Black,2);

            
            p.DashStyle = DashStyle.DashDot;
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;  // 更正： 垂直居中
            format.Alignment = StringAlignment.Center;      // 水平居中

            switch (mode)
            {
                case 0:  // 小里程到大里程
                    m.Translate(sx, 0);
                    pm.Multiply(m);
                    myGraphics.Transform = pm;
                    
                    
                    p.StartCap = LineCap.RoundAnchor;
                    p.EndCap = LineCap.ArrowAnchor;
                    myGraphics.DrawLine(p, 0, sy+ height /2, totalLen, sy + height /2);

                    areaf = new RectangleF(0, sy, totalLen, height);
                    myGraphics.DrawString(startM + "~" + endM, new Font("宋体", 12), Brushes.Black, areaf,format);
                    sy += height;
                    // 逐条绘制分项工程
                    for (int i = 0; i < done.Count; i++)
                    {
                        
                        myGraphics.DrawRectangle(Pens.Black, 0, sy, totalLen, height);

                        if (unitLen < 4)
                        {
                            myGraphics.FillRectangle(new SolidBrush(backColor[i]), 0, sy, (float)(totalLen * done[i]), height);
                            myGraphics.FillRectangle(notDoneB, (float)(totalLen * done[i]) + 1, sy, (float)(totalLen * (1 - done[i])), height);
                        }
                        else
                        {
                            float j;
                            for (j = 1; j < (float)(totalLen * done[i]); j += unitLen)
                            {
                                myGraphics.FillRectangle(new SolidBrush(backColor[i]), j, sy + 1, unitLen - 2, height - 3);
                                myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, height - 3);
                            }



                            //myGraphics.DrawString(description[i] + " " + Math.Round(done[i] * 100, 2) + "%", new Font("宋体", 12), Brushes.Red, new PointF(5, y + 5));
                            for (; j < (float)(totalLen); j += unitLen)
                            {
                                myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, height - 3);
                            }
                        }
                        //myGraphics.DrawRectangle(Pens.Black, (int)(totalLen * done[i]) + 1, sy, (int)(totalLen * done[i]) % height - 1, height - 1);
                        sy += height;
                    }
                    break;
                case 1:// 大里程到小里程
                    m.Translate(sx, 0);
                    pm.Multiply(m);
                    myGraphics.Transform = pm;

                    areaf = new RectangleF(0, sy, totalLen,height);

                    p.StartCap = LineCap.ArrowAnchor;
                    p.EndCap = LineCap.RoundAnchor;
                    myGraphics.DrawLine(p, 0, sy+ height /2, totalLen, sy + height /2);
                    myGraphics.DrawString(startM + "~" + endM, new Font("宋体", 12), Brushes.Black, areaf, format);
                    sy += height;

                    for (int i = 0; i < done.Count; i++)
                    {
                        myGraphics.DrawRectangle(Pens.Black, 0, sy, totalLen, height);
                        if (unitLen < 4)
                        {
                            myGraphics.FillRectangle(notDoneB, 0, sy, (float)(totalLen * (1 - done[i])), height);
                            myGraphics.FillRectangle(new SolidBrush(backColor[i]), (float)(totalLen * (1 - done[i])) + 1, sy, (float)(totalLen * done[i]), height);
                        }
                        else
                        {
                            float j;
                            for (j = 1; j < (float)(totalLen * done[i]); j += unitLen)
                            {

                                myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, height - 3);
                            }


                            for (; j < (float)(totalLen); j += unitLen)
                            {
                                myGraphics.FillRectangle(new SolidBrush(backColor[i]), j, sy + 1, unitLen - 2, height - 3);
                                myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, height - 3);
                            }
                        }
                        //myGraphics.DrawString(description[i] + " " + Math.Round(done[i] * 100, 2) + "%", new Font("宋体", 12), Brushes.Red, new PointF(5, y + 5));
                        sy += height;
                    }
                    break;
                case 2:// 45度 斜井
                   m.Translate(sx, (int)(th / 2));
                   m.Rotate(90);
                   m.Shear(0, -0.5f);
                    pm.Multiply(m);
                    myGraphics.Transform = pm;

                    areaf = new RectangleF(0, sy, totalLen,height);

                    p.StartCap = LineCap.ArrowAnchor;
                    p.EndCap = LineCap.RoundAnchor;
                    myGraphics.DrawLine(p, 0, sy+ height /2, totalLen, sy + height /2);
                    myGraphics.DrawString(startM + "~" + endM, new Font("宋体", 12), Brushes.Black, areaf, format);
                    sy += height;
                    //height = height / 2;
                    //sy = sy / 2;
                    for (int i = 0; i < done.Count; i++)
                    {
                        myGraphics.DrawRectangle(Pens.Black, 0, sy, totalLen, height);

                        if (unitLen < 4)
                        {
                            myGraphics.FillRectangle(notDoneB, 0, sy, (float)(totalLen * (1 - done[i])), height);
                            myGraphics.FillRectangle(new SolidBrush(backColor[i]), (float)(totalLen * (1 - done[i])) + 1, sy, (float)(totalLen * done[i]), height);
                        }
                        else
                        {
                            float j;
                            for (j = 1; j < (float)(totalLen * done[i]); j += unitLen)
                            {

                                myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, height - 3);
                            }


                            for (; j < (float)(totalLen); j += unitLen)
                            {
                                myGraphics.FillRectangle(new SolidBrush(backColor[i]), j, sy + 1, unitLen - 2, height - 3);
                                myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, height - 3);
                            }
                        }
                        //myGraphics.FillRectangle(notDoneB, new Rectangle(0, sy, (int)(totalLen * (1 - done[i])), height));
                        //myGraphics.DrawRectangle(Pens.BlanchedAlmond, new Rectangle((int)(totalLen * (1 - done[i])) + 1, sy, (int)(totalLen * done[i]), height));
                        //myGraphics.FillRectangle(new SolidBrush(Color.LightBlue), new Rectangle((int)(totalLen * (1 - done[i])) + 1, sy, (int)(totalLen * done[i]), height));
                        //myGraphics.DrawString(Math.Round(done[i] * 100, 2) + "%", new Font("宋体", 12), Brushes.Red, new PointF(5, y + 5));
                        sy += height;
                    }
                    break;
                case 3: // 305度 斜井
                   m.Translate(sx, (int)(- th / 2));
                   m.Rotate(270);
                   m.Shear(0, 0.5f);
                    pm.Multiply(m);
                    myGraphics.Transform = pm;
                    areaf = new RectangleF(0, sy, totalLen,height);

                    p.StartCap = LineCap.ArrowAnchor;
                    p.EndCap = LineCap.RoundAnchor;
                    myGraphics.DrawLine(p, 0, sy+ height /2, totalLen, sy + height /2);
                    myGraphics.DrawString(startM + "~" + endM, new Font("宋体", 12), Brushes.Black, areaf, format);
                    sy += height;
                    //height = height / 2;
                    //sy = sy / 2;
                    for (int i = 0; i < done.Count; i++)
                    {
                        myGraphics.DrawRectangle(Pens.Black, 0, sy, totalLen, height);

                        if (unitLen < 4)
                        {
                            myGraphics.FillRectangle(notDoneB, 0, sy, (float)(totalLen * (1 - done[i])), height);
                            myGraphics.FillRectangle(new SolidBrush(backColor[i]), (float)(totalLen * (1 - done[i])) + 1, sy, (float)(totalLen * done[i]), height);
                        }
                        else
                        {
                            float j;
                            for (j = 1; j < (float)(totalLen * done[i]); j += unitLen)
                            {

                                myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, height - 3);
                            }


                            for (; j < (float)(totalLen); j += unitLen)
                            {
                                myGraphics.FillRectangle(new SolidBrush(backColor[i]), j, sy + 1, unitLen - 2, height - 3);
                                myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, height - 3);
                            }
                        }
                        //myGraphics.FillRectangle(notDoneB, new Rectangle(0, sy, (int)(totalLen * (1 - done[i])), height));
                        //myGraphics.DrawRectangle(Pens.BlanchedAlmond, new Rectangle((int)(totalLen * (1 - done[i])) + 1, sy, (int)(totalLen * done[i]), height));
                        //myGraphics.FillRectangle(new SolidBrush(Color.LightBlue), new Rectangle((int)(totalLen * (1 - done[i])) + 1, sy, (int)(totalLen * done[i]), height));
                        //myGraphics.DrawString(Math.Round(done[i] * 100, 2) + "%", new Font("宋体", 12), Brushes.Red, new PointF(5, y + 5));
                        sy += height;
                    }
                    break;       
                //case 4:
                //    m.Translate(x, 0);
                //    m.Multiply(pm);
                //    myGraphics.Transform = m;
                //    for (int i = 0; i < done.Count; i++)
                //    {
                        
                //        myGraphics.DrawRectangle(Pens.BlanchedAlmond, new Rectangle(0, y, (int)(totalLen * done[i]), height));
                //        myGraphics.FillRectangle(new SolidBrush(Color.LightBlue), new Rectangle(0, y, (int)(totalLen * done[i]), height));
                //        myGraphics.FillRectangle(notDoneB, new Rectangle((int)(totalLen * done[i]) + 1, y, (int)(totalLen * (1 - done[i])), height));
                //        myGraphics.DrawString(description[i] + " " + Math.Round(done[i] * 100, 2) + "%", new Font("宋体", Math.Abs(height - 10) / 2), Brushes.Red, new PointF(5, y + 5));
                //        y += height;
                //    }
                //    break;
                default:
                    Console.WriteLine(startM + "\t" + endM + " tunnel type error");
                    break;
            }
        }



    }
    public class ProgressGIS
    {
        private double bwidth = 400;
        private double bheight = 150;
        private double scale = 1.0;
        private void draw_Progress(Graphics myGraphics, List<double> progress, List<string> description, double tw, double th, int mode)
        {
            int totalLen = (int)tw;
            //int doneLen;
            int height = (int)(th / progress.Count);
            int y = -(int)(th / 2);
            SolidBrush notDoneB = new SolidBrush(Color.LightGray);
            for (int i = 0; i < progress.Count; i++)
            {
                myGraphics.DrawRectangle(Pens.BlanchedAlmond, new Rectangle(0, y, (int)(totalLen * progress[i]), height));
                myGraphics.FillRectangle(new SolidBrush(Color.LightBlue), new Rectangle(0, y, (int)(totalLen * progress[i]), height));
                myGraphics.FillRectangle(notDoneB, new Rectangle((int)(totalLen * progress[i]) + 1, y, (int)(totalLen * (1 - progress[i])), height));
                myGraphics.DrawString(description[i] + " " + Math.Round(progress[i] * 100, 2) + "%", new Font("宋体", Math.Abs(height - 10) / 2), Brushes.Red, new PointF(5, y + 5));
                y += height;
            }

        }
    }

}
