using TMPro;
using UnityEngine;

public class CloneCount : MonoBehaviour
{

    public string targetTag = "Clone"; // カウントしたいタグ名
    private int objectCount = 0;
    public TextMeshProUGUI clonecount;

    void Update()
    {
        // 指定したタグを持つすべてのオブジェクトを検索し、配列として取得する
        GameObject[] objects = GameObject.FindGameObjectsWithTag(targetTag);

        // 配列の長さを取得してカウントする
        objectCount = objects.Length;

        // UIに表示するなど、他の処理もここに追加できます
        clonecount.text = objectCount.ToString(); 
    }
}
