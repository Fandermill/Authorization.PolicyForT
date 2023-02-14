using ApplicationLayerLibA;
using ApplicationLayerLibA.Commands;
using Authorization.PolicyForT.Extensions.DependencyInjection;
using MediatR;
using SampleApp;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging();

var applicationLayerLibA = typeof(IApplicationLayerATag).Assembly;
builder.Services.AddMediatR(applicationLayerLibA);
builder.Services.AddPolicyForT<IBaseRequest>(applicationLayerLibA);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/create-customer", (IMediator mediator) => mediator.Send(new CreateCustomer.Command { Name = "Test" }));

app.Run();
