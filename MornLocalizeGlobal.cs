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
        [SerializeField] private List<MornLocalizeFontSet> _defaultFontSets;
        [SerializeField] private List<MornLocalizeFont> _otherFonts;
        [SerializeField] private string _debugLanguageKey = "jp";
        public MornLocalizeMasterData MasterData => _masterData;
        public string DebugLanguageKey => _debugLanguageKey;

        public MornLocalizeFont GetDefaultFont(string languageKey)
        {
            foreach (var fontSet in _defaultFontSets)
            {
                if (fontSet.LanguageKey == languageKey)
                {
                    return fontSet.Font;
                }
            }

            return _defaultFontSets[0].Font;
        }

        public string GetFontMaterialKey(Material material)
        {
            var fontList = new List<MornLocalizeFont>();
            fontList.AddRange(_defaultFontSets.ConvertAll(x => x.Font));
            fontList.AddRange(_otherFonts);
            foreach (var font in fontList)
            {
                foreach (var materialSet in font.Materials)
                {
                    if (materialSet.Material == material)
                    {
                        return materialSet.MaterialType;
                    }
                }
            }

            return null;
        }
    }
}