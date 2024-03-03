using Akka.Actor;

namespace AkkaNetConsole
{
    internal class ValidationActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;

        public ValidationActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object rawMessage)
        {
            var message = rawMessage as string;
            if (string.IsNullOrEmpty(message))
            {
                _consoleWriterActor.Tell(new Messages.NullInputError("No input received"));
            }
            else if (ValidateMessage(message))
            {
                _consoleWriterActor.Tell(new Messages.InputSuccess("Valid message received!"));
            }
            else
            {
                _consoleWriterActor.Tell(new Messages.ValidationError("Invalid message received"));
            }

            Sender.Tell(new Messages.ContinueProcessing());
        }

        private bool ValidateMessage(string message)
        {
            bool isValid = message.Length % 2 == 0;
            return isValid;
        }
    }
}