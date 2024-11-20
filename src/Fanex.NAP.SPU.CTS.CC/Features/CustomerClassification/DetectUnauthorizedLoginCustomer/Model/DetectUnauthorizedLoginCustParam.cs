namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DetectUnauthorizedLoginCustomer
{
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class DetectUnauthorizedLoginCustParam
    {
        public DetectUnauthorizedLoginCustParam() { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "<Pending>")]
        public DetectUnauthorizedLoginCustParam(bool isLicensee)
        {
            this.IsLicensee = isLicensee;

            if (isLicensee)
            {
                this.FirstBetFromDay = -3;
                this.FirstBetToDay = -1;
                this.LastScanCustID = CTS_SystemParameters.UnauthorizedLogin_LastScanCustID_Licensee;
                this.LastScanDate = CTS_SystemParameters.UnauthorizedLogin_LastScanDate_Licensee;
            }
            else
            {
                this.FirstBetFromDay = -14;
                this.FirstBetToDay = -8;
                this.LastScanCustID = CTS_SystemParameters.UnauthorizedLogin_LastScanCustID_Credit;
                this.LastScanDate = CTS_SystemParameters.UnauthorizedLogin_LastScanDate_Credit;
            }
        }

        public int FirstBetFromDay { get; set; }

        public int FirstBetToDay { get; set; }

        public CTS_SystemParameters LastScanCustID { get; set; }

        public CTS_SystemParameters LastScanDate { get; set; }

        public bool IsLicensee { get; set; }
    }
}
