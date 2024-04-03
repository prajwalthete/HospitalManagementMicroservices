using System.Text.Json.Serialization;

namespace DoctorManagementService.Entity
{
    public class DoctorEntity
    {
        [JsonIgnore]
        public int DoctorID { get; set; }
        public string Specialization { get; set; }
        public bool IsAvailable { get; set; }
        public string Qualifications { get; set; }
        public string Schedule { get; set; }


    }
}
