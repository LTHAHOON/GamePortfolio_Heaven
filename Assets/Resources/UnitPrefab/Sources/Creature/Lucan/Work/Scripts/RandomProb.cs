using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomProb
{
    public T Choose<T>(Dictionary<T, float> probs)
    {
        float total = 0;

        foreach (float elem in probs.Values)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;
        for (int i = 0; i < probs.Count; i++)
        {
            if (randomPoint < probs.ElementAt(i).Value)
            {
                return probs.ElementAt(i).Key;
            }
            else
            {
                randomPoint -= probs.ElementAt(i).Value;
            }
        } 
        return GetKeyOfMaxVaule<T>(probs);
    }
    public T GetKeyOfMaxVaule<T>(Dictionary<T, float> probs)
    {
        T key = default;
        float maxValue = float.MinValue;
        foreach(var prob in probs)
        {
            if(maxValue < prob.Value)
            {
                maxValue = prob.Value;
                key = prob.Key;
            }
        }
        return key;
    } 
}

