using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace NVShopeApi.Models
{
    public class ModEPin
    {
        public string RegNo { get; set; }
        public string Amount { get; set; }
        public string PMode { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string ClientName { get; set; }
        public string ChequeNo { get; set; }
        public string AccountHName { get; set; }
        public string UtrNo { get; set; }
        public string Remarks { get; set; }
        public string Receipt { get; set; }
        public string PaytmNo { get; set; }
        public string PinNo { get; set; }
        public string Status { get; set; }
    }
}