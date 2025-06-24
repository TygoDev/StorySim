using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public Image dialogueBoxImage;
    public Image portraitImage;
    public Image backgroundImage;
    public float transitionWaitTime = 2f;

    private Dictionary<int, DialogueLine> dialogueDict;
    private int currentId;
    private DialogueLine currentLine;
    private string chapterName;
    private string fullLine;

    public GameObject choicePanel;
    public Button[] choiceButtons;

    private bool isTyping = false;
    private bool isWaiting = false;

    void Start()
    {
        portraitImage.enabled = false;
        LoadChapter(DialogueLoader.chapter);
        LoadDialogue(DialogueLoader.dialogueFile);
        currentId = 1;
        ShowLine(currentId);
    }

    void Update()
    {
        if (isWaiting)
            return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = fullLine;
                isTyping = false;
                ShowChoicesIfNeeded(currentLine);
            }
            else if (!choicePanel.activeInHierarchy)
            {
                AdvanceDialogue();
            }
        }
    }

    void LoadChapter(string pChapterName)
    {
        chapterName = pChapterName;
    }

    void LoadDialogue(string fileName)
    {
        TextAsset file = Resources.Load<TextAsset>("Dialogue/"+ chapterName +"/"+ fileName);
        DialogueLine[] lines = JsonHelper.FromJson<DialogueLine>(file.text);
        dialogueDict = new Dictionary<int, DialogueLine>();
        foreach (DialogueLine line in lines)
        {
            dialogueDict[line.id] = line;
        }
    }

    void ShowLine(int id)
    {
        if (dialogueDict.TryGetValue(id, out DialogueLine line))
        {
            dialogueBoxImage.enabled = true;
            currentLine = line;
            SetPortrait(line.portrait);
            SetBackground(line.background);
            speakerText.text = line.speaker;
            fullLine = line.text;
            StartCoroutine(TypeSentence(fullLine));
        }
    }

    void SetPortrait(string portraitName)
    {
        if (!string.IsNullOrEmpty(portraitName))
        {
            Sprite sprite = Resources.Load<Sprite>("Sprites/" + portraitName);
            if (sprite != null)
            {
                portraitImage.sprite = sprite;
                portraitImage.enabled = true;
            }
            else
            {
                Debug.LogWarning("Portrait not found: " + portraitName);
                portraitImage.enabled = false;
            }
        }
        else
        {
            portraitImage.enabled = false;
        }
    }

    void SetBackground(string backgroundName)
    {
        if (!string.IsNullOrEmpty(backgroundName))
        {
            Sprite sprite = Resources.Load<Sprite>("Sprites/" + backgroundName);
            if (sprite != null)
            {
                backgroundImage.sprite = sprite;
                backgroundImage.enabled = true;
            }
            else
            {
                Debug.LogWarning("Portrait not found: " + backgroundName);
                backgroundImage.enabled = false;
            }
        }
        else
        {
            backgroundImage.enabled = false;
        }
    }

    void ShowChoicesIfNeeded(DialogueLine line)
    {
        if (line.choices != null && line.choices.Length > 0)
        {
            ShowChoices(line.choices);
        }
        else
        {
            currentId = line.nextId;
        }
    }

    void ShowChoices(DialogueChoice[] choices)
    {
        choicePanel.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Length)
            {
                var choice = choices[i];

                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choice.text;

                choiceButtons[i].onClick.RemoveAllListeners();

                choiceButtons[i].onClick.AddListener(() => {
                    HideChoices();

                    if (!string.IsNullOrEmpty(choice.nextFile))
                    {
                        StartCoroutine(LoadNextFileWithDelay(choice.nextFile, transitionWaitTime));
                    }
                    else if (choice.nextId == -1)
                    {
                        StartCoroutine(ReturnToChapterSelect(transitionWaitTime));
                    }
                    else
                    {
                        ShowLine(choice.nextId);
                    }
                });
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void HideChoices()
    {
        choicePanel.SetActive(false);
    }

    void AdvanceDialogue()
    {
        if (currentLine == null) return;

        if (!string.IsNullOrEmpty(currentLine.nextFile))
        {
            StartCoroutine(LoadNextFileWithDelay(currentLine.nextFile, transitionWaitTime));
        }
        else if (currentId == -1)
        {
            StartCoroutine(ReturnToChapterSelect(transitionWaitTime));
        }
        else
        {
            ShowLine(currentId);
        }
    }

    void EndDialogue()
    {
        dialogueText.text = "";
        speakerText.text = "";
        dialogueBoxImage.enabled = false;
        portraitImage.enabled = false;
        HideChoices();
        currentId = -1;
    }


    IEnumerator LoadNextFileWithDelay(string fileName, float delaySeconds)
    {
        isWaiting = true;
        EndDialogue();

        yield return new WaitForSeconds(delaySeconds);

        LoadDialogue(fileName);
        currentId = 1;
        ShowLine(currentId);

        isWaiting = false;
    }

    IEnumerator ReturnToChapterSelect(float delaySeconds)
    {
        isWaiting = true;
        EndDialogue();

        yield return new WaitForSeconds(delaySeconds);

        SceneManager.LoadScene("ChapterSelection");

        isWaiting = false;
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
        isTyping = false;

        ShowChoicesIfNeeded(currentLine);
    }
}
