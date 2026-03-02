using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.ViewModels;

namespace ProgressoAcademico.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string senha)
        {
            //  depois voc� liga com autentica��o real
            if (email == "admin@ufabc.edu.br" && senha == "123")
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Erro = "Usu�rio ou senha inv�lidos";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // TODO: persistir usuário (mock for now)
            // In a real app, create the user and redirect to login or auto-login.

            TempData["SuccessMessage"] = "Conta criada com sucesso. Faça login.";
            return RedirectToAction("Login");
        }

        // -----------------------------------------------------------
        // Esqueceu senha
        // -----------------------------------------------------------

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // TODO: implementar envio de e-mail real. Por enquanto apenas mostra mensagem.
            TempData["SuccessMessage"] =
                "Se o e-mail estiver cadastrado, você receberá instruções para redefinir sua senha.";

            // manter o usuário na mesma página para verificação
            return View();
        }
    }
}
