using UnityEngine;

public class Elephant : MonoBehaviour
{
    public Transform player;
    public float peakHeight = 3f;       // 最高高度
    private Rigidbody2D rb;
    private bool hasJumped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!hasJumped && player != null)
        {
            JumpToPlayer();
            hasJumped = true;
        }
    }

    void JumpToPlayer()
    {
        float g = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);

        Vector2 startPos = transform.position;
        Vector2 targetPos = player.position;

        // --- 垂直方向の初速度計算 ---
        float heightDiff = peakHeight - startPos.y;
        if (heightDiff <= 0f)
        {
            heightDiff = 0.1f; // 最高高度が低い場合の保険
        }

        float Vy = Mathf.Sqrt(2 * g * heightDiff);

        // --- Player までの水平距離 ---
        float Dx = targetPos.x - startPos.x;

        // --- 落下までにかかる合計時間 ---
        float T_total = (Vy / g) * 2f;

        // --- 水平初速度 ---
        float Vx = Dx / T_total;

        // --- 初速を設定 ---
        rb.linearVelocity = new Vector2(Vx, Vy);
    }
}
