# AkkaNetTest

I'm following a boot camp from Petabridge in order to start playing around with the [Akka .Net Library](https://getakka.net/articles/intro/what-is-akka.html) which is an implementation of the Actor Model.

## Current Flow
The idea is to create a console utility that monitor (tail) log files. Show the initial contents of the file and when a line is appended to file, that line is showed in the console (tail).

The current workflow is as follows:
- The *consoleReaderActor* actor starts and prints instructions to the console to capture the path to the file to be provided by the user.
- When the file path is entered the consoleReaderActor passes it to the *FileValidationActor* 
- It will validate whether the file exists (Valid) or not (invalid) by printing validation messages through the *ConsoleWriterActor* accordingly.
- If the path is valid it tells the *TailCoordinatorActor* to start tailing the file through the *TailActor* in a parent-child relationship between them.
- The *TailActor* uses the *FileObserver* utility to monitor the file and the *ConsoleWriterActor* to write the file contents and new lines to the console.



## First flow (Old):
This is what we have built so far to demonstrate how the Akka .Net Actor model works:

- Create an Actor System
- Create an actor that is in charge of writing output messages to the console ( **ConsoleWriterActor**).
- Create an actor that is in charge of validating input messages (**ValidationActor** ).
- Create an actor that is in charge of reading input messages from the console (**consoleReaderActor**)

The workflow is as follows:

- The **consoleReaderActor** actor starts and prints instructions to the console and stands by waiting for user input messages.
- When a message is entered the consoleReaderActor passes it to the **ValidationActor** 
- it will validate if the message length is even (Valid) or if it is odd (invalid) and 
- it will print the validation messages through the **ConsoleWriterActor** accordingly.
