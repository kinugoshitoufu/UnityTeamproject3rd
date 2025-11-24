using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;

public class StageSelectScript : MonoBehaviour
{

    public List<sceneClass> scenes = new List<sceneClass>();
    public GameObject cursor;
    public float firstWaitTime = 0.6f;//長押し時、最初の一回カーソルが止まる時間
    public float waitTime = 0.1f;//カーソルが待機する時間
    public float deadZone = 0.2f;//コントローラーでどこから入力と見なすか

    private int nowPoint = 0;//カーソルが現在示している場所
    private int beforePoint = 0;//カーソルが以前示していた場所
    private float inputNum;
    private float inputTime = 0;//入力時間(長押しか単発押しかの計測で使用)
    private bool inputMode = true;//入力を受け付けるかどうか
    private bool longPress = false;//長押しかどうか？
    private bool firstFlag = false;//
    private bool waitCompleteFalg = false;
    private Coroutine WaitCoroutine;
    private Vector2 cursorPos;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1"))
        {
            if (scenes[nowPoint].sceneName == "")
            {
                Debug.Log("シーンの設定がされていません");
                return;
            }
            SceneManager.LoadScene(scenes[nowPoint].sceneName);
        }

        // 入力を取る
        inputNum = Input.GetAxisRaw("Horizontal");

        // 長押し時間を計測
        inputTime += Time.deltaTime;

        // 入力されていない場合
        if (Mathf.Abs(inputNum) <= deadZone)
        {
            inputTime = 0;
            longPress = false; // 長押しフラグをリセット
            firstFlag = false;//初回止めフラグをリセット
            //単発押しの手触り確保
            if (waitCompleteFalg)
            {
                inputMode = true;
                StopCoroutine(WaitCoroutine);
            }
        }

        // 長押し判定(最初の一回だけピタっと止めるフラグを立てる)
        if (inputTime >= waitTime && firstFlag == false)
        {
            longPress = true; // 長押しフラグを立てる
            firstFlag = true;
        }

        // 入力モードが無効なら何もしない
        if (!inputMode) return;

        // 左右にカーソル移動
        if (inputNum > deadZone) nowPoint++;
        else if (inputNum < -deadZone) nowPoint--;

        // 左右端を超えたら戻る
        if (nowPoint >= scenes.Count) nowPoint = 0;
        else if (nowPoint < 0) nowPoint = scenes.Count - 1;

        // カーソルの座標更新
        if (nowPoint != beforePoint)
        {
            cursorPos = scenes[nowPoint].sprite.transform.position;
            cursor.transform.position = cursorPos;

            // カーソルの移動後、入力可能にする
            WaitCoroutine = StartCoroutine(SetInputMode());
            beforePoint = nowPoint;
        }



    }

    IEnumerator SetInputMode()
    {
        //カーソルが到達したら入力出来ない時間を取る
        inputMode = false;
        waitCompleteFalg = false;

        yield return new WaitForSeconds(waitTime);
        waitCompleteFalg = true;

        //長押しの場合は最初の一回だけピタッと止める時間を作る
        if (longPress) yield return new WaitForSeconds(firstWaitTime - waitTime);

        //入力可能にする
        inputMode = true;
        inputNum = 0;
        longPress = false;
    }

}

[System.Serializable]
//シーンの画像と移動先を紐づける
public class sceneClass
{
    public GameObject sprite = null;
    public string sceneName = "";
}

