using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction
{
    Ally,
    Enemy
}

public static class GameTags
{
    public static readonly string Ally = Faction.Ally.ToString();
    public static readonly string Enemy = Faction.Enemy.ToString();
    public static readonly string EnemyNexus = "EnemyNexus";
    public static readonly string Ground = "Ground";
}
