using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using static System.TimeZoneInfo;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager instance;
    public BannerAnimation[] banners=new BannerAnimation[2];

    //垂幕オープン～ライトアップまでに必要なもの
    [Header("垂幕オープン～ライトアップ")]
    [SerializeField] private GameObject Bg_InFront;//背景手前のやつ
    [SerializeField] private GameObject Bg_Black;// 黒背景
    [SerializeField] private GameObject Banner_InFront;//手前の垂幕
    [SerializeField] private Light2D stageLight;//ステージ全体のライト
    [SerializeField] private List<Light2D> charcterLight = new List<Light2D>();//プレイヤーやボスを照らすライト
    [SerializeField] private GameObject SleepBoss;//寝ているボス
    [SerializeField] private Vector3 setPosition = new Vector3(-5, -2.86f, 0);//ライトが当たるポジション

    [Header("フェード・ライトの時間設定")]
    [SerializeField] private bool fadeOut = false;
    [SerializeField] private float fadeSpeed = 1.0f;
    [SerializeField] private float showTime = 1.0f;

    //垂幕クローズ、リザルト表示までに必要なもの
    [Header("垂幕クローズ～リザルト表示")]
    [SerializeField] private Image result;//リザルト画面のUI
    [SerializeField] private GameObject smoke;//煙幕
    [SerializeField] private GameObject boss;//ボスの画像
    [SerializeField] private Image panel;//リザルトが見やすくなるような黒背景
    [SerializeField] private GameObject fadeBg;// 黒背景
    [SerializeField] private Vector3 floorPos = new Vector3(-5, -2.86f, 0);
    [SerializeField] private Vector3 bossPos = new Vector3(5, 3f, 0);
    [SerializeField] private Camera mainCamera;//メインカメラ
    [SerializeField] private Camera zoomCamra;//ズームカメラ
    [SerializeField] private float transitionTime = 0.5f;
    [SerializeField] private float targetScale = 0.5f;//スローモーションの速さ


    private SpriteRenderer sprRender;
    private bool openFlag = false;//幕が上がっているかどうか?

    //カメラのサイズの初期値
    private Vector3 startPos;
    private float startSize;

    public bool StartLecoding { get { return startLecoding; } }
    private bool startLecoding = false;
    private bool checkInputStart = false;
    private bool completeInput = false;

    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //カメラの初期値を保存しておく
        startPos = mainCamera.transform.position;
        startSize = mainCamera.orthographicSize;

        //背景手前を非表示
        if (Bg_InFront != null) Bg_InFront.gameObject.SetActive(false);
        //ライトを消す
        foreach (var light in charcterLight) light.intensity = 0;
        //ボスを消す
        SleepBoss.gameObject.SetActive(false);

        sprRender=fadeBg.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Time.timeScale = targetScale;
        //Time.fixedDeltaTime = 0.02f * Time.timeScale;

        //幕が上がったら、寝ているボスを表示する
        if (PlayerScript.instance.StartFlag==true)
        {
            if (openFlag) return;
            //SleepBoss.gameObject.SetActive(true);
            openFlag = true;
            foreach (var banner in banners) banner.Open();
            //banner.Open();
        }

        WaitUntilInput();


        //if (PlayerScript.instance.deadFlag)
        //{
        //    //if (PlayerScript.instance.StartFlag == false) return;
        //    Debug.Log("プレイヤーの死亡を確認。世界の幕が閉じます");
        //    foreach (var banner in banners) banner.Close();
        //    openFlag = false;
        //    //banner.Close();
        //}
    }


    public IEnumerator Clear()
    {
        //スローモーションを開始
        //StartCoroutine(changeSlow(targetScale));

        //ボスを倒したらカメラをzoomさせる
        //yield return StartCoroutine(ZoomCamera(mainCamera, zoomCamra));

        StartCoroutine(ZoomCamera(mainCamera, zoomCamra));
        StartCoroutine(RotateBoss(Elephant.elephant.gameObject, -90f, transitionTime));

        //yield return CoroutineRunner.WaitAll(ZoomCamera(mainCamera, zoomCamra),RotateBoss(Elephant.elephant.gameObject,-90f,transitionTime));

        yield return new WaitForSeconds(transitionTime);

        Debug.Log("zoom終了です。リザルト表示に移行していきます");

        //カメラを戻す
        mainCamera.transform.position = startPos;
        mainCamera.orthographicSize = startSize;

        StartCoroutine(Close());
        



    }

    public IEnumerator Close()
    {
        //既に垂幕が閉じている場合は、呼び出さない
        if (!openFlag) yield break;
        Debug.Log("プレイヤーの死亡を確認。世界の幕が閉じます");

        //垂幕を閉じる
        foreach (var banner in banners) banner.Close();
        //煙幕を中央に出現
        Instantiate(smoke,floorPos, Quaternion.identity);
        Instantiate(boss, bossPos, Quaternion.identity);
        //リザルトを降ろす
        StartCoroutine(result.GetComponent<UIFloatingMove>().FlutterMoveSmoothCoroutine(new Vector2(-50, 0f), 2f, 10f, 2f));
        panel.gameObject.SetActive(true);
        openFlag = false;

        Debug.Log("パネルはtrueになりました");

        //何かのキーが押されるまで待機
        checkInputStart = true;

    }

    void WaitUntilInput()
    {
        if (checkInputStart == false) return;
        if (Input.anyKey)
        {
            completeInput = true;
            checkInputStart = false;
            Debug.Log("キーが押されました");
            StartCoroutine(SceneChange());
        }
        
    }

    IEnumerator SceneChange()
    {
        result.gameObject.SetActive(false);
        fadeBg.gameObject.SetActive(true);
        sprRender.sortingOrder = 8;
        yield return FadeIn();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        PlayerScript.instance.ChangeSetPosition(setPosition);
        
        //寝ているボスを非表示にする
        SleepBoss.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);

        //後ろの暗転を消す
        Bg_Black.gameObject.SetActive(false);
        Banner_InFront.gameObject.SetActive(false);

        //ライトをつける
        foreach (var light in charcterLight) light.intensity = 1;

        //プレイヤーと立ったボスを見せる時間
        yield return new WaitForSeconds(showTime*0.3f);

        //動けるようにする
        PlayerScript.instance.StopPlayer(false);

        //プレイヤーの録画開始をONにする
        startLecoding = true;

        //画面全体が明るくなるまで待機
        yield return new WaitForSeconds(showTime);

        //明るくなる
        Banner_InFront.gameObject.SetActive(true);
        if (fadeOut) yield return StartCoroutine(LightToBright(stageLight));
        else stageLight.intensity = 1f;

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

    IEnumerator ZoomCamera(Camera start,Camera end)
    {
        //始点カメラと終点カメラから取得
        float  timer = 0;
        Vector3 startPos = start.transform.position;
        Vector3 endPos = end.transform.position;
        float startSize = start.orthographicSize;
        float endSize = end.orthographicSize;

        bool isZooming = true;

        while (isZooming)
        {
            timer += Time.unscaledDeltaTime; // スローでも一定速度
            float t = Mathf.Clamp01(timer / transitionTime);

            // 位置補間
            mainCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
            // サイズ補間（2Dの場合はorthographicSizeでズーム）
            mainCamera.orthographicSize = Mathf.Lerp(startSize, endSize, t);

            if (t >= 1f)
            {
                isZooming = false;
            }

            yield return null;
        }
        
    }

    IEnumerator RotateBoss(GameObject rotateObj,float targetAngle,float rotateSeconds)
    {
        float rotateSpeed = targetAngle / rotateSeconds;

        while (rotateObj.transform.eulerAngles.z != targetAngle)
        {
            // 現在の回転
            float currentAngle = rotateObj.transform.eulerAngles.z;

            // 角度をなめらかに近づける
            float newAngle = Mathf.MoveTowardsAngle(
                currentAngle,
                targetAngle,
                Mathf.Abs(rotateSpeed) * Time.deltaTime
            );

            rotateObj.transform.rotation = Quaternion.Euler(0f, 0f, newAngle);

            if (rotateObj.transform.rotation.z == targetAngle) yield break;

            yield return null;
        }
        

    }


    IEnumerator changeSlow(float targetScale)
    {
        Time.timeScale = targetScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        yield return null;

        //while (Time.timeScale!=targetScale)
        //{
        //    //スローモーションにグラデーションを持たせる
        //    Time.timeScale = Mathf.Lerp(Time.timeScale, targetScale, 10 * Time.unscaledDeltaTime);
        //    //物理演算がカクつかないように周期の倍率を合わせる
        //    Time.fixedDeltaTime = 0.02f * Time.timeScale;

        //    yield return null;
        //}
        
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
