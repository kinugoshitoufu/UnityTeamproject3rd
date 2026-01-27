using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class SleepBossScript : MonoBehaviour
{
    //[SerializeField] private CircleCollider2D hitCircle;//当たり判定
    [SerializeField] private int childCount = 2;//子オブジェクトの数
    private int DestroyCount=0;

    //削除カウントを増やす
    public void Count()
    {
        //削除カウントを増やす
        DestroyCount++;

        //全ての部位が破壊されたら
        if (DestroyCount == childCount)
        {
            Debug.Log("暗転をする処理を今から書こう");
            StartCoroutine(ScreenManager.instance.NextStep());
            //StartCoroutine(NextStep());
        }
    }

}
