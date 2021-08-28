using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

namespace multi_tab
{
    public partial class Form3 : Form
    {
        string SelectedPortName;
        string SelectedBaudrate;
        string SelectedParity;
        string SelectedStopBits;
        string SelectedDataBits;
        Boolean receiveEnable = false;
        int point_1 = 0;
        int point_2 = 0;
        int point_3 = 0;
        int point_4 = 0;
        private List<byte> buffer = new List<byte>(42);
        Boolean stopped = false;

        public Form3()
        {
            
            InitializeComponent();
            SerialInit();//设置默认值
            GetPortNameAndConfig();
            InitChart();
            
        }
        void SerialInit()
        {
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
             //设置默认值
             SelectedPortName = serialPort1.PortName;
             SelectedBaudrate = serialPort1.BaudRate.ToString();
             SelectedParity = serialPort1.Parity.ToString();
             SelectedStopBits = serialPort1.StopBits.ToString();
             SelectedDataBits = serialPort1.DataBits.ToString();
             
             comboBox1.Text = SelectedPortName;
             comboBox2.Text = SelectedBaudrate;
             comboBox3.Text = SelectedDataBits;
             comboBox4.Text = SelectedStopBits;
             comboBox5.Text = SelectedParity;  
        
        }
//



        
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {

            if (receiveEnable == true && checkBox11.Checked)
            {
                int n = serialPort1.BytesToRead;
                byte[] buf = new byte[n];
                serialPort1.Read(buf, 0, n);

                //1.缓存数据
                buffer.AddRange(buf);

                //2.完整性判断
                while (buffer.Count >=7)//头（2字节）、长度（1字节）、数据（2字节）、校验位（2字节）；根据设计不同而不同
                {
                    //2.1 查找数据头
                    switch(buffer[0] ){
                        case 0x01:
                       int len = buffer[2];
            
                       if (buffer.Count < len + 5) //数据区尚未接收完整
                       {
                        break;
                       }
                       //得到完整的数据，复制到ReceiveBytes中进行校验
                       byte[] ReceiveBytes = new byte[len + 5];
                       buffer.CopyTo(0, ReceiveBytes, 0, len + 5);
                    
                        //开始校验---自定义实现

                       if (ReceiveBytes[len + 3] != 0x38 || ReceiveBytes[len + 4] != 0x7a) //校验失败，最后一个字节是校验位
                      {
                        buffer.RemoveRange(0, len + 5);
                        // MessageBox.Show("数据包不正确！");
                        continue;
                       } 


                      //数据处理部分
                       if (radioButton6.Checked)//ascii
                       {
                           string receivedmsg = Convert.ToString(buffer);
                           richTextBox1.Invoke(new MethodInvoker(delegate
                           {
                               //string time = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                               string time = DateTime.Now.ToString("HH:mm:ss:fff");
                               richTextBox1.AppendText(time + " " + receivedmsg + "\n");//添加内容。AppendText函数等同：
                               //设置输出和滚动条一起移动
                               richTextBox1.Select(richTextBox1.TextLength, 0);//设置光标的位置到文本尾
                               richTextBox1.ScrollToCaret();//滚动到控件光标处 
                           }));

                       }
                       else
                       {//hex

                           richTextBox1.Invoke(new MethodInvoker(delegate
                           {
                               string time = DateTime.Now.ToString("HH:mm:ss:fff");
                               richTextBox1.AppendText(time + " ");
                               for (int i = 0; i < len+5; i++)
                               {
                                   string receivedmsg = Convert.ToString(buffer[i], 16).ToUpper();
                                   if (buf[i] < 16)
                                       richTextBox1.AppendText("0" + receivedmsg + " ");//：
                                   else
                                       richTextBox1.AppendText(receivedmsg + " ");//：
                               }
                               richTextBox1.AppendText("\n");
                               richTextBox1.Select(richTextBox1.TextLength, 0);//设置光标的位置到文本尾
                               richTextBox1.ScrollToCaret();//滚动到控件光标处 

                           }));


                       }
                            if (radioButton7.Checked)
                                chart1.Invoke(new MethodInvoker(delegate
                                {

                                    chart1.Series[0].Points.AddXY((point_1++), (buffer[3] << 8 | buffer[4]) / 10);     

                                }));


                        ///执行对数据进行处理操作RunReceiveDataCallback(ReceiveBytes);

                       buffer.RemoveRange(0, len + 5);




                        break;
                        case 0x02:
                             len = buffer[2];
            
                       if (buffer.Count < len + 5) //数据区尚未接收完整
                       {
                        break;
                       }
                       //得到完整的数据，复制到ReceiveBytes中进行校验
                       ReceiveBytes = new byte[len + 5];
                       buffer.CopyTo(0, ReceiveBytes, 0, len + 5);
                    
                        //开始校验---自定义实现

                       if (ReceiveBytes[len + 3] != 0xdb || ReceiveBytes[len + 4] != 0xc8) //校验失败，最后一个字节是校验位
                      {
                        buffer.RemoveRange(0, len + 5);
                         MessageBox.Show("数据包不正确！");
                        continue;
                       } 


                      //数据处理部分
                       if (radioButton6.Checked)//ascii
                       {
                           string receivedmsg = Convert.ToString(buffer);
                           richTextBox1.Invoke(new MethodInvoker(delegate
                           {
                               //string time = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                               string time = DateTime.Now.ToString("HH:mm:ss:fff");
                               richTextBox1.AppendText(time + " " + receivedmsg + "\n");//添加内容。AppendText函数等同：
                               //设置输出和滚动条一起移动
                               richTextBox1.Select(richTextBox1.TextLength, 0);//设置光标的位置到文本尾
                               richTextBox1.ScrollToCaret();//滚动到控件光标处 
                           }));

                       }
                       else
                       {//hex

                           richTextBox1.Invoke(new MethodInvoker(delegate
                           {
                               string time = DateTime.Now.ToString("HH:mm:ss:fff");
                               richTextBox1.AppendText(time + " ");
                               for (int i = 0; i < len+5; i++)
                               {
                                   string receivedmsg = Convert.ToString(buffer[i], 16).ToUpper();
                                   if (buf[i] < 16)
                                       richTextBox1.AppendText("0" + receivedmsg + " ");//：
                                   else
                                       richTextBox1.AppendText(receivedmsg + " ");//：
                               }
                               richTextBox1.AppendText("\n");
                               richTextBox1.Select(richTextBox1.TextLength, 0);//设置光标的位置到文本尾
                               richTextBox1.ScrollToCaret();//滚动到控件光标处 

                           }));


                       }
                            if (radioButton7.Checked)
                                chart1.Invoke(new MethodInvoker(delegate
                                {

                                    chart1.Series[3].Points.AddXY((point_2++), ((buffer[5] << 8) | buffer[6]));
                                   // MessageBox.Show(Convert.ToString(buffer[5] << 8 & buffer[6],10));

                                }));

                        
                        ///执行对数据进行处理操作RunReceiveDataCallback(ReceiveBytes);

                       buffer.RemoveRange(0, len + 5);
                        break;
                        case 0x03:
                              len = buffer[2];
            
                       if (buffer.Count < len + 5) //数据区尚未接收完整
                       {
                        break;
                       }
                       //得到完整的数据，复制到ReceiveBytes中进行校验
                       ReceiveBytes = new byte[len + 5];
                       buffer.CopyTo(0, ReceiveBytes, 0, len + 5);
                    
                        //开始校验---自定义实现

                       if (ReceiveBytes[len + 3] != 0x5a || ReceiveBytes[len + 4] != 0x3d) //校验失败，最后一个字节是校验位
                      {
                        buffer.RemoveRange(0, len + 5);
                         MessageBox.Show("数据包不正确！");
                        continue;
                       } 


                      //数据处理部分
                       if (radioButton6.Checked)//ascii
                       {
                           string receivedmsg = Convert.ToString(buffer);
                           richTextBox1.Invoke(new MethodInvoker(delegate
                           {
                               //string time = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                               string time = DateTime.Now.ToString("HH:mm:ss:fff");
                               richTextBox1.AppendText(time + " " + receivedmsg + "\n");//添加内容。AppendText函数等同：
                               //设置输出和滚动条一起移动
                               richTextBox1.Select(richTextBox1.TextLength, 0);//设置光标的位置到文本尾
                               richTextBox1.ScrollToCaret();//滚动到控件光标处 
                           }));

                       }
                       else
                       {//hex

                           richTextBox1.Invoke(new MethodInvoker(delegate
                           {
                               string time = DateTime.Now.ToString("HH:mm:ss:fff");
                               richTextBox1.AppendText(time + " ");
                               for (int i = 0; i < len+5; i++)
                               {
                                   string receivedmsg = Convert.ToString(buffer[i], 16).ToUpper();
                                   if (buf[i] < 16)
                                       richTextBox1.AppendText("0" + receivedmsg + " ");//：
                                   else
                                       richTextBox1.AppendText(receivedmsg + " ");//：
                               }
                               richTextBox1.AppendText("\n");
                               richTextBox1.Select(richTextBox1.TextLength, 0);//设置光标的位置到文本尾
                               richTextBox1.ScrollToCaret();//滚动到控件光标处 

                           }));


                       }
                            if (radioButton7.Checked)
                                chart1.Invoke(new MethodInvoker(delegate
                                {

                                    chart1.Series[6].Points.AddXY((point_3++), (buffer[3] << 8 | buffer[4]));
                                    chart1.Series[7].Points.AddXY((point_4++), (buffer[5] << 8 | buffer[6])); 

                                }));
                                             

                        ///执行对数据进行处理操作RunReceiveDataCallback(ReceiveBytes);

                       buffer.RemoveRange(0, len + 5);
                        break;
                        break;
                        default:
                        buffer.RemoveAt(0);
                        break;
                    
                    }
                    
                    
                        
                }
            }
            else if(receiveEnable == true) {
                int n = serialPort1.BytesToRead;
                byte[] buf = new byte[n];
                serialPort1.Read(buf, 0, n);

                if (radioButton6.Checked)//ascii
                {
                    string receivedmsg = Convert.ToString(buf);
                    richTextBox1.Invoke(new MethodInvoker(delegate
                    {
                        //string time = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        string time = DateTime.Now.ToString("HH:mm:ss:fff");
                        richTextBox1.AppendText(time + " "+ receivedmsg + "\n");//添加内容。AppendText函数等同：
                        //设置输出和滚动条一起移动
                        richTextBox1.Select(richTextBox1.TextLength, 0);//设置光标的位置到文本尾
                        richTextBox1.ScrollToCaret();//滚动到控件光标处 
                    }));

                }
                else
                {//hex

                    richTextBox1.Invoke(new MethodInvoker(delegate
                    {
                        string time = DateTime.Now.ToString("HH:mm:ss:fff");
                        richTextBox1.AppendText(time + " ");
                        for (int i = 0; i < n; i++)
                        {
                            string receivedmsg = Convert.ToString(buf[i], 16).ToUpper();
                            if(buf[i]<16)
                            richTextBox1.AppendText("0"+receivedmsg + " ");//：
                            else
                                richTextBox1.AppendText( receivedmsg + " ");//：
                        }
                        richTextBox1.AppendText("\n");
                        richTextBox1.Select(richTextBox1.TextLength, 0);//设置光标的位置到文本尾
                        richTextBox1.ScrollToCaret();//滚动到控件光标处 

                    }));


                }
            
            
            
            
            
            
            
            
            
            
            
            }
        }

           


        private void FindComButton_Click(object sender, EventArgs e)
        {
            GetPortNameAndConfig();
        }
        private void button2_Click(object sender, EventArgs e)
        {
           
            if (SelectedPortName != "")
            {
                OpenPortName(SelectedPortName);
                receiveEnable = true;
            }

        }
        public void OpenPortName(string PortName)
        {
            
            serialPort1.PortName = PortName;
            serialPort1.BaudRate = Convert.ToInt32(SelectedBaudrate);
            serialPort1.DataBits = Convert.ToByte(SelectedDataBits);
            serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), SelectedParity, true);
            serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), SelectedStopBits, true);

            if (serialPort1.IsOpen)
            {
                MessageBox.Show(PortName + "已被其他程序打开");
            }
            else 
            {
                try
                {
                    serialPort1.Open();
                }
                catch (Exception)
                {
                    MessageBox.Show(PortName + "已被其他程序打开");
                }
            }

        }

        public void GetPortNameAndConfig()
        {
            foreach (string s in SerialPort.GetPortNames())
            {
                if (s != "" || (s.ToLower()).StartsWith("com"))
                {
                    comboBox1.Items.Add(s);
                }
            }

            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                comboBox5.Items.Add(s);
            }
            foreach (string s in Enum.GetNames(typeof(StopBits)))
            {
                comboBox4.Items.Add(s);
            }

       
        
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedPortName = comboBox1.SelectedItem.ToString();
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedBaudrate = comboBox2.SelectedItem.ToString();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedStopBits = comboBox4.SelectedItem.ToString();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedDataBits = comboBox3.SelectedItem.ToString();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedParity = comboBox5.SelectedItem.ToString();
        }
       

        private void button3_Click(object sender, EventArgs e)
        {
           // Thread readThread = new Thread(read);
           // readThread.Start();

            receiveEnable = true;
        }
        
        public void write()
        {
             try
                {
                    
                    foreach (string line in richTextBox2.Lines)
                    {
                        if (radioButton4.Checked)//ascii
                        {
                            serialPort1.WriteLine(line);                        
                        }                            
                        else
                        {//hex
                            string[] substr =line.Split(' ');
                            byte[] bytes = new byte[substr.Length];
                            for (int j = 0; j < substr.Length; j++)
                            {

                                bytes[j] = Convert.ToByte(substr[j], 16);//16进制相互转换
                               // MessageBox.Show(Convert.ToString(bytes[j], 16));

                            }
                            //MessageBox.Show(line);
                            Thread.Sleep(100);
                             
                            serialPort1.Write(bytes, 0, bytes.Length);
                        }                          
                    }               
                }
                catch (Exception) {
                   // MessageBox.Show("发送异常");
                
                }
            
        }
         private void button4_Click(object sender, EventArgs e)
        {

            write();
        }

         private void button5_Click(object sender, EventArgs e)
         {
            // readThread.Abort();
             stopped = true;
             receiveEnable = false;
           
         }

         private void InitChart()
         {
             //定义图表区域
             chart1.ChartAreas.Clear();
             ChartArea chartArea1 = new ChartArea("C1");
             ChartArea chartArea2 = new ChartArea("C2");
             ChartArea chartArea3 = new ChartArea("C3");


             chart1.ChartAreas.Add(chartArea1);
             chart1.ChartAreas.Add(chartArea2);
             chart1.ChartAreas.Add(chartArea3);

             //定义存储和显示点的容器
             chart1.Series.Clear();
             Series series0 = new Series("series1");
             Series series1 = new Series("series2");
             Series series2 = new Series("series3");
             Series series3 = new Series("series4");
             Series series4 = new Series("series5");
             Series series5 = new Series("series6");
             Series series6 = new Series("series7");
             Series series7 = new Series("series8");
             Series series8 = new Series("series9");

             chart1.Series.Add(series0);
             chart1.Series.Add(series1);
             chart1.Series.Add(series2);
             chart1.Series.Add(series3);
             chart1.Series.Add(series4);
             chart1.Series.Add(series5);
             chart1.Series.Add(series6);
             chart1.Series.Add(series7);
             chart1.Series.Add(series8);


             chart1.Series[0].ChartArea = "C1";
             chart1.Series[1].ChartArea = "C1";
             chart1.Series[2].ChartArea = "C1";


             //series代表曲线 曲线所在的图
             chart1.Series[3].ChartArea = "C2";
             chart1.Series[4].ChartArea = "C2";
             chart1.Series[5].ChartArea = "C2";

             chart1.Series[6].ChartArea = "C3";
             chart1.Series[7].ChartArea = "C3";
             chart1.Series[8].ChartArea = "C3";
   
             


             //设置可放大缩小
             for (int j = 0; j < 3; j++)
             {
                 chart1.ChartAreas[j].CursorX.IsUserEnabled = true;
                 chart1.ChartAreas[j].CursorX.IsUserSelectionEnabled = true;
                 chart1.ChartAreas[j].AxisX.ScaleView.Zoomable = true;
                 // Enable range selection and zooming end user interface
                 chart1.ChartAreas[j].CursorY.IsUserEnabled = true;
                 chart1.ChartAreas[j].CursorY.IsUserSelectionEnabled = true;
                 chart1.ChartAreas[j].AxisY.ScaleView.Zoomable = true;
                 chart1.ChartAreas[j].AxisX.ScrollBar.Size = 5;
                 chart1.ChartAreas[j].AxisY.ScrollBar.Size = 5;

             }
                 // Enable range selection and zooming end user interface

             //设置图表显示样式 注释掉使用默认样式
             //chart1.ChartAreas[0].AxisY.Minimum = 0;
             //chart1.ChartAreas[0].AxisY.Maximum = 100;
             //chart1.ChartAreas[0].AxisX.Interval = 1;
            // chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Silver;
             //chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;
             chart1.ChartAreas[0].AxisY.Title = "风速传感器 ";
             chart1.ChartAreas[0].AxisX.Title = "时间/s";

             chart1.ChartAreas[1].AxisY.Title = "风向传感器";
             chart1.ChartAreas[1].AxisX.Title = "时间/s";

             chart1.ChartAreas[2].AxisY.Title = "温湿度传感器";
             chart1.ChartAreas[2].AxisX.Title = "时间/s";


             //设置chart的总标题标题
             chart1.Titles.Clear();
             chart1.Titles.Add("S01");
             chart1.Titles[0].Text = "传感器数据显示";
             chart1.Titles[0].ForeColor = Color.RoyalBlue;
             chart1.Titles[0].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);

             //设置图表显示样式
             chart1.Series[0].Color = Color.Red;
             chart1.Series[1].Color = Color.Blue;
             chart1.Series[2].Color = Color.Green;
             chart1.Series[3].Color = Color.Red;
             chart1.Series[4].Color = Color.Blue;
             chart1.Series[5].Color = Color.Green;
             chart1.Series[6].Color = Color.Red;
             chart1.Series[7].Color = Color.Blue;
             chart1.Series[8].Color = Color.Green;

             if (radioButton1.Checked)
             {
                 chart1.Titles[0].Text = string.Format("传感器数据 {0} 显示", radioButton1.Text);
                 for (int i = 0; i <9 ; i++)
                 chart1.Series[i].ChartType = SeriesChartType.Line;//设置折线

             }
             if (radioButton2.Checked)
             {
                 chart1.Titles[0].Text = string.Format("传感器数据 {0} 显示", radioButton2.Text);
                 for (int i = 0; i < 9; i++)
                 chart1.Series[i].ChartType = SeriesChartType.Spline;//设置曲线

             }
             for (int i = 0; i < 9;i++ )
                 chart1.Series[i].Points.Clear();
         }

         private void radioButton1_CheckedChanged(object sender, EventArgs e)
         {
             if (radioButton1.Checked)
             {
                 chart1.Titles[0].Text = string.Format("传感器数据 {0} 显示", radioButton1.Text);
                 for (int i = 0; i < 9; i++)
                     chart1.Series[i].ChartType = SeriesChartType.Line;//设置折线
             }

         }

         private void radioButton2_CheckedChanged(object sender, EventArgs e)
         {
             if (radioButton2.Checked)
             {
                 chart1.Titles[0].Text = string.Format("传感器数据 {0} 显示", radioButton2.Text);
                 for (int i = 0; i < 9; i++)
                     chart1.Series[i].ChartType = SeriesChartType.Spline;//设置曲线
             }
         }





         private void button1_Click(object sender, EventArgs e)
         {
             if (stopped == false) {
                 MessageBox.Show("请先停止");
          
             
             } else {
             
               serialPort1.Close();
            
             }

              
         }




         private void groupBox6_Enter(object sender, EventArgs e)
         {

         }

         private void checkBox1_CheckedChanged(object sender, EventArgs e)
         {
             
             chart1.Series[0].Color = (checkBox1.Checked)? Color.Red : Color.Transparent;
         }


         private void checkBox2_CheckedChanged(object sender, EventArgs e)
         {
             chart1.Series[1].Color = (checkBox2.Checked) ? Color.Blue : Color.Transparent;
         }

         private void checkBox3_CheckedChanged(object sender, EventArgs e)
         {
             chart1.Series[2].Color = (checkBox3.Checked) ? Color.Green : Color.Transparent;
         }

         private void checkBox4_CheckedChanged(object sender, EventArgs e)
         {
             chart1.Series[3].Color = (checkBox4.Checked) ? Color.Red : Color.Transparent;

         }
         private void checkBox5_CheckedChanged(object sender, EventArgs e)
         {
             chart1.Series[4].Color = (checkBox5.Checked) ? Color.Blue : Color.Transparent;

         }

         private void checkBox6_CheckedChanged(object sender, EventArgs e)
         {
             chart1.Series[5].Color = (checkBox6.Checked) ? Color.Green : Color.Transparent;

         }

         private void checkBox7_CheckedChanged(object sender, EventArgs e)
         {
             chart1.Series[6].Color = (checkBox7.Checked) ? Color.Red : Color.Transparent;

         }

         private void checkBox8_CheckedChanged(object sender, EventArgs e)
         {
             chart1.Series[7].Color = (checkBox8.Checked) ? Color.Blue : Color.Transparent;

         }

         private void checkBox9_CheckedChanged(object sender, EventArgs e)
         {
             chart1.Series[8].Color = (checkBox9.Checked) ? Color.Green : Color.Transparent;

         }

         private void checkBox10_CheckedChanged(object sender, EventArgs e)
         {

         }

         private void Form3_FormClosed(object sender, FormClosedEventArgs e)
         {
             receiveEnable = false;
         }

         private void textBox1_TextChanged(object sender, EventArgs e)
         {
             timer1.Interval = Convert.ToInt32(textBox1.Text);
         }

         private void checkBox11_CheckedChanged(object sender, EventArgs e)
         {
             if (checkBox11.Checked)
             {
                 timer1.Interval = Convert.ToInt32(textBox1.Text);
                 timer1.Start();


             }
             else {

                 timer1.Stop();
             
             
             }
         }

         private void timer1_Tick(object sender, EventArgs e)
         {
             write();
         }




    }
}
