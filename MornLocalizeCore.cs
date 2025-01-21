using System;
using UniRx;

namespace MornLocalize
{
    public sealed class MornLocalizeCore
    {
        private readonly Subject<string> _languageChangeSubject = new();
        public IObservable<string> OnLanguageChanged => _languageChangeSubject;
        public string CurrentLanguage { get; private set; }

        public MornLocalizeCore(string language)
        {
            CurrentLanguage = language;
        }
        
        public void ChangeLanguage(string language)
        {
            CurrentLanguage = language;
            _languageChangeSubject.OnNext(language);
        }
    }
}