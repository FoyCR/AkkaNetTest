using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaNetConsole
{
    public class FileValidatorActor : UntypedActor
    {
        private readonly IActorRef _consoleWriteActor;
        private readonly IActorRef _tailCoordinatorActor;

        public FileValidatorActor(IActorRef consoleWriteActor, IActorRef tailCoordinatorActor)
        {
            _consoleWriteActor = consoleWriteActor;
            _tailCoordinatorActor = tailCoordinatorActor;
        }

        protected override void OnReceive(object rawMessage)
        {
            var message = rawMessage as string;
            if (string.IsNullOrEmpty(message))
            {
                //Signal that the user need to supply an input
                _consoleWriteActor.Tell(new Messages.NullInputError("Input was blank. Please try again \n"));

                //tell sender to continue doing its thing (this actor doesn't care)
                Sender.Tell(new Messages.ContinueProcessing());
            }
            else
            {
                if (IsFileUri(message))
                {
                    //Signal successful input
                    _consoleWriteActor.Tell(new Messages.InputSuccess($"Start processing for {message}"));
                    //start tail coordinator
                    _tailCoordinatorActor.Tell(new TailCoordinatorActor.StartTail(message, _consoleWriteActor));
                }
                else
                {
                    //Signal that input was incorrect
                    _consoleWriteActor.Tell(new Messages.ValidationError($"{message} is not an existing URI on disk."));
                    //tell sender to continue doing its thing (this actor doesn't care)
                    Sender.Tell(new Messages.ContinueProcessing());
                }
            }
        }

        /// <summary>
        /// Check if file exists at pat provided by the user
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static bool IsFileUri(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}