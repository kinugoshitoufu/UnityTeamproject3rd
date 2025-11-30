using UnityEngine;
using System.Collections;
using System;

public class TornadoScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.0f;

    
    private bool init = false;//初期化が完了したかどうか？
    private bool moveFlag = false;

    private Vector2 growSpeed = new Vector2(0, 0);//竜巻が大きくなるスピード
    private Vector2 maxSize;//竜巻の最大サイズ
    private Vector2 targetPos;//竜巻が目指す地点

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!init) return;

        //竜巻が画面外に出たら
        if (Mathf.Abs(transform.position.x) >= Mathf.Abs(targetPos.x))
        {
            Debug.Log("竜巻を消します");
            //Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (!init) return;
        Grow();
        Move1();
    }

    //竜巻を大きくしていく
    void Grow()
    {
        transform.localScale += new Vector3(growSpeed.x/50, growSpeed.y/50, 0);
    }

    //竜巻を一直線に画面端に移動させる
    void Move1()
    {
        if (!moveFlag) return;
        transform.position = Vector3.MoveTowards(transform.position, new Vector2(targetPos.x,transform.position.y), moveSpeed*Time.deltaTime);
    }


    //竜巻生成時の初期化
    public IEnumerator Init(float growSeconds,int direction)
    {
        //最大サイズを記録する
        maxSize = transform.localScale;
        //進行方向から竜巻の到達先を設定(画面右端か左端か)
        targetPos = (direction > 0) ? Camera.main.ViewportToWorldPoint(Vector2.one) : Camera.main.ViewportToWorldPoint(Vector2.zero);
        targetPos = new Vector3(targetPos.x+direction * (transform.lossyScale.x*4), 0,0);
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
    public IEnumerator FiringTornado1()
    {
        moveFlag = true;
        //竜巻が画面外に行くまで待機
        yield return new WaitUntil(() => Mathf.Abs(transform.position.x) >= Mathf.Abs(targetPos.x));
        Destroy(gameObject);
    }




}
