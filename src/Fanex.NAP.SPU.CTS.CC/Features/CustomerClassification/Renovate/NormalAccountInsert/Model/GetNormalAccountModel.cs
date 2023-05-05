namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert
{
    using System.Collections.Generic;

    public class GetNormalAccountModel
    {
        public GetNormalAccountModel()
        {
            this.NormalAccount = new List<GetNormalAccountEntity>();
            this.IsContinute = false;
        }

        public IEnumerable<GetNormalAccountEntity> NormalAccount { get; set; }

        public long ScannedMaxId { get; set; }

        public bool IsContinute { get; set; }
    }
}
