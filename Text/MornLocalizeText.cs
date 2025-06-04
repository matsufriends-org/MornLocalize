using TMPro;
using UniRx;
using UnityEngine;
using VContainer;

namespace MornLocalize
{
    [ExecuteAlways]
    public sealed class MornLocalizeText : MonoBehaviour
    {
        [SerializeField, ReadOnly] private TMP_Text _label;
        [SerializeField] private MornLocalizeString _text;
        [Inject] private MornLocalizeCore _core;
        
        public MornLocalizeString Text
        {
            get => _text;
            set => _text = value;
        }

        private void OnEnable()
        {
            if (Application.isPlaying && _core != null)
            {
                _core.OnLanguageChanged.Subscribe(Adjust).AddTo(this);
                Adjust(_core.CurrentLanguage);
            }
        }

        private void Reset()
        {
            _label = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                var global = MornLocalizeGlobal.I;
                if (global == null)
                {
                    return;
                }

                Adjust(global.DebugLanguageKey);
            }
        }

        public void Adjust(string languageKey)
        {
            var global = MornLocalizeGlobal.I;
            if (global == null || _label == null)
            {
                return;
            }
            
            var text = _text.Get(languageKey);
            if (_label.text != text)
            {
                _label.text = text;
                MornLocalizeGlobal.SetDirty(_label);
            }
        }
    }
}