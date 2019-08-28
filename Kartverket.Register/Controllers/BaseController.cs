using System.Security.Claims;
using System.Web.Mvc;
using Geonorge.AuthLib.Common;
using Kartverket.Register.Services;

namespace Kartverket.Register.Controllers
{
    /// <summary>
    /// This class provides utility methods useful in most controllers. Controllers should inherit from this class.
    /// </summary>
    public abstract class BaseController : Controller
    {
        private readonly UserAuthorization _userAuthorization;

        protected BaseController()
        {
            _userAuthorization = new UserAuthorization();
        }

        public string CurrentUserOrganizationName()
        {
            return ClaimsPrincipal.Current.GetOrganizationName();
        }

        /// <summary>
        /// Check if current user is Admin
        /// </summary>
        /// <returns></returns>
        public bool IsAdmin()
        {
            return _userAuthorization.IsAdmin();
        }

        /// <summary>
        /// Check if user has metadata editor role.
        /// this method had a (legacy?) check on role=nd.metadata before GeoID refactoring.
        /// </summary>
        /// <returns></returns>
        public bool IsEditor()
        {
            return _userAuthorization.IsEditor();
        }
        
        /// <summary>
        /// Check if user has DOK-admin role.
        /// </summary>
        /// <returns></returns>
        public bool IsDokAdmin()
        {
            return _userAuthorization.IsDokAdmin();
        }

        /// <summary>
        /// Check if user has DOK-editor role.
        /// </summary>
        /// <returns></returns>
        public bool IsDokEditor()
        {
            return _userAuthorization.IsDokEditor();
        }
    }
}