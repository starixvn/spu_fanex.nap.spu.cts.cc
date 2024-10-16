namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewCustomer
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownline;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownlineCustomerClassification.NewCustomerCustomerClassification.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using System.Linq;

    public interface INewCustomerClassificationDataAccess
    {
        NewCustomerClassificationModel GetNewCTSCustomerCategory();
    }

    public class NewCustomerClassificationDataAccess : INewCustomerClassificationDataAccess
    {
        private readonly IDynamicRepository _repository;

        public NewCustomerClassificationDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public NewCustomerClassificationModel GetNewCTSCustomerCategory()
        {
            var criteria = new NewCustomerCategoryGetCriteria();

            (var vvipClassification, var paClassification, var normalClassification) = _repository.FetchMultiple<VVIPCategoryEntity, InsertProblemAccountEntity, NormalRescanEntity>(criteria);

            return new NewCustomerClassificationModel()
            {
                VVIPClassification = vvipClassification ?? Enumerable.Empty<VVIPCategoryEntity>(),
                PAClassification = paClassification ?? Enumerable.Empty<InsertProblemAccountEntity>(),
                NormalClassification = normalClassification ?? Enumerable.Empty<NormalRescanEntity>(),
                LastCTSCustID = criteria.LastCTSCustID
            };
        }
    }
}