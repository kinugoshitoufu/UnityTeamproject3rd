using NUnit;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using System.Collections.Generic;
using System;
[RequireComponent(typeof(Rigidbody2D))]

public class Elephant : Boss
{
    public static Elephant elephant;
    public Transform player;
    public Collider2D collider2D;

    //処理が終了したか判定、待機用
    public bool EventEnd { get { return eventEnd; } }
    private bool eventEnd = false;
    private bool moveFlag=false;
    private float moveSpeed;
    private Vector2 targetPos2;

    [Header("攻撃パラメータ(待機、余韻など)")]
    public List<ElephantAttackParameters> attackParms = new List<ElephantAttackParameters>(Enum.GetValues(typeof(ElephantTechnique)).Length);

    //ヒップドロップ関連の関数達
    private bool Jumping=false;

    [Header("Phase1: 急上昇")]
    public float riseForce = 15f;
    public float riseGravityScale = 0.8f;

    [Header("Phase2: 滞空")]
    public float floatTime = 2.0f;
    public float floatGravityScale = 0.05f;
    public float floatMoveSpeed = 4f;

    [Header("Phase3: 急降下")]
    public float fallGravityScale = 4f;
    public float fallStartDistance = 0.5f;
    private enum JumpState { None, Rising, Floating, Falling }
    private  JumpState state = JumpState.None;
    private Vector2 targetPos;
    private float floatTimer = 0f;
    public bool JumpFinished=false;


    [Header("ボール生成時関連の設定")]
    //ボール関連の関数達
    public GameObject Ball;
    private Transform ballRightSpawner;
    private Transform ballLeftSpawner;
    public float ballJumpForce = 5f;　　//ジャンプ力
    public float highGravity = 50f;     //高重力
    public float targetHeight = 2f;   //指定の高さ
    public bool balljump = false;
    private bool BallCreate = false;
    private bool hasBallJumped = false;
    private bool hasIncreasedGravity = false;
    public bool RightJumpFinished = false;
    private bool isballjump=false;

    [Header("攻撃判定")]
    public Collider2D AttackCollider;
    private int AttackCount=0;
    public bool AttackFinished = false;
    public bool AttackEnd = false;

    [Header("ジャンプ関連の設定")]
    public float targetX = 8f;
    public float peakHeight = 3f;
    public float gravityScale = 1f;

    [Header("歩き関連の設定")]
    public float interval = 1f;  // 1秒ごと
    private float timer = 0f;
    private float Walktimer = 0f;   //何秒歩いたかの関数
    private float WalkAfterGlow = 0.4f;//歩いた後の待機余韻時間
    private bool WalkFinished = false;
    public float GetWalktimer()
    {
        return Walktimer;
    }

    public void ResetWalkTimer()
    {
        Walktimer = 0;
    }
     
    public int GetAttackCount()
    {
        return AttackCount;
    }
    float defaultMass;
    void Start()
    {
        base.Start();
        rb.gravityScale = gravityScale;
        AttackCollider.enabled = false;
        collider2D=GetComponent<Collider2D>();
        //defaultMass = rb.mass;
    }

    void Update()
    {
        base.Update();
        if (!waitComplete) return;
        //if(!isballjump)
        //{
        //    Debug.Log("isballjumpがフォルスになりました!!");
        //}
       

        //RightJump();
        //BallJump();
        //Attack();
        //Walk();
        //StartJumpAction();

        // 自分自身のx座標とPlayerのx座標の差の絶対値を取る
        //float distanceX = Mathf.Abs(transform.position.x - player.position.x);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Awake()
    {
        elephant = this;
    }
    
    //移動
    public void Walk()
    {
        //Debug.Log("プレイヤーに向かって移動中");
        timer += Time.deltaTime;
        Walktimer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            Vector3 pos = transform.position;
            // Player の方向 (右なら +1、左なら -1)
            float direction = Mathf.Sign(player.position.x - pos.x);
            // X に 1 ずつ近づく
            pos.x += direction * 1f;
            transform.position = pos;
            WalkFinished = true;

        }
    }
    public bool CheckPlayerDirection()
    {
        float dirToPlayer = player.position.x - transform.position.x;
        float facingDir = Mathf.Sign(transform.localScale.x);

        //プレイヤー自体が向いている方向にいる
        if(Mathf.Sign(dirToPlayer) == facingDir)
        {
            return true;
        }
        //向いていなかったら
        else
        {
            return false;
        }
    }

    public IEnumerator WalkCoroutine()
    {
        eventEnd = false;
        WalkFinished = false;

        

        //プレイヤーに到達するまで歩かせる
        while (Vector2.Distance(transform.position, player.position) >= transform.localScale.x * 0.5f)
        {
            if (player == null) { eventEnd = true; yield break; }
            if (isJumping) { yield break; }
            Walk();
            yield return null;
        }

        //移動完了を待機
        yield return new WaitUntil(() => WalkFinished == true);

        Debug.Log("プレイヤーの移動が完了");

        //移動後、余韻を持たせる
        yield return new WaitForSeconds(WalkAfterGlow);
        eventEnd = true;

    }

    //ヒップドロップ
    private bool isJumping = false;

    IEnumerator JumpCoroutine()
    {
        isJumping = true;
        eventEnd = false;

        if (player == null)
        {
            eventEnd = true;
            yield break;
        }

        // =============================
        // 1. 上昇フェーズ
        // =============================
        rb.gravityScale = riseGravityScale;
        float riseHorizontalSpeed = 2f;
        float dirX = Mathf.Sign(player.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(dirX * riseHorizontalSpeed, riseForce);

        while (rb.linearVelocity.y > 0f)
        {
            yield return null;
        }
        
        // =============================
        // 2. 浮遊フェーズ
        // =============================
        rb.gravityScale = floatGravityScale;
        rb.linearVelocity = Vector2.zero;

        float floatTimer = 0f;
        float maxFloatSpeed = 8f; // 画面端対応用（調整可）
        Vector3 lockedTargetPos = Vector3.zero;
        bool isTargetLocked = false;

        while (floatTimer < floatTime)
        {
            Vector3 targetPos = player.position;

            float remainTime = floatTime - floatTimer;
            float dx = targetPos.x - transform.position.x;

            // 残り時間で必ず届くための横速度
            float needSpeedX = dx / Mathf.Max(remainTime, 0.01f);

            // 速度上限をかけて不自然な加速を防ぐ
            float clampedSpeedX = Mathf.Clamp(
                needSpeedX,
                -maxFloatSpeed,
                maxFloatSpeed
            );

            rb.linearVelocity = new Vector2(clampedSpeedX, 0f);

            floatTimer += Time.deltaTime;
            Debug.Log("2. 滞空フェーズ");
            yield return null;
        }

        // =============================
        // 3. 落下フェーズ（ヒップドロップ）
        // =============================
        rb.gravityScale = fallGravityScale;
        
        // 落下開始時の位置とターゲット
        Vector3 startPos = transform.position;
        Vector3 target = isTargetLocked ? lockedTargetPos : player.position;

        float g = Physics2D.gravity.y * fallGravityScale;
        float fallTime = Mathf.Sqrt((2f * Mathf.Max(0.01f, startPos.y - target.y)) / -g);

        float needVx = (target.x - startPos.x) / fallTime;

        rb.linearVelocity = new Vector2(needVx, 0f);
        while (transform.position.y > target.y && transform.position.y > -3f)
        {
            Debug.Log("3. 落下フェーズ");
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        Jumping = true;
        eventEnd = true;
        isJumping = false;
    }




    //突進
    public IEnumerator Rush()
    {
        yield return null;
    }

    //移動攻撃1(低い突進)
    public IEnumerator MoveAttack1()
    {
        //移動攻撃用のパラメータを取得
        ElephantAttackParameters moveParm = getParam(ElephantTechnique.MoveAttack1);

        //移動攻撃1の設定がされていない場合は終了
        if (moveParm == null || eventEnd == true) yield break;

        //処理を開始
        eventEnd = false;

        //攻撃準備が終わるまで待機
        yield return StartCoroutine(PreparaAttack(moveParm.proTime.preparationTime));

        //攻撃が終わるまで待機
        yield return StartCoroutine(OnMove(moveParm.proTime.attackTime));

        //攻撃後の待機余韻が終わるまで待機
        yield return StartCoroutine(Afterglow(moveParm.proTime.afterglowTime));

        //処理を終了
        //CountAttack(SnakeTechnique.MoveAttack1);
        eventEnd = true;
        Debug.Log("移動攻撃が完了しました");

        //テスト
        //if(tornado1)StartCoroutine(Tornado1());
        //else StartCoroutine(Tornado2());

    }

    //移動攻撃
    IEnumerator OnMove(float moveSeconds)
    {
        //初期設定
        Debug.Log("移動攻撃を開始");
        //プレイヤーを貫通するように

        //rb.mass = 0;
        //box.isTrigger = true;
        //rb.gravityScale = 0;

        //移動開始
        moveFlag = true;
        //画面端までの距離を取得
        targetPos2 = (Direction > 0) ? RightEdge : LeftEdge;
        //targetPos.x += (-Direction * tornadoPrefab.transform.lossyScale.x * 2);
        var distance = targetPos2.x - transform.position.x;
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
        if (Vector2.Distance(transform.position,targetPos2)<=0.5f)//Mathf.Abs(transform.position.x) >= targetPos2.x
        {
            if (Mathf.Sign(transform.position.x) != Mathf.Sign(targetPos2.x)) return;
            //停止させる
            moveFlag = false;
            //box.isTrigger = false;
            //rb.mass = 1;
            //rb.gravityScale = 1;

            rb.linearVelocityX = 0;
            transform.position = new Vector3(targetPos2.x, transform.position.y);
            rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        }

    }

    //右端ジャンプ
    public IEnumerator RightJumpCoroutine()
    {
        if (!hasBallJumped)
        {
            eventEnd = false;

            float g = Physics2D.gravity.y * rb.gravityScale;

            Vector2 startPos = transform.position;
            float dx = targetX - startPos.x;

            float peakY = startPos.y + peakHeight;
            float vy = Mathf.Sqrt(-2f * g * peakHeight);

            float tUp = -vy / g;
            float tDown = Mathf.Sqrt(2f * peakHeight / -g);
            float totalTime = tUp + tDown;

            float vx = dx / totalTime;

            rb.linearVelocity = new Vector2(vx, vy);

            while (transform.position.x < targetX)
            {
                yield return null;
            }

            rb.linearVelocity = Vector2.zero;
            transform.position = new Vector2(targetX, transform.position.y);

            hasBallJumped = true;
            eventEnd = true;
        }
    }

    //左端ジャンプ
    public IEnumerator LeftJumpCoroutine()
    {
        if (!hasBallJumped)
        {
            eventEnd = false;

            float g = Physics2D.gravity.y * rb.gravityScale;

            Vector2 startPos = transform.position;
            float dx = targetX - startPos.x;   // targetX を左側に設定すれば負になる

            // 放物線の最高点
            float peakY = startPos.y + peakHeight;

            // 垂直初速
            float vy = Mathf.Sqrt(-2f * g * peakHeight);

            // 空中時間
            float tUp = -vy / g;
            float tDown = Mathf.Sqrt(2f * peakHeight / -g);
            float totalTime = tUp + tDown;

            // 水平速度（dx が負なので左向き）
            float vx = dx / totalTime;

            // 初速を与える
            rb.linearVelocity = new Vector2(vx, vy);

            while (transform.position.x > targetX)
            {
                Debug.Log("左端ジャンプ中");
                yield return null;
            }

            rb.linearVelocity = Vector2.zero;
            hasBallJumped = true;
            eventEnd = true;
        }
    }

    //ボールの生成ジャンプ
    public IEnumerator BallJump()
    {
        isballjump = false;
        eventEnd = false;
        if (!hasBallJumped)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, ballJumpForce);
            rb.gravityScale = 1f;    // 上昇中は通常重力
            hasBallJumped = true;
        }
        //ジャンプ中のみ高さを監視
        if (hasBallJumped && !hasIncreasedGravity)
        {
            //指定の座標に到達するまで、待機
            while (transform.position.y <= targetHeight)
            {
                BallCreate = true;
                yield return null;
            }
            //到達後の処理
            Debug.Log("指定の座標に到達!! 重力を増やして急降下");
            rb.gravityScale = fallGravityScale;
            hasIncreasedGravity = true;
        }
        eventEnd = true;
        isballjump = true;
    }

    //攻撃判定
    public IEnumerator Attack()
    {
        eventEnd = false;
        ElephantAttackParameters NoseAttack=getParam(ElephantTechnique.NoseAttack);
        //待機時間分、待機
        yield return StartCoroutine(PreparaAttack(NoseAttack.proTime.preparationTime));
        AttackCollider.enabled = true;
        AttackCount++;
        yield return StartCoroutine(AttackCoroutine(NoseAttack.proTime.attackTime));
        AttackCollider.enabled = false;
        yield return StartCoroutine(Afterglow(NoseAttack.proTime.afterglowTime));
        eventEnd = true;
    }

    //ジャンプ攻撃判定
    public IEnumerator StartJumpAction()
    {
        eventEnd = false;
        ElephantAttackParameters HipDrop = getParam(ElephantTechnique.hipdrop);
        yield return StartCoroutine(PreparaAttack(HipDrop.proTime.preparationTime));
        if (!isJumping)
        {
            //Debug.Log("コルーチン起動!!");
            yield return StartCoroutine(JumpCoroutine());
        }
        else
        {
            eventEnd = true;
        }
        yield return StartCoroutine(Afterglow(HipDrop.proTime.afterglowTime));
        eventEnd = true;
    }

    //ボールジャンプ攻撃判定 
    public IEnumerator BallandJump()
    {
        eventEnd = false;
        ElephantAttackParameters balljump = getParam(ElephantTechnique.hipdrop);
        yield return StartCoroutine(PreparaAttack(balljump.proTime.preparationTime));
        if (!isballjump)
        {
            yield return StartCoroutine(BallJump());
        }
        yield return StartCoroutine(Afterglow(balljump.proTime.afterglowTime));
        eventEnd = true;
    }

    //攻撃準備
    IEnumerator PreparaAttack(float waitSeconds, bool colorChange = true)
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

    //攻撃中
    IEnumerator AttackCoroutine(float waitSeconds)
    {
        //初期設定
        Debug.Log("攻撃を開始");
        //移動攻撃のアニメーションの再生
        Debug.Log("攻撃のアニメーションを再生");
        //攻撃準備時間分、待機
        yield return new WaitForSeconds(waitSeconds);
        //完了
        Debug.Log("攻撃が完了");
        PlayerScript.instance.SetEvilStareStop(false);
        //spr.color = Color.red;
    }

    //攻撃余韻
    IEnumerator Afterglow(float afterGlowSeconds)
    {
        //初期設定
        //if(afterGlowSeconds!= 0) spr.color = Color.green;//攻撃準備中を示す
        //rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        Debug.Log("攻撃余韻で待機を開始");
        //余韻アニメーションの再生
        Debug.Log("攻撃余韻のアニメーションを再生");
        //攻撃余韻時間分、待機
        yield return new WaitForSeconds(afterGlowSeconds);
        //完了
        Debug.Log("攻撃余韻が完了");
        //rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (BallCreate && collision.gameObject.CompareTag("Ground"))
        {
            balljump = true;
            //Instantiate(Ball, ballRightSpawner.position, Quaternion.identity);
            //Instantiate(Ball, ballLeftSpawner.position, Quaternion.identity);
            BallCreate = false;
            Debug.Log("ボール生成が可能になりました!!");
        }
        //if (Jumping&&collision.gameObject.CompareTag("Player"))
        //{
        //    Debug.Log("プレイヤーに当たっています");
        //}
        //if (collider2D.gameObject.CompareTag("Player"))
        //{
        //    rb.mass = 99999f;
        //}

    }

    //技のパラメータ取得用関数
    ElephantAttackParameters getParam(ElephantTechnique tecName)
    {
        ElephantAttackParameters returnParam = null;
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

// 象ボス専用enum
[System.Serializable]
public enum ElephantTechnique
{
    None,
    [InspectorName("鼻で近接攻撃")]
    NoseAttack,
    [InspectorName("ヒップドロップ攻撃1")]
    hipdrop,
    [InspectorName("ヒップドロップ攻撃(ボール生成版)")]
    hipdrop2,
    [InspectorName("移動攻撃1")]
    MoveAttack1,
}

//象ボス用の攻撃パラメータ
[System.Serializable]
public class ElephantAttackParameters : attackParameters
{
    [Header("技名")]
    public ElephantTechnique technique = ElephantTechnique.None;
}