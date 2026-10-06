using System;
using System.Collections.Generic;
using System.Text;

namespace TeacherERP.Infrastructure.Settings
{
    public class MailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Mail { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
