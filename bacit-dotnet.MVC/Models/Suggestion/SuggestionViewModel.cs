using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Models.Suggestion
{
    /**
    * Denne metoden er for å slette en kategori
    * @Parameter ()
    * @Return Index View for Suggestion (forslag).
    */
    public class SuggestionViewModel
    {
        public List<SuggestionEntity> suggestions;
        public List<CategoryEntity> categories;
    }
}
