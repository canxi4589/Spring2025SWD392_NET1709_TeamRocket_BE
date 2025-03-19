using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.DTOs.DTOs
{
    public class EmailBodyTemplate
    {
        public static string CreateAccountEmail(string staffName, string staffMail, string staffPassword)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Welcome to the Team</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            text-align: center;
            padding: 20px;
        }}
        .container {{
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
            max-width: 600px;
            margin: auto;
        }}
        .content {{
            padding: 20px;
        }}
        .warning {{
            color: #d9534f;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='content'>
            <h2>Welcome to the Team, {staffName}!</h2>
            <p>We are pleased to inform you that your staff account has been successfully created.</p>
            <p><strong>Email:</strong> {staffMail}</p>
            <p><strong>Temporary Password:</strong> {staffPassword}</p>
            <p>Please use these credentials to log in to your account and change your password immediately.</p>
            <p class='warning'>⚠ Important: Do NOT share your account details with anyone. Unauthorized access or leakage of credentials may result in serious consequences.</p>
            <p>Best regards,<br> The Admin Team</p>
        </div>
    </div>
</body>
</html>";
        }
        public static string GetThankYouEmail(string userName)
        {
            return $@"
    <!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>Thank You</title>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
                text-align: center;
                padding: 20px;
            }}
            .container {{
                background-color: #ffffff;
                padding: 20px;
                border-radius: 8px;
                box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
                max-width: 600px;
                margin: auto;
            }}
            .content {{
                padding: 20px;
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='content'>
                <h2>Thank You, {userName}!</h2>
                <p>We appreciate your time and effort in being a part of our community.</p>
                <p>If you have any questions, feel free to reach out.</p>
                <p>Best regards,<br> The Team</p>
            </div>
        </div>
    </body>
    </html>";
        }

        public static string GetRegistrationConfirmationEmail(string imgUrl, string email, string linkUrl)
        {
            return $@"
    <!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>Registration Confirmation</title>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
                text-align: center;
                padding: 20px;
            }}
            .container {{
                background-color: #ffffff;
                padding: 20px;
                border-radius: 8px;
                box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
                max-width: 600px;
                margin: auto;
            }}
            .banner {{
                width: 100%;
                max-height: 200px;
                object-fit: cover;
                border-radius: 8px 8px 0 0;
            }}
            .content {{
                padding: 20px;
            }}
            .button {{
                display: inline-block;
                background-color: #007bff;
                color: #ffffff;
                padding: 12px 20px;
                text-decoration: none;
                border-radius: 5px;
                font-weight: bold;
                margin-top: 20px;
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <img src='{imgUrl}' alt='Banner' class='banner'>
            <div class='content'>
                <h2>Welcome to Our Platform!</h2>
                <p>Dear {email},</p>
                <p>Thank you for registering! Please confirm your email address by clicking the button below.</p>
                <a href='{linkUrl}' class='button'>Confirm Registration</a>
                <p>If you didn’t request this, you can safely ignore this email.</p>
                <p>Best regards,<br> The Team</p>
            </div>
        </div>
    </body>
    </html>";
        }

        public static string GetServiceApprovalEmail(string userName, bool isApproved, string actionLinkApproved, string actionLinkRejected)
        {
            string statusMessage = isApproved
                ? "We are pleased to inform you that your service has been approved! You can now start receiving bookings."
                : "Unfortunately, your service has been rejected. Please review our guidelines and make the necessary adjustments before resubmitting.";

            string actionText = isApproved ? "View Your Service" : "Review Submission";
            string actionLink = isApproved ? actionLinkApproved : actionLinkRejected;
            string buttonColor = isApproved ? "#28a745" : "#dc3545";

            return $@"
    <!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>Service Approval Status</title>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
                text-align: center;
                padding: 20px;
            }}
            .container {{
                background-color: #ffffff;
                padding: 20px;
                border-radius: 8px;
                box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
                max-width: 600px;
                margin: auto;
            }}
            .content {{
                padding: 20px;
            }}
            .button {{
                display: inline-block;
                padding: 10px 20px;
                margin-top: 20px;
                text-decoration: none;
                color: #ffffff;
                background-color: {buttonColor};
                border-radius: 5px;
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='content'>
                <h2>Hello, {userName}!</h2>
                <p>{statusMessage}</p>
                <p><a href='{actionLink}' class='button'>{actionText}</a></p>
                <p>If you have any questions, feel free to contact our support team.</p>
                <p>Best regards,<br> The Team</p>
            </div>
        </div>
    </body>
    </html>";
        }

        public static string GetRejectionEmail(string userName, string reason, Guid serviceId, string serviceName)
        {
            return $@"
    <!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>Service Request Rejected</title>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
                text-align: center;
                padding: 20px;
            }}
            .container {{
                background-color: #ffffff;
                padding: 20px;
                border-radius: 8px;
                box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
                max-width: 600px;
                margin: auto;
            }}
            .content {{
                padding: 20px;
            }}
            .reason {{
                color: #d9534f;
                font-weight: bold;
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='content'>
                <h2>Service Request Rejected</h2>
                <p>Dear {userName},</p>
                <p>We regret to inform you that your service request has been rejected.</p>
                <p><strong>Service ID:</strong> {serviceId}</p>
                <p><strong>Service Name:</strong> {serviceName}</p>
                <p><strong>Reason for Rejection:</strong> <span class='reason'>{reason}</span></p>
                <p>If you have any questions or need further clarification, please contact our support team.</p>
                <p>Best regards,<br> The Team</p>
            </div>
        </div>
    </body>
    </html>";
        }

        public static string GetRejectionEmailForHousekeeper(string userName, string reason)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Housekeeper Registration Rejected</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            text-align: center;
            padding: 20px;
        }}
        .container {{
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
            max-width: 600px;
            margin: auto;
        }}
        .content {{
            padding: 20px;
        }}
        .reason {{
            color: #d9534f;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='content'>
            <h2>Housekeeper Registration Rejected</h2>
            <p>Dear {userName},</p>
            <p>We regret to inform you that your application to become a housekeeper has been rejected.</p>
            <p><strong>Reason for Rejection:</strong> <span class='reason'>{reason}</span></p>
            <p>If you have any questions or need further clarification, please contact our support team.</p>
            <p>Best regards,<br> The Team</p>
        </div>
    </div>
</body>
</html>";
        }

        public static string GetApprovalEmailForHousekeeper(string userName)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Housekeeper Registration Approved</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            text-align: center;
            padding: 20px;
        }}
        .container {{
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
            max-width: 600px;
            margin: auto;
        }}
        .content {{
            padding: 20px;
        }}
        .button {{
            display: inline-block;
            padding: 10px 20px;
            font-size: 16px;
            color: #ffffff;
            background-color: #28a745;
            text-decoration: none;
            border-radius: 5px;
            margin-top: 15px;
        }}
        .button:hover {{
            background-color: #218838;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='content'>
            <h2>Housekeeper Registration Approved</h2>
            <p>Dear {userName},</p>
            <p>Congratulations! Your application to become a housekeeper has been approved.</p>
            <p>You can now log in and visit your dashboard to start receiving job requests.</p>
            <a href='[YOUR_LOGIN_URL]' class='button'>Go to Dashboard</a>
            <p>Best regards,<br> The Team</p>
        </div>
    </div>
</body>
</html>";
        }

        public static string ApproveRefundRequestForCustomer(string housekeeperName, string customerName, Guid bookingId, string serviceName, decimal refundPrice)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Refund Request Approved</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            text-align: center;
            padding: 20px;
        }}
        .container {{
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
            max-width: 600px;
            margin: auto;
        }}
        .content {{
            padding: 20px;
        }}
        .highlight {{
            color: #28a745;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='content'>
            <h2>Refund Request Approved</h2>
            <p>Dear {customerName},</p>
            <p>We are pleased to inform you that your refund request has been approved.</p>
            <p><strong>Booking ID:</strong> <span class='highlight'>{bookingId}</span></p>
            <p><strong>Service:</strong> {serviceName}</p>
            <p><strong>Housekeeper:</strong> {housekeeperName}</p>
            <p><strong>Refund Amount:</strong> <span class='highlight'>{refundPrice}</span></p>
            <p>The refunded amount will be credited back to your wallet shortly.</p>
            <p>If you have any questions, please feel free to contact our support team.</p>
            <p>Best regards,<br> The Team</p>
        </div>
    </div>
</body>
</html>";
        }

        public static string RejectRefundRequestForCustomer(string housekeeperName, string customerName, Guid bookingId, string serviceName, decimal refundPrice, string reason)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Refund Request Rejected</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            text-align: center;
            padding: 20px;
        }}
        .container {{
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
            max-width: 600px;
            margin: auto;
        }}
        .content {{
            padding: 20px;
        }}
        .highlight {{
            color: #d9534f;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='content'>
            <h2>Refund Request Rejected</h2>
            <p>Dear {customerName},</p>
            <p>We regret to inform you that your refund request has been rejected.</p>
            <p><strong>Booking ID:</strong> <span class='highlight'>{bookingId}</span></p>
            <p><strong>Service:</strong> {serviceName}</p>
            <p><strong>Housekeeper:</strong> {housekeeperName}</p>
            <p><strong>Requested Refund Amount:</strong> <span class='highlight'>{refundPrice}</span></p>
            <p><strong>Reason for Rejection:</strong> <span class='highlight'>{reason}</span></p>
            <p>If you have any questions or need further clarification, please contact our support team.</p>
            <p>Best regards,<br> The Team</p>
        </div>
    </div>
</body>
</html>";
        }
        public static string RejectRefundRequestForHousekeeper(string housekeeperName, string customerName, Guid bookingId, string serviceName, decimal refundPrice, string reason)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Refund Request Confirmed</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            text-align: center;
            padding: 20px;
        }}
        .container {{
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
            max-width: 600px;
            margin: auto;
        }}
        .content {{
            padding: 20px;
        }}
        .highlight {{
            color: #d9534f;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='content'>
            <h2>Refund Request Confirmed</h2>
            <p>Dear {housekeeperName},</p>
            <p>The refund request for a recent booking has been confirmed, and you are responsible for the refund payment.</p>
            <p><strong>Booking ID:</strong> <span class='highlight'>{bookingId}</span></p>
            <p><strong>Service:</strong> {serviceName}</p>
            <p><strong>Customer:</strong> {customerName}</p>
            <p><strong>Refund Amount:</strong> <span class='highlight'>{refundPrice}</span></p>
            <p><strong>Reason for Refund:</strong> <span class='highlight'>{reason}</span></p>
            <p>Please ensure that the necessary payment is processed promptly. If you have any concerns or questions, contact our support team.</p>
            <p>Best regards,<br> The Team</p>
        </div>
    </div>
</body>
</html>";
        }


    }
}
