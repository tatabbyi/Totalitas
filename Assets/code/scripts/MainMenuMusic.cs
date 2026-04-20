using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    [SerializeField] private AudioClip menuClip;
    [SerializeField, Range(0f, 1f)] private float volume = 0.75f;

    private void Start()
    {
        if (menuClip == null)
            return;

        AudioSource source = GetComponent<AudioSource>();
        if (source == null)
            source = gameObject.AddComponent<AudioSource>();

        source.clip = menuClip;
        source.loop = true;
        source.volume = volume;
        source.spatialBlend = 0f;
        source.playOnAwake = false;
        source.Play();
    }
}
