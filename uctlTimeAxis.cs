using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading;

namespace ToolFunction
{
    /// <summary>
    /// 相当于杂志社的角色
    /// </summary>
    public partial class uctlTimeAxis : UserControl
    {
        #region 图像配置


        /// <summary>
        /// 蓝色宽2
        /// </summary>
        static Pen p1 = new Pen(Color.Blue, 2);
        /// <summary>
        /// 绿色宽3
        /// </summary>
        static Pen p2 = new Pen(Color.Green, 3);
        /// <summary>
        /// 灰色宽1
        /// </summary>
        static Pen p3 = new Pen(Color.Gray, 1);
        /// <summary>
        /// 微软雅黑
        /// </summary>
        static Font f1 = new Font("微软雅黑", 9, FontStyle.Regular);
        /// <summary>
        /// 圆心X坐标
        /// </summary>
        public static int CenterOfTheCircleX = 31;
        /// <summary>
        /// 圆心Y坐标
        /// </summary>
        public static int CenterOfTheCircleY = 46;
        /// <summary>
        /// 说明条目X坐标
        /// </summary>
        public static int ItemStartX = 45;
        /// <summary>
        /// 条目X修正量
        /// </summary>
        public static int ItemFixX = -20;
        /// <summary>
        /// 条目Y修正量
        /// </summary>
        public static int ItemFixY = 10;
        public static Point CenterOfThePie = new Point(CenterOfTheCircleX, CenterOfTheCircleY);
        /// <summary>
        /// 命中Pie半径
        /// </summary>
        public static int PieRadius = 6;
        /// <summary>
        /// 白点Pie半径
        /// </summary>
        public static int PieRadius2 = 8;
        /// <summary>
        /// 圆半径
        /// </summary>
        public static int CircleRadius = 9;
        /// <summary>
        /// 圆心距
        /// </summary>
        public static int CircleSpace = 80;
        /// <summary>
        /// 轴X起点
        /// </summary>
        public static int LineStartX = CenterOfTheCircleX;
        /// <summary>
        /// 轴Y起点
        /// </summary>
        public static int LineStartY = CenterOfTheCircleY;
        /// <summary>
        /// 内部Key定义
        /// </summary>
        public static string Key = "";
        /// <summary>
        /// 绘图事件
        /// </summary>
        public event EventHandler<KeyValueEventArgs> KeyValueChangeEventHandler;
        /// <summary>
        /// 流程字典
        /// </summary>
        public static SortedDictionary<string, string> sdict = new SortedDictionary<string, string>();
        /// <summary>
        /// 水平标示
        /// </summary>
        private static readonly int HorizontalTimeAxis = 0;
        /// <summary>
        /// 垂直标志
        /// </summary>
        private static readonly int VerticalTimeAxis = 1;

        /// <summary>
        /// 绘制标志，0为水平；1为垂直。
        /// </summary>
        public static int TimeAxisModle = 0;
       
        #endregion

        public uctlTimeAxis()
        {
            InitializeComponent();
        }

        public uctlTimeAxis(SortedDictionary<string, string> s)
        {
            InitializeComponent();
            if (sdict != null)
            {
                sdict = s;
            }
        }

        public uctlTimeAxis(SortedDictionary<string, string> s, int model)
        {
            InitializeComponent();
            if (sdict != null)
            {
                sdict = s;
                TimeAxisModle = model;
                if (HorizontalTimeAxis==TimeAxisModle)
                {
                    KeyValueChangeEventHandler += new EventHandler<KeyValueEventArgs>(SetHorizontalStep);
                }
                else if (VerticalTimeAxis==TimeAxisModle)
                {
                     KeyValueChangeEventHandler += new EventHandler<KeyValueEventArgs>(SetVerticalStep);
                }
            }
        }

        public void SetKeyValue(string s)
        {
            if (uctlTimeAxis.sdict.Keys.Contains(s))
            {
                EventHandler<KeyValueEventArgs> l = KeyValueChangeEventHandler;
                if (l != null)
                {
                    l(this, new KeyValueEventArgs(s));
                }
            }
           
        }

        /// <summary>
        /// 初始化水平进图轴
        /// 2015-04-15
        /// 吴海龙
        /// </summary>
        public void InitHorizontalTimeAxis()
        {
            try
            {
                int TempCenterOfTheCircleX = CenterOfTheCircleX;
                using (Graphics g = this.CreateGraphics())
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;  //使绘图质量最高，即消除锯齿
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.DrawString("正在执行:", f1, Brushes.Black, new PointF(CenterOfTheCircleX, 5));
                    g.DrawLine(p3, new Point(TempCenterOfTheCircleX, CenterOfTheCircleY), new Point((sdict.Keys.Count - 1) * CircleSpace + CenterOfTheCircleX, CenterOfTheCircleY));
                    foreach (var item in sdict.Keys)
                    {
                        g.FillEllipse(Brushes.Gray, TempCenterOfTheCircleX - CircleRadius, CenterOfTheCircleY - CircleRadius, CircleRadius * 2, CircleRadius * 2);
                        g.FillEllipse(Brushes.White, TempCenterOfTheCircleX - PieRadius2, CenterOfTheCircleY - PieRadius2, PieRadius2 * 2, PieRadius2 * 2);
                        g.DrawString(sdict[item], f1, Brushes.DarkGray, new PointF(TempCenterOfTheCircleX + ItemFixX, CenterOfTheCircleY + ItemFixY));
                        TempCenterOfTheCircleX = TempCenterOfTheCircleX + CircleSpace;
                    }
                }
            }
            catch (Exception exp)
            {
                CommonFunction.WriteLog(exp, "绘制失败");
            }
        }

        /// <summary>
        /// 初始化垂直进度轴
        /// 2015-04-15
        /// 吴海龙
        /// </summary>
        public void InitVerticalTimeAxis()
        {
            try
            {
                int TempCenterOfThePieY = CenterOfTheCircleY;
                using (Graphics g = this.CreateGraphics())
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;  //使绘图质量最高，即消除锯齿
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.DrawString("正在执行:", f1, Brushes.Black, new PointF(CenterOfTheCircleX, 5));
                    g.DrawLine(p3, new Point(LineStartX, TempCenterOfThePieY), new Point(LineStartX, (sdict.Keys.Count - 1) * CircleSpace + TempCenterOfThePieY));
                    foreach (var item in sdict.Keys)
                    {
                        g.FillEllipse(Brushes.Gray, CenterOfTheCircleX - CircleRadius, TempCenterOfThePieY - CircleRadius, CircleRadius * 2, CircleRadius * 2);
                        g.FillEllipse(Brushes.White, CenterOfTheCircleX - PieRadius2, TempCenterOfThePieY - PieRadius2, PieRadius2 * 2, PieRadius2 * 2);
                        g.DrawString(sdict[item], f1, Brushes.DarkGray, new PointF(ItemStartX, TempCenterOfThePieY - ItemFixY));
                        TempCenterOfThePieY = TempCenterOfThePieY + CircleSpace;
                    }
                }
            }
            catch (Exception exp)
            {
                CommonFunction.WriteLog(exp, "绘制失败");
            }
            //-------------

            //try
            //{
            //    int x = 20;
            //    int y = 20;
            //    using (Graphics g = this.CreateGraphics())
            //    {
            //        g.SmoothingMode = SmoothingMode.HighQuality;  //使绘图质量最高，即消除锯齿
            //        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //        g.CompositingQuality = CompositingQuality.HighQuality;
            //        g.DrawString("正在执行:", f1, Brushes.Black, new PointF(x, 5));
            //        g.DrawLine(p3, new Point(35, y), new Point(35, sdict.Keys.Count * 80));
            //        foreach (var item in sdict.Keys)
            //        {
            //            g.FillEllipse(Brushes.Gray, 25, y + 20, 18, 18);
            //            g.FillEllipse(Brushes.White, 26, y + 21, 16, 16);
            //            g.DrawString(sdict[item], f1, Brushes.DarkGray, new PointF(45, y+20));
            //            y = y + 80;
            //        }
            //    }
            //}
            //catch (Exception exp)
            //{
            //    CommonFunction.WriteLog(exp, "绘制失败");
            //}
        }

        /// <summary>
        /// 由外部graphics初始化时间轴.
        /// </summary>
        /// <param name="graphics"></param>
        public static void InitTimeAxis(Graphics graphics)
        {
            try
            {
                int x = 20;
                using (Graphics g = graphics)
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;  //使绘图质量最高，即消除锯齿
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    Point topLeft = new Point(5, 5);
                    Size howBig = new Size(sdict.Keys.Count * 80, 5);
                    Rectangle rectangleArea = new Rectangle(topLeft, howBig);
                    g.DrawString("正在执行:", f1, Brushes.Black, new PointF(x, 5));
                    g.DrawLine(p3, new Point(x, 35), new Point(sdict.Keys.Count * 80, 35));
                    foreach (var item in sdict.Keys)
                    {
                        g.FillEllipse(Brushes.Gray, x + 20, 25, 18, 18);
                        g.FillEllipse(Brushes.White, x + 21, 26, 16, 16);
                        g.DrawString(sdict[item], f1, Brushes.DarkGray, new PointF(x, 45));
                        x = x + 80;
                    }
                }
            }
            catch (Exception exp)
            {
                CommonFunction.WriteLog(exp, "绘制失败");
            }
        }

        /// <summary>
        /// 初始化字典
        /// 2015-04-15
        /// 吴海龙
        /// 废弃不用，已改为在构造函数中初始化字典。
        /// </summary>
        /// <param name="d"></param>
        public void SetDict(SortedDictionary<string, string> d)
        {
            sdict = d;
        }

        /// <summary>
        /// 执行水平step
        /// 2015-04-16
        /// 吴海龙
        /// </summary>
        /// <param name="key"></param>
        public void SetHorizontalStep()
        {
            if ("" == Key)
            {
                return;
            }
            int x = 20;
            using (Graphics g = this.CreateGraphics())
            {
                g.SmoothingMode = SmoothingMode.HighQuality;  //使绘图质量最高，即消除锯齿
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                foreach (var item in sdict.Keys)
                {
                    g.DrawLine(p2, new Point(10, 35), new Point(x + 20, 35));
                    g.DrawString(sdict[item], f1, Brushes.Green, new PointF(x, 45));
                    g.FillEllipse(Brushes.White, x + 20, 25, 18, 18);
                    g.DrawEllipse(p3, x + 20, 25, 18, 18);
                    g.FillEllipse(Brushes.Green, x + 23, 28, 12, 12);
                    if (item == Key)
                    {
                        break;
                    }
                    x = x + 80;
                }
            }

        }


        /// <summary>
        /// 执行垂直step
        /// 2015-04-16
        /// 吴海龙
        /// </summary>
        /// <param name="key"></param>
        public void SetVerticalStep()
        {
            if ("" == Key)
            {
                return;
            }
            int x = 20;
            int y = 20;
            using (Graphics g = this.CreateGraphics())
            {
                g.SmoothingMode = SmoothingMode.HighQuality;  //使绘图质量最高，即消除锯齿
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                foreach (var item in sdict.Keys)
                {
                    g.DrawString(sdict[item], f1, Brushes.Green, new PointF(45, y + 20));
                    g.FillEllipse(Brushes.White, 25, y + 20, 18, 18);
                    g.DrawEllipse(p3, 25, y + 20, 18, 18);
                    g.DrawLine(p2, new Point(34, 20), new Point(34, y + 25));
                    g.FillEllipse(Brushes.Green, 28, y + 23, 12, 12);
                    if (item == Key)
                    {
                        break;
                    }
                    y = y + 80;
                }
            }
        }


        /// <summary>
        /// 执行step
        /// 2015-04-15
        /// 吴海龙
        /// </summary>
        /// <param name="key"></param>
        public void SetVerticalStep(string key)
        {
            if ("" == KeyValueEventArgs.Key)
            {
                return;
            }
            int x = 20;
            int y = 20;
            using (Graphics g = this.CreateGraphics())
            {
                g.SmoothingMode = SmoothingMode.HighQuality;  //使绘图质量最高，即消除锯齿
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                foreach (var item in sdict.Keys)
                {
                    g.DrawString(sdict[item], f1, Brushes.Green, new PointF(45, y + 20));
                    g.FillEllipse(Brushes.White, 25, y + 20, 18, 18);
                    g.DrawEllipse(p3, 25, y + 20, 18, 18);
                    g.DrawLine(p2, new Point(34, 20), new Point(34, y + 25));
                    g.FillEllipse(Brushes.Green, 28, y + 23, 12, 12);
                    if (item == KeyValueEventArgs.Key)
                    {
                        break;
                    }
                    y = y + 80;
                }
            }
        }


        /// <summary>
        /// 执行step
        /// 2015-04-15
        /// 吴海龙
        /// </summary>
        /// <param name="key"></param>
        public static void SetStep(Graphics graphics, string key)
        {
            int x = 20;
            using (Graphics g = graphics)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;  //使绘图质量最高，即消除锯齿
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                foreach (var item in sdict.Keys)
                {
                    g.DrawLine(p2, new Point(10, 35), new Point(x + 20, 35));
                    g.DrawString(sdict[item], f1, Brushes.Green, new PointF(x, 45));
                    g.FillEllipse(Brushes.White, x + 20, 25, 18, 18);
                    g.DrawEllipse(p3, x + 20, 25, 18, 18);
                    g.FillEllipse(Brushes.Green, x + 23, 28, 12, 12);
                    if (item == key)
                    {
                        break;
                    }
                    x = x + 80;
                }

            }
        }

        /// <summary>
        /// 执行step
        /// 2015-04-15
        /// 吴海龙
        /// </summary>
        /// <param name="key"></param>
        public static void SetStep(Control control, string key)
        {
            int x = 20;
            using (Graphics g = control.CreateGraphics())
            {
                g.SmoothingMode = SmoothingMode.HighQuality;  //使绘图质量最高，即消除锯齿
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                foreach (var item in sdict.Keys)
                {
                    g.DrawLine(p2, new Point(10, 35), new Point(x + 20, 35));
                    g.DrawString(sdict[item], f1, Brushes.Green, new PointF(x, 45));
                    g.FillEllipse(Brushes.White, x + 20, 25, 18, 18);
                    g.DrawEllipse(p3, x + 20, 25, 18, 18);
                    g.FillEllipse(Brushes.Green, x + 23, 28, 12, 12);
                    if (item == key)
                    {
                        break;
                    }
                    x = x + 80;
                }

            }
        }

        /// <summary>
        /// 垂直步进方法
        /// /// 2015-04-16
        /// 吴海龙
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SetVerticalStep(object sender, KeyValueEventArgs e)
        {
            int TempCenterOfThePieY = CenterOfTheCircleY;
            if ("" == KeyValueEventArgs.Key)
            {
                return;
            }
            using (Graphics g = this.CreateGraphics())
            {
                g.SmoothingMode = SmoothingMode.HighQuality;  //使绘图质量最高，即消除锯齿
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                foreach (var item in sdict.Keys)
                {
                    g.DrawString(sdict[item], f1, Brushes.Green, ItemStartX, TempCenterOfThePieY - ItemFixY);
                    g.DrawEllipse(Pens.Green, CenterOfTheCircleX - CircleRadius, TempCenterOfThePieY - CircleRadius, CircleRadius * 2, CircleRadius * 2);
                    g.DrawLine(p2, new Point(LineStartX, LineStartY), new Point(LineStartX, TempCenterOfThePieY));
                    g.FillEllipse(Brushes.Green, CenterOfTheCircleX - PieRadius, TempCenterOfThePieY - PieRadius, PieRadius * 2, PieRadius * 2);
                    if (item == KeyValueEventArgs.Key)
                    {
                        break;
                    }
                    TempCenterOfThePieY = TempCenterOfThePieY + CircleSpace;
                }
            }
            //----------------------------------
            //if ("" == KeyValueEventArgs.Key)
            //{
            //    return;
            //}
            //int x = 20;
            //int y = 20;
            //using (Graphics g = this.CreateGraphics())
            //{
            //    g.SmoothingMode = SmoothingMode.HighQuality;  //使绘图质量最高，即消除锯齿
            //    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //    g.CompositingQuality = CompositingQuality.HighQuality;
            //    foreach (var item in sdict.Keys)
            //    {
            //        g.DrawString(sdict[item], f1, Brushes.Green, new PointF(45, y + 20));
            //        g.DrawEllipse(Pens.Green, 25, y + 20, 18, 18);
            //        g.DrawLine(p2, new Point(34, 20), new Point(34, y + 25));
            //        g.FillEllipse(Brushes.Green, 28, y + 23, 12, 12);
            //        if (item == KeyValueEventArgs.Key)
            //        {
            //            break;
            //        }
            //        y = y + 80;
            //    }
            //}
        }


        /// <summary>
        /// 水平步进方法
        /// /// 2015-04-16
        /// 吴海龙
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SetHorizontalStep(object sender, KeyValueEventArgs e)
        {
            if ("" == KeyValueEventArgs.Key)
            {
                return;
            }

            int TempCenterOfTheCircleX = CenterOfTheCircleX;
            using (Graphics g = this.CreateGraphics())
            {
                g.SmoothingMode = SmoothingMode.HighQuality;  //使绘图质量最高，即消除锯齿
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                foreach (var item in sdict.Keys)
                {
                    g.DrawLine(p2, new Point(CenterOfTheCircleX, CenterOfTheCircleY), new Point(TempCenterOfTheCircleX, CenterOfTheCircleY));
                    g.DrawString(sdict[item], f1, Brushes.Green, new PointF(TempCenterOfTheCircleX + ItemFixX, CenterOfTheCircleY + ItemFixY));
                    //g.FillEllipse(Brushes.White, TempCenterOfTheCircleX, CenterOfTheCircleY, CircleRadius * 2, CircleRadius * 2);
                    g.DrawEllipse(p3, TempCenterOfTheCircleX - CircleRadius, CenterOfTheCircleY - CircleRadius, CircleRadius * 2, CircleRadius * 2);
                    g.FillEllipse(Brushes.Green, TempCenterOfTheCircleX - PieRadius, CenterOfTheCircleY - PieRadius, PieRadius * 2, PieRadius * 2);
                    if (item == KeyValueEventArgs.Key)
                    {
                        break;
                    }
                    TempCenterOfTheCircleX = TempCenterOfTheCircleX + CircleSpace;
                }
            }
        }

        /// <summary>
        /// 初始化事件
        /// 2015-04-16
        /// 吴海龙
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InitTimeAxis(object sender, KeyValueEventArgs e)
        {
            InitHorizontalTimeAxis();
        }

        private void uctlTimeAxis_Paint(object sender, PaintEventArgs e)
        {
            if (TimeAxisModle == HorizontalTimeAxis)
            {
                InitHorizontalTimeAxis();
                //MessageBox.Show(TimeAxisModle.ToString() + "横向");
            }
            else if (TimeAxisModle == VerticalTimeAxis)
            {
                InitVerticalTimeAxis();
                //MessageBox.Show(TimeAxisModle.ToString() + "垂直");
            }

        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    InitTimeAxis();
        //    SetStep();
        //}

        /// <summary>
        /// 相当于杂志的角色
        /// </summary>
        public class KeyValueEventArgs : EventArgs
        {
            public static string Key = "";
            public KeyValueEventArgs(string s)
            {
                Key = s;
            }
        }


    }
}
