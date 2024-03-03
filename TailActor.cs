using Akka.Actor;
using System.Text;
using System.IO;

namespace AkkaNetConsole
{
    public class TailActor : UntypedActor
    {
        #region Message classes (types)

        /// <summary>
        /// Signal that the file has changed and we need to read the next line of the the file
        /// </summary>
        public class FileWrite
        {
            public string FileName { get; private set; }

            public FileWrite(string fileName)
            {
                FileName = fileName;
            }
        }

        /// <summary>
        /// Signal that the OS had an error accessing the file
        /// </summary>
        public class FileError
        {
            public string FileName { get; private set; }
            public string Reason { get; set; }

            public FileError(string fileName, string reason)
            {
                FileName = fileName;
                Reason = reason;
            }
        }

        /// <summary>
        /// Signal to read the initial contents of the file at actor startup
        /// </summary>
        public class InitialRead
        {
            public string FileNAme { get; private set; }
            public string Text { get; private set; }

            public InitialRead(string fileNAme, string text)
            {
                FileNAme = fileNAme;
                Text = text;
            }
        }

        #endregion Message classes (types)

        private readonly string _filePath;
        private readonly IActorRef _reporterActor;
        private readonly FileObserver _observer;
        private readonly Stream _fileStream;
        private readonly StreamReader _fileStreamReader;

        public TailActor(IActorRef reporterActor, string filePath)
        {
            _reporterActor = reporterActor;
            _filePath = filePath;

            //start watching file for changes
            _observer = new FileObserver(Self, Path.GetFullPath(_filePath));
            _observer.Start();

            //open the file stream with shared read/write permissions (so file can be written to while open)
            _fileStream = new FileStream(Path.GetFullPath(_filePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _fileStreamReader = new StreamReader(_fileStream, Encoding.UTF8);

            //Read the initial contents of the file and send it to the console as first message
            var fileContent = _fileStreamReader.ReadToEnd();
            Self.Tell(new InitialRead(_filePath, fileContent));
        }

        /// <summary>
        /// Handling InitialRead, FileWrite or FileError messages
        /// </summary>
        /// <param name="rawMessage"></param>
        protected override void OnReceive(object rawMessage)
        {
            if (rawMessage is FileWrite)
            {
                //move file cursor forward pull results from cursor to end of file and write to output (assuming a log file type format that is append-only)
                var fileContent = _fileStreamReader.ReadToEnd();
                if (!string.IsNullOrEmpty(fileContent))
                {
                    _reporterActor.Tell(fileContent);
                }
            }
            else if (rawMessage is FileError errorMessage)
            {
                _reporterActor.Tell($"Tail error: {errorMessage.Reason}");
            }
            else if (rawMessage is InitialRead initialMessage)
            {
                _reporterActor.Tell(initialMessage.Text);
            }
        }
    }
}