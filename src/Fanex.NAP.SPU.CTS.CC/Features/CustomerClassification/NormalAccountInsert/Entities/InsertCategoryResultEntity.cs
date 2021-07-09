namespace Fanex.NAP.SPU.CTS.Features.CustomerClassification.NormalAccountInsert
{
    using System.Collections.Generic;

    public class InsertCategoryResultEntity
    {
        public string ErrorMessage { get; set; }

        public IEnumerable<long> CustIds { get; set; }
    }
}