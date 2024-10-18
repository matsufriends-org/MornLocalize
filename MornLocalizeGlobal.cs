using MornGlobal;
using UnityEngine;

namespace MornLocalize
{
    [CreateAssetMenu(fileName = nameof(MornLocalizeGlobal), menuName = "Morn/" + nameof(MornLocalizeGlobal))]
    internal sealed class MornLocalizeGlobal : MornGlobalBase<MornLocalizeGlobal>
    {
#if DISABLE_MORN_LOCALIZE_LOG
        protected override bool ShowLog => false;
#else
        protected override bool ShowLog => true;
#endif
        protected override string ModuleName => nameof(MornLocalize);
        [SerializeField] private MornLocalizeMasterData _masterData;
        public MornLocalizeMasterData MasterData => _masterData;
    }
}