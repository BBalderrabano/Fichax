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
    public GameObject prefab;

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

    public float chipFallDuration = 0.4f;

    void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();

        gridLayoutGroup.constraintCount = width;
        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);

        board = new Chip[width, height];
        gridElementPositionCache = new Vector3[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject go = Instantiate(prefab, transform);

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

                child.GetComponent<LayoutElement>().ignoreLayout = true;

                child.SetParent(transform);

                child.transform.position = validPos.position;
                child.transform.localPosition = new Vector3(child.transform.localPosition.x, child.transform.localPosition.y, 0);

                board[validPos.grid_x, validPos.grid_y] = child.GetComponent<Chip>();
            }

            UpdateBoard();
        }

        return isValid;
    }

    public void UpdateBoard() {
        
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

                    board[x, y] = null;
                    board[x, target_y] = chip;

                    LeanTween.moveY(chip.gameObject, gridElementPositionCache[x, target_y].y, chipFallDuration)
                        .setOnComplete(()=> {
                            blockInteractions = false;
                        })
                        .setEaseOutBounce();
                }
            }
        }
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

                if (distance < 0.4f)
                {
                    grid_x = x;
                    grid_y = y;
                    gridPosition = check;

                    return;
                }
            }
        }
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
