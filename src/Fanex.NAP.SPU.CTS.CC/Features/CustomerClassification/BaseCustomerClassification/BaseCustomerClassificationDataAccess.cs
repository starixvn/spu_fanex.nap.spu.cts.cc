namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{
    using Fanex.Data.Repository;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IBaseCustomerClassificationDataAccess
    {
        public ResultInsertCustomerClassificationModel InsertCustomerClassification(CCInputFlow inputFlow, InsertCustomerClassificationModel model);

        ResultRemoveCustomerClassificationModel RemoveCustomerClassification(CCInputFlow inputFlow, RemoveCustomerClassificationModel model);
    }

    public class BaseCustomerClassificationDataAccess : IBaseCustomerClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public BaseCustomerClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }      

        public ResultInsertCustomerClassificationModel InsertCustomerClassification(CCInputFlow inputFlow, InsertCustomerClassificationModel model)
        {
            var criteria = new InsertCustomerClassificationCriteria
            {
                InputFlowID = (int)inputFlow,
                CustClassificationJson = model.CustClassificationJson
            };

            var custInfos = repository.Fetch<CustInfoClassificationEntity>(criteria);

            return new ResultInsertCustomerClassificationModel { CustInfos = custInfos, ErrorMessage = criteria.ErrorMessage };
        }

        public ResultRemoveCustomerClassificationModel RemoveCustomerClassification(CCInputFlow inputFlow, RemoveCustomerClassificationModel model)
        {
            var criteria = new RemoveCustomerClassificationCriteria
            {
                InputFlowID = (int)inputFlow,
                CustClassificationJson = model.CustClassificationJson,               
            };

            var custInfos = repository.Fetch<CustInfoClassificationEntity>(criteria);

            return new ResultRemoveCustomerClassificationModel { CustInfos = custInfos };
        }
    }
}
