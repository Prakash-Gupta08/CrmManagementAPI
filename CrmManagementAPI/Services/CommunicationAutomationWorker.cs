using CrmManagementAPI.Common;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace CrmManagementAPI.Services
{
    /// <summary>
    /// Runs the "Automation" tab jobs every day at the configured IST time:
    ///   - Auto Payment Reminders      (default 09:30 IST)
    ///   - Auto Tender Deadline Alerts (default 08:00 IST)
    /// The worker wakes up every 30 seconds, and runs a job once per day when its time has come.
    /// Duplicate protection (re-send every N days / each tender+window once) is inside the service,
    /// so a restart of the application never double-sends.
    /// </summary>
    public class CommunicationAutomationWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly AutomationSettings _settings;
        private readonly ILogger<CommunicationAutomationWorker> _logger;

        public CommunicationAutomationWorker(
            IServiceScopeFactory scopeFactory,
            IOptions<AutomationSettings> options,
            ILogger<CommunicationAutomationWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _settings = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var lastRun = new Dictionary<string, DateOnly>();

            try
            {
                // give the application / database a moment to start
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = CommunicationsConsoleService.IstNow();

                    await RunIfDue("payment", _settings.PaymentReminders, now, lastRun,
                        svc => svc.RunPaymentReminderAutomation());

                    await RunIfDue("tender", _settings.TenderAlerts, now, lastRun,
                        svc => svc.RunTenderAlertAutomation());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Communication automation loop failed.");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private async Task RunIfDue(
            string key,
            AutomationJobSettings job,
            DateTime now,
            Dictionary<string, DateOnly> lastRun,
            Func<ICommunicationsConsoleService, Task<APIResponse>> action)
        {
            if (!job.Enabled) return;

            var runAt = TimeSpan.TryParse(job.RunAtIst, CultureInfo.InvariantCulture, out var ts)
                ? ts
                : new TimeSpan(9, 0, 0);

            var today = DateOnly.FromDateTime(now);
            if (lastRun.TryGetValue(key, out var done) && done == today) return;
            if (now.TimeOfDay < runAt) return;

            lastRun[key] = today;

            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ICommunicationsConsoleService>();

            var response = await action(service);
            _logger.LogInformation("Automation job '{Job}' finished: {Message}", key, response.ActionResponse);
        }
    }
}