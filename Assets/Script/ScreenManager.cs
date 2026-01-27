using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager instance;

    [SerializeField] private GameObject Bg_InFront;//背景手前のやつ
    [SerializeField] private GameObject Bg_Black;// 黒背景
    [SerializeField] private Light2D stageLight;//ステージ全体のライト
    [SerializeField] private List<Light2D> charcterLight = new List<Light2D>();//プレイヤーやボスを照らすライト
    [SerializeField] private GameObject SleepBoss;//寝ているボス

    [SerializeField] private bool fadeOut = false;
    [SerializeField] private float fadeSpeed = 1.0f;
    [SerializeField] private float showTime = 1.0f;

    private SpriteRenderer sprRender;
    private bool openFlag = false;//幕が上がっているかどうか?
    public bool StartLecoding { get { return startLecoding; } }
    private bool startLecoding = false;

    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //背景手前を非表示
        if (Bg_InFront != null) Bg_InFront.gameObject.SetActive(false);
        //ライトを消す
        foreach (var light in charcterLight) light.intensity = 0;
        //ボスを消す
        SleepBoss.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //幕が上がったら、寝ているボスを表示する
        if (PlayerScript.instance.StartFlag==true)
        {
            if (openFlag) return;
            SleepBoss.gameObject.SetActive(true);
            openFlag = true;
        }
    }

    public void ShowSleepBoss()
    {
        SleepBoss.gameObject.SetActive(true);
        openFlag = true;
    }

    public IEnumerator NextStep()
    {
        //部位が破壊されたら、暗転
        if (fadeOut) yield return StartCoroutine(LightToDark(stageLight, 0));

        //プレイヤーが動けないようにする
        PlayerScript.instance.StopPlayer(true);
        //プレイヤーの位置変更
        PlayerScript.instance.ChangeSetPosition(new Vector3(-5, -2.86f, 0));
        
        //寝ているボスを非表示にする
        SleepBoss.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);

        //後ろの暗転を消す
        Bg_Black.gameObject.SetActive(false);

        //プレイヤーの録画開始をONにする
        startLecoding = true;

        //ライトをつける
        foreach (var light in charcterLight) light.intensity = 1;

        //プレイヤーと立ったボスを見せる時間
        yield return new WaitForSeconds(showTime);

        //明るくなる
        if (fadeOut) yield return StartCoroutine(LightToBright(stageLight));
        else stageLight.intensity = 1f;

        //動けるようにする
        PlayerScript.instance.StopPlayer(false);

    }

    //画面を暗くする
    IEnumerator LightToDark(Light2D light, float darkNum = 0.05f)
    {
        float num = 0;
        light.intensity = 1;//明るさを初期化

        //透明度が0になるまで少しずつ減らす
        while (light.intensity > darkNum)
        {
            num += Time.deltaTime / fadeSpeed;
            light.intensity = Mathf.Lerp(light.intensity, darkNum, num);
            yield return null;
        }

        Debug.Log("画面を暗くする");
    }

    //画面を明るくする
    IEnumerator LightToBright(Light2D light)
    {
        float num = 0;

        light.intensity = 0.05f;

        //透明度が0になるまで少しずつ減らす
        while (light.intensity < 1)
        {
            num += Time.deltaTime / fadeSpeed;
            light.intensity = Mathf.Lerp(light.intensity, 1, num);
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
            num += Time.deltaTime / fadeSpeed;
            var colorNum = sprRender.color;
            colorNum.a = Mathf.Lerp(colorNum.a, 0, num);
            sprRender.color = colorNum;
            yield return null;
        }

        Debug.Log("完了しました、フェードアウトが");
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
            num += Time.deltaTime / fadeSpeed;
            var colorNum = sprRender.color;
            colorNum.a = Mathf.Lerp(colorNum.a, 1, num);
            sprRender.color = colorNum;
            yield return null;
        }

        Debug.Log("完了しました、フェードインが");
    }
}
