using System.Text.Json.Serialization;

namespace PatientManagementService.Entity
{
    public class PatientEntity
    {
        [JsonIgnore]
        public int PatientID { get; set; } // Primary Key for Patient table which is UserId from UserTbale

        public string MedicalHistory { get; set; }
        public string Insurance { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }

        // Navigation Properties (optional)
        //  public User User { get; set; } // Map to User table (assuming foreign key relationship)
    }
}
