using System.Text.Json.Serialization;

namespace AppointmentManagementService.Entity
{
    public class AppointmentEntity
    {
        public int AppointmentID { get; set; }
        public DateTime AppointmentDate { get; set; }
        [JsonIgnore]
        public int DoctorID { get; set; }
        [JsonIgnore]
        public int PatientID { get; set; }
    }
}
