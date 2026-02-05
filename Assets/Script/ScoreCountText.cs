using TMPro;
using UnityEngine;

public class ScoreCountText : MonoBehaviour
{
    public TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Score:" + PlayerScript.instance.Score.ToString();
    }
}
