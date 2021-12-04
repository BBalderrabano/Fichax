using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingScoreDisplay : MonoBehaviour
{
    public GameObject endPosition;

    [System.NonSerialized]
    public Vector3 startPosition;

    private Vector3 middlePoint;

    private float timeElapsed;
    public float duration = 0.5f;

    private void Start()
    {
        startPosition = transform.position;

        //josh.position.x + (mark.position.x - josh.position.x) / 2;

        middlePoint = endPosition.transform.position - startPosition;
        middlePoint = new Vector3(middlePoint.x * UnityEngine.Random.Range(0.8f, 1.2f), middlePoint.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (timeElapsed < duration) {
            transform.position = QuadraticCurve(startPosition, middlePoint, endPosition.transform.position, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
        }
    }

    Vector2 QuadraticCurve(Vector2 a, Vector2 b, Vector2 c, float t) {
        Vector2 p1 = Lerp(a, b, t);
        Vector2 p2 = Lerp(b, c, t);
        return Lerp(p1, p2, t);
    }

    private Vector2 Lerp(Vector2 a, Vector2 b, float t)
    {
        return a + (b - a) * t;
    }
}
