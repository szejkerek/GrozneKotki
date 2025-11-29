using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public static class ListExtensions
{
    public static List<T> SelectRandomElements<T>(this List<T> list, int count)
    {
        List<T> copiedList = new List<T>(list);
        List<T> selectedElements = new List<T>();

        if (selectedElements.Count > count)
        {
            Debug.LogWarning("Number of selected elements is greater than given list!");
            return copiedList;
        }

        while (selectedElements.Count < count)
        {
            int randomIndex = UnityEngine.Random.Range(0, copiedList.Count);
            selectedElements.Add(copiedList[randomIndex]);
            copiedList.RemoveAt(randomIndex);
        }

        return selectedElements;
    }

    public static T SelectRandomElement<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("The list is empty or null.");
        }

        int randomIndex = UnityEngine.Random.Range(0, list.Count);
        return list[randomIndex];
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

}
