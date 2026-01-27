using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SleepBossPartScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Light2D stageLight;//ステージ全体のライト
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
        //透明度が0になるまで少しずつ減らす
        while (sprRender.color.a > 0)
        {
            num += Time.deltaTime;
            var colorNum=sprRender.color;
            colorNum.a = Mathf.Lerp(colorNum.a,0, num);
            sprRender.color = colorNum;
            yield return null;
        }

        Debug.Log("完了しました、フェードアウトが");
    }

    //画面を暗くする
    IEnumerator LightToDark()
    {
        float num = 0;
        stageLight.intensity = 1;//明るさを初期化

        //透明度が0になるまで少しずつ減らす
        while (stageLight.intensity > 0.05)
        {
            num += Time.deltaTime;
            stageLight.intensity = Mathf.Lerp(stageLight.intensity, 0.05f, num);
            yield return null;
        }

        Debug.Log("画面を暗くする");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //弾に触れたら
        if (collision.gameObject.CompareTag("Bullet"))
        {
            //判定を消す
            hitCircle.enabled = false;
            transform.parent.GetComponent<SleepBossScript>().Count();

            Debug.Log("部位が破壊されました");
            StartCoroutine(FadeOut());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //弾に触れたら
        if (collision.gameObject.CompareTag("Bullet"))
        {
            //判定を消す
            hitCircle.enabled = false;
            transform.parent.GetComponent<SleepBossScript>().Count();

            Debug.Log("部位が破壊されました");
            StartCoroutine(FadeOut());
        }
    }
}
