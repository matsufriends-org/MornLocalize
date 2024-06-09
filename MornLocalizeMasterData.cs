using System.Linq;
using Cysharp.Threading.Tasks;
using MornSpreadSheet;
using UnityEngine;

namespace MornLocalize
{
    [CreateAssetMenu(fileName = nameof(MornLocalizeMasterData), menuName = "MornLocalize/" + nameof(MornLocalizeMasterData))]
    public sealed class MornLocalizeMasterData : ScriptableObject
    {
        private static MornLocalizeMasterData instance;
        public static MornLocalizeMasterData Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                var instances = Resources.FindObjectsOfTypeAll<MornLocalizeMasterData>();
                if (instances.Length == 0)
                {
                    MornLocalizeLogger.LogError("MornLocalizeMasterData not found");
                    return null;
                }

                if (instances.Length > 1)
                {
                    MornLocalizeLogger.LogError("MornLocalizeMasterData is duplicated");
                    return null;
                }

                instance = instances[0];
                return instance;
            }
        }
        [SerializeField] private string sheetId;
        [SerializeField] private string sheetName;
        [SerializeField] private string defaultLanguage;
        [SerializeField] [SerializeReference] private MornLocalizeLanguageDictionary data;

        internal async UniTask LoadAsync()
        {
            var core = new MornSpreadSheetLoader(sheetId);
            var sheet = await core.LoadSheetAsync(sheetName);
            if (sheet == null)
            {
                MornLocalizeLogger.LogError("Failed to load sheet");
                return;
            }
            
            data = new MornLocalizeLanguageDictionary();
            for (var x = 2; x <= sheet.colCount; x++)
            {
                var languageName = sheet.Get(1, x).AsString();
                if (string.IsNullOrEmpty(languageName))
                {
                    continue;
                }

                if (data.ContainsKey(languageName))
                {
                    MornLocalizeLogger.LogWarning($"Duplicated language: {languageName}");
                    continue;
                }

                var localizeKeyToString = new MornLocalizeKeyDictionary();
                for (var y = 2; y <= sheet.rowCount; y++)
                {
                    var key = sheet.Get(y, 1).AsString();
                    var value = sheet.Get(y, x).AsString();
                    localizeKeyToString.Add(key, value);
                }

                data.Add(languageName, localizeKeyToString);
            }
        }

        internal string[] GetLanguages()
        {
            return data.Keys.ToArray();
        }

        internal string[] GetKeys()
        {
            return data.FirstOrDefault().Value.Keys.ToArray();
        }

        public string Get(string key)
        {
            return Get(defaultLanguage, key);
        }

        public string Get(string language, string key)
        {
            if (data == null)
            {
                MornLocalizeLogger.LogError("Data is not loaded");
                return null;
            }

            if (!data.ContainsKey(language))
            {
                MornLocalizeLogger.LogError($"Language not found: {language}");
                return null;
            }

            var languageData = data[language];
            if (!languageData.ContainsKey(key))
            {
                MornLocalizeLogger.LogError($"Key not found: {key}");
                return null;
            }

            return languageData[key];
        }

        public bool TryGet(string key, out string value)
        {
            return TryGet(defaultLanguage, key, out value);
        }

        public bool TryGet(string language, string key, out string value)
        {
            value = "";
            if (data == null)
            {
                return false;
            }

            if (!data.ContainsKey(language))
            {
                return false;
            }

            var languageData = data[language];
            if (!languageData.ContainsKey(key))
            {
                return false;
            }

            value = languageData[key];
            return true;
        }
    }
}