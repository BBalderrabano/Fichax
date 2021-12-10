using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour
{
    [Header("Constant Variables")]
    public static PlayerData singleton;

    [SerializeField]
    private int AMOUNT_TO_MATCH = 5;
    public int GET_AMOUNT_TO_MATCH() { return AMOUNT_TO_MATCH; }

    public RARITY GetRarity(RARITY rarity) { return rarity; }

    [Header("Match Variables")]

    [SerializeField]
    private int score = 0;

    public void AddScore(int amount) { score += amount; }

    public int GetScore() { return score; }

    [SerializeField]
    private int xp = 0;

    [SerializeField]
    private int max_xp = 10;

    private void Start()
    {
        singleton = this;
    }
}

public enum RARITY { 
    NORMAL = 100,
    MAGIC = 800,
    RARE = 20000,
    UNIQUE = 80000,
    VERY_RARE = 100000
}