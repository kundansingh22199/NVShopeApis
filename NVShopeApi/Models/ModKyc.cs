using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NVShopeApi.Models
{
    public class ModKyc
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public string AccountType { get; set; }
        public string Branch { get; set; }
        public string Ifsc { get; set; }
        public string AadharNo { get; set; }
        public string PancardNo { get; set; }
        public string PanDoc { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string Nominee { get; set; }
        public string Relation { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string District { get; set; }
        public string PinCode { get; set; }
        public string UserImg { get; set; }
    }
}
