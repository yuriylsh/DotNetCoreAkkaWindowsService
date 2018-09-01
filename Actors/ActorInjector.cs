using Akka.Actor;
using Akka.DI.Core;

namespace DotNetCoreAkkaWindowsService.Actors
{
    public interface IActorInjector
    {
        IActorRef GetActor<TActor>(IActorContext context, string actorName) where TActor: ActorBase;
    }

    public class ActorInjector: IActorInjector
    {
        public IActorRef GetActor<TActor>(IActorContext context, string actorName)
            where TActor: ActorBase
        {
            return context.ActorOf(context.DI().Props<TActor>(), actorName);
        }
    }
}