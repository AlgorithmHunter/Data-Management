using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;
namespace WpfApp1.Other
{
    public static class wServiceCollection
    {

        public static IServiceCollection AddDatabaseService(this IServiceCollection builder)
        {
            builder.AddSingleton<Database>(serviceProvider =>
            {
                string? dbPath = "Data Source=" +"C://";// App.Config.GetConnectionString("customerdatabase");
                return new Database(dbPath == null ? "Data Source="+ Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Default.db") : dbPath,DataSourceType.SQLLITE);
            });
            return builder;
        }
    }
}