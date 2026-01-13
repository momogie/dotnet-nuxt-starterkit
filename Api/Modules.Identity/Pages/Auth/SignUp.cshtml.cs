    using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.Pages.Auth
{
    public class SignUpModel : PageModel
    {

        public bool IsInvalidCredential { get; set; }
        public void OnGet()
        {
        }
    }
}
