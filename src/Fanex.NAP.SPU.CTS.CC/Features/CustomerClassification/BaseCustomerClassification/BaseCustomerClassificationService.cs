namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{   
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IBaseCustomerClassificationService
    {
        GetRescanCustomerClassificationModel GetRescanCustomerClassification(string custIDs);

        ResultInsertCustomerClassificationModel InsertCustomerClassification(CCInputFlow inputFlow, InsertCustomerClassificationModel model);

        ResultRemoveCustomerClassificationModel RemoveCustomerClassification(CCInputFlow inputFlow, RemoveCustomerClassificationModel model);
    }

    public class BaseCustomerClassificationService : IBaseCustomerClassificationService
    {
        private readonly IBaseCustomerClassificationDataAccess baseDataAccess;

        public BaseCustomerClassificationService(IBaseCustomerClassificationDataAccess baseDataAccess)
        {
            this.baseDataAccess = baseDataAccess;
        }

        public GetRescanCustomerClassificationModel GetRescanCustomerClassification(string custIDs)
        {
            return this.baseDataAccess.GetRescanCustomerClassification(custIDs);
        }

        public ResultInsertCustomerClassificationModel InsertCustomerClassification(CCInputFlow inputFlow, InsertCustomerClassificationModel model)
        {
            return this.baseDataAccess.InsertCustomerClassification(inputFlow, model);
        }

        public ResultRemoveCustomerClassificationModel RemoveCustomerClassification(CCInputFlow inputFlow, RemoveCustomerClassificationModel model)
        {
            return this.baseDataAccess.RemoveCustomerClassification(inputFlow, model);
        }

    }
}
