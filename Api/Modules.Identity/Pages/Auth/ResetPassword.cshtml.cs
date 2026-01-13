using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.Pages.Auth
{
    public class ResetPasswordModel : PageModel
    {

        public bool IsInvalidCredential { get; set; }
        public void OnGet()
        {
        }
    }
}
