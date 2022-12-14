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
     */
    public class SuggestionRepository : ISuggestionRepository
    {
        private readonly ISqlConnector sqlConnector;

        //Constructor med dependency injection
        public SuggestionRepository(ISqlConnector sqlConnector)
        {
            this.sqlConnector = sqlConnector;
        }

        /**
         * Denne metoden er for å lage et nytt forslag
         * @Parameter SuggestionEntity
         * @Return ???
         */
        public int CreateSuggestion(SuggestionEntity suggestion)
        {
            //spørring
            suggestion.suggestion_id = GetNewSuggestionID();
            var suggestionQuery = @"INSERT INTO Suggestion(title, description, status, ownership_emp_id, author_emp_id) VALUES (@title, @description, @status, @ownership_emp_id, @poster_emp_id)";
            var timestampquery = @"INSERT INTO SuggestionTimestamp(suggestion_id, dueByTimestamp) VALUES (@suggestion_id, @dueByTimestamp)";
            var categoriesQuery = @"INSERT INTO SuggestionCategory(suggestion_id, category_id) VALUES (@suggestion_id, @categoryid)";
            var imageQuery = @"INSERT INTO Image(emp_id, suggestion_id, image_filepath) VALUES(@emp_id, @suggestion_id, @image_filepath)";

            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen
                connection.Execute(suggestionQuery, new { suggestion.title, suggestion.description, status = suggestion.status.ToString(), suggestion.ownership_emp_id, poster_emp_id = suggestion.author_emp_id });
                connection.Execute(timestampquery, new { suggestion_id = suggestion.suggestion_id, dueByTimestamp = suggestion.timestamp.dueByTimestamp });

                //foreach-loop for kategorier
                foreach (CategoryEntity category in suggestion.categories)
                {
                    //kobler kategori-spørring til databasen (kobler hver kategori gjeldende for forslaget til forslaget)
                    connection.Execute(categoriesQuery, new { suggestion_id = suggestion.suggestion_id, categoryid = category.category_id });
                }
                if (suggestion.images.Count > 0 && suggestion.images.ElementAt(0) != null)
                {
                    foreach (ImageEntity image in suggestion.images)
                    {
                        connection.Execute(imageQuery, new { emp_id = suggestion.author_emp_id, suggestion_id = suggestion.suggestion_id, image_filepath = image.image_filepath });
                    }
                }
            }
            //returnerer tallverdi på 1
            return 1;
        }

        /*
        * Denne metoden henter alle forslagene i databasen komplett med kategorier og timestamp. 
        * @Return en liste med forslag
        */
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
        * Denne metoden gjør at du kan hente detaljer av ett forslag
        * Detaljene er info, bilder, kommentarer som tilhører forslaget
        * @Parameter suggestion_id
        * @Return SuggestionEntity med 
        */
        public SuggestionEntity GetSuggestionBySuggestionID(int suggestion_id)
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
                //Grupperer kategoriene
                if (fullSuggestion.categories.Count > 0 && fullSuggestion.categories.ElementAt(0) != null)
                {
                    var groupedCategories = fullSuggestion.categories.GroupBy(category => category.category_id).Select(category => category.First()).ToList();
                    fullSuggestion.categories = groupedCategories;
                }
                //returner hele forslaget
                return fullSuggestion;
            }
        }

        /**
        * Denne metoden henter forslagene fra en forfatter
        * @Parameter author_emp_id
        * @Return forslagene til forfatteren
        */
        public List<SuggestionEntity> GetSuggestionsByAuthorID(int author_emp_id)
        {
            //spørring
            var query = @"SELECT s.*, sts.*, c.*, i.* FROM Suggestion AS s INNER JOIN 
            SuggestionTimestamp AS sts ON s.suggestion_id = sts.suggestion_id  RIGHT JOIN 
            SuggestionCategory AS sc ON sc.suggestion_id = s.suggestion_id INNER JOIN 
            Category AS c ON sc.category_id = c.category_id  LEFT JOIN 
            Image as i on i.suggestion_id = s.suggestion_id WHERE s.author_emp_id = @author_emp_id";

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
         * Denne metoden er for å oppdatere et forslag med en suggestion entity. 
         * @Parameter SuggestionEntity 
         * @Return antall rader som er blit endret
         */
        public int UpdateSuggestion(SuggestionEntity suggestion)
        {
            //Query for oppdatering av beskrivelse, status og eier for ett forslag. 
            var query = @"UPDATE Suggestion SET description = @description, ownership_emp_id = @ownership_emp_id WHERE suggestion_id = @suggestion_id";
            //Query for å oppdatere timetamps.
            var updateTimestampQuery = @"UPDATE SuggestionTimestamp SET dueByTimestamp = @dueByTimestamp, lastUpdatedTimestamp = @lastUpdatedTimestamp WHERE suggestion_id = @suggestion_id";

            //starter connection til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //lager variabel result for antall rader som blir endret i databasen gjennom query
                var result = connection.Execute(query, new { description = suggestion.description, ownership_emp_id = suggestion.ownership_emp_id, suggestion_id = suggestion.suggestion_id });
                result += connection.Execute(updateTimestampQuery, new { dueByTimestamp = suggestion.timestamp.dueByTimestamp, lastUpdatedTimestamp = suggestion.timestamp.lastUpdatedTimestamp, suggestion_id = suggestion.suggestion_id });
                //returnerer antall rader påvirket
                return result;
            }
        }

        /**
         * Denne metoden er for å oppdatere status på ett forslag.
         * @Parameter suggestion_id og string status
         * @Return 2 rader
         */
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


        /**
         * Denne metoden gjør at du kan oppdatere om et forslag er favoritt eller ikke i databasen. 
         * @Parameter int id og bool update
         */
        public void Favorite(int suggestion_id, bool update)
        {
            //spørring
            var updateQuery = @"UPDATE Suggestion SET favorite = @state WHERE suggestion_id = @suggestion_id";
            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                connection.Execute(updateQuery, new { state = update, suggestion_id = suggestion_id });
            }

        }

        /**
         * Denne metoden henter en ny forslags id. 
         * Trengs for å lage nytt forslag.
         */
        private int GetNewSuggestionID()
        {
            //spørring
            var query = @"SELECT COUNT(*) FROM Suggestion";
            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var count = connection.QueryFirst<int>(query);
                return count + 1;
            }
        }
     
        /**
         * Denne metoden gjør at du kan lage en ny kommentar i ett forslag
         * @Parameter CommentEntity
         * @Return en ny kommentar
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
            var query = @"UPDATE SuggestionComment SET description = @description, lastUpdatedTimestamp = @lastUpdatedTimestamp WHERE comment_id = @comment_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen - viser til hvilke rader som blir påvirket i databasen
                var affectedRows = connection.Execute(query, new { description = comment.description, lastUpdatedTimestamp = comment.lastUpdatedTimestamp, comment_id = comment.comment_id });

                //returnerer antall rader påvirket
                return affectedRows;
            }
        }

        /**
         * Denne metoden gjør at du kan slette kommentarer i ett forslag
         * @Parameter comment_id
         * @Return ???
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

    
    }
}
