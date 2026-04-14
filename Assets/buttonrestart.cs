using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonrestart : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(LoadBaseLevel1);
        }
    }

    void LoadBaseLevel1()
    {
        SceneManager.LoadScene("Baselevel1");
    }
}
