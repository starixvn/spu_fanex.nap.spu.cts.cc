namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.PAPotentialClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialGroupBetting;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartService;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Sentry;

    public interface IPAPotentialClassificationService
    {
        void SetTWInitialSmartSmarts(PADailyScanFromQueueModel paDailyScanFromQueue);

        void SetTWInitialGBs(PADailyScanFromQueueModel paDailyScanFromQueue);

        string BuildPAPotentialInfo(IEnumerable<ProblemAccountLosingClassificationEntity> problemClassification);
    }

    public class PAPotentialClassificationService : ProblemAccountServices, IPAPotentialClassificationService
    {
        private readonly IPAPotentialClassificationDataAccess paPotentialDataAccess;        

        public PAPotentialClassificationService(IBaseCustomerClassificationDataAccess baseDataAccess,
            IProblemAccountDataAccess problemAccountDataAccess,
            IPAPotentialClassificationDataAccess paPotentialDataAccess,
            ISentryClient sentryClient): base(baseDataAccess, problemAccountDataAccess, sentryClient)
        {
            this.paPotentialDataAccess = paPotentialDataAccess;
        }

        private Dictionary<long, TWInitialSmartEntity> DicTWInitialSmarts { get; set; }

        private Dictionary<long, TWInitialGBGetChangesEntity> DicTWInitialGBs { get; set; }        

        public void SetTWInitialSmartSmarts(PADailyScanFromQueueModel paDailyScanFromQueue)
        {
            if (paDailyScanFromQueue.PAPotentialScanCustIDs == null)
                return;

            var custTWInitialSmartCustIDs = paDailyScanFromQueue.PAPotentialScanCustIDs.Where(x => x.RemarkSourceType == (int)CustToBuildRemark.TWInitialSmart).Select(x => x.CustID);

            if (custTWInitialSmartCustIDs.Any())
            {
                this.DicTWInitialSmarts = paPotentialDataAccess.GetInitialSmartPA(string.Join(",", custTWInitialSmartCustIDs)).ToDictionary(x => x.CustID);
            }            
        }

        public void SetTWInitialGBs(PADailyScanFromQueueModel paDailyScanFromQueue)
        {
            if (paDailyScanFromQueue.PAPotentialScanCustIDs == null)
                return;

            var custTWInitialGBCustIDs = paDailyScanFromQueue.PAPotentialScanCustIDs.Where(x => x.RemarkSourceType == (int)CustToBuildRemark.TWInitialGB).Select(x => x.CustID);

            if (custTWInitialGBCustIDs.Any())
            {
                this.DicTWInitialGBs = paPotentialDataAccess.GetInitialTWGroupBetting(string.Join(",", custTWInitialGBCustIDs)).ToDictionary(x => x.CustID);
            }
        }

        private string BuildRemarkTWInitialSmart(ProblemAccountLosingClassificationEntity pa)
        {
            if (this.DicTWInitialSmarts != null)
            {
                this.DicTWInitialSmarts.TryGetValue(pa.CustId, out TWInitialSmartEntity twInitialSmartCust);

                if (twInitialSmartCust != null)
                {
                    return string.Format(Constants.TWInitialSmartRemark, twInitialSmartCust.DetectedDate.ToString(Constants.CONVERT_TO_DATETIME_MMMddyyyy), twInitialSmartCust.Probability);                    
                }
            }
            
            return null;
        }

        private string BuildRemarkTWInitialGB(ProblemAccountLosingClassificationEntity pa)
        {
            if (this.DicTWInitialGBs != null)
            {
                this.DicTWInitialGBs.TryGetValue(pa.CustId, out TWInitialGBGetChangesEntity twGBCust);

                if (twGBCust != null)
                {
                    return string.Format(Constants.TWTWInitialGBRemark, twGBCust.DetectedDate.ToString(Constants.CONVERT_TO_DATETIME_MMMddyyyy), twGBCust.GBTicketCount, twGBCust.SportName);                    
                }
            }
            
            return null;
        }

        public string BuildPAPotentialInfo(IEnumerable<ProblemAccountLosingClassificationEntity> problemClassification)
        {
            if (problemClassification != null && problemClassification.Any())
            {               
                var dailyPas = problemClassification.Select(x =>
                {
                    var pa = new ProblemAccountLosingClassificationModel(x);
                    
                    pa.Remark = BuildRemarkTWInitialSmart(x) ?? BuildRemarkTWInitialGB(x);
                   
                    return pa;
                });

                return JsonConvert.SerializeObject(dailyPas);
            }

            return JsonConvert.SerializeObject(problemClassification.Select(x => new ProblemAccountLosingClassificationModel(x)));
        }  
                
    }
}
