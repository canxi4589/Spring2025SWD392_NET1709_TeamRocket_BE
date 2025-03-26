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

        [Description("Refund transaction from customer")]
        RefundCustomer,

        [Description("Refund transaction from system")]
        RefundHousekeeper,

        [Description("Money taken from wallet for booking action transaction")]
        WalletPurchase,

        [Description("Transaction for VNPay booking action transaction")]
        VNPayPurchase,

        [Description("Money payback to wallet for cancel booking action transaction")]
        BookingCanceledPayback,

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
