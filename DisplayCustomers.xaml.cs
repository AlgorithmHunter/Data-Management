using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using WpfApp1.Models;
using static WpfApp1.MainWindow;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for DisplayCustomers.xaml
    /// </summary>
    public partial class DisplayCustomers : Page
    {
        public delegate void RefreshCustomerEntries();
        public event RefreshCustomerEntries OnRefreshenEntrylist;

      



        public static string search = "";
        private void refreshList()
        {
            DisplayCustomerEntries();
        }
        public DisplayCustomers()
        {
            InitializeComponent();
       
            DisplayCustomerEntries(false,search);
          
  

        }
    
        public void DisplayCustomerEntries(bool sort = false,string filter="")
        {
            var setting = UserSettingsHelpers.GetUserSettings();
            MainWindow.ChangeDataSource(setting==null?0: UserSettingsHelpers.GetUserSettings().DataSourceSetting.Index);


            var customerlist = App.customerdata != null ? App.customerdata.GetCustomerList() : null;

            if (customerlist != null)
            {
                if (sort)
                {
                    int customercount=customerlist.Count;
                    if (((Customer)CustomerList.Items[0]).Id < ((Customer)CustomerList.Items[customercount-1]).Id )
                    {
                        customerlist.Sort((x1, x2) => x2.Id.Value - x1.Id.Value);
                   
                    }
                   
                   
                    CustomerList.ItemsSource = customerlist;

                }
                else
                {

                    if(!string.IsNullOrEmpty(filter))
                    {
                        customerlist = customerlist.Where((x) => x.Name.ToLower().Contains(filter.ToLower())|| x.Lastname.ToLower().Contains(filter.ToLower())||x.Cellphone.ToLower().Contains(filter.ToLower()) || x.Country.ToLower().Contains(filter.ToLower())).ToList();
                    }
                    CustomerList.ItemsSource = customerlist;
                }
     
            }
        }
  static      public void searchData(object sender)
        {
            string searchkey = "";
            var newlist = App.customerdata.GetCustomerList();
            newlist = newlist.Where((x) => x.Name.Contains(searchkey)).ToList();

          
        }


     

        private void IdTouched(object sender, TouchEventArgs e)
        {
            IdClicked(sender);
        }
     public void SearchCustomer(object sender, TextChangedEventArgs e)
        {


        }
        private void IdMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            IdClicked(sender);
        }
        void IdClicked(object sender)
        {
            TextBlock idTextblock = (TextBlock)sender;
            var selecteditem = App.customerdata.GetCustomerList().Where((x) => x.Id == Convert.ToInt32(idTextblock.Text));
            var _customer = selecteditem.FirstOrDefault();
            var AddEntryWindow = new AddEntryWindow(_customer);

            OnRefreshenEntrylist += new RefreshCustomerEntries(refreshList);
            AddEntryWindow.RefreshCustomerEntriesDelegate = OnRefreshenEntrylist;
            AddEntryWindow.ShowDialog();
        }
        private void deleteClicked(object sender, MouseButtonEventArgs e)
        {
            Image imagedeletebutton = (Image)sender;
        var results=    MessageBox.Show("Are you sure you want to delete customer entry", "Delete Box", MessageBoxButton.OKCancel);
            if (results == MessageBoxResult.OK)
            {
                int customerid = Convert.ToInt32(imagedeletebutton.Tag.ToString());
              int deleteresult=  App.customerdata.RemoveCustomerById(customerid);

                if(deleteresult == -1)
                {
                    MessageBox.Show($"Error while deleting customer with id {customerid}","Error Box");
                }
                DisplayCustomerEntries();
            }
           
        }

        private void SortByID(object sender, MouseButtonEventArgs e)
        {
           
            DisplayCustomerEntries(true);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        

        }

       
    }
}
