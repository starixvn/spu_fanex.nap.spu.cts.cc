namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport
{   
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB.Model;
    using Fanex.NAP.SPU.CTS.Shares;   
    using Newtonsoft.Json;
    using System.Collections.Generic;    
    using System.Linq;   
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IProblemAccountBySportService
    {
        IEnumerable<ProblemAccountLosingBySportEntity> GetPALosingBySport(IEnumerable<BaseCustomerInfoBySportModel> custInfos, BySport_FunctionId functionId);

        IEnumerable<InsertPABySportResultModel> InsertProblemAccountBySport(IEnumerable<InsertProblemAccountBySportEntity> paBySports, CCInputFlow cCInputFlow, BySport_FunctionId functionId);
    }

    public class ProblemAccountBySportService : IProblemAccountBySportService
    {
        private readonly IProblemAccountBySportDataAccess _dataAccess;
        private readonly IBaseCustomerClassificationBySportDataAccess _baseDataAccess;

        public ProblemAccountBySportService(IProblemAccountBySportDataAccess dataAccess
            , IBaseCustomerClassificationBySportDataAccess baseDataAccess)
        {
            _dataAccess = dataAccess;
            _baseDataAccess = baseDataAccess;
        }

        public IEnumerable<ProblemAccountLosingBySportEntity> GetPALosingBySport(IEnumerable<BaseCustomerInfoBySportModel> custInfos, BySport_FunctionId functionId)
        {
            if (custInfos.IsNotNullOrEmpty())
            {
                var custInfosJson = JsonConvert.SerializeObject(custInfos);

                return _dataAccess.GetPALosingBySport(custInfosJson, functionId);
            }

            return Enumerable.Empty<ProblemAccountLosingBySportEntity>();
        }

        public IEnumerable<InsertPABySportResultModel> InsertProblemAccountBySport(IEnumerable<InsertProblemAccountBySportEntity> paBySports, CCInputFlow cCInputFlow, BySport_FunctionId functionId)
        {
            if (paBySports.IsNotNullOrEmpty())
            {
                var paBySportClassification = CombinePAWithLosingPerformace(paBySports, functionId);

                var modelInsert = new InsertCustomerClassificationBySportModel
                {
                    CustClassificationJson = JsonConvert.SerializeObject(paBySportClassification)
                };

                var insertPaResult = _baseDataAccess.InsertCustomerClassificationBySport(CCInputFlow.BySportInsertPA, modelInsert);

                if (insertPaResult.CustInfos.IsNotNullOrEmpty())
                {
                    return insertPaResult.CustInfos.Select(x => new InsertPABySportResultModel
                    {
                        CustID = x.CustID,
                        SportGroup = x.SportGroup
                    }).Distinct();
                }
            }

            return Enumerable.Empty<InsertPABySportResultModel>();
        }

        #region Private Methods
        private IEnumerable<InsertProblemAccountBySportEntity> CombinePAWithLosingPerformace(IEnumerable<InsertProblemAccountBySportEntity> paBySports, BySport_FunctionId functionId)
        {
            var custInfos = paBySports.Select(x => new BaseCustomerInfoBySportModel
            {
                CustID = x.CustID,
                SportGroup = x.SportGroup
            }).DistinctBy(x => new { x.CustID, x.SportGroup });

            var paBySportLosings = GetPALosingBySport(custInfos, functionId);           

            return MergePALosingClassification(paBySports, paBySportLosings);
        }

        private IEnumerable<InsertProblemAccountBySportEntity> MergePALosingClassification(IEnumerable<InsertProblemAccountBySportEntity> paBySports
            , IEnumerable<ProblemAccountLosingBySportEntity> paBySportLosings)
        {
            if (paBySports.IsNotNullOrEmpty())
            {
                return paBySports.GroupJoin(paBySportLosings ?? Enumerable.Empty<ProblemAccountLosingBySportEntity>(),
                                    pa => $"{pa.CustID}_{pa.SportGroup}",
                                    pl => $"{pl.CustId}_{pl.SportGroup}",
                                    (pa, pl) => new { pa, pl })
                    .SelectMany(
                         group => group.pl.DefaultIfEmpty(),
                          (group, temp) => new InsertProblemAccountBySportEntity
                          {
                              CTSCustID = group.pa.CTSCustID,
                              CustID = group.pa.CustID,
                              RoleID = group.pa.RoleID,
                              SubscriberID = group.pa.SubscriberID,
                              CategoryID = group.pa.CategoryID,
                              CategoryGroupID = group.pa.CategoryGroupID,
                              IsLicensee = group.pa.IsLicensee,
                              CreatedBy = group.pa.CreatedBy,
                              CreatedDate = group.pa.CreatedDate,
                              Remark = group.pa.Remark,
                              MatchID = group.pa.MatchID,
                              SportGroup = group.pa.SportGroup,
                               
                              BetCount = temp?.BetCount,
                              ActiveDays = temp?.ActiveDays,
                              TurnoverRM = temp?.TurnoverRM,
                              WinlossRM = temp?.WinlossRM,
                              WinlossStatus = temp?.WinlossStatus,
                              PerformanceTime = temp?.PerformanceTime
                          }
                    );
            }

            return Enumerable.Empty<InsertProblemAccountBySportEntity>();
        }

        #endregion
    }
}
