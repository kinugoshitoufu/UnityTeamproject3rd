using System.Collections;
using UnityEngine;

public class SleepBossScript : MonoBehaviour
{
    //[SerializeField] private CircleCollider2D hitCircle;//当たり判定
    [SerializeField] private GameObject Bg_InFront;//背景手前のやつ
    [SerializeField] private GameObject Bg_Black;// 黒背景
    [SerializeField] private int childCount = 2;//子オブジェクトの数
    [SerializeField] private bool fadeOut = false;
    [SerializeField] private float fadeSpeed = 1.0f;

    private SpriteRenderer sprRender;
    private int DestroyCount=0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //背景手前を非表示
        if(Bg_InFront!=null)Bg_InFront.gameObject.SetActive(false);
        sprRender=Bg_Black.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //垂幕が上がったら、ボスを表示する
        if (PlayerScript.instance.StartFlag)
        {

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }


    //削除カウントを増やす
    public void Count()
    {
        //削除カウントを増やす
        DestroyCount++;

        //全ての部位が破壊されたら
        if (DestroyCount == childCount)
        {
            Debug.Log("暗転をする処理を今から書こう");
            if (Bg_Black != null)
            {
                if (fadeOut) StartCoroutine(FadeIn());
                Bg_Black.GetComponent<SpriteRenderer>().sortingOrder = PlayerScript.instance.GetComponent<SpriteRenderer>().sortingOrder + 2;
            }

        }
    }

    IEnumerator FadeIn()
    {
        float num = 0;

        var Startcolor = sprRender.color;
        Startcolor.a = 0;
        sprRender.color = Startcolor;

        //透明度が0になるまで少しずつ減らす
        while (sprRender.color.a < 1)
        {
            num += Time.deltaTime/fadeSpeed;
            var colorNum = sprRender.color;
            colorNum.a = Mathf.Lerp(colorNum.a, 1, num);
            sprRender.color = colorNum;
            yield return null;
        }

        Debug.Log("完了しました、フェードアウトが");
    }

}
