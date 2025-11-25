using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float HP = 100;//HP
    public int waitFrameCount = 9;//待機するフレームのカウンター
    public Rigidbody2D rb;

    protected bool waitComplete = false;//最初の待機

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        StartCoroutine(Wait());//待機処理を呼び出す
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!waitComplete) return;
    }

    //最初に呼び出される待機処理
    IEnumerator Wait()
    {
        Debug.Log(waitFrameCount + "フレームの待機を開始します。");
        //指定したフレーム分、待機
        for(int i = 0; i < waitFrameCount; i++)
        {
            yield return null;
        }
        waitComplete = true;
        Debug.Log("待機処理が完了しました。");
    }

}

    