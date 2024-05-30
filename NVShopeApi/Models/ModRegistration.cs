using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NVShopeApi.Models
{
    public class ModRegistration
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Nominee { get; set; }
        public string Relation { get; set; }
        public string SponsorId { get; set; }
        public string UserID { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string PinCode { get; set; }
        public string PinNo { get; set; }
        public string PinValue { get; set; }
        public string PaymentMode { get; set; }
        public string USDTAddress { get; set; }
        public string BinanceId { get; set; }
        public string BinancePayId { get; set; }
        public string Password { get; set; }
        public string confirmPassword { get; set; }
        public string UserImg { get; set; }
        public string panno {  get; set; }
    }
    public class ModChangePassword
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

    }
    public class Respname
    {
        public string Name { get; set; }
    }
    public class ModToup
    {

        public string Amount { get; set; }
        public string DepositeId { get; set; }
        public string Password { get; set; }


    }
    public class ModWithdrawal
    {
        public string Amount { get; set; }
        public string usdtAddress { get; set; }
        public string Rate { get; set; }
        public string totaltoken { get; set; }
        public string Password { get; set; }

        public string Userid { get; set; }

        public string otp { get; set; }


    }
    public class ModWalletTransfer
    {
        public string Amount { get; set; }
        public string DepositeId { get; set; }
        public string Password { get; set; }

    }
    public class ModBuySU
    {

        public string token { get; set; }
        public string rate { get; set; }
        public string fromid { get; set; }
        public string amt { get; set; }
        public string HashNo { get; set; }

        public string recieveraddress { get; set; }

    }
    public class ModSupport
    {
        public string sendto { get; set; }
        public string mailsendto { get; set; }
        public string subject { get; set; }
        public string message { get; set; }

    }
    public class Modmail
    {
        public int id { get; set; }

    }
    public class ModForgetPassword
    {
        public string Userid { get; set; }

    }
    public class ModSendOtp
    {
        public string Userid { get; set; }

        public string otp { get; set; }

    }
    public class ModPassword
    {
        public string Password { get; set; }
    }
    public class modtree<T>
    {
        public string Status { get; set; }
        public T Payload { get; set; }
        public string Message { get; set; }
        public T userid { get; set; }

        public T Redcolor { get; set; }

        public T Greencolor { get; set; }

    }
    public class moddirecttree
    {
        public string userid { get; set; }
    }
    public class moddrawal
    {
        public string Amount { get; set; }
        public string usdtAddress { get; set; }
        public string Rate { get; set; }

        public string totaltoken { get; set; }
        public string Password { get; set; }

        public string otp { get; set; }
        public string Emailid { get; set; }
    }
    public class OtpModel
    {
        public string Status { get; set; }

        public string otp { get; set; }
        public string masterotp { get; set; }
        public string message { get; set; }

        public string Password { get; set; }





    }
}
