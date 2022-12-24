using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Sessions
{
    class SessionManager
    {
        private static SessionManager? _instance;
        private static readonly object Locker = new();

        public static SessionManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (Locker)
                {
                    _instance ??= new SessionManager();
                }

                return _instance;
            }
        }

        private readonly MemoryCache _cache;

        private SessionManager()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public Session CreateSession(int accountId, string email)
        {
            var session = new Session(Guid.NewGuid(), accountId, email, DateTime.Now);
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            _cache.Set(session.Guid, session, cacheEntryOptions);
            return session;
        }


        public bool SessionExists(Guid guid) => _cache.Get(guid) != null;
        public Session? GetSession(Guid guid) => (Session?)_cache.Get(guid);

        public Session? GetSession(string? guid) => 
            guid == null ? null : (Session?)_cache.Get(Guid.Parse(guid));

        public bool TryGetSession(Guid guid, out Session? session)
        {
            return _cache.TryGetValue(guid, out session);
        }
    }
}