using UnityEngine;
using System.Collections;

public class TongueScript : MonoBehaviour
{
    [SerializeField] private GameObject ground;//地面オブジェクト

    private float strechSpeed;//舌が伸びる速度
    private bool stretch = true;



    //舌を伸ばす
    public IEnumerator StretchTongue(float stretchTime)
    {
        stretch = true;
        //プレイヤーまでのベクトルを取得
        var playerPos = PlayerScript.instance.transform.position;
        Vector2 dir = (playerPos - transform.position).normalized;
        //プレイヤー方向に舌を回転
        transform.rotation = Quaternion.FromToRotation(Vector3.down, dir);
        //プレイヤー方向に伸ばした地面までの距離を求める
        float distance = Vector2.Distance(transform.position, new Vector2(playerPos.x + playerPos.x - transform.position.x, ground.transform.position.y));

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
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("地面にしっかりと当たっているよ");
            stretch = false;
        }
    }
}
