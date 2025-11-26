using Unity.VisualScripting;
using UnityEngine;

public class Elephant : Boss
{
    public Transform player;
    public GameObject Ball;

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

    private Rigidbody2D rb;
    private enum JumpState { None, Rising, Floating, Falling }
    private JumpState state = JumpState.None;
    private Vector2 targetPos;
    private float floatTimer = 0f;

    [Header("ボール生成時のジャンプ力")]
    //ボール関連の関数達
　　public float ballJumpForce = 5f;
    private bool hasJumped = false;
    public bool balljump = false;

    void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!waitComplete) return;
        //BallJump();
        //Jump();
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

        // --- ここを変更（斜めジャンプ）------------------
        float dirX = Mathf.Sign(targetPos.x - transform.position.x);
        float riseHorizontalSpeed = 2f;     // 水平方向ジャンプ力（調整可）
        rb.linearVelocity = new Vector2(dirX * riseHorizontalSpeed, riseForce);
        // ----------------------------------------------------

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

        // 滞空しながら着地点に横移動
        float dirX = Mathf.Sign(targetPos.x - transform.position.x);
        rb.linearVelocity = new Vector2(dirX * floatMoveSpeed, 0f);

        // 指定時間滞空したら落下フェーズへ
        if (floatTimer >= floatTime)
        {
            rb.gravityScale = fallGravityScale;
            state = JumpState.Falling;
        }
    }

    void BallJump()
    {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, ballJumpForce);
            hasJumped=true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasJumped&&collision.gameObject.CompareTag("Ground"))
        {
                Debug.Log("balljumpがtrueになりました!!");
                balljump = true;
                hasJumped = false;
        }
    }
}