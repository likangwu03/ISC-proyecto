using UnityEngine;
using TMPro;
using System.Collections;

public class ChatBubble : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMesh;
    [SerializeField]
    private TextMeshProUGUI emojiTextMesh;
    [SerializeField]
    private RectTransform backgroundRect;
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private float timePerCharacter = 0.05f;

    [SerializeField]
    private float fadeDuration = 2.0f;

    private float fadeTimer;
    private enum FadeState { FadeIn, Typing, Idle, FadeOut }
    private FadeState fadeState;

    public enum IconType
    {
        Happy,
        Neutral,
        Angry,
        TreatingPatient
    }

    private float timer;
    private int characterIndex;
    private string textToWrite;
    private string prefixText;

    private Coroutine setupRoutine;

    private void Start()
    {
        GAgent agent = GetComponentInParent<GAgent>();
        string agentName = agent.GetAgentName();
        prefixText = $"{agentName}: ";

        //Setup("Hello World!", IconType.Happy);
    }

    public void Setup(string message, IconType icon)
    {
        bool wasHidden = canvasGroup.alpha <= 0f || !gameObject.activeSelf;

        gameObject.SetActive(true);

        if (setupRoutine != null)
        {
            StopCoroutine(setupRoutine);
        }

        textToWrite = message;
        characterIndex = 0;
        timer = timePerCharacter;

        emojiTextMesh.SetText(GetIcon(icon));

        if (wasHidden)
        {
            canvasGroup.alpha = 0f;
            fadeTimer = 0f;
            fadeState = FadeState.FadeIn;
        }
        else
        {
            canvasGroup.alpha = 1f;
            fadeState = FadeState.Idle;
        }

        // Se lanza una corrutina porque sino el tamano del texto no se cacula bien al activar el GameObject en el mismo frame
        setupRoutine = StartCoroutine(SetupLayoutNextFrame());
    }

    private IEnumerator SetupLayoutNextFrame()
    {
        // Esperamos al siguiente frame
        yield return null;

        textMesh.SetText(prefixText + textToWrite);
        textMesh.ForceMeshUpdate();

        Vector2 textSize = textMesh.GetRenderedValues(false);

        Vector2 padding = new Vector2(200f, 90f);
        backgroundRect.sizeDelta = textSize + padding;

        Vector2 offset = new Vector2(-180f, 0f);
        backgroundRect.anchoredPosition =
            new Vector2(textSize.x / 2f, 0f) + offset;

        textMesh.SetText(prefixText);
    }

    public void StartFadeOut()
    {
        if (fadeState == FadeState.Idle)
        {
            fadeTimer = 0f;
            fadeState = FadeState.FadeOut;
        }
    }


    private string GetIcon(IconType type)
    {
        return type switch
        {
            IconType.Happy => "\U0001F60A",             // 😊
            IconType.Neutral => "\U0001F610",           // 😐
            IconType.Angry => "\U0001F620",             // 😠
            IconType.TreatingPatient => "\U0001FA7A",   // Estetoscopio
            _ => "\U00002753"                           // ❓
        };
    }

    private void Update()
    {
        switch (fadeState)
        {
            case FadeState.FadeIn:
                fadeTimer += Time.deltaTime;
                canvasGroup.alpha = Mathf.SmoothStep(0f, 1f, fadeTimer / fadeDuration);

                if (fadeTimer >= fadeDuration)
                {
                    canvasGroup.alpha = 1f;
                    fadeState = FadeState.Typing;
                }
                break;
            case FadeState.Typing:
                timer -= Time.deltaTime;
                while (timer <= 0f)
                {
                    timer += timePerCharacter;
                    characterIndex++;

                    string text = textToWrite.Substring(0, characterIndex);
                    textMesh.SetText(prefixText + text);

                    if (characterIndex >= textToWrite.Length)
                    {
                        fadeState = FadeState.Idle;
                        break;
                    }
                }
                break;
            case FadeState.Idle:
                break;
            case FadeState.FadeOut:
                fadeTimer += Time.deltaTime;
                canvasGroup.alpha = Mathf.SmoothStep(1f, 0f, fadeTimer / fadeDuration);

                if (fadeTimer >= fadeDuration)
                {
                    canvasGroup.alpha = 0f;
                    gameObject.SetActive(false);
                }
                break;
        }
    }
}
