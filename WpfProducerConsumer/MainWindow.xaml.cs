using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WpfProducerConsumer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        class Item
        {
            public string name = "一个产品";
        }
        //生产速度
        static int pro;
        static int con;
        // 产品队列缓存
        static Queue<Item> queue = new Queue<Item>();
        static readonly int BUFFER_SIZE = 3;

        // 同步标记       
        static Semaphore fillCount = new Semaphore(0, BUFFER_SIZE);
        static Semaphore emptyCount = new Semaphore(BUFFER_SIZE, BUFFER_SIZE);
        static Mutex bufferMutex = new Mutex();

        //生产
        void producer()
        {
            int productNumber = 0;
            while (true)
            {
                var item = ProduceItem(productNumber++);

                // 还有生产权限时，进入下面的代码
                emptyCount.WaitOne();
                bufferMutex.WaitOne();


                // 将产品放入buffer中
                putItemIntoBuffer(item);
                bufferMutex.ReleaseMutex();

                // 释放一个拿去权限
                fillCount.Release();
            }
        }
        Item ProduceItem(int number)
        {
            Thread.Sleep(TimeSpan.FromSeconds(pro));//生产速度
            Item item = new Item() { name = "产品" + number };
            if (this != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    this.TextBox3.Text += ("生产了" + item.name + "\n");
                    this.TextBox3.ScrollToEnd();
                }));
                return item;
            }
            return null;
        }
        void putItemIntoBuffer(Item item)
        {
            queue.Enqueue(item);
            if (this != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    this.TextBox3.Text += ("将" + item.name + "放入队列，现在有" + queue.Count + "个" + "\n");
                    this.TextBox3.ScrollToEnd();
                }));
            }
        }
        
        //消费
        void consumer()
        {
            while (true)
            {
                // 等待一个拿去权限
                fillCount.WaitOne();
                bufferMutex.WaitOne();

                // 移除一个物品
                var item = removeItemFromBuffer();
                bufferMutex.ReleaseMutex();

                // 释放一个生产权限
                emptyCount.Release();
                ConsumItem(item);
            }
        }
        Item removeItemFromBuffer()
        {
            var item = queue.Peek();
            queue.Dequeue();
            if (this != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    this.TextBox3.Text += ("将" + item.name + "取出队列，现在有" + queue.Count + "个" + "\n");
                    this.TextBox3.ScrollToEnd();
                }));
                return item;
            }
            return null;
        }
        void ConsumItem(Item item)
        {
            Thread.Sleep(TimeSpan.FromSeconds(con));//消费速度
            if (this != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    this.TextBox3.Text += ("消费了" + item.name + "\n");
                    this.TextBox3.ScrollToEnd();
                }));
            }
        }

        static Thread producerThread;
        static Thread consumerThread;

        public MainWindow()
        {
            InitializeComponent();
           
        }
       
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            pro = Convert.ToInt32(TextBox1.Text);
            con = Convert.ToInt32(TextBox2.Text);

            producerThread = new Thread(this.producer);
            consumerThread = new Thread(this.consumer);

            producerThread.Start();
            consumerThread.Start();

        }
        private void TextBox3_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Environment.Exit(0);
        }
    }

}
