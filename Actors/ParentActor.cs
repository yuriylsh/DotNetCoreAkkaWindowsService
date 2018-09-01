using Akka.Actor;
using Akka.Event;

namespace DotNetCoreAkkaWindowsService.Actors
{
    public class ParentActor : ReceiveActor
    {
        public ParentActor(IActorInjector actorInjector)
        {
            var child = actorInjector.GetActor<ChildActor>(Context, "child_actor");
            Receive<object>(msg =>
            {
                Context.GetLogger()
                    .Info(
                        $"{nameof(ParentActor)} says: received a message {msg}.");
                child.Tell("blah");
            });
        }
    }
}