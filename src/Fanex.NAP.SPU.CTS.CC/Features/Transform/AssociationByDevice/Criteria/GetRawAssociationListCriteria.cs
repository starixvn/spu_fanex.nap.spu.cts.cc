namespace Fanex.NAP.SPU.CTS.Features.Transform.AssociationByDevice.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetRawAssociationListCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfAssociation")]
        public int NumberOfAssociation { get; set; }

        [SpParam(SqlParamName = "ip_NoOfBatch")]
        public int NumberOfBatch { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transform_Association_GetPackage";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
