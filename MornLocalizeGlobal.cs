using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MornGlobal;
using UnityEngine;

namespace MornLocalize
{
    [CreateAssetMenu(fileName = nameof(MornLocalizeGlobal), menuName = "Morn/" + nameof(MornLocalizeGlobal))]
    public sealed class MornLocalizeGlobal : MornGlobalBase<MornLocalizeGlobal>
    {
        protected override string ModuleName => nameof(MornLocalize);
        [SerializeField] private MornLocalizeSettings _settings;
        [SerializeField] private List<MornLocalizeFont> _otherFonts;
        [SerializeField] private string _debugLanguageKey = "jp";
        public MornLocalizeSettings Settings => _settings;
        internal string DebugLanguageKey => _debugLanguageKey;

        public static void OpenMasterData()
        {
            var masterData = I.Settings;
            masterData.Open();
        }

        public async static UniTask LoadMasterDataAsync(CancellationToken ct =default)
        {
            var masterData = I.Settings;
            await masterData.UpdateAsync(true, true, ct);
        }
        
        internal static void Log(string message)
        {
            I.LogInternal(message);
        }
        
        internal static void LogWarning(string message)
        {
            I.LogWarningInternal(message);
        }
        
        internal static void LogError(string message)
        {
            I.LogErrorInternal(message);
        }
        
        internal static void SetDirty(Object obj)
        {
            I.SetDirtyInternal();
        }
        
    }
}