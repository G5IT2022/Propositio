using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using System;

namespace bacit_dotnet.MVC.Helpers
{

    /**
     * Statisk klasse for å abstrahere bort sjekken om brukeren eier en ting/er admin
     * 
     * **/
    public static class AuthorizationHelper
    {
 
        /// <summary>
        /// Denne metoden sjekker om brukeren som er logget inn er Admin
        /// </summary>
        /// <param name="context">HTTPContexten for nåværende sesjon</param>
        /// <returns>bool, true hvis brukeren er admin, false hvis ikke</returns>
        public static bool UserIsAdmin(HttpContext context)
        {
            if (context.User.IsInRole("Admin"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Denne metoden sjekker om brukeren som er logget inn eier entiteten assossiert med iden i paramteret, suggestion_id for eksempel
        /// </summary>
        /// <param name="context">HTTPContexten for nåværende sesjon</param>
        /// <param name="id">en id for en entitet</param>
        /// <returns>bool, true hvis brukeren er eier, false hvis ikke</returns>
        public static bool UserIsOwner(int id, HttpContext context)
        {
            var uid = Int32.Parse(context.User.FindFirstValue(ClaimTypes.UserData));
            if (uid == id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Denne metoden sjekker om brukeren som er logget inn eier entiteten assossiert med iden i paramteret eller admin
        /// shorthand for å skrive if(Authorizatoinhelper.userisadmin || authorizationhelper.userisowner)
        /// </summary>
        /// <param name="context">HTTPContexten for nåværende sesjon</param>
        /// <param name="id">en id for en entitet</param>
        /// <returns>bool, true hvis brukeren er eier eller admin, false hvis ikke</returns>
        public static bool UserIsAny(int id, HttpContext context)
        {
            if (UserIsAdmin(context) || UserIsOwner(id, context))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
