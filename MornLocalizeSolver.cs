using System;
using UnityEngine;

namespace MornLocalize
{
    [Serializable]
    public class MornLocalizeSolver
    {
        [SerializeField] private string key;

        public string Get(string language)
        {
            return MornLocalizeMasterData.Instance.Get(language, key);
        }

        public string Get()
        {
            return MornLocalizeMasterData.Instance.Get(key);
        }
    }
}