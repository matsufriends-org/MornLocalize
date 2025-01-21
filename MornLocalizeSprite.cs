using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace MornLocalize
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class MornLocalizeSprite : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private List<MornLocalizeSpriteSet> _images = new();
        [Inject] private MornLocalizeCore _core;

        private void Reset()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void Awake()
        {
            _core.OnLanguageChanged.Subscribe(UpdateSprite).AddTo(this);
            UpdateSprite(_core.CurrentLanguage);
        }

        private void UpdateSprite(string languageKey)
        {
            var setImage = _images.Find(x => x.LanguageKey == languageKey);
            if (setImage.Sprite != null)
            {
                _renderer.sprite = setImage.Sprite;
            }
        }
    }
}