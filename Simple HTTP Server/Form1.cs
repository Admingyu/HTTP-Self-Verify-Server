using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Collections;


namespace Simple_HTTP_Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
           

        }

        //判断HOSTS是否包含str
        private static bool checkHosts(string filename,string str)
        {
            StreamReader sr = new StreamReader(filename);
            string allText = sr.ReadToEnd();
            sr.Close();
           return allText.Contains(str);
            
        }


        /// <summary>
        /// 修改hosts文件
        /// </summary>
        private static void updateHosts(string ip, string host)
        {
            string path = @"C:\WINDOWS\system32\drivers\etc\hosts";
            //通常情况下这个文件是只读的，所以写入之前要取消只读
            File.SetAttributes(path, File.GetAttributes(path) & (~FileAttributes.ReadOnly));//取消只读
            //1.创建文件流
            FileStream fs = new FileStream(path, FileMode.Append);
            //2.创建写入器
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            //3.开始写入
            bool result = false;//标识是否写入成功
            try
            {
                StringBuilder sb = new StringBuilder();
                //追加模式写入
                sb.Append(ip);//IP地址
                sb.Append("   ");
                sb.Append(host);//网址
                sw.WriteLine(sb.ToString());
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                //4.关闭写入器
                if (sw != null)
                {
                    sw.Close();
                }
                //5.关闭文件流
                if (fs != null)
                {
                    fs.Close();
                }
            }
            if (result == true)
            {
                //MessageBox.Show("写入成功！");
                File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.ReadOnly);//设置只读
            }
            else
            {
                //MessageBox.Show("写入失败！");
                return;
            }
        }



        /// <summary>
        /// //http服务器部分
        /// </summary>
        //定义是否执行完技术标志
        bool isActive = true;

        public void Listen(string address)
        {
            

            if (!HttpListener.IsSupported)
                throw new InvalidOperationException(
                    "请确保使用WindowsXP以上版本的Windows!");

            //初始化Http监听器
            HttpListener listener = new HttpListener();

            //初始化服务器URL
            //string[] prefixes = new string[] { address };
            // foreach (string prefix in prefixes)
            //{
            //   listener.Prefixes.Add(prefix);
            // }
            listener.Prefixes.Add(address);



            //创建新[线程开始代码]循环监听
           ;
            ThreadStart Ths = new ThreadStart(delegate
            {


                do//循环开关服务器接受请求
                {
                    //开启服务器
                    listener.Start();
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    if (request.HttpMethod == "GET")
                    {
                        textBox1.AppendText("\r\n接收到GET请求");
                        OnGetRequest(request, response);

                    }
                    else
                    {
                        textBox1.AppendText("\r\n接收到POST请求");
                        OnPostRequest(request, response);

                    }
                    //关闭服务
                    listener.Stop();
                    
                }
                while (isActive);//isActive时不退出循环继续接受 
            });

            Thread Th = new Thread(Ths);//创建新线程
            Th.Start();//开启线程

        }

        private void OnPostRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            throw new NotImplementedException();
        }

        private void OnGetRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            //定义返回字符串
            string responseString = "Your product is activated";

            //定义响应输出流
            Stream output = response.OutputStream;

            //将字符串编码成字节
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);

            //获取字节长度
            response.ContentLength64 = buffer.Length;

            //将字节和字节长度写入响应输出流
            output.Write(buffer, 0, buffer.Length);

            //关闭
            output.Close();

            textBox1.AppendText("\r\n成功");

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //如果hosts里面不存在相应hosts则添加
            if (!checkHosts(@"C:\WINDOWS\system32\drivers\etc\hosts", "www.example.com")) {
                textBox1.AppendText("\r\n正在添加HOSTS...");
                updateHosts("127.0.0.1", "www.pubmedchina.com");
            }
            else
            {
                textBox1.AppendText("\r\nHOSTS已存在...");
            }
            
            if (button1.Text == "开启监听")//开启监听
            {
                textBox1.AppendText("\r\n监听服务开启,等待验证请求...");

                if (checkBox1.Checked==true)  {
                    
                    isActive = true;                   
                }
                else
                {
                    isActive = false;               
                }

                Listen("http://127.0.0.1:80/");

                button1.Text = "关闭监听并退出";//切换按钮文字
            }

            else
            {
                textBox1.AppendText("\r\n关闭线程并退出...");
                isActive = false;
                Application.ExitThread();
            }

            

            

            
        }

        

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            int code = random.Next(147100000, 147189999);
            Clipboard.SetText(code.ToString());
            textBox1.AppendText("\r\n验证码复制成功:" + code);
        }
    }
}
