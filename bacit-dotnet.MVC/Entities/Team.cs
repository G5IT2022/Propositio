namespace bacit_dotnet.MVC.Entities
{
    public class TeamEntity
    {
        public int team_id { get; set; }
        public int team_lead_id { get; set; }
        public string team_name { get; set; }   
        public List<EmployeeEntity> employees { get; set; }
    }

}
