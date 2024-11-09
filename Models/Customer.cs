using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WpfApp1.Models
{
    public class Customer
    {

        private string ? _name;
        private string? _lastname;
        private double? _customerTotalSpend;

        private DateTime ? _LastPurchaseDate;
        private DateTime? _LastUpdated;
        private DateTime? _DateAdded;
        private int? _id;
        private string ?_country;
        private string? _cellphone;

        public string ?Name { get => _name; set => _name = value; }
        public string ?Lastname { get => _lastname; set => _lastname = value; }
        public int ?Id { get => _id; set => _id = value; }
        public string ?Country { get => _country; set => _country = value; }
        public string ?Cellphone { get => _cellphone; set => _cellphone = value; }
        public double? CustomerTotalSpend { get => _customerTotalSpend; set => _customerTotalSpend = value; }
        public DateTime? LastPurchaseDate { get => _LastPurchaseDate; set => _LastPurchaseDate = value; }
        public DateTime? LastUpdated { get => _LastUpdated; set => _LastUpdated = value; }
        public DateTime? DateAdded { get => _DateAdded; set => _DateAdded = value; }
        public override string ToString()
        {
            return $"{_id},{Name},{Lastname},{Cellphone},{Country},{CustomerTotalSpend},{LastPurchaseDate},{DateAdded},{LastUpdated}";
        }
    }
}
