using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class VertexCounter : MonoBehaviour
{
    void Start()
    {
        // Lista wyników (nazwa obiektu + liczba vertexów)
        List<(string name, int vertices)> results = new List<(string, int)>();

        // Pobieramy wszystkie obiekty w scenie
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            int vertexCount = 0;

            // MeshFilter (statyczne modele)
            MeshFilter mf = obj.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                vertexCount += mf.sharedMesh.vertexCount;
            }

            // SkinnedMeshRenderer (postacie / animowane modele)
            SkinnedMeshRenderer smr = obj.GetComponent<SkinnedMeshRenderer>();
            if (smr != null && smr.sharedMesh != null)
            {
                vertexCount += smr.sharedMesh.vertexCount;
            }

            if (vertexCount > 0)
            {
                results.Add((obj.name, vertexCount));
            }
        }

        // Sortowanie malej¹co
        var sorted = results.OrderByDescending(r => r.vertices);

        // Wypisanie
        Debug.Log("=== Vertex Count (od najciê¿szego do najl¿ejszego) ===");
        foreach (var r in sorted)
        {
            Debug.Log($"{r.name}: {r.vertices} vertexów");
        }
    }
}
