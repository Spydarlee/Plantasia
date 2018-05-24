using Mono.Csv;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class LocalisationManager : MonoBehaviour
{
    // ---------------------------------------------------------------------

    public enum Language
    {
        English,
        French,
        German,
        Spanish,
        Japanese
    }

    // ---------------------------------------------------------------------

    [System.Serializable]
    public struct LanguageData
    {
        public Language     Language;
        public TextAsset    LocFile;
    }

    // ---------------------------------------------------------------------

    public static LocalisationManager   Instance = null;
    public Language                     CurrentLanguage = Language.English;
    public List<LanguageData>           Languages = new List<LanguageData>();
    public const string                 kInvalidLocID = "INVALID_LOC_ID";

    // ---------------------------------------------------------------------

    public delegate void LanguageChangeAction();
    public event LanguageChangeAction OnChangeLanguage = null;

    // ---------------------------------------------------------------------

    private Dictionary<Language, LanguageData> mLanguages = new Dictionary<Language, LanguageData>();
    private Dictionary<string, string> mLocEntries = new Dictionary<string, string>();

    // ---------------------------------------------------------------------

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < Languages.Count; i++)
        {
            mLanguages.Add(Languages[i].Language, Languages[i]);
        }

        // Load the default language
        LoadLocFile(mLanguages[CurrentLanguage].LocFile, ref mLocEntries);
    }

    // ---------------------------------------------------------------------

    public void ChangeLanguage(Language newLanaguage)
    {
        if (newLanaguage != CurrentLanguage)
        {
            CurrentLanguage = newLanaguage;

            Debug.Log("Changing language to: " + CurrentLanguage.ToString());
            LoadLocFile(mLanguages[CurrentLanguage].LocFile, ref mLocEntries);

            if (OnChangeLanguage != null)
            {
                OnChangeLanguage.Invoke();
            }
        }
    }

    // ---------------------------------------------------------------------

    public string GetLocText(string locID)
    {
        if (mLocEntries.ContainsKey(locID))
        {
            return mLocEntries[locID];
        }

        Debug.LogError("LocalisationManager doesn't contain an entry with the locID: " + locID);
        return "Missing loctext: " + locID;
    }

    // ---------------------------------------------------------------------

    private static void LoadLocFile(TextAsset locFile, ref Dictionary<string, string> locEntries)
    {
        locEntries.Clear();

        List<string> row = new List<string>();
        using (var reader = new CsvFileReader(new MemoryStream(locFile.bytes)))
        {
            // First row contains headers, so load that but ignore it
            reader.ReadRow(row);

            // Read data row by row
            while (reader.ReadRow(row))
            {
                var locID = row[0];
                var locText = row[1];
                locEntries.Add(locID, locText);
            }
        }
    }

    // ---------------------------------------------------------------------
}
