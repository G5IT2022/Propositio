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

        public int Create(CategoryEntity category)
        {
            var query = @"INSERT INTO Category(category_name) VALUES(@category_name)";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var affectedRows = connection.Execute(query, new { category_name = category.category_name });
                return affectedRows;
            }
        }



        public CategoryEntity Get(int category_id)
        {
            var query = @"SELECT * FROM Category WHERE category_id = @category_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var category = connection.QueryFirstOrDefault<CategoryEntity>(query, new { category_id = category_id });
                return category;
            }
        }

        public List<CategoryEntity> GetAll()
        {
            var query = @"SELECT * FROM Category";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var category = connection.Query<CategoryEntity>(query);
                return category.ToList();
            }
        }

        public List<CategoryEntity> GetCategoriesForSuggestion(int id)
        {
            var query = @"SELECT c.category_name FROM Category as c INNER JOIN
                SuggestionCategory as sc on c.category_id = sc.category_id INNER JOIN Suggestion as s on s.suggestion_id = sc.suggestion_id WHERE s.suggestion_id = @suggid";
            using(var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var categories = connection.Query<CategoryEntity>(query, new { suggid = id });
                return categories.ToList();
            }
        }
        public int Update(CategoryEntity category)
        {
            var query = @"UPDATE Category SET @category_name WHERE category_id = @category_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var affectedRows = connection.Execute(query, new { category_name = category.category_name, category_id = category.category_id });
                return affectedRows;
            }
        }
        public int Delete(CategoryEntity category)
        {
            var query = @"DELETE FROM Category WHERE category_id = @category_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var affectedRows = connection.Execute(query, new { category_id = category.category_id });
                return affectedRows;
            }
        }


    }

}

