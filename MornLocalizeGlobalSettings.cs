using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MornLocalize
{
    [CreateAssetMenu(fileName = nameof(MornLocalizeGlobalSettings), menuName = "Morn/" + nameof(MornLocalizeGlobalSettings))]
    internal sealed class MornLocalizeGlobalSettings : ScriptableObject
    {
        [SerializeField] private MornLocalizeMasterData _masterData;
        internal static MornLocalizeGlobalSettings Instance { get; private set; }
        public MornLocalizeMasterData MasterData => _masterData;

        private void OnEnable()
        {
            Instance = this;
            MornLocalizeLogger.Log("Global Settings Loaded");
#if UNITY_EDITOR
            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            if (preloadedAssets.Contains(this) && preloadedAssets.Count(x => x is MornLocalizeGlobalSettings) == 1)
            {
                return;
            }

            preloadedAssets.RemoveAll(x => x is MornLocalizeGlobalSettings);
            preloadedAssets.Add(this);
            PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
            MornLocalizeLogger.Log("Global Settings Added to Preloaded Assets!");
#endif
        }

        private void OnDisable()
        {
            Instance = null;
            MornLocalizeLogger.Log("Global Settings Unloaded");
        }
    }
}