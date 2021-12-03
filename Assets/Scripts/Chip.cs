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

    public void SetChipVariant(Variant newVariant) {
        this.chipVariant = newVariant;

        switch (newVariant)
        {
            case Variant.RED:
                GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case Variant.GREEN:
                GetComponent<SpriteRenderer>().color = Color.green;
                break;
            case Variant.PINK:
                GetComponent<SpriteRenderer>().color = new Color(1f, 0.495283f, 0.8719454f);
                break;
            case Variant.PURPLE:
                GetComponent<SpriteRenderer>().color = new Color(0.7206894f, 0.25f, 1f);
                break;
            case Variant.YELLOW:
                GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
            case Variant.ORANGE:
                GetComponent<SpriteRenderer>().color = new Color(1f, 0.4725827f, 0f);
                break;
            case Variant.BLUE:
                GetComponent<SpriteRenderer>().color = Color.blue;
                break;
        }
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