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
        [Description("Deposit transaction")]
        Deposit,
        [Description("Withdraw request by staff")]
        WithdrawRequestStaff,
        [Description("Withdraw by staff")]
        WithdrawStaff,
        [Description("Withdraw rejected by staff")]
        WithdrawRejectStaff,
        [Description("Show transaction history for staff")]
        ShowHistoryStaff,
        [Description("Withdraw request by user")]
        WithdrawRequestUser,
        [Description("Withdraw by user")]
        WithdrawUser,
        [Description("Withdraw rejected by user")]
        WithdrawRejectUser,
        [Description("Show transaction history for user")]
        ShowHistoryUser
    }
}
