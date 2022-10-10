using bacit_dotnet.MVC.Models;
using bacit_dotnet.MVC.Repositories.Comment;
using Microsoft.AspNetCore.Mvc;

namespace bacit_dotnet.MVC.Controllers
{
    public class CommentController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICommentRepository commentRepository;

        //Constructor
        public CommentController(ILogger<HomeController> logger, ICommentRepository commentRepository)
        {
            _logger = logger;
            this.commentRepository = commentRepository;
        }

        public IActionResult Index()
        {
            CommentViewModel model = new CommentViewModel();
            model.comments = commentRepository.GetAll();
            return View(model);
        }
    }
}
