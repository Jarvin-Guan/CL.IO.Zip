using CL.IO.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false; 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fromDic = Path.GetDirectoryName(typeof(Form1).Assembly.Location) + "\\GitHub.app";

            string toZip = Path.GetDirectoryName(typeof(Form1).Assembly.Location) + "\\a.zip";
            if (File.Exists(toZip))
            {
                File.Delete(toZip);
            }
            double percent = 0;
            ZipHandler handler = ZipHandler.GetInstance();
            TaskFactory fastory = new TaskFactory();
            fastory.StartNew(() =>
            {
                handler.PackDirectory(fromDic, toZip, (num) =>
                {
                    textBox1.Text = num.ToString() + "%";
                    progressBar1.Value = Convert.ToInt32(num);
                });
            });
            File.Delete(toZip);
        }
    }
}
