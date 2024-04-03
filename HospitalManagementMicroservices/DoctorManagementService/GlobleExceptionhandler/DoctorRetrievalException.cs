using System.Runtime.Serialization;

namespace DoctorManagementService.GlobleExceptionhandler
{
    [Serializable]
    internal class DoctorRetrievalException : Exception
    {
        public DoctorRetrievalException()
        {
        }

        public DoctorRetrievalException(string? message) : base(message)
        {
        }

        public DoctorRetrievalException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DoctorRetrievalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}