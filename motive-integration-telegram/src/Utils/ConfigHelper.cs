using DotNetEnv;

namespace motive_integration_telegram.src.Utils
{
    public static class ConfigHelper
    {
        private static bool _isLoaded = false;

        private static void EnsureLoaded()
        {
            if (!_isLoaded)
            {
                try
                {
                    Env.Load();
                    _isLoaded = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not load .env file: {ex.Message}");
                }
            }
        }

        public static string GetEnvVar(string key, string? defaultValue = null)
        {
            EnsureLoaded();

            var value = Environment.GetEnvironmentVariable(key);

            if (string.IsNullOrWhiteSpace(value))
            {
                if (defaultValue != null)
                    return defaultValue;

                throw new NotImplementedException($"Environment variable '{key}' is missing.");
            }

            return value;
        }
    }
}
