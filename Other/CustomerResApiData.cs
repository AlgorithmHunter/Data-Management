using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WpfApp1.Models;
namespace WpfApp1.Other
{
 public   class CustomerResApiData
    {
        private string _httpuri;
       public CustomerResApiData(string httpuri)
        {
            _httpuri = httpuri;
        }

        public string Httpuri { get => _httpuri; set => _httpuri = value; }
   public     async Task<IEnumerable<Customer>?> GetCustomersAsync()
        {

            try
            {      
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
       var results=    await  httpClient.GetAsync(_httpuri);
            var content =await results.Content.ReadAsStringAsync();

            content =content.Replace("'","\"");
            var list =JsonSerializer.Deserialize<List<Customer>>(content,JsonSerializerOptions.Default);
                return list;
            }catch(Exception ex)
            {
                return null;
            }
         
        }
    }
}
