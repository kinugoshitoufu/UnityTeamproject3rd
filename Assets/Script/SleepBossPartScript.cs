using System.Collections;
using UnityEngine;

public class SleepBossPartScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private CircleCollider2D hitCircle;
    private SpriteRenderer sprRender;

    void Start()
    {
        hitCircle = GetComponent<CircleCollider2D>();
        sprRender = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator FadeOut()
    {
        float num = 0;
        //“§–¾“x‚ª0‚É‚È‚é‚Ü‚Å­‚µ‚¸‚ÂŒ¸‚ç‚·
        while (sprRender.color.a > 0)
        {
            num += Time.deltaTime;
            var colorNum=sprRender.color;
            colorNum.a = Mathf.Lerp(colorNum.a,0, num);
            sprRender.color = colorNum;
            yield return null;
        }

        Debug.Log("Š®—¹‚µ‚Ü‚µ‚½AƒtƒF[ƒhƒAƒEƒg‚ª");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //’e‚ÉG‚ê‚½‚ç
        if (collision.gameObject.CompareTag("Bullet"))
        {
            //”»’è‚ğÁ‚·
            hitCircle.enabled = false;
            transform.parent.GetComponent<SleepBossScript>().Count();

            Debug.Log("•”ˆÊ‚ª”j‰ó‚³‚ê‚Ü‚µ‚½");
            StartCoroutine(FadeOut());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //’e‚ÉG‚ê‚½‚ç
        if (collision.gameObject.CompareTag("Bullet"))
        {
            //”»’è‚ğÁ‚·
            hitCircle.enabled = false;
            transform.parent.GetComponent<SleepBossScript>().Count();

            Debug.Log("•”ˆÊ‚ª”j‰ó‚³‚ê‚Ü‚µ‚½");
            StartCoroutine(FadeOut());
        }
    }
}
