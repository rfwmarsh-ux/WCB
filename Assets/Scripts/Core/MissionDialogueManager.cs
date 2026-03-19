using UnityEngine;

public class MissionSpeechBubble : MonoBehaviour
{
    public MissionGiverType giverType;
    public string message;
    public float displayDuration = 5f;
    public float fadeSpeed = 2f;

    private float timer;
    private bool isVisible;
    private float alpha = 0f;

    private GameObject bubble;
    private GameObject characterImage;
    private TMPro.TextMeshPro textDisplay;

    public void Initialize(MissionGiverType type, string text)
    {
        giverType = type;
        message = text;
        
        CreateBubble();
        Show();
    }

    private void CreateBubble()
    {
        bubble = new GameObject("SpeechBubble");
        bubble.transform.SetParent(transform);
        bubble.transform.localPosition = new Vector3(0, 4f, 0);

        GameObject bubbleBg = new GameObject("BubbleBg");
        bubbleBg.transform.SetParent(bubble.transform);
        
        SpriteRenderer bgSr = bubbleBg.AddComponent<SpriteRenderer>();
        bgSr.sprite = SpriteHelper.GetDefaultSprite();
        bgSr.color = new Color(1f, 1f, 0.9f, 0.95f);
        bgSr.sortingOrder = 100;
        bubbleBg.transform.localScale = new Vector3(8f, 4f, 1f);

        GameObject bubbleTail = new GameObject("BubbleTail");
        bubbleTail.transform.SetParent(bubble.transform);
        
        SpriteRenderer tailSr = bubbleTail.AddComponent<SpriteRenderer>();
        tailSr.sprite = SpriteHelper.GetDefaultSprite();
        tailSr.color = new Color(1f, 1f, 0.9f, 0.95f);
        tailSr.sortingOrder = 100;
        bubbleTail.transform.localScale = new Vector3(1f, 1f, 1f);
        bubbleTail.transform.localPosition = new Vector3(-2f, -2f, 0);
        bubbleTail.transform.rotation = Quaternion.Euler(0, 0, 45);

        textDisplay = CreateText(bubble);
        
        bubble.SetActive(false);
    }

    private TMPro.TextMeshPro CreateText(GameObject parent)
    {
        GameObject textGO = new GameObject("MessageText");
        textGO.transform.SetParent(parent.transform);
        textGO.transform.localPosition = new Vector3(0, 0, -0.1f);

        TMPro.TextMeshPro tmp = textGO.AddComponent<TMPro.TextMeshPro>();
        tmp.fontSize = 4;
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
        tmp.color = Color.black;
        tmp.text = message;
        
        return tmp;
    }

    public void Show()
    {
        isVisible = true;
        timer = displayDuration;
        bubble.SetActive(true);
    }

    public void Hide()
    {
        isVisible = false;
    }

    private void Update()
    {
        if (!isVisible) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Hide();
            Destroy(gameObject, 1f);
            return;
        }

        if (timer < 1f)
        {
            alpha = Mathf.Lerp(alpha, 0, fadeSpeed * Time.deltaTime);
        }
        else if (alpha < 1f)
        {
            alpha = Mathf.Lerp(alpha, 1f, fadeSpeed * Time.deltaTime);
        }

        UpdateAlpha();
    }

    private void UpdateAlpha()
    {
        SpriteRenderer[] srs = bubble.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in srs)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }

        if (textDisplay != null)
        {
            Color c = textDisplay.color;
            c.a = alpha;
            textDisplay.color = c;
        }
    }
}

public class MissionDialogueManager : MonoBehaviour
{
    public static MissionDialogueManager Instance { get; private set; }

    private GameObject dialoguePanel;
    private MissionSpeechBubble currentBubble;
    private MissionGiverType currentGiver;
    private string currentMessage;
    private bool isDisplaying;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        CreateDialoguePanel();
    }

    private void CreateDialoguePanel()
    {
        dialoguePanel = new GameObject("MissionDialoguePanel");
        dialoguePanel.transform.position = new Vector3(Screen.width / 2, 80, 0);
        
        SpriteRenderer sr = dialoguePanel.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.sortingOrder = 50;
        sr.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        dialoguePanel.transform.localScale = new Vector3(Screen.width - 100, 120, 1f);
    }

    public void ShowDialogue(MissionGiverType giver, string message)
    {
        if (isDisplaying && currentBubble != null)
        {
            Destroy(currentBubble.gameObject);
        }

        currentGiver = giver;
        currentMessage = message;
        
        MissionGiverCharacter giverChar = MissionGiverManager.Instance.GetGiver(giver);
        if (giverChar == null) return;

        GameObject bubbleObj = new GameObject("DialogueBubble");
        bubbleObj.transform.SetParent(dialoguePanel.transform);
        bubbleObj.transform.localPosition = new Vector3(-400 + (int)giver * 100, 30, 0);

        currentBubble = bubbleObj.AddComponent<MissionSpeechBubble>();
        currentBubble.Initialize(giver, message);
        
        isDisplaying = true;
    }

    public void ShowQuickDialogue(string message)
    {
        ShowDialogue(MissionGiverType.Dave, message);
    }

    public void HideDialogue()
    {
        if (currentBubble != null)
        {
            currentBubble.Hide();
            Destroy(currentBubble.gameObject, 1f);
        }
        isDisplaying = false;
    }

    private void OnGUI()
    {
        if (!isDisplaying) return;

        GUI.skin.label.fontSize = 18;
        GUI.color = Color.white;
        
        string giverName = currentGiver.ToString();
        GUI.Label(new Rect(Screen.width / 2 - 100, 20, 200, 30), giverName);
    }
}
