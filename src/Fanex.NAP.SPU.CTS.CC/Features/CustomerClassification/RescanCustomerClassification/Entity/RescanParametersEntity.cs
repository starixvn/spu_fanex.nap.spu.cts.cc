namespace Fanex.NAP.SPU.CTS.Features.CustomerClassification.RescanCustomerClassification
{
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class RescanParametersEntity
    {
        public IEnumerable<long> CustIds { get; set; }

        public bool IsCheckPAFromNAP { get; set; }

        public bool IsPushCCToSoon88 { get; set; }

        public FunctionId FunctionId { get; set; }

        public int InternalBatchSize { get; set; }

        public int ExternalBatchSize { get; set; }

        public string ServiceName { get; set; }

        public bool IsWriteLog { get; set; }

        public string FeedSoon88API { get; set; }
    }
}