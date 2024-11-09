using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using WpfApp1.Models;

using WpfApp1.Other;
using Microsoft.Win32;
using System.IO;
using static WpfApp1.MainWindow;
using System.Diagnostics;

using System.Text.Json;
using MaterialDesignThemes.Wpf;
using System.Security.Policy;
using System.Reflection;
#pragma warning disable CS8602
namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    enum FileType
    {
        CSV,TEXT,JSON
    };
    public partial class MainWindow : Window
    {
        static bool canupdatelist = true;
        static bool isthreadrunning = true;
        public delegate void RefreshlistData();
        public event RefreshlistData OnRefreshlist;
  
  

        bool firstLoad = true;
        private void refreshList()
        {
            RefreshDataMethod();
        }
        public MainWindow()
        {
            InitializeComponent();
            if (App.customerdata == null)
                return;
            App.customerdata.createcustomerTable();
         
 
        }
   

        public  static   void ChangeDataSource(int index)
        {
            try
            {

           
            if (App.customerdata == null)
                return;
           
            if (index == 0)
            {
                var dbPath = App.Config.GetConnectionString("customerdatabaseSQLLITE");
                if (App.Config.GetValue<string>("Other:DBConnectionPathType").ToLower() =="default")
                {
               
                    dbPath = "Data Source=" + System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Customers.db");

                }
              
                App.customerdata.Database.ResetConnection(dbPath, DataSourceType.SQLLITE);
                UserSettingsHelpers.SaveUserSettings(new UserSettings() { DataSourceSetting = new DataSourceSetting() { Name = "customerdatabaseSQLLITE", Index = 0 }     });
            }
            else if (index == 1)
            {
                var dbPath = App.Config.GetConnectionString("customerdatabaseMSSQL");

                if (App.Config.GetValue<string>("Other:DBConnectionPathType").ToLower() == "default")
                {
                    dbPath = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={ System.IO.Path.GetFullPath("Data/Customers.mdf")};Integrated Security=True;Connect Timeout=30";

                }

                App.customerdata.Database.ResetConnection(dbPath, DataSourceType.MSSQL);
                UserSettingsHelpers.SaveUserSettings(new UserSettings() { DataSourceSetting = new DataSourceSetting() { Name = "customerdatabaseMSSQL", Index = 1 } });
            }
            App.customerdata.createcustomerTable();
            }
            catch (Exception ex)
            {
               
            //    File.AppendAllText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CustomerTraceLog.txt"), ex.Message+" " + Environment.CurrentDirectory);
                MessageBox.Show(ex.Message);
                
            }
        }
    
      
        static int comboselectedIndex = 0;
        private void selectition_Changed(object sender, SelectionChangedEventArgs e)
        {
            comboselectedIndex = SqlDataSourceCombo.SelectedIndex;
            LoadingProgress.Visibility = Visibility.Visible;
            var comboitem = new ComboBoxItem() { Content = "" };
            Task.Run(() =>
            {


                Dispatcher.Invoke(new Action(() =>
                {
                    comboitem = (ComboBoxItem)SqlDataSourceCombo.SelectedValue;
                    SqlDataSourceCombo.IsEnabled = false;
            ;

                }));

                ChangeDataSource(comboselectedIndex);
                Dispatcher.Invoke(new Action(() =>
                {
                    LoadingProgress.Visibility = Visibility.Collapsed;
                    SqlDataSourceCombo.IsEnabled = true;               
                    MainContentFrame.Refresh();
                }));
            });


        }

        private void AddEntry_Click(object sender, RoutedEventArgs e)
        {
            var newwindow = new AddEntryWindow();
            OnRefreshlist += new RefreshlistData(refreshList);
            newwindow.AddEntryDelegate = OnRefreshlist;
            newwindow.ShowDialog();
      

        }
        public void RefreshDataMethod()
        {
      
            MainContentFrame.Refresh();
        }
        public void RefreshData_Click(object sender, RoutedEventArgs e)
        {
       
            RefreshDataMethod();

        }

 
        private void Export_Click(object sender, RoutedEventArgs e)
        {
            export(FileType.CSV);
        }
        void ImportDataFromFile(FileType importtype,Stream fs)
        {
            if (App.customerdata == null)
                return;

            var sr = new StreamReader(fs);
            if (importtype == FileType.CSV)
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var item = line.Split(",");
                    if (item.Length == 9)
                    {
                        var customer = new Customer();
                        customer.Id = Convert.ToInt32(item[0]);
                        customer.Name = Convert.ToString(item[1]);
                        customer.Lastname = Convert.ToString(item[2]);
                        customer.Cellphone = Convert.ToString(item[3]);
                        customer.Country = Convert.ToString(item[4]);
                        customer.CustomerTotalSpend = Convert.ToDouble(item[5]);
                        customer.LastPurchaseDate = Helpers.castdate(item[6]);
                        customer.DateAdded = Helpers.castdate(item[7]);
                        customer.LastUpdated = Helpers.castdate(item[8]);
                        App.customerdata.AddCustomerEntry(customer);
                    }
                }
            }
            else if (importtype == FileType.JSON)
            {
                try
                {


                    var jsonstring = sr.ReadToEnd();
                    var jsoncustomerlist = JsonSerializer.Deserialize<List<Customer>>(jsonstring);
                    if (jsoncustomerlist != null)
                    {


                        if (jsoncustomerlist.Count > 0)
                        {
                            foreach (Customer customer in jsoncustomerlist)
                            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                                App.customerdata.AddCustomerEntry(customer);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                            }
                        }

                    }
                }catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Box", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            sr.Close();
            fs.Close();
        }
       void export(FileType exporttype)
        {
            LoadingProgress.Visibility = Visibility.Visible;

            if (App.customerdata == null)
                return;
            var openfileDLG = new OpenFolderDialog();
            if (openfileDLG.ShowDialog() == true)
            {
                var filepath = openfileDLG.FolderName + "/" + $"Export-{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}-{Guid.NewGuid()}";
                if (exporttype == FileType.TEXT)
                {
                    filepath +=(".txt");
                }else if (exporttype == FileType.CSV)
                {
                    filepath += (".csv");
                }else if (exporttype==FileType.JSON)
                {
                    filepath += (".json");
                }
                Task.Run(() =>
                {


                    bool isexportSuccessfull = false;
                    if (exporttype == FileType.CSV || exporttype == FileType.TEXT)
                    {

                        using (var fs = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            using (var str = new StreamWriter(fs))
                            {
                                str.Write(App.customerdata.GetCustomerCsvList());
                                isexportSuccessfull = true;
                            }
                        }

                    }
                    else if (exporttype == FileType.JSON)
                    {
                        File.WriteAllText(filepath, App.customerdata.GetCustomerJsonList());
                        isexportSuccessfull = true;

                    }
                    if (isexportSuccessfull)
                    {

                        Dispatcher.Invoke(new Action(() =>
                        {
                            LoadingProgress.Visibility = Visibility.Collapsed;
                            MessageBox.Show("File exported successfully");

                        }));
                    }
                       
                });
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog= new OpenFileDialog();

           openFileDialog.Filter= "CSV File|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {



                var fs = openFileDialog.OpenFile();

                LoadingProgress.Visibility = Visibility.Visible;
                Task.Run(() =>
                {
                    ImportDataFromFile(FileType.CSV, fs);


                    Dispatcher.Invoke(new Action(() =>
                    {
                        LoadingProgress.Visibility = Visibility.Collapsed;
                        MessageBox.Show("Done adding new entries");

                        MainContentFrame.Refresh();
                    }));



                });
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var setting = UserSettingsHelpers.GetUserSettings();
            var _index = setting == null ? 0 : UserSettingsHelpers.GetUserSettings().DataSourceSetting.Index;
            SqlDataSourceCombo.SelectedIndex = _index;
        }

    
        private void ExportCsv_Click(object sender, RoutedEventArgs e)
        {
            export(FileType.CSV);
        }
        private void ExportTXT_Click(object sender, RoutedEventArgs e)
        {
            export(FileType.TEXT);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ExportJSON_Click(object sender, RoutedEventArgs e)
        {
            export(FileType.JSON);
        }

        private void ImportJSON_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "JSON File|*.json";
            if (openFileDialog.ShowDialog() == true)
            {

                var fs = openFileDialog.OpenFile();
                LoadingProgress.Visibility = Visibility.Visible;

                Task.Run(() =>
                {
                    ImportDataFromFile(FileType.JSON, fs);


                    Dispatcher.Invoke(new Action(() =>
                    {
                        LoadingProgress.Visibility = Visibility.Collapsed;
                        MessageBox.Show("Done adding new entries");
                        MainContentFrame.Refresh();
                    }));



                });
            }
        }

        private void ImportHttpJSON_Click(object sender, RoutedEventArgs e)
        {
            LoadingProgress.Visibility = Visibility.Visible;

            Task.Run(async() =>
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var customerlist = await App.customerresapi.GetCustomersAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                if (customerlist != null)
                {
                    foreach (Customer customer in customerlist)
                    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        App.customerdata.AddCustomerEntry(customer);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    }
                    Dispatcher.Invoke(new Action(() =>
                    {
                        MessageBox.Show("Done adding new entries");
                        MainContentFrame.Refresh();
                        LoadingProgress.Visibility = Visibility.Collapsed;
                    }));


                }
                else
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        MessageBox.Show("Error occured while trying to add new entries", "Error Box", MessageBoxButton.OK, MessageBoxImage.Error);
                        MainContentFrame.Refresh();
                        LoadingProgress.Visibility = Visibility.Collapsed;
                    }));
                }
            });

        }

     

        private void SearchCustomer(object sender, TextChangedEventArgs e)
        {

            DisplayCustomers.search = customerSuggestBox.Text;
            MainContentFrame.Refresh();
        }

        private void DeleteAllCustomers(object sender, RoutedEventArgs e)
        {
            if (App.customerdata == null)
                return;
            var results = MessageBox.Show("Are you sure you want to delete all customer entries", "Delete Box", MessageBoxButton.OKCancel);
            if (results == MessageBoxResult.OK)
            {
                LoadingProgress.Visibility = Visibility.Visible;
                App.customerdata.RemoveEntries();
                MainContentFrame.Refresh();
                LoadingProgress.Visibility = Visibility.Collapsed;
            }
        }
    }
}
