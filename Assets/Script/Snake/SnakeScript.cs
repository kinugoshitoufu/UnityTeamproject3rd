using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SnakeScript : Boss
{
    //public static SnakeScript SnakeInstance;
    [Header("攻撃パラメータ(待機、余韻など)")]
    public List<SnakeAttackParameters> attackParms = new List<SnakeAttackParameters>();

    [Header("竜巻関連(0ならプレファブの値を参照)")]
    [SerializeField] private TornadoScript tornadoPrefab;
    [SerializeField] private float torandoSpeed=0;
    [SerializeField] private float torandoHeight=0;
    [Header("竜巻1か2か(確認テスト用,true=1,false=2)")]
    public bool tornado1 = true;

    // ========== 参照用 ==========
    private float moveSpeed;//移動速度
    private bool eventFlag = false;//処理を行っているかどうか?
    private bool eventWaitComplete = false;//外部の処理の待機が完了したかどうか？(竜巻などに使う)
    private bool moveFlag = false;

    private Vector2 targetPos;
    private BoxCollider2D box;
    private SpriteRenderer spr;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spr=GetComponent<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
        //待機処理完了を待つ
        if (!waitComplete) return;


        base.Update();

        //HPで条件分岐
        if (ratioHP >= 0.5f)
        {
            //HP50%以上の処理
            if(!eventFlag)StartCoroutine(MoveAttack1());
            if (!eventFlag) StartCoroutine(Tornado1());
            if(!eventFlag) StartCoroutine(EvilStare());
        }
        else
        {
            //HP50%以下の処理
        }

       

    }

    private void FixedUpdate()
    {
        Move();
    }

    //移動攻撃1(低い突進)
    IEnumerator MoveAttack1()
    {
        //移動攻撃用のパラメータを取得
        SnakeAttackParameters moveParm = getParam(SnakeTechnique.MoveAttack1);

        //移動攻撃1の設定がされていない場合は終了
        if (moveParm == null) yield break;

        //処理を開始
        eventFlag = true;

        //攻撃準備が終わるまで待機
        yield return StartCoroutine(PreparaAttack(moveParm.proTime.preparationTime));

        //攻撃が終わるまで待機
        yield return StartCoroutine(OnMove(moveParm.proTime.attackTime));

        //攻撃後の待機余韻が終わるまで待機
        yield return StartCoroutine(Afterglow(moveParm.proTime.afterglowTime));

        //処理を終了
        //eventFlag = false;

        //テスト
        if(tornado1)StartCoroutine(Tornado1());
        else StartCoroutine(Tornado2());

    }

    //竜巻1
    IEnumerator Tornado1()
    {
        //竜巻1用のパラメータを取得
        SnakeAttackParameters tornado1Parm = getParam(SnakeTechnique.Tornado1);

        //竜巻1の設定がされていない場合は終了
        if (tornado1Parm == null) yield break;

        //処理を開始
        eventFlag = true;

        //竜巻の生成
        TornadoScript tornado = Instantiate(tornadoPrefab, this.transform.position + new Vector3(-Direction * (transform.lossyScale.x / 2), 0),Quaternion.identity);

        //攻撃準備が終わるまで待機
        yield return CoroutineRunner.WaitAll(PreparaAttack(tornado1Parm.proTime.preparationTime), tornado.Init(tornado1Parm.proTime.preparationTime, Direction,torandoSpeed,torandoHeight));

        //攻撃が終わるまで待機
        yield return CoroutineRunner.WaitAll(Attack(tornado1Parm.proTime.attackTime), tornado.FiringTornado1());

        //攻撃後の待機余韻が終わるまで待機
        yield return StartCoroutine(Afterglow(tornado1Parm.proTime.afterglowTime));

        //処理を終了
        //eventFlag = false;

        //テスト
        StartCoroutine(EvilStare());

    }

    //竜巻2
    IEnumerator Tornado2()
    {
        //竜巻2用のパラメータを取得
        SnakeAttackParameters tornado2Parm = getParam(SnakeTechnique.Tornado2);

        //竜巻2の設定がされていない場合は終了
        if (tornado2Parm == null) yield break;

        //処理を開始
        eventFlag = true;

        //竜巻の生成
        TornadoScript tornado = Instantiate(tornadoPrefab, this.transform.position + new Vector3(-Direction * (transform.lossyScale.x / 2), 0), Quaternion.identity);

        //攻撃準備が終わるまで待機
        yield return CoroutineRunner.WaitAll(PreparaAttack(tornado2Parm.proTime.preparationTime), tornado.Init(tornado2Parm.proTime.preparationTime, Direction,torandoSpeed,torandoHeight));

        //攻撃が終わるまで待機
        yield return CoroutineRunner.WaitAll(Attack(tornado2Parm.proTime.attackTime), tornado.FiringTornado2());

        //攻撃後の待機余韻が終わるまで待機
        yield return StartCoroutine(Afterglow(tornado2Parm.proTime.afterglowTime));

        //処理を終了
        //eventFlag = false;

        //テスト
        StartCoroutine(EvilStare());
    }

    //蛇睨み
    IEnumerator EvilStare()
    {
        
        //蛇睨み用のパラメータを取得
        SnakeAttackParameters evilParm = getParam(SnakeTechnique.EvilStare);

        //竜巻1の設定がされていない場合は終了
        if (evilParm == null) yield break;
        
        //処理を開始
        eventFlag = true;

        //攻撃準備が終わるまで待機
        yield return StartCoroutine(PreparaAttack(evilParm.proTime.preparationTime));

        //攻撃が終わるまで待機
        spr.color = Color.black;
        PlayerScript.instance.SetEvilStareStop(true);
        yield return StartCoroutine(Attack(evilParm.proTime.attackTime));

        //攻撃余韻が終わるまで待機
        spr.color = Color.red;
        PlayerScript.instance.SetEvilStareStop(false);
        yield return StartCoroutine(Afterglow(evilParm.proTime.afterglowTime));

        //処理を終了
        eventFlag = false;

    }

   

    //攻撃準備
    IEnumerator PreparaAttack(float waitSeconds)
    {
        //初期設定
        Debug.Log("攻撃準備を開始");
        //待機アニメーションの再生
        Debug.Log("攻撃待機のアニメーションを再生");
        //攻撃準備時間分、待機
        yield return new WaitForSeconds(waitSeconds);
        //完了
        Debug.Log("攻撃準備が完了");

    }

    //攻撃
    IEnumerator Attack(float waitSeconds)
    {
        //初期設定
        Debug.Log("攻撃を開始");
        //移動攻撃のアニメーションの再生
        Debug.Log("攻撃のアニメーションを再生");
        //攻撃準備時間分、待機
        yield return new WaitForSeconds(waitSeconds);
        //完了
        Debug.Log("攻撃が完了");
    }

    //攻撃余韻
    IEnumerator Afterglow(float afterGlowSeconds)
    {
        //初期設定
        Debug.Log("攻撃余韻で待機を開始");
        //余韻アニメーションの再生
        Debug.Log("攻撃余韻のアニメーションを再生");
        //攻撃余韻時間分、待機
        yield return new WaitForSeconds(afterGlowSeconds);
        //完了
        Debug.Log("攻撃余韻が完了");
        rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;

    }

    //移動攻撃
    IEnumerator OnMove(float moveSeconds)
    {
        //初期設定
        Debug.Log("移動攻撃を開始");
        //プレイヤーを貫通するように
        box.isTrigger = true;
        rb.gravityScale = 0;
        //移動開始
        moveFlag = true;
        //画面端までの距離を取得
        targetPos = (Direction > 0) ? RightEdge : LeftEdge;
        targetPos.x += (-Direction*tornadoPrefab.transform.lossyScale.x*2);
        var distance = targetPos.x - transform.position.x;
        //速度を計算
        moveSpeed = (distance / moveSeconds);
        //攻撃時間分待機
        yield return new WaitForSeconds(moveSeconds);
        //完了
        Debug.Log("移動攻撃が完了");
    }
    

    //移動
    void Move()
    {
        if (!moveFlag) return;
        //移動させる
        rb.linearVelocityX = moveSpeed;

        //目的地を超えた場合
        if (Mathf.Abs(transform.position.x)>=targetPos.x)
        {
            if (Mathf.Sign(transform.position.x) != Mathf.Sign(targetPos.x)) return;
            //停止させる
            moveFlag = false;
            box.isTrigger = false;
            rb.gravityScale = 1;
            rb.linearVelocityX = 0;
            transform.position = new Vector3(targetPos.x,transform.position.y);
            rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        }

    }

    

    //技のパラメータ取得用関数
    SnakeAttackParameters getParam(SnakeTechnique tecName)
    {
        SnakeAttackParameters returnParam=null;
        foreach (var param in attackParms)
        {
            if (param.technique == tecName)
            {
                returnParam = param;
                break;
            }
        }
        return returnParam;
    }




}

// 蛇ボス専用enum
[System.Serializable]
public enum SnakeTechnique
{
    None,
    [InspectorName("移動攻撃1")]
    MoveAttack1,
    [InspectorName("移動攻撃2")]
    MoveAttack2,
    [InspectorName("ヘビにらみ")]
    EvilStare,
    [InspectorName("舌で突き刺し")]
    TongueStab,
    [InspectorName("咆哮")]
    Roar,
    [InspectorName("竜巻1")]
    Tornado1,
    [InspectorName("竜巻2")]
    Tornado2,
}

// 蛇ボス用の攻撃パラメータ
[System.Serializable]
public class SnakeAttackParameters : attackParameters
{
    [Header("技名")]
    public SnakeTechnique technique = SnakeTechnique.None;
}
