using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories.Suggestion
{
    public interface ISuggestionRepository
    {
        void Create(SuggestionEntity suggestion);
        List<SuggestionEntity> GetAll();
        List<SuggestionEntity> GetByEmployeeID(int author_emp_id);
        SuggestionEntity GetById(int id);
        List<SuggestionEntity> GetByStatus(STATUS status);
        public int GetNewSuggestionID();

    }
}
