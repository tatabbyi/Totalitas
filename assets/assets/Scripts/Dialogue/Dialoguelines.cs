using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Line")]
public class DialogueLine : ScriptableObject
{
    [TextArea(1, 2)] public string speakerName;
    [TextArea(3, 10)] public string text;
    public Sprite portrait;
    [Range(0.005f, 0.1f)] public float typewriterSpeed = 0.03f;
}