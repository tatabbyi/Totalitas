using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene01 : MonoBehaviour {

    public GameObject fadeScreenIn;
    public GameObject charYemayaTalking;
    public GameObject charYemayaScared;
    public GameObject charEnemy;

    RawImage _portraitYemayaTalking;
    RawImage _portraitYemayaScared;
    RawImage _portraitEnemy;

    public GameObject TextBox;
    //public GameObject TextBoxEnemy;

    [SerializeField] AudioSource Nya;
    [SerializeField] AudioSource uwu;
    [SerializeField] AudioSource EvilLaugh;
    [SerializeField] AudioSource Scream;
    [SerializeField] AudioSource Woosh;

    [SerializeField] string textToSpeak;
    [SerializeField] int currentTextLength;
    [SerializeField] int textLenght;
    [SerializeField] GameObject mainTextObject;

    [SerializeField] GameObject nextButton;
    [SerializeField] int eventPos = 0;

    [SerializeField] GameObject charName;
    [SerializeField] TMPro.TMP_FontAsset yemayaFont;

    [SerializeField] private string guideSceneName = "Guide";

    const float DialogueFontSize = 35f;

    void Awake()
    {
        GuideFlow.DisableGuideWorldRenderingIfOverlay(gameObject.scene);
        FixNullTextureRawImagesInGuideScene();
        ApplyUnifiedFontsToStaticWidgets();
        RemoveStalePortraitCanvasGroups();
        SetupPortraitRawImages();
    }

    /// <summary>
    /// CanvasGroup on the same GameObject as a RawImage can leave the portrait solid white; remove any we added earlier.
    /// </summary>
    void RemoveStalePortraitCanvasGroups()
    {
        foreach (var go in new[] { charYemayaTalking, charYemayaScared, charEnemy })
        {
            if (go == null)
                continue;
            var cg = go.GetComponent<CanvasGroup>();
            if (cg != null)
                Destroy(cg);
        }
    }

    /// <summary>
    /// Keep portrait GameObjects active and hide with RawImage color alpha (not SetActive, not CanvasGroup on same GO).
    /// </summary>
    void SetupPortraitRawImages()
    {
        _portraitYemayaTalking = ActivateAndGetRawImage(charYemayaTalking);
        _portraitYemayaScared = ActivateAndGetRawImage(charYemayaScared);
        _portraitEnemy = ActivateAndGetRawImage(charEnemy);
        SetPortraitVisible(_portraitYemayaTalking, false);
        SetPortraitVisible(_portraitYemayaScared, false);
        SetPortraitVisible(_portraitEnemy, false);
    }

    static RawImage ActivateAndGetRawImage(GameObject go)
    {
        if (go == null)
            return null;
        go.SetActive(true);
        return go.GetComponent<RawImage>();
    }

    static void SetPortraitVisible(RawImage ri, bool visible)
    {
        if (ri == null)
            return;
        var c = ri.color;
        c.a = visible ? 1f : 0f;
        ri.color = c;
        ri.raycastTarget = visible;
    }

    /// <summary>
    /// Only tint panels that intentionally have no texture. Never assign whiteTexture to character portraits or it stays white.
    /// </summary>
    void FixNullTextureRawImagesInGuideScene()
    {
        Scene s = gameObject.scene;
        foreach (var ri in Object.FindObjectsByType<RawImage>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (ri == null || ri.gameObject.scene != s || ri.texture != null)
                continue;
            string n = ri.gameObject.name;
            if (n != "FadeIn" && n != "TextBox" && n != "BackName")
                continue;
            ri.texture = Texture2D.whiteTexture;
        }
    }

    void ApplyUnifiedFontsToStaticWidgets()
    {
        if (yemayaFont == null)
            return;
        if (TextBox != null)
        {
            var speak = TextBox.GetComponent<TMPro.TMP_Text>();
            if (speak != null)
                ApplyDialogueStyle(speak);
        }
        if (charName != null)
        {
            var cn = charName.GetComponent<TMPro.TMP_Text>();
            if (cn != null)
                ApplyDialogueStyle(cn);
        }
        if (nextButton != null)
        {
            var nb = nextButton.GetComponentInChildren<TMPro.TMP_Text>(true);
            if (nb != null)
                ApplyDialogueStyle(nb);
        }
    }

    void ApplyDialogueStyle(TMPro.TMP_Text tmp)
    {
        if (tmp == null || yemayaFont == null)
            return;
        tmp.font = yemayaFont;
        tmp.fontStyle = TMPro.FontStyles.Normal;
        tmp.fontSize = DialogueFontSize;
    }

    void PrepareSpeakLine(string line)
    {
        var tmp = TextBox.GetComponent<TMPro.TMP_Text>();
        ApplyDialogueStyle(tmp);
        tmp.text = line;
    }

    void SetCharNameLabel(string label)
    {
        if (charName == null)
            return;
        var tmp = charName.GetComponent<TMPro.TMP_Text>();
        tmp.text = label;
        ApplyDialogueStyle(tmp);
    }

    void Update()
    {
        textLenght = TextCreator.charCount; 
    }

    void Start()
    {
        StartCoroutine(EventStart());
    }

    IEnumerator EventStart()
    {
        //event 1 (realtime so paused Time.timeScale cannot block the fade)
        yield return new WaitForSecondsRealtime(2f);
        if (fadeScreenIn != null)
            fadeScreenIn.SetActive(false);
        SetPortraitVisible(_portraitYemayaTalking, true);
        //this is where our text function will be;
        mainTextObject.SetActive(true);

        textToSpeak = "Greetings, Warrior. Totalitas is a game where you build and protect the maze from enemies" +
            " across different eras of life. You will be required to time‑travel and rely on your survival instincts.";
        PrepareSpeakLine(textToSpeak);
        currentTextLength = textToSpeak.Length;
        TextCreator.runTextPrint = true;
        Nya.Play();
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => textLenght >= currentTextLength);
        yield return new WaitForSeconds(0.05f);

        SetPortraitVisible(_portraitYemayaTalking, false);

        nextButton.SetActive(true);
        eventPos = 2;

        
    }

    IEnumerator EventTwo()
    {
        //event 2
        nextButton.SetActive(false);
        SetPortraitVisible(_portraitYemayaTalking, true);
        TextBox.SetActive(true);

        //line to change name of the speaker - works
        //charName.GetComponent<TMPro.TMP_Text>().text = "Enemie";  
        

        textToSpeak = "In 1831, the city of Breslau was under siege by soldiers from Dorpat. The people of Breslau fought bravely and heroically, " +
            "forcing the enemy to retreat. The only problem now is that they are attempting to fall back into the maze, where important artefacts are kept.";
        PrepareSpeakLine(textToSpeak);
        currentTextLength = textToSpeak.Length;
        TextCreator.runTextPrint = true;
        Nya.Play();
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => textLenght >= currentTextLength);
        yield return new WaitForSeconds(0.05f);
        SetPortraitVisible(_portraitYemayaTalking, false);

        nextButton.SetActive(true);
        eventPos = 3;

    }

    IEnumerator EventThree()
    {
        //event 3
        nextButton.SetActive(false);
        SetPortraitVisible(_portraitYemayaTalking, true);
        TextBox.SetActive(true);

        //line to change name of the speaker - works
        //charName.GetComponent<TMPro.TMP_Text>().text = "Enemie";  
        

        textToSpeak = "My quest for you is to protect the maze from any enemy trying to invade it.";
        PrepareSpeakLine(textToSpeak);
        currentTextLength = textToSpeak.Length;
        TextCreator.runTextPrint = true;
        uwu.Play();
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => textLenght >= currentTextLength);
        yield return new WaitForSeconds(0.05f);
        SetPortraitVisible(_portraitYemayaTalking, false);

        nextButton.SetActive(true);
        eventPos = 4;

    }

    IEnumerator EventFour()
    {
        nextButton.SetActive(false);

        SetCharNameLabel("Enemy");

        SetPortraitVisible(_portraitYemayaTalking, false);
        SetPortraitVisible(_portraitYemayaScared, true);
        SetPortraitVisible(_portraitEnemy, true);
        Scream.Play();

        yield return new WaitForSeconds(0.5f);
        EvilLaugh.Play();

        textToSpeak = "Retreat to the maze! We might still have a chance to steal their rare artefacts!";
        PrepareSpeakLine(textToSpeak);
        currentTextLength = textToSpeak.Length;
        TextCreator.runTextPrint = true;

        yield return new WaitForSeconds(1.05f);
        yield return new WaitUntil(() => textLenght >= currentTextLength);
        yield return new WaitForSeconds(0.05f);

        nextButton.SetActive(true);
        eventPos = 5;
    }

    IEnumerator EventFive()
    {
        nextButton.SetActive(false);

        Woosh.Play();
        SetPortraitVisible(_portraitEnemy, false);

        // Switch back to Yemaya
        SetCharNameLabel("Yemaya");
        SetPortraitVisible(_portraitYemayaScared, false);
        SetPortraitVisible(_portraitYemayaTalking, true);
        Nya.Play();

        textToSpeak = "I hear them coming. Good luck, Warrior — and stay alive.";
        PrepareSpeakLine(textToSpeak);
        currentTextLength = textToSpeak.Length;
        TextCreator.runTextPrint = true;

        yield return new WaitForSeconds(1.05f);
        yield return new WaitUntil(() => textLenght >= currentTextLength);
        yield return new WaitForSeconds(0.05f);

        nextButton.SetActive(true);
        eventPos = 6;
    }



    public void NextButton()
    {
        if (eventPos == 2)
        {
            StartCoroutine(EventTwo());
        }
        if (eventPos == 3)
        {
            StartCoroutine(EventThree());
        }
        if (eventPos == 4)
        {
            StartCoroutine(EventFour());
        }
        if (eventPos == 5)
        {
            StartCoroutine(EventFive());
        }
        if (eventPos == 6)
        {
            string next = GuideFlow.ConsumeNextSceneOrDefault("BaseLevel1");
            if (GuideFlow.TryFinishGuideOverlay(next, guideSceneName))
                return;
            if (!Application.CanStreamedLevelBeLoaded(next))
            {
                Debug.LogError($"Guide: scene '{next}' is not in Build Settings.");
                return;
            }
            SceneManager.LoadScene(next);
        }
    }
}
