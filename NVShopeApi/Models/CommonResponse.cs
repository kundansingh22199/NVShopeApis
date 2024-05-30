using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace NVShopeApi.Models
{
    public class CommonResponse
    {
        public string Status { get; set; }
        public Object Payload { get; set; }
        public string Message { get; set; }
    }
    public class TransferPin
    {
        public string SendTo { get; set; }
        public string PinNo { get; set; }

    }
    public class CommonRegisterDetails
    {
        public int Id { get; set; }
        public string RegNo { get; set; }
        public string SponsorId {  get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class treemodel
    {
        public string userid { get; set; }
        public string name { get; set; }
        public string jointype { get; set; }
        public List<treemodelll> Payload { get; set; }
    }
    public class treemodelll
    {
        public string userid { get; set; }
        public string name { get; set; }
        public string jointype { get; set; }
        public List<treemodellllll> Payload { get; set; }

    }
    public class treemodellllll
    {
        public string userid { get; set; }
        public string name { get; set; }
        public string jointype { get; set; }

    }
    public class CommonStringTree
    {
        public string userid { get; set; }
        public string jointype { get; set; }
        public string name { get; set; }
        public string Status { get; set; }
        public List<treemodel> Payload { get; set; }
        public string Message { get; set; }
    }
    public class modOtp
    {
        public string otp { get; set; }
        public string mobile { get; set; }
    }
    public class ModWalletTransf
    {
        public decimal WorkingWallet { get; set; }
        public decimal PurchaseWallet { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
    }
    public class ModVendorTransf
    {
        public decimal VendorWallet { get; set; }
        public decimal RepurchaseWallet { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
    }
    public class ModConfTransfer
    {
        public string RegNo {  get; set; }
        public string Password {  get; set; }
        public string Amount { get; set; }
        public string Remark { get; set; }
        public string Type { get; set; }
    }
    public class ModImg
    {
        public string Img { get; set; }
    }
    public class ModFile
    {
        public string File { get; set; }
        public string Id { get; set; }
        public string Action { get; set; }
    }
    public class CommonBinaryTree
    {
        public string userid { get; set; }
        public string name { get; set; }
        public string jointype { get; set; }
        public string Sid { get; set; }
        public string LeftTotal { get; set; }
        public string RightTotal { get; set; }
        public string Doj { get; set; }
        public string TotalLeftId { get; set; }
        public string TotalRightId { get; set; }
        public string CarryLeft { get; set; }
        public string CarryRight { get; set; }
        public string ActiveLeft { get; set; }
        public string ActiveRight { get; set; }
        public string Status { get; set; }
        public List<treemodel1> Payload { get; set; }
        public string Message { get; set; }
        public string Position { get; set; }
        
    }
    public class treemodel1
    {
        public string userid { get; set; }
        public string name { get; set; }
        public string jointype { get; set; }
        public string Sid { get; set; }
        public string LeftTotal { get; set; }
        public string RightTotal { get; set; }
        public string Doj { get; set; }
        public string TotalLeftId { get; set; }
        public string TotalRightId { get; set; }
        public string CarryLeft { get; set; }
        public string CarryRight { get; set; }
        public string ActiveLeft { get; set; }
        public string ActiveRight { get; set; }
        public string Position { get; set; }
        public List<treemodel2> Payload { get; set; }
    }
    public class treemodel2
    {
        public string userid { get; set; }
        public string name { get; set; }
        public string jointype { get; set; }
        public string Sid { get; set; }
        public string LeftTotal { get; set; }
        public string RightTotal { get; set; }
        public string Doj { get; set; }
        public string TotalLeftId { get; set; }
        public string TotalRightId { get; set; }
        public string CarryLeft { get; set; }
        public string CarryRight { get; set; }
        public string ActiveLeft { get; set; }
        public string ActiveRight { get; set; }
        public string Position { get; set; }
        public List<treemodel3> Payload { get; set; }

    }
    public class treemodel3
    {
        public string userid { get; set; }
        public string name { get; set; }
        public string jointype { get; set; }
        public string Sid { get; set; }
        public string LeftTotal { get; set; }
        public string RightTotal { get; set; }
        public string Doj { get; set; }
        public string TotalLeftId { get; set; }
        public string TotalRightId { get; set; }
        public string CarryLeft { get; set; }
        public string CarryRight { get; set; }
        public string ActiveLeft { get; set; }
        public string ActiveRight { get; set; }
        public string Position { get; set; }
    }
    public class Geneology
    {
        public string RegNo { get; set; }
        public string Name { get; set; }
        public string JoinType { get; set; }
        public string SponsorName { get; set; }
        public string SponsorID { get; set; }
        public string BusinessVolLeft { get; set; }
        public string BusinessVolRight { get; set; }
        public string DirectRefLeft { get; set; }
        public string DirectRefRight { get; set; }
        public string AccumulateLeft { get; set; }
        public string AccumulateRight { get; set; }
        public string MatchingLeft { get; set; }
        public string MatchingRight { get; set; }
        public string CarryForwardLeft { get; set; }
        public string CarryForwardRight { get; set; }
        public string NOL { get; set; }
        public string NOR { get; set; }
        public string DOJ { get; set; }
        public string TWLeftID { get; set; }
        public string TWRightID { get; set; }
        public string Position { get; set; }
    }

    public class ModPagination
    {
        public int limit { get; set; }
        public int lastIndex { get; set; }
    }
    public class ModProductRequest
    {
        public string ProductName { get; set; }
        public string Address { get; set; }
    }
    public class ModFreeTopup
    {
        public string regno { get; set; }
        public string pinno { get; set; }

    }
    public class ModFranchises
    {
        public string SponsorId { get; set; }
        public string Uid { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string Investment { get; set; }
        public string FloorArea { get; set; }
        public string CommProperty { get; set; }
        public string Details { get; set; }
    }
    public class ModContact
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Message { get; set; }
    }
    public class ModRecharge
    {
        public string Mobile { get; set; }
        public string Name { get; set; }
        public string IdNo { get; set; }
        public string MailId { get; set; }
    }
}
