using NUnit;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
[RequireComponent(typeof(Rigidbody2D))]

public class Elephant : Boss
{
    public Transform player;

    //ヒップドロップ関連の関数達
    [Header("Phase1: 急上昇")]
    public float riseForce = 15f;
    public float riseGravityScale = 0.8f;

    [Header("Phase2: 滞空")]
    public float floatTime = 0.6f;
    public float floatGravityScale = 0.05f;
    public float floatMoveSpeed = 2f;

    [Header("Phase3: 急降下")]
    public float fallGravityScale = 4f;
    public float fallStartDistance = 0.5f;

    private enum JumpState { None, Rising, Floating, Falling }
    private JumpState state = JumpState.None;
    private Vector2 targetPos;
    private float floatTimer = 0f;

    [Header("ボール生成時関連の設定")]
    //ボール関連の関数達
    public float ballJumpForce = 5f;　　//ジャンプ力
    public float highGravity = 50f;     // 高重力
    public float targetHeight = 2f;   // 指定の高さ
    public bool balljump = false;
    private bool hasJumped = false;
    private bool hasBallJumped = false;
    private bool hasIncreasedGravity = false;

    [Header("攻撃判定")]
    public Collider2D AttackCollider;

    [Header("ジャンプ関連の設定")]
    public float targetX = 8f;
    public float peakHeight = 3f;
    public float gravityScale = 1f;

    [Header("歩き関連の設定")]
    public float WalkSpeed = 1f;


    void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        AttackCollider.enabled = false;
    }

    void Update()
    {
        if (!waitComplete) return;
        //BallJump();
        //Jump();
        //Attack();
        //RightJump();
        //Walk();
        // 自分自身のx座標とPlayerのx座標の差の絶対値を取る
        float distanceX = Mathf.Abs(transform.position.x - player.position.x);

        if (distanceX >= 4f&& distanceX <= 10f)
        {
            // Playerからx座標でちょうど5離れている時の処理
            Debug.Log("Playerからx座標で4～10離れている！");
        }
    }

    void Walk()
    {
        transform.position += new Vector3(WalkSpeed * Time.deltaTime, 0f, 0f);
    }

    void Jump()
    {
        switch (state)
        {
            case JumpState.None:
                StartJump();
                break;

            case JumpState.Rising:
                CheckRiseToFloat();
                break;

            case JumpState.Floating:
                ExecuteFloating();
                break;

            case JumpState.Falling:
                // 物理に任せて落下
                break;
        }
    }

    // -----------------------------------------------------
    // ジャンプ開始 (Player位置を一度だけ取得)
    // -----------------------------------------------------
    void StartJump()
    {
        targetPos = player.position;

        rb.gravityScale = riseGravityScale;

        // 斜めジャンプ
        float dirX = Mathf.Sign(targetPos.x - transform.position.x);
        float riseHorizontalSpeed = 2f;     // 水平方向ジャンプ力（調整可）
        rb.linearVelocity = new Vector2(dirX * riseHorizontalSpeed, riseForce);

        state = JumpState.Rising;
    }

    // -----------------------------------------------------
    // 上昇 → 最高点付近になったら Float へ
    // -----------------------------------------------------
    void CheckRiseToFloat()
    {
        if (rb.linearVelocity.y <= 0f)
        {
            // 滞空開始
            rb.gravityScale = floatGravityScale;
            rb.linearVelocity = Vector2.zero;

            floatTimer = 0f;
            state = JumpState.Floating;
        }
    }

    // -----------------------------------------------------
    // 滞空中 ＋ ゆっくり横移動
    // -----------------------------------------------------
    void ExecuteFloating()
    {
        floatTimer += Time.deltaTime;

        // 滞空中はゆっくり横移動
        float dirX = Mathf.Sign(targetPos.x - transform.position.x);
        rb.linearVelocity = new Vector2(dirX * floatMoveSpeed, 0f);

        if (floatTimer >= floatTime)
        {
            rb.gravityScale = fallGravityScale;

            // 落下にかかる時間を計算して着地位置を保証
            float startY = transform.position.y;
            float endY = targetPos.y;  // 着地したい Player の Y

            float g = Physics2D.gravity.y * fallGravityScale; // 重力加速度（負）

            // h = v0 * t + 1/2 * g * t^2  （v0=0）
            // t = sqrt( 2 * (startY - endY) / -g )
            float fallTime = Mathf.Sqrt((2f * (startY - endY)) / -g);

            //この時間でちょうど x に到達する横速度をセット
            float needVx = (targetPos.x - transform.position.x) / fallTime;

            rb.linearVelocity = new Vector2(needVx, 0f);

            state = JumpState.Falling;
        }
    }


    void RightJump()
    {
        if (!hasBallJumped)
        {
            if (player.position.x >= -8 && player.position.x <= -5)
            {
                float g = Physics2D.gravity.y * rb.gravityScale;

                Vector2 startPos = transform.position;
                float dx = targetX - startPos.x;

                // 放物線の最高点を決める
                float peakY = startPos.y + peakHeight;

                // 垂直初速（上向きなので -g を使う）
                float vy = Mathf.Sqrt(-2f * g * peakHeight);

                // 空中にいる時間（上昇 + 下降）
                float tUp = -vy / g;
                float tDown = Mathf.Sqrt(2f * (peakY - startPos.y) / -g);
                float totalTime = tUp + tDown;

                // 水平速度（一定）
                float vx = dx / totalTime;

                // 初速ベクトルを与える
                rb.linearVelocity = new Vector2(vx, vy);
            }
            hasBallJumped = true;
        }
    }
    void BallJump()
    {
        if (!hasBallJumped)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, ballJumpForce);
            rb.gravityScale = 1f;    // 上昇中は通常重力
            hasBallJumped = true;
        }
        //ジャンプ中のみ高さを監視
        if (hasBallJumped && !hasIncreasedGravity)
        {
            if (transform.position.y >= targetHeight)
            {
                Debug.Log("指定の座標に到達!! 重力を増やして急降下");
                rb.gravityScale = fallGravityScale;

                hasIncreasedGravity = true;
            }
        }
    }

    void Attack()
    {
        //Debug.Log("判定オン!!!");
        AttackCollider.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasJumped && collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("balljumpがtrueになりました!!");
            balljump = true;
            hasJumped = false;
        }
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if(other.CompareTag("Player"))
    //    {
    //        Debug.Log("プレイヤーにダメージ!!!!");
    //    }
    //}
}