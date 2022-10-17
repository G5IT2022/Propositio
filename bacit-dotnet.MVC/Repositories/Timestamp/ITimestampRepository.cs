using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories.Timestamp
{
    public interface ITimestampRepository
    {
        public TimestampEntity Get(int suggestion_id);
        public TimestampEntity Create(int suggestion_id, DateTime dueByTimestamp);
    }
}
