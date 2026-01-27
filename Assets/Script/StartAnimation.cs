using UnityEngine;
using System.Collections;

public class StartAnimation : MonoBehaviour
{
    public Transform banner;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerScript.instance.StartFlag == true)
        {
            StartCoroutine(Move(banner,new Vector3(0,10,0),2.0f,Easing.Ease.OutQuart,false));
            PlayerScript.instance.StartFlag = false;
            ScreenManager.instance.ShowSleepBoss();
        }
        
    }
    public IEnumerator Move(Transform transform, Vector3 destinationPos, float seconds, Easing.Ease easing, bool absolute)
    {
        // イージング関数の取得
        var Ease = Easing.GetEasingMethod(easing);

        // 現在点と移動先の設定
        Vector3 staPos = transform.localPosition;
        Vector3 endPos = absolute ? destinationPos : staPos + destinationPos;
        // 初期地点と目標地点の差
        Vector3 difPos = endPos - staPos;

        // N秒かけて移動させる
        float e = 0;
        while (true)
        {
            yield return null;
            e += Time.deltaTime / seconds;
            if (e >= 1.0f)
            {
                transform.localPosition = endPos;
                break;
            }
            Vector3 nextPos = staPos + Ease(e) * difPos;
            transform.localPosition = nextPos;
        }
    }
}
