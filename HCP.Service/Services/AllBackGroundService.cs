using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Repository.Enums;

namespace HCP.Service.Services
{
    public class AllBackGroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AllBackGroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5); // Check every 5 minutes
        private readonly TimeSpan _expirationDuration = TimeSpan.FromMinutes(15); // Expire after 15 minutes

        public AllBackGroundService(
            IServiceProvider serviceProvider,
            ILogger<AllBackGroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Deposit Transaction Expiration Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndExpireDepositTransactions(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while checking and expiring deposit transactions.");
                }

                // Wait for the next check interval
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Deposit Transaction Expiration Service stopped.");
        }

        private async Task CheckAndExpireDepositTransactions(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            _logger.LogInformation("Starting check for pending deposit transactions...");

            var now = DateTime.UtcNow;
            var expirationThreshold = now.Subtract(_expirationDuration); // Calculate the threshold once

            var pendingDeposits = unitOfWork.Repository<WalletTransaction>()
                .GetAll()
                .Where(t => t.Type == TransactionType.Deposit.ToString() &&
                            ((t.Status == TransactionStatus.Pending.ToString() &&
                            t.CreatedDate <= expirationThreshold) || t.Status == TransactionStatus.Fail.ToString())) // Compare directly
                .ToList();

            if (!pendingDeposits.Any())
            {
                _logger.LogInformation("No pending deposit transactions to expire at this time.");
                return;
            }

            _logger.LogInformation($"Found {pendingDeposits.Count} pending deposit transactions to expire.");

            foreach (var transaction in pendingDeposits)
            {
                try
                {
                    _logger.LogInformation($"Processing transaction {transaction.Id}, CreatedDate: {transaction.CreatedDate}, Age: {(now - transaction.CreatedDate).TotalMinutes} minutes");


                    unitOfWork.Repository<WalletTransaction>().Delete(transaction);
                    await unitOfWork.SaveChangesAsync();

                    _logger.LogInformation($"Expired deposit transaction {transaction.Id} for user {transaction.UserId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to expire deposit transaction {transaction.Id}");
                }
            }
        }
    }
}