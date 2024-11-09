using System.Configuration;
using System.Data;
using System.IO;
using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WpfApp1.Models;
using WpfApp1.Other;
using System.Reflection;
namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {


        public static CustomerData? customerdata { get; set; }
        public static CustomerResApiData? customerresapi { get; set; }
        public static IConfiguration? Config { get; private set; }
        public App()
        {

            try
            {



                Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
                   var services = new ServiceCollection();
             //       WpfApp1.Other.wServiceCollection.AddDatabaseService(services);
                 //   services.AddDatabaseService();
                    services.AddSingleton<LoggingMessageWriter>();
                    services.BuildServiceProvider();


                var dataSource = UserSettingsHelpers.GetUserSettings();
                DataSourceType dataSourceType = DataSourceType.SQLLITE;
                if (dataSource == null)
                {
                    dataSource = new UserSettings() { DataSourceSetting = new DataSourceSetting() { Name = "customerdatabaseSQLLITE", Index = 0 } };
                }
                if (dataSource.DataSourceSetting.Name.Contains("MSSQL"))
                {
                    dataSourceType = DataSourceType.MSSQL;
                }
                string? dbPath = App.Config.GetConnectionString(dataSource.DataSourceSetting.Name);

                if (App.Config.GetValue<string>("Other:DBConnectionPathType").ToLower() == "default")
                {

                    if (dataSource.DataSourceSetting.Index == 0)
                    {
                        dbPath = "Data Source=" + System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Customers.db");
                    }
                    else if (dataSource.DataSourceSetting.Index == 1)
                    {
                        dbPath = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={System.IO.Path.GetFullPath("Data/Customers.mdf")};Integrated Security=True;Connect Timeout=30";
                    }
                }
                customerdata = new Database(dbPath == null ? "Data Source=" + Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Customers.db") : dbPath, dataSourceType).Customerdata;

                string? resuri = App.Config.GetValue<string>("Other:ResApiUri");
                if (resuri != null)
                {
                    customerresapi = new CustomerResApiData(resuri);
                }

            }
            catch (Exception ex)
            {
              //  File.AppendAllText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CustomerTraceLog.txt"), ex.Message );
                MessageBox.Show(ex.Message);
            }
        }
    protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
           
        }
    }

}
