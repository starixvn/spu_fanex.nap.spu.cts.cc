namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DetectUnauthorizedLoginCustomer
{
    using System;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class DetectUnauthorizedLoginCustParam
    {
        public DetectUnauthorizedLoginCustParam() { }

        public DetectUnauthorizedLoginCustParam(bool isLicensee)
        {
            this.IsLicensee = isLicensee;

            if (isLicensee)
            {
                this.FirstBetFromDay = -3;
                this.FirstBetToDay = -1;
                this.LastScanCustID = CTS_DC_SystemParameters.UnauthorizedLogin_LastScanCustID_Licensee;
                this.LastScanDate = CTS_DC_SystemParameters.UnauthorizedLogin_LastScanDate_Licensee;
            }
            else
            {
                this.FirstBetFromDay = -14;
                this.FirstBetToDay = -8;
                this.LastScanCustID = CTS_DC_SystemParameters.UnauthorizedLogin_LastScanCustID_Credit;
                this.LastScanDate = CTS_DC_SystemParameters.UnauthorizedLogin_LastScanDate_Credit;
            }
        }

        public int FirstBetFromDay { get; set; }

        public int FirstBetToDay { get; set; }

        public CTS_DC_SystemParameters LastScanCustID { get; set; }

        public CTS_DC_SystemParameters LastScanDate { get; set; }

        public bool IsLicensee { get; set; }
    }
}
