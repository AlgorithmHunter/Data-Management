using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace WpfApp1.Models
{
    public class UserSettings
    {
        private DataSourceSetting _DataSourceSetting;

        public DataSourceSetting DataSourceSetting { get => _DataSourceSetting; set => _DataSourceSetting = value; }
    }
    public class DataSourceSetting
    {
        private string _Name;
        private int _Index;

        public string Name { get => _Name; set => _Name = value; }
        public int Index { get => _Index; set => _Index = value; }
    }
    public class UserSettingsHelpers
    {

        public static void CreateUserSettings()
        {
            var fs = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)) + "/userSettings.json", FileMode.Create, FileAccess.ReadWrite);
           var sr = new StreamWriter(fs);
                sr.WriteAsync(JsonSerializer.Serialize(new UserSettings() { DataSourceSetting = new DataSourceSetting() { Name= "SQLLITE", Index=0 } }));
            sr.Close();
            fs.Close();
        }


        public static void SaveUserSettings(UserSettings setting)
        {
            try
            {
               var sett = JsonSerializer.Serialize(setting);
                 File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)) + "/userSettings.json", sett);
            
            }
            catch(Exception ex)
            {
               
            }
        }
        public static UserSettings? GetUserSettings()
        {
            UserSettings? setting = null;
            try
            {
                var fs = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)) + "/userSettings.json", FileMode.Open, FileAccess.Read);
                var sr = new StreamReader(fs);
                var settingData = sr.ReadToEnd();
                    setting = JsonSerializer.Deserialize<UserSettings>(settingData);
                sr.Close();
                fs.Close();
            }catch (Exception ex)
            {
                return null;
            }
       

            return setting;
        }
    }
}
