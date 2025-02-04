using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MornSpreadSheet;
using UnityEngine;
using UnityEngine.Networking;

namespace MornLocalize
{
    [CreateAssetMenu(
        fileName = nameof(MornLocalizeMasterData),
        menuName = "Morn/Localize/" + nameof(MornLocalizeMasterData))]
    public sealed class MornLocalizeMasterData : ScriptableObject
    {
        [SerializeField] private string _sheetId;
        [SerializeField] private string _getSheetsApiUrl;
        [SerializeField] private List<string> _sheetNames;
        [SerializeField] private string _defaultLanguage;
        [SerializeField] private List<MornLocalizeLanguageDictionary> _sheets;
        internal bool IsValid => _sheets != null && _sheets.Count > 0;

        [System.Serializable]
        private class SheetNameList
        {
            public string[] Names;
        }

        internal void Open()
        {
            var core = new MornSpreadSheetLoader(_sheetId);
            foreach (var sheetName in _sheetNames)
            {
                core.Open(sheetName);
            }
        }

        internal async UniTask LoadAsync()
        {
            var core = new MornSpreadSheetLoader(_sheetId);
            var sheets = new List<MornSpreadSheet.MornSpreadSheet>();

            // Apiからシート一覧を取得し更新
            if (!string.IsNullOrEmpty(_getSheetsApiUrl))
            {
                using var request = UnityWebRequest.Get(_getSheetsApiUrl);
                await request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var json = request.downloadHandler.text;
                    var sheetNames = JsonUtility.FromJson<SheetNameList>("{\"Names\":" + json + "}").Names;
                    _sheetNames.Clear();
                    _sheetNames.AddRange(sheetNames);
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(this);
#endif
                }
                else
                {
                    MornLocalizeGlobal.I.LogError("エラー: " + request.error);
                }
            }

            foreach (var sheetName in _sheetNames)
            {
                MornLocalizeGlobal.I.Log("Loading sheet: " + sheetName);
                var sheet = await core.LoadSheetAsync(sheetName);
                sheets.Add(sheet);
            }

            if (sheets.Count == 0)
            {
                MornLocalizeGlobal.I.LogError("Failed to load sheet");
                return;
            }

            _sheets.Clear();
            foreach (var sheet in sheets)
            {
                var sheetData = new MornLocalizeLanguageDictionary();
                for (var x = 2; x <= sheet.ColCount; x++)
                {
                    var languageName = sheet.Get(1, x).AsString();
                    if (string.IsNullOrEmpty(languageName))
                    {
                        continue;
                    }

                    if (sheetData.ContainsKey(languageName))
                    {
                        MornLocalizeGlobal.I.LogWarning($"Duplicated language: {languageName}");
                        continue;
                    }

                    var localizeKeyToString = new MornLocalizeKeyDictionary();
                    for (var y = 2; y <= sheet.RowCount; y++)
                    {
                        var key = sheet.Get(y, 1).AsString();
                        var value = sheet.Get(y, x).AsString();
                        if (string.IsNullOrEmpty(key) || key == "key")
                        {
                            continue;
                        }

                        localizeKeyToString.Add(key, value);
                    }

                    sheetData.Add(languageName, localizeKeyToString);
                }
                _sheets.Add(sheetData);
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public string[] GetLanguages()
        {
            var hashSet = new HashSet<string>();
            foreach (var sheet in _sheets)
            {
                foreach (var language in sheet.Keys)
                {
                    hashSet.Add(language);
                }
            }
            
            return hashSet.ToArray(); 
        }

        public int GetLanguageCount()
        {
            return GetLanguages().Length;
        }

        public string GetLanguage(int languageIndex)
        {
            return GetLanguages()[languageIndex];
        }

        internal string[] GetKeys()
        {
            var hashSet = new HashSet<string>();
            foreach (var sheet in _sheets)
            {
                var languageData = sheet.Values.FirstOrDefault();
                foreach (var key in languageData.Keys)
                {
                    if (!hashSet.Add(key))
                    {
                        MornLocalizeGlobal.I.LogError($"Duplicated key: {key}");
                    }
                }
            }
            
            return hashSet.ToArray();
        }

        public string Get(string key)
        {
            return Get(_defaultLanguage, key);
        }

        public string Get(string language, string key)
        {
            foreach (var sheet in _sheets)
            {
                if (!sheet.ContainsKey(language))
                {
                    continue;
                }

                if (!sheet[language].ContainsKey(key))
                {
                    continue;
                }
                
                return sheet[language][key];
            }
            
            MornLocalizeGlobal.I.LogError($"Not found: [{language}] [{key}]");
            return "Not Found";
        }

        public bool TryGet(string key, out string value)
        {
            return TryGet(_defaultLanguage, key, out value);
        }

        public bool TryGet(string language, string key, out string value)
        {
            foreach (var sheet in _sheets)
            {
                if (!sheet.ContainsKey(language))
                {
                    continue;
                }

                if (!sheet[language].ContainsKey(key))
                {
                    continue;
                }
                
                value = sheet[language][key];
                return true;
            }
            
            value = "Not Found";
            return false;
        }
    }
}