using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Enums
{
    public enum TransactionType
    {

        [Description("Money taken from wallet for booking action transaction")]
        BookingPurchase,

        [Description("Deposit (nap tien) transaction")]
        Deposit,

        [Description("Withdraw transaction type")]
        Withdraw,

        [Description("Show Withdraw request for staff")]
        WithdrawRequestStaff,

        [Description("Show Withdraw by staff")]
        WithdrawStaff,

        [Description("Show Withdraw rejected by staff")]
        WithdrawRejectStaff,

        [Description("Show transaction history for staff")]
        ShowHistoryStaff,

        [Description("Show Withdraw request by user")]
        WithdrawRequestUser,

        [Description("Show Withdraw by user")]
        WithdrawUser,

        [Description("Show Withdraw rejected by user")]
        WithdrawRejectUser,

        [Description("Show transaction history (bao gom nap tien rut tien (include deposit, done/pending/fail withdraw) for user")]
        ShowWithdrawHistoryUser,

        [Description("Show transaction history (bao gom nap tien rut tien (include deposit, done/pending/fail withdraw) for user")]
        ShowAllHistoryUser
    }
}
