namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.DailyNormalClassification
{
    using System;
    using System.Collections.Generic;

    public class DailyNormalClassificationEntity
    {
        public int op_LastCategoryID { get; set; }

        public UInt64 op_LastCustID { get; set; }

        public IEnumerable<Int64> CustID { get; set; }
    }
}