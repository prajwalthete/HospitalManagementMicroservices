using System.Runtime.Serialization;

namespace DoctorManagementService.GlobleExceptionhandler
{
    [Serializable]
    internal class DoctorCreationException : Exception
    {
        public DoctorCreationException()
        {
        }

        public DoctorCreationException(string? message) : base(message)
        {
        }

        public DoctorCreationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DoctorCreationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}