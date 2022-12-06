namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities
{
    using System;

    public class CustomerMainDBModel
    {
        private DateTime createdDate;

        public int CustID { get; set; }

        public int CustSubID { get; set; }

        public string UserName { get; set; }

        public string UserName2 { get; set; }

        public string Site { get; set; }

        public int SiteID { get; set; }

        public int RoleID { get; set; }

        public int CurrencyID { get; set; }

        public string Currency { get; set; }

        public int SRecommend { get; set; }

        public int MRecommend { get; set; }

        public int Recommend { get; set; }

        public DateTime CreatedDate
        {
            get { return Convert.ToDateTime(createdDate.ToString("yyyy-MM-dd HH:mm:ss.ffff")); }
            set { createdDate = value; }
        }

        public int CustStatusID { get; set; }

        public int Danger1 { get; set; }

        public int Danger2 { get; set; }

        public int Danger3 { get; set; }

        public int Danger4 { get; set; }

        public int Danger5 { get; set; }

        public int Danger6 { get; set; }

        public int Danger7 { get; set; }

        public bool IsLicensee { get; set; }
        
        public bool IsLicenseeVIP { get; set; }

        public bool IsLicenseeBA { get; set; }
    }
}