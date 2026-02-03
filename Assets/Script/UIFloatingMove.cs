using System.Collections;
using UnityEngine;

public class UIFloatingMove : MonoBehaviour
{
    [SerializeField] private RectTransform rect;

    /// <summary>
    /// 紙のようにひらひらしながら自然に着地する移動
    /// </summary>
    public IEnumerator MoveFlutterSmooth(
        Vector2 targetPos,
        float duration,
        float waveStrength,
        float waveSpeed)
    {
        StartCoroutine(FlutterMoveSmoothCoroutine(
            targetPos, duration, waveStrength, waveSpeed));

        yield return null;
    }

    public IEnumerator FlutterMoveSmoothCoroutine(
        Vector2 targetPos,
        float duration,
        float waveStrength,
        float waveSpeed)
    {
        Vector2 startPos = rect.anchoredPosition;
        Quaternion startRot = rect.localRotation;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            // 移動（等速 → 自然にしたければ Ease を変えてもOK）
            Vector2 basePos = Vector2.Lerp(startPos, targetPos, t);

            // ゴールに近づくほど揺れが弱くなる
            float damp = 1f - t;

            // ひらひら揺れ
            float wave = Mathf.Sin(time * waveSpeed) * waveStrength * damp;

            // 回転も同様に減衰
            float rotZ = Mathf.Sin(time * waveSpeed) * waveStrength * 2f * damp;

            rect.anchoredPosition = basePos + new Vector2(wave, 0f);
            rect.localRotation = Quaternion.Euler(0f, 0f, rotZ);

            yield return null;
        }

        Debug.Log("2/3　完了");

        // もう補正しない（そのまま自然停止）
        rect.anchoredPosition = targetPos;
        rect.localRotation = startRot;

        Debug.Log("2/4　完了");

        yield break;
    }
}
