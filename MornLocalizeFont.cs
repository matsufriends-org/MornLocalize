using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MornLocalize
{
    [CreateAssetMenu(fileName = nameof(MornLocalizeFont), menuName = "Morn/" + nameof(MornLocalizeFont))]
    public sealed class MornLocalizeFont : ScriptableObject
    {
        [SerializeField] private TMP_FontAsset _font;
        [SerializeField] private List<MornLocalizeFontMaterialSet> _materials;
        public TMP_FontAsset Font => _font;
        public List<MornLocalizeFontMaterialSet> Materials => _materials;

        public bool TryGetMaterial(string materialType, out Material material)
        {
            foreach (var materialSet in _materials)
            {
                if (materialSet.MaterialType == materialType)
                {
                    material = materialSet.Material;
                    return true;
                }
            }

            material = null;
            return false;
        }
    }
}