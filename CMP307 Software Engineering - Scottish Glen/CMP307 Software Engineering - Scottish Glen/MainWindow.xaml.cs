using MySql.Data.MySqlClient;
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

namespace CMP307_Software_Engineering___Scottish_Glen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // database connection string
            string connectionString = "server=lochnagar.abertay.ac.uk;user id=sql2205883;password=U28c2jYBnhZN;database=sql2205883";
            // create mysql connection
            MySqlConnection conn = new MySqlConnection(connectionString);
            // open database connection
            conn.Open();
            // create mysql command
            MySqlCommand command = new MySqlCommand("SELECT * FROM Asset", conn);
            // read through database records
            MySqlDataReader reader = command.ExecuteReader();
            // add assets to listbox
            while (reader.Read())
            {
                AssetsListBox.Items.Add("Asset ID: " + reader.GetString(0) + " System Name: " + reader.GetString(1) +
                    "\nModel: " + reader.GetString(2) + " Manufacturer: " + reader.GetString(3) 
                    + "\nType: " + reader.GetString(4) + "IP Address" + reader.GetString(5));
            }
            // close database connection
            conn.Close();
        }

        private void AddAssetButton(object sender, RoutedEventArgs e)
        {
            // open the Add Asset window
            AddAsset AddA = new AddAsset();
            AddA.Show();
        }
    }
}
