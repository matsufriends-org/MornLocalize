using UnityEngine;
using VContainer;

namespace MornLocalize
{
    [ExecuteAlways]
    public sealed class MornLocalizeTextSync : MonoBehaviour
    {
        [SerializeField] private MornLocalizeText[] _texts;
        [SerializeField] private MornLocalizeString _localize;
        [Inject] private MornLocalizeCore _core;

        private void OnValidate()
        {
            foreach (var text in _texts)
            {
                text.Text = _localize;
            }
        }

        private void Reset()
        {
            _texts = GetComponentsInChildren<MornLocalizeText>(true);
        }
    }
}