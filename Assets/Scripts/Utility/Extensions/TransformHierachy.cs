// TransformHierarchyExtensions

using UnityEngine;

public static class TransformHierarchyExtensions
{
    public static void DestroyChildren(this Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            var child = parent.GetChild(i);
            Object.Destroy(child.gameObject);
        }
    }

    public static Transform FindInChildren(this Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;

            var result = child.FindInChildren(name);
            if (result != null) return result;
        }

        return null;
    }

    public static void SetLayerRecursively(this Transform parent, int layer)
    {
        parent.gameObject.layer = layer;
        foreach (Transform child in parent)
        {
            child.SetLayerRecursively(layer);
        }
    }
}