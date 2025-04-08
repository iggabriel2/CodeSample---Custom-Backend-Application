using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.View
{
    public class AzSpUserView
    {
        public int Id { get; set; }
		public string UserName { get; set; }
		public Boolean EmailConfirmed { get; set; }
	    public Decimal SubscriptionAmount { get; set; }
        public Int32 PaymentSchedule { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PlanName { get; set; }
        public string Email { get; set; }
        public string SubscriptionStatus { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string? CCLastFour { get; set; }

        public bool FreeTrial { get; set; }
        public int PromoCode { get; set; }
    }
}
