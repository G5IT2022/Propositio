using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories
{
    public interface ISuggestionRepository
    {
        /**
         * Interface for forslag repositoryet. Metodene er organisert etter CRUD (Create, Read, Update, Delete) og div på slutten
         * **/

        /*
         * Suggestion 
         */

        //  Create

        /// <summary>
        /// Creates a new suggestion in the database
        /// </summary>
        /// <param name="suggestion">A suggestion entity</param>
        /// <returns></returns>
        public int CreateSuggestion(SuggestionEntity suggestion);

        //  Read

        /// <summary>
        /// Returns a list of all suggestions in the db, fully mapped with categories, images and timestamps
        /// </summary>
        /// <returns>A list of suggestion entities</returns>
        public List<SuggestionEntity> GetAll();

        /// <summary>
        /// Returns a single suggestion from the db, fully mapped with images, comments, categories and timestamps
        /// </summary>
        /// <param name="suggestion_id">The ID of a suggestion</param>
        /// <returns>A list of suggestion entities</returns>
        public SuggestionEntity GetSuggestionBySuggestionID(int suggestion_id);

        /// <summary>
        /// Returns a list of all suggestions by the authorID specified, fully mapped with categories, images and timestamps
        /// </summary>
        /// <param name="author_emp_id">An employee ID for the author of the suggestion</param>
        /// <returns>A list of suggestion entities</returns>
        public List<SuggestionEntity> GetSuggestionsByAuthorID(int author_emp_id);


        //  Update

        /// <summary>
        /// Updates a suggestion in the db
        /// </summary>
        /// <param name="suggestion">A suggestionentity to map the updates to the db from</param>
        /// <returns>An int representing the number of rows affected<returns>
        public int UpdateSuggestion(SuggestionEntity suggestion);
        /// <summary>
        /// Updates the status of a suggestion in the database
        /// </summary>
        /// <param name="suggestion_id">The ID of a suggestion</param>
        /// <param name="status">A string converted from the STATUS enum representing the different statuses that suggestions can have</param>
        /// <returns>An int representing the number of rows affected<returns>
        public int UpdateSuggestionStatus(int suggestion_id, string status);

        /// <summary>
        /// Updates whether or not a suggestion is favorited in the db
        /// </summary>
        /// <param name="suggestion_id">The ID of a suggestion</param>
        /// <param name="update">a boolean representing if the suggestion favorite property should be true or false</param>
        /// <returns>An int representing the number of rows affected<returns>
        public void Favorite(int suggestion_id, bool update);

   


        /**
         * Comments
         * **/

        /// <summary>
        /// Creates a new comment in the database
        /// </summary>
        /// <param name="comment">A comment entity</param>
        /// <returns>An integer of the number of rows affected, 1 if successful</returns>
        public int CreateComment(CommentEntity comment);

        /// <summary>
        /// Updates a comment in the database
        /// </summary>
        /// <param name="comment">A comment entity</param>
        /// <returns>An integer of the number of rows affected, 1 if successful</returns>
        public int UpdateComment(CommentEntity comment);
        /// <summary>
        /// Deletes a comment from the database
        /// </summary>
        /// <param name="comment_id">A comment id</param>
        /// <returns>An integer of the number of rows affected, 1 if successful</returns>
        public int DeleteComment(int comment_id);
    }
}
