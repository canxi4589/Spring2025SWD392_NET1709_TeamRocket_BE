using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Constance
{
    public static class CheckoutConst
    {

        //message
        public const string CreateCheckoutError = "Failed to create checkout.";
        public const string CreateCheckoutSuccess = "Checkout created successfully!";

        public const string UpdateCheckoutSuccess = "Checkout updated successfully!";
        public const string UpdateCheckoutError = "Failed to update checkout.";

        public const string GetCheckoutNullError = "There is no checkout matched";
    }
}
