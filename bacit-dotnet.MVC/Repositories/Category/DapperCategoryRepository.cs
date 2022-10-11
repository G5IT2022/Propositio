using bacit_dotnet.MVC.DataAccess;
using bacit_dotnet.MVC.Entities;
using Dapper;
using Dapper.Contrib.Extensions;
using MySqlConnector;

namespace bacit_dotnet.MVC.Repositories.Category
{
    public class DapperCategoryRepository : ICategoryRepository
    {
        private readonly ISqlConnector sqlConnector;

        public DapperCategoryRepository(ISqlConnector sqlConnector)
        {
            this.sqlConnector = sqlConnector;
        }

        public List<CategoryEntity> getAll()
        {
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var category = connection.Query<CategoryEntity>("SELECT * FROM Category");
                return category.ToList();
            }
        }
        public void addCategories(List<CategoryEntity> categories, int suggid)
        {
            var query = @"INSERT INTO SuggestionCategory(suggestion_id, category_id) VALUES(@suggid, @categoryid)";
            foreach (CategoryEntity category in categories)
            {
                using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
                {
                    connection.Execute(query, new { suggid = suggid, categoryid = category.category_id });
                }
            }
        }

        public List<CategoryEntity> getCategoriesForSuggestion(int id)
        {
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var query = @"SELECT c.category_name FROM Category as c INNER JOIN
                SuggestionCategory as sc on c.category_id = sc.category_id INNER JOIN Suggestion as s on s.suggestion_id = sc.suggestion_id WHERE s.suggestion_id = @suggid";
                var categories = connection.Query<CategoryEntity>(query, new { suggid = id });

                return categories.ToList();
            }

        }
    }

    }

