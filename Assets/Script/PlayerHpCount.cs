using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpCount : MonoBehaviour
{
    public GameObject[] image;
    private int temphp = PlayerScript.instance.Hp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        temphp = PlayerScript.instance.Hp;
    }

    // Update is called once per frame
    void Update()
    {
        int hp = PlayerScript.instance.Hp;
        if (hp < temphp)
        {
            image[temphp - 1].SetActive(false);
            temphp--;
        }


    }
}
