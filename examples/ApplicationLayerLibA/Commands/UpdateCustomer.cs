using ApplicationLayerLibA.Security;
using Authorization.PolicyForT.Requirements;
using MediatR;

namespace ApplicationLayerLibA.Commands;

public static class UpdateCustomer
{
    public class Command : IRequest<int>
    {
        public required int Id { get; set; }
        public required string NewName { get; set; }
    }

    public class Policy : AbstractPolicy<Command>
    {
        public Policy()
        {
            Requirements = Require.Permissions(Permission.CanUpdateCustomer);
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
