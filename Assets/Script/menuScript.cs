using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class menuScript : MonoBehaviour
{
    public bool menuopen = false;
    public UnityEngine.UI.Button button;
    public UnityEngine.UI.Button button1;
    public GameObject menuimage;
    public GameObject menubutton;
    public GameObject menubutton1;
    public static menuScript instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        menuopen = false;
        menuimage.SetActive(false);
        menubutton.SetActive(false);
        menubutton1.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (menuopen)
        {
            GameManager.Instance?.SetMenuState(true);
            Time.timeScale = 0.0f;
        }
        else if (!menuopen)
        {
            GameManager.Instance?.SetMenuState(false);
            Time.timeScale = 1.0f;
        }
        menuimage.SetActive(menuopen);
        menubutton.SetActive(menuopen);
        menubutton1.SetActive(menuopen);

        if (PlayerScript.instance.deadFlag == false)
        {
            if (Input.GetKeyDown("joystick button 7"))
            {
                button.Select();
                menuopen = !menuopen;
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                button.Select();
                menuopen = !menuopen;
            }
        }
    }
    public void BackGameScene()
    {
        Time.timeScale = 1.0f;
        menuopen = false;
    }
    public void StringArgFunction(string s)
    {
        Time.timeScale = 1.0f;
        menuopen = false;
        SceneManager.LoadScene(s);
    }
    public void BacktoTitle()
    {
        UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
