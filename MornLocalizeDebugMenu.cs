using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MornDebug;
using UnityEngine;

namespace MornLocalize
{
    [CreateAssetMenu(fileName = nameof(MornLocalizeDebugMenu), menuName = "Morn/" + nameof(MornLocalizeDebugMenu))]
    public sealed class MornLocalizeDebugMenu : MornDebugMenuBase
    {
        public override IEnumerable<(string key, Action action)> GetMenuItems()
        {
            yield return ("Localize", () =>
            {
                if (GUILayout.Button("ローカライズシートを開く"))
                {
                    MornLocalizeGlobal.OpenMasterData();
                }

                if (GUILayout.Button("ローカライズシートを更新"))
                {
                    MornLocalizeGlobal.UpdateMasterDataAsync().Forget();
                }
            });
        }
    }
}