using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfEvent
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void updateTxtBox(string value);  //声明委托
        public static updateTxtBox output2;
        private void updateTxt2(string str)
        {
            eventTxtBox.Text += str;
            eventTxtBox.ScrollToEnd();
        }
        public MainWindow()
        {
            InitializeComponent();
            output2 = (updateTxt2);
        }

        private void fireBtn_Click(object sender, RoutedEventArgs e)
        {
            FireAlarm fireAlarm = new FireAlarm();
            FireHandlerClass fireHandler = new FireHandlerClass(fireAlarm);
            FireWatcherClass fireWatcher = new FireWatcherClass(fireAlarm);
            fireAlarm.ActivateFireAlarm(roomTxtBox.Text, int.Parse(ferocityTxtBox.Text));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Environment.Exit(0);
        }



        public class FireEventArgs : EventArgs
        {
            public string room;
            public int ferocity;
            public FireEventArgs(string room, int ferocity)
            {
                this.room = room;
                this.ferocity = ferocity;
            }
        }
        public class FireAlarm
        {
            public delegate void FireEventHandler(object sender, FireEventArgs fe);
            public event FireEventHandler fireEvent;

            public void ActivateFireAlarm(string room, int ferocity)
            {
                FireEventArgs fireEventArgs = new FireEventArgs(room, ferocity);
                fireEvent(this, fireEventArgs);
            }
        }

        public class FireHandlerClass 
        {
            public FireHandlerClass(FireAlarm fireAlarm)
            {
                fireAlarm.fireEvent += new FireAlarm.FireEventHandler(ExtinguishFire);
            }

            void ExtinguishFire(object sender, FireEventArgs fe)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    output2(sender.ToString() + "对象调用，灭火事件 ExtinguishFire函数\n");

                }));

                if (fe.ferocity < 2)
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        output2("火情发生在" + fe.room + "，浇水后被扑灭了\n");
                    }));
                else if (fe.ferocity < 5)
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        output2("火情发生在" + fe.room + "，使用灭火器后被扑灭了\n");
                    }));
                else
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        output2("火情发生在" + fe.room + "，灭不了打119吧\n");
                    }));
            }
        }

        public class FireWatcherClass
        {
            public FireWatcherClass(FireAlarm fireAlarm)
            {
                fireAlarm.fireEvent += new FireAlarm.FireEventHandler(WatchFire);
            }

            void WatchFire(object sender, FireEventArgs fe)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    output2(sender.ToString() + "对象调用，群众发现火情事件 WatchFire函数\n");
                }));

                if (fe.ferocity < 2)
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        output2("群众发现火情发生在" + fe.room + "，浇水后被扑灭了\n");
                    }));
                else if (fe.ferocity < 5)
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        output2("群众发现火情发生在" + fe.room + "，群众帮助下使用灭火器后被扑灭了\n");
                    }));
                else
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        output2("群众发现火情发生在" + fe.room + "，群众无法控制，拨打119\n");
                    }));
            }
        }


    }
   
}
