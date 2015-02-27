using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ToolFunction
{
    public partial class uctlMessageBox : UserControl
    {
        public uctlMessageBox( string mess)
        {
            InitializeComponent();
            disappeartime.Start();//ʱ��ռ俪ʼ����
            lab_mess.Text = mess;
        }

        private void disappeartime_Tick(object sender, EventArgs e)
        {
                this.FindForm().Opacity = this.FindForm().Opacity - 0.1;//�ı䴰��͸����
                if (this.FindForm().Opacity == 0)//������͸����Ϊ0ʱ(������������)
                {
                    this.FindForm().Close();//�رմ���
                }
            
        }
        /// <summary>
        /// ֻ��ѡ��Yes�Ž�����Ϣ��ʾ
        /// </summary>
        /// <param name="dr">��ѡ����</param>
        /// <param name="message">��Ϣ����</param>
        public static void Show(DialogResult dr,string message)
        {
            if (dr ==DialogResult.Yes)
            {
                uctlMessageBox umb = new uctlMessageBox(message);
                CommonFunction.ShowForm( umb);
            }
           
        }
        /// <summary>
        /// ��һ�����������أ���ѡ������г����жϣ�ֻ�ṩ��Ϣ�������á�
        /// </summary>
        /// <param name="message"></param>
        public static void Show(string message)
        {
            uctlMessageBox umb = new uctlMessageBox(message);
            CommonFunction.ShowForm( umb);
           
        }
    }
}
