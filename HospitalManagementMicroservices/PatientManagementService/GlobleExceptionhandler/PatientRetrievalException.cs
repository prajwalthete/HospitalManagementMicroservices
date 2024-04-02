using System.Runtime.Serialization;

namespace PatientManagementService.GlobleExceptionhandler
{
    public class PatientRetrievalException : Exception
    {
        public PatientRetrievalException()
        {
        }

        public PatientRetrievalException(string? message) : base(message)
        {
        }

        public PatientRetrievalException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected PatientRetrievalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
