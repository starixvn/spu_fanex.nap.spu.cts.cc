namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    using Fanex.Data.Repository;

    public class ParticipantGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "AFC_DC_Participant_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
