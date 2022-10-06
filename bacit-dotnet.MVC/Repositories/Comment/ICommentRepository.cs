using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories.Comment
{
    public interface ICommentRepository
    {
        public List<CommentEntity> GetAll();
    }
}
