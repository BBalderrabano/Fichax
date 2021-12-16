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

    public LevelSystem playerLevelData;

    [Header("Match Variables")]
    public int start_max_xp = 10;
    
    public float max_xp_increment = 1.5f;

    private void Awake()
    {
        singleton = this;
        playerLevelData = new LevelSystem(start_max_xp, max_xp_increment);
    }
}

public enum RARITY { 
    NORMAL = 100,
    MAGIC = 800,
    RARE = 20000,
    UNIQUE = 80000,
    VERY_RARE = 100000
}