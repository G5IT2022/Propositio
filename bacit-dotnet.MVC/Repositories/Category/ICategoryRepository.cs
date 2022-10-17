using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories.Category
{

    public interface ICategoryRepository
    {
        /// <summary>
        /// Creates a new category in the database
        /// </summary>
        /// <param name="category">A category entity</param>
        /// <returns>An integer of the number of rows affected, 1 if successful</returns>
        public int Create(CategoryEntity category);
        /// <summary>
        /// Retrieves all the categories from the database as a list of category entities
        /// </summary>
        /// <returns>A list of CategoryEntity</returns>
        public List<CategoryEntity> GetAll();
        /// <summary>
        /// Retrieves a single CategoryEntity from the database
        /// </summary>
        /// <param name="category_id">A category ID</param>
        /// <returns>A single category entity</returns>
        public CategoryEntity Get(int category_id);
        /// <summary>
        /// Retrieves all the categories for a single suggestion from the database
        /// </summary>
        /// <param name="suggestion_id">The id of the suggestion</param>
        /// <returns>A list of every category that belongs to the suggestion</returns>
        public List<CategoryEntity> GetCategoriesForSuggestion(int suggestion_id);
        /// <summary>
        /// Updates the name of a category
        /// </summary>
        /// <param name="category">A category entity</param>
        /// <returns>An integer of the number of rows affected, 1 if successful</returns>
        public int Update(CategoryEntity category);
        /// <summary>
        /// Deletes a category from the database
        /// </summary>
        /// <param name="category">A category entity</param>
        /// <returns>An integer of the number of rows affected, 1 if successful</returns>
        public int Delete(CategoryEntity category);

       
    }

}

