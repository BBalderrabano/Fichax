using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Relic : ScriptableObject
{
    public RARITY baseRarity;

    public string relicName;

    [TextArea(3, 10)]
    public string relicDescription;

    public Sprite relicIcon;

    public abstract void OnPickUp();
}
