using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;

public class StageSelectScript : MonoBehaviour
{
   
    public List<sceneClass> scenes =new List<sceneClass>();
    public GameObject cursor;
    public float cursorSpeed = 0.3f;
    public float firstWaitTime = 0.3f;
    public float waitTime = 0.1f;//カーソルが待機する時間
    

    private int nowPoint = 0;//カーソルが現在示している場所
    private int beforePoint = 0;//カーソルが以前示していた場所
    private float num;
    private float beforeNum = 0;
    private float inputTime=0;//入力時間(長押しか単発押しかの計測で使用)
    private bool inputMode = true;//入力を受け付けるかどうか

    bool tanpatu = false;
    private Vector2 cursorPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (scenes[nowPoint].sceneName == "")
            {
                Debug.Log("シーンの設定がされていません");
                return;
            }
            SceneManager.LoadScene(scenes[nowPoint].sceneName);
        }

        //入力を取る
        num = Input.GetAxisRaw("Horizontal");

        //入力時間を入れる
        inputTime += Time.deltaTime;
        if (num == 0)
        {
            inputTime = 0;
            tanpatu = false;
        }

        if (inputTime >= waitTime / 2)
        {
            if (tanpatu) return;
            StartCoroutine(CursorStopper());
            tanpatu = true;
        }

        if (inputMode == false) return;

        //更新
        if (num > 0) nowPoint++;
        else if (num < 0) nowPoint--;

        //左右端を超えたら戻る
        if (nowPoint >= scenes.Count) nowPoint = 0;
        else if (nowPoint < 0) nowPoint = scenes.Count - 1;

        //カーソルの座標更新
        if (nowPoint != beforePoint)
        {
      
            cursorPos = scenes[nowPoint].sprite.transform.position;
            cursor.transform.position = cursorPos;
            //入力待機
            if(Mathf.Abs(beforeNum)==0&&tanpatu==false)StartCoroutine(SetInputMode(true));
            else StartCoroutine(SetInputMode());

            beforePoint = nowPoint;

        }

        beforeNum = num;

    }

    //カーソルの位置の更新
    IEnumerator CursorUpdate2()
    {
        inputMode = false;//入力を受け付けなくする
        Vector2 cursorPos2 = scenes[nowPoint].sprite.transform.position;//カーソルが次に行く座標を設定

        //カーソルを動かす
        while (Vector3.Distance(cursor.transform.position,cursorPos)>=0.1f){
            cursor.transform.position = Vector3.MoveTowards(cursor.transform.position, cursorPos, cursorSpeed);
            yield return null;
        }
        yield return new WaitForSeconds(waitTime);//カーソルが到達したら入力出来ない時間を取る
        //inputMode = true;
    }


    //カーソルの位置の更新
    void CursorUpdate()
    {
        //カーソルを動かす
        cursor.transform.position = Vector3.MoveTowards(cursor.transform.position, cursorPos, cursorSpeed);

        if (Vector3.Distance(cursor.transform.position, cursorPos) < 0.1f)
        {
            //StartCoroutine(SetInputMode());
            //beforePoint = nowPoint;
            //yield return new WaitForSeconds(waitTime);//カーソルが到達したら入力出来ない時間を取る
            inputMode = true;
            
        }

    }

    IEnumerator SetInputMode(bool first=false)
    {
        //カーソルが到達したら入力出来ない時間を取る
        inputMode = false;
        yield return new WaitForSeconds(waitTime);
        //最初はストッパーをかけておく
        //if (first) yield return new WaitForSeconds(firstWaitTime - waitTime);
        //入力可能にする
        inputMode = true;
    }

    IEnumerator CursorStopper()
    {
        //最初はストッパーをかけておく
        yield return new WaitForSeconds(firstWaitTime - waitTime);
        inputMode = true;
    }



}

[System.Serializable]
//シーンの画像と移動先を紐づける
public class sceneClass
{
    public GameObject sprite=null;
    public string sceneName="";
}




//else
//{
//    //StartCoroutine(CursorUpdate2());
//    CursorUpdate();
//    return;
//}

//if (Input.GetKeyDown(KeyCode.RightArrow))
//{
//    num = 1;
//}
//else if (Input.GetKeyDown(KeyCode.LeftArrow))
//{
//    num = -1;
//}
//else
//{
//    num = 0;
//}