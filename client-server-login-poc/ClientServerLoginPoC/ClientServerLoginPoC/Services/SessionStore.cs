using System.Collections.Concurrent;

namespace ClientServerLoginPoC.Services
{
    public class SessionStore
    {
        private readonly ConcurrentDictionary<string, string> _sessions = new();

        public string CreateSession(string email)
        {
            var token = Guid.NewGuid().ToString("N");
            _sessions[token] = email;
            return token;
        }

        public bool TryGetEmail(string token, out string email)
            => _sessions.TryGetValue(token, out email!);
    }

}
