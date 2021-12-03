namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    public class InsertAccountParasEntity
    {
        public string AccountID { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string CountryCode { get; set; }

        public string Email { get; set; }

        public string DocumentID { get; set; }

        public int? DocumentTypeID { get; set; }

        public string BankName { get; set; }

        public string BankAccountID { get; set; }

        public int? FraudID { get; set; }
    }
}