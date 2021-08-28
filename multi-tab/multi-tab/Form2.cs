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
    public partial class Form2 : Form
    {
        string SelectedPortName;
        string SelectedBaudrate;
        string SelectedParity;
        string SelectedStopBits;
        string SelectedDataBits;
        Boolean receiveEnable = false;
        int point = 0;
        private List<byte> buffer = new List<byte>(42);
        Boolean stopped = false;

        public Form2()
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



        
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {

           // receiveEnable = false;




            if (receiveEnable == true)
            {
                int n = serialPort1.BytesToRead;
                byte[] buf = new byte[n];
                serialPort1.Read(buf, 0, n);

                //1.缓存数据
                buffer.AddRange(buf);

                //2.完整性判断
                while (buffer.Count > 6)//头（1字节）、长度（4字节）、校验位（1字节）；根据设计不同而不同
                {
                    //2.1 查找数据头
                    if (buffer[0] == '$') //传输数据有帧头，用于判断
                    {
                        if (buffer.Count < 42) //数据区尚未接收完整
                        {
                            break;
                        }

                        //开始校验
                        if (buffer[41] != 41) //校验失败，最后一个字节是校验位
                        {
                            buffer.Clear();
                            //MessageBox.Show("数据包不正确！");
                            continue;
                        }


                        for (int j = 1; j < 41; j = j + 4)
                        {

                            byte[] byteTemp = new byte[4] { buffer[j], buffer[j + 1], buffer[j + 2], buffer[j + 3] };
                            float fTemp = BitConverter.ToSingle(byteTemp, 0);

                            //float fTemp = Convert.ToSingle(byteTemp);


                            if (radioButton7.Checked)
                                chart1.Invoke(new MethodInvoker(delegate
                                {
                                    switch (j)
                                    {
                                        case 1: chart1.Series[0].Points.AddXY((point), fTemp); break;
                                        case 5: chart1.Series[1].Points.AddXY((point), fTemp); break;
                                        case 9: chart1.Series[2].Points.AddXY((point), fTemp); break;
                                        case 13: chart1.Series[3].Points.AddXY((point), fTemp); break;
                                        case 17: chart1.Series[4].Points.AddXY((point), fTemp); break;
                                        case 21: chart1.Series[5].Points.AddXY((point), fTemp); break;
                                        case 25: chart1.Series[6].Points.AddXY((point), fTemp); break;
                                        case 29: chart1.Series[7].Points.AddXY((point), fTemp); break;
                                        case 33: chart1.Series[8].Points.AddXY((point), fTemp); break;
                                        case 37: chart1.Series[8].Points.AddXY((point), fTemp); break;
                                        default: break;

                                    }

                                }));

                        }
                        point++;//

                        ///执行对数据进行处理操作RunReceiveDataCallback(ReceiveBytes);
                        buffer.Clear();
                    }
                    else //帧头不正确时，记得清除
                    {
                        buffer.RemoveAt(0);
                    }
                }
            }
            else {
                int n = serialPort1.BytesToRead;
                byte[] buf = new byte[n];
                serialPort1.Read(buf, 0, n);


            
            
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
                //receiveEnable = true;
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
             Series series0 = new Series("accX");
             Series series1 = new Series("accY");
             Series series2 = new Series("accZ");
             Series series3 = new Series("gyroX");
             Series series4 = new Series("gyroY");
             Series series5 = new Series("gyroZ");
             Series series6 = new Series("ACC");
             Series series7 = new Series("GYRO");
             Series series8 = new Series("ANGLE");

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
             chart1.ChartAreas[0].AxisY.Title = "加速度传感器 ";
             chart1.ChartAreas[0].AxisX.Title = "时间/s";

             chart1.ChartAreas[1].AxisY.Title = "陀螺仪传感器";
             chart1.ChartAreas[1].AxisX.Title = "时间/s";

             chart1.ChartAreas[2].AxisY.Title = "数据融合";
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
             receiveEnable = false;
             if (stopped == false)
             {

                 MessageBox.Show("请先停止");

             }
             else {

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

         private void Form2_FormClosed(object sender, FormClosedEventArgs e)
         {
             receiveEnable = false;
         }






         private void Form2_Load(object sender, EventArgs e)
         {

         }

         private void label5_Click(object sender, EventArgs e)
         {

         }

         private void label6_Click(object sender, EventArgs e)
         {

         }

         private void button3_Click_1(object sender, EventArgs e)
         {
             receiveEnable = true;
         }

         private void button4_Click(object sender, EventArgs e)
         {
             stopped = true;
             receiveEnable = false;
         }




    }
}
