using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories.Category
{

    public interface ICategoryRepository
    {
        public List<CategoryEntity> getAll();
        public List<CategoryEntity> getCategoriesForSuggestion(int id);
        public void addCategories(List<CategoryEntity> categories, int suggid);
    }

}

