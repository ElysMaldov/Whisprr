namespace Whisprr.Caching.Interfaces;

public interface ICaching
{
  string Prefix
  {
    get;
  }

  Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null);
  Task<T?> GetAsync<T>(string key);
}

