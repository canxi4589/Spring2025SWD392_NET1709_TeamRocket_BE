using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Constance
{
    public static class TransactionConst
    {
        //Error
        public const string NotFoundError = "Transaction Not Found";
        public const string DepositFail = "Cannot deposit";
        public const string RefundFail = "Cannot refund";
        public const string RefundProcessFail = "Cannot process refund";
        public const string RefundProcessSuccessfully = "Refund Request has been processed!";

        //Success
        public const string SuccessTaskMessage = "Task done successfully";
    }
}
