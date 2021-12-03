using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CodeMonkey.Utils;

public class ChipPicker : MonoBehaviour
{
    private bool blockingInteractions = false;

    private Vector3 startPosition;

    [SerializeField]
    private float rotationDuration = 0.25f;
    private int rotateTimes;

    private bool isDragging() { return dragTime >= dragTreshold && !blockingInteractions; }

    private bool dragging = false;
    private float dragTreshold = 0.25f;
    private float dragTime = 0f;

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
        if (!isDragging())
        {
            rotateTimes++;
        }
        else
        {
            LeanTween.move(gameObject, startPosition, 0.4f)
                .setOnStart(() => {
                    blockingInteractions = true;
                })
                .setOnComplete(() => {
                    blockingInteractions = false;
                })
                .setEaseOutQuart();
        }

        dragTime = 0;
        dragging = false;
    }

    private void Update()
    {
        if(rotateTimes > 0 && !blockingInteractions)
        {
            LeanTween.rotateZ(gameObject, gameObject.transform.eulerAngles.z - 90f, rotationDuration)
                .setOnStart(()=> {
                    blockingInteractions = true;

                    foreach (Transform child in transform)
                    {
                        LeanTween.rotateLocal(child.gameObject, new Vector3(0, 0, child.localEulerAngles.z + 90f), rotationDuration);
                    }
                })
                .setOnComplete(() => {
                    blockingInteractions = false;
                    rotateTimes = Mathf.Max(0, rotateTimes - 1);
                });
        }

        if (dragging)
        {
            dragTime += Time.deltaTime;
        }

        if (isDragging())
        {
            Vector3 mousePos = UtilsClass.GetMouseWorldPosition();
            Vector3 targetPos = new Vector3(mousePos.x, mousePos.y + 1);

            transform.position = Vector3.Lerp(transform.position, targetPos, 10f *Time.deltaTime);
        }
    }
}
