using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CodeMonkey.Utils;
using System.Linq;
using System;

public class ChipPicker : MonoBehaviour
{
    public GridManager manager;

    private bool blockInteractions = false;

    private bool BlockingInteractions() { return blockInteractions || manager.blockInteractions; }

    private Vector3 startPosition;

    [SerializeField]
    private float rotationDuration = 0.25f;
    private int rotateTimes = 0;

    private bool IsDragging() { return dragTime >= dragTreshold && !BlockingInteractions(); }

    private bool dragging = false;
    private float dragTreshold = 0.25f;
    private float dragTime = 0f;

    public float spawnAnimationDuration = 1f;

    private void Awake()
    {
        startPosition = transform.position;
    }

    void OnMouseDown()
    {
        dragging = true;
    }

    void OnMouseUp()
    {
        if (!IsDragging())
        {
            rotateTimes++;
        }
        else
        {
            bool isValid = manager.DropAtPosition(transform);

            if (!isValid)
            {
                LeanTween.move(gameObject, startPosition, 0.4f)
                    .setOnStart(() =>
                    {
                        blockInteractions = true;
                    })
                    .setOnComplete(() =>
                    {
                        blockInteractions = false;
                    })
                    .setEaseOutQuart();
            }
            else {

                transform.position = startPosition;
            }
        }

        dragTime = 0;
        dragging = false;
    }

    private void Update()
    {
        if(rotateTimes > 0 && !BlockingInteractions())
        {
            blockInteractions = true;

            LeanTween.rotateZ(gameObject, gameObject.transform.eulerAngles.z - 90f, rotationDuration)
                .setOnStart(()=> {
                    foreach (Transform child in transform)
                    {
                        LeanTween.rotateLocal(child.gameObject, new Vector3(0, 0, child.localEulerAngles.z + 90f), rotationDuration);
                    }
                })
                .setOnComplete(() => {
                    blockInteractions = false;
                    rotateTimes = Mathf.Max(0, rotateTimes - 1);
                });
        }

        if (dragging)
        {
            dragTime += Time.deltaTime;
        }

        if (IsDragging())
        {
            Vector3 mousePos = UtilsClass.GetMouseWorldPosition();
            Vector3 targetPos = new Vector3(mousePos.x, mousePos.y + 1);

            transform.position = Vector3.Lerp(transform.position, targetPos, 10f * Time.deltaTime);

        }
        else if (!BlockingInteractions() && transform.childCount == 0) {
            CreateNewChips();
        }
    }

    void CreateNewChips(int amount = 2) {

        List<Variant> all = Enum.GetValues(typeof(Variant)).Cast<Variant>().ToList();

        List<Variant> exclude = new List<Variant>() { Variant.NONE, Variant.RAINBOW };

        for (int i = 0; i < amount; i++)
        {
            List<Variant> probable = all.Except(exclude).ToList();

            GameObject go = Instantiate(manager.chipPrefab, transform);

            go.LeanScale(go.transform.localScale, spawnAnimationDuration).setFrom(go.transform.localScale * 1.4f).setEaseOutExpo();
            go.LeanAlpha(1f, spawnAnimationDuration * 0.5f).setFrom(0f);

            Chip next = go.GetComponent<Chip>();

            next.SetChipVariant(probable[UnityEngine.Random.Range(0, probable.Count)]);

            exclude.Add(next.GetChipVariant());
        }
    }
}
