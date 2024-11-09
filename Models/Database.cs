
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Windows.Controls;
using WpfApp1.Other;
namespace WpfApp1.Models
{
    public enum DataSourceType
    {
        MSSQL,SQLLITE
    }
    public class Database
    {

      
    private IDbConnection _connection;
        private CustomerData? _customerdata;
        private DataSourceType _dataSourceType;
        public CustomerData? Customerdata { get => _customerdata; set => _customerdata = value; }
        public DataSourceType DataSourceType { get => _dataSourceType; set => _dataSourceType = value; }

        public Database()
        {
            _connection = new
             SQLiteConnection(App.Config.GetConnectionString("defaultdatabase"));
    
        }
        public Database(string dbPath,DataSourceType type=DataSourceType.SQLLITE)
        {

            ResetConnection(dbPath,type);


        }
        public void ResetConnection(string dbPath, DataSourceType type = DataSourceType.SQLLITE)
        {
            CloseConnection();


            if (type == DataSourceType.SQLLITE)
            {
                _connection = new
      SQLiteConnection(dbPath);
            }
            else if (type == DataSourceType.MSSQL)
            {
                _connection = new SqlConnection(dbPath);
            }

            _dataSourceType = type;
            OpenConnection();

            Customerdata = new CustomerData(this);
        }
        public void  OpenConnection() {

            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

        
                                       }
        public void CloseConnection() {
            if (_connection != null)
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
          
        
        }
        //Creates a new table using table name and columns
        public int createTable(string TableName,string commaseperatedcolumns){
            int results = -1;
            try
            {
         
                var command = _connection.CreateCommand();
                if (_dataSourceType == DataSourceType.SQLLITE)
                {
                    command.CommandText = $"CREATE TABLE IF NOT EXISTS {TableName} (" + commaseperatedcolumns + ")";
                }else if (_dataSourceType == DataSourceType.MSSQL)
                {
                    command.CommandText = $"if  OBJECT_ID('{TableName}') is  null   CREATE TABLE {TableName} (" + commaseperatedcolumns + ");";
                
                }

                    results = command.ExecuteNonQuery();
         
            }catch(Exception ex)
            {
                // EventLog.WriteEntry("Database", ex.Message);
                Console.WriteLine(ex.Message);
                results = -1;
            }
            return results;
        }
        public  void RemoveData(string sql)
        {
            var command = _connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }
        public int InsertData(string query)
        {
            int results = -1;
            try
            {
            
              if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                    var command = _connection.CreateCommand();
                    command.CommandText = query;
                   
                    results = command.ExecuteNonQuery();
                _connection.Close();
             
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("INSERT", ex.Message);
                results = -1;
            }
            return results;
        }
        public int DeleteData(string query)
        {
            int results = -1;
            try
            {

                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                var command = _connection.CreateCommand();
                command.CommandText = query;

                results = command.ExecuteNonQuery();
                _connection.Close();

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Delete", ex.Message);
                results = -1;
            }
            return results;
        }


        public int UpdateData(string query)
        {
            int results = -1;
            try
            {

                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                var command = _connection.CreateCommand();
                command.CommandText = query;

                results = command.ExecuteNonQuery();
                _connection.Close();

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Update", ex.Message);
                results = -1;
            }
            return results;
        }
        public List<object[]> ? QueryData(string query)
        {
            var newlist = new List<object[]>();
            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }

                var command = _connection.CreateCommand();
            command.CommandText =query;
                command.Connection = _connection;
          var _reader = command.ExecuteReader();


               
            while (_reader.Read())
            {
                
                object[] ?t=new object[_reader.FieldCount];
             _reader.GetValues(t);
                newlist.Add(t);

            }
            } catch (Exception ex)
            {
                EventLog.WriteEntry("QueryData",  ex.Message);
                return null;
            }
            _connection.Close();
            return newlist;
        }

    }


}
