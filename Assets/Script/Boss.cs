using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float hp = 100;//HP
    public int waitFrameCount = 9;//待機するフレームのカウンター
    public Rigidbody2D rb;

    private bool waitComplete = false;//最初の待機

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Wait());
        if (!waitComplete) return;
        Debug.Log("待機処理は終了しました");
    }

    //最初に呼び出される待機処理
    IEnumerator Wait()
    {
        //指定したフレーム分、待機
        for(int i = 0; i < waitFrameCount; i++)
        {
            yield return null;
        }
        waitComplete = true;
    }

}

    