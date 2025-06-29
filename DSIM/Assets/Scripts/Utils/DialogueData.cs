[System.Serializable]
public class DialogueChoice
{
    public string text;
    public string nextFile;
    public string nextChapter;
    public int nextId;
}

[System.Serializable]
public class DialogueLine
{
    public int id;
    public string speaker;
    public string text;
    public string portrait;
    public string background;
    public string nextChapter;
    public string nextFile;
    public int nextId;
    public DialogueChoice[] choices;
}
