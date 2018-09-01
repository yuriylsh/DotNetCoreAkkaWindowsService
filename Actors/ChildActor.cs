using Akka.Actor;
using Akka.Event;
using Microsoft.Extensions.Configuration;

namespace DotNetCoreAkkaWindowsService.Actors
{
    public class ChildActor  : ReceiveActor
    {
        public ChildActor(IConfiguration configuration)
        {
            Receive<object>(msg => Context.GetLogger().Info($"{nameof(ChildActor)} says: {msg}, testProperty = {configuration["testProperty"]}"));
        }
    }
}