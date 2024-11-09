using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WpfApp1.Models;
namespace WpfApp1.Other
{
    public class CustomerData
    {
        private Database _database;

        public Database Database { get => _database; set => _database = value; }

        public CustomerData()
        {
            _database = new Database();
        }
        public CustomerData(Database database)
        {
            _database = database;
        }

        public void createcustomerTable()
        {
            var results = -1;
            if (_database.DataSourceType == DataSourceType.SQLLITE)
            {
                results = Database.createTable("Customer", "id integer primary key AUTOINCREMENT,name varchar(256) null,lastname varchar(256) null,country varchar(256) null,cellphone varchar(20) null,CustomerTotalSpend Real null, LastPurchaseDate datetime null, Dateadded datetime null,Lastupdated datetime null");
            }
            else if (_database.DataSourceType == DataSourceType.MSSQL)
            {
                results = Database.createTable("Customer", "id integer primary key identity,name varchar(256) null,lastname varchar(256) null,country varchar(256) null,cellphone varchar(20) null,CustomerTotalSpend Real null, LastPurchaseDate datetime null, Dateadded datetime null,Lastupdated datetime null");
            }


            if (results == -1)
            {
                //     throw new Exception("Failed to create a new table");
            }
        }
        public void AddCustomerEntry(Customer customer)
        {
            var DFormat_SQLite = App.Config.GetValue<string>("Other:DFormat_SQLite");

            Database.InsertData($"insert into customer(name,lastname,cellphone,country,customerTotalSpend, LastPurchaseDate,dateadded,lastupdated)values('{customer.Name}','{customer.Lastname}','{customer.Cellphone}','{customer.Country}','{customer.CustomerTotalSpend}','{customer.LastPurchaseDate.Value.ToString(DFormat_SQLite)}','{customer.DateAdded.Value.ToString(DFormat_SQLite)}','{customer.LastUpdated.Value.ToString(DFormat_SQLite)}')");
        }
        public int UpdateCustomerEntry(Customer customer)
        {
            var DFormat_SQLite = App.Config.GetValue<string>("Other:DFormat_SQLite");

           return Database.UpdateData($"update customer set name='{customer.Name}',lastname='{customer.Lastname}',cellphone='{customer.Cellphone}',country='{customer.Country}',customerTotalSpend='{customer.CustomerTotalSpend}', LastPurchaseDate='{customer.LastPurchaseDate.Value.ToString(DFormat_SQLite)}',dateadded='{customer.DateAdded.Value.ToString(DFormat_SQLite)}',lastupdated='{customer.LastUpdated.Value.ToString(DFormat_SQLite)}' where id='{customer.Id}'");
        }
        public void RemoveEntries()
        {
            Database.DeleteData("delete from customer");
        }
        public int RemoveCustomerById(int id)
        {
            var query = $"delete from customer where id='{id}'";
        /*    if (Database.DataSourceType == DataSourceType.SQLLITE)
            {
                query = $"delete * customer where id='{id}'";
            }*/
            return  Database.DeleteData(query);
        }
        public string GetCustomerCsvList()
        {
            var _customerList = Database.QueryData("select id,name,lastname,cellphone,country,customerTotalSpend, LastPurchaseDate,dateadded,lastupdated from customer");
            var newlist = new List<Customer>();
            StringBuilder stringBuilder = new StringBuilder();

            foreach (object[] item in _customerList)
            {
                stringBuilder.Append($"{item[0].ToString()},{item[1].ToString()},{item[2].ToString()},{item[3].ToString()}" +
                    $",{item[4].ToString()},{item[5].ToString()},{item[6].ToString()},{item[7].ToString()},{item[8].ToString()}\n");
            }

            return stringBuilder.ToString();
        }
        public string GetCustomerJsonList()
        {                   
           var jsoncustomers= JsonSerializer.Serialize(GetCustomerList());
            return jsoncustomers;
        }
        public List<Customer> GetCustomerList()
        {

            var _customerList = Database.QueryData("select id,name,lastname,cellphone,country,customerTotalSpend, LastPurchaseDate,dateadded,lastupdated from customer");
            var newlist = new List<Customer>();
            foreach (object[] item in _customerList)
            {
                var customer = new Customer();
                customer.Id = Convert.ToInt32(item[0]);
                customer.Name = Convert.ToString(item[1]);
                customer.Lastname = Convert.ToString(item[2]);
                customer.Cellphone = Convert.ToString(item[3]);
                customer.Country = Convert.ToString(item[4]);
                customer.CustomerTotalSpend = Convert.ToDouble(item[5]);
                customer.LastPurchaseDate = Convert.ToDateTime(item[6]);
                customer.DateAdded = Convert.ToDateTime(item[7]);
                customer.LastUpdated = Convert.ToDateTime(item[8]);
                newlist.Add(customer);
            }
            return newlist;

        }

    }
}
