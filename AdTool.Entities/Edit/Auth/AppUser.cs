using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Edit.Auth
{
    public class AppUser
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public bool IsActive { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PasswordHash { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? LastLockOutDate { get; set; }

        public bool IsLockedOut { get; set; }

        public DateTime? FailedPwdAttemptDate { get; set; }

        public int FailedPwdAttemptCount { get; set; }

        public DateTime? LastPwdChangedDate { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public decimal SubscriptionAmount { get; set; }
        public int PaymentPlan { get; set; }

        public DateTime JoinDate { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public int PaymentSchedule { get; set; }
        public Boolean AgreeWithPromoEmail { get; set; }
        public string? EmailCode { get; set; }
        public DateTime? EmailCodeDate { get; set; }
        public String? TempPass { get; set; }
        public DateTime? TempPassDate { get; set; }

        public int AccountType { get; set; }

        public int PromoCode { get; set; }
        public bool FreeTrial { get; set; }

    }

}
