using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chip : MonoBehaviour
{
    public Variant Chip_Variant;

    public int Board_X;
    public int Board_Y;

    public void ChangeVariant(Variant newVariant) {

        this.Chip_Variant = newVariant;

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
    BLUE
}