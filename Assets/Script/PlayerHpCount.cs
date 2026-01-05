using TMPro;
using UnityEngine;

public class PlayerHpCount : MonoBehaviour
{
    public TextMeshProUGUI HpText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int hp = PlayerScript.instance.Hp;
        HpText.text = hp.ToString();
    }
}
