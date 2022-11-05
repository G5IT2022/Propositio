using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Helpers
{
    public static class FilterHelper
    {
        //Filtrerer forslag på status, tar inn en liste med forslag og en streng som er filteret også filtrerer den basert på det med
        //Collection.FindAll metoden, denne metoden går gjennom alle tingene i listen og sjekker det mot et kriterie. 
        public static List<SuggestionEntity> FilterSuggestions(List<SuggestionEntity> suggestions, string filter)
        {
            switch (filter)
            {
                case "planStatus":
                    var filteredPlan = suggestions.FindAll(s => s.status == STATUS.PLAN);
                    suggestions = filteredPlan;
                    break;
                case "doStatus":
                    var filteredDo = suggestions.FindAll(s => s.status == STATUS.DO);
                    suggestions = filteredDo;
                    break;
                case "studyStatus":
                    var filteredStudy = suggestions.FindAll(s => s.status == STATUS.STUDY);
                    suggestions = filteredStudy;
                    break;
                case "actStatus":
                    var filteredAct = suggestions.FindAll(s => s.status == STATUS.ACT);
                    suggestions = filteredAct;
                    break;
                case "justDoIt":
                    var filteredJDI = suggestions.FindAll(s => s.status == STATUS.JUSTDOIT);
                    suggestions = filteredJDI;
                    break;
                case "lastWeek":
                // var filteredLastWeek = suggestions.FindAll(s => s.timestamp.createdTimestamp.)
                default:

                    break;
            }
            return suggestions;
        }

        //Filtrerer basert på kategori på en litt uelegant måte men det fungerer. 
        //Sjekker kategoriene på hvert forslag og ser om det er noen som matcher også returnerer den de som matcher
        public static List<SuggestionEntity> FilterCategories(List<SuggestionEntity> suggestions, CategoryEntity category)
        {
            List<SuggestionEntity> suggestionsToReturn = new List<SuggestionEntity>();
            foreach(SuggestionEntity suggestion in suggestions)
            {
                foreach(CategoryEntity suggestionCategory in suggestion.categories)
                {
                    //Burde egentlig skrive om .equals metoden her til å matche entities...
                    if (suggestionCategory.category_name.Equals(category.category_name))
                    {
                        suggestionsToReturn.Add(suggestion);
                    }
                }
            }
            return suggestionsToReturn;
        }
    }

}
