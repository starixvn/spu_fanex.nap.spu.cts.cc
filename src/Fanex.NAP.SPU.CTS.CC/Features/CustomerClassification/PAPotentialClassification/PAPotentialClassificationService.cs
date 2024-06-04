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
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;

    public interface IPAPotentialClassificationService
    {
        void SetTWInitialSmartSmarts(PADailyScanFromQueueModel paDailyScanFromQueue);

        void SetTWInitialGBs(PADailyScanFromQueueModel paDailyScanFromQueue);

        string BuildPAPotentialInfo(IEnumerable<ProblemAccountLosingClassificationEntity> problemClassification);
    }

    public class PAPotentialClassificationService : IPAPotentialClassificationService
    {
        private readonly IPAPotentialClassificationDataAccess paPotentialDataAccess;        

        public PAPotentialClassificationService(IPAPotentialClassificationDataAccess paPotentialDataAccess)
        {
            this.paPotentialDataAccess = paPotentialDataAccess;
        }

        private Dictionary<long, RescanTWInitialSmartPAEntity> DicTWInitialSmarts { get; set; }

        private Dictionary<long, TWInitialGBGetChangesEntity> DicTWInitialGBs { get; set; }        

        private IEnumerable<CustToBuildRemarkEntity> custTWInitialSmarts { get; set; }

        public void SetTWInitialSmartSmarts(PADailyScanFromQueueModel paDailyScanFromQueue)
        {
            var custTWInitialSmarts = paDailyScanFromQueue.PAPotentialScanCustIDs.Where(x => x.RemarkSourceType == (int)CustToBuildRemark.TWInitialSmart);

            if (custTWInitialSmarts.Any())
            {
                this.DicTWInitialSmarts = paPotentialDataAccess.CheckExistInitialSmartPA(string.Join(",", this.custTWInitialSmarts)).ToDictionary(x => x.CustID);
            }            
        }

        public void SetTWInitialGBs(PADailyScanFromQueueModel paDailyScanFromQueue)
        {           
            var custTWInitialGBs = paDailyScanFromQueue.PAPotentialScanCustIDs.Where(x => x.RemarkSourceType == (int)CustToBuildRemark.TWInitialGB);

            if (custTWInitialGBs.Any())
            {
                this.DicTWInitialGBs = paPotentialDataAccess.GetInitialTWGroupBetting(string.Join(",", custTWInitialGBs)).ToDictionary(x => x.CustID);
            }            
        }

        private string BuildRemarkTWInitialSmart(ProblemAccountLosingClassificationEntity pa)
        {
            if (this.DicTWInitialSmarts != null)
            {
                this.DicTWInitialSmarts.TryGetValue(pa.CustId, out RescanTWInitialSmartPAEntity twInitialSmartCust);

                if (twInitialSmartCust != null)
                {
                    return $"Detection Date: {twInitialSmartCust.DetectedDate.ToString(Constants.CONVERT_TO_DATETIME_MMMddyyyy)} - Probability: {twInitialSmartCust.Probability}";
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
                    return $"Detection Date: { twGBCust.DetectedDate.ToString(Constants.CONVERT_TO_DATETIME_MMMddyyyy)} - GB Bet Count: {twGBCust.GBTicketCount} - Sport: { twGBCust.SportName }";
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
