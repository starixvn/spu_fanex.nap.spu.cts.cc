namespace Fanex.NAP.SPU.CTS.CC.Features.AffectedCustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;    
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IAffectedCustomerClassificationService
    {
        IEnumerable<LatestClassificationEntity> InsertVVIP(VVIPCustomerInsertEntity custInfo);
    }

    public class AffectedCustomerClassificationService : IAffectedCustomerClassificationService
    {
        private readonly IAffectedCustomerClassificationDataAccess affectedCustClassificationDataAccess;
        private readonly ISentryClient sentryClient;

        public AffectedCustomerClassificationService(ICustomerCategoryDataAccess dataAccess, ISentryClient sentryClient)
        {
            //this.dataAccess = dataAccess;
            //this.sentryClient = sentryClient;
        }

        //public CustomerCategoryQueueModel GetCustomerCategoryQueue()
        //{
        //    //return dataAccess.GetCustomerCategoryQueue();
        //}

        public IEnumerable<LatestClassificationEntity> InsertVVIP(VVIPCustomerInsertEntity custInfo)
        {
            if (custInfo != null && custInfo.CustInfo != null && custInfo.CustInfo.Any())
            {
                return affectedCustClassificationDataAccess.InsertVVIP(custInfo);
            }

            return Enumerable.Empty<LatestClassificationEntity>();
        }

        //public IEnumerable<LatestClassificationEntity> InsertVVIP(IEnumerable<VVIPCustomerInsertEntity> model, VVIP_FromAction fromAction, long? queueID, bool isAffectDownline);
        //{
        //    return dataAccess.InsertVVIP(model);
        //}

        //public IEnumerable<LatestClassificationEntity> RemoveCustomerCategory(CustomerCategoryQueueModel model)
        //{
        //    return dataAccess.RemoveCustomerCategory(model);
        //}
    }
}
