﻿using System;

namespace MornLocalize
{
    /// <summary> 言語名 -> その言語のシート </summary>
    [Serializable]
    internal class LanguageToDataDictionary : SerializedDictionary<string, KeyToContentDictionary>
    {
    }
}