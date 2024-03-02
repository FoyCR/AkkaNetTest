using Akka.Actor;
using System.ComponentModel.DataAnnotations;

namespace AkkaNetConsole
{
    internal class ConsoleReaderActor : UntypedActor
    {
        public const string ExitCommand = "quit";
        public const string StartCommand = "start";
        private IActorRef _validationActor;

        public ConsoleReaderActor(IActorRef validationActor)
        {
            _validationActor = validationActor;
        }

        protected override void OnReceive(object rawMessage)
        {
            if (rawMessage.Equals(StartCommand))
            {
                printInstructions();
            }          

            GetAndValidateInput();
        }

        private void GetAndValidateInput()
        {
            var message = Console.ReadLine();
            
            if (!string.IsNullOrEmpty(message) && string.Equals(message, ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                Context.System.Terminate();
                return;
            }
            _validationActor.Tell(message);
        }

        private void printInstructions()
        {
            Console.WriteLine("Write whatever you want into the console!");
            Console.WriteLine("Some entries will pass validation, and some won't...\n\n");
            Console.WriteLine($"Type '{ExitCommand}' to quit this application at any time.\n");
        }
    }
}