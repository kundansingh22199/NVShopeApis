using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NVShopeApi.Models
{
    public class ModLogin
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }
    public class ModUserlogin
    {
        public string Name { get; set; }
        public string UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}