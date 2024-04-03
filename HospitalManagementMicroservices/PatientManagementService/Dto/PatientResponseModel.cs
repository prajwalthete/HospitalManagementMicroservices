namespace PatientManagementService.Dto
{
    public class PatientResponseModel
    {
        public int PatientID { get; set; }
        public string MedicalHistory { get; set; }
        public string Insurance { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
    }
}
