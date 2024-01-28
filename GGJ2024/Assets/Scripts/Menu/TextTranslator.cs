using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextTranslator : MonoBehaviour
{
    TextMeshProUGUI _text;
    string _key;
    // Start is called before the first frame update
    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _key = _text.text;

        _SetText();
        Data.OnLanguageChanged.AddListener(_SetText);
    }

    private void _SetText()
    {
        _text.text = Data.LOCALIZATION[_key][Data.CURRENT_LANGUAGE];
    }
}
