using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Resultado
{
    public interface INotificadorResultados
    {
        Task NotificarCambioResultado(int eventoPruebaId, object resultado);
    }
}
