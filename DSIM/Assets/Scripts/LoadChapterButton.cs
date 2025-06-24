using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadChapterButton : MonoBehaviour
{
    public string chapterName;
    public string fileName;

    public void StartChapter()
    {
        DialogueLoader.LoadDialogueScene(chapterName, fileName);
    }
}
