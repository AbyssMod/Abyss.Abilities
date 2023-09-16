using System;
using Abyss.Api.Abilities;
using BepInEx;
using JetBrains.Annotations;
using UnityEngine;

namespace Abyss;

[BepInPlugin(Id, Name, Version)]
[UsedImplicitly]
internal class Abilities : DredgeMod
{
    private const string Id = "com.grahamkracker.abyss.abilities";
    private const string Name = "Abyss.Abilities";
    private const string Version = "0.0.1";

    public static GameObject AbilityManager { get; private set; } = null!;

    private void Awake()
    {
        AbilityManager = new GameObject("Abyss.AbilityManager");
        DontDestroyOnLoad(AbilityManager);
    }
}