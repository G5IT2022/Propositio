using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI.Relational;
using System.Security.Claims;

namespace bacit_dotnet.MVC.Helpers
{
    public static class LoggingHelper
    {
        public static string PageAccessedLog(int emp_id, string pageName)
        {
            return string.Format("User with emp_id: {0} accessed {1} on {2}", emp_id, pageName, DateTime.Now);
        }

        public static string EntityCreatedLogSuccess(int emp_id, string entityName)
        {
            return string.Format("User with emp_id: {0} created new {1} on {2}", emp_id.ToString(), entityName, DateTime.Now);
        }
        public static string EntityCreatedLogFailed(int emp_id, string entityName, string extraError = "Undefined")
        {
            return string.Format("User with emp_id: {0} failed to create new {1} on {2} because {3}", emp_id.ToString(), entityName, DateTime.Now, extraError);
        }

        public static string EntityUpdatedLogSuccess(int emp_id, string entityName)
        {
            return string.Format("User with emp_id: {0}  updated {1} on {2}", emp_id.ToString(), entityName, DateTime.Now);
        }
        public static string EntityUpdatedLogFailed(int emp_id, string entityName, string extraError = "Undefined")
        {
            return string.Format("User with emp_id: {0} failed to update {1} on {2} because {3}", emp_id.ToString(), entityName, DateTime.Now, extraError);
        }
         public static string EntityDeleteLogSuccess(int emp_id, string entityName)
        {
            return string.Format("User with emp_id: {0}  deleted {1} on {2}", emp_id.ToString(), entityName, DateTime.Now);
        }
        public static string EntityDeleteLogFailed(int emp_id, string entityName, string extraError = "Undefined")
        {
            return string.Format("User with emp_id: {0} failed to delete {1} on {2} because {3}", emp_id.ToString(), entityName, DateTime.Now, extraError);
        }

    }
}
