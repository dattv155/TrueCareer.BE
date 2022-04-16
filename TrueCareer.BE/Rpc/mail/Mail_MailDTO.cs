using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Rpc.mail
{
    public class Mail_MailDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<string> Recipients { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}