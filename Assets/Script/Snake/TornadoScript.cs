using UnityEngine;
using System.Collections;
using System;

public class TornadoScript : MonoBehaviour
{
    [Header("竜巻パラメータ(ボス側から設定してたら、そちらが優先)")]
    [SerializeField] private float moveSpeed = 2.0f;
    [Header("放物線の高さ")]
    [SerializeField] private float toranadoHeight=0f;
    [SerializeField] private Transform cloneHeight;

    private bool init = false;//初期化が完了したかどうか？

    private Vector2 growSpeed = new Vector2(0, 0);//竜巻が大きくなるスピード
    private Vector2 maxSize;//竜巻の最大サイズ
    private Vector2 startPos;//竜巻の開始地点
    private Vector2 targetPos;//竜巻が目指す地点

    private const int FIEXD_DURATION = 50;

    void FixedUpdate()
    {
        if (!init) return;
        Grow();
    }

    //竜巻を大きくしていく
    void Grow()
    {
        transform.localScale += new Vector3(growSpeed.x/ FIEXD_DURATION, growSpeed.y/ FIEXD_DURATION, 0);
    }

    //竜巻生成時の初期化
    public IEnumerator Init(float growSeconds,int direction,float speed=0,float height=0)
    {
        
        //最大サイズを記録する
        maxSize = transform.localScale;
        //開始地点を記録
        startPos = transform.position;
        //ボス側から速度が指定された場合はそちらを優先する
        if (speed != 0) moveSpeed = speed;
        //進行方向から竜巻の到達先を設定(画面右端か左端か)
        targetPos = (direction > 0) ? Camera.main.ViewportToWorldPoint(Vector2.one) : Camera.main.ViewportToWorldPoint(Vector2.zero);
        targetPos = new Vector3(targetPos.x+direction * (transform.lossyScale.x*4), transform.position.y,0);
        //放物線の高さ(設定されてない場合、分身一体分の高さを採用)
        toranadoHeight = height;
        if (toranadoHeight == 0&&cloneHeight!=null) toranadoHeight = cloneHeight.lossyScale.y;
        //サイズを初期化する
        transform.localScale = new Vector3(0, 0, 0);
        //大きくなるサイズを計測する
        growSpeed.x = maxSize.x / growSeconds;
        growSpeed.y = maxSize.y / growSeconds;
        //初期化を完了させる
        init = true;

        

        //攻撃準備が終わったら
        yield return new WaitForSeconds(growSeconds);
        //竜巻の成長を止める
        growSpeed = new Vector2(0, 0);
        transform.localScale = maxSize;

        Debug.Log("最大サイズになりました");
        
    }


    //竜巻発射
    public IEnumerator FiringTornado1(float speed=0)
    {
        //距離から目標地点に到達するまでの時間を計算
        float distance = Vector2.Distance(startPos, targetPos);
        float time = distance / moveSpeed;
        float t = 0;

        //目標地に到達するまで移動させる
        while (t < 1)
        {
            t += Time.deltaTime / time;
            transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        //画面外に出たら削除する
        Destroy(gameObject);
    }


    //竜巻2発射
    public IEnumerator FiringTornado2(float height = 0)
    {
        //距離から目標地点に到達するまでの時間を計算
        float distance = Vector2.Distance(startPos,targetPos);
        float time = distance / moveSpeed;
        float t = 0;
        //放物線の山の高さが設定されてない場合
        if (height == 0) height = toranadoHeight;

        //目標地に到達するまで移動させる
        while (t < 1)
        {
            //放物線の高さを目標地点までの進行度で決める
            t += Time.deltaTime / time;
            float parabolicY = 4 * height * t * (1 - t);//(0・1、始点と終点が最も低く0.5の中間地点が最高点）

            //放物線の高さ(縦の上がり下がりの値)を直線移動に足す
            transform.position = Vector2.Lerp(startPos, targetPos, t) + Vector2.up * parabolicY;
            yield return null;
        }
        //画面外に出たら削除する
        Destroy(gameObject);
    }


}
