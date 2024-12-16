using static Fanex.NAP.SPU.CTS.Shares.Constants;

namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor
{
    public class NormalPoolMonitoringEntity
    {
        public int IssueTypeID { get; set; }

        public long NumberOfRecord { get; set; }

        public JobMonitor_IssueType MapIssueTypeID
        {
            get
            {
                return (JobMonitor_IssueType)IssueTypeID;
            }
        }
    }
}