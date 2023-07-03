using System;

namespace ReWork_WPF.Exceptions
{
    public class UserInputException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UserInputException"/> class with the specified error message.
        /// </summary>
        /// <param name="message">
        ///     The error message that describes the exception.
        /// </param>
        public UserInputException(string message) : base(message)
        {
        }
    }
}
