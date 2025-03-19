using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Constance
{
    public static class RequestConst
    {
        public const string GetRequestNullError = "Something wrong getting Pending Request";
        public const string UpdateRequestSuccess = "Approve/Reject Service Created Request Successfully";
        public const string UpdateRequestStatusError = "Service status can only be updated if it is Pending";

        //email subject
        public const string RejectEmailSubject = "Information about your Service Creation...";
        public const string ApproveEmailSubject = "Information about your Service Creation...";
        public const string RefundEmailCustomerSubject = "Information about your Refund Request...";
        public const string RefundEmailHousekeeperSubject = "Information about the Refund Request made by your Customer...";

        //status
        public const string Pending = "Pending";
    }
}
