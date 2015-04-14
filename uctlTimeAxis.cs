using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ToolFunction
{
    public partial class uctlTimeAxis : UserControl
    {
        /// <summary>
        /// 蓝色
        /// </summary>
        Pen p1 = new Pen(Color.Blue, 2);
        /// <summary>
        /// 宋体加黑
        /// </summary>
        Font f1 = new Font("微软雅黑", 15, FontStyle.Bold);

        public uctlTimeAxis()
        {
            InitializeComponent();
        }

        public void InitControl(SortedDictionary<string, string> sdict)
        {
            using (Graphics g = this.CreateGraphics())
            {
                Point topLeft = new Point(0, 0);
                Size howBig = new Size(sdict.Keys.Count * 80, 50);
                Rectangle rectangleArea = new Rectangle(topLeft, howBig);
                g.DrawRectangle(p1, rectangleArea);
                g.FillRectangle(Brushes.Pink, rectangleArea);
                int x = 10;
                foreach (var item in sdict.Keys)
                {
                    g.DrawString(item, f1, Brushes.Black, new PointF(x, 10));
                    g.DrawString(sdict[item], f1, Brushes.Black, new PointF(x, 80));
                    x = x + 80;

                }

            }
        }

        public void SetStep(string key, SortedDictionary<string, string> sdict)
        {
            using (Graphics g = this.CreateGraphics())
            {
                int x = 10;
                foreach (var item in sdict.Keys)
                {
                   
                    g.DrawString(item, f1, Brushes.Green, new PointF(x, 10));
                    g.DrawString(sdict[item], f1, Brushes.Green, new PointF(x, 80));
                    x = x + 80;
                    if (item == key)
                    {
                        break;
                    }

                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SortedDictionary<string, string> sdict = new SortedDictionary<string, string>();
            sdict.Add("时间1", "明细1");
            sdict.Add("时间2", "明细2");
            sdict.Add("时间3", "明细3");
            sdict.Add("时间4", "明细4");
            sdict.Add("时间5", "明细5");
            sdict.Add("时间6", "明细6");
            sdict.Add("时间7", "明细7");
            InitControl(sdict);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.FindForm().Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SortedDictionary<string, string> sdict = new SortedDictionary<string, string>();
            sdict.Add("时间1", "明细1");
            sdict.Add("时间2", "明细2");
            sdict.Add("时间3", "明细3");
            sdict.Add("时间4", "明细4");
            sdict.Add("时间5", "明细5");
            sdict.Add("时间6", "明细6");
            sdict.Add("时间7", "明细7");
            SetStep(textBox1.Text, sdict);
        }
    }
}
