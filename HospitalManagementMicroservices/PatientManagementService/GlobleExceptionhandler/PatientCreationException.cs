using System.Runtime.Serialization;

namespace PatientManagementService.GlobleExceptionhandler
{

    public class PatientCreationException : Exception
    {
        public PatientCreationException()
        {
        }

        public PatientCreationException(string? message) : base(message)
        {
        }

        public PatientCreationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected PatientCreationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}