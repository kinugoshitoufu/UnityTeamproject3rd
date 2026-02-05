using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpCount : MonoBehaviour
{
    public GameObject[] image;
    public int temphp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        temphp = PlayerScript.instance.HpMax;
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
