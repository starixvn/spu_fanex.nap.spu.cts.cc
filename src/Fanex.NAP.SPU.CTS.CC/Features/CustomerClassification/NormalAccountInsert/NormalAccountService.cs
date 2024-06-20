namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.WrapperCustomerClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface INormalAccountService
    {
        GetNormalAccountModel GetNormalAccounts();

        GetNormalAccountModel GetNormalAccounts_Test();

        ResultInsertCustomerClassificationModel InsertNormalAccount(IEnumerable<InsertNormalAccountModel> custs, string serviceName, bool isWriteLog);

        void CompleteNormalAccountInsert(long scannedMaxId, bool isSuccess);

        IEnumerable<InsertNormalAccountModel> MergeWithTaggedNormalTaggingFlow(IEnumerable<GetNormalAccountEntity> custs);

        IEnumerable<InsertNormalAccountModel> MergeWithTWTaggedCustAndCTSTaggedCust(IEnumerable<GetNormalAccountEntity> custs);

    }

    public class NormalAccountService : INormalAccountService
    {
        private readonly INormalAccountDataAccess dataAccess;       
        private readonly ISentryClient sentryClient;

        public NormalAccountService(INormalAccountDataAccess dataAccess, ISentryClient sentryClient)
        {
            this.dataAccess = dataAccess;           
            this.sentryClient = sentryClient;
        }

        public GetNormalAccountModel GetNormalAccounts()
        {
            return dataAccess.GetNormalAccount();
        }

        public GetNormalAccountModel GetNormalAccounts_Test()
        {
            return new GetNormalAccountModel()
            {
                NormalAccount = new List<GetNormalAccountEntity>() { new GetNormalAccountEntity {
                     CustId = 1, ActiveDays = 1,
                      BetCount = 1, CategoryId = 201, CategoryGroupId = 201,
                       TurnoverRM = 100,
                        WinlossRM = 100
                } },
                ScannedMaxId = 123456,
                IsContinute = true
            };
        }

        public ResultInsertCustomerClassificationModel InsertNormalAccount(IEnumerable<InsertNormalAccountModel> custs, string serviceName, bool isWriteLog)
        {
            var result = new ResultInsertCustomerClassificationModel();
                      
            if (custs != null && custs.Any())
            {
                var modelInsert = new InsertCustomerClassificationModel()
                {                    
                    CustClassificationJson = JsonConvert.SerializeObject(custs)
                };

                result = dataAccess.InsertNormalCustomerClassification(modelInsert);

                Utilities.LogSentryInfo(serviceName, "InsertNormalAccount done", modelInsert, isWriteLog, sentryClient);

                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Utilities.LogSentryError(serviceName, "InsertNormalAccount throw Exception", modelInsert, new Exception(result.ErrorMessage), sentryClient);
                }
            }
            
            return result;
        }

        public void CompleteNormalAccountInsert(long scannedMaxId, bool isSuccess)
        {
            if (isSuccess)
            {
                dataAccess.CompleteNormalAccountInsert(scannedMaxId);
            }
        }

        public IEnumerable<InsertNormalAccountModel> MergeWithTaggedNormalTaggingFlow(IEnumerable<GetNormalAccountEntity> custs)
        {
            if (custs == null || !custs.Any())
            {
                return Enumerable.Empty<InsertNormalAccountModel>();
            }

            var twTaggedCusts = GetGroupBettingAndRejectCustomer(custs.Where(x => x.ScanTaggingTWType == 1));

            var custAfterCheckTWTagged = MergeWithTWTaggedCust(custs, twTaggedCusts);

            var custIdAfterCheckTWTagged = string.Join(",", custAfterCheckTWTagged.Where(x => x.TaggingID == 0).Select(x => x.CustId).Distinct());

            var custAssWithPA = GetCustomerAssociationProblemAccount(custIdAfterCheckTWTagged);

            var finalNormalAcc = MergeWithCTSTaggedCust(custAfterCheckTWTagged, custAssWithPA);

            return finalNormalAcc;
        }

        public IEnumerable<InsertNormalAccountModel> MergeWithTWTaggedCustAndCTSTaggedCust(IEnumerable<GetNormalAccountEntity> custs)
        {
            if (custs == null || !custs.Any())
            {
                return Enumerable.Empty<InsertNormalAccountModel>();
            }

            var twTaggingSRPCusts = GetGroupBettingAndRejectCustomer(custs.Where(x => x.ScanTaggingTWType == 1));

            var custAfterCheckTWTagged = MergeWithTWTaggedCustAndNewTWCust(custs, twTaggingSRPCusts);

            var custIdAfterCheckTWTagged = string.Join(",", custAfterCheckTWTagged.Where(x => x.TaggingID == 0).Select(x => x.CustId).Distinct());

            var custAssWithPA = GetCustomerAssociationProblemAccount(custIdAfterCheckTWTagged);

            var finalNormalAcc = MergeWithCTSTaggedCust(custAfterCheckTWTagged, custAssWithPA);

            return finalNormalAcc;
        }

        #region private method      

        private IEnumerable<CTSTaggedCustEntity> GetCustomerAssociationProblemAccount(string custIdList)
        {
            return dataAccess.GetCustsAssociationProblemAccount(custIdList);
        }

        private IEnumerable<TWTaggedCustEntity> GetGroupBettingAndRejectCustomer(IEnumerable<GetNormalAccountEntity> custs)
        {
            if (custs != null && custs.Any())
            {
                var custIds = string.Join(",", custs.Select(x => x.CustId).Distinct());

                return dataAccess.CheckGroupBettingAndRejectCustomer(custIds);
            }

            return new List<TWTaggedCustEntity>();
        }

        private IEnumerable<NormalAccountAfterCheckTWDetectionModel> MergeWithTWTaggedCust(IEnumerable<GetNormalAccountEntity> custs, IEnumerable<TWTaggedCustEntity> twTaggedCust)
        {
            var custsMapping = custs ?? new List<GetNormalAccountEntity>();
            var twTaggedCustMapping = twTaggedCust ?? new List<TWTaggedCustEntity>();

            return custsMapping.GroupJoin(twTaggedCustMapping,
                               custClass => custClass.CustId,
                               taggedCust => taggedCust.CustID,
                               (custClass, taggedCusts) => new { custClass, taggedCusts })
                               .SelectMany(
                                    group => group.taggedCusts.DefaultIfEmpty(),
                                    (group, temp) => new NormalAccountAfterCheckTWDetectionModel(group.custClass, temp));
        }

        private IEnumerable<NormalAccountAfterCheckTWDetectionModel> MergeWithTWTaggedCustAndNewTWCust(IEnumerable<GetNormalAccountEntity> custs, IEnumerable<TWTaggedCustEntity> twTaggingSRPCusts)
        {
            var custsMapping = custs ?? new List<GetNormalAccountEntity>();
            var twTaggingSRPCustMapping = twTaggingSRPCusts ?? new List<TWTaggedCustEntity>();
           
            var result = from cust in custsMapping
                         join srpCust in twTaggingSRPCustMapping on cust.CustId equals srpCust.CustID into srpCust_JoinTable
                         from srpCustDefault in srpCust_JoinTable.DefaultIfEmpty()                         
                         select MappingNormalAccountAfterCheckTWDetectionModel(cust, srpCustDefault);

            return result;
        }

        private NormalAccountAfterCheckTWDetectionModel MappingNormalAccountAfterCheckTWDetectionModel(GetNormalAccountEntity cust, TWTaggedCustEntity twTaggingSRPCust)
        {
            if (twTaggingSRPCust != null)
            {
                return new NormalAccountAfterCheckTWDetectionModel(cust, twTaggingSRPCust);
            }         

            return new NormalAccountAfterCheckTWDetectionModel(cust);
        }

        private IEnumerable<InsertNormalAccountModel> MergeWithCTSTaggedCust(IEnumerable<NormalAccountAfterCheckTWDetectionModel> custs, IEnumerable<CTSTaggedCustEntity> ctsTaggedCust)
        {
            var custsMapping = custs ?? new List<NormalAccountAfterCheckTWDetectionModel>();
            var ctsTaggedCustMapping = ctsTaggedCust ?? new List<CTSTaggedCustEntity>();

            return custsMapping.GroupJoin(ctsTaggedCustMapping,
                               custClass => custClass.CustId,
                               taggedCust => taggedCust.CustID,
                               (custClass, taggedCusts) => new { custClass, taggedCusts })
                               .SelectMany(
                                    group => group.taggedCusts.DefaultIfEmpty(),
                                    (group, temp) => new InsertNormalAccountModel(group.custClass, temp));
        }

        #endregion
    }
}
