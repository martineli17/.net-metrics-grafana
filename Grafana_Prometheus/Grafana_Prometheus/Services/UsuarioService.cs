using Grafana_Prometheus.DataBase;
using Grafana_Prometheus.Interfaces;
using Grafana_Prometheus.Models;
using Prometheus;

namespace Grafana_Prometheus.Services
{
    public class UsuarioService : IUsuarioService
    {
        private static readonly Counter _counter = Metrics.CreateCounter("mymetrics_database_operations", "Operações no Banco de Dados", "operation", "schema");
        public UsuarioService()
        {
        }
        public async Task AddAsync(User user)
        {
            _counter.WithLabels("Write", "User").Inc();
            await Task.Run(() => GrafanaAPIContext.Users.Add(user));
        }

        public async Task<List<User>> GetAsync(string? name)
        {
            _counter.WithLabels("Read", "User").Inc();
            return await Task.Run(() => GrafanaAPIContext.Users.Where(x => name is null || x.Name == name).ToList());
        }

        public async Task RemoveAsync(string name)
        {
            _counter.WithLabels("Write", "User").Inc();
            await Task.Run(() => GrafanaAPIContext.Users.Remove(GrafanaAPIContext.Users.First(x => x.Name == name)));
        }
    }
}
