using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;

public static class Data
{
    // Start is called before the first frame update 
    public static string CURRENT_LANGUAGE = "en";

    private static string[] _LANGUAGES;
    public static string[] LANGUAGES{
        get{
            if(_LANGUAGES == null) _LoadLocalizationData();
            return _LANGUAGES;
        }
    }
    private static Dictionary<string, Dictionary<string, string>> _LOCALIZATION;

    public static Dictionary<string, Dictionary<string, string>> LOCALIZATION{
        get{
            if(_LOCALIZATION ==null) _LoadLocalizationData();
            return _LOCALIZATION;
        }
    }
    // Singleton
    private static UnityEvent _OnLanguageChanged;
    public static UnityEvent OnLanguageChanged{
        get{
            if(_OnLanguageChanged ==null)
                _OnLanguageChanged = new UnityEvent();
            return _OnLanguageChanged;
        }
    }

    private static string _ReadFromFile(string path){
         /*TextAsset textAsset = Resources.Load<TextAsset>(path);
         if (textAsset != null)
        {
            // Access the JSON content
            string json = textAsset.text;
            Debug.Log("JSON Content: " + json);
            return json;
        }
        else
        {
            Debug.LogError("Failed to load the JSON file from Resources.");
        }*/
        if(File.Exists(path)){
            using(StreamReader reader = new StreamReader(path)){
                string json = reader.ReadToEnd();
                return json;
            }
        }else{
            Debug.LogWarning("File not found !");
        }
        return "ok";
    }

    private static void _LoadLocalizationData(){
        _LOCALIZATION = new Dictionary<string, Dictionary<string, string>>();
        //string json = _ReadFromFile("Resources/localization_data.json");
        string json = _ReadFromFile(Path.Combine(Application.dataPath,"Resources","localization_data.json"));
        LocalizationData d = JsonUtility.FromJson<LocalizationData>(json);
        _LANGUAGES = d.languages;

        foreach (LocalizationMapping map in d.table)
        {
            _LOCALIZATION[map.key] = new Dictionary<string, string>();
            foreach (LocalizationValue val in map.values)
            {
                _LOCALIZATION[map.key].Add(val.lang, val.value);
            }
        }
    }
}
