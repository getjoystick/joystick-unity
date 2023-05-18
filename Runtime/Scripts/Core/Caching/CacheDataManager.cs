using UnityEngine;

namespace JoystickRemoteConfig.Core
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

        public static bool HasCache(string key)
        {
            if (PlayerPrefs.HasKey(CacheDataKeyPrefix + key) && PlayerPrefs.HasKey(CacheDataExpirationKeyPrefix + key))
            {
                return !string.IsNullOrWhiteSpace(PlayerPrefs.GetString(CacheDataKeyPrefix + key)) &&
                       PlayerPrefs.GetFloat(CacheDataExpirationKeyPrefix + key) > Time.time;
            }

            return false;
        }

        public static void ClearCache(string key)
        {
            if (PlayerPrefs.HasKey(CacheDataKeyPrefix + key))
            {
                PlayerPrefs.DeleteKey(CacheDataKeyPrefix + key);
                PlayerPrefs.DeleteKey(CacheDataExpirationKeyPrefix + key);
                PlayerPrefs.Save();
            }
        }
    }
}
