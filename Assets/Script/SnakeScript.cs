using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SnakeScript : Boss
{
    public static SnakeScript SnakeInstance;
    [Header("攻撃パラメータ(待機、余韻など)")]
    public List<SnakeAttackParameters> attackParms = new List<SnakeAttackParameters>();

    [SerializeField] private Vector2 rightEdge;//画面右端
    [SerializeField] private Vector2 leftEdge; //画面左端

    private float moveSpeed;//移動速度
    private bool moveFlag = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
        //待機処理完了を待つ
        if (!waitComplete) return;

        //HPで条件分岐
        if (ratioHP >= 50)
        {
            //HP50%以上の処理
            StartCoroutine(MoveAttack1());
            
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
        SnakeAttackParameters moveParm=null;
        foreach (var param in attackParms) 
        {
            if (param.technique == SnakeTechnique.MoveAttack1) moveParm = param;
            break;
        }

        //移動攻撃1の設定がされていない場合は終了
        if (moveParm == null) yield break;

        //攻撃準備を行う
        yield return StartCoroutine(PreparaAttack(moveParm.proTime.preparationTime));

        //攻撃
        yield return StartCoroutine(OnMove(moveParm.proTime.attackTime));

        //攻撃後の待機余韻
        yield return StartCoroutine(Afterglow(moveParm.proTime.afterglowTime));

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

    //移動攻撃
    IEnumerator OnMove(float moveSeconds)
    {
        //初期設定
        Debug.Log("移動攻撃を開始");
        //移動攻撃のアニメーションの再生
        Debug.Log("移動攻撃のアニメーションを再生");
        //移動開始
        moveFlag = true;
        //画面端までの距離を取得
        var distance = leftEdge.x - transform.position.x;
        //速度を計算
        moveSpeed = distance / moveSeconds;
        //攻撃時間分待機
        yield return new WaitForSeconds(moveSeconds);
        //移動フラグを降ろす
        moveFlag = false;
        //完了
        Debug.Log("移動攻撃が完了");
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
    }


    //移動
    void Move()
    {
        if (!moveFlag) return;
        //移動させる
        rb.linearVelocityX = moveSpeed;

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
