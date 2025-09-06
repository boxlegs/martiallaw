﻿using BepInEx;
using BepInEx.Logging;
using BlackMagicAPI.Managers;
using FishNet;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using HarmonyLib;

//Rename this to match the name of your mod, This needs to match the RootNamespace in the `.csproj` so edit that as well.
// e.g. <RootNamespace>ModTemplate</RootNamespace>
namespace MartialLawSpell;

// Ensure that BepInEx only loads your mod DLL into Mage Arena


[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInProcess("MageArena")]
[BepInDependency("com.magearena.modsync", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.d1gq.black.magic.api", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.d1gq.fish.utilities", BepInDependency.DependencyFlags.HardDependency)]

public class MartialLawSpell : BaseUnityPlugin
{
    

    internal static MartialLawSpell Instance { get; private set; }

    internal static new ManualLogSource Logger { get; private set; }

    private const string VersionString = "1.0.2";
    private static Harmony? Harmony;
    public static string modsync = "all";


    private void Awake()
    {

        Logger = base.Logger;
        Instance = this;
        Harmony = new(PluginInfo.PLUGIN_GUID);
        Harmony.PatchAll();

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} ({PluginInfo.PLUGIN_GUID}) is loading...");

        BlackMagicManager.RegisterSpell(this, typeof(MartialLawSpellData), typeof(MartialLawSpellLogic));

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} ({PluginInfo.PLUGIN_GUID}) loaded successfully.");
    }
}
