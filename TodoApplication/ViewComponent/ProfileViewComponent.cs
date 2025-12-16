using Microsoft.AspNetCore.Mvc;


namespace UserManagementWeb.ViewComponents
{
    public class ProfileViewComponent : ViewComponent
    {
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("Default", "Suwas");
        }
    }
}