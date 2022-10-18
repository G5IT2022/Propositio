using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories.Comment
{
    public interface ICommentRepository
    {
        /// <summary>
        /// Creates a new comment in the database
        /// </summary>
        /// <param name="comment">A comment entity</param>
        /// <returns>An integer of the number of rows affected, 1 if successful</returns>
        public int Create(CommentEntity comment);
        /// <summary>
        /// Retrieves all the comments from the database as a list of comment entities
        /// </summary>
        /// <returns>A list of CommentEntity</returns>
        public int GetNewCommentID();
        public List<CommentEntity> GetAll();
        /// <summary>
        /// Retrieves a single comment from the database with a matching comment_id
        /// </summary>
        /// <param name="comment_id">A comment id</param>
        /// <returns>A CommentEntity</returns>
        public CommentEntity Get(int comment_id);
        /// <summary>
        /// Retrieves a list of comments for a suggestion
        /// </summary>
        /// <param name="suggestion_id">A suggestion id</param>
        /// <returns>A list of CommentEntity</returns>
        public List<CommentEntity> GetCommentsForSuggestion(int suggestion_id);
        /// <summary>
        /// Updates a comment in the database
        /// </summary>
        /// <param name="comment">A comment entity</param>
        /// <returns>An integer of the number of rows affected, 1 if successful</returns>
        public int Update(CommentEntity comment);
        /// <summary>
        /// Deletes a comment from the database
        /// </summary>
        /// <param name="comment">A comment entity</param>
        /// <returns>An integer of the number of rows affected, 1 if successful</returns>
        public int Delete(CommentEntity comment);
    }
}
