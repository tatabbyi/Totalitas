using System.Collections;
using UnityEngine;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class Dialogue1 : MonoBehaviour
{
    [Tooltip("TextMeshProUGUI component that will display dialogue.")]
    public TextMeshProUGUI textComponent;

    [Tooltip("Lines of dialogue to display.")]
    public string[] lines;

    [Tooltip("Seconds between each character when typing.")]
    public float textSpeed = 0.05f;

    [Tooltip("Automatically start showing dialogue on Start().")]
    public bool autoStart = true;

    private int index;
    private bool isTyping;
    private Coroutine typingCoroutine;

    [Tooltip("Keys used to advance the dialogue (when not typing) or complete the current line (when typing).")]
    public KeyCode[] advanceKeys = new[] { KeyCode.Space, KeyCode.Return, KeyCode.KeypadEnter };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (textComponent == null)
        {
            Debug.LogError("Dialogue1: TextComponent is not assigned. Assign a TextMeshProUGUI in the Inspector.", this);
            return;
        }

        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("Dialogue1: Lines array is empty. Add dialogue lines in the Inspector.", this);
            textComponent.text = string.Empty;
            return;
        }

        if (autoStart)
        {
            textComponent.text = string.Empty;
            StartDialogue();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsAdvanceKeyPressed())
        {
            if (isTyping)
            {
                // Finish current line instantly
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }

                textComponent.text = lines[index];
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    bool IsAdvanceKeyPressed()
    {
        // New Input System support (if enabled in project settings)
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame ||
                Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.numpadEnterKey.wasPressedThisFrame)
            {
                return true;
            }
        }
        return false;
#else
        // Legacy Input system fallback (only if Input System not enabled)
        foreach (var key in advanceKeys)
        {
            if (Input.GetKeyDown(key))
            {
                return true;
            }
        }
        return false;
#endif
    }

    void StartDialogue()
    {
        index = 0;
        typingCoroutine = StartCoroutine(TypeLine());
    }

    /// <summary>
    /// Advances the dialogue (or finishes the current line if still typing).
    /// Can be wired to a UI Button OnClick event.
    /// </summary>
    public void AdvanceDialogue()
    {
        if (isTyping)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            textComponent.text = lines[index];
            isTyping = false;
        }
        else
        {
            NextLine();
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
            // Reached end of dialogue, optionally disable this component
            enabled = false;
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        textComponent.text = string.Empty;

        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

}
