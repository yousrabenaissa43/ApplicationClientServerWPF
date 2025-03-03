using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPClient.model.BL
{
    public class Log
    {
        public int Id { get; set; }  // Primary key
        public string Message { get; set; }  // Log message
        public DateTime Timestamp { get; set; }  // When the message was logged
    }
}
