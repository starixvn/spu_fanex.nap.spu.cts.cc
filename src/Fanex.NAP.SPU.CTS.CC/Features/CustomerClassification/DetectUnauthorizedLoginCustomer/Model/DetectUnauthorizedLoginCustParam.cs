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
                this.LastScanCustID = SystemParameters.UnauthorizedLogin_LastScanCustID_Licensee;
                this.LastScanDate = SystemParameters.UnauthorizedLogin_LastScanDate_Licensee;
            }
            else
            {
                this.FirstBetFromDay = -14;
                this.FirstBetToDay = -8;
                this.LastScanCustID = SystemParameters.UnauthorizedLogin_LastScanCustID_Credit;
                this.LastScanDate = SystemParameters.UnauthorizedLogin_LastScanDate_Credit;
            }
        }

        public int FirstBetFromDay { get; set; }

        public int FirstBetToDay { get; set; }

        public SystemParameters LastScanCustID { get; set; }

        public SystemParameters LastScanDate { get; set; }

        public bool IsLicensee { get; set; }
    }
}
