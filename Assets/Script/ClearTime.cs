using TMPro;
using UnityEngine;

public class ClearTime : MonoBehaviour
{
    public TextMeshProUGUI Cleartime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       Cleartime.text = "ClearTime:" + PlayerScript.instance.ClearTime.ToString(); 
    }
}
