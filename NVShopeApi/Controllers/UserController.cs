using MetaMinesAPI.ClassFiles;
using Microsoft.IdentityModel.Tokens;
using NVShopeApi.ClassFiles;
using NVShopeApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting;
using System.Web;
using System.Web.Http;
using static Microsoft.IdentityModel.Tokens.Utility;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using BCrypt.Net;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Reflection;
using System.Web.Helpers;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace NVShopeApi.Controllers
{
    [RoutePrefix("api")]
    public class UserController : ApiController
    {
        DAL dl = new DAL();

        [HttpPost]
        [Route("UserLogin")]
        public CommonResponse UserLogin(ModLogin objlogin)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                DataTable dt = new DataTable();
                if ((objlogin.UserId != "" || objlogin.UserId != string.Empty) && (objlogin.Password != "" || objlogin.Password != string.Empty))
                {
                    dt = dl.getlogin(objlogin);
                    if (dt != null)
                    {
                        ModUserlogin objmod = new ModUserlogin();
                        objmod.UserId = Convert.ToString(dt.Rows[0]["Userid"]);
                        int Status = Convert.ToInt32(dt.Rows[0]["st"]);
                        objmod.Name = Convert.ToString(dt.Rows[0]["uName"]);
                        if (Status == 1)
                        {
                            objmod.RefreshToken = TokenManager.GenerateToken(Convert.ToString(objmod.UserId));
                            comm.Status = "1";
                            comm.Payload = objmod;
                            comm.Message = "Login Successfull";
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Login Failed";
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Login Failed";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "All Field Are Required";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [HttpPost]
        [Route("Autosignup")]
        public CommonResponse UserRegistration(ModRegistration objreg)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                if ((objreg.SponsorId != null || objreg.SponsorId != string.Empty) && (objreg.MobileNo != null || objreg.MobileNo != string.Empty) && (objreg.Position != null || objreg.Position != string.Empty) && (objreg.Email != null || objreg.Email != string.Empty) && (objreg.Password != null || objreg.Password != string.Empty) && (objreg.PinValue != null || objreg.PinValue != string.Empty))
                {
                    if (objreg.Password == objreg.confirmPassword)
                    {
                        if (dl.GetSponsorId(objreg))
                        {
                            if (!dl.CheckMobileExist(objreg))
                            {
                                if (!dl.GetEmail(objreg))
                                {
                                    if (!dl.CheckPanNoExist(objreg))
                                    {
                                        if (objreg.PinValue.ToLower() == "free")
                                        {
                                            objreg.PinNo = "1212";
                                        }
                                        else
                                        {
                                            objreg.PinNo = "0";
                                        }
                                        int i = (int)dl.CreateAccount(objreg);
                                        if (i > 0)
                                        {
                                            int id = dl.GetIdBySponsorId(objreg);
                                            string RegNo = dl.GetAppMstRegNo(objreg, id);
                                            CommonRegisterDetails crd = new CommonRegisterDetails();
                                            crd.Id = id;
                                            crd.RegNo = RegNo;
                                            crd.Password = objreg.Password;
                                            crd.SponsorId = objreg.SponsorId;
                                            crd.Email = objreg.Email;
                                            crd.MobileNo = objreg.MobileNo;
                                            SendSMS(RegNo, objreg.Name, objreg.Password, objreg.MobileNo);
                                            SendRegistrationInfo(RegNo, objreg.Email, objreg.Password, objreg.Name);
                                            comm.Status = "1";
                                            comm.Payload = crd;
                                            comm.Message = "Registration Successfully";
                                        }
                                        else
                                        {
                                            comm.Status = "0";
                                            comm.Message = "Registration Failed.. ";

                                        }
                                    }
                                    else
                                    {
                                        comm.Status = "0";
                                        comm.Message = "panno Can Use Only 3 Times..";
                                    }
                                }
                                else
                                {
                                    comm.Status = "0";
                                    comm.Message = "Email Id Can Use Only 3 Times..";
                                }
                            }
                            else
                            {
                                comm.Status = "0";
                                comm.Message = "Mobile No Can Use Only 3 Times..";
                            }
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "This Sponsor Id Not Available ";
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Your Password and Confirm Password are not Match";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "All Fields are required ";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [HttpPost]
        [Route("Signupbypin")]
        public CommonResponse UserRegistrationByPinNo(ModRegistration objreg)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                if ((objreg.SponsorId != null || objreg.SponsorId != string.Empty) && (objreg.MobileNo != null || objreg.MobileNo != string.Empty) && (objreg.Position != null || objreg.Position != string.Empty) && (objreg.Email != null || objreg.Email != string.Empty) && (objreg.Password != null || objreg.Password != string.Empty) && (objreg.PinNo != null || objreg.PinNo != string.Empty))
                {
                    if (objreg.Password == objreg.confirmPassword)
                    {
                        if (dl.GetSponsorId(objreg))
                        {
                            if (!dl.CheckMobileExist(objreg))
                            {
                                if (!dl.GetEmail(objreg))
                                {
                                    if (!dl.CheckPanNoExist(objreg))
                                    {
                                        int pinno = dl.GetPinNo(objreg);
                                        if (pinno == 1)
                                        {
                                            int i = (int)dl.CreateAccount(objreg);
                                            if (i > 0)
                                            {
                                                int id = dl.GetIdBySponsorId(objreg);
                                                string RegNo = dl.GetAppMstRegNo(objreg, id);
                                                CommonRegisterDetails crd = new CommonRegisterDetails();
                                                crd.Id = id;
                                                crd.RegNo = RegNo;
                                                crd.Password = objreg.Password;
                                                crd.SponsorId = objreg.SponsorId;
                                                crd.Email = objreg.Email;
                                                crd.MobileNo = objreg.MobileNo;

                                                SendSMS(RegNo, objreg.Name, objreg.Password, objreg.MobileNo);
                                                SendRegistrationInfo(RegNo, objreg.Email, objreg.Password, objreg.Name);
                                                comm.Status = "1";
                                                comm.Payload = crd;
                                                comm.Message = "Registration Successfully";
                                            }
                                            else
                                            {
                                                comm.Status = "0";
                                                comm.Message = "Registration Failed.. ";

                                            }
                                        }
                                        else
                                        {
                                            comm.Status = "0";
                                            comm.Message = "Please Enter a valid Pin No.!";
                                        }
                                    }
                                    else
                                    {
                                        comm.Status = "0";
                                        comm.Message = "panno Can Use Only 3 Times..";
                                    }
                                }
                                else
                                {
                                    comm.Status = "0";
                                    comm.Message = "Email Id Can Use Only 3 Times..";
                                }
                            }
                            else
                            {
                                comm.Status = "0";
                                comm.Message = "Mobile No Can Use Only 3 Times..";
                            }
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "This Sponsor Id Not Available ";
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Your Password and Confirm Password are not Match";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "All Fields are required ";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [HttpGet]
        [Route("Checksponsor")]
        public CommonResponse checkSponsor(string sid)
        {
            CommonResponse comm = new CommonResponse();
            if (!string.IsNullOrEmpty(sid))
            {
                Respname rsp = new Respname();
                string name = dl.GetSponsor(sid);
                if (name != "" && name != null)
                {
                    rsp.Name = name;
                    comm.Status = "1";
                    comm.Payload = rsp;
                    comm.Message = "Sponsor Id Avilable";
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Sponsor Id Not Available ";
                }
            }
            else
            {
                comm.Status = "0";
                comm.Message = "Sid is Required ";
            }
            return comm;
        }
        [Route("ChangePassword")]
        [HttpPost]
        public CommonResponse ChangePassword(ModChangePassword objpsd)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string username = checkvalidate_user();
                if (username != "" && username != string.Empty && username!=null)
                {
                    string pass = "";
                    int st = 0;

                    if ((objpsd.OldPassword != "" || objpsd.OldPassword != string.Empty) && (objpsd.NewPassword != "" || objpsd.NewPassword != string.Empty))
                    {
                        pass = dl.getpassword(username, "getpass");

                        if (pass == objpsd.OldPassword)
                        {
                            st = dl.updatePassword(objpsd, username, "changepass");

                            if (st == 1)
                            {
                                comm.Status = "1";
                                comm.Message = "Your Password Changed Successfull";
                                comm.Payload = "";
                            }
                            else
                            {
                                comm.Status = "0";
                                comm.Message = "Your Password Changed Failed";
                            }

                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Your Old Password Is Wrong";
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "All Field Are Required";
                    }

                }
                else
                {
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }

            return comm;
        }
        [Route("ChangeTranPassword")]
        [HttpPost]
        public CommonResponse ChangeTranPassword(ModChangePassword objpsd)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string username = checkvalidate_user();
                
                if (username != "" && username != string.Empty && username != null)
                {
                    string pass = "";
                    int st = 0;

                    if ((objpsd.OldPassword != "" || objpsd.OldPassword != string.Empty) && (objpsd.NewPassword != "" || objpsd.NewPassword != string.Empty))
                    {
                        pass = dl.getpassword(username, "gettrxnpass");

                        if (pass == objpsd.OldPassword)
                        {
                            st = dl.updatePassword(objpsd, username, "trxnpass");

                            if (st == 1)
                            {
                                comm.Status = "1";
                                comm.Message = "Your Password Changed Successfull";
                                comm.Payload = "";
                            }
                            else
                            {
                                comm.Status = "0";
                                comm.Message = "Your Password Changed Failed";
                            }

                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Your Old Password Is Wrong";
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "All Field Are Required";
                    }

                }
                else
                {
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }

            return comm;
        }
        [Route("Dashboarddetails")]
        [HttpGet]
        public CommonResponse UserDashBoard()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string username = checkvalidate_user();
                if (username != "" && username != string.Empty && username!=null)
                {
                    DataTable dt1 = new DataTable();
                    dt1 = dl.getdashboard(username);
                    if (dt1 != null && dt1.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Payload = dt1;
                        comm.Message = "Details Fetch Successfully";
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Fetch";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }

            return comm;
        }
        [HttpGet]
        [Route("Dashboarddetails2")]
        public CommonResponse UserDashBoard2()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string username = checkvalidate_user();
                if (username != "" && username != string.Empty && username != null)
                {
                    DataTable dt1 = new DataTable();
                    dt1 = dl.getdashboard2(username);
                    if (dt1 != null && dt1.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Payload = dt1;
                        comm.Message = "Details Fetch Successfully";
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Fetch";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }

            return comm;
        }
        [Route("GetPopup")]
        [HttpGet]
        public CommonResponse GetPupup() 
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                DataTable dt = new DataTable();
                dt = dl.PupupDetails();

                if (dt != null && dt.Rows.Count > 0)
                {
                    comm.Status = "1";
                    comm.Message = "Details Fetch Successfull";
                    comm.Payload = dt;
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Data Not Found";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }

            return comm;
        }
        [Route("PopupActive")]
        [HttpPost]
        public CommonResponse ActivePupup(string Id)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    string userId = checkvalidate_user();

                    if (userId != "" && userId != string.Empty && userId != null)
                    {
                        if (dl.UpdatePupup(Convert.ToInt32(Id), "Active",""))
                        {
                            comm.Status = "1";
                            comm.Message = "Popup Activated Successfull";
                            comm.Payload = "Active";
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Popup Activation Failed";
                        }

                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Access Denied";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Id Must Be Required";
                }
                
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }

            return comm;
        }
        [Route("PopupInActive")]
        [HttpPost]
        public CommonResponse InActivePupup(string Id)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    string userId = checkvalidate_user();

                    if (userId != "" && userId != string.Empty && userId != null)
                    {
                        if (dl.UpdatePupup(Convert.ToInt32(Id), "InActive", ""))
                        {
                            comm.Status = "1";
                            comm.Message = "Popup Deactivated Successfull";
                            comm.Payload = "Deactive";
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Popup Deactivation Failed";
                        }

                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Access Denied";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Id Must Be Required";
                }

            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }

            return comm;
        }
        [Route("PopupDelete")]
        [HttpPost]
        public CommonResponse DeletePupup(string Id)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    string userId = checkvalidate_user();

                    if (userId != "" && userId != string.Empty && userId != null)
                    {
                        if (dl.UpdatePupup(Convert.ToInt32(Id), "Delete", ""))
                        {
                            comm.Status = "1";
                            comm.Message = "Popup Deleted Successfull";
                            comm.Payload = "Deleted";
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Popup Deletion Failed";
                        }

                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Access Denied";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Id Must Be Required";
                }

            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }

            return comm;
        }
        [Route("PopupAdd")]
        [HttpPost]
        public CommonResponse AddPupup(ModImg obj)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                if (!string.IsNullOrEmpty(obj.Img))
                {
                    string userId = checkvalidate_user();
                    if (userId != "" && userId != string.Empty && userId!=null)
                    {
                        string str = SaveImage(obj.Img);
                        obj.Img = str;
                        if (dl.UpdatePupup(0, "Add", obj.Img))
                        {
                            
                            comm.Status = "1";
                            comm.Message = "Popup Upload Successfully";
                            comm.Payload = obj.Img;
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Failed Popup Upload";
                        }

                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Access Denied";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Image Must Be Required";
                }

            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }

            return comm;
        }
        [Route("CheckUserDetails")]
        [HttpGet]
        public CommonResponse UserDetails()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.UserDetails(userId);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }

            return comm;
        }
        [Route("UpdateUserDetails")]
        [HttpPost]
        public CommonResponse UpdateUserDetails(ModRegistration reg)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                if((reg.UserImg != null || reg.UserImg != string.Empty))
                {
                    string userId = checkvalidate_user();
                    if (userId != "" && userId != string.Empty && userId != null)
                    {
                        reg.UserID=userId;
                        string str = "";
                        if (reg.UserImg.Length > 100)
                        {
                            str = SaveImage(reg.UserImg);
                            reg.UserImg = str;
                        }
                        else
                        {
                            DataTable dt = new DataTable();
                            dt = dl.UserDetails(userId);
                            reg.UserImg = dt.Rows[0]["UserImg"].ToString();
                        }
                        if (dl.UpdateProfile(reg))
                        {
                            DataTable dt = dl.UserDetails(userId);
                            comm.Status = "1";
                            comm.Message = "Details Update Successfull";
                            comm.Payload = dt;
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Details Not Update";
                        }

                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Access Denied";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "All Fields are required ";
                }
                
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }

            return comm;
        }
        [Route("PackageDetails")]
        [HttpGet]
        public CommonResponse GetPackageDetails()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "ShowMyPackageDetails");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Sendopt")]
        [HttpGet]
        public CommonResponse ValidateProfile()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();
                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.UserDetails(userId);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        Random rd = new Random();
                        string otp = rd.Next(100000, 999999).ToString();
                        string name = dt.Rows[0]["Name"].ToString();
                        string MobileNo = dt.Rows[0]["MobileNo"].ToString();
                        System.Net.WebClient client = new System.Net.WebClient();
                        string msgtxt = "Dear " + Convert.ToString(name) + " your OTP for transaction is " + otp + ". For more Details Visit www.nvshoppe.com NvShop";
                        string baseurl = "http://142.132.157.45/http-api.php?username=NvShop&password=654321&senderid=NVSHPE&route=12&number=" + Convert.ToString(MobileNo) + "&message=" + msgtxt + "&templateid=1707169995731479084";
                        //msn1.Text = baseurl;
                        try
                        {
                            
                            Stream data = client.OpenRead(baseurl);
                            StreamReader reader = new StreamReader(data);
                            string s = reader.ReadToEnd();
                            data.Close();
                            reader.Close();
                            modOtp mod = new modOtp();
                            //string encotp = HashPassword(otp);
                            var textBytes = Encoding.UTF8.GetBytes(otp);
                            var str=Convert.ToBase64String(textBytes);
                            mod.otp = str;
                            mod.mobile = MobileNo;
                            comm.Status = "1";
                            comm.Message = "Otp Send Successfully";
                            comm.Payload = mod;
                        }
                        catch (Exception ex)
                        {
                            comm.Status = "-1";
                            comm.Message = "Error sending OTP: " + ex.Message;
                            comm.Payload = "";
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }

            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        //public static string HashPassword(string password)
        //{
        //    using (var sha256 = SHA256.Create())
        //    {
        //        // Compute hash from password
        //        byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

        //        // Convert byte array to a string representation
        //        StringBuilder builder = new StringBuilder();
        //        for (int i = 0; i < hashedBytes.Length; i++)
        //        {
        //            builder.Append(hashedBytes[i].ToString("x2"));
        //        }
        //        return builder.ToString();
        //    }
        //}
        [Route("Kycupdate")]
        [HttpPost]
        public CommonResponse UpdateKyc(ModKyc kyc) 
        { 
            CommonResponse comm = new CommonResponse();
            try
            {
                if((!string.IsNullOrEmpty(kyc.Name)) && (!string.IsNullOrEmpty(kyc.AccountNo)) && (!string.IsNullOrEmpty(kyc.AccountType)) && (!string.IsNullOrEmpty(kyc.BankName)) && (!string.IsNullOrEmpty(kyc.Branch)) && (!string.IsNullOrEmpty(kyc.Ifsc)) && (!string.IsNullOrEmpty(kyc.PancardNo)) && (!string.IsNullOrEmpty(kyc.AadharNo))  && (!string.IsNullOrEmpty(kyc.Email)) && (!string.IsNullOrEmpty(kyc.MobileNo)) && (!string.IsNullOrEmpty(kyc.Nominee)) && (!string.IsNullOrEmpty(kyc.Relation))  && (!string.IsNullOrEmpty(kyc.District)) && (!string.IsNullOrEmpty(kyc.State)) && (!string.IsNullOrEmpty(kyc.Country)) && (!string.IsNullOrEmpty(kyc.PinCode)))
                {
                    string userId = checkvalidate_user();

                    if (userId != "" && userId != string.Empty && userId != null)
                    {
                        kyc.UserId = userId;
                        DataTable dt = new DataTable();
                        dt = dl.Get_WithDrawReport(userId, "kycanduserdetails");
                        if (kyc.PanDoc.Length > 100)
                        {
                            string str = SaveImage(kyc.PanDoc);
                            kyc.PanDoc = str;
                        }
                        else
                        {
                            kyc.PanDoc = dt.Rows[0]["PanDoc"].ToString();
                        }
                        if (kyc.UserImg.Length > 100)
                        {
                            string str1 = SaveImage(kyc.UserImg);
                            kyc.UserImg = str1;
                        }
                        else
                        {
                            kyc.UserImg = dt.Rows[0]["UserImg"].ToString();
                        }
                        int rsp = dl.UpdateKyc(kyc);
                        if (rsp > 0)
                        {
                            comm.Payload = kyc;
                            comm.Status = "1";
                            comm.Message = "Kyc Updated Successfull";
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Failed to Update Kyc";
                        }

                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Access Denied";
                    }
                    
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "All Field Are Required";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("AllMembers")]
        [HttpGet]
        public CommonResponse GetAllMembers(int limit,int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.TeamDetails(userId, "getallmember", lastIndex, limit);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("DownlineMember")]
        [HttpGet]
        public CommonResponse GetAllDownline(int limit, int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dl.Get_AllReports(limit, lastIndex, userId, "getDownlineDetails");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Directteam")]
        [HttpGet]
        public CommonResponse GetDirectTeam(int limit, int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();
                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                   dt= dl.Get_AllReports(limit, lastIndex, userId, "getrefrallist");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Levelwisedownline")]
        [HttpGet]
        public CommonResponse Levelwisedownline(int limit, int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();
                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt=dl.Get_AllReports(limit, lastIndex, userId, "Level_Details");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("LeftMembers")]
        [HttpGet]
        public CommonResponse GetLeftMembers(int limit, int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();
                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.TeamDetails(userId, "getallleftmember", lastIndex, limit);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("RightMembers")]
        [HttpGet]
        public CommonResponse GetRightMembers(int limit, int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();
                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.TeamDetails(userId, "getallrightmember", lastIndex, limit);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Referralincome")]
        [HttpGet]
        public CommonResponse GetReferralincome(int limit, int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();
                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_AllReports(limit, lastIndex,userId, "Direct_income");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Binaryincome")]
        [HttpGet]
        public CommonResponse GetBinaryincome(int limit, int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();
                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_AllReports(limit, lastIndex, userId, "binaryincome");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Repurchaseincome")]
        [HttpGet]
        public CommonResponse GetRepurchaseincome(int limit, int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();
                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_AllReports(limit, lastIndex, userId, "Repuchase_Income");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Venderincome")]
        [HttpGet]
        public CommonResponse GetVenderincome(int limit, int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();
                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_AllReports(limit, lastIndex, userId, "Vender_Income");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Carfundincome")]
        [HttpGet]
        public CommonResponse GetCarfundincome()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();
                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "CarFundIncome");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Housefundincome")]
        [HttpGet]
        public CommonResponse GetHousefundincome()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();
                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "HouseFundIncome");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Cptobonousincome")]
        [HttpGet]
        public CommonResponse GetCPTOBonusIncome()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();
                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "CPTOBonusIncome");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Pool1Income")]
        [HttpGet]
        public CommonResponse GetPool1Income()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "Pool1Income");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Pool2Income")]
        [HttpGet]
        public CommonResponse GetPool2Income()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "Pool2Income");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Pool3Income")]
        [HttpGet]
        public CommonResponse GetPool3Income()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "Pool3Income");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Pool4Income")]
        [HttpGet]
        public CommonResponse GetPool4Income()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "Pool4Income");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Pool5Income")]
        [HttpGet]
        public CommonResponse GetPool5Income()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();
                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "Pool5Income");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Pool6Income")]
        [HttpGet]
        public CommonResponse GetPool6Income()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "Pool6Income");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("royalityincome")]
        [HttpGet]
        public CommonResponse GetRoyalityIncome()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();
                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "royalityincome");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("RechargeIncome")]
        [HttpGet]
        public CommonResponse GetRechargeIncome(int limit, int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();
                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_AllReports(limit, lastIndex, userId, "Recharge_Income");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Totalincome")]
        [HttpGet]
        public CommonResponse GetTotalIncome(int limit, int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.GetTotalIncome(userId,"","", lastIndex, limit, "totalincome");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Totalincomebydate")]
        [HttpGet]
        public CommonResponse GetTotalIncomebyDate(int limit, int lastIndex,string fromdate, string todate)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.GetTotalIncome(userId, fromdate, todate, lastIndex, limit, "totalincomebydate");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        var myArray = Array.Empty<string>();
                        comm.Status = "1";
                        comm.Message = "Details Not Found";
                        comm.Payload = myArray;
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Repurchasewithincome")]
        [HttpGet]
        public CommonResponse GetRepurchaseWithIncome()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt=dl.Get_WithDrawReport(userId, "RepurchasewithIncome");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Rewardincome")]
        [HttpGet]
        public CommonResponse GetRewardIncome()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt=dl.Get_WithDrawReport(userId, "RewardIncome");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("RechargeReport")]
        [HttpGet]
        public CommonResponse GetRechargeRpt(int limit, int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_AllReports(limit, lastIndex, userId, "RechargeRpt");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("UsedCoupon")]
        [HttpGet]
        public CommonResponse GetUsedCouponList()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.UsedCouponList(userId);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("UnUsedCoupon")]
        [HttpGet]
        public CommonResponse GetUnUsedCouponList()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.UnUsedCouponList(userId);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        decimal str = Convert.ToDecimal(dt.Rows[0]["UnUsed"]);
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = str;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Epinrequest")]
        [HttpPost]
        public CommonResponse SendEpinRequest(ModEPin mod)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string UserId = checkvalidate_user();
                if (UserId!=null && UserId!=string.Empty && UserId != "")
                {
                    mod.RegNo= UserId;
                    if((mod.PMode != null || mod.PMode != string.Empty) && (mod.Amount != null || mod.Amount != string.Empty) && (mod.PinNo != null || mod.PinNo != string.Empty))
                    {
                        if (mod.PMode.ToLower() == "through online") 
                        {

                            mod.BankName = "";
                            mod.AccountHName = "";
                            mod.UtrNo = "";
                            mod.Receipt = "";
                            var guid = Guid.NewGuid().ToString();
                            string img=SaveReceiptAsImage(guid);
                            mod.Receipt = img;
                            mod.Status = "5";
                            int res = dl.EPinRequest(mod);
                            if (res > 0)
                            {
                                
                                comm.Status = "1";
                                comm.Payload = "";
                                comm.Message = "E-Pin Request Sent Successfull";
                            }
                            else
                            {
                                comm.Status = "0";
                                comm.Message = "E-Pin Request Failed";
                            }
                        }
                        else if(mod.PMode.ToLower() == "through bank")
                        {
                            if((!string.IsNullOrEmpty(mod.BankName)) && (!string.IsNullOrEmpty(mod.AccountHName)) && (!string.IsNullOrEmpty(mod.UtrNo) && (!string.IsNullOrEmpty(mod.Receipt))))
                            {
                                
                                mod.Status = "0";
                                mod.Receipt=SaveImage(mod.Receipt);
                                int res = dl.EPinRequest(mod);
                                if (res > 0)
                                {
                                    
                                    comm.Status = "1";
                                    comm.Payload = "";
                                    comm.Message = "E-Pin Request Sent Successfull";
                                }
                                else
                                {
                                    comm.Status = "0";
                                    comm.Message = "E-Pin Request Failed";
                                }
                            }
                            else
                            {
                                comm.Status = "0";
                                comm.Message = "Requires Bank Details";
                            }
                        }
                        else if(mod.PMode.ToLower() == "cash")
                        {
                            if(mod.Receipt != null || mod.Receipt != string.Empty)
                            {
                                mod.BankName = "";
                                mod.AccountHName = "";
                                mod.UtrNo = "";
                                mod.Status = "0";
                                mod.Receipt = SaveImage(mod.Receipt);
                                int res = dl.EPinRequest(mod);
                                if (res > 0)
                                {
                                    comm.Status = "1";
                                    comm.Payload = "";
                                    comm.Message = "E-Pin Request Sent Successfull";
                                }
                                else
                                {
                                    comm.Status = "0";
                                    comm.Message = "E-Pin Request Failed";
                                }
                            }
                            else
                            {
                                comm.Status = "0";
                                comm.Message = "Upload Receipt Required";
                            }
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Select Valid Payment Mode";
                        }
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Epinrequestreport")]
        [HttpGet]
        public CommonResponse GetEPinRequestList()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();
                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "epinrequestrpt");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("TransferPin")]
        [HttpPost]
        public CommonResponse TransferPinNo(List<TransferPin> tpList)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                foreach (var tp in tpList)
                {
                    if (!string.IsNullOrEmpty(tp.SendTo) && (!string.IsNullOrEmpty(tp.PinNo)))
                    {
                        string userId = checkvalidate_user();

                        if (userId != "" && userId != string.Empty && userId != null)
                        {
                            if (userId == tp.SendTo)
                            {
                                comm.Status = "0";
                                comm.Message = "Please Enter Different UserId";
                            }
                            else
                            {
                                if (dl.CheckUserId(tp.SendTo))
                                {
                                    if (dl.TransferPin(tp, userId))
                                    {
                                        comm.Status = "1";
                                        comm.Message = "Pin Transfer Successful";
                                        comm.Payload = tpList; 
                                    }
                                    else
                                    {
                                        comm.Status = "0";
                                        comm.Message = "Pin Transfer Failed";
                                    }
                                }
                                else
                                {
                                    comm.Status = "0";
                                    comm.Message = "Invalid RegNo";
                                }
                            }
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Access Denied";
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "All Fields are required";
                    }
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }

            return comm;
        }

        [Route("Pinfortransfer")]
        [HttpGet]
        public CommonResponse GetPinForTransfer()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "pinmstlist");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("PinReceived")]
        [HttpGet]
        public CommonResponse GetPinReceived(int limit, int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_AllReports(limit, lastIndex, userId, "totpin");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("PinTransferReport")]
        [HttpGet]
        public CommonResponse GetPinTransferReport(int limit, int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_AllReports(limit, lastIndex, userId, "PinTransferReport");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("UnusedPin")]
        [HttpGet]
        public CommonResponse GetUnusedPin(int limit, int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_AllReports(limit, lastIndex, userId, "unusedPin");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("UsedPin")]
        [HttpGet]
        public CommonResponse GetUsedPin(int limit, int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_AllReports(limit, lastIndex, userId, "usedPin");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Vendorlist")]
        [HttpGet]
        public CommonResponse GetVendorlist()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "vendorlistrpt");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("SupportTeam")]
        [HttpPost]
        public CommonResponse Supportmail(ModSupport sup)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                if (!string.IsNullOrEmpty(sup.sendto) && (!string.IsNullOrEmpty(sup.mailsendto)) && (!string.IsNullOrEmpty(sup.subject)) && (!string.IsNullOrEmpty(sup.message)))
                {
                    string userId = checkvalidate_user();

                    if (userId != "" && userId != string.Empty && userId != null)
                    {
                        int res = dl.ComposeMail(sup, userId);
                        if (res > 0)
                        {
                            comm.Status = "1";
                            comm.Message = "Ticket Generate Successfull";
                            comm.Payload = sup;
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Ticket Generate Failed";
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Access Denied";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "All Fields are required ";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }

            return comm;
        }
        [Route("Inbox")]
        [HttpGet]
        public CommonResponse GetInbox()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "inboxreportlist");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Outbox")]
        [HttpGet]
        public CommonResponse GetOutbox()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "outboxreportlist");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("DeleteInbox")]
        [HttpGet]
        public CommonResponse DeleteInbox(int id)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    if (id != 0)
                    {
                        int i = dl.DeleteInbox(userId, id);
                        if (i > 0)
                        {
                            comm.Status = "1";
                            comm.Message = "Deleted Successfull";
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Data Not Deleted";
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Id is Required for Deletion";
                    }
                    
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        public string SaveImage(string Receipt)
        {
            string picture = "";
            string base64String3 = Receipt;
            var ctx3 = HttpContext.Current;
            var root3 = ctx3.Server.MapPath("~/UploadImage");
            try
            {
                byte[] fileBytes3 = Convert.FromBase64String(base64String3);
                var timestamp = DateTime.Now.ToFileTime();
                string picname3 = dl.ExtractFileName(fileBytes3, dl.ExtractFileExtension(fileBytes3)) + ".jpg";
                picture = timestamp + picname3;
                string savefile3 = Path.Combine(root3, picture);
                File.WriteAllBytes(savefile3, fileBytes3);

            }
            catch (Exception ex)
            {

            }
            return picture;
        }
        public string SaveReceiptAsImage(string receipt)
        {
            string folderPath = HttpContext.Current.Server.MapPath("~/UploadImage/");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = $"{Guid.NewGuid()}.png";

            string filePath = Path.Combine(folderPath, fileName);

            using (Bitmap bmp = CreateImageFromReceipt(receipt))
            {
                bmp.Save(filePath, ImageFormat.Png);
            }
            return fileName;
        }
        private Bitmap CreateImageFromReceipt(string receipt)
        {
            Bitmap bmp = new Bitmap(400, 200); 
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.Clear(Color.White);

                using (Font font = new Font("Arial", 12))
                {
                    using (SolidBrush brush = new SolidBrush(Color.Black))
                    {
                        gfx.DrawString(receipt, font, brush, new PointF(10, 10));
                    }
                }
            }

            return bmp;
        }
        public string checkvalidate_user()
        {
            string check = "";
            string username = "";
            System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
            string userId = "";
            if (headers.Contains("X-Auth-Token"))
            {
                username = TokenManager.ValidateToken(userId = headers.GetValues("X-Auth-Token").First());
            }


            return username;
        }
        [Route("Matchingtree")]
        [HttpGet]
        public JObject MatchingTree(string userid = "")
        {
            string username = "";
            username = checkvalidate_user();
            CommonBinaryTree obj = new CommonBinaryTree();
            List<treemodel1> firstlist = new List<treemodel1>();

            if (username != "" && username != string.Empty && username != null)
            {
                string uid = userid;
                if (uid != "" && uid != null)
                {
                    username = uid;
                }
                Geneology objtr = new Geneology();
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();
                objtr = dl.GetDetailsByUID(username);

                dt1 = dl.getsponsorid("sp_tree1", username);
                if (dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        List<treemodel2> secondlist = new List<treemodel2>();
                        treemodel1 tm = new treemodel1();
                        tm.userid = Convert.ToString(dt1.Rows[i]["AppMstRegNo"]);
                        tm.name = Convert.ToString(dt1.Rows[i]["appmstfname"]);
                        tm.jointype = Convert.ToString(dt1.Rows[i]["JoinType"]);
                        tm.Sid = Convert.ToString(dt1.Rows[i]["SponsorID"]);
                        tm.Doj = Convert.ToString(dt1.Rows[i]["appmstdoj"]);
                        tm.TotalLeftId = Convert.ToString(dt1.Rows[i]["totalleftid"]);
                        tm.TotalRightId = Convert.ToString(dt1.Rows[i]["totalrightid"]);
                        tm.ActiveLeft = Convert.ToString(dt1.Rows[i]["Left_Active"]);
                        tm.ActiveRight = Convert.ToString(dt1.Rows[i]["Right_Active"]);
                        tm.CarryLeft = Convert.ToString(dt1.Rows[i]["carryleft"]);
                        tm.CarryRight = Convert.ToString(dt1.Rows[i]["carryright"]);
                        tm.LeftTotal = Convert.ToString(dt1.Rows[i]["AppMstLeftTotal"]);
                        tm.RightTotal = Convert.ToString(dt1.Rows[i]["AppMstRightTotal"]);
                        tm.Position = Convert.ToString(dt1.Rows[i]["AppMstLeftRight"]);
                        tm.Payload = secondlist;
                        firstlist.Add(tm);
                        dt2 = dl.getsponsorid("sp_tree2", tm.userid);
                        if (dt2.Rows.Count > 0)
                        {
                            for (int j = 0; j < dt2.Rows.Count; j++)
                            {
                                List<treemodel3> thirdlist = new List<treemodel3>();
                                treemodel2 tm2 = new treemodel2();
                                tm2.userid = Convert.ToString(dt2.Rows[j]["AppMstRegNo"]); ;
                                tm2.name = Convert.ToString(dt2.Rows[j]["appmstfname"]);
                                tm2.jointype = Convert.ToString(dt2.Rows[j]["JoinType"]);
                                tm2.Sid = Convert.ToString(dt2.Rows[j]["SponsorID"]);
                                tm2.LeftTotal = Convert.ToString(dt2.Rows[j]["AppMstLeftTotal"]);
                                tm2.RightTotal = Convert.ToString(dt2.Rows[j]["AppMstRightTotal"]);
                                tm2.Doj = Convert.ToString(dt2.Rows[j]["appmstdoj"]);
                                tm2.TotalLeftId = Convert.ToString(dt2.Rows[j]["totalleftid"]);
                                tm2.TotalRightId = Convert.ToString(dt2.Rows[j]["totalrightid"]);
                                tm2.CarryLeft = Convert.ToString(dt2.Rows[j]["carryleft"]);
                                tm2.CarryRight = Convert.ToString(dt2.Rows[j]["carryright"]);
                                tm2.ActiveLeft = Convert.ToString(dt2.Rows[j]["Left_Active"]);
                                tm2.ActiveRight = Convert.ToString(dt2.Rows[j]["Right_Active"]);
                                tm2.Position = Convert.ToString(dt2.Rows[j]["AppMstLeftRight"]);
                                tm2.Payload = thirdlist;
                                secondlist.Add(tm2);
                                dt3 = dl.getsponsorid("sp_tree3", tm2.userid);
                                if (dt3.Rows.Count > 0)
                                {
                                    for (int k = 0; k < dt3.Rows.Count; k++)
                                    {
                                        treemodel3 tm3 = new treemodel3();
                                        tm3.userid = Convert.ToString(dt3.Rows[k]["AppMstRegNo"]); ;
                                        tm3.name = Convert.ToString(dt3.Rows[k]["appmstfname"]);
                                        tm3.jointype = Convert.ToString(dt3.Rows[k]["JoinType"]);
                                        tm3.Sid = Convert.ToString(dt3.Rows[k]["SponsorID"]);
                                        tm3.LeftTotal = Convert.ToString(dt3.Rows[k]["AppMstLeftTotal"]);
                                        tm3.RightTotal = Convert.ToString(dt3.Rows[k]["AppMstRightTotal"]);
                                        tm3.Doj = Convert.ToString(dt3.Rows[k]["appmstdoj"]);
                                        tm3.TotalLeftId = Convert.ToString(dt3.Rows[k]["totalleftid"]);
                                        tm3.TotalRightId = Convert.ToString(dt3.Rows[k]["totalrightid"]);
                                        tm3.CarryLeft = Convert.ToString(dt3.Rows[k]["carryleft"]);
                                        tm3.CarryRight = Convert.ToString(dt3.Rows[k]["carryright"]);
                                        tm3.ActiveLeft = Convert.ToString(dt3.Rows[k]["Left_Active"]);
                                        tm3.ActiveRight = Convert.ToString(dt3.Rows[k]["Right_Active"]);
                                        tm3.Position = Convert.ToString(dt3.Rows[k]["AppMstLeftRight"]);
                                        thirdlist.Add(tm3);
                                    }
                                }
                            }
                        }
                    }
                }

                if (obj != null)
                {
                    obj.Status = "1";
                    obj.userid = username;
                    obj.name = objtr.Name;
                    obj.jointype = objtr.JoinType;
                    obj.Sid = objtr.SponsorID;
                    //obj.Position = objtr.Position;
                    obj.LeftTotal = objtr.NOL;
                    obj.RightTotal = objtr.NOR;
                    obj.Doj = objtr.DOJ;
                    obj.TotalLeftId = objtr.BusinessVolLeft;
                    obj.TotalRightId = objtr.BusinessVolRight;
                    obj.CarryLeft = objtr.CarryForwardLeft;
                    obj.CarryRight = objtr.CarryForwardRight;
                    obj.ActiveLeft = objtr.MatchingLeft;
                    obj.ActiveRight = objtr.MatchingRight;
                    obj.Payload = firstlist;
                    obj.Message = "Data Found..";

                }
                else
                {
                    obj.Status = "0";
                    obj.Message = "Data Not Found..";
                }
            }
            else
            {
                obj.Message = "Access Denied";
            }

            return JObject.Parse(JsonConvert.SerializeObject(obj, Formatting.None));
        }
        [Route("pooltree1")]
        [HttpGet]
        public JObject DirectTree(string userid1 = "")
        {
            string username = "";
            username = checkvalidate_user();
            CommonStringTree obj = new CommonStringTree();
            List<treemodel> firstlist = new List<treemodel>();

            if (username != "" && username != string.Empty && username != null)
            {
                string uid = userid1;
                if (uid != "" && uid != null)
                {
                    username = uid;
                }
                treemodelll objtr = new treemodelll();
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();
                objtr = dl.getname(username);

                dt1 = dl.getsponsorid("usp_pooltreeone", username);
                if (dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        List<treemodelll> secondlist = new List<treemodelll>();
                        treemodel tm = new treemodel();
                        tm.userid = Convert.ToString(dt1.Rows[i]["RegNo"]);
                        tm.name = Convert.ToString(dt1.Rows[i]["name"]);
                        tm.jointype = Convert.ToString(dt1.Rows[i]["JoinType"]);
                        dt2 = dl.getsponsorid("usp_pooltreeone", tm.userid);
                        if (dt2.Rows.Count > 0)
                        {
                            for (int j = 0; j < dt2.Rows.Count; j++)
                            {
                                List<treemodellllll> thirdlist = new List<treemodellllll>();
                                treemodelll tm2 = new treemodelll();
                                tm2.userid = Convert.ToString(dt2.Rows[j]["RegNo"]);
                                tm2.name = Convert.ToString(dt2.Rows[j]["name"]);
                                tm2.jointype = Convert.ToString(dt2.Rows[j]["JoinType"]);
                                tm2.Payload = thirdlist;
                                secondlist.Add(tm2);
                                dt3 = dl.getsponsorid("usp_pooltreeone", Convert.ToString(dt2.Rows[j]["RegNo"]));
                                if (dt3.Rows.Count > 0)
                                {
                                    for (int k = 0; k < dt3.Rows.Count; k++)
                                    {
                                        treemodellllll tm3 = new treemodellllll();
                                        tm3.userid = Convert.ToString(dt3.Rows[k]["RegNo"]); ;
                                        tm3.name = Convert.ToString(dt3.Rows[k]["name"]);
                                        tm3.jointype = Convert.ToString(dt3.Rows[k]["JoinType"]);
                                        thirdlist.Add(tm3);
                                    }
                                }
                                //tm2.Payload = thirdlist;
                                
                            }

                        }
                        tm.Payload = secondlist;
                        firstlist.Add(tm);
                    }
                }

                if (obj != null)
                {
                    obj.Status = "1";
                    obj.userid = username;
                    obj.name = objtr.name;
                    obj.jointype = objtr.jointype;
                    obj.Payload = firstlist;
                    obj.Message = "Data Found..";

                }
                else
                {
                    obj.Status = "0";
                    obj.Message = "Data Not Found..";
                }
            }
            else
            {
                obj.Message = "Access Denied";
            }

            return JObject.Parse(JsonConvert.SerializeObject(obj, Formatting.None));
        }
        [Route("pooltree2")]
        [HttpGet]
        public JObject DirectTree2(string userid1 = "")
        {
            string username = "";
            username = checkvalidate_user();
            CommonStringTree obj = new CommonStringTree();
            List<treemodel> firstlist = new List<treemodel>();

            if (username != "" && username != string.Empty && username != null)
            {
                string uid = userid1;
                if (uid != "" && uid != null)
                {
                    username = uid;
                }
                treemodelll objtr = new treemodelll();
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();
                objtr = dl.getname2(username);

                dt1 = dl.getsponsorid("usp_pooltreetwo", username);
                if (dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        List<treemodelll> secondlist = new List<treemodelll>();
                        treemodel tm = new treemodel();
                        tm.userid = Convert.ToString(dt1.Rows[i]["RegNo"]);
                        tm.name = Convert.ToString(dt1.Rows[i]["name"]);
                        tm.jointype = Convert.ToString(dt1.Rows[i]["JoinType"]);


                        dt2 = dl.getsponsorid("usp_pooltreetwo", Convert.ToString(dt1.Rows[i]["RegNo"]));
                        if (dt2.Rows.Count > 0)
                        {
                            for (int j = 0; j < dt2.Rows.Count; j++)
                            {
                                List<treemodellllll> thirdlist = new List<treemodellllll>();
                                treemodelll tm2 = new treemodelll();
                                tm2.userid = Convert.ToString(dt2.Rows[j]["RegNo"]);
                                tm2.name = Convert.ToString(dt2.Rows[j]["name"]);
                                tm2.jointype = Convert.ToString(dt2.Rows[j]["JoinType"]);
                                tm2.Payload = thirdlist;
                                secondlist.Add(tm2);
                                dt3 = dl.getsponsorid("usp_pooltreetwo", Convert.ToString(dt2.Rows[j]["RegNo"]));
                                if (dt3.Rows.Count > 0)
                                {
                                    for (int k = 0; k < dt3.Rows.Count; k++)
                                    {
                                        treemodellllll tm3 = new treemodellllll();
                                        tm3.userid = Convert.ToString(dt3.Rows[k]["RegNo"]); ;
                                        tm3.name = Convert.ToString(dt3.Rows[k]["name"]);
                                        tm3.jointype = Convert.ToString(dt3.Rows[k]["JoinType"]);
                                        thirdlist.Add(tm3);
                                    }
                                }
                                //tm2.Payload = thirdlist;
                                //secondlist.Add(tm2);
                            }

                        }
                        tm.Payload = secondlist;
                        firstlist.Add(tm);
                    }
                }

                if (obj != null)
                {
                    obj.Status = "1";
                    obj.userid = username;
                    obj.name = objtr.name;
                    obj.jointype=objtr.jointype;

                    obj.Payload = firstlist;
                    obj.Message = "Data Found..";

                }
                else
                {
                    obj.Status = "0";
                    obj.Message = "Data Not Found..";
                }
            }
            else
            {
                obj.Message = "Access Denied";
            }

            return JObject.Parse(JsonConvert.SerializeObject(obj, Formatting.None));
        }
        [Route("poolonelist")]
        [HttpGet]
        public CommonResponse GetPoolonelist()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.GetPoolOneLists(userId);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Orderdetails")]
        [HttpGet]
        public CommonResponse GetRepurchaseReport()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_RepurchaseReport(userId, "GetOrderByRegno"); ;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("Offlinepurchase")]
        [HttpGet]
        public CommonResponse GetOfflinepurchase()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_OfflinePurchase(userId); ;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("WalletTransfer")]
        [HttpGet]
        public CommonResponse GetWalletTransfer()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "Wallet_Transfer");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("WalletReceive")]
        [HttpGet]
        public CommonResponse GetWallet_Receive()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "Wallet_Receive");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("TransferWallet")]
        [HttpPost]
        public CommonResponse Wallettransfer(ModWalletTransf wlt) 
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string UserId = checkvalidate_user();
                if (UserId != null && UserId != string.Empty)
                {
                    if ((wlt.WorkingWallet != 0 || wlt.WorkingWallet != null) && (wlt.PurchaseWallet != 0 || wlt.PurchaseWallet != null) && (wlt.Amount != 0 || wlt.Amount != null) && (wlt.Remark != "" || wlt.Remark != null))
                    {
                        if (wlt.Amount >= 5)
                        {
                            if (wlt.Amount > wlt.WorkingWallet)
                            {
                                comm.Status = "0";
                                comm.Message = "Your transaction could not completed, Please check your e-Wallet Balance";
                                return comm;
                            }
                            else if(wlt.Amount > 0)
                            {
                                string mobile = dl.GetMobileNo(UserId);
                                Random rd = new Random();
                                string otp = rd.Next(100000, 999999).ToString();
                                if (!string.IsNullOrEmpty(mobile))
                                {
                                    System.Net.WebClient client = new System.Net.WebClient();
                                    string msgtxt = "Your Transaction OTP is " + otp + "www.nvshoppe.com NvShop" + "";
                                    string baseurl = "http://138.201.239.130/http-api.php?username=NvShop&password=654321&senderid=NvShop&route=12&number=" + mobile + "&message=" + msgtxt + "&templateid=1207162124447893717";

                                    try
                                    {
                                        Stream data = client.OpenRead(baseurl);
                                        StreamReader reader = new StreamReader(data);
                                        string s = reader.ReadToEnd();
                                        data.Close();
                                        reader.Close();
                                    }
                                    catch(Exception ex)
                                    {
                                        comm.Status = "0";
                                        comm.Message = "Something went wrong, When Sending Otp...!";
                                    }
                                }
                                else
                                {
                                    comm.Status = "0";
                                    comm.Message = "Mobile No Not Found...!";
                                }
                            }
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Amount is Greatter Than or Equal to Rs 5...!";
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Required All Field...!";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            
            return comm;
        }
        [Route("ConfirmWalletTransfer")]
        [HttpPost]
        public CommonResponse ConfirmPayment(ModConfTransfer mod)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                if(!string.IsNullOrEmpty(mod.RegNo) && !string.IsNullOrEmpty(mod.Password))
                {
                    if (dl.CheckUserPassword(mod.RegNo, mod.Password))
                    {
                        if (!string.IsNullOrEmpty(mod.RegNo) && !string.IsNullOrEmpty(mod.Amount) && !string.IsNullOrEmpty(mod.Remark))
                        {
                            if (dl.TransferWallet(mod.RegNo, mod.Amount, mod.Remark))
                            {
                                comm.Status = "1";
                                comm.Message = "Sucessfull Payment";
                            }
                            else
                            {
                                comm.Status = "0";
                                comm.Message = "Payment Failed";
                            }
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Required All Field...!";
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Invalid RegNo or Password...!";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "RegNo and Password Field Required...!";
                }
                
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("FileUpload")]
        [HttpPost]
        public CommonResponse Post(ModFile obj)
        {
            CommonResponse comm=new CommonResponse();
            string UserId = checkvalidate_user();
            if (UserId != null && UserId != string.Empty)
            {
                if(!string.IsNullOrEmpty(obj.Action)) 
                {
                    if(obj.Action.ToLower() == "add")
                    {
                        if (!string.IsNullOrEmpty(obj.File))
                        {
                            string str = SaveFile(obj.File);
                            if (dl.SetBusiness(0, str, "Add"))
                            {
                                comm.Status = "1";
                                comm.Payload = str;
                                comm.Message = "File Uploaded Successfull";
                            }
                            else
                            {
                                comm.Status = "0";
                                comm.Message = "Failed To Upload";
                            }
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "File Required";
                        }
                    }
                    else if (obj.Action.ToLower() == "active")
                    {
                        if (!string.IsNullOrEmpty(obj.Id))
                        {
                            if (dl.SetBusiness(Convert.ToInt32(obj.Id), "", "Active"))
                            {
                                comm.Status = "1";
                                comm.Message = "File Active Successfull";
                            }
                            else
                            {
                                comm.Status = "0";
                                comm.Message = "Failed To Active";
                            }
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Id Field Required";
                        }
                    }
                    else if (obj.Action.ToLower() == "inactive")
                    {
                        if (!string.IsNullOrEmpty(obj.Id))
                        {
                            if (dl.SetBusiness(Convert.ToInt32(obj.Id), "", "InActive"))
                            {
                                comm.Status = "1";
                                comm.Message = "File InActive Successfull";
                            }
                            else
                            {
                                comm.Status = "0";
                                comm.Message = "Failed To InActive";
                            }
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Id Field Required";
                        }
                    }
                    else if (obj.Action.ToLower() == "delete")
                    {
                        if (!string.IsNullOrEmpty(obj.Id))
                        {
                            if (dl.SetBusiness(Convert.ToInt32(obj.Id), "", "Delete"))
                            {
                                comm.Status = "1";
                                comm.Message = "File Deleted Successfull";
                            }
                            else
                            {
                                comm.Status = "0";
                                comm.Message = "Failed To Delete";
                            }
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Id Field Required";
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Action Will be Required";
                    }
                }
                else
                {

                }
                
            }
            else
            {
                comm.Status = "0";
                comm.Message = "Access Denied";
            }
            return comm;
        }
        public string SaveFile(string Receipt)
        {
            string picture = "";
            string base64String3 = Receipt;
            var ctx3 = HttpContext.Current;
            var root3 = ctx3.Server.MapPath("~/UploadFile");
            try
            {
                byte[] fileBytes3 = Convert.FromBase64String(base64String3);
                var timestamp = DateTime.Now.ToFileTime();
                string picname3 = dl.ExtractFileName(fileBytes3, dl.ExtractFileExtension(fileBytes3)) + ".pdf";
                picture = timestamp + picname3;
                string savefile3 = Path.Combine(root3, picture);
                File.WriteAllBytes(savefile3, fileBytes3);

            }
            catch (Exception ex)
            {

            }
            return picture;
        }
        [Route("Businessdetails")]
        [HttpGet]
        public CommonResponse GetBusiness()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.GetBusinessDetails();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("RepurchaseInctransfer")]
        [HttpPost]
        public CommonResponse TransferRepurVendor(ModVendorTransf wlt)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string UserId = checkvalidate_user();
                if (UserId != null && UserId != string.Empty)
                {
                    if ((wlt.VendorWallet != 0 || wlt.VendorWallet != null) && (wlt.RepurchaseWallet != 0 || wlt.RepurchaseWallet != null) && (wlt.Amount != 0 || wlt.Amount != null) && (wlt.Remark != "" || wlt.Remark != null))
                    {
                        if (wlt.Amount >= 5)
                        {
                            if (wlt.Amount > wlt.VendorWallet)
                            {
                                comm.Status = "0";
                                comm.Message = "Your transaction could not completed, Please check your e-Wallet Balance";
                                return comm;
                            }
                            else if (wlt.Amount > 0)
                            {
                                string mobile = dl.GetMobileNo(UserId);
                                Random rd = new Random();
                                string otp = rd.Next(100000, 999999).ToString();
                                if (!string.IsNullOrEmpty(mobile))
                                {
                                    System.Net.WebClient client = new System.Net.WebClient();
                                    string msgtxt = "Your Transaction OTP is " + otp + "www.nvshoppe.com NvShop" + "";
                                    string baseurl = "http://138.201.239.130/http-api.php?username=NvShop&password=654321&senderid=NvShop&route=12&number=" + mobile + "&message=" + msgtxt + "&templateid=1207162124447893717";

                                    try
                                    {
                                        Stream data = client.OpenRead(baseurl);
                                        StreamReader reader = new StreamReader(data);
                                        string s = reader.ReadToEnd();
                                        data.Close();
                                        reader.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        comm.Status = "0";
                                        comm.Message = "Something went wrong, When Sending Otp...!";
                                    }
                                }
                                else
                                {
                                    comm.Status = "0";
                                    comm.Message = "Mobile No Not Found...!";
                                }
                            }
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Amount is Greatter Than or Equal to Rs 5...!";
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Required All Field...!";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }

            return comm;
        }
        [Route("ConfirmRepurchaseIncTransfer")]
        [HttpPost]
        public CommonResponse ConfirmVendorTransfer(ModConfTransfer mod)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                if (!string.IsNullOrEmpty(mod.RegNo) && !string.IsNullOrEmpty(mod.Password))
                {
                    if (dl.CheckUserPassword(mod.RegNo, mod.Password))
                    {
                        if (!string.IsNullOrEmpty(mod.RegNo) && !string.IsNullOrEmpty(mod.Amount) && !string.IsNullOrEmpty(mod.Type))
                        {
                            if (dl.TransferVendorWallet(mod.RegNo, mod.Amount, mod.Type))
                            {
                                comm.Status = "1";
                                comm.Message = "Sucessfull Payment";
                            }
                            else
                            {
                                comm.Status = "0";
                                comm.Message = "Payment Failed";
                            }
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Required All Field...!";
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Invalid RegNo or Password...!";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "RegNo and Password Field Required...!";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("getteambydate")]
        [HttpGet]
        public CommonResponse GetAllMembersByDate(int limit, int lastIndex, string fromdate, string todate,string type)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    if(!string.IsNullOrEmpty(fromdate) && !string.IsNullOrEmpty(todate) && !string.IsNullOrEmpty(type))
                    {
                        DataTable dt = new DataTable();
                        if (type == "all")
                        {
                            dt = dl.TeamDetailsByDate(userId, "getallmemberbydate", lastIndex, limit, fromdate, todate);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                comm.Status = "1";
                                comm.Message = "Details Fetch Successfull";
                                comm.Payload = dt;
                            }
                            else
                            {
                                var myArray = Array.Empty<string>();
                                comm.Status = "1";
                                comm.Message = "Details Not Found";
                                comm.Payload = myArray;
                            }
                        }
                        else if (type == "left")
                        {
                            dt = dl.TeamDetailsByDate(userId, "getallleftmemberbydate", lastIndex, limit, fromdate, todate);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                comm.Status = "1";
                                comm.Message = "Details Fetch Successfull";
                                comm.Payload = dt;
                            }
                            else
                            {
                                var myArray = Array.Empty<string>();
                                comm.Status = "1";
                                comm.Message = "Details Not Found";
                                comm.Payload = myArray;
                            }
                        }
                        else if(type == "right")
                        {
                            dt = dl.TeamDetailsByDate(userId, "getallrightmemberbydate", lastIndex, limit, fromdate, todate);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                comm.Status = "1";
                                comm.Message = "Details Fetch Successfull";
                                comm.Payload = dt;
                            }
                            else
                            {
                                var myArray = Array.Empty<string>();
                                comm.Status = "1";
                                comm.Message = "Details Not Found";
                                comm.Payload = myArray;
                            }
                        }
                        else if (type == "direct")
                        {
                            dt = dl.Get_TeamsByDate(limit, lastIndex, userId, "getrefrallistbydate", fromdate, todate);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                comm.Status = "1";
                                comm.Message = "Details Fetch Successfull";
                                comm.Payload = dt;
                            }
                            else
                            {
                                var myArray = Array.Empty<string>();
                                comm.Status = "1";
                                comm.Message = "Details Not Found";
                                comm.Payload = myArray;
                            }
                        }
                        else if (type == "downline")
                        {
                            dt = dl.Get_TeamsByDate(limit, lastIndex, userId, "Level_Detailsbydate", fromdate, todate);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                comm.Status = "1";
                                comm.Message = "Details Fetch Successfull";
                                comm.Payload = dt;
                            }
                            else
                            {
                                var myArray = Array.Empty<string>();
                                comm.Status = "1";
                                comm.Message = "Details Not Found";
                                comm.Payload = myArray;
                            }
                        }
                        else
                        {
                            var myArray = Array.Empty<string>();
                            comm.Status = "0";
                            comm.Message = "Invalid Type Name";
                            comm.Payload = myArray;
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "All fields are required";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("getteambylevel")]
        [HttpGet]
        public CommonResponse GetTeamByLevel(int limit, int lastIndex, int levelno)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    if (levelno!=0 && levelno>0)
                    {
                        DataTable dt = new DataTable();
                        dt = dl.Get_TeamsByLevel(limit, lastIndex, userId, "Level_Detailsbylevelno", levelno);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            comm.Status = "1";
                            comm.Message = "Details Fetch Successfull";
                            comm.Payload = dt;
                        }
                        else
                        {
                            var myArray = Array.Empty<string>();
                            comm.Status = "1";
                            comm.Message = "Details Not Found";
                            comm.Payload = myArray;
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "All fields are required";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("kycdetails")]
        [HttpGet]
        public CommonResponse GetKycDetails()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();
                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "kycanduserdetails");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("withdrawreport")]
        [HttpGet]
        public CommonResponse Getwithdrawreport()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.Get_WithDrawReport(userId, "withdrawreport");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("purchasewalletreport")]
        [HttpGet]
        public CommonResponse Getpurchasewalletreport(int limit, int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.GetPurchaseWalletReport(userId, lastIndex, limit);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("repurchasewalletreport")]
        [HttpGet]
        public CommonResponse GetRepurchasewalletreport(int limit, int lastIndex)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.GetRePurchaseWalletReport(userId, lastIndex, limit);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }

                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("productlist")]
        [HttpGet]
        public CommonResponse GetSelectedProduct()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.SelectProduct(userId);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("productrequestrpt")]
        [HttpGet]
        public CommonResponse GetOrderProductList()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataTable dt = new DataTable();
                    dt = dl.ProductData(userId);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        comm.Payload = dt;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("productrequest")]
        [HttpPost]
        public CommonResponse productrequest(ModProductRequest obj)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    if(!string.IsNullOrEmpty(obj.ProductName) && !string.IsNullOrEmpty(obj.Address))
                    {
                        if (!dl.CheckProductDeliverByAdmin(userId))
                        {
                            if (dl.ProductRequest(userId, obj))
                            {
                                comm.Status = "1";
                                comm.Message = "Request Send Successful";
                                comm.Payload = obj;
                            }
                            else
                            {
                                comm.Status = "0";
                                comm.Message = "Request Not Send";
                            }
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Request Already Send.";
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "All Field Are Required";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }

            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("freetopup")]
        [HttpPost]
        public CommonResponse freetopup(ModFreeTopup tp)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                if (!string.IsNullOrEmpty(tp.regno) && (!string.IsNullOrEmpty(tp.pinno)))
                {
                    string userId = checkvalidate_user();

                    if (userId != "" && userId != string.Empty && userId != null)
                    {
                        DataTable dt = dl.UserDetails(tp.regno);
                        if (dt.Rows.Count>0)
                        {
                            
                            string email = dt.Rows[0]["email"].ToString();
                            string name = dt.Rows[0]["Name"].ToString();
                            if (dl.CheckTopup(tp.regno))
                            {
                                comm.Status = "0";
                                comm.Message = "Regno Already Activated";
                            }
                            else
                            {
                                if (dl.FreeTopUp(tp))
                                {

                                    SendTopupInfo(tp.regno, email, name);
                                    comm.Status = "1";
                                    comm.Message = "Regno TopUp Successfully";
                                    comm.Payload = tp;
                                }
                                else
                                {
                                    comm.Status = "0";
                                    comm.Message = "TopUp Failed";
                                }
                            }
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Invalid RegNo";
                        }

                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Access Denied";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "All Fields are required ";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }

            return comm;
        }
        public void SendTopupInfo(string RegNo, string email, string name)
        {
            string strCompany = "N V SHOPPE";
            string strCompanyDomain = "nvshoppe.com";
            string strFromEmail = "noreply@nvshoppe.com";
            string strFromEmailPassword = "121212qaz";
            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            string msg = string.Empty;
            try
            {
                MailAddress fromAddress = new MailAddress(strFromEmail);
                message.From = fromAddress;
                message.To.Add(email);
                message.Subject = strCompany + " : TopUp Information";
                message.IsBodyHtml = true;

                string _mailBodyHTML = @"<div bgcolor='#f0f0f0' style='background-color:#f0f0f0;margin:10px 0;padding:10px 0;width:100%'>
                                    <span ><font color='#888888'></font></span><span class='HOEnZb'><font color='#888888'></font></span>
                                    <table style='border-spacing:0' width='100%' height='100%' cellspacing='0' cellpadding='0' border='0' bgcolor='#f0f0f0'>
                                    <tbody><tr>
                                    <td style='border-collapse:collapse' valign='top' bgcolor='#f0f0f0' align='center'>
                                    <span ><font color='#888888'></font></span>
                                    <span><font color='#888888'> </font></span>
                                    <table style='border-bottom:1px solid #e0e0e0;border-spacing:0;color:#434343;font-family:Helvetica,Verdana,sans-serif;max-width:600px;overflow:hidden' cellspacing='0' cellpadding='0' border='0' bgcolor='#ffffff'>
                                    <tbody><tr>
                                    <td style='border-collapse:collapse;border-left:1px solid #e0e0e0'>
                                    <div style='background-color:#0e4595;border:1px solid #042f6d;border-radius:2px;height:5px'></div>
                                    </td>
                                    </tr>
                                    <tr>
                                    <td  style='border-bottom:1px solid #efefef;border-collapse:collapse;border-left:1px solid #e0e0e0;max-width:600px;padding:20px 0;text-align:center' align='center'>
                                    <a href='{compDomain}' style='color:#0e4595;text-decoration:underline;word-break:break-word;word-wrap:break-word' target='_blank' data-saferedirecturl=''>
                                    <img src='{logoSrc}' alt='{compName}' style='max-width:80%' >
                                    </a>
                                    </td>
                                    </tr>
                                    <tr>
                                    <td  style='background:#ffffff;border-collapse:collapse;border-left:1px solid #e0e0e0;padding-left:30px;padding-right:30px' bgcolor='#ffffff'>
                                    <br>
                                    <p style='color:#434343;font-size:14px;line-height:1.5;margin:0 0 25px 0'>
                                    Hi {userName},
                                    </p>
                                    <p style='color:#434343;font-size:14px;line-height:1.5;margin:0 0 25px 0'>
                                    A registration request is initiated at {compDomain}. If this was not you, please ensure the security
                                    of your email account and contact support immediately. <br/><br/> 
                                    Please use below account information.
                                    </p>
                                    <p style='color:#434343;font-size:14px;line-height:1.5;margin:0 0 25px 0'>
                                    Your Login ID: <b>{userLoginID}<b/>
                                    </p>
                                    <p style='color:#434343;font-size:14px;line-height:1.5;margin:0 0 25px 0'>
                                    Your Password: <b>{ppassword}<b/>
                                    </p>
                                    
                                    <p style='color:#434343;font-size:14px;line-height:1.5;margin:0 0 25px 0'>
                                    Best Regards, <br>
                                    <a href='{compDomain}' style='color:#0e4595;text-decoration:underline;word-break:break-word;word-wrap:break-word' target='_blank' data-saferedirecturl=''>{compName}</a>
                                    </p>
                                    </td>
                                    </tr>
                                    <tr>
                                    <td style='border-collapse:collapse;border-left:1px solid #e0e0e0'>
                                    <span ><font color='#888888'>
                                    </font></span><span ><font color='#888888'>
                                    </font></span><table class='m_2264787707347477873m_-8276338735438771956m_-8110430706966875067footer' style='border-spacing:0;border-top:1px solid #efefef;width:100%' width='100%' height='100%' cellspacing='0' cellpadding='0' border='0' bgcolor='#fbfbfb'>
                                    <tbody><tr>
                                    <td 
                                    style='border-collapse:collapse;border-left:none;color:#434343;font-size:12px;line-height:20px;padding:5px;text-align:center;vertical-align:middle;width:60%' valign='middle'>
                                    Follow us:
                                    <a href='' class='' style='color:#00aced;font-weight:bold;margin:0 10px;text-decoration:none;word-break:break-word;word-wrap:break-word' target='_blank' data-saferedirecturl=''>
                                    Twitter
                                    </a>
                                    <a href='' class='' style='color:#6d83bc;font-weight:bold;margin:0 10px;text-decoration:none;word-break:break-word;word-wrap:break-word' target='_blank' data-saferedirecturl=''>
                                    Facebook
                                    </a>
                                    </td>
                                    <td class='m_2264787707347477873m_-8276338735438771956m_-8110430706966875067help-me' style='border-collapse:collapse;color:#434343;font-size:12px;padding:5px;text-align:center;vertical-align:middle;width:40%' valign='middle'>
                                    This is an auto generated mail, Do not revert over this mail.<br><br>
                                    <a href='' style='color:#0e4595;text-decoration:underline;word-break:break-word;word-wrap:break-word' target='_blank' data-saferedirecturl=''></a><span ><font color='#888888'>
                                    </font></span></td></tr></tbody></table><span class='HOEnZb'><font color='#888888'>
                                    </font></span></td></tr></tbody></table><span class='HOEnZb'><font color='#888888'>
                                    </font></span></td></tr></tbody></table><div><div >
                                    </div>";
                _mailBodyHTML = _mailBodyHTML.Replace("{userName}", name);
                _mailBodyHTML = _mailBodyHTML.Replace("{portalURL}", "http://nvshoppe.com");
                _mailBodyHTML = _mailBodyHTML.Replace("{userLoginID}", RegNo);
                _mailBodyHTML = _mailBodyHTML.Replace("{logoSrc}", "http://nvshoppe.com/images/logo-dark.png");
                _mailBodyHTML = _mailBodyHTML.Replace("{compName}", strCompany);
                _mailBodyHTML = _mailBodyHTML.Replace("{compDomain}", strCompanyDomain);

                message.Body = _mailBodyHTML;

                smtpClient.Host = "dedrelay.secureserver.net";   //-- Donot change.
                smtpClient.Port = 25; //--- Donot change
                smtpClient.EnableSsl = false;//--- Donot change
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(strFromEmail, strFromEmailPassword);

                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
        }
        [Route("poolachived")]
        [HttpGet]
        public CommonResponse Getpoolachived()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                string userId = checkvalidate_user();

                if (userId != "" && userId != string.Empty && userId != null)
                {
                    DataSet ds = dl.GetAllDataBalancenewdash(userId);
                    DataTable dt = ds.Tables[1];
                    DataTable dt2= ds.Tables[3];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        comm.Status = "1";
                        comm.Message = "Details Fetch Successfull";
                        var payload = new { business = dt2 , poolachived = dt };
                        comm.Payload = payload;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Details Not Found";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Access Denied";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        public void SendSMS(string uid, string name, string password, string mobile)
        {
            try
            {
                WebClient client = new WebClient();
                string msgtxt = "Dear " + name + " Your account is successfully created. Your User Id is : " + uid + "  and Password is :" + password + " www.nvshoppe.com";
                string baseurl = "http://www.kit19.com/ComposeSMS.aspx?username=Jwala347677&password=53424&sender=TXTSMS&to=" + mobile + "&message=" + msgtxt + "&priority=1&dnd=1&unicode=0";
                Stream data = client.OpenRead(baseurl);
                StreamReader reader = new StreamReader(data);
                string s = reader.ReadToEnd();
                data.Close();
                reader.Close();
            }
            catch
            {

            }

        }
        public void SendRegistrationInfo(string RegNo, string email, string pass, string name)
        {
            string strCompany = "N V SHOPPE";
            string strCompanyDomain = "nvshoppe.com";
            string strFromEmail = "noreply@nvshoppe.com";
            string strFromEmailPassword = "121212qaz";
            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            string msg = string.Empty;
            try
            {
                MailAddress fromAddress = new MailAddress(strFromEmail);
                message.From = fromAddress;
                message.To.Add(email);
                message.Subject = strCompany + " : Registration Information";
                message.IsBodyHtml = true;

                string _mailBodyHTML = @"<div bgcolor='#f0f0f0' style='background-color:#f0f0f0;margin:10px 0;padding:10px 0;width:100%'>
                                    <span ><font color='#888888'></font></span><span class='HOEnZb'><font color='#888888'></font></span>
                                    <table style='border-spacing:0' width='100%' height='100%' cellspacing='0' cellpadding='0' border='0' bgcolor='#f0f0f0'>
                                    <tbody><tr>
                                    <td style='border-collapse:collapse' valign='top' bgcolor='#f0f0f0' align='center'>
                                    <span ><font color='#888888'></font></span>
                                    <span><font color='#888888'> </font></span>
                                    <table style='border-bottom:1px solid #e0e0e0;border-spacing:0;color:#434343;font-family:Helvetica,Verdana,sans-serif;max-width:600px;overflow:hidden' cellspacing='0' cellpadding='0' border='0' bgcolor='#ffffff'>
                                    <tbody><tr>
                                    <td style='border-collapse:collapse;border-left:1px solid #e0e0e0'>
                                    <div style='background-color:#0e4595;border:1px solid #042f6d;border-radius:2px;height:5px'></div>
                                    </td>
                                    </tr>
                                    <tr>
                                    <td  style='border-bottom:1px solid #efefef;border-collapse:collapse;border-left:1px solid #e0e0e0;max-width:600px;padding:20px 0;text-align:center' align='center'>
                                    <a href='{compDomain}' style='color:#0e4595;text-decoration:underline;word-break:break-word;word-wrap:break-word' target='_blank' data-saferedirecturl=''>
                                    <img src='{logoSrc}' alt='{compName}' style='max-width:80%' >
                                    </a>
                                    </td>
                                    </tr>
                                    <tr>
                                    <td  style='background:#ffffff;border-collapse:collapse;border-left:1px solid #e0e0e0;padding-left:30px;padding-right:30px' bgcolor='#ffffff'>
                                    <br>
                                    <p style='color:#434343;font-size:14px;line-height:1.5;margin:0 0 25px 0'>
                                    Hi {userName},
                                    </p>
                                    <p style='color:#434343;font-size:14px;line-height:1.5;margin:0 0 25px 0'>
                                    A registration request is initiated at {compDomain}. If this was not you, please ensure the security
                                    of your email account and contact support immediately. <br/><br/> 
                                    Please use below account information.
                                    </p>
                                    <p style='color:#434343;font-size:14px;line-height:1.5;margin:0 0 25px 0'>
                                    Your Login ID: <b>{userLoginID}<b/>
                                    </p>
                                    <p style='color:#434343;font-size:14px;line-height:1.5;margin:0 0 25px 0'>
                                    Your Password: <b>{ppassword}<b/>
                                    </p>
                                    
                                    <p style='color:#434343;font-size:14px;line-height:1.5;margin:0 0 25px 0'>
                                    Best Regards, <br>
                                    <a href='{compDomain}' style='color:#0e4595;text-decoration:underline;word-break:break-word;word-wrap:break-word' target='_blank' data-saferedirecturl=''>{compName}</a>
                                    </p>
                                    </td>
                                    </tr>
                                    <tr>
                                    <td style='border-collapse:collapse;border-left:1px solid #e0e0e0'>
                                    <span ><font color='#888888'>
                                    </font></span><span ><font color='#888888'>
                                    </font></span><table class='m_2264787707347477873m_-8276338735438771956m_-8110430706966875067footer' style='border-spacing:0;border-top:1px solid #efefef;width:100%' width='100%' height='100%' cellspacing='0' cellpadding='0' border='0' bgcolor='#fbfbfb'>
                                    <tbody><tr>
                                    <td 
                                    style='border-collapse:collapse;border-left:none;color:#434343;font-size:12px;line-height:20px;padding:5px;text-align:center;vertical-align:middle;width:60%' valign='middle'>
                                    Follow us:
                                    <a href='' class='' style='color:#00aced;font-weight:bold;margin:0 10px;text-decoration:none;word-break:break-word;word-wrap:break-word' target='_blank' data-saferedirecturl=''>
                                    Twitter
                                    </a>
                                    <a href='' class='' style='color:#6d83bc;font-weight:bold;margin:0 10px;text-decoration:none;word-break:break-word;word-wrap:break-word' target='_blank' data-saferedirecturl=''>
                                    Facebook
                                    </a>
                                    </td>
                                    <td class='m_2264787707347477873m_-8276338735438771956m_-8110430706966875067help-me' style='border-collapse:collapse;color:#434343;font-size:12px;padding:5px;text-align:center;vertical-align:middle;width:40%' valign='middle'>
                                    This is an auto generated mail, Do not revert over this mail.<br><br>
                                    <a href='' style='color:#0e4595;text-decoration:underline;word-break:break-word;word-wrap:break-word' target='_blank' data-saferedirecturl=''></a><span ><font color='#888888'>
                                    </font></span></td></tr></tbody></table><span class='HOEnZb'><font color='#888888'>
                                    </font></span></td></tr></tbody></table><span class='HOEnZb'><font color='#888888'>
                                    </font></span></td></tr></tbody></table><div><div >
                                    </div>";
                _mailBodyHTML = _mailBodyHTML.Replace("{ppassword}", pass);
                _mailBodyHTML = _mailBodyHTML.Replace("{userName}", name);
                _mailBodyHTML = _mailBodyHTML.Replace("{portalURL}", "http://nvshoppe.com");
                _mailBodyHTML = _mailBodyHTML.Replace("{userLoginID}", RegNo);
                _mailBodyHTML = _mailBodyHTML.Replace("{logoSrc}", "http://nvshoppe.com/images/logo-dark.png");
                _mailBodyHTML = _mailBodyHTML.Replace("{compName}", strCompany);
                _mailBodyHTML = _mailBodyHTML.Replace("{compDomain}", strCompanyDomain);

                message.Body = _mailBodyHTML;

                smtpClient.Host = "dedrelay.secureserver.net";   //-- Donot change.
                smtpClient.Port = 25; //--- Donot change
                smtpClient.EnableSsl = false;//--- Donot change
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(strFromEmail, strFromEmailPassword);

                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
        }
        public void SendLoginSMS(string completeName, string appmstpassword, string mobile)
        {
            try
            {
                System.Net.WebClient client = new System.Net.WebClient();
                string msgtxt = "Dear " + completeName + " your login password is " + appmstpassword + ". www.nvshoppe.com NvShop";
                string baseurl = "http://142.132.157.45/http-api.php?username=NvShop&password=654321&senderid=NVSHPE&route=12&number=" + mobile + "&message=" + msgtxt + "&templateid=1707170133965490047";
                Stream data = client.OpenRead(baseurl);
                StreamReader reader = new StreamReader(data);
                string s = reader.ReadToEnd();
                data.Close();
                reader.Close();
            }
            catch
            {
            }
        }
        [Route("forgetpassword")]
        public CommonResponse ForgetPassword(string userid)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                if (userid != null)
                {
                    DataTable dt = new DataTable();
                    if (dl.CheckUserId(userid))
                    {
                        dt = dl.UserDetails(userid);
                        if (dt.Rows.Count > 0)
                        {
                            string name = dt.Rows[0]["name"].ToString();
                            string password = dt.Rows[0]["password"].ToString();
                            string mobile = dt.Rows[0]["MobileNo"].ToString();
                            SendLoginSMS(name, password, mobile);
                            string msgtxt = "Dear " + name + " your login password is " + password + ". www.nvshoppe.com NvShop";
                            comm.Status = "1";
                            comm.Message = "User Details Send Successfull on Your Mobile No";
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Details Not Found";
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Invalid UserId";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Userid Not Empty";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("binarytree")]
        [HttpGet]
        public CommonBinaryTree GetBinaryTree(string userid = "")
        {
            CommonBinaryTree comm = new CommonBinaryTree();
            string userId = checkvalidate_user();
            if (userId != "" && userId != string.Empty && userId != null)
            {
                string uid = userid;
                if (uid != "" && uid != null)
                {
                    userId = uid;
                }
                CommonBinaryTree comm1 = new CommonBinaryTree();
                comm1 = dl.GetFirstTree(userId);
                List<treemodel1> firstlist = new List<treemodel1>();

                DataTable dt1 = dl.getsponsorid("sp_tree1", userId);

                foreach (DataRow row1 in dt1.Rows)
                {
                    treemodel1 tm = new treemodel1();
                    List<treemodel2> secondlist = new List<treemodel2>();
                    PopulateTreeModel1(tm, row1);

                    DataTable dt2 = dl.getsponsorid("sp_tree2", tm.userid);

                    foreach (DataRow row2 in dt2.Rows)
                    {
                        treemodel2 tm2 = new treemodel2();
                        List<treemodel3> thirdlist = new List<treemodel3>();
                        PopulateTreeModel2(tm2, row2);

                        DataTable dt3 = dl.getsponsorid("sp_tree3", tm2.userid);
                        if (dt3.Rows.Count > 1)
                        {
                            foreach (DataRow row3 in dt3.Rows)
                            {
                                treemodel3 tm3 = new treemodel3();
                                PopulateTreeModel3(tm3, row3);
                                thirdlist.Add(tm3);
                            }
                        }
                        else
                        {

                        }

                        tm2.Payload = thirdlist;
                        secondlist.Add(tm2);
                    }

                    tm.Payload = secondlist;
                    firstlist.Add(tm);
                }

                if (comm != null)
                {
                    comm = comm1;
                    comm.Status="1";
                    comm.Payload = firstlist;
                    comm.Message = "Data Found..!";
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Data Not Found..!";
                }
            }
            else
            {
                comm.Status = "0";
                comm.Message = "Access Denied";
            }
            return comm;
        }
        private void PopulateTreeModel1(treemodel1 tm, DataRow row)
        {
            tm.userid = Convert.ToString(row["AppMstRegNo"]);
            tm.name = Convert.ToString(row["appmstfname"]);
            tm.jointype = Convert.ToString(row["JoinType"]);
            tm.Sid = Convert.ToString(row["SponsorID"]);
            tm.Doj = Convert.ToString(row["appmstdoj"]);
            tm.TotalLeftId = Convert.ToString(row["totalleftid"]);
            tm.TotalRightId = Convert.ToString(row["totalrightid"]);
            tm.ActiveLeft = Convert.ToString(row["Left_Active"]);
            tm.ActiveRight = Convert.ToString(row["Right_Active"]);
            tm.CarryLeft = Convert.ToString(row["carryleft"]);
            tm.CarryRight = Convert.ToString(row["carryright"]);
            tm.LeftTotal = Convert.ToString(row["AppMstLeftTotal"]);
            tm.RightTotal = Convert.ToString(row["AppMstRightTotal"]);
            tm.Position = Convert.ToString(row["AppMstLeftRight"]);
        }
        private void PopulateTreeModel2(treemodel2 tm2, DataRow row)
        {
            tm2.userid = Convert.ToString(row["AppMstRegNo"]);
            tm2.name = Convert.ToString(row["appmstfname"]);
            tm2.jointype = Convert.ToString(row["JoinType"]);
            tm2.Sid = Convert.ToString(row["SponsorID"]);
            tm2.LeftTotal = Convert.ToString(row["AppMstLeftTotal"]);
            tm2.RightTotal = Convert.ToString(row["AppMstRightTotal"]);
            tm2.Doj = Convert.ToString(row["appmstdoj"]);
            tm2.TotalLeftId = Convert.ToString(row["totalleftid"]);
            tm2.TotalRightId = Convert.ToString(row["totalrightid"]);
            tm2.CarryLeft = Convert.ToString(row["carryleft"]);
            tm2.CarryRight = Convert.ToString(row["carryright"]);
            tm2.ActiveLeft = Convert.ToString(row["Left_Active"]);
            tm2.ActiveRight = Convert.ToString(row["Right_Active"]);
            tm2.Position = Convert.ToString(row["AppMstLeftRight"]);
        }
        private void PopulateTreeModel3(treemodel3 tm3, DataRow row)
        {
            tm3.userid = Convert.ToString(row["AppMstRegNo"]);
            tm3.name = Convert.ToString(row["appmstfname"]);
            tm3.jointype = Convert.ToString(row["JoinType"]);
            tm3.Sid = Convert.ToString(row["SponsorID"]);
            tm3.LeftTotal = Convert.ToString(row["AppMstLeftTotal"]);
            tm3.RightTotal = Convert.ToString(row["AppMstRightTotal"]);
            tm3.Doj = Convert.ToString(row["appmstdoj"]);
            tm3.TotalLeftId = Convert.ToString(row["totalleftid"]);
            tm3.TotalRightId = Convert.ToString(row["totalrightid"]);
            tm3.CarryLeft = Convert.ToString(row["carryleft"]);
            tm3.CarryRight = Convert.ToString(row["carryright"]);
            tm3.ActiveLeft = Convert.ToString(row["Left_Active"]);
            tm3.ActiveRight = Convert.ToString(row["Right_Active"]);
            tm3.Position = Convert.ToString(row["AppMstLeftRight"]);
        }







        [Route("venderlistdashboard")]
        [HttpGet]
        public CommonResponse Getvenderlistdashboard()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                DataTable dt = new DataTable();
                dt = dl.GetVenderList();
                if (dt != null && dt.Rows.Count > 0)
                {
                    comm.Status = "1";
                    comm.Message = "Details Fetch Successfull";
                    comm.Payload = dt;
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Details Not Found";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("franchiselistdashboard")]
        [HttpGet]
        public CommonResponse Getfranchiselistdashboard()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                DataTable dt = new DataTable();
                dt = dl.GetFranchise();
                if (dt != null && dt.Rows.Count > 0)
                {
                    comm.Status = "1";
                    comm.Message = "Details Fetch Successfull";
                    comm.Payload = dt;
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Details Not Found";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("statelist")]
        [HttpGet]
        public CommonResponse GetStateList()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                DataTable dt = new DataTable();
                dt = dl.GetStateList();
                if (dt != null && dt.Rows.Count > 0)
                {
                    comm.Status = "1";
                    comm.Message = "Details Fetch Successfull";
                    comm.Payload = dt;
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Details Not Found";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("citylist")]
        [HttpGet]
        public CommonResponse GetCityList(int stateid)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                DataTable dt = new DataTable();
                dt = dl.GetCityList(stateid);
                if (dt != null && dt.Rows.Count > 0)
                {
                    comm.Status = "1";
                    comm.Message = "Details Fetch Successfull";
                    comm.Payload = dt;
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Details Not Found";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("franchiseenquiry")]
        [HttpPost]
        public CommonResponse FranchiesEnquiry(ModFranchises obj)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                if (!string.IsNullOrEmpty(obj.SponsorId) && (!string.IsNullOrEmpty(obj.Name)) && (!string.IsNullOrEmpty(obj.Address)) && (!string.IsNullOrEmpty(obj.State)) && (!string.IsNullOrEmpty(obj.City)) && (!string.IsNullOrEmpty(obj.Email)) && (!string.IsNullOrEmpty(obj.MobileNo)) && (!string.IsNullOrEmpty(obj.Investment)) && (!string.IsNullOrEmpty(obj.FloorArea)) && (!string.IsNullOrEmpty(obj.CommProperty)) && (!string.IsNullOrEmpty(obj.Details)))
                {
                    string name = dl.GetSponsor(obj.SponsorId);
                    if(name != null && name!="")
                    {
                        if (dl.AddFranchaises(obj))
                        {
                            comm.Status = "1";
                            comm.Message = "Good Job!, Enquiry Saved Successfully!";
                            comm.Payload = obj;
                        }
                        else
                        {
                            comm.Status = "0";
                            comm.Message = "Opps!, Enquiry Not Saved!";
                        }
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Invalid Sponsor Id...!";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "All Fields are required ";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }

            return comm;
        }
        [Route("marquelist")]
        [HttpGet]
        public CommonResponse GetMarqueList()
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                dt = dl.GetMarqueFranchies();
                dt1 = dl.GetMarquePoolAchived();
                if (dt != null && dt.Rows.Count > 0)
                {
                    var payload = new { newfranchise = dt, bonousachiver = dt1 };
                    comm.Status = "1";
                    comm.Message = "Details Fetch Successfull";
                    comm.Payload = payload;
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Details Not Found";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("contact")]
        [HttpPost]
        public CommonResponse AddContact(ModContact obj)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                if (!string.IsNullOrEmpty(obj.Name) && (!string.IsNullOrEmpty(obj.Email)) && (!string.IsNullOrEmpty(obj.Mobile)) && (!string.IsNullOrEmpty(obj.Message)))
                {
                    if (dl.AddContact(obj))
                    {
                        comm.Status = "1";
                        comm.Message = "Submit Successfull";
                        comm.Payload = obj;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Opps!, Failed to Send Details!";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "All Fields are required ";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }

            return comm;
        }
        [Route("acheverlist")]
        [HttpGet]
        public CommonResponse GetAcheverList()
        {
            CommonResponse comm = new CommonResponse();
            DataTable dt = new DataTable();
            try
            {
                dt = dl.GetAcheverList();
                if (dt != null && dt.Rows.Count > 0)
                {
                    comm.Status = "1";
                    comm.Message = "Details Fetch Successfull";
                    comm.Payload = dt;
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Details Not Found";
                    comm.Payload = dt;
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Payload = dt;
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("acheverlistbyrank")]
        [HttpGet]
        public CommonResponse GetAcheverList(string rank)
        {
            CommonResponse comm = new CommonResponse();
            DataTable dt = new DataTable();
            try
            {
                dt = dl.GetAcheverListByRank(rank);
                if (dt != null && dt.Rows.Count > 0)
                {
                    comm.Status = "1";
                    comm.Message = "Details Fetch Successfull";
                    comm.Payload = dt;
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Details Not Found";
                    comm.Payload = dt;
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Payload = dt;
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("awardgallery")]
        [HttpGet]
        public CommonResponse GetAwardGallery()
        {
            DataTable dt = new DataTable();
            CommonResponse comm = new CommonResponse();
            try
            {
                dt = dl.GetAwardGallery();
                if (dt != null && dt.Rows.Count > 0)
                {
                    comm.Status = "1";
                    comm.Message = "Details Fetch Successfull";
                    comm.Payload = dt;
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Details Not Found";
                    comm.Payload = dt;
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
                comm.Payload = dt;
            }
            return comm;
        }
        [Route("slider")]
        [HttpGet]
        public CommonResponse GetSlider()
        {
            CommonResponse comm = new CommonResponse();
            DataTable dt = new DataTable();
            try
            {
                dt = dl.GetSlider();
                if (dt != null && dt.Rows.Count > 0)
                {
                    comm.Status = "1";
                    comm.Message = "Details Fetch Successfull";
                    comm.Payload = dt;
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "Details Not Found";
                    comm.Payload = dt;
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Payload = dt;
                comm.Message = "Something went wrong, Please try again...!";
            }
            return comm;
        }
        [Route("recharge")]
        [HttpPost]
        public CommonResponse DoingRecharge(ModRecharge obj)
        {
            CommonResponse comm = new CommonResponse();
            try
            {
                if (!string.IsNullOrEmpty(obj.Mobile) && (!string.IsNullOrEmpty(obj.Name)) && (!string.IsNullOrEmpty(obj.IdNo)) && (!string.IsNullOrEmpty(obj.MailId)))
                {
                    if (dl.AddRecharge(obj))
                    {
                        comm.Status = "1";
                        comm.Message = "Recharge Successfull";
                        comm.Payload = obj;
                    }
                    else
                    {
                        comm.Status = "0";
                        comm.Message = "Opps!, Failed to Recharge!";
                    }
                }
                else
                {
                    comm.Status = "0";
                    comm.Message = "All Fields are required ";
                }
            }
            catch (Exception)
            {
                comm.Status = "0";
                comm.Message = "Something went wrong, Please try again...!";
            }

            return comm;
        }
    }
}
