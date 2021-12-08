using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayout))]
public class GridManager : MonoBehaviour
{
    [Header("Board Setups")]
    public GameObject chipPrefab;

    public GameObject flyingScorePrefab;

    public GameObject scoreContainer;

    [System.NonSerialized]
    public bool blockInteractions = false;

    [SerializeField]
    private int width, height;

    [SerializeField]
    private float cellSize = 250f;

    private GridLayoutGroup gridLayoutGroup;

    private List<GameObject> gridElements = new List<GameObject>();

    private Vector3[,] gridElementPositionCache;

    public Chip[,] board;

    [Space(10)]
    [Header("Sprite and Animations")]
    public float boardOpacity = 0.5f;

    public float chipFallSpeed = 1f;

    private int comboCounter = 1;
    private int multiCounter = 1;

    void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();

        gridLayoutGroup.constraintCount = width;
        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);

        board = new Chip[width, height];
        gridElementPositionCache = new Vector3[width, height];

        currentChain = new List<Chip>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject go = Instantiate(chipPrefab, transform);

                go.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, boardOpacity);

                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 100);

                Chip chip = go.GetComponent<Chip>();

                chip.Board_X = x;
                chip.Board_Y = y;

                gridElements.Add(go);
            }
        }

        UpdateGrid(gridLayoutGroup);

        foreach (GameObject go in gridElements)
        {
            Chip chip = go.GetComponent<Chip>();

            gridElementPositionCache[chip.Board_X, chip.Board_Y] = go.transform.position;
        }
    }

    public void UpdateGrid(LayoutGroup gridLayoutGroup)
    {
        gridLayoutGroup.CalculateLayoutInputHorizontal();
        gridLayoutGroup.CalculateLayoutInputVertical();
        gridLayoutGroup.SetLayoutHorizontal();
        gridLayoutGroup.SetLayoutVertical();
    }

    public bool DropAtPosition(Transform chipPicker) {

        bool isValid = false;

        List<ValidPosition> validPositions = new List<ValidPosition>();

        for (int i = 0; i < chipPicker.childCount; i++)
        {
            Transform child = chipPicker.GetChild(i);

            GetGridPosition(child.position, out int x, out int y, out Vector3 gridPosition);

            if (x >= 0 && y >= 0 && board[x,y] == null)
            {
                isValid = true;

                validPositions.Add(new ValidPosition(x, y, gridPosition));
            }
            else
            {
                return false;
            }
        }

        if (isValid) {

            for (int i = chipPicker.childCount - 1; i >= 0 ; i--)
            {
                Transform child = chipPicker.GetChild(i);
                ValidPosition validPos = validPositions[i];
                Chip childChip = child.GetComponent<Chip>();

                child.GetComponent<LayoutElement>().ignoreLayout = true;

                child.SetParent(transform);

                child.transform.position = validPos.position;
                child.transform.localPosition = new Vector3(child.transform.localPosition.x, child.transform.localPosition.y, 0);

                board[validPos.grid_x, validPos.grid_y] = childChip;

                childChip.SetChipPosData(validPos.grid_x, validPos.grid_y, validPos.position);
            }

            UpdateBoard();
        }

        return isValid;
    }

    private int fallingAmount = 0;

    public void UpdateBoard() {

        fallingAmount = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (board[x, y] == null) {
                    continue;
                }

                Chip chip = board[x, y];

                int target_y = -1;

                IsChipFloating(x, y, out bool isFloating, out target_y);

                if (isFloating) {
                    blockInteractions = true;
                    fallingAmount++;

                    board[x, y] = null;
                    board[x, target_y] = chip;

                    chip.SetChipPosData(x, target_y, gridElementPositionCache[x, target_y]);

                    LeanTween.moveY(chip.gameObject, gridElementPositionCache[x, target_y].y, Mathf.Abs(target_y - y) / chipFallSpeed)
                        .setOnComplete(()=> {
                            FallingEnded();
                        })
                        .setEaseOutBounce();
                }
            }
        }

        if (fallingAmount == 0)
            CheckForMatches();
    }

    void FallingEnded() {
        fallingAmount--;

        if (fallingAmount <= 0)
            CheckForMatches();
    }

    void IsChipFloating(int x, int y, out bool isFloating, out int target_y) {
        isFloating = false;
        target_y = y;

        int yOffset = -1;

        while (y + yOffset >= 0 && board[x, y + yOffset] == null) {
            isFloating = true;
            target_y = y + yOffset;
            yOffset--;
        }
    }

    private List<Chip> currentChain;

    void CheckForMatches() {
        List<Chip> alreadyChecked = new List<Chip>();
        List<Chip> matchedThisTurn = new List<Chip>();

        int matched = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (board[x, y] == null || alreadyChecked.Contains(board[x,y]))
                    continue;

                List<Chip> currentCheck = CheckForMatches(x, y);

                alreadyChecked.AddRange(currentCheck);

                if (currentCheck.Count >= PlayerData.GET_AMOUNT_TO_MATCH()) {
                    matched++;

                    matchedThisTurn.AddRange(currentCheck);
                }

                currentCheck.Clear();
            }
        }

        if (matched > 0)
        {
            if (matched > 1) {
                multiCounter = matched;
            }

            foreach (Chip chip in matchedThisTurn)
            {
                board[chip.Board_X, chip.Board_Y] = null;

                int score = chip.OnMatch(matchedThisTurn, currentChain, multiCounter, comboCounter);

                CreateFlyingScore(chip, score);
            }

            comboCounter++;

            UpdateBoard();
        }
        else {
            comboCounter = 1;
            multiCounter = 1;
            currentChain.Clear();

            blockInteractions = false;
        }
    }

    void CreateFlyingScore(Chip chip, int score) {
        GameObject flyingDisplay = Instantiate(flyingScorePrefab, chip.position, Quaternion.identity, transform.parent);

        FlyingScoreDisplay scoreDisplay = flyingDisplay.GetComponent<FlyingScoreDisplay>();

        scoreDisplay.manager = this;
        scoreDisplay.score = score;
        scoreDisplay.endPosition = scoreContainer;

        chip.transform.SetParent(flyingDisplay.transform);
        chip.transform.GetComponent<SpriteRenderer>().sortingOrder = 9;

        LeanTween.scale(chip.gameObject, chip.transform.localScale * 0.3f, 1f).setDelay(0.1f).setEaseOutCirc();

        Gradient newTrailColor = new Gradient();

        GradientColorKey[] gColorKey;
        GradientAlphaKey[] gAlphaKey;

        gColorKey = new GradientColorKey[3];
        gColorKey[0].color = chip.gameObject.GetComponent<SpriteRenderer>().color;
        gColorKey[0].time = 0.0f;
        gColorKey[1].color = chip.gameObject.GetComponent<SpriteRenderer>().color;
        gColorKey[1].time = 0.8f;
        gColorKey[2].color = Color.white;
        gColorKey[2].time = 1.0f;

        gAlphaKey = new GradientAlphaKey[3];
        gAlphaKey[0].alpha = 1.0f;
        gAlphaKey[0].time = 0.0f;
        gAlphaKey[1].alpha = 1.0f;
        gAlphaKey[1].time = 0.5f;
        gAlphaKey[2].alpha = 0.0f;
        gAlphaKey[2].time = 1.0f;

        newTrailColor.SetKeys(gColorKey, gAlphaKey);

        flyingDisplay.GetComponent<TrailRenderer>().colorGradient = newTrailColor;
    }

    List<Chip> CheckForMatches(int x, int y) {
        Chip pointer = board[x, y];

        List<Chip> possible = new List<Chip>();

        List<Chip> toCheck = new List<Chip>() { pointer };

        while (pointer != null) {

            if (!possible.Contains(pointer)) {

                toCheck.AddRange(CheckArround(pointer));

                possible.Add(pointer);
            }

            toCheck.Remove(pointer);

            if (toCheck.Count > 0)
                pointer = toCheck.First();
            else
                pointer = null;
        }

        return possible;
    }

    List<Chip> CheckArround(Chip target) {
        List<Chip> matches = new List<Chip>();

        for (int xOffset = -1; xOffset <= 1; xOffset++)
        {
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                if (target.Board_X + xOffset < 0 || target.Board_X + xOffset > (width - 1) || target.Board_Y + yOffset < 0 || target.Board_Y + yOffset > (height - 1))
                    continue;

                //Skip Diagonals
                if (xOffset != 0 && yOffset != 0)
                    continue;

                Chip compare = board[target.Board_X + xOffset, target.Board_Y + yOffset];

                if (compare == null || compare == target)
                    continue;

                if (target.MatchesWithChip(compare)) {
                    matches.Add(compare);
                }
            }
        }
        return matches;
    }

    public void GetGridPosition(Vector3 pos, out int grid_x, out int grid_y, out Vector3 gridPosition)
    {
        grid_x = -1;
        grid_y = -1;
        gridPosition = Vector3.zero;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 check = gridElementPositionCache[x, y];

                float distance = Vector2.Distance(check, pos);

                if (distance < 0.5f)
                {
                    grid_x = x;
                    grid_y = y;
                    gridPosition = check;

                    return;
                }
            }
        }
    }

    public void AddScore(int amount) {
        Debug.Log(amount);
    }
}

class ValidPosition {

    public int grid_x;
    public int grid_y;
    public Vector3 position;

    public ValidPosition(int grid_x, int grid_y, Vector3 position) {
        this.grid_x = grid_x;
        this.grid_y = grid_y;
        this.position = new Vector3(position.x, position.y, 0);
    }
}
