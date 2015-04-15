using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

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
        /// <summary>
        /// 横坐标
        /// </summary>

        public static SortedDictionary<string, string> sdict = new SortedDictionary<string, string>();
        public uctlTimeAxis()
        {
            InitializeComponent();
            sdict.Add("1", "读取配置");
            sdict.Add("2", "选择模板");
            sdict.Add("3", "确认数据");
            sdict.Add("4", "生成代码");
            if (sdict != null)
            {
                this.Width = (sdict.Keys.Count + 2) * 80;
                InitTimeAxis();
            }
        }

        public uctlTimeAxis(SortedDictionary<string, string> s)
        {
            InitializeComponent();
            sdict.Add("1", "读取配置");
            sdict.Add("2", "选择模板");
            sdict.Add("3", "确认数据");
            sdict.Add("4", "生成代码");
            if (sdict != null)
            {
                this.Width = (sdict.Keys.Count + 2) * 80;
                sdict = s;
                InitTimeAxis();
                this.Refresh();
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
                        //Label l = new Label();
                        //this.Controls.Add(l);
                        //l.Location = new Point(x, 50);
                        //l.Text = sdict[item];
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
        /// <param name="gg"></param>
        public static void InitTimeAxis(Graphics gg)
        {
            try
            {
                int x = 20;
                using (Graphics g = gg)
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
                        //Label l = new Label();
                        //this.Controls.Add(l);
                        //l.Location = new Point(x, 50);
                        //l.Text = sdict[item];
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

        private void button1_Click(object sender, EventArgs e)
        {
            InitTimeAxis();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.FindForm().Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        
        private void button1_Click_1(object sender, EventArgs e)
        {
            InitTimeAxis();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.FindForm().Close();
        }
    }
}
