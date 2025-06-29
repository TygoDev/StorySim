using UnityEngine;

public static class GameProgress
{
    public static string currentChapterUnlocked = "ExampleChapter1";

    public static void LoadProgress()
    {
        currentChapterUnlocked = PlayerPrefs.GetString("CurrentChapter", currentChapterUnlocked);
    }

    public static void SaveProgress(string chapterUnlocked)
    {
        currentChapterUnlocked = chapterUnlocked;
        PlayerPrefs.SetString("CurrentChapter", chapterUnlocked);
        PlayerPrefs.Save();
    }
}
