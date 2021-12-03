using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayout))]
public class GridManager : MonoBehaviour
{
    [SerializeField]
    private int width, height;

    [SerializeField]
    private float cellSize = 250f;

    private GridLayoutGroup gridLayoutGroup;

    public Chip[,] board;

    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();

        gridLayoutGroup.constraintCount = width;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject go = Instantiate(prefab, transform);
                go.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
            }
        }
    }
}
