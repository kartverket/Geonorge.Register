using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Geonorge.AuthLib.Common;

namespace Kartverket.Register.Services
{
    public class UserAuthorization
    {
        /// <summary>
        /// Check if user has metadata admin role.
        /// </summary>
        /// <returns></returns>
        public bool IsAdmin()
        {
            return GetCurrentUser().IsInRole(GeonorgeRoles.MetadataAdmin);
        }

        /// <summary>
        /// Check if user has register manager role.
        /// </summary>
        /// <returns></returns>
        public bool IsRegisterManager()
        {
            return GetCurrentUser().IsInRole(GeonorgeRoles.RegisterManager);
        }

        public string GetOrganizationNumber()
        {
            return GetCurrentUser().GetOrganizationOrgnr();
        }

        /// <summary>
        /// Check if user has metadata editor role.
        /// this method had a (legacy?) check on role=nd.metadata before GeoID refactoring.
        /// </summary>
        /// <returns></returns>
        public bool IsEditor()
        {
            return GetCurrentUser().IsInRole(GeonorgeRoles.MetadataEditor);
        }
        
        /// <summary>
        /// Check if user has DOK-admin role.
        /// </summary>
        /// <returns></returns>
        public bool IsDokAdmin()
        {
            return GetCurrentUser().IsInRole(GeonorgeRoles.DokAdmin);
        }

        /// <summary>
        /// Check if user has DOK-editor role.
        /// </summary>
        /// <returns></returns>
        public bool IsDokEditor()
        {
            return GetCurrentUser().IsInRole(GeonorgeRoles.DokEditor);
        }

        private ClaimsPrincipal GetCurrentUser()
        {
            return ClaimsPrincipal.Current;
        }
    }
}