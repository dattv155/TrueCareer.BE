using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCareer.Rpc
{
    public class MailRoute : Root
    {
        public const string Default = Rpc + Module + "/mail";

        public const string Authenticate = Default + "/authenticate";
        public const string Create = Default + "/create";
        public const string Resend = Default + "/resend";
        public const string Send = Default + "/send";
    }
}