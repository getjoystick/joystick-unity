using UnityEngine;

namespace JoystickRemote.Core
{
    public class CacheDataManager
    {
        private const string CacheDataKeyPrefix = "CacheData_";
        private const string CacheDataExpirationKeyPrefix = "CacheDataExpiration_";

        public static void SetCache(string key, string value, float expirationTime = float.PositiveInfinity)
        {
            PlayerPrefs.SetString(CacheDataKeyPrefix + key, value);
            PlayerPrefs.SetFloat(CacheDataExpirationKeyPrefix + key, Time.time + expirationTime);
            PlayerPrefs.Save();
        }

        public static string GetCache(string key)
        {
            if (PlayerPrefs.HasKey(CacheDataKeyPrefix + key))
            {
                var cacheDataJson = PlayerPrefs.GetString(CacheDataKeyPrefix + key, string.Empty);
                var expirationTime = PlayerPrefs.GetFloat(CacheDataExpirationKeyPrefix + key, 0);
                
                if (expirationTime > Time.time)
                {
                    return cacheDataJson;
                }

                PlayerPrefs.DeleteKey(CacheDataKeyPrefix + key);
                PlayerPrefs.DeleteKey(CacheDataExpirationKeyPrefix + key);
                PlayerPrefs.Save();
            }

            return string.Empty;
        }

        public static void ClearCache(string key = null)
        {
            if (key == null)
            {
                // Clear all cached data
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }
            else
            {
                if (PlayerPrefs.HasKey(key))
                {
                    PlayerPrefs.DeleteKey(CacheDataKeyPrefix + key);
                    PlayerPrefs.DeleteKey(CacheDataExpirationKeyPrefix + key);
                    PlayerPrefs.Save();
                }
            }
        }
    }
}
