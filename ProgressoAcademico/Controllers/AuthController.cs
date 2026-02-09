using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.ViewModels;
using Microsoft.EntityFrameworkCore;
using ProgressoAcademico.Context;

namespace ProgressoAcademico.Controllers
{
    public class AuthApiController : Controller
    {
        private readonly ProgressoAcademicoDbContext _context;

        public AuthApiController(ProgressoAcademicoDbContext context)
        {
            _context = context;
        }

        // GET: /Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {

            // Validação básica do modelo , o que essa parte faz é verificar se os campos obrigatórios foram preenchidos e se os dados estão no formato correto
            if (!ModelState.IsValid)
                return View(model);

            // Aqui é onde ocorre a autenticação do usuário, o que essa parte faz é buscar no banco de dados um usuário que corresponda ao email fornecido,
            // que esteja ativo e que não tenha sido excluído. Se o usuário for encontrado, ele verifica se a senha fornecida corresponde à senha armazenada (que está hashada).
            // Se a senha for válida, o processo de login continua (ainda precisa ser implementado), caso contrário, uma mensagem de erro é exibida.
            var usuario = _context.Usuarios
                .Include(u => u.UsuarioPerfil)
                .Include(u => u.VinculoInstitucional)
                .FirstOrDefault(u =>
                    u.Email == model.Email &&
                    u.Ativo == true &&
                    u.DataExclusao == null
                );

            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuário não encontrado ou inativo.");
                return View(model);
            }

            bool senhaValida = BCrypt.Net.BCrypt.Verify(
                model.Senha,
                usuario.SenhaHash
            );

            if (!senhaValida)
            {
                ModelState.AddModelError("", "Senha inválida.");
                return View(model);
            }

            // 🔐 AQUI entra autenticação por cookie (próximo passo)
            // Por enquanto apenas simula login válido

            return RedirectToAction("Index", "Home");
        }
    }
}
