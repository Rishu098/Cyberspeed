using UnityEngine;

public static class SaveManager
{
    const string KEY = "MEMORY_SAVE";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(KEY, json);
        PlayerPrefs.Save();
    }

    public static SaveData Load()
    {
        if (!PlayerPrefs.HasKey(KEY))
            return null;

        string json = PlayerPrefs.GetString(KEY);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(KEY);
    }
}
