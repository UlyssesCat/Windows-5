using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinServer
{
    public partial class server : Form
    {
        public delegate void UpdateTxt(string msg);
        public UpdateTxt updateTxt;
        public void UpdateTxtMethod(string msg)
        {
            if (msg == null)
                return;
            else
                listBox1.Items.Add(msg);
        }


        public server()
        {
            updateTxt = new UpdateTxt(UpdateTxtMethod);
            InitializeComponent();
            Thread thread = new Thread((ThreadStart)delegate
            {
                pipeServer.BeginWaitForConnection(WaitForConnectionCallback, pipeServer);
            });
            thread.Start();
        }
        NamedPipeServerStream pipeServer =
            new NamedPipeServerStream("testpipe", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
        private void server_Load(object sender, EventArgs e)
        {
        }
        private void WaitForConnectionCallback(IAsyncResult ar)
        {
            var pipeServer = (NamedPipeServerStream)ar.AsyncState;
            pipeServer.EndWaitForConnection(ar);
            StreamReader sr = new StreamReader(pipeServer);
            while (true)
            {
                //if (!pipeServer.IsConnected) break;
                if (pipeServer.IsConnected)
                {
                    String item;
                    if (sr.ReadLine() == null) { item = ""; }
                    else { item = sr.ReadLine(); }
                    this.BeginInvoke(updateTxt,item);
                }
                else
                    break;
            }
        }

    }
}
