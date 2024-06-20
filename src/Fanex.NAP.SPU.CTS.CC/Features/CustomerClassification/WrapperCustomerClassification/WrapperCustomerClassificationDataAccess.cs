namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.WrapperCustomerClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IWrapperCustomerClassificationDataAccess
    {
        public ResultInsertCustomerClassificationModel InsertCustomerClassification(CCInsertInputFlow inputFlow, InsertCustomerClassificationModel model);
    }

    public class WrapperCustomerClassificationDataAccess : IWrapperCustomerClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public WrapperCustomerClassificationDataAccess()
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
