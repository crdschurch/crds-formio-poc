using RestSharp;

namespace crds_formio_poc.Services
{
    public class FormIoService
    {
        public string GetFormData(string formName)
        {
            var formIoClient = new RestClient("https://crossroads.form.io/");
            var formRequest = new RestRequest(formName);

            var formData = formIoClient.Execute(formRequest).Content;
            return formData;
        }
    }
}
