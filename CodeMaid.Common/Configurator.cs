using System;
using System.Threading.Tasks;
using PCLStorage;
using Newtonsoft.Json;

namespace CodeMaid.Common
{
    public class Configurator
    {
        private const string ConfigFileName = "codecleaner.config";
        private static Configurator _instance;

        public bool IsOrderingEnabled { get; set; } = true;

        public bool IsPrivatingEnabled { get; set; } = true;

        private Configurator()
        {
        }

        public async static Task<Configurator> LoadConfiguration()
        {
            _instance = _instance ?? await LoadPersistedConfiguration();

            return _instance;
        }

        public async static Task SaveConfiguration(Configurator config)
        {
            _instance = config;

            var file = await FileSystem.Current.LocalStorage.CreateFileAsync(ConfigFileName, CreationCollisionOption.ReplaceExisting);
            await file.WriteAllTextAsync(JsonConvert.SerializeObject(config));
        }

        private async static Task<Configurator> LoadPersistedConfiguration()
        {
            Configurator result = new Configurator();

            if (await PCLStorage.FileSystem.Current.LocalStorage.CheckExistsAsync(ConfigFileName) == PCLStorage.ExistenceCheckResult.FileExists)
            {
                var file = await FileSystem.Current.LocalStorage.GetFileAsync(ConfigFileName);

                var rawConfig = await file.ReadAllTextAsync();
                result = JsonConvert.DeserializeObject<Configurator>(rawConfig);
            }

            return result;
        }
    }
}
