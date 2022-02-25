using Grafana_Prometheus.Interfaces;
using Grafana_Prometheus.Services;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

#region Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("local", builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
#endregion

#region Configurations
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseCors("local");
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

#region Prometheus
var counter = Metrics.CreateCounter("mymetrics_call_requests_count", "Quantidade de Requisições nos Endpoints", "method", "endpoint", "code");
var exceptions = Metrics.CreateCounter("mymetrics_exceptions_requests_count", "Quantidade de Exceptions nas Requisições", "method", "endpoint", "message", "inner");
var timer = Metrics.CreateSummary("mymetrics_timer_request", "Tempo de Execução das Requisições", "method", "endpoint");

app.Use(async (context, next) =>
{
    var method = context.Request.Method;
    var path = context.Request.Path;
    try
    {
        using (timer.WithLabels(method, path).NewTimer())
        {
            await next.Invoke();
        }
        counter.WithLabels(method, path, context.Response.StatusCode.ToString()).Inc();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        exceptions.WithLabels(method, path, ex.Message, ex.InnerException?.Message ?? "").Inc();
    }
});

app.UseMetricServer();
app.UseHttpMetrics();
#endregion

#endregion

app.Run();
