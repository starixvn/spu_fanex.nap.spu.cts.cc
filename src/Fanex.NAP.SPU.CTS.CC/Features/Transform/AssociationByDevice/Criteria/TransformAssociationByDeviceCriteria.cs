namespace Fanex.NAP.SPU.CTS.Features.Transform.AssociationByDevice.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class TransformAssociationByDeviceCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_AssociationList")]
        public string AssociationList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_InsertAssociationByDevice";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
