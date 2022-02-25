using Grafana_Prometheus.Models;

namespace Grafana_Prometheus.Interfaces
{
    public interface IUsuarioService
    {
        Task AddAsync(User user);
        Task RemoveAsync(string name);
        Task<List<User>> GetAsync(string? name);
    }
}
