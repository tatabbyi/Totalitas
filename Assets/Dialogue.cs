using UnityEngine;
using TMPro;   // Remove this if you use legacy Text
using UnityEngine.InputSystem;

public class DialogueController : MonoBehaviour
{
    public TMP_Text dialogueText;   // Assign in Inspector
    public string[] lines;          // Fill in Inspector

    private int index = 0;

    void Start()
    {
        if (dialogueText == null)
        {
            Debug.LogWarning("DialogueController: dialogueText is not assigned.", this);
            return;
        }

        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("DialogueController: lines array is empty.", this);
            return;
        }

        index = 0;
        dialogueText.text = lines[index];
    }

    void Update()
    {
        if (IsClick())
        {
            NextLine();
        }
    }

    private bool IsClick()
    {
        bool clicked = false;

#if ENABLE_INPUT_SYSTEM
        clicked = Mouse.current?.leftButton.wasPressedThisFrame ?? false;
#endif
        clicked |= Input.GetMouseButtonDown(0);

        return clicked;
    }

    void NextLine()
    {
        if (lines == null || lines.Length == 0 || dialogueText == null)
            return;

        if (index < lines.Length - 1)
        {
            index++;
            dialogueText.text = lines[index];
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
