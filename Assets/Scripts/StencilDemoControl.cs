using System;
using UnityEngine;

public class StencilDemoControl : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector3 startingPosition;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float moveDuration;
    [SerializeField] private AnimationCurve movementCurve;

    [Header("Scale")] 
    [SerializeField] private Vector3 startingScale;
    [SerializeField] private Vector3 targetScale;
    [SerializeField] private float scaleDuration;
    [SerializeField] private AnimationCurve scaleCurve;

    private bool movementCompleted;
    private float movementCompleteTime;

    private void Start()
    {
        transform.position = startingPosition;
        transform.localScale = startingScale;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(startingPosition, targetPosition, 
            movementCurve.Evaluate(Time.time / moveDuration));

        if (!movementCompleted && Time.time / moveDuration >= 1.0f)
        {
            movementCompleted = true;
            movementCompleteTime = Time.time;
        }

        if (Time.time / moveDuration >= 1.0f)
        {
            transform.localScale = Vector3.Lerp(startingScale, targetScale, 
                scaleCurve.Evaluate((Time.time - movementCompleteTime) / scaleDuration));
        }
        
        if (Time.time - movementCompleteTime >= scaleDuration)
        {
            Destroy(this);
        }
    }
}