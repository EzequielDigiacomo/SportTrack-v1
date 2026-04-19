using SportTrack_v1.Entidades.Entidades;

namespace SportTrack_v1.Controladores.Auth
{
    public interface ITokenService
    {
        string CreateToken(Usuario usuario);
    }
}
