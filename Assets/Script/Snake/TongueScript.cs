using UnityEngine;
using System.Collections;

public class TongueScript : MonoBehaviour
{
    [SerializeField] private GameObject ground;//地面オブジェクト

    public bool Hit { get { return hit;} private set { } }

    private float strechSpeed;//舌が伸びる速度
    private bool stretch = true;
    private bool hit = false;//プレイヤーに当たったかどうか



    //舌を伸ばす
    public IEnumerator StretchTongue(float stretchTime)
    {
        stretch = true;
        hit = false;
        //プレイヤーまでのベクトルを取得
        var playerPos = PlayerScript.instance.transform.position;
        Vector2 dir = (playerPos - transform.position).normalized;
        //プレイヤー方向に舌を回転
        transform.rotation = Quaternion.FromToRotation(Vector3.down, dir);
        //ボスとプレイヤーの位置関係から舌を飛ばすY方向を決定
        var targetY = (dir.y>0) ? Camera.main.ViewportToWorldPoint(Vector2.one).y : Camera.main.ViewportToWorldPoint(Vector2.zero).y;

        //プレイヤー方向に伸ばした地面までの距離を求める
        float distance = Vector2.Distance(transform.position, new Vector2(playerPos.x + playerPos.x - transform.position.x, targetY));

        Debug.Log("distance=" + distance);
        //舌を伸ばすスピードを計算する
        strechSpeed = distance / stretchTime;

        //舌が床に突き刺さるまで舌を伸ばす
        while (stretch)
        {
            transform.localScale += new Vector3(0, strechSpeed * Time.deltaTime);
            yield return null;
        }


    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //地面・壁に触れたら舌を伸ばすのを停止
        if (collision.gameObject.CompareTag("Ground")|| collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("地面にしっかりと当たっているよ");
            stretch = false;
        }

        //プレイヤーに触れたらフラグを立て、硬直時間を変更する
        if (collision.gameObject.CompareTag("Player"))
        {
            hit = true;
        }


    }
}
