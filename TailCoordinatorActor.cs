﻿using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaNetConsole
{
    public class TailCoordinatorActor : UntypedActor
    {
        #region message classes (types)

        /// <summary>
        /// Start tailing the file at user specified path
        /// </summary>
        public class StartTail
        {
            public string FilePath { get; private set; }
            public IActorRef ReporterActor { get; private set; }

            public StartTail(string filePath, IActorRef reporterActor)
            {
                FilePath = filePath;
                ReporterActor = reporterActor;
            }
        }

        /// <summary>
        /// Stop tailing the file at user specified path
        /// </summary>
        public class StopTail
        {
            public string FilePath { get; set; }

            public StopTail(string filePath)
            {
                FilePath = filePath;
            }
        }

        #endregion message classes (types)

        /// <summary>
        /// Handling StartTail or StopTail messages
        /// </summary>
        /// <param name="rawMessage"></param>
        protected override void OnReceive(object rawMessage)
        {
            if (rawMessage is StartTail message)
            {
                //Creating our first parent/child relationship. The TailActor instance created is a child of this instance of TailCoordinatorActor
                Context.ActorOf(Props.Create(() => new TailActor(message.ReporterActor, message.FilePath)));
            }
        }

        /// <summary>
        /// As the TailCoordinatorActor is the supervisor (father) of TailActor
        /// We implement this custom Supervisor strategy
        /// </summary>
        /// <returns></returns>
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                10,                       // Max number of retries
                TimeSpan.FromSeconds(30),// Within time range
                x => //local Only Decider
                {
                    // ArithmeticException is not an application critical. We just ignore the error and resume;
                    if (x is ArithmeticException) return Directive.Resume;
                    // Error that we can not recover from, stop the failing actor;
                    else if (x is NotSupportedException) return Directive.Stop;
                    //In all other cases, just restart the failing actor;
                    else return Directive.Restart;
                });
        }
    }
}