using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using MornGlobal;
using UnityEngine;

[assembly: InternalsVisibleTo("MornLocalize.Editor")]
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
            I.Settings.Open();
        }

        public async static UniTask UpdateMasterDataAsync(CancellationToken ct = default)
        {
            await I.Settings.UpdateAsync();
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
            I.SetDirtyInternal(obj);
        }
    }
}