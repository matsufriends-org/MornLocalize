using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MornEditor;
using MornSpreadSheet;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MornLocalize
{
    [CreateAssetMenu(fileName = nameof(MornLocalizeSettings), menuName = "Morn/" + nameof(MornLocalizeSettings))]
    public sealed class MornLocalizeSettings : ScriptableObject
    {
        [SerializeField] private string _defaultLanguage;
        [SerializeField] private List<MornSpreadSheetMaster> _masterList;
        [SerializeField] private LanguageToDataDictionary _dictionary;
        internal bool IsLoading;

        internal void Open()
        {
            foreach (var master in _masterList)
            {
                master.Open();
            }
        }

        public async UniTask UpdateAsync(bool isUpdateSheet, bool isUpdateKey, CancellationToken ct = default)
        {
            if (IsLoading)
            {
                MornLocalizeGlobal.LogWarning("すでにタスクを実行中です");
                return;
            }

            if (_masterList.Count == 0)
            {
                MornLocalizeGlobal.LogError("マスターデータがありません。");
                return;
            }

            IsLoading = true;
            MornLocalizeGlobal.Log("<size=30>タスク開始</size>");
            if (isUpdateSheet)
            {
                foreach (var master in _masterList)
                {
                    await master.UpdateSheetNamesAsync(ct);
                    await master.DownloadSheetAsync(ct);
                }
            }

            if (isUpdateKey)
            {
                _dictionary.Clear();
                foreach (var master in _masterList)
                {
                    foreach (var sheet in master.Sheets)
                    {
                        var headerRow = sheet.GetRow(1);
                        var colCount = headerRow.GetCells().Count(x => !string.IsNullOrEmpty(x.AsString()));
                        foreach (var row in sheet.GetRows().Skip(1))
                        {
                            var key = row.GetCell(1).AsString();
                            if (string.IsNullOrWhiteSpace(key))
                            {
                                MornLocalizeGlobal.LogWarning($"{sheet.SheetName} シートに空のキーが存在するためスキップします。\n該当行:{row.AsString()}");
                                continue;
                            }
                            
                            for (var colIdx = 1; colIdx <= colCount; colIdx++)
                            {
                                var language = headerRow.GetCell(colIdx).AsString();
                                if (!_dictionary.ContainsKey(language))
                                {
                                    _dictionary.Add(language, new KeyToContentDictionary());
                                }

                                var keyToContentDict = _dictionary[language];
                                if (!keyToContentDict.ContainsKey(key))
                                {
                                    if (row.TryGetCell(colIdx, out var cell))
                                    {
                                        keyToContentDict.Add(key, cell.AsString());
                                    }
                                    else
                                    {
                                        keyToContentDict.Add(key, "Not Found.");
                                    }
                                }
                                else
                                {
                                    MornLocalizeGlobal.LogWarning($"Keyが重複しています。Language: {language} Key: {key} ");
                                }
                            }
                        }
                    }
                }
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
            IsLoading = false;
            MornLocalizeGlobal.Log("<size=30>タスク終了</size>");
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
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MornLocalizeSettings))]
    internal sealed class MornLocalizeSettingsEditor : Editor
    {
        private bool _isLoading;
        private Vector2 _scrollPosition;

        public override void OnInspectorGUI()
        {
            var settings = (MornLocalizeSettings)target;
            MornEditorUtil.Draw(
                new MornEditorUtil.MornEditorOption
                {
                    IsEnabled = true,
                    IsBox = true,
                    IsIndent = true,
                    Color = settings.IsLoading ? Color.red : null,
                    Header = "ステータス: " + (settings.IsLoading ? "タスク実行中" : "待機"),
                },
                () =>
                {
                    if (GUILayout.Button("URLを開く"))
                    {
                        settings.Open();
                    }

                    MornEditorUtil.Draw(
                        new MornEditorUtil.MornEditorOption
                        {
                            IsEnabled = !settings.IsLoading,
                        },
                        () =>
                        {
                            if (GUILayout.Button("シート更新 & Keyを登録する"))
                            {
                                settings.UpdateAsync(true, true).Forget();
                            }

                            if (GUILayout.Button("シート更新"))
                            {
                                settings.UpdateAsync(true, false).Forget();
                            }

                            if (GUILayout.Button("Keyを登録する"))
                            {
                                settings.UpdateAsync(false, true).Forget();
                            }
                        });
                    MornEditorUtil.Draw(
                        new MornEditorUtil.MornEditorOption
                        {
                            IsEnabled = settings.IsLoading,
                        },
                        () =>
                        {
                            if (GUILayout.Button("強制解除"))
                            {
                                settings.IsLoading = false;
                            }
                        });
                });
            base.OnInspectorGUI();
        }
    }
#endif
}