using ProgressoAcademico.Application.DTOs.Usuarios;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.Cadastro;
using ProgressoAcademico.Models.ViewModels.ChangePassword;
using ProgressoAcademico.Models.ViewModels.Register;
using ProgressoAcademico.Models.ViewModels.ProfessorPerfil;

namespace ProgressoAcademico.Services.Usuarios;

public class UsuarioService : IUsuarioService
{
    private const long TamanhoMaximoFotoBytes = 2 * 1024 * 1024;
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<CriarUsuarioResponseDto?> CriarProfessorAsync(RegisterViewModel dto)
    {
        if (await _usuarioRepository.EmailExisteAsync(dto.Email))
            return null;

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            PerfilAcesso = PerfisAcesso.Professor,
            Ativo = true,
            DataCriacao = DateTime.UtcNow,
            DataExclusao = null,
            DataUltimoProgresso = dto.DataUltimoProgresso
        };

        await _usuarioRepository.AdicionarAsync(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();

        var perfil = new UsuarioPerfil
        {
            UsuarioId = usuario.UsuarioId,
            Apelido = dto.Apelido,
            FotoPerfil = dto.FotoPerfil
        };

        await _usuarioRepository.AdicionarPerfilAsync(perfil);

        var vinculo = new VinculoInstitucional
        {
            UsuarioId = usuario.UsuarioId,
            InstituicaoId = dto.InstituicaoId,
            TipoVinculoId = dto.TipoVinculoId,
            NivelId = dto.NivelId,
            DataIngressoInstituicao = dto.DataIngressoInstituicao,
            DataUltimaProgressao = dto.DataUltimoProgresso,
            Ativo = true
        };

        await _usuarioRepository.AdicionarVinculoAsync(vinculo);
        await _usuarioRepository.SalvarAlteracoesAsync();

        return new CriarUsuarioResponseDto
        {
            UsuarioId = usuario.UsuarioId,
            PerfilAcesso = usuario.PerfilAcesso
        };
    }

    public async Task<CriarUsuarioResponseDto?> CriarProfessorCadastroAsync(CadastroViewModel dto)
    {
        if (await _usuarioRepository.EmailExisteAsync(dto.Email))
            return null;

        var usuario = new Usuario
        {
            Nome = dto.Nome.Trim(),
            Email = dto.Email.Trim(),
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            PerfilAcesso = PerfisAcesso.Professor,
            Ativo = true,
            DataCriacao = DateTime.UtcNow,
            DataExclusao = null,
            DataUltimoProgresso = dto.DataUltimoProgresso
        };

        await _usuarioRepository.AdicionarAsync(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();

        var deveCriarPerfil = !string.IsNullOrWhiteSpace(dto.Apelido);

        if (deveCriarPerfil)
        {
            await _usuarioRepository.AdicionarPerfilAsync(new UsuarioPerfil
            {
                UsuarioId = usuario.UsuarioId,
                Apelido = dto.Apelido
            });
        }

        if (dto.ModoCadastro == CadastroModos.Completo)
        {
            await _usuarioRepository.AdicionarVinculoAsync(new VinculoInstitucional
            {
                UsuarioId = usuario.UsuarioId,
                InstituicaoId = dto.InstituicaoId!.Value,
                TipoVinculoId = dto.TipoVinculoId!.Value,
                NivelId = dto.NivelId!.Value,
                DataIngressoInstituicao = dto.DataIngressoInstituicao!.Value,
                DataUltimaProgressao = dto.DataUltimoProgresso,
                Ativo = true
            });
        }

        await _usuarioRepository.SalvarAlteracoesAsync();

        return new CriarUsuarioResponseDto
        {
            UsuarioId = usuario.UsuarioId,
            PerfilAcesso = usuario.PerfilAcesso
        };
    }

    public async Task<bool> AlterarSenhaAsync(int usuarioId, ChangePasswordViewModel dto)
    {
        var usuario = await _usuarioRepository.ObterAtivoPorIdComPerfilAsync(usuarioId);

        if (usuario == null)
            return false;

        if (!BCrypt.Net.BCrypt.Verify(dto.SenhaAtual, usuario.SenhaHash))
            return false;

        usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.NovaSenha);
        await _usuarioRepository.SalvarAlteracoesAsync();

        return true;
    }

    public async Task<bool> AnonimizarUsuarioAsync(int usuarioId)
    {
        var usuario = await _usuarioRepository.ObterAtivoPorIdComPerfilAsync(usuarioId);

        if (usuario == null)
            return false;

        usuario.Ativo = false;
        usuario.DataExclusao = DateTime.UtcNow;
        usuario.Nome = $"Usuario excluido {usuario.UsuarioId}";
        usuario.Email = $"usuario-excluido-{usuario.UsuarioId}-{Guid.NewGuid():N}@anon.local";
        usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString("N"));
        usuario.PerfilAcesso = PerfisAcesso.Professor;

        if (usuario.UsuarioPerfil != null)
        {
            usuario.UsuarioPerfil.Apelido = null;
            usuario.UsuarioPerfil.FotoPerfil = null;
            usuario.UsuarioPerfil.FotoPerfilArquivo = null;
            usuario.UsuarioPerfil.FotoPerfilContentType = null;
            usuario.UsuarioPerfil.FotoPerfilAtualizadaEm = null;
        }

        await _usuarioRepository.SalvarAlteracoesAsync();

        return true;
    }

    public async Task<ProfessorPerfilDto?> ObterPerfilProfessorAsync(int usuarioId)
    {
        return await _usuarioRepository.ObterResumoPerfilProfessorAsync(usuarioId);
    }

    public async Task<FotoPerfilDto?> ObterFotoPerfilAsync(int usuarioId)
    {
        var usuario = await _usuarioRepository.ObterAtivoPorIdComPerfilAsync(usuarioId);
        var perfil = usuario?.UsuarioPerfil;

        if (usuario?.PerfilAcesso != PerfisAcesso.Professor
            || perfil?.FotoPerfilArquivo == null
            || perfil.FotoPerfilArquivo.Length == 0
            || string.IsNullOrWhiteSpace(perfil.FotoPerfilContentType))
        {
            return null;
        }

        return new FotoPerfilDto
        {
            Arquivo = perfil.FotoPerfilArquivo,
            ContentType = perfil.FotoPerfilContentType
        };
    }

    public async Task<AtualizarPerfilResultadoDto> AtualizarPerfilProfessorAsync(
        int usuarioId,
        ProfessorPerfilEdicaoViewModel model)
    {
        var usuario = await _usuarioRepository.ObterAtivoPorIdComPerfilAsync(usuarioId);

        if (usuario == null || usuario.PerfilAcesso != PerfisAcesso.Professor)
            return AtualizarPerfilResultadoDto.Falha("Professor nao encontrado.");

        if (await _usuarioRepository.EmailExisteParaOutroUsuarioAsync(model.Email.Trim(), usuarioId))
            return AtualizarPerfilResultadoDto.Falha("Este e-mail ja esta em uso por outro usuario.");

        if (PossuiVinculoParcial(model))
            return AtualizarPerfilResultadoDto.Falha("Para atualizar o vinculo funcional, preencha instituicao, tipo de vinculo, nivel atual e data de ingresso.");

        byte[]? fotoBytes = null;
        string? fotoContentType = null;

        if (model.Foto != null && model.Foto.Length > 0)
        {
            if (model.Foto.Length > TamanhoMaximoFotoBytes)
                return AtualizarPerfilResultadoDto.Falha("A foto deve ter no maximo 2 MB.");

            await using var stream = model.Foto.OpenReadStream();
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            fotoBytes = memory.ToArray();
            fotoContentType = DetectarContentTypeImagem(fotoBytes);

            if (fotoContentType == null)
                return AtualizarPerfilResultadoDto.Falha("Envie uma imagem PNG ou JPEG valida.");
        }

        var perfil = usuario.UsuarioPerfil;

        if (perfil == null)
        {
            perfil = new UsuarioPerfil { UsuarioId = usuario.UsuarioId };
            await _usuarioRepository.AdicionarPerfilAsync(perfil);
            usuario.UsuarioPerfil = perfil;
        }

        usuario.Nome = model.NomeCompleto.Trim();
        usuario.Email = model.Email.Trim();
        usuario.DataUltimoProgresso = model.DataUltimoProgresso;
        perfil.Apelido = NormalizarNomeSocial(model.NomeSocial);

        AtualizarVinculoProfessor(usuario, model);

        if (fotoBytes != null)
        {
            perfil.FotoPerfil = null;
            perfil.FotoPerfilArquivo = fotoBytes;
            perfil.FotoPerfilContentType = fotoContentType;
            perfil.FotoPerfilAtualizadaEm = DateTime.UtcNow;
        }
        else if (model.RemoverFoto)
        {
            perfil.FotoPerfil = null;
            perfil.FotoPerfilArquivo = null;
            perfil.FotoPerfilContentType = null;
            perfil.FotoPerfilAtualizadaEm = null;
        }

        await _usuarioRepository.SalvarAlteracoesAsync();
        return AtualizarPerfilResultadoDto.Atualizado();
    }

    private static void AtualizarVinculoProfessor(Usuario usuario, ProfessorPerfilEdicaoViewModel model)
    {
        var possuiDadosDeVinculo = model.InstituicaoId.HasValue
            || model.TipoVinculoId.HasValue
            || model.NivelId.HasValue
            || model.DataIngressoInstituicao.HasValue;

        if (!possuiDadosDeVinculo)
            return;

        var vinculo = usuario.VinculosInstitucionais.FirstOrDefault(v => v.Ativo);

        if (vinculo == null)
        {
            vinculo = new VinculoInstitucional
            {
                UsuarioId = usuario.UsuarioId,
                Ativo = true
            };
            usuario.VinculosInstitucionais.Add(vinculo);
        }

        vinculo.InstituicaoId = model.InstituicaoId.GetValueOrDefault();
        vinculo.TipoVinculoId = model.TipoVinculoId.GetValueOrDefault();
        vinculo.NivelId = model.NivelId.GetValueOrDefault();
        vinculo.DataIngressoInstituicao = model.DataIngressoInstituicao.GetValueOrDefault();
        vinculo.DataUltimaProgressao = model.DataUltimoProgresso;
    }

    private static bool PossuiVinculoParcial(ProfessorPerfilEdicaoViewModel model)
    {
        var preenchidos = new[]
        {
            model.InstituicaoId.HasValue,
            model.TipoVinculoId.HasValue,
            model.NivelId.HasValue,
            model.DataIngressoInstituicao.HasValue
        };

        return preenchidos.Any(valor => valor) && preenchidos.Any(valor => !valor);
    }

    private static string? NormalizarNomeSocial(string? nomeSocial)
    {
        return string.IsNullOrWhiteSpace(nomeSocial) ? null : nomeSocial.Trim();
    }

    private static string? DetectarContentTypeImagem(byte[] arquivo)
    {
        if (arquivo.Length >= 8
            && arquivo[0] == 0x89
            && arquivo[1] == 0x50
            && arquivo[2] == 0x4E
            && arquivo[3] == 0x47
            && arquivo[4] == 0x0D
            && arquivo[5] == 0x0A
            && arquivo[6] == 0x1A
            && arquivo[7] == 0x0A)
        {
            return "image/png";
        }

        if (arquivo.Length >= 3
            && arquivo[0] == 0xFF
            && arquivo[1] == 0xD8
            && arquivo[2] == 0xFF)
        {
            return "image/jpeg";
        }

        return null;
    }
}
