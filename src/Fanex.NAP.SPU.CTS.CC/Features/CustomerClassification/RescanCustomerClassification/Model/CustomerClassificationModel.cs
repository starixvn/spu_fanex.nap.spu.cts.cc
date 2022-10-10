namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification
{
    public class CustomerClassificationModel
    {
        public int CategoryID { get; set; }

        public int CategoryType { get; set; }

        public string CategoryName { get; set; }

        public int CategoryGroup { get; set; }

        public int ParentID { get; set; }

        public int CustID { get; set; }

        public string ParentCategoryName { get; set; }

        public bool IsProbation { get; set; }
    }
}
