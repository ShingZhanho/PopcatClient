using System;

namespace PopcatClient.Languages.Exceptions
{
    public class InvalidLanguagePackFormatException : Exception
    {
        public InvalidLanguagePackFormatException(string message) : base(message) { }
        
        public InvalidLanguagePackFormatException(string message, Exception innerException) : base(message, innerException) { }
    }
}