using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem
{
    private float score = 0;
    private int level = 1;
    private float xp = 0;
    private int max_xp = 10;
    private float max_xp_increment = 0f;

    public void AddScore(float amount)
    {
        score += amount;
        xp += amount;

        while (xp >= max_xp) {
            level++;
            xp -= max_xp;
            max_xp = Mathf.FloorToInt(max_xp * max_xp_increment);
        }
    }

    public float GetScore() { return score; }

    public void SetScore(float amount) { score = amount; }

    public float GetLevel() { return level; }

    public float GetXp() { return xp; }

    public float GetXpToLevel() { return max_xp; }

    public LevelSystem(int max_xp, float max_xp_increment) {
        this.max_xp = max_xp;
        this.max_xp_increment = max_xp_increment;
    }
}
