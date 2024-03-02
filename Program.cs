using System;
using Akka.Actor;

namespace AkkaNetConsole
{
    internal class Program
    {
        public static ActorSystem foyActorSystem;

        private static void Main(string[] args)
        {
            foyActorSystem = ActorSystem.Create("foyActorSyxtem"); //Initialize the actor system of Akka .Net

            //Props consoleWriterActorProps = Props.Create(typeof(ConsoleWriterActor)); //NEVER NEVER Use this syntax things will compile but won't run properly
            Props consoleWriterActorProps = Props.Create(() => new ConsoleWriterActor());
            IActorRef consoleWriterActor = foyActorSystem.ActorOf(consoleWriterActorProps, "consoleWriterActor"); //we name the actor for easier logging purposes

            Props validationActorProps = Props.Create(() => new ValidationActor(consoleWriterActor)); //Lambda syntaxis
            IActorRef validationActor = foyActorSystem.ActorOf(validationActorProps, "validationActor");

            Props consoleReaderActorProps = Props.Create<ConsoleReaderActor>(validationActor); //Generic syntaxis
            IActorRef consoleReaderActor = foyActorSystem.ActorOf(consoleReaderActorProps, "consoleReaderActor");

            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand); //Start console reader actor

            foyActorSystem.WhenTerminated.Wait(); //blocks the main thread from exiting until the actor system is shut down
        }
    }
}