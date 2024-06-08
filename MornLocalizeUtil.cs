using System;
using System.Collections.Generic;
using UnityEngine;

namespace MornLocalize
{
    [Serializable]
    internal class MornLocalizeDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private TKey[] keys;
        [SerializeField] private TValue[] values;

        public void OnBeforeSerialize()
        {
            keys = new TKey[Count];
            values = new TValue[Count];
            var index = 0;
            foreach (var pair in this)
            {
                keys[index] = pair.Key;
                values[index] = pair.Value;
                index++;
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            for (var i = 0; i < keys.Length; i++)
            {
                Add(keys[i], values[i]);
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