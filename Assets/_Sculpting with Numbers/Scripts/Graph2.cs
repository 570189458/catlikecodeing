using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Graph2 : MonoBehaviour
{
    [SerializeField]
    private Transform pointPrefab = default;

    [SerializeField, Range(10, 200)]
    private int resolution = 10;

    // [SerializeField, Range(0, 2)]
    // private int function = 0;

    [SerializeField]
    private FunctionLibrary.FunctionName function = default;

    [SerializeField, Min(10f)] 
    private float functionDuration = 1f, transitionDuration = 1f;
    
    private Transform[] points;

    private float duration;

    private bool transitioning;
    private FunctionLibrary.FunctionName transitionFunction;
    
    public enum TransitionMode
    {
        Circle,
        Random
    }

    [SerializeField]
    private TransitionMode transitionMode = TransitionMode.Circle;

    private void Awake()
    {
        float step = 2f / resolution;
        // var position = Vector3.zero;
        var scale = Vector3.one * step;
        points = new Transform[resolution * resolution];
        for (int i = 0; i < points.Length; i++)
        {
            // if (x == resolution)
            // {
            //     x = 0;
            //     z += 1;
            // }

            Transform point = Instantiate(pointPrefab);
            // point.localPosition = Vector3.right * ((i + 0.5f) / 5f - 1f);
            // position.x = (x + 0.5f) * step - 1f;
            // position.z = (z + 0.5f) * step - 1f;
            // position.y = position.x * position.x * position.x;
            // point.localPosition = position;
            point.localScale = scale;
            points[i] = point;
            point.SetParent(transform, false);
        }
    }

    private void Update()
    {
        duration += Time.deltaTime;
        if (transitioning)
        {
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();
        }

        if (transitioning)
        {
            UpdateFunctionTransition();
        }
        else
        {
            UpdateFunction();
        }
    }

    void PickNextFunction()
    {
        function = transitionMode == TransitionMode.Circle
            ? FunctionLibrary.GetNextFunctionName(function)
            : FunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }

    void UpdateFunctionTransition()
    {
        FunctionLibrary.Function 
            from = FunctionLibrary.GetFunction(transitionFunction),
            to = FunctionLibrary.GetFunction(function);
        float progress = duration / transitionDuration;
        float time = Time.time;
        float step = 2f / resolution;
        float v = 0.5f * step - 1f;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;
            }

            float u = (x + 0.5f) * step - 1f;
            // float v = (z + 0.5f) * step - 1f;
            points[i].localPosition = FunctionLibrary.Morph(u, v, time, from, to, progress);
            // Transform point = points[i];
            // Vector3 position = point.localPosition;
            // if(function == 0)
            //     position.y = FunctionLibrary.Wave(position.x, time);
            // else if(function == 1)
            //     position.y = FunctionLibrary.MultiWave(position.x, time);
            // else
            //     position.y = FunctionLibrary.Ripple(position.x, time);
            // position.y = f(position.x, position.z, time);
            // point.localPosition = position;
        }
    }
    
    void UpdateFunction()
    {
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);
        float time = Time.time;
        float step = 2f / resolution;
        float v = 0.5f * step - 1f;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;
            }

            float u = (x + 0.5f) * step - 1f;
            // float v = (z + 0.5f) * step - 1f;
            points[i].localPosition = f(u, v, time);
            // Transform point = points[i];
            // Vector3 position = point.localPosition;
            // if(function == 0)
            //     position.y = FunctionLibrary.Wave(position.x, time);
            // else if(function == 1)
            //     position.y = FunctionLibrary.MultiWave(position.x, time);
            // else
            //     position.y = FunctionLibrary.Ripple(position.x, time);
            // position.y = f(position.x, position.z, time);
            // point.localPosition = position;
        }
    }
}
