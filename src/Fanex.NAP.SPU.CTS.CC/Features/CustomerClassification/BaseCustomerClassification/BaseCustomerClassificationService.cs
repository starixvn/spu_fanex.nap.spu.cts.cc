namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IBaseCustomerClassificationService
    {
        ResultInsertCustomerClassificationModel InsertCustomerClassification(CCInputFlow inputFlow, InsertCustomerClassificationModel model);
    }

    public class BaseCustomerClassificationService : IBaseCustomerClassificationService
    {
        private readonly IBaseCustomerClassificationDataAccess baseDataAccess;

        public BaseCustomerClassificationService(IBaseCustomerClassificationDataAccess baseDataAccess)
        {
            this.baseDataAccess = baseDataAccess;
        }

        public ResultInsertCustomerClassificationModel InsertCustomerClassification(CCInputFlow inputFlow, InsertCustomerClassificationModel model)
        {
            return this.baseDataAccess.InsertCustomerClassification(inputFlow, model);
        }

    }
}
