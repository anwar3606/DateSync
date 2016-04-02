using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using System.Management;
using System.IO;
using System.Threading;
using System.Globalization;

namespace DateSync
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		string selectedPortName = "";
		SerialPort selectedSerialPort;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			using (var searcher = new ManagementObjectSearcher("SELECT * FROM WIN32_SerialPort"))
			{
				string[] portnames = SerialPort.GetPortNames();
				var ports = searcher.Get().Cast<ManagementBaseObject>().ToList();
				var tList = (from n in portnames join p in ports on n equals p["DeviceID"].ToString() select n + " - " + p["Caption"]).ToList();

				int i = 0;
				serialPortLists.Items.Clear();
				foreach (string s in tList)
				{
					ComboboxItem item = new ComboboxItem();
					item.Text = s;
					item.Value = portnames[i];
					i++;

					serialPortLists.Items.Add(item);
				}
			}
		}

		private void serialPortLists_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
		}

		private void syncTimeBtn_Click(object sender, RoutedEventArgs e)
		{
			if (serialPortLists.SelectedItem != null)
			{
				selectedPortName = ((ComboboxItem)(serialPortLists.SelectedItem)).Value.ToString();
			}
			else return;



			DateTime now = DateTime.Now;
			timeLabel.Content = now.ToString();



			selectedSerialPort = new SerialPort();
			selectedSerialPort.PortName = selectedPortName;
			selectedSerialPort.BaudRate = 9600;
			selectedSerialPort.Open();

			workingLabel.Visibility = Visibility.Visible;
			syncComplete_label.Visibility = Visibility.Collapsed;

			new Thread(() =>
			{

				selectedSerialPort.Write("second:" + now.Second + ";");
				Thread.Sleep(500);
				selectedSerialPort.Write("minute:" + now.Minute + ";");
				Thread.Sleep(500);
				selectedSerialPort.Write("hour:" + now.ToString("hh") + ";");
				Thread.Sleep(500);
				selectedSerialPort.Write("date:" + now.Day + ";");
				Thread.Sleep(500);
				selectedSerialPort.Write("month:" + now.Month + ";");
				Thread.Sleep(500);
				selectedSerialPort.Write("year:" + now.Year + ";");
				Thread.Sleep(500);
				selectedSerialPort.Write("day:" + (int)now.DayOfWeek + ";");
				Thread.Sleep(500);
				selectedSerialPort.Write("meridiem:" + now.ToString("tt", CultureInfo.InvariantCulture) + ";");
				Thread.Sleep(500);
				selectedSerialPort.Close();

				Application.Current.Dispatcher.Invoke(new Action(() => {

					workingLabel.Visibility = Visibility.Collapsed;
					syncComplete_label.Visibility = Visibility.Visible;
					/* Your code here */
				}));


			}).Start();







		}
	}


	public class ComboboxItem
	{
		public string Text { get; set; }
		public object Value { get; set; }
		public override string ToString()
		{
			return Text;
		}

		public static explicit operator ComboboxItem(int v)
		{
			throw new NotImplementedException();
		}
	}


}
