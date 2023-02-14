using ApplicationLayerLibA.Security;
using Authorization.PolicyForT.Requirements;
using MediatR;

namespace ApplicationLayerLibA.Commands;

public static class CreateCustomer
{
    public class Command : IRequest<int>
    {
        public required string Name { get; set; }
    }

    public class Policy : AbstractPolicy<Command>
    {
        //public Policy() => Require2("");
        public Policy()
        {
            Requirements = Require.Permissions(Permission.CanCreateCustomer);
        }
    }

    public class Handler : IRequestHandler<Command, int>
    {
        public Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            // do work

            return Task.FromResult(1);
        }
    }
}
