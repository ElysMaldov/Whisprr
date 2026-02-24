using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(busConfigurator =>
{
  busConfigurator.SetKebabCaseEndpointNameFormatter();
  busConfigurator.UsingRabbitMq((context, configurator) =>
  {
    configurator.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), h =>
    {
      h.Username(builder.Configuration["MessageBroker:Username"]!);
      h.Password(builder.Configuration["MessageBroker:Password"]!);
    });

    configurator.ConfigureEndpoints(context);
  });
});

var host = builder.Build();
host.Run();
