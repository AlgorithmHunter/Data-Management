using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Other
{
   public class Helpers
    {
         public static DateTime ?castdate(object date)
        {

            DateTime outdate;
            bool isdatevalid = false;
            if (date != null)
            {
                isdatevalid= DateTime.TryParse(date.ToString(), out outdate);
                if (isdatevalid)
                    return outdate;
            }
         
            return null;
        }

    }
}
