using System.Collections;
using UnityEngine;

public class UIFloatingMove : MonoBehaviour
{
    [SerializeField] private RectTransform rect;

    /// <summary>
    /// Ž†‚Ì‚æ‚¤‚É‚Ð‚ç‚Ð‚ç‚µ‚È‚ª‚çŽ©‘R‚É’…’n‚·‚éˆÚ“®
    /// </summary>
    public void MoveFlutterSmooth(
        Vector2 targetPos,
        float duration,
        float waveStrength,
        float waveSpeed)
    {
        StartCoroutine(FlutterMoveSmoothCoroutine(
            targetPos, duration, waveStrength, waveSpeed));
    }

    private IEnumerator FlutterMoveSmoothCoroutine(
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

            // ˆÚ“®i“™‘¬ ¨ Ž©‘R‚É‚µ‚½‚¯‚ê‚Î Ease ‚ð•Ï‚¦‚Ä‚àOKj
            Vector2 basePos = Vector2.Lerp(startPos, targetPos, t);

            // ƒS[ƒ‹‚É‹ß‚Ã‚­‚Ù‚Ç—h‚ê‚ªŽã‚­‚È‚é
            float damp = 1f - t;

            // ‚Ð‚ç‚Ð‚ç—h‚ê
            float wave = Mathf.Sin(time * waveSpeed) * waveStrength * damp;

            // ‰ñ“]‚à“¯—l‚ÉŒ¸Š
            float rotZ = Mathf.Sin(time * waveSpeed) * waveStrength * 2f * damp;

            rect.anchoredPosition = basePos + new Vector2(wave, 0f);
            rect.localRotation = Quaternion.Euler(0f, 0f, rotZ);

            yield return null;
        }

        // ‚à‚¤•â³‚µ‚È‚¢i‚»‚Ì‚Ü‚ÜŽ©‘R’âŽ~j
        rect.anchoredPosition = targetPos;
        rect.localRotation = startRot;
    }
}
