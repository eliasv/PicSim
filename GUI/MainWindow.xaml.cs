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
using System.IO;
using PicLib;


namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected PIC pic;
        protected System.Timers.Timer CLK = new System.Timers.Timer();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".hex";
            ofd.Filter = "HEX Files (*.hex)|*.hex|All Files (*.*)|*.*";
            Nullable<bool> selected = ofd.ShowDialog();

            if(selected==true)
            {
                lstISA.Items.Clear();
                lstHex.Items.Clear();
                string fname = ofd.FileName;
                StreamReader sr = new StreamReader(ofd.OpenFile());
                while(!sr.EndOfStream)
                    lstHex.Items.Add(sr.ReadLine());
                pic = new PIC(fname);
                foreach(var x in pic.decompile())
                {
                    lstISA.Items.Add(x);
                }
                mnuRun.IsEnabled = true;
                CLK.Interval = pic.getclkInterval()/2;
                CLK.Elapsed += CLK_Elapsed;
                CLK.AutoReset = true;
                
            }
        }

        private void CLK_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            int current = pic.PC;
            //lstISA.SelectedItem = lstISA.FindName(pic.getCurrent().ToString());
            var result = from o in lstISA.Items.OfType<picWord>()
                         where o.getAddress() == current
                         select o;
            //lstISA.SelectedItem=lstISA.Items.GetItemAt(current);
            
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_Run_Click(object sender, RoutedEventArgs e)
        {
            if ((String)mnuRun.Header == "_Run")
            {
                pic.start();
                CLK.Start();
                mnuRun.Header = "_Stop";
            }
            else
            {
                CLK.Stop();
                pic.stop();
                mnuRun.Header = "_Run";
            }

        }
    }
}
