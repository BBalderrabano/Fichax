using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chip : MonoBehaviour
{
    private Variant chipVariant;

    public int Board_X;
    public int Board_Y;

    public Vector3 position;

    [Header("Base Colors")]
    public Color RED = Color.red;
    public Color GREEN = Color.green;
    public Color PINK = new Color(1f, 0.495283f, 0.8719454f);
    public Color PURPLE = new Color(0.7206894f, 0.25f, 1f);
    public Color YELLOW = Color.yellow;
    public Color ORANGE = new Color(1f, 0.4725827f, 0f);
    public Color BLUE = Color.blue;

    public void SetChipPosData(int x, int y, Vector3 position) {
        this.Board_X = x;
        this.Board_Y = y;
        this.position = position;
    }

    public void SetChipVariant(Variant newVariant)
    {
        this.chipVariant = newVariant;

        switch (newVariant)
        {
            case Variant.RED:
                GetComponent<SpriteRenderer>().color = RED;
                break;
            case Variant.GREEN:
                GetComponent<SpriteRenderer>().color = GREEN;
                break;
            case Variant.PINK:
                GetComponent<SpriteRenderer>().color = PINK;
                break;
            case Variant.PURPLE:
                GetComponent<SpriteRenderer>().color = PURPLE;
                break;
            case Variant.YELLOW:
                GetComponent<SpriteRenderer>().color = YELLOW;
                break;
            case Variant.ORANGE:
                GetComponent<SpriteRenderer>().color = ORANGE;
                break;
            case Variant.BLUE:
                GetComponent<SpriteRenderer>().color = BLUE;
                break;
        }
    }

    public bool MatchesWithChip(Chip compare) {
        return this.chipVariant == compare.chipVariant || chipVariant == Variant.RAINBOW || compare.chipVariant == Variant.RAINBOW;
    }

    public Variant GetChipVariant() {
        return chipVariant;
    }
}

public enum Variant
{
    NONE,
    RED,
    GREEN,
    PINK,
    PURPLE,
    YELLOW,
    ORANGE,
    BLUE,
    RAINBOW
}