using UnityEngine;
using System.Collections.Generic;

public class SnakeScript : Boss
{
    public static SnakeScript SnakeInstance;
    [Header("攻撃パラメータ(待機、余韻など)")]
    public List<SnakeAttackParameters> attackParms = new List<SnakeAttackParameters>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        //待機処理完了を待つ
        if (!waitComplete) return;

        //HPで条件分岐
        if (ratioHP >= 50)
        {
            //HP50%以上の処理
        }
        else
        {
            //HP50%以下の処理
        }

    }
}

// 蛇ボス専用enum
[System.Serializable]
public enum SnakeTechnique
{
    None,
    [InspectorName("移動攻撃1")]
    MoveAttack1,
    [InspectorName("移動攻撃2")]
    MoveAttack2,
    [InspectorName("ヘビにらみ")]
    EvilStare,
    [InspectorName("舌で突き刺し")]
    TongueStab,
    [InspectorName("咆哮")]
    Roar,
    [InspectorName("竜巻1")]
    Tornado1,
    [InspectorName("竜巻2")]
    Tornado2,
}

// 蛇ボス用の攻撃パラメータ
[System.Serializable]
public class SnakeAttackParameters : attackParameters
{
    [Header("技名")]
    public SnakeTechnique technique = SnakeTechnique.None;
}
