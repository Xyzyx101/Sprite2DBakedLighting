﻿using UnityEngine;
using System.Linq;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class BakedLigthingSpriteData : MonoBehaviour
{
    public GameObject content;
    public Mesh mesh;
    
    public void BuildMesh()
    {
        if (content)
        {
            DestroyImmediate(content);
            content = null;
        }

        content = new GameObject("Mesh");
        content.isStatic = gameObject.isStatic;
        Transform holderTransform = content.transform;
        holderTransform.SetParent(transform);
        holderTransform.localPosition = Vector3.zero;
        holderTransform.localScale = Vector3.one;
        holderTransform.localRotation = Quaternion.identity;

        MeshFilter filter = content.AddComponent<MeshFilter>();
        MeshRenderer renderer = content.AddComponent<MeshRenderer>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = spriteRenderer.sprite;

        if (mesh)
        {
            DestroyImmediate(mesh);
            mesh = null;
        }

        if (sprite)
        {
            mesh = new Mesh();
            mesh.name = sprite.name + "_Mesh";
            mesh.vertices = sprite.vertices.Select(x => (Vector3)x).ToArray();
            mesh.uv = sprite.uv;
            mesh.triangles = sprite.triangles.Select(x => (int)x).ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            MaterialPropertyBlock block = new MaterialPropertyBlock();
            block.SetTexture("_MainTex", sprite.texture);
            block.SetColor("_Color", spriteRenderer.color);
            renderer.SetPropertyBlock(block);
            renderer.sharedMaterial = spriteRenderer.sharedMaterial;
            renderer.sortingLayerID = spriteRenderer.sortingLayerID;
            renderer.sortingOrder = spriteRenderer.sortingOrder;

            spriteRenderer.enabled = false;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(renderer);
#endif
        }

        filter.sharedMesh = mesh;
    }
    
    public void Clear()
    {
        if (content)
        {
            DestroyImmediate(content);
            content = null;
        }

        if (mesh)
        {
            DestroyImmediate(mesh);
            mesh = null;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;
    }
    
    public void TransferLightmapData()
    {
        if (!content)
        {
            return;
        }

        MeshRenderer renderer = content.GetComponent<MeshRenderer>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.lightmapScaleOffset = renderer.lightmapScaleOffset;
        spriteRenderer.lightmapIndex = renderer.lightmapIndex;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(spriteRenderer);
#endif
    }
}

