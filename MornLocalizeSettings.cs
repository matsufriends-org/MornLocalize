using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MornSpreadSheet;
using UnityEngine;

namespace MornLocalize
{
    [CreateAssetMenu(fileName = nameof(MornLocalizeSettings), menuName = "Morn/" + nameof(MornLocalizeSettings))]
    public sealed class MornLocalizeSettings : ScriptableObject
    {
        [SerializeField] private string _defaultLanguage;
        [SerializeField] private List<MornSpreadSheetMaster> _masterList;
        [SerializeField] private LanguageToDataDictionary _dictionary;

        internal void Open()
        {
            foreach (var master in _masterList)
            {
                master.Open();
            }
        }

        /// <summary>マスターリストを取得</summary>
        internal IReadOnlyList<MornSpreadSheetMaster> GetMasterList() => _masterList;

        /// <summary>辞書をクリア</summary>
        internal void ClearDictionary() => _dictionary.Clear();

        /// <summary>ローカライズデータを追加</summary>
        internal void AddLocalizeData(string language, string key, string content)
        {
            if (!_dictionary.ContainsKey(language))
            {
                _dictionary.Add(language, new KeyToContentDictionary());
            }

            var keyToContentDict = _dictionary[language];
            if (!keyToContentDict.ContainsKey(key))
            {
                keyToContentDict.Add(key, content);
            }
            else
            {
                MornLocalizeGlobal.LogWarning($"Keyが重複しています。Language: {language} Key: {key} ");
            }
        }

        public string Get(string language, string key)
        {
            TryGet(language, key, out var value);
            return value;
        }

        public bool TryGet(string language, string key, out string value)
        {
            if (_dictionary.ContainsKey(language))
            {
                if (_dictionary[language].ContainsKey(key))
                {
                    value = _dictionary[language][key];
                    return true;
                }
            }

            value = "Not Found.";
            return false;
        }

        public async UniTask UpdateAsync()
        {
#if UNITY_EDITOR
            await MornLocalizeDownloader.UpdateWithProgressAsync(this, true, true);
#else
            await UniTask.CompletedTask;
#endif
        }
    }
}