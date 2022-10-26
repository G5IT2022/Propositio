using bacit_dotnet.MVC.DataAccess;
using bacit_dotnet.MVC.Entities;
using Dapper;
using Dapper.Contrib.Extensions;
using MySqlConnector;

namespace bacit_dotnet.MVC.Repositories
{
    public class SuggestionRepository : ISuggestionRepository
    {
        private readonly ISqlConnector sqlConnector;

        public SuggestionRepository(ISqlConnector sqlConnector)
        {
            this.sqlConnector = sqlConnector;
        }
        public List<SuggestionEntity> GetAll()
        {
            var query = @"@SELECT s.*, sts.*, c.* FROM Suggestion AS s INNER JOIN SuggestionTimestamp AS sts ON s.suggestion_id = sts.suggestion_id
            INNER JOIN SuggestionCategory AS sc ON sc.suggestion_id = s.suggestion_id INNER JOIN Category AS c ON sc.category_id = c.category_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var suggestions = connection.Query<SuggestionEntity, TimestampEntity, CategoryEntity, SuggestionEntity>(query, (suggestion, timestamp, category) =>
                {
                    suggestion.timestamp = timestamp;
                    if (suggestion.categories == null)
                    {
                        suggestion.categories = new List<CategoryEntity>();
                    }
                    suggestion.categories.Add(category);
                    return suggestion;
                }, splitOn: "timestamp_id, category_id");
                var result = suggestions.GroupBy(s => s.suggestion_id).Select(suggestion =>
                {
                    var groupedSuggestion = suggestion.First();
                    groupedSuggestion.categories = suggestion.Select(e => e.categories.Single()).ToList();
                    return groupedSuggestion;
                });
                return result.ToList();
            }
        }
        public void Favorite(int id, bool update)
        {
            var updateQuery = @"UPDATE Suggestion SET favorite = @state WHERE suggestion_id = @suggestion_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                connection.Execute(updateQuery, new { state = update, suggestion_id = id });
            }

        }
        private int GetNewSuggestionID()
        {
            var query = @"SELECT COUNT(*) FROM Suggestion";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var count = connection.QueryFirst<int>(query);
                return count + 1;
            }
        }
        public SuggestionEntity GetSuggestionBySuggestionID(int suggestion_id)
        {
            var query = @"SELECT s.*, sts.*, c.* FROM Suggestion AS s INNER JOIN 
            SuggestionTimestamp AS sts ON s.suggestion_id = sts.suggestion_id INNER JOIN 
            SuggestionCategory AS sc ON sc.suggestion_id = s.suggestion_id INNER JOIN 
            Category AS c ON sc.category_id = c.category_id WHERE s.suggestion_id = @suggestion_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var suggestions = connection.Query<SuggestionEntity, TimestampEntity, CategoryEntity, SuggestionEntity>(query, (suggestion, timestamp, category) =>
                {
                    suggestion.timestamp = timestamp;
                    if (suggestion.categories == null)
                    {
                        suggestion.categories = new List<CategoryEntity>();
                    }
                    suggestion.categories.Add(category);
                    return suggestion;
                }, new { suggestion_id }, splitOn: "timestamp_id, category_id");
                var result = suggestions.GroupBy(s => s.suggestion_id).Select(suggestion =>
                {
                    var groupedSuggestion = suggestion.First();
                    groupedSuggestion.categories = suggestion.Select(s => s.categories.Single()).ToList();
                    return groupedSuggestion;
                });
                return result.ElementAt(0);
            }
        }
        public SuggestionEntity GetSuggestionBySuggestionIDWithCommentsAndImages(int suggestion_id)
        {
            var query = @"SELECT s.*, sts.*, c.*, co.*, i.* FROM Suggestion AS s 
            INNER JOIN SuggestionTimestamp AS sts ON s.suggestion_id = sts.suggestion_id 
            INNER JOIN SuggestionCategory AS sc ON sc.suggestion_id = s.suggestion_id 
            INNER JOIN Category AS c ON sc.category_id = c.category_id 
            INNER JOIN SuggestionComment AS co ON co.suggestion_id = s.suggestion_id
            INNER JOIN Image AS i ON i.suggestion_id = s.suggestion_id WHERE s.suggestion_id = @suggestion_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var suggestions = connection.Query<SuggestionEntity, TimestampEntity, CategoryEntity, CommentEntity, ImageEntity, SuggestionEntity>(query, (suggestion, timestamp, category, comment, image) =>
                {
                    suggestion.timestamp = timestamp;
                    if (suggestion.categories == null)
                    {
                        suggestion.categories = new List<CategoryEntity>();
                    }
                    if (suggestion.comments == null)
                    {
                        suggestion.comments = new List<CommentEntity>();
                    }
                    if (suggestion.images == null)
                    {
                        suggestion.images = new List<ImageEntity>();
                    }
                    suggestion.categories.Add(category);
                    suggestion.comments.Add(comment);
                    suggestion.images.Add(image);
                    return suggestion;
                }, new { suggestion_id }, splitOn: "timestamp_id, category_id, comment_id, image_id");
                var result = suggestions.GroupBy(s => s.suggestion_id).Select(suggestion =>
                {
                    var groupedSuggestion = suggestion.First();
                    groupedSuggestion.categories = suggestion.Select(s => s.categories.Single()).ToList();
                    groupedSuggestion.comments = suggestion.Select(s => s.comments.Single()).ToList();
                    groupedSuggestion.images = suggestion.Select(s => s.images.Single()).ToList();
                    return groupedSuggestion;
                });
                return result.ElementAt(0);
            }
        }
        public List<SuggestionEntity> GetSuggestionsByAuthorID(int author_emp_id)
        {
            var query = @"SELECT s.*, sts.*, c.* FROM Suggestion AS s INNER JOIN 
            SuggestionTimestamp AS sts ON s.suggestion_id = sts.suggestion_id  INNER JOIN 
            SuggestionCategory AS sc ON sc.suggestion_id = s.suggestion_id INNER JOIN 
            Category AS c ON sc.category_id = c.category_id WHERE s.author_emp_id = @author_emp_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var suggestions = connection.Query<SuggestionEntity, TimestampEntity, CategoryEntity, SuggestionEntity>(query, (suggestion, timestamp, category) =>
                {
                    suggestion.timestamp = timestamp;
                    if (suggestion.categories == null)
                    {
                        suggestion.categories = new List<CategoryEntity>();
                    }
                    suggestion.categories.Add(category);
                    return suggestion;
                }, new { author_emp_id }, splitOn: "timestamp_id, category_id");
                var result = suggestions.GroupBy(s => s.suggestion_id).Select(suggestion =>
                {
                    var groupedSuggestion = suggestion.First();
                    groupedSuggestion.categories = suggestion.Select(s => s.categories.Single()).ToList();
                    return groupedSuggestion;
                });
                return result.ToList();
            }
        }
        public int CreateSuggestion(SuggestionEntity suggestion)
        {
            suggestion.suggestion_id = GetNewSuggestionID();
            var suggestionQuery = @"INSERT INTO Suggestion(title, description, status, ownership_emp_id, author_emp_id) VALUES (@title, @description, 'PLAN', @ownership_emp_id, @poster_emp_id)";
            var timestampquery = @"INSERT INTO SuggestionTimestamp(suggestion_id, dueByTimestamp) VALUES (@suggestion_id, @dueByTimestamp)";
            var cateogiresQuery = @"INSERT INTO SuggestionCategory(suggestion_id, category_id) VALUES (@suggestion_id, @categoryid)";


            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                connection.Execute(suggestionQuery, new { suggestion.title, suggestion.description, suggestion.ownership_emp_id, poster_emp_id = suggestion.author_emp_id });
                connection.Execute(timestampquery, new {suggestion_id = suggestion.suggestion_id, dueByTimestamp = suggestion.timestamp.dueByTimestamp});
                foreach (CategoryEntity category in suggestion.categories)
                {
                    connection.Execute(cateogiresQuery, new { suggestion_id = suggestion.suggestion_id, categoryid = category.category_id });
                }
            }
            return 1;
        }
        public int CreateComment(CommentEntity comment)
        {
            string query = @"INSERT INTO SuggestionComment(emp_id, suggestion_id, description) 
            VALUES(@emp_id, @suggestion_id, @description)";

            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                int result = connection.Execute(query, new { comment.emp_id, comment.suggestion_id, comment.description });
                return result;
            }
        }
        public int UpdateComment(CommentEntity comment)
        {
            throw new NotImplementedException();
        }
        public int DeleteComment(CommentEntity comment)
        {
            var query = @"DELETE FROM SuggestionComment WHERE comment_id = @comment_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var affectedRows = connection.Execute(query, new { comment.comment_id });
                return affectedRows;
            }
        }
        public List<CategoryEntity> GetAllCategories()
        {
            var query = @"SELECT Category.category_id, Category.category_name FROM Category";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var categories = connection.Query<CategoryEntity>(query);
                return categories.ToList();
            }
        }
        //metoder for bilder
        //Legg til bilder
        public int CreateImage(ImageEntity image)
        {
            var query = @"INSERT INTO Image(emp_id, suggestion_id, image_file) 
                VALUES (@emp_id, @suggestion_id, @image_file);";

            //starter connection
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                int result =
                connection.Execute(query, new { emp_id = image.emp_id, suggestion_id = image.suggestion_id, image_file = image.image_file});
                return result;
            }
        }
        //metode som henter alle bilder
        public List<ImageEntity> GetAllImages()
        {
            var query = @"SELECT * FROM Image;";

            //starter connection til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //lager variabel images bildene og hentes fra databasen
                var images = connection.Query<ImageEntity>(query);
                return images.ToList();
            }
        }
        //metode som henter et bilde ved bruk av bildeID
        public ImageEntity GetImage(int image_id)
        {
            var query = @"SELECT * FROM Image WHERE image_id = @image_id;";

            //starter connection til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //lager variabel img for et bilde og hentes fra databasen
                //referere image_id i parameter med image_id i databasen --> gjennom connection
                var img = connection.QueryFirstOrDefault<ImageEntity>(query, new { image_id = image_id });
                return img;
            }
        }
        //metode som sletter forslag ved bruk av bildeID
        public int DeleteImage(int image_id)
        {
            var query = @"DELETE FROM Image WHERE image_id = @image_id;";

            //starter connection til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //lager variabel result for antall rader slettet som hentes fra databasen gjennom query
                //referere image_id i parameter med image_id i databasen --> gjennom connection
                var result = connection.Execute(query, new { image_id = image_id });
                return result;
            }
        }
    }
}
