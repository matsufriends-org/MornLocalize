using System;
using UnityEngine;

namespace MornLocalize
{
    [Serializable]
    public class MornLocalizeSolver
    {
        [SerializeField] private string _key;

        public string Get(string language)
        {
            return MornLocalizeMasterData.Instance.Get(language, _key);
        }

        public string Get()
        {
            return MornLocalizeMasterData.Instance.Get(_key);
        }
    }
}