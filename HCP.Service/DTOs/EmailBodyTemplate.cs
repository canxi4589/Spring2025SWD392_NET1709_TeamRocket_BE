using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.DTOs
{
    public class EmailBodyTemplate
    {
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

    }
}
