//using HCP.Repository.Entities;
//using HCP.Repository.Interfaces;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace HCP.Service.Services.NotificationService
//{
//    public class NotificationService
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly UserManager<AppUser> _userManager;

//        public NotificationService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager )
//        {
//            _unitOfWork = unitOfWork;
//            _userManager = userManager;
//        }
//        public async Task<Notification> SaveNotificationAsync(string userId, string content, string? returnUrl)
//        {
//            var notification = new Notification
//            {
//                Id = Guid.NewGuid(),
//                UserId = userId,
//                Content = content,
//                ReturnUrl = returnUrl,
//                IsRead = false,
//                CreatedDate = DateTime.UtcNow
//            };
//            _context.Notifications.Add(notification);
//            await _context.SaveChangesAsync();
//            return notification;
//        }

//        public async Task SendFcmNotificationAsync(string userId, string content)
//        {
//            var user = await _userManager.FindByIdAsync(userId);
//            if (user?.FcmToken == null) return;

//            var message = new FirebaseAdmin.Messaging.Message
//            {
//                Token = user.FcmToken,
//                Notification = new FirebaseAdmin.Messaging.Notification
//                {
//                    Title = "New Booking",
//                    Body = content
//                }
//            };
//            await FirebaseMessaging.DefaultInstance.SendAsync(message);
//        }

//        public async Task<List<Notification>> GetUserNotificationsAsync(string userId)
//        {
//            return await _context.Notifications
//                .Where(n => n.UserId == userId)
//                .OrderByDescending(n => n.CreatedDate)
//                .Take(10)
//                .ToListAsync();
//        }
    

//}
//}
