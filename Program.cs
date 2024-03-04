using System;
using Akka.Actor;
using Akka.Routing;

namespace AkkaNetConsole
{
    internal class Program
    {
        public static ActorSystem foyActorSystem;

        private static void Main(string[] args)
        {
            foyActorSystem = ActorSystem.Create("foyActorSyxtem"); //Initialize the actor system of Akka .Net

            //Props consoleWriterActorProps = Props.Create(typeof(ConsoleWriterActor)); //NEVER NEVER Use this syntax things will compile but won't run properly
            IActorRef consoleWriterActor = foyActorSystem.ActorOf(Props.Create(() => new ConsoleWriterActor()), "consoleWriterActor"); //we name the actor for easier logging purposes
            //Think of Props as a recipe for making an actor.Technically, Props is a configuration class that encapsulates all the information needed to make an instance of a given type of actor.

            //Props validationActorProps = Props.Create(() => new ValidationActor(consoleWriterActor)); //replaced by fileValidatorActor
            // IActorRef validationActor = foyActorSystem.ActorOf(validationActorProps, "validationActor");

            //make TailCoordinatorActor
            Props tailCoordinatorProps = Props.Create(() => new TailCoordinatorActor()); //Lambda syntax
            IActorRef tailCoordinatorActor = foyActorSystem.ActorOf(tailCoordinatorProps, "tailCoordinatiorActor");

            //pass tailerCoordinatorActor to fileValidatorActorProps
            Props fileValidatorActorProps = Props.Create(() => new FileValidatorActor(consoleWriterActor, tailCoordinatorActor));
            IActorRef fileValidatorActor = foyActorSystem.ActorOf(fileValidatorActorProps, "fileValidatorActor");

            Props consoleReaderActorProps = Props.Create<ConsoleReaderActor>(fileValidatorActor); //Generic syntax
            IActorRef consoleReaderActor = foyActorSystem.ActorOf(consoleReaderActorProps, "consoleReaderActor");

            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand); //Start console reader actor

            foyActorSystem.WhenTerminated.Wait(); //blocks the main thread from exiting until the actor system is shut down
        }
    }
}