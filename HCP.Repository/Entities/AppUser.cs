using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class AppUser : IdentityUser
    {
        [MaxLength(100)]
        public string FullName { set; get; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? Birthday { set; get; }

        [DataType(DataType.Text)]
        public string Avatar { get; set; } = "https://villagesonmacarthur.com/wp-content/uploads/2020/12/Blank-Avatar.png";

        [DataType(DataType.Text)]
        public string? PDF { get; set; }
        public string? IdCardFront { get; set; }
        public string? IdCardBack { get; set; }
        public bool? IsHousekeeperVerified { get; set; }
        public bool Gender { get; set; }
        public double BalanceWallet { get; set; } = 0;
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<WalletTransaction> WalletTransactions { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<CleaningService> Services { get; set; }

        //public ICollection<Notification> SentNotifications { get; set; }
        //public ICollection<Notification> ReceivedNotifications { get; set; }


    }
}
