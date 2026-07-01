using MassTransit;
using MessageService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    // Register your new consumer class
    x.AddConsumer<PatientRegisteredConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("admin");
            h.Password("admin123");
        });
        
        cfg.UseMessageRetry(r =>
        {
            r.Interval(5, TimeSpan.FromSeconds(5));
        });

        cfg.ReceiveEndpoint("message-service", e =>
        {
            e.ConfigureConsumers(context);
        });
    });
});

var host = builder.Build();
host.Run();