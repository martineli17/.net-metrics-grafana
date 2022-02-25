namespace Grafana_Prometheus.Models
{
    public class User
    {
        public string Name { get; private set; }
        public User(string name)
        {
            Name = name;
        }
    }
}
