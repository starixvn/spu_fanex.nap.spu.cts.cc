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

        //public ResultInsertCustomerClassificationModel InsertVVIPCustomerClassification(InsertCustomerClassificationModel model)
        //{
        //    return InsertCustomerClassification(CCInsertInputFlow.VVIP, model);
        //}

        //public ResultInsertCustomerClassificationModel InsertPACustomerClassification(InsertCustomerClassificationModel model)
        //{
        //    return InsertCustomerClassification(CCInsertInputFlow.PA, model);
        //}

        //public ResultInsertCustomerClassificationModel InsertPotentialCustomerClassification(InsertCustomerClassificationModel model)
        //{
        //    return InsertCustomerClassification(CCInsertInputFlow.Potential, model);
        //}

        //public ResultInsertCustomerClassificationModel InsertRobotCustomerClassification(InsertCustomerClassificationModel model)
        //{
        //    return InsertCustomerClassification(CCInsertInputFlow.Robot, model);
        //}

        public ResultInsertCustomerClassificationModel InsertCustomerClassification(CCInsertInputFlow inputFlow, InsertCustomerClassificationModel model)
        {
            var criteria = new InsertCustomerClassificationCriteria
            {
                InputFlowID = (int)inputFlow,
                CustClassificationJson = model.CustClassificationJson
            };

            var custIDs = repository.Fetch<long>(criteria);

            return new ResultInsertCustomerClassificationModel { CustIDs = custIDs, ErrorMessage = criteria.ErrorMessage };
        }
    }
}
