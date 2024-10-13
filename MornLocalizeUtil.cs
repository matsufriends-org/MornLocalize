using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MornLocalize
{
    public static class MornLocalizeUtil
    {
        public static void OpenMasterData()
        {
            var masterData = MornLocalizeGlobalSettings.Instance.MasterData;
            masterData.Open();
        }

        public static void LoadMasterData()
        {
            var masterData = MornLocalizeGlobalSettings.Instance.MasterData;
            masterData.LoadAsync().Forget();
        }
    }

    [Serializable]
    internal class MornLocalizeDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private TKey[] _keys;
        [SerializeField] private TValue[] _values;

        public void OnBeforeSerialize()
        {
            _keys = new TKey[Count];
            _values = new TValue[Count];
            var index = 0;
            foreach (var pair in this)
            {
                _keys[index] = pair.Key;
                _values[index] = pair.Value;
                index++;
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            for (var i = 0; i < _keys.Length; i++)
            {
                Add(_keys[i], _values[i]);
            }
        }
    }

    internal static class MornLocalizeLogger
    {
#if DISABLE_MORN_LOCALIZE_LOG
        private const bool SHOW_LOG = false;
#else
        private const bool SHOW_LOG = true;
#endif
        private const string PREFIX = "[MornLocalize] ";

        public static void Log(string message)
        {
            if (SHOW_LOG)
            {
                Debug.Log(PREFIX + message);
            }
        }

        public static void LogError(string message)
        {
            if (SHOW_LOG)
            {
                Debug.LogError(PREFIX + message);
            }
        }

        public static void LogWarning(string message)
        {
            if (SHOW_LOG)
            {
                Debug.LogWarning(PREFIX + message);
            }
        }
    }
}