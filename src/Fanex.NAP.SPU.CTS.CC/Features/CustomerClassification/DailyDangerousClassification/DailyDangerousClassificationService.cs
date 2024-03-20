namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyDangerousClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyDangerousClassificationService
    {
        DangerousToCCModel GetDangerousClassification(bool isLicensee);

        bool InsertDangerousClassification(IEnumerable<DangerousToCCEntity> dangerousCCs, bool isLicensee, PushDataToExternalEntity paras);

        IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(IEnumerable<long> custIds, FunctionId funcId);       
    }

    public class DailyDangerousClassificationService : IDailyDangerousClassificationService
    {
        private readonly IDailyDangerousClassificationDataAccess dangerousClassificationDataAccess;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;  
        private readonly string serviceName;

        public DailyDangerousClassificationService(IProblemAccountServices problemAccountServices,
            IDailyDangerousClassificationDataAccess dangerousClassificationDataAccess,
            ICustClassToMainDBService custClassToMainDBService)
        {        
            this.problemAccountServices = problemAccountServices;
            this.dangerousClassificationDataAccess = dangerousClassificationDataAccess;
            this.custClassToMainDBService = custClassToMainDBService;
            this.serviceName = "DailyDangerousClassificationService";
        }

        public DangerousToCCModel GetDangerousClassification(bool isLicensee)
        {
            return dangerousClassificationDataAccess.GetDangerousClassification(isLicensee);
        }

        public IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(IEnumerable<long> custIds, FunctionId funcId)
        {
            return problemAccountServices.GetProblemAccountLosingClassification(custIds, funcId);
        }

        public bool InsertDangerousClassification(IEnumerable<DangerousToCCEntity> dangerousCCs, bool isLicensee, PushDataToExternalEntity paras)
        {           
            if (dangerousCCs != null && dangerousCCs.Any())
            {                
                var pas = dangerousCCs.Select(x =>
                {                   
                    return new InsertProblemAccountEntity
                    {
                        CustID = x.CustID,
                        CTSCustID = x.CTSCustID,
                        RoleID = x.RoleID,
                        SubscriberID = x.SubscriberID,
                        CategoryID = x.CategoryID,
                        CategoryGroup = x.CategoryID,
                        IsLicensee = isLicensee,
                        IsFromAI = true,
                        IsDirectPA = false,
                        IsFromCTS = true,
                        IsFromTVS = false,
                        IsFromTW = false,                        
                        CreatedBy = Constants.StarixITId,
                        Remark = $"Detection Date: { x.ClassifiedDate.ToString(Constants.CONVERT_TO_DATETIME_ddMMMyyyy)} - Dangerous Score: {(int)(x.ClassifiedScore * 100)}%"
                    };
                });

                var pasChunks = pas.Chunk(paras.ExternalBatchSize);

                foreach (var pasChunk in pasChunks) 
                {
                    var paCustIDs = problemAccountServices.InsertProblemAccountPackage(pasChunk, FunctionId.DailyDangerousClassification, InsertPA_ByType.Category, false, serviceName);                   

                    custClassToMainDBService.PushDataToExternal(paCustIDs, paras);
                }                
            }

            return true;
        }      
    }
}
