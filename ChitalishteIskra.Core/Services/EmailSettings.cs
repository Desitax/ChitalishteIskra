using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Core.Services
{
    public class EmailSettings
    {
        public string SenderEmail { get; set; } = null!;
        public string SenderName { get; set; } = null!;
        public string SenderPassword { get; set; } = null!;
        public string SmtpServer { get; set; } = null!;
        public int SmtpPort { get; set; }
    }
}
