using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{

    #region Class References
    AudioSource audioSource;
    #endregion

    #region Private Fields
    [Header("UI Screens")]
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject menuScreen;

    
    #endregion

    #region Properties

    #endregion

    #region Start Up
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        EnableStartScreen();

        audioSource.Play();
    }
    #endregion

    #region Class Methods
    private void EnableStartScreen()
    {
        startScreen.SetActive(true);
        menuScreen.SetActive(false);
    }
    private void EnableMenuScreen()
    {
        startScreen.SetActive(false);
        menuScreen.SetActive(true);
    }
    #endregion

    #region Buttons
    public void Button_Start()
    {
        EnableMenuScreen();
    }

    public void Button_Play()
    {
        SceneManager.LoadScene(1);
    }
    #endregion
}
