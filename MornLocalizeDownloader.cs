#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MornSpreadSheet;
using UnityEditor;

namespace MornLocalize
{
    /// <summary>エディタ専用のローカライズデータダウンロード機能</summary>
    public static class MornLocalizeDownloader
    {
        /// <summary>進捗表示付きで更新</summary>
        public async static UniTask UpdateWithProgressAsync(MornLocalizeSettings settings, bool isUpdateSheet,
            bool isUpdateKey)
        {
            var masterList = settings.GetMasterList();
            if (masterList.Count == 0)
            {
                MornLocalizeGlobal.LogError("マスターデータがありません。");
                return;
            }

            var cancellationTokenSource = new CancellationTokenSource();

            // 総シート数を計算
            var totalSheetCount = 0;
            if (isUpdateSheet)
            {
                foreach (var master in masterList)
                {
                    totalSheetCount += master.SheetNames.Count;
                }
            }

            try
            {
                await UniTask.SwitchToMainThread();
                MornLocalizeGlobal.Log("<size=30>タスク開始</size>");
                if (isUpdateSheet)
                {
                    var currentSheetIndex = 0;
                    foreach (var master in masterList)
                    {
                        // シート名更新はMornSpreadSheetDownloaderで別途実行する必要があります

                        // シートごとにダウンロード（進捗表示対応）
                        var sheets = new List<MornSpreadSheet.MornSpreadSheet>();
                        foreach (var sheetName in master.SheetNames)
                        {
                            var progress = (float)currentSheetIndex / totalSheetCount;
                            var cancelled = EditorUtility.DisplayCancelableProgressBar(
                                "ローカライズデータ更新",
                                $"ダウンロード中: {sheetName} ({currentSheetIndex + 1}/{totalSheetCount})",
                                progress);
                            if (cancelled)
                            {
                                cancellationTokenSource.Cancel();
                                break;
                            }

                            // タスクの実行中は定期的に進捗バーを再表示してUIをブロック
                            var downloadTask = MornSpreadSheetDownloader.LoadSheetAsync(
                                master.SheetId,
                                sheetName,
                                cancellationTokenSource.Token);
                            while (!downloadTask.Status.IsCompleted())
                            {
                                // 進捗バーを再表示してUIをブロック
                                cancelled = EditorUtility.DisplayCancelableProgressBar(
                                    "ローカライズデータ更新",
                                    $"ダウンロード中: {sheetName} ({currentSheetIndex + 1}/{totalSheetCount})",
                                    progress);
                                if (cancelled)
                                {
                                    cancellationTokenSource.Cancel();
                                    break;
                                }

                                await UniTask.Yield();
                            }

                            if (cancellationTokenSource.Token.IsCancellationRequested)
                            {
                                break;
                            }

                            var sheet = await downloadTask;
                            if (sheet != null)
                            {
                                sheets.Add(sheet);
                            }

                            currentSheetIndex++;
                        }

                        master.SetSheets(sheets);
                        EditorUtility.SetDirty(master);
                        if (cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                }

                if (isUpdateKey && !cancellationTokenSource.Token.IsCancellationRequested)
                {
                    // キーを更新
                    await UpdateKeysAsync(settings, masterList, cancellationTokenSource.Token);
                }

                // 最後の進捗を100%に
                EditorUtility.DisplayCancelableProgressBar("ローカライズデータ更新", "完了処理中...", 1f);
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                if (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    MornLocalizeGlobal.Log("<size=30>タスク完了</size>");
                }
            }
            catch (OperationCanceledException)
            {
                MornLocalizeGlobal.Log("<size=30>タスクがキャンセルされました</size>");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("エラー", $"エラーが発生しました: {ex.Message}", "OK");
                MornLocalizeGlobal.LogError($"タスク中にエラーが発生しました: {ex}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                cancellationTokenSource?.Dispose();
            }
        }

        /// <summary>キーを更新する</summary>
        private async static UniTask UpdateKeysAsync(MornLocalizeSettings settings,
            IReadOnlyList<MornSpreadSheetMaster> masterList, CancellationToken cancellationToken)
        {
            settings.ClearDictionary();
            foreach (var master in masterList)
            {
                foreach (var sheet in master.Sheets)
                {
                    var headerRow = sheet.GetRow(1);
                    var colCount = headerRow.GetCells().Count(x => !string.IsNullOrEmpty(x.AsString()));
                    foreach (var row in sheet.GetRows().Skip(1))
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        var key = row.GetCell(1).AsString();
                        if (string.IsNullOrWhiteSpace(key))
                        {
                            MornLocalizeGlobal.LogWarning(
                                $"{sheet.SheetName} シートに空のキーが存在するためスキップします。\n該当行:{row.AsString()}");
                            continue;
                        }

                        for (var colIdx = 1; colIdx <= colCount; colIdx++)
                        {
                            var language = headerRow.GetCell(colIdx).AsString();
                            if (row.TryGetCell(colIdx, out var cell))
                            {
                                var content = cell.AsString();

                                // ""を"に置き換える処理
                                if (content.Contains("\"\""))
                                {
                                    content = content.Replace("\"\"", "\"");
                                }

                                settings.AddLocalizeData(language, key, content);
                            }
                            else
                            {
                                settings.AddLocalizeData(language, key, "Not Found.");
                            }
                        }
                    }
                }
            }
        }
    }
}
#endif