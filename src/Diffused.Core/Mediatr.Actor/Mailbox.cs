﻿using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using MediatR;

namespace Diffused.Core.Mediatr.Actor
{
    public class Mailbox<TRequest> : IMailbox
    {
        private readonly ActionBlock<TRequest> queue;

        public Mailbox(IMediator mediator, CancellationToken cancellationToken)
        {
            queue = new ActionBlock<TRequest>(async request => await mediator.Send((IRequest<ActorResult>) request,cancellationToken));
        }

        public Task<bool> Send(TRequest request)
        {
            var actorRequest = request as ActorRequest;
            actorRequest?.SetHandleNow();

            return queue.SendAsync(request);
        }

        public Task<bool> Post(TRequest request)
        {
            var actorRequest = request as ActorRequest;
            actorRequest?.SetHandleNow();

            return Task.FromResult(queue.Post(request));
        }

        public void Complete() => queue.Complete();

        public Task Completion => queue.Completion;

    }
}