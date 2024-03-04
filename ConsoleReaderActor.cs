using Akka.Actor;

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
            Console.WriteLine("Please provide the URI of a log file on disk to tail.\n");
        }
    }
}