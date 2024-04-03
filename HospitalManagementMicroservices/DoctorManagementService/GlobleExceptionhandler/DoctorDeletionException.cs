using System.Runtime.Serialization;

namespace DoctorManagementService.GlobleExceptionhandler
{
    [Serializable]
    internal class DoctorDeletionException : Exception
    {
        public DoctorDeletionException()
        {
        }

        public DoctorDeletionException(string? message) : base(message)
        {
        }

        public DoctorDeletionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DoctorDeletionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}