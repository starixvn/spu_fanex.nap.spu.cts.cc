namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerCategory
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerCategoryQueueCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_VVIPQueue_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
