using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Fraction
{
    Ally,
    Enemy
}

public static class GameTags
{
    public static readonly string Ally = Fraction.Ally.ToString();
    public static readonly string Enemy = Fraction.Enemy.ToString();
    public static readonly string EnemyNexus = "EnemyNexus";
    public static readonly string Ground = "Ground";
}
