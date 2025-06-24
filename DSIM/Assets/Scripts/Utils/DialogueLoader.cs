using UnityEngine.SceneManagement;

public static class DialogueLoader
{
    public static string chapter;
    public static string dialogueFile;

    public static void LoadDialogueScene(string pChapter, string pFile)
    {
        dialogueFile = pFile;
        chapter = pChapter;
        SceneManager.LoadScene("DialogueScene");
    }
}
