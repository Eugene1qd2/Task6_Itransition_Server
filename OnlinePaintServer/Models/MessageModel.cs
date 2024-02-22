using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlinePaintServer.Models
{
    public class MessageModel
    {
        public string Command { get; set; }
        public string SenderId { get; set; }
        public object Data { get; set; }
        public MessageModel()
        {
            
        }
    }
}
