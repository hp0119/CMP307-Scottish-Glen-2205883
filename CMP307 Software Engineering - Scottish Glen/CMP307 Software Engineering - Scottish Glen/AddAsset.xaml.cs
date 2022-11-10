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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Data;
using System.CodeDom;
using Google.Protobuf.WellKnownTypes;
using System.Data.SqlTypes;
using MySql.Data.Types;
using Microsoft.EntityFrameworkCore.Storage;
using System.Management;
using System.Net;
using System.Net.Sockets;

namespace CMP307_Software_Engineering___Scottish_Glen
{
    /// <summary>
    /// Interaction logic for AddAsset.xaml
    /// </summary>
    public partial class AddAsset : Window
    {
        public AddAsset()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // call the method to get the hardware information to fill in textboxes
            GetHardwareInfo();
        }

        private void AddButton_Clicked(object sender, RoutedEventArgs e)
        {
            // database connection string
            string connectionString = "server=lochnagar.abertay.ac.uk;user id=sql2205883;password=U28c2jYBnhZN;database=sql2205883";
            // create mysql connection
            MySqlConnection conn = new MySqlConnection(connectionString);
            // open database connection
            conn.Open();
            // create mysql command
            MySqlCommand command = new MySqlCommand("INSERT INTO Asset (SystemName, Model, Manufacturer, Type, IPAddress, PurchaseDate, AdditionalInfo) " +
                "VALUES (@sysName, @model, @manufacturer, @type, @ipAddress, @purchaseDate, @additionalInfo)", conn);
            // validating the textboxes
            if (sysNameTB.Text == "" || sysNameTB.Text == null)
            {
                errorTextBlock.Text = "System Name must be filled in";
            }
            else if (modelTB.Text == "" || modelTB.Text == null)
            {
                errorTextBlock.Text = "Model must be filled in";
            }
            else if (manufacturerTB.Text == "" || manufacturerTB.Text == null)
            {
                errorTextBlock.Text = "Manufacturer must be filled in";
            }
            else if (typeTB.Text == "" || typeTB.Text == null)
            {
                errorTextBlock.Text = "Type must be filled in";
            }
            else if (ipAddressTB.Text == "" || ipAddressTB.Text == null)
            {
                errorTextBlock.Text = "IP Address must be filled in";
            }
            // inserting asset data into database
            else
            {
                command.Parameters.Add("@sysName", MySqlDbType.VarChar).Value = sysNameTB.Text;
                command.Parameters.Add("@model", MySqlDbType.VarChar).Value = modelTB.Text;
                command.Parameters.Add("@manufacturer", MySqlDbType.VarChar).Value = manufacturerTB.Text;
                command.Parameters.Add("@type", MySqlDbType.VarChar).Value = typeTB.Text;
                command.Parameters.Add("@ipAddress", MySqlDbType.VarChar).Value = ipAddressTB.Text;
                //command.Parameters.Add("@purchaseDate", MySqlDbType.Date).Value = purchaseDateTB.Text;
                command.Parameters.Add("@additionalInfo", MySqlDbType.VarChar).Value = additionalInfoTB.Text;
                if (purchaseDateTB.Text == "" || purchaseDateTB.Text == null)
                {
                    command.Parameters.AddWithValue("@purchaseDate", DBNull.Value);
                }
                else
                {
                    command.Parameters.Add("@purchaseDate", MySqlDbType.Date).Value = purchaseDateTB.Text;
                }
                // execute non query
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    errorTextBlock.Text = ex.Message;
                }
                // close database connection
                conn.Close();
                // set input textboxes to null
                sysNameTB.Text = null;
                modelTB.Text = null;
                manufacturerTB.Text = null;
                typeTB.Text = null;
                ipAddressTB.Text = null;
                purchaseDateTB.Text = null;
                additionalInfoTB.Text = null;
                errorTextBlock.Text = null;
            }
        }

        public void GetHardwareInfo()
        {
            // find the system name
            sysNameTB.Text = GetSysName();
            // find the model and manufacturer
            GetModelManf();
            // find the IP address
            ipAddressTB.Text = GetIPAddress().ToString();
        }



        // find the system name
        public string GetSysName()
        {
            string sysName = Environment.MachineName;
            return sysName;
        }

        // find the model and manufacturer
        public void GetModelManf()
        {
            // set up system query to find system information
            System.Management.SelectQuery mQuery = new System.Management.SelectQuery(@"Select * from Win32_ComputerSystem");

            using (System.Management.ManagementObjectSearcher objSearcher = new System.Management.ManagementObjectSearcher(mQuery))
            {
                foreach (System.Management.ManagementObject mObject in objSearcher.Get())
                {
                    // find the model and manufacturer information
                    mObject.Get();
                    try
                    {
                        modelTB.Text = mObject["Model"].ToString();
                        manufacturerTB.Text = mObject["Manufacturer"].ToString();
                    }
                    catch (Exception ex)
                    {
                        errorTextBlock.Text = ex.Message;
                    }
                }
            }
        }

        // find the IP address
        public IPAddress GetIPAddress()
        {
            IPAddress[] hostAddresses = Dns.GetHostAddresses("");

            foreach (IPAddress hostAddress in hostAddresses)
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    return hostAddress;
                }
            }
            return null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // set input textboxes to null
            sysNameTB.Text = null;
            modelTB.Text = null;
            manufacturerTB.Text = null;
            typeTB.Text = null;
            ipAddressTB.Text = null;
            purchaseDateTB.Text = null;
            additionalInfoTB.Text = null;
            // refresh the main window listbox
            MainWindow mw = new MainWindow();
            mw.AssetsListBox.Items.Refresh();
        }

        private void PurchaseDateCB_Checked(object sender, RoutedEventArgs e)
        {
            // make the purchase date textbox visible
            purchaseDateTB.Visibility = Visibility.Visible;
        }

        private void PurchaseDateCB_Unchecked(object sender, RoutedEventArgs e)
        {
            // make the purchase date textbox Hidden
            purchaseDateTB.Visibility = Visibility.Hidden;
        }
    }
}
