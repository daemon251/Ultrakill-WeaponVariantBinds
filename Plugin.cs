using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;

using UnityEngine;
using HarmonyLib;
using System.Runtime.CompilerServices;
using System;
using UnityEngine.SocialPlatforms;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace WeaponVariantBinds;

//TO DO:
//organize code
//
//unequipped weapons cause problems?

[BepInPlugin("WeaponVariantBinds", "WeaponVariantBinds", "0.01")]
public class Plugin : BaseUnityPlugin
{
    public static bool modEnabled = true;
    public static GameObject weapon = null;

    public static bool scrollEnabled = false;
    public static bool scrollVariation = false;
    public static bool scrollReversed = false;
    public static bool scrollWeapons = false;

    private void Awake()
    {
        PluginConfig.WeaponVariantBindsConfig();
        Harmony harmony = new Harmony("WeaponVariantBinds");
        harmony.PatchAll();
        Logger.LogInfo("Plugin WeaponVariantBinds is loaded!");
    }

    public static bool IsGameplayScene() //copied from UltraTweaker
    {
        string[] NonGameplay = {"Intro","Bootstrap","Main Menu","Level 2-S","Intermission1","Intermission2"};
        if(SceneHelper.CurrentScene == null) {return false;}
        return !NonGameplay.Contains(SceneHelper.CurrentScene);
    }
    public static bool IsMenu()
    {
        if(!MonoSingleton<OptionsManager>.Instance.paused && !MonoSingleton<FistControl>.Instance.shopping && !GameStateManager.Instance.PlayerInputLocked)
        {
            return false;
        }
        return true;
    }

    WeaponCycle[] wcArray = new WeaponCycle[2]; //0 is last wc, 1 is new wc

    int newSlot = -1;
    int newVariation = -1;

    public void SwitchToNextCustomSlot()
    {
        WeaponCycle wcTemp = wcArray[1];
        int index1 = 0;
        for(int i = 0; i < PluginConfig.weaponCycles.Length; i++)
        {
            if(wcTemp == PluginConfig.weaponCycles[i])
            {
                index1 = i + 1;
                if(index1 >= PluginConfig.weaponCycles.Length)
                {
                    index1 = 0;
                }
            }
        }
        for(int i = 0; i < PluginConfig.weaponCycles.Length; i++)
        {
            bool emptyArray = true;
            for(int j = 0; j < PluginConfig.weaponCycles[index1].weaponEnums.Length; j++)
            {
                if(PluginConfig.weaponCycles[index1].weaponEnums[j] != PluginConfig.WeaponEnum.None)
                {
                    emptyArray = false;
                }
            }
            if(emptyArray)
            {
                index1++;
                if(index1 >= PluginConfig.weaponCycles.Length)
                {
                    index1 = 0;
                }
            }
            else {break;}
        }

        wcArray[0] = wcArray[1];
        wcArray[1] = PluginConfig.weaponCycles[index1];

        WeaponCycle wc = wcArray[1];
        if(wc.rememberVariation == false) {wc.currentIndex = 0;}
        for (int j = 0; j < wc.weaponEnums.Length; j++)
        {
            if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None) {wc.currentIndex += 1;}
            if(wc.currentIndex >= wc.weaponEnums.Length) {wc.currentIndex = 0;}
        }
        int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
        newSlot = arr[0]; newVariation = arr[1];
    }

    public void SwitchToPrevCustomSlot()
    {
        WeaponCycle wcTemp = wcArray[1];
        int index1 = 0;
        for(int i = 0; i < PluginConfig.weaponCycles.Length; i++)
        {
            if(wcTemp == PluginConfig.weaponCycles[i])
            {
                index1 = i - 1;
                if(index1 < 0)
                {
                    index1 = PluginConfig.weaponCycles.Length - 1;
                }
            }
        }
        for(int i = 0; i < PluginConfig.weaponCycles.Length; i++)
        {
            bool emptyArray = true;
            for(int j = 0; j < PluginConfig.weaponCycles[index1].weaponEnums.Length; j++)
            {
                if(PluginConfig.weaponCycles[index1].weaponEnums[j] != PluginConfig.WeaponEnum.None)
                {
                    emptyArray = false;
                }
            }
            if(emptyArray)
            {
                index1 += -1;
                if(index1 < 0)
                {
                    index1 = PluginConfig.weaponCycles.Length - 1;
                }
            }
            else {break;}
        }

        wcArray[0] = wcArray[1];
        wcArray[1] = PluginConfig.weaponCycles[index1];

        WeaponCycle wc = wcArray[1];
        if(wc.rememberVariation == false) {wc.currentIndex = 0;}
        for (int j = 0; j < wc.weaponEnums.Length; j++)
        {
            if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None) {wc.currentIndex += 1;}
            if(wc.currentIndex >= wc.weaponEnums.Length) {wc.currentIndex = 0;}
        }
        int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
        newSlot = arr[0]; newVariation = arr[1];
    }

    public void VariationBindLogic()
    {
        if(MonoSingleton<InputManager>.Instance.InputSource.Actions.Weapon.VariationSlot1.WasPressedThisFrame()) 
        {
            WeaponCycle wc = wcArray[1];
            wc.currentIndex = 0;
            for (int j = 0; j < wc.weaponEnums.Length; j++)
            {
                if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None) {wc.currentIndex += 1;}
                if(wc.currentIndex >= wc.weaponEnums.Length) {wc.currentIndex = 0;}
            }
            int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
            newSlot = arr[0]; newVariation = arr[1];
        }
        if(MonoSingleton<InputManager>.Instance.InputSource.Actions.Weapon.VariationSlot2.WasPressedThisFrame()) 
        {
            WeaponCycle wc = wcArray[1];
            wc.currentIndex = 1;
            for (int j = 0; j < wc.weaponEnums.Length; j++)
            {
                if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None) {wc.currentIndex += 1;}
                if(wc.currentIndex >= wc.weaponEnums.Length) {wc.currentIndex = 0;}
            }
            int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
            newSlot = arr[0]; newVariation = arr[1];
        }
        if(MonoSingleton<InputManager>.Instance.InputSource.Actions.Weapon.VariationSlot3.WasPressedThisFrame()) 
        {
            WeaponCycle wc = wcArray[1];
            wc.currentIndex = 2;
            for (int j = 0; j < wc.weaponEnums.Length; j++)
            {
                if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None) {wc.currentIndex += 1;}
                if(wc.currentIndex >= wc.weaponEnums.Length) {wc.currentIndex = 0;}
            }
            int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
            newSlot = arr[0]; newVariation = arr[1];
        }
    }

    public void ScrollLogic()
    {
        //implement scrollWeapons
        if(wcArray[1] != null && wcArray[1].scrollThroughWeaponCycle == true) //slightly buggy?
        {
            if(MonoSingleton<PrefsManager>.Instance.prefMap.ContainsKey("scrollEnabled")) {scrollEnabled =   (bool)MonoSingleton<PrefsManager>.Instance.prefMap["scrollEnabled"];}
            if(MonoSingleton<PrefsManager>.Instance.prefMap.ContainsKey("scrollVariations")) {scrollVariation = (bool)MonoSingleton<PrefsManager>.Instance.prefMap["scrollVariations"];}
            if(MonoSingleton<PrefsManager>.Instance.prefMap.ContainsKey("scrollReversed")) {scrollReversed =  (bool)MonoSingleton<PrefsManager>.Instance.prefMap["scrollReversed"];}
            if(MonoSingleton<PrefsManager>.Instance.prefMap.ContainsKey("scrollWeapons")) {scrollWeapons =  (bool)MonoSingleton<PrefsManager>.Instance.prefMap["scrollWeapons"];}

            float mult = 1f;
            if(scrollReversed){mult = -1f;}
            float value = Input.GetAxis("Mouse ScrollWheel") * mult; 
            if(scrollEnabled && scrollVariation)
            {
                if(value >= 0.1f)
                {
                    WeaponCycle wc = wcArray[1];
                    wc.currentIndex += 1;
                    for (int i = 0; i < wc.weaponEnums.Length; i++)
                    {
                        if(wc.currentIndex >= wc.weaponEnums.Length) 
                        {
                            wc.currentIndex = 0;
                            if(scrollWeapons)
                            {
                                SwitchToNextCustomSlot();
                                break;
                            }
                        }
                        if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None) {wc.currentIndex += 1;}
                    }
                    int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
                    newSlot = arr[0]; newVariation = arr[1];
                }
                if(value <= -0.1f)
                {
                    WeaponCycle wc = wcArray[1];
                    wc.currentIndex += -1;
                    for (int i = 0; i < wc.weaponEnums.Length; i++)
                    {
                        if(wc.currentIndex < 0) 
                        {
                            wc.currentIndex = wc.weaponEnums.Length - 1;
                            if(scrollWeapons)
                            {
                                SwitchToPrevCustomSlot();
                                break;
                            }
                        }
                        if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None) {wc.currentIndex += -1;}
                    }
                    int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
                    newSlot = arr[0]; newVariation = arr[1];
                }
            }

            if(scrollEnabled && !scrollVariation && scrollWeapons)
            {
                if(value >= 0.1f)
                {
                    SwitchToNextCustomSlot();
                }
                if(value <= -0.1f)
                {
                    SwitchToPrevCustomSlot();
                }
            }
        }
    }

    public void AlterVariationLogic()
    {
        if(MonoSingleton<InputManager>.Instance.InputSource.NextVariation.WasPerformedThisFrame)
        {
            WeaponCycle wc = wcArray[1];
            wc.currentIndex += 1;
            for (int i = 0; i < wc.weaponEnums.Length; i++)
            {
                if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None) {wc.currentIndex += 1;}
                if(wc.currentIndex >= wc.weaponEnums.Length) {wc.currentIndex = 0;}
            }
            int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
            newSlot = arr[0]; newVariation = arr[1];
        }
        if(MonoSingleton<InputManager>.Instance.InputSource.PreviousVariation.WasCanceledThisFrame)
        {
            WeaponCycle wc = wcArray[1];
            wc.currentIndex += -1;
            for (int i = 0; i < wc.weaponEnums.Length; i++)
            {
                if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None) {wc.currentIndex += -1;}
                if(wc.currentIndex < 0) {wc.currentIndex = wc.weaponEnums.Length - 1;}
            }
            int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
            newSlot = arr[0]; newVariation = arr[1];
        }
    }

    public void AlterWeaponLogic()
    {
        if(MonoSingleton<InputManager>.Instance.InputSource.NextWeapon.WasCanceledThisFrame)
        {
            SwitchToNextCustomSlot();
        }

        if(MonoSingleton<InputManager>.Instance.InputSource.PrevWeapon.WasCanceledThisFrame)
        {
            SwitchToPrevCustomSlot();
        }
    }

    public void LastWeaponLogic()
    {
        if(wcArray[0] != null && MonoSingleton<InputManager>.Instance.InputSource.LastWeapon.WasCanceledThisFrame)
        {
            WeaponCycle wcTemp = wcArray[1];
            wcArray[1] = wcArray[0];
            wcArray[0] = wcTemp;

            WeaponCycle wc = wcArray[1];
            int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
            newSlot = arr[0]; newVariation = arr[1];
        }
    }

    public void SwitchWeaponLogic()
    {
        for (int i = 0; i < PluginConfig.weaponCycles.Length; i++)
        {
            WeaponCycle wc = PluginConfig.weaponCycles[i];
            if(Input.GetKeyDown(wc.useCode))
            {
                bool resetWCIndex = true;
                if(wc.rememberVariation == true || wc == wcArray[1]) //if not switching to the weapon we had out last
                {
                    resetWCIndex = false;
                }
                if(wc != wcArray[1])
                {
                    wcArray[0] = wcArray[1];
                    wcArray[1] = wc;
                }

                if(wc.swapBehavior == SwapBehaviorEnum.NextVariation)
                {
                    wc.currentIndex += 1;
                    if(resetWCIndex == true) {wc.currentIndex = 0;}
                }
                else if(wc.swapBehavior == SwapBehaviorEnum.SameVariation)
                {
                    wc.currentIndex += 0;
                    if(resetWCIndex == true) {wc.currentIndex = 0;}
                }
                else if(wc.swapBehavior == SwapBehaviorEnum.FirstVariation)
                {
                    wc.currentIndex = 0;
                }

                for (int j = 0; j < wc.weaponEnums.Length; j++)
                {
                    if(wc.currentIndex >= wc.weaponEnums.Length) {wc.currentIndex = 0;}
                    if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None) {wc.currentIndex += 1;}
                }
                int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
                newSlot = arr[0]; newVariation = arr[1];
            }
        }
    }

    public void InputLogic()
    {
        newSlot = -1;
        newVariation = -1;

        VariationBindLogic();
        ScrollLogic();
        AlterVariationLogic();
        AlterWeaponLogic();
        LastWeaponLogic();
        SwitchWeaponLogic();

        if(newVariation != -1 && newSlot != -1)
        {
            if     (newSlot == 0) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot1[newVariation], true);}
            else if(newSlot == 1) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot2[newVariation], true);}
            else if(newSlot == 2) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot3[newVariation], true);}
            else if(newSlot == 3) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot4[newVariation], true);}
            else if(newSlot == 4) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot5[newVariation], true);}
        }
    }

    public void Update()
    {
        if(!IsGameplayScene() || !modEnabled || IsMenu()) {return;}
        
        InputLogic();
    }
}
