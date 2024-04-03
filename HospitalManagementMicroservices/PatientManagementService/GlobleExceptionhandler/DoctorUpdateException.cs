using System.Runtime.Serialization;

namespace PatientManagementService.GlobleExceptionhandler
{
    [Serializable]
    internal class DoctorUpdateException : Exception
    {
        public DoctorUpdateException()
        {
        }

        public DoctorUpdateException(string? message) : base(message)
        {
        }

        public DoctorUpdateException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DoctorUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}