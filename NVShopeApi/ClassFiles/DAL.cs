using NVShopeApi.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using MetaMinesAPI.ClassFiles;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Net.NetworkInformation;
using System.Xml.Linq;
using System.Text;
using System.IO;

namespace NVShopeApi.ClassFiles
{
    public class DAL:ClsConnection
    {
        DataUtility dut = new DataUtility();
        SqlConnection con = new SqlConnection(str);
        
        public DataTable getlogin(ModLogin obj)
        {

            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_Login", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmailAddress", obj.UserId);
                cmd.Parameters.AddWithValue("@Password", obj.Password);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            catch (Exception ex)
            {
                return null;
            }
            return dt;
        }
        public string GetUserName(string userid)
        {
            string username = "";
            //string qstr = "Select Appmstregno from Appmst where appmstregno='" + userid + "'";
            //SqlConnection con = new SqlConnection(str);
            SqlCommand com = new SqlCommand("SP_UserAuthenticateDetails", con);
            com.CommandType= CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Action", "GetRegNo");
            com.Parameters.AddWithValue("@UserId", userid);
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable t = new DataTable();
            da.Fill(t);
            if (t.Rows.Count > 0)
            {
                return username;
            }
            else
            {
                return null;
            }
        }
        public int GetAppMstId()
        {

            int iAppMstId;
            //iAppMstId = Convert.ToInt32(dut.SqlScaler("select Max(AppMstID) from AppMst"));
            SqlCommand cmd = new SqlCommand("SP_UserAuthenticateDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "GetMaxId");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable t = new DataTable();
            da.Fill(t);
            if (t.Rows.Count > 0)
            {
                iAppMstId = Convert.ToInt32(t.Rows[0][0].ToString());
                iAppMstId = iAppMstId + 1;
                return iAppMstId;
            }
            else
            {
                return 0;
            }
            
        }
        public int GetIdBySponsorId(ModRegistration reg)
        {
            int id;
            //id = Convert.ToInt32(dut.SqlScaler("select max(appmstid) from appmst where sponsorid= '" + reg.SponsorId + "'"));
            //return id;
            SqlCommand cmd = new SqlCommand("SP_UserAuthenticateDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "GetIdBySponsorid");
            cmd.Parameters.AddWithValue("@UserId", reg.SponsorId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable t = new DataTable();
            da.Fill(t);
            if (t.Rows.Count > 0)
            {
                id = Convert.ToInt32(t.Rows[0][0].ToString());
                return id;
            }
            else
            {
                return 0;
            }
        }
        public int GetPinNo(ModRegistration reg)
        {
            int PinNo;
            //PinNo = Convert.ToInt32(dut.SqlScaler("select count(pinno) from pinmst where paidappmstid=0 and pinno= '" + reg.PinNo + "'"));
            //return PinNo;
            SqlCommand cmd = new SqlCommand("SP_UserAuthenticateDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "GetPinNo");
            cmd.Parameters.AddWithValue("@UserId", reg.PinNo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable t = new DataTable();
            da.Fill(t);
            if (t.Rows.Count > 0)
            {
                PinNo = Convert.ToInt32(t.Rows[0][0].ToString());
                return PinNo;
            }
            else
            {
                return 0;
            }
        }
        public string GetAppMstRegNo(ModRegistration reg, int idd)
        {
            //string appmstregno;
            //appmstregno = Convert.ToString(dut.SqlScaler("select appmstregno from appmst where sponsorid= '" + reg.SponsorId + "' and appmstid>=" + idd + ""));
            SqlCommand cmd = new SqlCommand("SP_UserAuthenticateDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "GetNewRegNo");
            cmd.Parameters.AddWithValue("@UserId", reg.SponsorId);
            cmd.Parameters.AddWithValue("@Id", idd);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable t = new DataTable();
            da.Fill(t);
            if (t.Rows.Count > 0)
            {
                string regno = t.Rows[0][0].ToString();
                return regno;
            }
            else
            {
                return "";
            }
        }
        public bool GetSponsorId(ModRegistration reg)
        {
            try
            {
                //string qstr = "Select Appmstregno from Appmst where appmstregno='" + reg.SponsorId + "' ";
                //SqlCommand com = new SqlCommand(qstr, con);
                SqlCommand cmd = new SqlCommand("SP_UserAuthenticateDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetRegNo");
                cmd.Parameters.AddWithValue("@UserId", reg.SponsorId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable t = new DataTable();
                da.Fill(t);
                if (t.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            
        }
        public string GetSponsor(string sid)
        {
            string name = "";
            try
            {
                SqlCommand cmd = new SqlCommand("SP_UserAuthenticateDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetRegNoName");
                cmd.Parameters.AddWithValue("@UserId", sid);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable t = new DataTable();
                da.Fill(t);
                if (t.Rows.Count > 0)
                {
                    name = t.Rows[0]["AppMstFname"].ToString();
                }
                else
                {
                    name="";
                }
            }
            catch (Exception)
            {
                name = "";
            }
            return name;
        }
        public DataTable GetRegNo(ModRegistration reg)
        {
            try
            {
                //string qstr = "Select Appmstregno from Appmst where appmstregno='" + reg.UserID + "' ";
                SqlCommand com = new SqlCommand("SP_UserAuthenticateDetails", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Action", "GetRegNo");
                com.Parameters.AddWithValue("@UserId", reg.UserID);
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable t = new DataTable();
                da.Fill(t);
                return t;
            }
            catch (Exception)
            {
                return null; ;
            }

        }
        public bool GetEmail(ModRegistration reg)
        {
            try
            {
                //string qstr2 = "Select Email from Appmst where email='" + reg.Email + "' ";
                //SqlCommand cmd = new SqlCommand(qstr2, con);
                SqlCommand cmd = new SqlCommand("SP_UserAuthenticateDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetEmail");
                cmd.Parameters.AddWithValue("@UserId", reg.Email);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool CheckMobileExist(ModRegistration reg)
        {
            try
            {
                //string qstr2 = "Select Email from Appmst where AppMstMobile='" + reg.MobileNo + "' ";
                //SqlCommand cmd = new SqlCommand(qstr2, con);
                SqlCommand cmd = new SqlCommand("SP_UserAuthenticateDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetMobile");
                cmd.Parameters.AddWithValue("@UserId", reg.MobileNo);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool CheckPanNoExist(ModRegistration reg)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("SP_UserAuthenticateDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetPanNo");
                cmd.Parameters.AddWithValue("@UserId", reg.panno);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (string.IsNullOrEmpty(reg.panno))
                {
                    return false;
                }
                else if (dt.Rows.Count > 2)
                {
                    return true;
                }
                
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int CreateAccount(ModRegistration reg)
        {
            try
            {
                int iAppMstId = GetAppMstId();
                SqlCommand cmd = new SqlCommand("Proc_Insert_Data", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sponserid", reg.SponsorId);
                cmd.Parameters.AddWithValue("@AppMstLeftRight", reg.Position);
                cmd.Parameters.AddWithValue("@AppMstFName", reg.Name);
                cmd.Parameters.AddWithValue("@AppMstState", reg.State);
                cmd.Parameters.AddWithValue("@AppMstCity", reg.District);
                cmd.Parameters.AddWithValue("@AppMstAddress1", reg.Address);
                cmd.Parameters.AddWithValue("@AppMstZipCode", "");
                cmd.Parameters.AddWithValue("@AppMstPinCode", reg.PinCode);
                cmd.Parameters.AddWithValue("@AppMstPassword", reg.Password);
                cmd.Parameters.AddWithValue("@AppMstID", iAppMstId);
                cmd.Parameters.AddWithValue("@AppMstRegNo", reg.UserID);
                cmd.Parameters.AddWithValue("@email", reg.Email);
                cmd.Parameters.AddWithValue("@mobile", reg.MobileNo);
                cmd.Parameters.AddWithValue("@country", reg.Country);
                cmd.Parameters.AddWithValue("@P_Money", "");
                cmd.Parameters.AddWithValue("@Payee_Id", "");
                cmd.Parameters.AddWithValue("@Bitcoin", "");
                cmd.Parameters.AddWithValue("@FatherName", reg.FatherName);
                cmd.Parameters.AddWithValue("@Nominee", reg.Nominee);
                cmd.Parameters.AddWithValue("@Relation", reg.Relation);
                cmd.Parameters.AddWithValue("@pinno", reg.PinNo);
                cmd.Parameters.AddWithValue("@panno", reg.panno);
                cmd.CommandTimeout = 500;
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i == -1)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public string getpassword(string user, string actions)
        {

            SqlCommand cmd = new SqlCommand("Proc_UpdatePassword", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@appmstregno", user);
            cmd.Parameters.AddWithValue("@Action", actions);
            DataTable t1 = new DataTable();
            SqlDataAdapter dr = new SqlDataAdapter(cmd);
            dr.Fill(t1);
            if (t1.Rows.Count > 0)
            {
                return t1.Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }
        public int updatePassword(ModChangePassword objpsd, string userid,string action)
        {
            int st = 0;
            try
            {

                SqlCommand com1 = new SqlCommand("Proc_UpdatePassword", con);
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.AddWithValue("@appmstregno", userid);
                com1.Parameters.AddWithValue("@AppMstPassword", objpsd.NewPassword);
                com1.Parameters.AddWithValue("@Action", action);
                con.Open();
                com1.CommandTimeout = 50000;
                int i = com1.ExecuteNonQuery();
                con.Close();
                if (i > 0)
                {
                    st = 1;
                }
                else
                {
                    st = 0;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return st;
        }
        public DataTable getdashboard(string userid)
        {

            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_GetDashboardDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userid);
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                dr.Fill(dt);
            }
            catch (Exception ex)
            {
                return null; ;
            }
            return dt;
        }
        public DataTable getdashboard2(string userid)
        {

            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_GetSecondDashboard", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userid);
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                dr.Fill(dt);
            }
            catch (Exception ex)
            {
                return null; 
            }
            return dt;
        }
        public DataTable PupupDetails()
        {

            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_PopupDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", 0);
                cmd.Parameters.AddWithValue("@Action", "Search");
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                dr.Fill(dt);
            }
            catch (Exception ex)
            {
                return null;
            }
            return dt;
        }
        public bool UpdatePupup(int id, string Action,string Img)
        {

            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_PopupDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Img", Img);
                cmd.Parameters.AddWithValue("@Action", Action);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public DataTable UserDetails(string UserId)
        {

            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_UserDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                dr.Fill(dt);
                if(dt!=null && dt.Rows.Count>0) 
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool UpdateProfile(ModRegistration reg)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateEditProfile", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RegNo", reg.UserID);
                //cmd.Parameters.AddWithValue("@name", reg.Name);
                //cmd.Parameters.AddWithValue("@email", reg.Email);
                //cmd.Parameters.AddWithValue("@country", reg.Country);
                //cmd.Parameters.AddWithValue("@MobileNo", reg.MobileNo);
                //cmd.Parameters.AddWithValue("@address", reg.Address);
                //cmd.Parameters.AddWithValue("@district", reg.District);
                //cmd.Parameters.AddWithValue("@state", reg.State);
                //cmd.Parameters.AddWithValue("@pincode", reg.PinCode);
                //cmd.Parameters.AddWithValue("@Nominee", reg.Nominee);
                //cmd.Parameters.AddWithValue("@Relation", reg.Relation);
                cmd.Parameters.AddWithValue("@userImage", reg.UserImg);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public DataTable TeamDetails(string UserId,string flag,int lastindex,int limit)
        {

            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_GetTeamReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@flag", flag);
                cmd.Parameters.AddWithValue("@lastindex", lastindex);
                cmd.Parameters.AddWithValue("@limit", limit);
                cmd.CommandTimeout = 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                dr.Fill(dt);
                if(dt!=null && dt.Rows.Count>0) 
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public DataTable Get_WithDrawReport(string UserId, string flag)
        {

            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("GetWithDrawReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                //cmd.Parameters.AddWithValue("@bitcoinaddress", coinaddress);
                cmd.Parameters.AddWithValue("@flag", flag);
                cmd.CommandTimeout = 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }

            }
            catch (Exception ex)
            {

            }

            return t1;

        }
        public DataTable Get_AllReports(int limit, int lastIndex, string UserId, string flag)
        {

            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_AllReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@lastindex", lastIndex);
                cmd.Parameters.AddWithValue("@limit",limit);
                cmd.Parameters.AddWithValue("@flag", flag);
                cmd.CommandTimeout = 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }

            }
            catch (Exception ex)
            {

            }

            return t1;

        }
        public DataTable Get_TeamsByDate(int limit, int lastIndex, string UserId, string flag, string fromdate,string todate)
        {

            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_AllReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@lastindex", lastIndex);
                cmd.Parameters.AddWithValue("@limit", limit);
                cmd.Parameters.AddWithValue("@flag", flag);
                cmd.Parameters.AddWithValue("@fromdate", fromdate);
                cmd.Parameters.AddWithValue("@todate", todate);
                cmd.CommandTimeout = 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }

            }
            catch (Exception ex)
            {

            }

            return t1;

        }
        public DataTable Get_TeamsByLevel(int limit, int lastIndex, string UserId, string flag, int levelno)
        {

            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_AllReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@lastindex", lastIndex);
                cmd.Parameters.AddWithValue("@limit", limit);
                cmd.Parameters.AddWithValue("@flag", "Level_Detailsbylevelno");
                cmd.Parameters.AddWithValue("@levelno", levelno);
                cmd.CommandTimeout = 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }

            }
            catch (Exception ex)
            {

            }

            return t1;

        }
        public DataTable TeamDetailsByDate(string UserId, string flag, int lastindex, int limit,string fromdate,string todate)
        {

            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_GetTeamReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@flag", flag);
                cmd.Parameters.AddWithValue("@lastindex", lastindex);
                cmd.Parameters.AddWithValue("@limit", limit);
                cmd.Parameters.AddWithValue("@fromdate", fromdate);
                cmd.Parameters.AddWithValue("@todate", todate);
                cmd.CommandTimeout = 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                dr.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public DataTable ShowTotalIncome(string UserId)
        {
            try
            {
                SqlCommand com = new SqlCommand("Proc_Payout_Summery", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@regno", UserId);
                com.CommandTimeout = 1000;
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable t = new DataTable();
                da.Fill(t);
                if (t != null && t.Rows.Count > 0)
                {
                    return t;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        public DataTable GetTotalIncome(string UserId,string fromdate,string todate,int lastindex,int limit,string Action)
        {
            try
            {
                SqlCommand com = new SqlCommand("SP_GetTotalIncom", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@regno", UserId);
                com.Parameters.AddWithValue("@fromdate", fromdate);
                com.Parameters.AddWithValue("@todate", todate);
                com.Parameters.AddWithValue("@lastindex", lastindex);
                com.Parameters.AddWithValue("@limit", limit);
                com.Parameters.AddWithValue("@Action", Action);
                com.CommandTimeout = 1000;
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable t = new DataTable();
                da.Fill(t);
                if (t != null && t.Rows.Count > 0)
                {
                    return t;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        public int UpdateKyc(ModKyc kyc)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("SP_UpdateKyc", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", kyc.Email);
                cmd.Parameters.AddWithValue("@Account_name", kyc.Name);
                cmd.Parameters.AddWithValue("@AccountNo", kyc.AccountNo);
                cmd.Parameters.AddWithValue("@AccountType", kyc.AccountType);
                cmd.Parameters.AddWithValue("@bank", kyc.BankName);
                cmd.Parameters.AddWithValue("@branch", kyc.Branch);
                cmd.Parameters.AddWithValue("@Ifsc", kyc.Ifsc);
                cmd.Parameters.AddWithValue("@PancardNo", kyc.PancardNo);
                cmd.Parameters.AddWithValue("@PancardDoc", kyc.PanDoc);
                cmd.Parameters.AddWithValue("@Aadhar", kyc.AadharNo);


                cmd.Parameters.AddWithValue("@RegNo", kyc.UserId);
                cmd.Parameters.AddWithValue("@country", kyc.Country);
                cmd.Parameters.AddWithValue("@MobileNo", kyc.MobileNo);
                cmd.Parameters.AddWithValue("@district", kyc.District);
                cmd.Parameters.AddWithValue("@state", kyc.State);
                cmd.Parameters.AddWithValue("@pincode", kyc.PinCode);
                cmd.Parameters.AddWithValue("@Nominee", kyc.Nominee);
                cmd.Parameters.AddWithValue("@Relation", kyc.Relation);
                cmd.Parameters.AddWithValue("@userImage", kyc.UserImg);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i >0)
                {
                    return i;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable UsedCouponList(string UserId)
        {

            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_Used_Coupon", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                dr.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public DataTable UnUsedCouponList(string UserId)
        {

            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_UnUsed_Coupon", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                dr.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int EPinRequest(ModEPin mod)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_BankDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Regno", mod.RegNo);
                cmd.Parameters.AddWithValue("@Remark", mod.Remarks);
                cmd.Parameters.AddWithValue("@Receipt", mod.Receipt);
                cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(mod.Amount));
                cmd.Parameters.AddWithValue("@UTRNO", mod.UtrNo);
                cmd.Parameters.AddWithValue("@pMode", mod.PMode);
                cmd.Parameters.AddWithValue("@BankName", mod.BankName);
                cmd.Parameters.AddWithValue("@BranchName", mod.BranchName);
                cmd.Parameters.AddWithValue("@ClientName", mod.ClientName);
                cmd.Parameters.AddWithValue("@chequeno", mod.ChequeNo);
                cmd.Parameters.AddWithValue("@AccountHName", mod.AccountHName);
                cmd.Parameters.AddWithValue("@PaytmNo", mod.PaytmNo);
                cmd.Parameters.AddWithValue("@PinNo", Convert.ToInt32(mod.PinNo));
                cmd.Parameters.AddWithValue("@status", Convert.ToInt32(mod.Status));
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i > 0)
                {
                    return i;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public bool TransferPin(TransferPin pin, string UserId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("SP_TransferPin", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RegNo", UserId);
                cmd.Parameters.AddWithValue("@SendTo", pin.SendTo);
                cmd.Parameters.AddWithValue("@PinNo", pin.PinNo);
                con.Open();
                int i= cmd.ExecuteNonQuery();
                con.Close();
                if (i>0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public int ComposeMail(ModSupport mail, string UserId)
        {
            try
            {
                string ticket = "";
                Random rnd = new Random();
                string charPool = "0123456789abcdefghijklmnopqrstuvwxyz";
                StringBuilder rs = new StringBuilder();
                int a = 1;
                charPool = "0123456789";
                rs = new StringBuilder();
                while (a > 0)
                {
                    rs.Append(charPool[(int)(rnd.NextDouble() * charPool.Length)]);
                    if (rs.Length == 6)
                    {
                        ticket = rs.ToString();
                        break;
                    }
                }
                SqlCommand cmd = new SqlCommand("usp_ComposeMail", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", 1);
                cmd.Parameters.AddWithValue("@Subject", mail.subject);
                cmd.Parameters.AddWithValue("@Message", mail.message);
                cmd.Parameters.AddWithValue("@RegNo", UserId);
                cmd.Parameters.AddWithValue("@ToRegNo", mail.sendto);
                cmd.Parameters.AddWithValue("@Ticket", ticket);
                cmd.Parameters.AddWithValue("@sendto", mail.mailsendto);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i > 0)
                {
                    return i;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public int DeleteInbox(string UserId,int Id)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("delete from tbl_mail where id="+Id+"and RegNo='"+ UserId+"'", con);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i > 0)
                {
                    return i;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public bool CheckUserId(string uid)
        {
            try
            {
                //string qstr = "Select Appmstregno from Appmst where appmstregno='" + reg.SponsorId + "' ";
                //SqlCommand com = new SqlCommand(qstr, con);
                SqlCommand cmd = new SqlCommand("SP_UserAuthenticateDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetRegNo");
                cmd.Parameters.AddWithValue("@UserId", uid);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable t = new DataTable();
                da.Fill(t);
                if (t.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }
        public string ExtractFileName(byte[] fileBytes, string fileExtension)
        {
            // Generate a unique temporary file path
            string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + fileExtension);

            try
            {
                // Write the byte array content to the temporary file
                File.WriteAllBytes(tempFilePath, fileBytes);

                // Retrieve the file name from the temporary file's metadata
                string fileName = Path.GetFileName(tempFilePath);

                return fileName;
            }
            finally
            {
                // Delete the temporary file
                File.Delete(tempFilePath);
            }
        }
        public string ExtractFileExtension(byte[] fileBytes)
        {
            // Generate a unique temporary file path
            string tempFilePath = Path.GetTempFileName();

            try
            {
                // Write the byte array content to the temporary file
                File.WriteAllBytes(tempFilePath, fileBytes);

                // Get the file extension from the temporary file's extension
                string fileExtension = Path.GetExtension(tempFilePath);

                return fileExtension;
            }
            finally
            {
                // Delete the temporary file
                File.Delete(tempFilePath);
            }
        }
        public treemodelll getname(string userid)
        {
            treemodelll obj = new treemodelll();

            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("select p.regno,p.name, a.jointype from Tbl_Pool_one p left join appmst a on p.regno=a.appmstregno where p.regno='" + userid + "' order by p.id", con);
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                dr.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["jointype"] == null)
                    {
                        obj.jointype = "0.0";
                    }
                    else
                    {
                        obj.name = Convert.ToString(dt.Rows[0]["name"]);
                        obj.userid = Convert.ToString(dt.Rows[0]["regno"]);
                        obj.jointype = Convert.ToString(dt.Rows[0]["jointype"]);
                    }
                }
            }
            catch
            {
                //obj.name = "";
                //obj.jointype = "";
            }
            return obj;
        }
        public treemodelll getname2(string userid)
        {
            treemodelll obj = new treemodelll();

            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("select p.regno,p.name,a.jointype from Tbl_Pool_two p left join Appmst a on p.regno=a.appmstregno where p.regno='" + userid + "' order by p.id", con);
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                dr.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["jointype"] == null)
                    {
                        obj.jointype = "0.0";
                    }
                    else
                    {
                        obj.name = Convert.ToString(dt.Rows[0]["name"]);
                        obj.userid = Convert.ToString(dt.Rows[0]["regno"]);
                        obj.jointype = Convert.ToString(dt.Rows[0]["jointype"]);
                    }
                }
            }
            catch
            {
                //obj.name = "";
                //obj.jointype = "";
            }
            return obj;
        }
        public DataTable getsponsorid(string flag,string userid)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand(flag, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AppMstRegNo", userid);
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                dr.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable GetPoolOneLists(string UserId)
        {

            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("GetPoolNo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                dr.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public DataTable Get_RepurchaseReport(string UserId, string flag)
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_UserOrder", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RegNo", UserId);
                //cmd.Parameters.AddWithValue("@bitcoinaddress", coinaddress);
                cmd.Parameters.AddWithValue("@flag", flag);
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }

            }
            catch (Exception ex)
            {

            }

            return t1;

        }
        public DataTable Get_OfflinePurchase(string UserId)
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_Offline_Purchase", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }

            }
            catch (Exception ex)
            {

            }

            return t1;

        }
        public string GetMobileNo(string UserId)
        {
            try
            {
                //string qstr2 = "Select Email from Appmst where AppMstMobile='" + reg.MobileNo + "' ";
                //SqlCommand cmd = new SqlCommand(qstr2, con);
                SqlCommand cmd = new SqlCommand("SP_UserAuthenticateDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetMobileNo");
                cmd.Parameters.AddWithValue("@UserId", UserId);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string mobile = dt.Rows[0]["AppMstMobile"].ToString();
                    return mobile;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        public Geneology GetDetailsByUID(string UID)
        {
            Geneology geneology = new Geneology();
            if (UID != null)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(str))
                    {
                        using (SqlCommand cmd = new SqlCommand("USP_GetGeneologyDetailsByID", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@UID", UID);
                            con.Open();
                            SqlDataReader rdr = cmd.ExecuteReader();
                            if (rdr.HasRows)
                            {
                                while (rdr.Read())
                                {
                                    geneology.RegNo = rdr["AppMstRegNo"].ToString();
                                    geneology.Name = rdr["appmstfname"].ToString();
                                    geneology.JoinType = rdr["jointype"].ToString();
                                    geneology.SponsorID = rdr["SponsorID"].ToString();
                                    geneology.DOJ = rdr["appmstdoj"].ToString();
                                    geneology.BusinessVolLeft = rdr["totalleftid"].ToString();
                                    geneology.BusinessVolRight = rdr["totalrightid"].ToString();
                                    geneology.DirectRefLeft = "0";
                                    geneology.DirectRefRight = "0";
                                    //geneology.Per = "0";    ///rdr["per"].ToString();
                                    geneology.AccumulateLeft = "0";// rdr["AccumulateLeft"].ToString();
                                    geneology.AccumulateRight = "0";//  rdr["AccumulateRight"].ToString();
                                    geneology.MatchingLeft = rdr["left_active"].ToString();
                                    geneology.MatchingRight = rdr["right_active"].ToString();
                                    geneology.TWLeftID = rdr["TWLeftID"].ToString();
                                    geneology.TWRightID = rdr["TWRightID"].ToString();
                                    geneology.CarryForwardLeft = rdr["carryleft"].ToString();
                                    geneology.CarryForwardRight = rdr["carryright"].ToString();
                                    geneology.NOL = rdr["AppMstLeftTotal"].ToString();
                                    geneology.NOR = rdr["AppMstRightTotal"].ToString();
                                    geneology.JoinType = rdr["jointype"].ToString();
                                }
                            }
                            con.Close();
                        }
                    }
                }
                catch
                {
                }
            }
            return geneology;
        }
        public bool SetBusiness(int id, string file, string Action)
        {

            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("Sp_BusinessDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@ImageName", file);
                cmd.Parameters.AddWithValue("@Action", Action);
                cmd.CommandTimeout = 1000;
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public DataTable GetBusinessDetails()
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("Sp_BusinessDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetData");
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }
            }
            catch (Exception ex)
            {

            }

            return t1;

        }
        public bool TransferWallet(string UserId, string Amount, string Remark)
        {
            try
            {
                SqlCommand com = new SqlCommand("USP_Wallet_Transfer", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@regno", UserId);
                com.Parameters.AddWithValue("@Amount", Amount);
                com.Parameters.AddWithValue("@UpdateID", UserId);
                com.Parameters.AddWithValue("@Type", 2);
                com.Parameters.AddWithValue("@Remark", Remark);

                con.Open();
                com.CommandTimeout = 500;
                int i = (int)com.ExecuteNonQuery();
                con.Close();
                if (i > 0)
                {
                    string sql = "delete from TempBonus where RegNo='" + UserId + "' ";
                    com = new SqlCommand(sql, con);
                    con.Open();
                    com.ExecuteNonQuery();
                    con.Close();

                    string sql1 = "update Tbl_Ewallet set active=11 where RegNo='" + UserId + "' and active=9";
                    com = new SqlCommand(sql, con);
                    con.Open();
                    com.ExecuteNonQuery();
                    con.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool TransferVendorWallet(string UserId, string Amount, string type)
        {
            try
            {
                SqlCommand com = new SqlCommand("proc_TransferRepurVendInc", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@regno", UserId);
                com.Parameters.AddWithValue("@Amount", Amount);
                com.Parameters.AddWithValue("@UpdateID", UserId);
                com.Parameters.AddWithValue("@Type", type);
                con.Open();
                com.CommandTimeout = 500;
                int i = (int)com.ExecuteNonQuery();
                con.Close();
                if (i > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool CheckUserPassword(string UserId,string Password)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("select appmstregno from appmst where appmstregno='" + UserId + "' and appmstlogin='" + Password + "'", con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt=new DataTable();
                sda.Fill(dt);
                if(dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetPurchaseWalletReport(string UserId,int lastindex,int limit)
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_PurchaseWallet", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@regno", UserId);
                cmd.Parameters.AddWithValue("@lastindex", lastindex);
                cmd.Parameters.AddWithValue("@limit", limit);
                cmd.CommandTimeout = 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }
            }
            catch (Exception ex)
            {

            }
            return t1;

        }
        public DataTable GetRePurchaseWalletReport(string UserId, int lastindex, int limit)
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_RePurchaseIncomeList", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@regno", UserId);
                cmd.Parameters.AddWithValue("@lastindex", lastindex);
                cmd.Parameters.AddWithValue("@limit", limit);
                cmd.CommandTimeout= 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }
            }
            catch (Exception ex)
            {

            }
            return t1;

        }
        public DataTable SelectProduct(string UserId)
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("USP_SelectProByPackage", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Userid", UserId);
                cmd.CommandTimeout = 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }
            }
            catch (Exception ex)
            {

            }

            return t1;

        }
        public DataTable ProductData(string UserId)
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("select productname,createdate,remark,address,case when status=0 then 'Pending' else case when status=1 then 'Approve' else case when status=2 then 'cancel' end end end as status from tbl_Product_request where regno='" + UserId + "'", con);
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }
            }
            catch (Exception ex)
            {

            }
            return t1;
        }
        public bool CheckProductDeliverByAdmin(string UserId)
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("select regno from tbl_DeliverProductByAdmin where regno='" + UserId + "'", con);
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool ProductRequest(string UserId, ModProductRequest obj)
        {
            try
            {
                string qstr = "insert into tbl_Product_request(regno,ProductName,Address,status,createdate) values('" + UserId + "','" + obj.ProductName + "','" + obj.Address + "',0,(dateadd(minute,(30),dateadd(hour,(5),getutcdate()))))";
                SqlCommand cmd = new SqlCommand(qstr, con);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i>0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool FreeTopUp(ModFreeTopup pin)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Proc_Zero_Pin_Topup", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AppMstRegNo", pin.regno);
                cmd.Parameters.AddWithValue("@pinno", pin.pinno);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet GetAllDataBalancenewdash(string UserId)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlCommand cmd = new SqlCommand("GetBalance_newdash", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);

                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                dr.Fill(ds);

            }
            catch (Exception ex)
            {
            }
            return ds;
        }
        public bool CheckTopup(string UserId)
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("select regno from Tbl_Transaction_Details where regno='" + UserId + "'", con);
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }









        public CommonBinaryTree GetFirstTree(string UID)
        {
            CommonBinaryTree comm = new CommonBinaryTree();
            if (UID != null)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(str))
                    {
                        using (SqlCommand cmd = new SqlCommand("USP_GetGeneologyDetailsByID", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@UID", UID);
                            con.Open();
                            SqlDataReader rdr = cmd.ExecuteReader();
                            if (rdr.HasRows)
                            {
                                while (rdr.Read())
                                {
                                    comm.userid = rdr["AppMstRegNo"].ToString();
                                    comm.name = rdr["appmstfname"].ToString();
                                    comm.jointype = rdr["jointype"].ToString();
                                    comm.Sid = rdr["SponsorID"].ToString();
                                    comm.Doj = rdr["appmstdoj"].ToString();
                                    comm.TotalLeftId = rdr["totalleftid"].ToString();
                                    comm.TotalRightId = rdr["totalrightid"].ToString();
                                    comm.ActiveLeft = rdr["left_active"].ToString();
                                    comm.ActiveRight = rdr["right_active"].ToString();
                                    comm.CarryLeft = rdr["carryleft"].ToString();
                                    comm.CarryRight = rdr["carryright"].ToString();
                                    comm.LeftTotal = rdr["AppMstLeftTotal"].ToString();
                                    comm.RightTotal = rdr["AppMstRightTotal"].ToString();
                                }
                            }
                            con.Close();
                        }
                    }
                }
                catch
                {
                }
            }
            return comm;
        }
        public DataTable GetVenderList()
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_VenderList", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }
            }
            catch (Exception ex)
            {

            }

            return t1;

        }
        public DataTable GetFranchise()
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_Franchise", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }
            }
            catch (Exception ex)
            {

            }

            return t1;

        }
        public DataTable GetStateList()
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("select StateId, State from DDlState order by state", con);
                cmd.CommandTimeout = 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }
            }
            catch (Exception ex)
            {

            }

            return t1;

        }
        public DataTable GetCityList(int stateid)
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("select * from DDlCity where StateId ='"+ stateid+"' order by city", con);
                cmd.CommandTimeout = 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }
            }
            catch (Exception ex)
            {

            }

            return t1;

        }
        public bool AddFranchaises(ModFranchises obj)
        {
            try
            {
                Random r = new Random();
                string uid = "FN" + r.Next(100, 999999);
                obj.Uid = uid;
                con.Open();
                string qry = "insert into Tbl_Franchise(franchiseid,sponserid,name,address,city,state,email,mobileno,investment,floorarea,commercialproperty,details,status) values(@uid,@sponserid,@name,@address,@city,@state,@email,@mobileno,@investment,@floorarea,@commercialproperty,@details,@status)";
                SqlCommand cmd = new SqlCommand(qry, con);
                cmd.Parameters.AddWithValue("@uid ", obj.Uid);
                cmd.Parameters.AddWithValue("@sponserid", obj.SponsorId);
                cmd.Parameters.AddWithValue("@name", obj.Name);
                cmd.Parameters.AddWithValue("@address", obj.Address);
                cmd.Parameters.AddWithValue("@city", obj.City);
                cmd.Parameters.AddWithValue("@state", obj.State);
                cmd.Parameters.AddWithValue("@email", obj.Email);
                cmd.Parameters.AddWithValue("@mobileno", obj.MobileNo);
                cmd.Parameters.AddWithValue("@investment", obj.Investment);
                cmd.Parameters.AddWithValue("@floorarea", obj.FloorArea);
                cmd.Parameters.AddWithValue("@commercialproperty", obj.CommProperty);
                cmd.Parameters.AddWithValue("@details", obj.Details);
                cmd.Parameters.AddWithValue("@status", 0);
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i > 0)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetMarqueFranchies()
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("select upper(name) as name, address from Tbl_Franchise where status=1 order by id desc", con);
                cmd.CommandTimeout = 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }
            }
            catch (Exception ex)
            {

            }

            return t1;

        }
        public DataTable GetMarquePoolAchived()
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("select p.regno, Upper(name) name from Tbl_Pool_two p  order by id desc", con);
                cmd.CommandTimeout = 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }
            }
            catch (Exception ex)
            {

            }
            return t1;

        }
        public bool AddContact(ModContact obj)
        {
            try
            {
                
                SqlCommand cmd = new SqlCommand("SP_Contact", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", obj.Name);
                cmd.Parameters.AddWithValue("@Email", obj.Email);
                cmd.Parameters.AddWithValue("@Mobile", obj.Mobile);
                cmd.Parameters.AddWithValue("@Message", obj.Message);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i > 0)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetAcheverList()
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_AcheverList", con);
                cmd.CommandType=CommandType.StoredProcedure;
                cmd.CommandTimeout = 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }
            }
            catch (Exception ex)
            {

            }
            return t1;

        }
        public DataTable GetAcheverListByRank(string rank)
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_AcheverListByRank", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Rank", rank);
                cmd.CommandTimeout = 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }
            }
            catch (Exception ex)
            {

            }
            return t1;

        }
        public DataTable GetAwardGallery()
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_AwardGallery", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }
            }
            catch (Exception ex)
            {

            }
            return t1;

        }
        public DataTable GetSlider()
        {
            DataTable t1 = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("SP_GetSliderMaster", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 1000;
                SqlDataAdapter dr = new SqlDataAdapter(cmd);
                DataTable t2 = new DataTable();
                dr.Fill(t2);
                if (t2.Rows.Count > 0)
                {
                    t1 = t2;
                }
            }
            catch (Exception ex)
            {

            }
            return t1;

        }
        public bool AddRecharge(ModRecharge obj)
        {
            try
            {

                SqlCommand cmd = new SqlCommand("SP_Recharge", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MobileNo", obj.Mobile);
                cmd.Parameters.AddWithValue("@Name", obj.Name);
                cmd.Parameters.AddWithValue("@IdNo", obj.IdNo);
                cmd.Parameters.AddWithValue("@MailId", obj.MailId);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i > 0)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
