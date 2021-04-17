using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    [SerializeField, Range(1, 8)]
    private int depth = 4;

    [SerializeField]
    private Mesh _mesh = default;

    [SerializeField]
    private Material _material = default;

    private static Vector3[] directions =
    {
        Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back
    };

    private static Quaternion[] rotations =
    {
        Quaternion.identity,
        Quaternion.Euler(0, 0, -90f), Quaternion.Euler(0, 0, 90f),
        Quaternion.Euler(90f, 0, 0), Quaternion.Euler(-90f, 0, 0),
    };

    struct FractalPart
    {
        public Vector3 direction, worldPosition;
        public Quaternion rotation, worldRotation;
        public float spinAngle;
    }

    private FractalPart[][] paras;
    private Matrix4x4[][] matrices;

    private void Awake()
    {
        paras = new FractalPart[depth][];
        matrices = new Matrix4x4[depth][];
        int size = 1;
        // float scale = 1f;
        for (int i = 0; i < paras.Length; i++)
        {
            paras[i] = new FractalPart[size];
            matrices[i] = new Matrix4x4[size];
            size *= 5;
        }
        paras[0][0] = CreatePart(0);
        for (int i = 1; i < paras.Length; i++)
        {
            // scale *= 0.5f;
            FractalPart[] levelParts = paras[i];
            for (int j = 0; j < levelParts.Length; j += 5)
            {
                for (int k = 0; k < 5; k++)
                {
                    levelParts[j + k] = CreatePart(k);
                }
            }
        }
    }

    FractalPart CreatePart(int childIndex)
    {
        // var go = new GameObject("Fractal Part" + levelIndex + " C" + childIndex);
        // go.transform.localScale = scale * Vector3.one;
        // go.transform.SetParent(transform, false);
        // go.AddComponent<MeshFilter>().mesh = _mesh;
        // go.AddComponent<MeshRenderer>().material = _material;
        return new FractalPart()
        {
            direction = directions[childIndex],
            rotation = rotations[childIndex]
            // transform = go.transform
        };
    }

    private void Update()
    {
        // Quaternion deltaRotation = Quaternion.Euler(0f, 22.5f * Time.deltaTime, 0f);
        float spinAngleDelta = 22.5f * Time.deltaTime;
        FractalPart rootPart = paras[0][0];
        // rootPart.rotation *= deltaRotation;
        rootPart.spinAngle += spinAngleDelta;
        rootPart.worldRotation = rootPart.rotation * Quaternion.Euler(0, rootPart.spinAngle, 0);
        paras[0][0] = rootPart;
        matrices[0][0] = Matrix4x4.TRS(rootPart.worldPosition, rootPart.worldRotation, Vector3.one);
        float scale = 1f;
        for (int i = 1; i < paras.Length; i++)
        {
            scale *= 0.5f;
            FractalPart[] parentParts = paras[i - 1];
            FractalPart[] levelParts = paras[i];
            Matrix4x4[] levelMatrices = matrices[i];
            for (int j = 0; j < levelParts.Length; j++)
            {
                // Transform parentTransform = parentParts[j / 5].transform;
                FractalPart parent = parentParts[j / 5];
                FractalPart part = levelParts[j];
                // part.rotation *= deltaRotation;
                parent.spinAngle += spinAngleDelta;
                part.worldRotation = parent.worldRotation * (part.rotation * Quaternion.Euler(0, part.spinAngle, 0));
                part.worldPosition =
                    parent.worldPosition +  parent.worldRotation * (1.5f * scale * part.direction);
                levelParts[j] = part;
                levelMatrices[j] = Matrix4x4.TRS(part.worldPosition, part.worldRotation, scale * Vector3.one);
            }
        }
    }

    // private void Start()
    // {
    //     name = "Fractal " + depth;
    //     if (depth <= 1)
    //         return;
    //     Fractal childA = CreateChild(Vector3.up, Quaternion.identity);
    //     Fractal childB = CreateChild(Vector3.right, Quaternion.Euler(0, 0, -90f));
    //     Fractal childC = CreateChild(Vector3.left, Quaternion.Euler(0, 0, 90f));
    //     Fractal childD = CreateChild(Vector3.back, Quaternion.Euler(-90f, 0, 0));
    //     Fractal childE = CreateChild(Vector3.forward, Quaternion.Euler(90f, 0, 0));
    //     
    //     childA.transform.SetParent(transform, false);
    //     childB.transform.SetParent(transform, false);
    //     childC.transform.SetParent(transform, false);
    //     childD.transform.SetParent(transform, false);
    //     childE.transform.SetParent(transform, false);
    // }
    //
    // private void Update()
    // {
    //     transform.Rotate(0, 22.5f * Time.deltaTime, 0f);
    // }
    //
    // Fractal CreateChild(Vector3 direction, Quaternion rotation)
    // {
    //     Fractal child = Instantiate(this);
    //     child.depth = depth - 1;
    //     child.transform.localPosition = 0.75f * direction;
    //     child.transform.localRotation = rotation;
    //     child.transform.localScale = 0.5f * Vector3.one;
    //     return child;
    // }
}
