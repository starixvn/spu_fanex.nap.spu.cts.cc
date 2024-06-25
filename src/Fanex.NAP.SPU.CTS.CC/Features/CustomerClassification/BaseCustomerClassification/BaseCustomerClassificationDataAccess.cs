namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{
    using Fanex.Data.Repository;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IBaseCustomerClassificationDataAccess
    {
        public ResultInsertCustomerClassificationModel InsertCustomerClassification(CCInsertInputFlow inputFlow, InsertCustomerClassificationModel model);
    }

    public class BaseCustomerClassificationDataAccess : IBaseCustomerClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public BaseCustomerClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }      

        public ResultInsertCustomerClassificationModel InsertCustomerClassification(CCInsertInputFlow inputFlow, InsertCustomerClassificationModel model)
        {
            var criteria = new InsertCustomerClassificationCriteria
            {
                InputFlowID = (int)inputFlow,
                CustClassificationJson = model.CustClassificationJson
            };

            var custInfos = repository.Fetch<CustInfoClassificationModel>(criteria);

            return new ResultInsertCustomerClassificationModel { CustInfos = custInfos, ErrorMessage = criteria.ErrorMessage };
        }
    }
}
