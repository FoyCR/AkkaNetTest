using Akka.Actor;

namespace AkkaNetConsole
{
    internal class ConsoleReaderActor : UntypedActor
    {
        public const string ExitCommand = "quit";
        public const string StartCommand = "start";
        private const string BaseActorPath = "akka://foyActorSyxtem/user/";
        /* This is the prefer way to this but in order to demonstrate ActorSelection we are no use it
         * private IActorRef _validationActor;

        public ConsoleReaderActor(IActorRef validationActor)
        {
            _validationActor = validationActor;
        }
        */

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
            // replaced by the ActorSelection way
            //_validationActor.Tell(message);
            Context.ActorSelection($"{BaseActorPath}fileValidatorActor").Tell(message); //we use the name of the actor (named in program.cs), not the class name
        }

        private void printInstructions()
        {
            Console.WriteLine("Please provide the URI of a log file on disk to tail.\n");
        }
    }
}