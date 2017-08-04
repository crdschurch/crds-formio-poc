using crds_formio_poc.Models;
using crds_formio_poc.Services;
using Microsoft.AspNetCore.Mvc;

namespace crds_formio_poc.Controllers
{
    public class FormController : Controller
    {
        public IActionResult Index()
        {
            const string formName = "campercampingcamp";
            var formIo = new FormIoService();
            var mpService = new MpService();

            var formdata = formIo.GetFormData(formName);
            var submissionData = mpService.GetFormPreFillData(formdata);

            var form = new FormIo
            {
                Form = formdata,
                FormName = formName,
                PrePopData = submissionData
            };
            return View(form);
        }
    }
}
