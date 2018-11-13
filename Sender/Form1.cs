using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CopyDataStruct;

namespace Sender
{
    public partial class Form1 : Form
    {
        //Win32 API函数
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(int hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);

        const int WM_COPYDATA = 0x004A;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int hWnd = FindWindow(null, @"Form1");
            if (hWnd == 0)
            {
                MessageBox.Show("未找到消息接受者！");
            }
            else
            {
                byte[] sarr = System.Text.Encoding.Default.GetBytes(txtString.Text);
                int len = sarr.Length;
                COPYDATASTRUCT cds;
                cds.dwData = (IntPtr)Convert.ToInt16(txtInt.Text);//可以是任意值
                cds.cbData = len + 1;//指定lpData内存区域的字节数
                cds.lpData = txtString.Text;//发送给目标窗口所在进程的数据
                SendMessage(hWnd, WM_COPYDATA, 0, ref cds);
            }
        }
    }
}
