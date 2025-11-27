using UnityEngine;
using UnityEngine.SceneManagement;

public class TitlemenuScript : MonoBehaviour
{
    public UnityEngine.UI.Button button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.Select();
    }

    // Update is called once per frame
    void Update()
    {
       
       
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }
    public void StringArgFunction(string s)
    {
        SceneManager.LoadScene(s);
    }
}
