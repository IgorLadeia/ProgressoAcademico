//using Microsoft.AspNetCore.Mvc;

//public class HomeController : Controller
//{
//    public IActionResult Index()
//    {
//        var model = new HomeViewModel
//        {
//            User = new UserSummaryViewModel
//            {
//                Name = "Igor Ladeia",
//                Role = "Graduando em Engenharia",
//                AvatarUrl = "/images/user-default.png"
//            },
//            Progresses = new List<AcademicProgressViewModel>
//            {
//                new AcademicProgressViewModel
//                {
//                    Id = 1,
//                    Title = "Dissertação de Mestrado",
//                    Description = "Elaboração da dissertação.",
//                    CreatedAt = DateTime.Now.AddDays(-10),
//                    StatusDescription = "Em Andamento",
//                    StatusCss = "status-progress",
//                    CanEdit = true,
//                    CanDelete = false,
//                    IsFinalized = false
//                },
//                new AcademicProgressViewModel
//                {
//                    Id = 2,
//                    Title = "Estágio Curricular",
//                    Description = "Estágio obrigatório.",
//                    CreatedAt = DateTime.Now.AddMonths(-2),
//                    StatusDescription = "Finalizado",
//                    StatusCss = "status-finalized",
//                    CanEdit = false,
//                    CanDelete = false,
//                    IsFinalized = true
//                }
//            }
//        };

//        ViewBag.UserName = model.User.Name;
//        ViewBag.UserRole = model.User.Role;

//        return View(model);
//    }
//}
