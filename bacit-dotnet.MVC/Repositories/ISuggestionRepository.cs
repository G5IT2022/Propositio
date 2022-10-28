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
        public SuggestionEntity GetSuggestionBySuggestionIDWithCommentsAndImages(int suggestion_id);
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

        /**
         * Images
         */

        /// <summary>
        /// Create new image in database
        /// </summary>
        /// <param name="image">image</param>
        /// <returns>An integer of number of rows affected, 1 if successful</returns>
        public int CreateImage(ImageEntity image);
        /// <summary>
        /// Retrieves all the images in the database as a list of image entities
        /// </summary>
        /// <returns>List of image entities</returns>
        public List<ImageEntity> GetAllImages();
        /// <summary>
        /// Retrieves a single image from the database matching the image_id
        /// </summary>
        /// <param name="image_id"></param>
        /// <returns>one image</returns>
        public ImageEntity GetImage(int image_id);
        /// <summary>
        /// Delete image with matching image_id
        /// </summary>
        /// <param name="image"></param>
        /// <returns>An integer for number of rows affected</returns>
        public int DeleteImage(int image_id);
    }
}
