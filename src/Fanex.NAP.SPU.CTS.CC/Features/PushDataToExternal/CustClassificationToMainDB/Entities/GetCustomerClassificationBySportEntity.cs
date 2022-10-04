namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{   
    public class GetCustomerClassificationBySportEntity
    {       
        public long CustID { get; set; }
     
        public int SportID { get; set; }

        public int CategoryID { get; set; }

        public int CustomerClass { get; set; }
        
        public string CustomerClassName { get; set; }
    }
}
