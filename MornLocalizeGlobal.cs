using System.Collections.Generic;
using MornGlobal;
using UnityEngine;

namespace MornLocalize
{
    [CreateAssetMenu(fileName = nameof(MornLocalizeGlobal), menuName = "Morn/" + nameof(MornLocalizeGlobal))]
    public sealed class MornLocalizeGlobal : MornGlobalBase<MornLocalizeGlobal>
    {
#if DISABLE_MORN_LOCALIZE_LOG
        protected override bool ShowLog => false;
#else
        protected override bool ShowLog => true;
#endif
        protected override string ModuleName => nameof(MornLocalize);
        [SerializeField] private MornLocalizeMasterData _masterData;
        [SerializeField] private List<MornLocalizeFont> _otherFonts;
        [SerializeField] private string _debugLanguageKey = "jp";
        public MornLocalizeMasterData MasterData => _masterData;
        public string DebugLanguageKey => _debugLanguageKey;

        internal void SetDirty(Object obj)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(obj);
#endif
        }
    }
}