using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class SleepBossScript : MonoBehaviour
{
    //[SerializeField] private CircleCollider2D hitCircle;//当たり判定
    [SerializeField] private GameObject Bg_InFront;//背景手前のやつ
    [SerializeField] private Light2D stageLight;//ステージ全体のライト
    [SerializeField] private List<Light2D> charcterLight = new List<Light2D>();//プレイヤーやボスを照らすライト
    //[SerializeField] private GameObject Bg_Black;// 黒背景
    [SerializeField] private bool fadeOut = false;
    [SerializeField] private int childCount = 2;//子オブジェクトの数
    [SerializeField] private float fadeSpeed = 1.0f;
    [SerializeField] private float showTime = 1.0f;

    private SpriteRenderer sprRender;
    private int DestroyCount=0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //背景手前を非表示
        if(Bg_InFront!=null)Bg_InFront.gameObject.SetActive(false);
        //sprRender=Bg_Black.GetComponent<SpriteRenderer>();
    }

    void State()
    {
        //switch()
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


    public IEnumerator NextStep()
    {
        //部位が破壊されたら、暗転
        //Bg_Black.GetComponent<SpriteRenderer>().sortingOrder = PlayerScript.instance.GetComponent<SpriteRenderer>().sortingOrder + 2;
        //if (fadeOut) yield return StartCoroutine(FadeIn());
        Debug.Log("NextStopが呼び出されています");

        if (fadeOut) yield return StartCoroutine(LightToDark());

        //プレイヤーが動けないようにする
        PlayerScript.instance.StopPlayer(true);
        PlayerScript.instance.ChangeSetPosition(new Vector3(-5,-2.86f,0));
        Debug.Log("プレイヤーの位置を変更");

        //寝ているボスを非表示にする
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);

        //暗転を後ろにする
        //Bg_Black.GetComponent<SpriteRenderer>().sortingOrder = 3;//仮置き(エレファントのオーダーに合わせる)

        //プレイヤーと立ったボスを見せる時間
        yield return new WaitForSeconds(showTime);

        //明るくなる
        if (fadeOut) yield return StartCoroutine(LightToBright());
        else stageLight.intensity = 1f;

        //if (fadeOut) yield return StartCoroutine(FadeOut());
        //else Bg_Black.SetActive(false);

        //動けるようにする
        PlayerScript.instance.StopPlayer(false);


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
            StartCoroutine(NextStep());
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

        Debug.Log("完了しました、フェードインが");
    }

    //画面を暗くする
    IEnumerator LightToDark()
    {
        float num = 0;
        stageLight.intensity = 1;//明るさを初期化

        //透明度が0になるまで少しずつ減らす
        while (stageLight.intensity > 0.05f)
        {
            num += Time.deltaTime / fadeSpeed;
            stageLight.intensity= Mathf.Lerp(stageLight.intensity, 0.05f, num);
            yield return null;
        }

        Debug.Log("画面を暗くする");
    }

    //画面を明るくする
    IEnumerator LightToBright()
    {
        float num = 0;

        stageLight.intensity = 0.05f;

        //透明度が0になるまで少しずつ減らす
        while (stageLight.intensity < 1)
        {
            num += Time.deltaTime / fadeSpeed;
            stageLight.intensity = Mathf.Lerp(stageLight.intensity, 1, num);
            yield return null;
        }

        Debug.Log("画面を明るくしました");
    }

    IEnumerator FadeOut()
    {
        float num = 0;
        //透明度が0になるまで少しずつ減らす
        while (sprRender.color.a > 0)
        {
            num += Time.deltaTime/fadeSpeed;
            var colorNum = sprRender.color;
            colorNum.a = Mathf.Lerp(colorNum.a, 0, num);
            sprRender.color = colorNum;
            yield return null;
        }

        Debug.Log("完了しました、フェードアウトが");
    }


    enum Name
    {
        PlayerLight,
        BossLight,
    }


    //IEnumerator Fade(int target)
    //{
    //    float num = 0;
    //}


    //enum State
    //{
        

    //}

}
