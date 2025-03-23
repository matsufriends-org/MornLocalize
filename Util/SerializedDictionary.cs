using System;
using System.Collections.Generic;
using UnityEngine;

namespace MornLocalize
{
    [Serializable]
    internal class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
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
}