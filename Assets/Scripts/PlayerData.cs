using UnityEngine;
using System.Collections;

public static class PlayerData
{
    private static int AMOUNT_TO_MATCH = 3;
    public static int GET_AMOUNT_TO_MATCH() { return AMOUNT_TO_MATCH; }

    public static RARITY GetRarity(RARITY rarity) { return rarity; }
}

public enum RARITY { 
    NORMAL = 100,
    MAGIC = 800,
    RARE = 20000,
    UNIQUE = 80000,
    VERY_RARE = 100000
}