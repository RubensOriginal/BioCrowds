using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TranslateTMP_Text : MonoBehaviour
{
    public static Language language = Language.EN_US;
    public enum Language
    {
        EN_US,
        PT_BR
    }
    [Multiline()]
    public string text_EN_US;
    [Multiline()]
    public string text_PT_BR;

    public TMP_Text text;


    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        if (language == Language.EN_US)
            text.text = text_EN_US;
        else if (language == Language.PT_BR)
            text.text = text_PT_BR;
    }
}