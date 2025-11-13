namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport
{   
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB.Model;
    using Fanex.NAP.SPU.CTS.Shares;   
    using Newtonsoft.Json;
    using System.Collections.Generic;    
    using System.Linq;   
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IProblemAccountBySportService
    {
        IEnumerable<ProblemAccountLosingBySportEntity> GetPALosingBySport(IEnumerable<BaseCustomerInfoBySportModel> custInfos);

        IEnumerable<long> InsertProblemAccountBySport(IEnumerable<InsertProblemAccountBySportEntity> paBySports, CCInputFlow cCInputFlow);
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

        public IEnumerable<ProblemAccountLosingBySportEntity> GetPALosingBySport(IEnumerable<BaseCustomerInfoBySportModel> custInfos)
        {
            if (custInfos.IsNotNullOrEmpty())
            {
                var custInfosJson = JsonConvert.SerializeObject(custInfos);

                return _dataAccess.GetPALosingBySport(custInfosJson);
            }

            return Enumerable.Empty<ProblemAccountLosingBySportEntity>();
        }

        public IEnumerable<long> InsertProblemAccountBySport(IEnumerable<InsertProblemAccountBySportEntity> paBySports, CCInputFlow cCInputFlow)
        {
            if (paBySports.IsNotNullOrEmpty())
            {
                var paBySportClassification = CombinePAWithLosingPerformace(paBySports);

                var modelInsert = new InsertCustomerClassificationBySportModel
                {
                    CustClassificationJson = JsonConvert.SerializeObject(paBySportClassification)
                };

                var insertPaResult = _baseDataAccess.InsertCustomerClassificationBySport(CCInputFlow.BySportInsertPA, modelInsert);

                if (insertPaResult.CustInfos.IsNotNullOrEmpty())
                {
                    return insertPaResult.CustInfos.Select(x => x.CustID).Distinct();
                }
            }

            return Enumerable.Empty<long>();
        }

        #region Private Methods
        private IEnumerable<InsertProblemAccountBySportEntity> CombinePAWithLosingPerformace(IEnumerable<InsertProblemAccountBySportEntity> paBySports)
        {
            var custInfos = paBySports.Select(x => new BaseCustomerInfoBySportModel
            {
                CustID = x.CustID,
                SportGroup = x.SportGroup
            }).DistinctBy(x => new { x.CustID, x.SportGroup });

            var paBySportLosings = GetPALosingBySport(custInfos);

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
                              SportGroup = group.pa.SportGroup,
                              MatchID = group.pa.MatchID,

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
