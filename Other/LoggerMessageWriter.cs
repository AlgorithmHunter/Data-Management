using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Other
{

    public class LoggingMessageWriter(
ILogger<LoggingMessageWriter> logger)
    {
        public void Write(string message) =>
        logger.LogInformation("Info: {Msg}", message);
    }



}
