using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories.Suggestion
{
    public interface ISuggestionRepository
    {
        List<SuggestionEntity> getAll();
        List<SuggestionEntity> getByEmployeeID(int id);
        SuggestionEntity getById(int id);
        List<SuggestionEntity> getByStatus(STATUS status);
        void Add(SuggestionEntity entity);
        public int getLatestSuggestionID();

    }
}
