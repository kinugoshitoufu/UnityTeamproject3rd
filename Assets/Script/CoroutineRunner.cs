using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;
    public static CoroutineRunner Instance
    {
        get
        {
            //コルーチンランナーがなければ生成
            if (_instance == null)
                _instance = new GameObject("[CoroutineRunner]").AddComponent<CoroutineRunner>();
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    //コルーチン汎用関数
    public static IEnumerator WaitAll(params IEnumerator[] coroutines)
    {
        int finishedCount = 0;

        foreach (var routine in coroutines)
        {
            // コルーチンランナーに関数を渡してStartCoroutineさせる
            CoroutineRunner.Instance.StartCoroutine(Wrapper(routine, () => finishedCount++));
        }

        // 全部完了するまで待機
        yield return new WaitUntil(() => finishedCount >= coroutines.Length);
    }

    public static IEnumerator Wait(IEnumerator coroutine, System.Action onFinish)
    {
        yield return CoroutineRunner.Instance.StartCoroutine(coroutine);
        onFinish?.Invoke();
    }


    private static IEnumerator Wrapper(IEnumerator routine, System.Action onFinish)
    {
        yield return routine;
        onFinish?.Invoke();
    }

}


