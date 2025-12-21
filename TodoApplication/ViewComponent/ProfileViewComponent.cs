using Microsoft.AspNetCore.Mvc;
using TodoApplication.Identity;


namespace UserManagementWeb.ViewComponents
{
    public class ProfileViewComponent : ViewComponent
    {
        private readonly ISystemInfoFromCookie _cookieInfo;
        public ProfileViewComponent(ISystemInfoFromCookie cookieInfo)
        {
            _cookieInfo = cookieInfo;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("Default", _cookieInfo.full_name);
        }
    }
}