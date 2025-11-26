using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float downSpeed = 3f;       // 下に移動する速度
    public float horizontalSpeed = 2f; // 横移動する速度
    private bool isMovingDown = true;  // 最初は下に移動中
    private bool onGround = false;     // 地面に触れたかどうか

    public float spawnInterval = 2f;      // 生成間隔

    public float undersize = 1f;
    public LayerMask floorLayer;

    private int horizontalDirection = 0; // 1なら右、-1なら左

    private void Update()
    {
        // 地面判定
        Vector2 underEnd = transform.position - new Vector3(0, undersize, 0);
        RaycastHit2D raycast = Physics2D.Linecast(transform.position, underEnd, floorLayer);
        if (raycast && !onGround)
        {
            onGround = true;
            isMovingDown = false;

            // 地面に着地した瞬間に左右の移動方向を決定
            if (transform.position.x < 0)
            {
                horizontalDirection = 1;  // 右に移動
            }
            else
            {
                horizontalDirection = -1; // 左に移動
            }
        }

        if (isMovingDown && !onGround)
        {
            // 下に移動
            transform.position += Vector3.down * downSpeed * Time.deltaTime;
        }
        else if (onGround)
        {
            // 地面に触れたら横移動（方向を保持して移動）
            transform.position += Vector3.right * horizontalDirection * horizontalSpeed * Time.deltaTime;
        }
    }
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
