//using Microsoft.AspNetCore.Mvc;

//namespace ProgressoAcademico.Controllers
//{
//    public class AccountController : Controller
//    {
//        [HttpGet]
//        public IActionResult Login()
//        {
//            return View();
//        }

//        [HttpPost]
//        public IActionResult Login(string email, string senha)
//        {
//            //  depois você liga com autenticação real
//            if (email == "admin@ufabc.edu.br" && senha == "123")
//            {
//                return RedirectToAction("Index", "Home");
//            }

//            ViewBag.Erro = "Usuário ou senha inválidos";
//            return View();
//        }
//    }
//}
