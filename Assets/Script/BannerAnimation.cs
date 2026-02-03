using UnityEngine;
using System.Collections;
using NUnit.Framework;

public class BannerAnimation : MonoBehaviour
{
    [SerializeField]private float openSeconds = 3.0f;
    [SerializeField] private float closeSeconds = 1.0f;
    [SerializeField]private Transform banner;
    [SerializeField] private Vector3 openPosition;

    private Animator animator;
    private bool openFlag = false;
    private bool eventFlag = false;

    void Start()
    {
        animator=GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーがゲーム開始した場合
        if (PlayerScript.instance.StartFlag == true&&!openFlag)
        {
            Debug.Log("垂幕を上げるぜ");
            Open();
        }


        if (PlayerScript.instance.deadFlag&&openFlag)
        {
            Close();
            Debug.Log("プレイヤーの死亡を確認。垂幕を閉じます");
        }

    }

    public void Open()
    {
        //アニメーションを開始させる
        animator.SetBool("OpenFlag", true);
        //垂幕を移動させる
        StartCoroutine(Move(banner, openPosition, openSeconds, Easing.Ease.OutQuart, false));
        //開幕させたことにする
        openFlag = true;
        //ボスを表示させる
        ScreenManager.instance.ShowSleepBoss();
    }

    public void Close()
    {
        //アニメーションを開始させる
        animator.SetBool("OpenFlag", false);
        //垂幕を移動させる
        StartCoroutine(Move(banner, -openPosition, closeSeconds, Easing.Ease.OutQuart, false));

        //yield return new WaitForSeconds(closeSeconds*0.5f);
        //連続で呼ばれないようにする
        openFlag = false;
        PlayerScript.instance.StartFlag = false;
        //ボスを表示させる
        ScreenManager.instance.ShowSleepBoss();
    }


    public IEnumerator Move(Transform transform, Vector3 destinationPos, float seconds, Easing.Ease easing, bool absolute)
    {
        if (eventFlag) yield break;
        eventFlag = true;

        // イージング関数の取得
        var Ease = Easing.GetEasingMethod(easing);

        // 現在点と移動先の設定
        Vector3 staPos = transform.localPosition;
        Vector3 endPos = absolute ? destinationPos : staPos + destinationPos;
        // 初期地点と目標地点の差
        Vector3 difPos = endPos - staPos;

        // N秒かけて移動させる
        float e = 0;
        while (true)
        {
            yield return null;
            e += Time.deltaTime / seconds;
            if (e >= 1.0f)
            {
                transform.localPosition = endPos;
                break;
            }
            Vector3 nextPos = staPos + Ease(e) * difPos;
            transform.localPosition = nextPos;
        }

        eventFlag = false;
    }
}
