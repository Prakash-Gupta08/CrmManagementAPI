using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model.Dashboard;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static CrmManagementAPI.Model.Dashboard.TenderCommandCentreResponse;
using TenderCommandCentreResponse = CrmManagementAPI.Model.Dashboard.TenderCommandCentreResponse;

namespace CrmManagementAPI.Services
{
    public class TenderCommandCentreService : ITenderCommandCentreService
    {
        private readonly db_context _context;
        // A tender in one of these stages is no longer "active"
        private static readonly string[] ClosedStatuses = { "Won", "Lost", "Cancelled" };
        public TenderCommandCentreService(db_context context)
        {
            _context = context;
        }

        private static APIResponse Ok(object result, string message = "Data found successfully.")
        {
            return new APIResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                ActionResponse = message,
                Result = result
            };
        }

        private static DateOnly Today() => DateOnly.FromDateTime(DateTime.Today);

        private static string Inr(decimal value)
        {
            var abs = Math.Abs(value);
            if (abs >= 10000000m) return $"₹{value / 10000000m:0.00} Cr";
            if (abs >= 100000m) return $"₹{value / 100000m:0.00} L";
            return $"₹{value:0}";
        }

        private static decimal Pct(decimal part, decimal whole)
            => whole <= 0 ? 0 : Math.Round(part * 100m / whole, 1);
        private async Task<TccSummaryResponse> BuildSummary()
        {
            var now = DateTime.Now;
            var in7Days = now.AddDays(7);

            var active = _context.tenders.Where(t => !ClosedStatuses.Contains(t.BidStatus));

            var activeCount = await active.CountAsync();
            var activePipeline = await active.SumAsync(t => t.EstimatedTenderValueInr ?? 0);

            // weighted pipeline = estimated value x win probability %
            var weighted = await active.SumAsync(t => (t.EstimatedTenderValueInr ?? 0) * (t.WinProbabilityPct ?? 0) / 100m);

            var wonQuery = _context.tenders.Where(t => t.BidStatus == "Won");
            var wonCount = await wonQuery.CountAsync();
            var wonValue = await wonQuery.SumAsync(t => t.EstimatedTenderValueInr ?? 0);

            var lostCount = await _context.tenders.CountAsync(t => t.BidStatus == "Lost");

            var bidsDue = await active.CountAsync(t => t.BidSubmissionDeadline != null &&t.BidSubmissionDeadline >= now &&
                t.BidSubmissionDeadline <= in7Days);

            var pendingGoNoGo = await _context.go_nogo_approvals.CountAsync(a =>
                a.Decision == null || a.Decision == "" || a.Decision == "Pending");

            return new TccSummaryResponse
            {
                ActiveTenders = activeCount,
                ActivePipelineInr = activePipeline,
                ActivePipelineText = $"{Inr(activePipeline)} pipeline",
                WeightedPipelineInr = weighted,
                WeightedPipelineText = Inr(weighted),
                WonCount = wonCount,
                WonValueInr = wonValue,
                WonValueText = Inr(wonValue),
                LostCount = lostCount,
                WinRatePct = Pct(wonCount, wonCount + lostCount),
                BidsDueIn7Days = bidsDue,
                PendingGoNoGo = pendingGoNoGo
            };
        }

        //Pipeline value by stage (all tenders)
        private async Task<List<TccStageValue>> BuildPipelineByStage()
        {
            var rows = await _context.tenders.GroupBy(t => t.BidStatus)
                .Select(g => new
                {
                    Stage = g.Key,
                    Count = g.Count(),
                    Value = g.Sum(x => x.EstimatedTenderValueInr ?? 0)
                }).OrderByDescending(x => x.Value).ToListAsync();

            return rows.Select(r => new TccStageValue
            {
                Stage = r.Stage,
                Count = r.Count,
                ValueInr = r.Value,
                ValueText = Inr(r.Value)
            }).ToList();
        }


        //Active pipeline by customer category   (tenders JOIN customers)
        private async Task<TccCategoryPipelineResponse> BuildPipelineByCategory()
        {
            var rows = await (from t in _context.tenders
                join c in _context.customers on t.CustomerId equals c.Id
                where t.BidStatus != "Lost" && t.BidStatus != "Cancelled"
                group t by c.Category into g
                select new
                {
                    Category = g.Key,
                    Value = g.Sum(x => x.EstimatedTenderValueInr ?? 0)
                })
                .OrderByDescending(x => x.Value).ToListAsync();

            var total = rows.Sum(r => r.Value);

            return new TccCategoryPipelineResponse
            {
                TotalInr = total,
                TotalText = Inr(total),
                Items = rows.Select(r => new TccCategoryValue
                {
                    Category = string.IsNullOrWhiteSpace(r.Category) ? "Other" : r.Category!,
                    ValueInr = r.Value,
                    ValueText = Inr(r.Value),
                    SharePct = Math.Round(Pct(r.Value, total), 0)
                }).ToList()
            };
        }


        //Upcoming bid deadlines(tenders JOIN customers LEFT JOIN users)3
        private async Task<List<TccDeadlineItem>> BuildUpcomingDeadlines(int top)
        {
            var startOfToday = DateTime.Today;

            var rows = await (from t in _context.tenders
                join c in _context.customers on t.CustomerId equals c.Id
                join u in _context.users on t.BidOwnerId equals (int?)u.Id into ownerJoin
                from owner in ownerJoin.DefaultIfEmpty()
                where !ClosedStatuses.Contains(t.BidStatus)&& t.BidSubmissionDeadline != null&& t.BidSubmissionDeadline >= startOfToday
                orderby t.BidSubmissionDeadline
                select new
                {
                    t.Id,
                    t.TenderNumber,
                    t.TenderTitle,
                    Customer = c.OrganizationName,
                    Owner = owner != null ? owner.FullName : null,
                    t.BidSubmissionDeadline,
                    t.BidStatus,
                    Value = t.EstimatedTenderValueInr ?? 0,
                    Win = t.WinProbabilityPct ?? 0
                }).Take(top).ToListAsync();

            return rows.Select(r => new TccDeadlineItem
            {
                TenderId = r.Id,
                TenderNumber = r.TenderNumber,
                TenderTitle = r.TenderTitle,
                Customer = r.Customer,
                Owner = r.Owner,
                Deadline = r.BidSubmissionDeadline,
                DaysLeft = (int)Math.Ceiling((r.BidSubmissionDeadline!.Value - DateTime.Now).TotalDays),
                BidStatus = r.BidStatus,
                ValueInr = r.Value,
                ValueText = Inr(r.Value),
                WinProbabilityPct = r.Win
            }).ToList();
        }

        //  Go / No-Go queue
        //    approvals JOIN tenders JOIN customers JOIN users(requester) JOIN users(approver)
        private async Task<List<TccGoNoGoItem>> BuildGoNoGoQueue(int top)
        {
            var rows = await (from a in _context.go_nogo_approvals
                join t in _context.tenders on a.TenderId equals t.Id
                join c in _context.customers on t.CustomerId equals c.Id
                join req in _context.users on a.RequestedById equals req.Id
                join apr in _context.users on a.ApproverId equals apr.Id
                orderby (a.Decision == null || a.Decision == "" || a.Decision == "Pending") ? 0 : 1,
                        a.RequestDate descending
                select new
                {
                    a.Id,
                    t.TenderNumber,
                    t.TenderTitle,
                    Customer = c.OrganizationName,
                    Value = a.TenderValueInr ?? t.EstimatedTenderValueInr ?? 0,
                    a.ExpectedMarginPct,
                    a.WinProbabilityPct,
                    RequestedBy = req.FullName,
                    Approver = apr.FullName,
                    a.RequestDate,
                    a.Decision,
                    a.ProjectRisk
                }).Take(top).ToListAsync();

            return rows.Select(r => new TccGoNoGoItem
            {
                ApprovalId = r.Id,
                TenderNumber = r.TenderNumber,
                TenderTitle = r.TenderTitle,
                Customer = r.Customer,
                ValueInr = r.Value,
                ValueText = Inr(r.Value),
                MarginPct = r.ExpectedMarginPct,
                WinProbabilityPct = r.WinProbabilityPct,
                RequestedBy = r.RequestedBy,
                Approver = r.Approver,
                RequestDate = r.RequestDate,
                Decision = string.IsNullOrWhiteSpace(r.Decision) ? "Pending" : r.Decision!,
                ProjectRisk = r.ProjectRisk
            }).ToList();
        }


        //Recent bid team activities   (tender_activities JOIN tenders LEFT JOIN users)
        private async Task<List<TccActivityItem>> BuildRecentActivities(int top)
        {
            return await (from act in _context.tender_activities
                join t in _context.tenders on act.TenderId equals t.Id
                join u in _context.users on act.OwnerId equals (int?)u.Id into ownerJoin
                from owner in ownerJoin.DefaultIfEmpty()
                orderby act.ActivityDate descending
                select new TccActivityItem
                {
                    ActivityId = act.Id,
                    TenderNumber = t.TenderNumber,
                    TenderTitle = t.TenderTitle,
                    Owner = owner != null ? owner.FullName : null,
                    ActivityType = act.ActivityType,
                    ActivityDate = act.ActivityDate,
                    Outcome = act.Outcome,
                    NextAction = act.NextAction,
                    NextActionDate = act.NextActionDate
                }).Take(top).ToListAsync();
        }


        //Execution - Billing - Collection cards   (purchase_orders + invoices)
        private async Task<TccBillingSummaryResponse> BuildBillingSummary()
        {
            var today = Today();

            var activePos = await _context.purchase_orders.CountAsync(p => p.PoStatus == "Active");

            var billed = await _context.invoices.SumAsync(i => i.TotalInvoiceValueInr);
            var collected = await _context.invoices.SumAsync(i => i.PaymentReceivedInr);
            var outstanding = await _context.invoices.SumAsync(i => i.OutstandingAmountInr);

            var overdue = await _context.invoices .Where(i => i.OutstandingAmountInr > 0 && i.PaymentDueDate != null && i.PaymentDueDate < today)
                .SumAsync(i => i.OutstandingAmountInr);

            var disputed = await _context.invoices.Where(i => i.InvoiceStatus == "Disputed")
                .SumAsync(i => i.OutstandingAmountInr);

            return new TccBillingSummaryResponse
            {
                ActivePos = activePos,
                TotalBilledInr = billed,
                TotalBilledText = Inr(billed),
                CollectedInr = collected,
                CollectedText = Inr(collected),
                OutstandingInr = outstanding,
                OutstandingText = Inr(outstanding),
                OverdueInr = overdue,
                OverdueText = Inr(overdue),
                DisputedInr = disputed,
                DisputedText = Inr(disputed)
            };
        }

        // PO register purchase_orders JOIN customers LEFT JOIN users LEFT JOIN tenders
        //    + billed / collected per PO from invoices (GROUP BY po_id)
        private async Task<List<TccPoRegisterItem>> BuildPoRegister(int top)
        {
            var invoiceTotals = await _context.invoices.GroupBy(i => i.PoId)
                .Select(g => new
                {
                    PoId = g.Key,
                    Billed = g.Sum(x => x.TotalInvoiceValueInr),
                    Collected = g.Sum(x => x.PaymentReceivedInr),
                    Outstanding = g.Sum(x => x.OutstandingAmountInr)
                })
                .ToDictionaryAsync(x => x.PoId);

            var pos = await (
                from p in _context.purchase_orders
                join c in _context.customers on p.CustomerId equals c.Id
                join u in _context.users on p.AccountOwnerId equals (int?)u.Id into ownerJoin
                from owner in ownerJoin.DefaultIfEmpty()
                join t in _context.tenders on p.TenderId equals (int?)t.Id into tenderJoin
                from tender in tenderJoin.DefaultIfEmpty()
                orderby p.PoStatus == "Active" ? 0 : 1, p.PoDate descending
                select new
                {
                    p.Id,
                    p.PoNumber,
                    p.ProjectName,
                    Customer = c.OrganizationName,
                    p.PoDate,
                    Owner = owner != null ? owner.FullName : null,
                    TenderNumber = tender != null ? tender.TenderNumber : null,
                    p.PbgValidityDate,
                    p.PoStatus,
                    p.PoValueInr
                })
                .Take(top).ToListAsync();

            return pos.Select(p =>
            {
                invoiceTotals.TryGetValue(p.Id, out var inv);
                var billed = inv?.Billed ?? 0;
                var collected = inv?.Collected ?? 0;
                var outstanding = inv?.Outstanding ?? 0;

                return new TccPoRegisterItem
                {
                    PoId = p.Id,
                    PoNumber = p.PoNumber,
                    ProjectName = p.ProjectName,
                    Customer = p.Customer,
                    PoDate = p.PoDate,
                    Owner = p.Owner,
                    TenderNumber = p.TenderNumber,
                    PbgValidityDate = p.PbgValidityDate,
                    PoStatus = p.PoStatus,
                    PoValueInr = p.PoValueInr,
                    BilledInr = billed,
                    CollectedInr = collected,
                    OutstandingInr = outstanding,
                    CollectedPct = Math.Round(Pct(collected, p.PoValueInr), 0),
                    PoValueText = Inr(p.PoValueInr),
                    BilledText = Inr(billed),
                    CollectedText = Inr(collected),
                    OutstandingText = Inr(outstanding)
                };
            }).ToList();
        }

        //Accounts receivable aging (overdue amount by bucket)
        private async Task<List<TccAgingBucket>> BuildReceivableAging()
        {
            var today = Today();

            var overdue = await _context.invoices
                .Where(i => i.OutstandingAmountInr > 0 && i.PaymentDueDate != null && i.PaymentDueDate < today)
                .Select(i => new { i.PaymentDueDate, i.OutstandingAmountInr }).ToListAsync();

            var withDays = overdue
                .Select(i => new
                {
                    Days = today.DayNumber - i.PaymentDueDate!.Value.DayNumber,
                    Amount = i.OutstandingAmountInr
                }).ToList();

            TccAgingBucket MakeBucket(string name, int from, int to)
            {
                var items = withDays.Where(x => x.Days >= from && x.Days <= to).ToList();
                var amount = items.Sum(x => x.Amount);
                return new TccAgingBucket
                {
                    Bucket = name,
                    InvoiceCount = items.Count,
                    AmountInr = amount,
                    AmountText = Inr(amount)
                };
            }

            return new List<TccAgingBucket>
            {
                MakeBucket("0-30d", 0, 30),
                MakeBucket("31-60d", 31, 60),
                MakeBucket("61-90d", 61, 90),
                MakeBucket("90d+", 91, int.MaxValue)
            };
        }


        //Outstanding invoices   (invoices JOIN customers JOIN purchase_orders)
        private async Task<List<TccOutstandingInvoiceItem>> BuildOutstandingInvoices(int top)
        {
            var today = Today();

            var rows = await (from i in _context.invoices
                join c in _context.customers on i.CustomerId equals c.Id
                join p in _context.purchase_orders on i.PoId equals p.Id
                where i.OutstandingAmountInr > 0
                orderby i.PaymentDueDate == null, i.PaymentDueDate
                select new
                {
                    i.Id,
                    i.InvoiceNumber,
                    Customer = c.OrganizationName,
                    p.PoNumber,
                    i.PaymentDueDate,
                    i.InvoiceStatus,
                    i.TotalInvoiceValueInr,
                    i.PaymentReceivedInr,
                    i.OutstandingAmountInr
                })
                .Take(top).ToListAsync();

            return rows.Select(r =>
            {
                var diff = r.PaymentDueDate.HasValue? today.DayNumber - r.PaymentDueDate.Value.DayNumber: 0;

                return new TccOutstandingInvoiceItem
                {
                    InvoiceId = r.Id,
                    InvoiceNumber = r.InvoiceNumber,
                    Customer = r.Customer,
                    PoNumber = r.PoNumber,
                    DueDate = r.PaymentDueDate,
                    DaysOverdue = diff > 0 ? diff : 0,
                    DaysToGo = diff < 0 ? -diff : 0,
                    InvoiceStatus = r.InvoiceStatus,
                    InvoiceValueInr = r.TotalInvoiceValueInr,
                    ReceivedInr = r.PaymentReceivedInr,
                    OutstandingInr = r.OutstandingAmountInr,
                    InvoiceValueText = Inr(r.TotalInvoiceValueInr),
                    ReceivedText = Inr(r.PaymentReceivedInr),
                    OutstandingText = Inr(r.OutstandingAmountInr)
                };
            }).ToList();
        }

        //Upcoming project milestones project_milestones JOIN purchase_orders JOIN customers
        private async Task<List<TccMilestoneItem>> BuildUpcomingMilestones(int top)
        {
            var today = Today();

            var rows = await (from m in _context.project_milestones
                join p in _context.purchase_orders on m.PoId equals p.Id
                join c in _context.customers on p.CustomerId equals c.Id
                where m.Status != "Completed"
                orderby m.PlannedDate == null, m.PlannedDate
                select new
                {
                    m.Id,
                    m.MilestoneName,
                    m.MilestoneType,
                    p.PoNumber,
                    Customer = c.OrganizationName,
                    m.PlannedDate,
                    m.Status,
                    m.Billable,
                    Unlocks = m.BillingAmountInr ?? 0
                })
                .Take(top).ToListAsync();

            return rows.Select(r =>
            {
                var days = r.PlannedDate.HasValue ? r.PlannedDate.Value.DayNumber - today.DayNumber : 0;
                var unlocks = r.Billable ? r.Unlocks : 0;

                return new TccMilestoneItem
                {
                    MilestoneId = r.Id,
                    MilestoneName = r.MilestoneName,
                    MilestoneType = r.MilestoneType,
                    PoNumber = r.PoNumber,
                    Customer = r.Customer,
                    PlannedDate = r.PlannedDate,
                    DaysLeft = days,
                    IsOverdue = r.PlannedDate.HasValue && days < 0,
                    UnlocksInr = unlocks,
                    UnlocksText = Inr(unlocks),
                    Status = r.Status
                };
            }).ToList();
        }

        //Tenders by portal
        private async Task<TccPortalResponse> BuildTendersByPortal()
        {
            var rows = await _context.tenders.GroupBy(t => t.TenderPortal)
                .Select(g => new { Portal = g.Key, Count = g.Count() }).ToListAsync();

            // merge NULL / empty portal into "Unknown"
            var merged = rows.GroupBy(r => string.IsNullOrWhiteSpace(r.Portal) ? "Unknown" : r.Portal!)
                .Select(g => new { Portal = g.Key, Count = g.Sum(x => x.Count) }).OrderByDescending(x => x.Count)
                .ToList();

            var total = merged.Sum(x => x.Count);

            return new TccPortalResponse
            {
                Total = total,
                Items = merged.Select(x => new TccPortalItem
                {
                    Portal = x.Portal,
                    Count = x.Count,
                    SharePct = Math.Round(Pct(x.Count, total), 0)
                }).ToList()
            };
        }
        //Public methods (what the controller calls)
        public async Task<APIResponse> GetSummary() => Ok(await BuildSummary());
        public async Task<APIResponse> GetPipelineByStage() => Ok(await BuildPipelineByStage());
        public async Task<APIResponse> GetPipelineByCategory() => Ok(await BuildPipelineByCategory());
        public async Task<APIResponse> GetUpcomingDeadlines(int top = 15) => Ok(await BuildUpcomingDeadlines(top));
        public async Task<APIResponse> GetGoNoGoQueue(int top = 10) => Ok(await BuildGoNoGoQueue(top));
        public async Task<APIResponse> GetRecentActivities(int top = 10) => Ok(await BuildRecentActivities(top));
        public async Task<APIResponse> GetBillingSummary() => Ok(await BuildBillingSummary());
        public async Task<APIResponse> GetPoRegister(int top = 20) => Ok(await BuildPoRegister(top));
        public async Task<APIResponse> GetReceivableAging() => Ok(await BuildReceivableAging());
        public async Task<APIResponse> GetOutstandingInvoices(int top = 15) => Ok(await BuildOutstandingInvoices(top));
        public async Task<APIResponse> GetUpcomingMilestones(int top = 10) => Ok(await BuildUpcomingMilestones(top));
        public async Task<APIResponse> GetTendersByPortal() => Ok(await BuildTendersByPortal());

        public async Task<APIResponse> GetAll()
        {
        // NOTE: one DbContext cannot run queries in parallel, so these run one after another.
            var result = new TenderCommandCentreResponse
            {
                Summary = await BuildSummary(),
                PipelineByStage = await BuildPipelineByStage(),
                PipelineByCategory = await BuildPipelineByCategory(),
                UpcomingDeadlines = await BuildUpcomingDeadlines(15),
                GoNoGoQueue = await BuildGoNoGoQueue(10),
                RecentActivities = await BuildRecentActivities(10),
                Billing = await BuildBillingSummary(),
                PoRegister = await BuildPoRegister(20),
                ReceivableAging = await BuildReceivableAging(),
                OutstandingInvoices = await BuildOutstandingInvoices(15),
                UpcomingMilestones = await BuildUpcomingMilestones(10),
                TendersByPortal = await BuildTendersByPortal()
            };

            return Ok(result, "Tender Command Centre data found successfully.");
        }
    }
}
