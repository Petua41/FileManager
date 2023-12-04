using System;

namespace FileManager.Infrastructure.Exceptions
{

    [Serializable]
    internal class InvalidCharacterInFilenameException : Exception
    {
        public InvalidCharacterInFilenameException() { }
        public InvalidCharacterInFilenameException(string message) : base(message) { }
        public InvalidCharacterInFilenameException(string message, Exception inner) : base(message, inner) { }
        protected InvalidCharacterInFilenameException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
