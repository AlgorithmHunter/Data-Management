using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
using System.Xml.Linq;
using WpfApp1.Models;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for AddEntryWindow.xaml
    /// </summary>
    public partial class AddEntryWindow : Window
    {
        private MainWindow newwindow;
        public MainWindow.RefreshlistData AddEntryDelegate;
        public DisplayCustomers.RefreshCustomerEntries RefreshCustomerEntriesDelegate;
        private Customer SelectedCustomer;
        private int ComandAction = 0;
        public AddEntryWindow()
        {
            InitializeComponent();
        
        }
        public AddEntryWindow(Customer customer)
        {
            InitializeComponent();
            SelectedCustomer = customer;
            txtName.Text = SelectedCustomer.Name;
            txtCellphone.Text = SelectedCustomer.Cellphone;
            txtSurname.Text = SelectedCustomer.Lastname;
            txtTotalSpend.Text = SelectedCustomer.CustomerTotalSpend.ToString();
            txtCountry.Text = SelectedCustomer.Country;
            dPickerLastPurchased.SelectedDate = SelectedCustomer.LastPurchaseDate;
            cmdSubmit.Content = "Update";
            ComandAction = 1;


        }
        private void cmdSubmit_Click(object sender, RoutedEventArgs e)
        {
            ChangeCustomerData(ComandAction);
        }
        void ChangeCustomerData(int option)
        {
            if (!string.IsNullOrEmpty(txtCellphone.Text) && !string.IsNullOrEmpty(txtName.Text)
                 && !string.IsNullOrEmpty(txtSurname.Text) && !string.IsNullOrEmpty(txtTotalSpend.Text) && !string.IsNullOrEmpty(txtCountry.Text) && !(dPickerLastPurchased.SelectedDate == null))
            {
                if (option == 0)


                {


                    App.customerdata.AddCustomerEntry(new Customer()
                    {
                        Cellphone = txtCellphone.Text,
                        Name = txtName.Text,
                        Lastname = txtSurname.Text,
                        Country = txtCountry.Text,
                        CustomerTotalSpend = Convert.ToDouble(txtTotalSpend.Text),
                        DateAdded = DateTime.Now,
                        LastPurchaseDate = dPickerLastPurchased.SelectedDate,
                        LastUpdated = DateTime.Now

                    });

                    newwindow = new MainWindow();
                    newwindow.Owner = this;


                    AddEntryDelegate.DynamicInvoke();
                }
                else if (option == 1)
                {
                    SelectedCustomer.Cellphone = txtCellphone.Text;
                    SelectedCustomer.Name = txtName.Text;
                    SelectedCustomer.Lastname = txtSurname.Text;
                    SelectedCustomer.Country = txtCountry.Text;
                    SelectedCustomer.CustomerTotalSpend = Convert.ToDouble(txtTotalSpend.Text);         
                    SelectedCustomer.LastPurchaseDate = dPickerLastPurchased.SelectedDate;
                    SelectedCustomer.LastUpdated = DateTime.Now;
             int results=       App.customerdata.UpdateCustomerEntry(SelectedCustomer);
                    if (results == -1)
                    {
                        MessageBox.Show("Failed to update the customer");
                    }
                    else
                    {
                        RefreshCustomerEntriesDelegate.DynamicInvoke();
                        MessageBox.Show("Customer data was updated succesfully");
                    }
                    
                }
              

            }
            else
            {

                MessageBox.Show("Entries must not be empty");
            }
        }
    
    }
}
