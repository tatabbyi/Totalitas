using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene01 : MonoBehaviour {

    public GameObject fadeScreenIn;
    public GameObject charYemayaTalking;
    public GameObject charYemayaScared;
    public GameObject charEnemy;
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
    [SerializeField] TMPro.TMP_FontAsset enemyFont;
 


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
        //event 1
        yield return new WaitForSeconds(2);
        fadeScreenIn.SetActive(false);
        charYemayaTalking.SetActive(true);
        //this is where our text function will be;
        mainTextObject.SetActive(true);

        textToSpeak = "Greetings, Warrior. Totalitas is a game where you build and protect the maze from enemies" +
            " across different eras of life. You will be required to time‑travel and rely on your survival instincts.";
        TextBox.GetComponent<TMPro.TMP_Text>().text = textToSpeak;
        currentTextLength = textToSpeak.Length;
        TextCreator.runTextPrint = true;
        Nya.Play();
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(1);
        Debug.Log($"Waiting: charCount={textLenght}, target={currentTextLength}");
        yield return new WaitUntil(() => textLenght >= currentTextLength);
        yield return new WaitForSeconds(0.05f);

        charYemayaTalking.SetActive(false);

        nextButton.SetActive(true);
        eventPos = 2;

        
    }

    IEnumerator EventTwo()
    {
        //event 2
        nextButton.SetActive(false);
        charYemayaTalking.SetActive(true);
        TextBox.SetActive(true);

        //line to change name of the speaker - works
        //charName.GetComponent<TMPro.TMP_Text>().text = "Enemie";  
        

        textToSpeak = "In 1831, the city of Breslau was under siege by soldiers from Dorpat. The people of Breslau fought bravely and heroically, " +
            "forcing the enemy to retreat. The only problem now is that they are attempting to fall back into the maze, where important artefacts are kept.";
        TextBox.GetComponent<TMPro.TMP_Text>().text = textToSpeak;
        currentTextLength = textToSpeak.Length;
        TextCreator.runTextPrint = true;
        Nya.Play();
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => textLenght == currentTextLength);
        yield return new WaitForSeconds(0.05f);
        charYemayaTalking.SetActive(false);

        nextButton.SetActive(true);
        eventPos = 3;

    }

    IEnumerator EventThree()
    {
        //event 3
        nextButton.SetActive(false);
        charYemayaTalking.SetActive(true);
        TextBox.SetActive(true);

        //line to change name of the speaker - works
        //charName.GetComponent<TMPro.TMP_Text>().text = "Enemie";  
        

        textToSpeak = "My quest for you is to protect the maze from any enemy trying to invade it.";
        TextBox.GetComponent<TMPro.TMP_Text>().text = textToSpeak;
        currentTextLength = textToSpeak.Length;
        TextCreator.runTextPrint = true;
        uwu.Play();
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => textLenght == currentTextLength);
        yield return new WaitForSeconds(0.05f);
        charYemayaTalking.SetActive(false);

        nextButton.SetActive(true);
        eventPos = 4;

    }

    IEnumerator EventFour()
    {
        nextButton.SetActive(false);

        charName.GetComponent<TMPro.TMP_Text>().text = "Enemy";

        charYemayaTalking.SetActive(false);   // hide talking version
        charYemayaScared.SetActive(true);
        charEnemy.SetActive(true);
        Scream.Play();

        yield return new WaitForSeconds(0.5f);
        EvilLaugh.Play();

        TextBox.GetComponent<TMPro.TMP_Text>().font = enemyFont;
        TextBox.GetComponent<TMPro.TMP_Text>().fontStyle = TMPro.FontStyles.Bold;
        TextBox.GetComponent<TMPro.TMP_Text>().fontSize = 68;
        textToSpeak = "Retreat to the maze! We might still have a chance to steal their rare artefacts!";
        TextBox.GetComponent<TMPro.TMP_Text>().text = textToSpeak;
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
        charEnemy.SetActive(false);

        // Switch back to Yemaya
        charName.GetComponent<TMPro.TMP_Text>().text = "Yemaya";
        charYemayaScared.SetActive(false);
        charYemayaTalking.SetActive(true);
        Nya.Play();

        TextBox.GetComponent<TMPro.TMP_Text>().font = yemayaFont;
        TextBox.GetComponent<TMPro.TMP_Text>().fontStyle = TMPro.FontStyles.Normal;
        TextBox.GetComponent<TMPro.TMP_Text>().fontSize = 35;
        textToSpeak = "I hear them coming. Good luck, Warrior — and stay alive.";
        TextBox.GetComponent<TMPro.TMP_Text>().text = textToSpeak;
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
    }
}
