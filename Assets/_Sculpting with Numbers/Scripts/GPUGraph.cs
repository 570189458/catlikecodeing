using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GPUGraph : MonoBehaviour
{

    [SerializeField, Range(10, 200)]
    private int resolution = 10;

    // [SerializeField, Range(0, 2)]
    // private int function = 0;

    [SerializeField]
    private FunctionLibrary.FunctionName function = default;

    [SerializeField, Min(10f)] 
    private float functionDuration = 1f, transitionDuration = 1f;

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
    
    private ComputeBuffer positionsBuffer;

    [SerializeField]
    private Material _material = default;

    [SerializeField]
    private Mesh _mesh = default;

    [SerializeField]
    public ComputeShader _computeShader = default;

    private static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time"),
        scaleId = Shader.PropertyToID("_Scale");

    private void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);
    }

    private void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
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

        UpdateFunctionOnGPU();
    }

    void PickNextFunction()
    {
        function = transitionMode == TransitionMode.Circle
            ? FunctionLibrary.GetNextFunctionName(function)
            : FunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }
    
    
    void UpdateFunctionOnGPU()
    {
        float step = 2f / resolution;
        _computeShader.SetInt(resolutionId, resolution);
        _computeShader.SetFloat(stepId, step);
        _computeShader.SetFloat(timeId, Time.time);
        _computeShader.SetBuffer(0, positionsId, positionsBuffer);
        int groups = Mathf.CeilToInt(resolution / 8f);
        _computeShader.Dispatch(0, groups, groups, 1);
        _material.SetBuffer(positionsId, positionsBuffer);
        _material.SetFloat(stepId, step);
        _material.SetBuffer(positionsId, positionsBuffer);
        _material.SetVector(scaleId, new Vector4(step, 1f / step));
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        Graphics.DrawMeshInstancedProcedural(_mesh, 0, _material, bounds, positionsBuffer.count);
    }
}
