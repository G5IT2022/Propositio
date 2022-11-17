using bacit_dotnet.MVC.Entities;
using Org.BouncyCastle.Asn1.Mozilla;

namespace bacit_dotnet.MVC.Models
{
    public class MyTeamsViewModel
    {
        public EmployeeEntity employee { get; set; }
        public List<TeamEntity> teams { get; set; }

    }
}

