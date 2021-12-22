using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Symbol : ScriptableObject
{
    public RARITY baseRarity;

    public string symbolName;

    [TextArea(3, 10)]
    public string symbolDescription;

    public Sprite symbolIcon;

    [System.NonSerialized]
    public Chip chip;

    public int score;

    public virtual int OnMatch(List<Chip> currentMatch, List<Chip> currentChain, int multiCounter, int comboCounter) {
        return score;
    }

    public virtual void OnChipGeneration(Chip chip) {
        this.chip = chip;
    }

    public virtual void OnEnterBoard(int x, int y) { }

    public virtual List<Symbol> OnExitBoard(List<Symbol> allSymbols) {
        chip = null;
        return allSymbols;
    }
}


