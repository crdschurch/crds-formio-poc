namespace crds_formio_poc.Services
{
    public class MpService
    {
        public string GetFormPreFillData(string formData)
        {
            var submission = "{data:{profileModuleFirstName:\"Andy\", profileModuleLastName:\"Canterbury\"}}";
            return submission;
        }
    }
}
