using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterSelectionManager : MonoBehaviour
{
    public Button[] chapterButtons;

    void Start()
    {
        GameProgress.LoadProgress();

        for (int i = 0; i < chapterButtons.Length; i++)
        {
            chapterButtons[i].interactable = (chapterButtons[i].GetComponent<LoadChapterButton>().chapterName == GameProgress.currentChapterUnlocked);
        }
    }

    public void StartChapter(string chapterName, string dialogueFile)
    {
        DialogueLoader.LoadDialogueScene(chapterName, dialogueFile);
    }
}
