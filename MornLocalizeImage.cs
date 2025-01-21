using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace MornLocalize
{
    [RequireComponent(typeof(Image))]
    public sealed class MornLocalizeImage : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private List<MornLocalizeSpriteSet> _images = new();
        [Inject] private MornLocalizeCore _core;

        private void Reset()
        {
            _image = GetComponent<Image>();
        }

        private void Awake()
        {
            _core.OnLanguageChanged.Subscribe(UpdateImage).AddTo(this);
            UpdateImage(_core.CurrentLanguage);
        }

        private void UpdateImage(string languageKey)
        {
            var setImage = _images.Find(x => x.LanguageKey == languageKey);
            if (setImage.Sprite != null)
            {
                _image.sprite = setImage.Sprite;
            }
        }
    }
}