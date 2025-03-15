using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Constance
{
    public static class BookingConst
    {
        // Success Messages
        public const string ProofSubmittedSuccessfully = "Proof submitted successfully";
        public const string BookingCreatedSuccessfully = "Booking created successfully";
        public const string BookingUpdatedSuccessfully = "Booking updated successfully";
        public const string BookingCancelledSuccessfully = "Booking cancelled successfully";

        // Error Messages
        public const string BookingNotFound = "Booking not found";
        public const string UnauthorizedProofSubmission = "You are not authorized to submit proof for this booking";
        public const string InvalidBookingStatusForProof = "Proof can only be submitted for OnGoing or Paid bookings";
        public const string ProofSubmissionFailed = "Failed to submit booking proof";
        public const string BookingAlreadyCompleted = "Booking is already completed";
        public const string InvalidBookingTimeSlot = "The selected time slot is not available";
        public const string BookingCancellationFailed = "Failed to cancel booking";

        // Booking Statuses
        public const string OnGoing = "OnGoing";
        public const string Paid = "Paid";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
        public const string Pending = "Pending";
        public const string Confirmed = "Confirmed";
    }
}
