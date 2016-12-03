using Hangfire.Dashboard;
using System.Web;
using Hangfire.Annotations;

namespace LivrETS
{
    class HangfireAutorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            if (HttpContext.Current.User.IsInRole("Administrator"))
                 return true;

            return false;
        }
     }
}
