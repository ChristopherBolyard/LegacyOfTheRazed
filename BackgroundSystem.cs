// Assets/Scripts/BackgroundSystem.cs
using UnityEngine;
using System.Collections.Generic;

public static class BackgroundSystem
{
    private static readonly Dictionary<BackgroundType, BackgroundDefinition> definitions = new()
    {
        {
            BackgroundType.MilitaryFamily,
            new BackgroundDefinition
            {
                type = BackgroundType.MilitaryFamily,
                title = "Stellar Guard Progeny",
                description = "Born to world leaders. Start with StellarGuard rep + Earth element.",
                startingFaction = FactionType.StellarGuard,
                startingCredits = 150
            }
        },
        {
            BackgroundType.Orphaned,
            new BackgroundDefinition
            {
                type