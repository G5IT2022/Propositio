using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories
{
    public interface ISuggestionRepository
    {
        /**
         * Suggestions
         * **/
        public void Favorite(int id, bool update);
        public int CreateSuggestion(SuggestionEntity suggestion);
        public SuggestionEntity GetSuggestionBySuggestionID(int suggestion_id);
        public SuggestionEntity GetSuggestionBySuggestionIDWithComments(int suggestion_id);
        public List<SuggestionEntity> GetSuggestionsByAuthorID(int author_emp_id);
        public List<SuggestionEntity> GetAll();

        /**
         * Categories
         * **/
        public List<CategoryEntity> GetAllCategories();


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
        /// <param name="comment">A comment entity</param>
        /// <returns>An integer of the number of rows affected, 1 if successful</returns>
        public int DeleteComment(CommentEntity comment);
    }
}
