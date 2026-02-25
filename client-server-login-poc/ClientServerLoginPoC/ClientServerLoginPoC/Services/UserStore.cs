using System.Collections.Concurrent;

namespace ClientServerLoginPoC.Services
{

    public class UserStore
    {
        private readonly ConcurrentDictionary<string, string> _users = new();

        public UserStore()
        {
            _users["demo@demo.com"] = "Demo123!";
        }

        public bool Validate(string email, string password)
            => _users.TryGetValue(email, out var saved) && saved == password;
    }

}
