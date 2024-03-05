using Akka.Actor;

namespace AkkaNetConsole
{
    internal class ConsoleWriterActor : UntypedActor
    {
        protected override void OnReceive(object rawMessage)
        {
            if (rawMessage is Messages.InputError error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error.Reason);
            }
            else if (rawMessage is Messages.InputSuccess success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(success.Reason);
            }
            else
                Console.Write(rawMessage);
            Console.ResetColor();
        }
    }
}