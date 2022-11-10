using bacit_dotnet.MVC.DataAccess;
using bacit_dotnet.MVC.Entities;
using Dapper;
using Dapper.Contrib.Extensions;
using MySqlConnector;

namespace bacit_dotnet.MVC.Repositories
{
    /**
     * Dette repositoryet har ansvaret for alt det som Suggestion, Category, Comment og Timestamp repositoryene gjorde før
     * Den samler alt på ett sted.
     * **/
    public class SuggestionRepository : ISuggestionRepository
    {
        private readonly ISqlConnector sqlConnector;

        public SuggestionRepository(ISqlConnector sqlConnector)
        {
            this.sqlConnector = sqlConnector;
        }

        /**
         * Denne metoden henter alle forslagene i databasen komplett med kategorier og timestamp. 
         * **/
        public List<SuggestionEntity> GetAll()
        {
            //spørring til databasen
            var query = @"SELECT s.*, sts.*, c.*, i.* FROM Suggestion AS s INNER JOIN 
            SuggestionTimestamp AS sts ON s.suggestion_id = sts.suggestion_id INNER JOIN 
            SuggestionCategory AS sc ON sc.suggestion_id = s.suggestion_id INNER JOIN 
            Category AS c ON sc.category_id = c.category_id LEFT JOIN Image AS i on i.suggestion_id = s.suggestion_id";

            //kobling til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //koble spørring til databasen for å hente informasjon
                var suggestions = connection.Query<SuggestionEntity, TimestampEntity, CategoryEntity, ImageEntity, SuggestionEntity>(query, (suggestion, timestamp, category, image) =>
                {
                    //variabel "suggestion.timestamp" i koden kobles med variabel timestamp i databasen
                    suggestion.timestamp = timestamp;

                    //hvis det er ingen kategorier i kategori-lista, lages det en ny kategori-liste
                    if (suggestion.categories == null)
                    {
                        suggestion.categories = new List<CategoryEntity>();
                    }
                    if (suggestion.images == null)
                    {
                        suggestion.images = new List<ImageEntity>();
                    }
                    //legg til kategori i kategori-liste for forslag
                    suggestion.categories.Add(category);
                    suggestion.images.Add(image);

                    //returner forslag
                    return suggestion;
                }, splitOn: "timestamp_id, category_id, image_id");

                //grupperer alle forslagene
                var result = suggestions.GroupBy(s => s.suggestion_id).Select(suggestion =>
                {
                    var groupedSuggestion = suggestion.First();
                    groupedSuggestion.categories = suggestion.Select(e => e.categories.Single()).ToList();
                    //groupedSuggestion.images = suggestion.Select(e => e.images.Single()).ToList();
                    return groupedSuggestion;
                });
                //returnerer en liste med forslag
                return result.ToList();
            }
        }

        /**
         * Denne metoden gjør at du kan oppdatere om et forslag er favoritt eller ikke i databasen. 
         * **/
        public void Favorite(int id, bool update)
        {
            var updateQuery = @"UPDATE Suggestion SET favorite = @state WHERE suggestion_id = @suggestion_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                connection.Execute(updateQuery, new { state = update, suggestion_id = id });
            }

        }

        /**
         * Denne metoden henter en ny forslags id. 
         * Trengs for å lage nytt forslag.
         * **/
        private int GetNewSuggestionID()
        {
            var query = @"SELECT COUNT(*) FROM Suggestion";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var count = connection.QueryFirst<int>(query);
                return count + 1;
            }
        }

        /**
         * Denne metoden henter et forslag basert på suggestion_id
         * **/
        public SuggestionEntity GetSuggestionBySuggestionID(int suggestion_id)
        {
            //spørring
            var query = @"SELECT s.*, sts.*, c.* FROM Suggestion AS s INNER JOIN 
            SuggestionTimestamp AS sts ON s.suggestion_id = sts.suggestion_id INNER JOIN 
            SuggestionCategory AS sc ON sc.suggestion_id = s.suggestion_id INNER JOIN 
            Category AS c ON sc.category_id = c.category_id WHERE s.suggestion_id = @suggestion_id";

            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen
                var suggestions = connection.Query<SuggestionEntity, TimestampEntity, CategoryEntity, SuggestionEntity>(query, (suggestion, timestamp, category) =>
                {
                    //kobler variabel suggestion.timestamp i koden med variabel timestamp i databasen
                    suggestion.timestamp = timestamp;

                    //hvis det er ingen kategorier for forslaget i kategori-lista, lages det ny liste for kategorier
                    if (suggestion.categories == null)
                    {
                        suggestion.categories = new List<CategoryEntity>();
                    }
                    //legg til kategori for forslag
                    suggestion.categories.Add(category);

                    //returner forslag
                    return suggestion;
                }, new { suggestion_id }, splitOn: "timestamp_id, category_id");
                //grupperer forslagene
                var result = suggestions.GroupBy(s => s.suggestion_id).Select(suggestion =>
                {
                    var groupedSuggestion = suggestion.First();
                    groupedSuggestion.categories = suggestion.Select(s => s.categories.Single()).ToList();
                    return groupedSuggestion;
                });
                //returner første element i forslagslista
                return result.ElementAt(0);
            }
        }

        //metode som henter detaljer for forslag
        //Detaljene er info, kommentarer og bilder som tilhører forslaget
        public SuggestionEntity GetSuggestionBySuggestionIDWithCommentsAndImages(int suggestion_id)
        {
            //spørring til databasen
            var query = @"SELECT s.*, sts.*, c.*, co.*, i.* FROM Suggestion AS s 
            INNER JOIN SuggestionTimestamp AS sts ON s.suggestion_id = sts.suggestion_id 
            INNER JOIN SuggestionCategory AS sc ON sc.suggestion_id = s.suggestion_id 
            INNER JOIN Category AS c ON sc.category_id = c.category_id 
            LEFT JOIN SuggestionComment AS co ON co.suggestion_id = s.suggestion_id 
            LEFT JOIN Image as i ON i.suggestion_id = s.suggestion_id WHERE s.suggestion_id = @suggestion_id";
            //hele forslaget
            var fullSuggestion = new SuggestionEntity();

            //connection til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //variabel "suggestions" som henter detaljer fra databasen
                var suggestions = connection.Query<SuggestionEntity, TimestampEntity, CategoryEntity, CommentEntity, ImageEntity, SuggestionEntity>(query, (suggestion, timestamp, category, comment, image) =>
                {
                    //linker variabel i kode med variabel i databasen
                    suggestion.timestamp = timestamp;
                    suggestion.categories.Add(category);
                    suggestion.comments.Add(comment);
                    suggestion.images.Add(image);
                    return suggestion; // returnere forslag
                    //splitOn deler opp elementene i "" under
                }, new { suggestion_id = suggestion_id }, splitOn: "timestamp_id, category_id, comment_id, image_id");

                //henter første element i lista
                fullSuggestion = suggestions.ElementAt(0);
                //foreach-loop for forslagene i lista
                foreach (SuggestionEntity suggestion in suggestions)
                {
                    //foreach-loop for kommentarer i lista tilhørende forslaget
                    foreach (CommentEntity comment in suggestion.comments)
                    {
                        if (!fullSuggestion.comments.Contains(comment))
                        {
                            fullSuggestion.comments.Add(comment);
                        }
                    }
                    //foreach-loop for bilder i lista tilhørende forslaget
                    foreach (ImageEntity image in suggestion.images)
                    {
                        if (!fullSuggestion.images.Contains(image))
                        {
                            fullSuggestion.images.Add(image);
                        }
                    }
                    //foreach-loop for kategorier i lista tilhørende forslaget
                    foreach (CategoryEntity category in suggestion.categories)
                    {
                        if (!fullSuggestion.categories.Contains(category))
                        {
                            fullSuggestion.categories.Add(category);
                        }
                    }
                }
                //grupperer bildene i forslaget
                if (fullSuggestion.images.Count > 0 && fullSuggestion.images.ElementAt(0) != null)
                {

                    var groupedImages = fullSuggestion.images.GroupBy(image => image.image_id).Select(image => image.First()).ToList();
                    fullSuggestion.images = groupedImages;
                }
                //grupperer kommentarene i forslaget
                if (fullSuggestion.comments.Count > 0 && fullSuggestion.comments.ElementAt(0) != null)
                {
                    var groupedComments = fullSuggestion.comments.GroupBy(comment => comment.comment_id).Select(comment => comment.First()).ToList();
                    fullSuggestion.comments = groupedComments;
                }
                //returner hele forslaget
                return fullSuggestion;
            }
        }

        /**
         * Denne metoden henter forslagene fra en forfatter
         */
        public List<SuggestionEntity> GetSuggestionsByAuthorID(int author_emp_id)
        {
            //spørring
            var query = @"SELECT s.*, sts.*, c.*, i.* FROM Suggestion AS s INNER JOIN 
            SuggestionTimestamp AS sts ON s.suggestion_id = sts.suggestion_id  RIGHT JOIN 
            SuggestionCategory AS sc ON sc.suggestion_id = s.suggestion_id INNER JOIN 
            Category AS c ON sc.category_id = c.category_id  LEFT JOIN Image as i on i.suggestion_id = s.suggestion_id WHERE s.author_emp_id = @author_emp_id";

            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen
                var suggestions = connection.Query<SuggestionEntity, TimestampEntity, CategoryEntity, ImageEntity, SuggestionEntity>(query, (suggestion, timestamp, category, image) =>
                {
                    //variabel timestamp i koden kobles til variabel timestamp i databasen
                    suggestion.timestamp = timestamp;
                    //Hvis det er ingen kategorier i kategori-lista for forslaget, lages det en ny
                    if (suggestion.categories == null)
                    {
                        suggestion.categories = new List<CategoryEntity>();
                    }


                    if (suggestion.images == null)
                    {
                        suggestion.images = new List<ImageEntity>();
                    }
                    //legg til kategori for forslaget
                    suggestion.categories.Add(category);
                    //legg til bilde for forslaget
                    suggestion.images.Add(image);

                    //returner forslaget
                    return suggestion;
                }, new { author_emp_id }, splitOn: "timestamp_id, category_id, image_id");

                //grupper forslagene
                var result = suggestions.GroupBy(s => s.suggestion_id).Select(suggestion =>
                {
                    var groupedSuggestion = suggestion.First();
                    groupedSuggestion.categories = suggestion.Select(s => s.categories.Single()).ToList();
                    groupedSuggestion.images = suggestion.Select(s => s.images.Single()).ToList();
                    return groupedSuggestion;
                });
                //returner forslagene til forfatteren
                return result.ToList();
            }
        }

        /**
         * Denne metoden lager nytt forslag
         */
        public int CreateSuggestion(SuggestionEntity suggestion)
        {
            //spørring
            suggestion.suggestion_id = GetNewSuggestionID();
            var suggestionQuery = @"INSERT INTO Suggestion(title, description, status, ownership_emp_id, author_emp_id) VALUES (@title, @description, 'PLAN', @ownership_emp_id, @poster_emp_id)";
            var timestampquery = @"INSERT INTO SuggestionTimestamp(suggestion_id, dueByTimestamp) VALUES (@suggestion_id, @dueByTimestamp)";
            var categoriesQuery = @"INSERT INTO SuggestionCategory(suggestion_id, category_id) VALUES (@suggestion_id, @categoryid)";
            var imageQuery = @"INSERT INTO Image(emp_id, suggestion_id, image_filepath) VALUES(@emp_id, @suggestion_id, @image_filepath)";

            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen
                connection.Execute(suggestionQuery, new { suggestion.title, suggestion.description, suggestion.ownership_emp_id, poster_emp_id = suggestion.author_emp_id });
                connection.Execute(timestampquery, new { suggestion_id = suggestion.suggestion_id, dueByTimestamp = suggestion.timestamp.dueByTimestamp });

                //foreach-loop for kategorier
                foreach (CategoryEntity category in suggestion.categories)
                {
                    //kobler kategori-spørring til databasen (kobler hver kategori gjeldende for forslaget til forslaget)
                    connection.Execute(categoriesQuery, new { suggestion_id = suggestion.suggestion_id, categoryid = category.category_id });
                }
                if(suggestion.images.Count > 0 && suggestion.images.ElementAt(0) != null)
                {
                    foreach(ImageEntity image in suggestion.images)
                    {
                    connection.Execute(imageQuery, new {emp_id = suggestion.author_emp_id, suggestion_id = suggestion.suggestion_id, image_filepath = image.image_filepath });
                    }
                }
            }
            //returnerer tallverdi på 1
            return 1;
        }
        /**
         * Denne metoden lager et nytt kommentar
         */
        public int CreateComment(CommentEntity comment)
        {
            //spørring
            string query = @"INSERT INTO SuggestionComment(emp_id, suggestion_id, description) 
            VALUES(@emp_id, @suggestion_id, @description)";

            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler kommentar-spørring til databasen (kobler hver kommentar gjeldende for forslaget til forslaget
                int result = connection.Execute(query, new { comment.emp_id, comment.suggestion_id, comment.description });
                return result;
            }
        }
        /**
         * Denne metoden oppdaterer kommentar --> ikke helt ferdig
         */
        public int UpdateComment(CommentEntity comment)
        {
            //Exception
            throw new NotImplementedException();
        }

        /**
         * Denne metoden sletter en kommentar
         */
        public int DeleteComment(int comment_id)
        {
            //spørring
            var query = @"DELETE FROM SuggestionComment WHERE comment_id = @comment_id";

            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen - viser til hvilke rader som blir påvirket i databasen
                var affectedRows = connection.Execute(query, new { comment_id = comment_id });

                //returnerer antall rader påvirket av å bli slettet
                return affectedRows;
            }
        }

        /**
         * Denne metoder henter alle kategoriene
         */
        public List<CategoryEntity> GetAllCategories()
        {
            //spørring
            var query = @"SELECT Category.category_id, Category.category_name FROM Category";
            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen
                var categories = connection.Query<CategoryEntity>(query);
                //returnerer alle kategorier --> kategoriliste
                return categories.ToList();
            }
        }
        //metoder for bilder
        //Legg til bilder
        public int CreateImage(ImageEntity image)
        {
            var query = @"INSERT INTO Image(emp_id, suggestion_id, image_file) 
                VALUES (@emp_id, @suggestion_id, @image_file);";

            //starter connection til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                int result =
                connection.Execute(query, new { emp_id = image.emp_id, suggestion_id = image.suggestion_id, image_file = image.image_filepath });
                //returnerer tallverdi på antall rader påvirket
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
                //returnerer alle bilder - bilde-liste
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
                //returnerer bildet
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
                //returnerer antall rader påvirket
                return result;
            }
        }
        //Henter forslag basert på status
        public List<SuggestionEntity> GetSuggestionsByStatus(string status)
        {
            //spørring
            var query = @"SELECT s.*, sts.*, c.*, i.* FROM Suggestion AS s INNER JOIN 
            SuggestionTimestamp AS sts ON s.suggestion_id = sts.suggestion_id  RIGHT JOIN 
            SuggestionCategory AS sc ON sc.suggestion_id = s.suggestion_id INNER JOIN 
            Category AS c ON sc.category_id = c.category_id  LEFT JOIN Image as i on i.suggestion_id = s.suggestion_id WHERE s.status = @status";

            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen
                var suggestions = connection.Query<SuggestionEntity, TimestampEntity, CategoryEntity, ImageEntity, SuggestionEntity>(query, (suggestion, timestamp, category, image) =>
                {
                    //variabel timestamp i koden kobles til variabel timestamp i databasen
                    suggestion.timestamp = timestamp;
                    //Hvis det er ingen kategorier i kategori-lista for forslaget, lages det en ny
                    if (suggestion.categories == null)
                    {
                        suggestion.categories = new List<CategoryEntity>();
                    }


                    if (suggestion.images == null)
                    {
                        suggestion.images = new List<ImageEntity>();
                    }
                    //legg til kategori for forslaget
                    suggestion.categories.Add(category);
                    //legg til bilde for forslaget
                    suggestion.images.Add(image);

                    //returner forslaget
                    return suggestion;
                }, new { status = status }, splitOn: "timestamp_id, category_id, image_id");

                //grupper forslagene
                var result = suggestions.GroupBy(s => s.suggestion_id).Select(suggestion =>
                {
                    var groupedSuggestion = suggestion.First();
                    groupedSuggestion.categories = suggestion.Select(s => s.categories.Single()).ToList();
                    groupedSuggestion.images = suggestion.Select(s => s.images.Single()).ToList();
                    return groupedSuggestion;
                });
                //returner forslagene basert på status
                return result.ToList();
            }
        }
        //Oppdaterer et forslag med en suggestionentity. Returnerer antall rader som er blitt endret
        public int UpdateSuggestion(SuggestionEntity suggestion)
        {
            //Query for oppdatering av beskrivelse, status og eier for ett forslag. 
            var query = @"UPDATE Suggestion SET description = @description, status = @status, ownership_emp_id = @ownership_emp_id WHERE suggestion_id = @suggestion_id";
            //Query for å oppdatere timetamps.
            var updateTimestampQuery = @"UPDATE SuggestionTimestamp SET dueByTimestamp = @dueByTimestamp, lastUpdatedTimestamp = @lastUpdatedTimestamp WHERE suggestion_id = @suggestion_id";

            //starter connection til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //lager variabel result for antall rader som blir endret i databasen gjennom query
                var result = connection.Execute(query, new { description = suggestion.description, status = suggestion.status.ToString(), ownership_emp_id = suggestion.ownership_emp_id, suggestion_id = suggestion.suggestion_id});
                result += connection.Execute(updateTimestampQuery, new { dueByTimestamp = suggestion.timestamp.dueByTimestamp, lastUpdatedTimestamp = suggestion.timestamp.lastUpdatedTimestamp, suggestion_id = suggestion.suggestion_id });
                //returnerer antall rader påvirket
                return result;
            }
        }

        //Oppdaterer status på ett forslag skal returnere 2 rader

        public int UpdateSuggestionStatus(int suggestion_id, string status)
        {
            var newTimestampName = status.ToLower() + "Timestamp";
            var query = @"UPDATE Suggestion SET status = @status WHERE suggestion_id = @suggestion_id";
            var timestampQuery = String.Format(@"UPDATE SuggestionTimestamp SET {0} = CURRENT_TIMESTAMP, lastUpdatedTimestamp = CURRENT_TIMESTAMP WHERE suggestion_id = @suggestion_id", newTimestampName);
                
               // @"$UPDATE SuggestionTimestamp SET {newTimestampName} = CURRENT_TIMESTAMP, lastUpdatedTimestamp = CURRENT_TIMESTAMP  WHERE suggestion_id = @suggestion_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var result = connection.Execute(query, new { suggestion_id = suggestion_id, status = status });
                result += connection.Execute(timestampQuery, new { timestampName = newTimestampName, suggestion_id = suggestion_id });
                return result;
            }
        }
    }
}
