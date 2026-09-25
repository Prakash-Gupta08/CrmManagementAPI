using CrmManagementAPI.AppDbContext;
using Microsoft.EntityFrameworkCore;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.AppDbContext;
using CrmManagementAPI.Services;




var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext (IMPORTANT: configure connection string)
builder.Services.AddDbContext<db_context>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MySqlConn")
    )
);

// Dependency Injection
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ILeadService, LeadService>();
builder.Services.AddScoped<ITenderService, TenderService>();
builder.Services.AddScoped<ITenderDocumentService, TenderDocumentService>();
builder.Services.AddScoped<ITenderAlertService, TenderAlertService>();
builder.Services.AddScoped<ITenderActivityService, TenderActivityService>();
builder.Services.AddScoped<IGoNoGoApprovalService, GoNoGoApprovalService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<IProjectMilestoneService, ProjectMilestoneService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IMessageTemplateService, MessageTemplateService>();
builder.Services.AddScoped<IMessageOutboxService, MessageOutboxService>();
builder.Services.AddScoped<IEscalationPolicyService, EscalationPolicyService>();
builder.Services.AddScoped<ILeadActivityService, LeadActivityService>();
builder.Services.AddScoped<ILeadConversionService, LeadConversionService>(); 
builder.Services.AddScoped<IOverviewService, OverviewService>();

var app = builder.Build();

// Enable Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();