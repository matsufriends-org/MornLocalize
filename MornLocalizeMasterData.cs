using System.Linq;
using Cysharp.Threading.Tasks;
using MornSpreadSheet;
using UnityEngine;

namespace MornLocalize
{
    [CreateAssetMenu(fileName = nameof(MornLocalizeMasterData), menuName = "Morn/Localize/" + nameof(MornLocalizeMasterData))]
    public sealed class MornLocalizeMasterData : ScriptableObject
    {
        [SerializeField] private string _sheetId;
        [SerializeField] private string _sheetName;
        [SerializeField] private string _defaultLanguage;
        [SerializeField] private MornLocalizeLanguageDictionary _data;
        internal bool IsValid => _data != null && _data.Count > 0;

        internal void Open()
        {
            var core = new MornSpreadSheetLoader(_sheetId);
            core.Open(_sheetName);
        }

        internal async UniTask LoadAsync()
        {
            var core = new MornSpreadSheetLoader(_sheetId);
            var sheet = await core.LoadSheetAsync(_sheetName);
            if (sheet == null)
            {
                MornLocalizeGlobal.I.LogError("Failed to load sheet");
                return;
            }

            _data = new MornLocalizeLanguageDictionary();
            for (var x = 2; x <= sheet.ColCount; x++)
            {
                var languageName = sheet.Get(1, x).AsString();
                if (string.IsNullOrEmpty(languageName))
                {
                    continue;
                }

                if (_data.ContainsKey(languageName))
                {
                    MornLocalizeGlobal.I.LogWarning($"Duplicated language: {languageName}");
                    continue;
                }

                var localizeKeyToString = new MornLocalizeKeyDictionary();
                for (var y = 2; y <= sheet.RowCount; y++)
                {
                    var key = sheet.Get(y, 1).AsString();
                    var value = sheet.Get(y, x).AsString();
                    localizeKeyToString.Add(key, value);
                }

                _data.Add(languageName, localizeKeyToString);
            }
        }

        public string[] GetLanguages()
        {
            return _data.Keys.ToArray();
        }

        public int GetLanguageCount()
        {
            return _data.Count;
        }

        public string GetLanguage(int languageIndex)
        {
            return _data.Keys.ElementAt(languageIndex);
        }

        internal string[] GetKeys()
        {
            return _data.FirstOrDefault().Value.Keys.ToArray();
        }

        public string Get(string key)
        {
            return Get(_defaultLanguage, key);
        }

        public string Get(string language, string key)
        {
            if (_data == null)
            {
                MornLocalizeGlobal.I.LogError("Data is not loaded");
                return null;
            }

            if (!_data.ContainsKey(language))
            {
                MornLocalizeGlobal.I.LogError($"Language not found: {language}");
                return null;
            }

            var languageData = _data[language];
            if (!languageData.ContainsKey(key))
            {
                MornLocalizeGlobal.I.LogError($"Key not found: {key}");
                return null;
            }

            return languageData[key];
        }

        public bool TryGet(string key, out string value)
        {
            return TryGet(_defaultLanguage, key, out value);
        }

        public bool TryGet(string language, string key, out string value)
        {
            value = "";
            if (_data == null)
            {
                return false;
            }

            if (!_data.ContainsKey(language))
            {
                return false;
            }

            var languageData = _data[language];
            if (!languageData.ContainsKey(key))
            {
                return false;
            }

            value = languageData[key];
            return true;
        }
    }
}