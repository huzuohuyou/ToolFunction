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
    
    public partial class uctlTimeAxis : UserControl
    {
      

        /// <summary>
        /// 蓝色宽2
        /// </summary>
        static Pen p1 = new Pen(Color.Blue, 2);
        /// <summary>
        /// 绿色宽2
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

        //public static Graphics g = null;
        //public static string Key = "";
        public static SortedDictionary<string, string> sdict = new SortedDictionary<string, string>();
        public uctlTimeAxis()
        {
            InitializeComponent();
        }

        public void SetKeyValue(object sender, KeyValueEventArgs e)
        {

        }

        public uctlTimeAxis(SortedDictionary<string, string> s)
        {
            InitializeComponent();
            if (sdict != null)
            {
                this.Width = (sdict.Keys.Count + 2) * 80;
                sdict = s;
            }
        }


        /// <summary>
        /// 初始化时间轴
        /// 2015-04-15
        /// 吴海龙
        /// </summary>
        public void InitTimeAxis()
        {
            try
            {
                int x = 20;
                using (Graphics g = this.CreateGraphics())
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
        /// 执行step
        /// 2015-04-16
        /// 吴海龙
        /// </summary>
        /// <param name="key"></param>
        public void SetStep()
        {
            if ("" == KeyValueEventArgs.Key)
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
                    if (item == KeyValueEventArgs.Key)
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
        public void SetStep(string key)
        {
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
        /// 步进方法
        /// /// 2015-04-16
        /// 吴海龙
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SetStep(object sender, KeyValueEventArgs e)
        {
            SetStep();
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
            InitTimeAxis();
        }

        private void uctlTimeAxis_Paint(object sender, PaintEventArgs e)
        {
            InitTimeAxis();
            SetStep();
            
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
