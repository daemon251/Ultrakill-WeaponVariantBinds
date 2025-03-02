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
using System.ComponentModel;
using System.Net.WebSockets;

namespace WeaponVariantBinds;

//TO DO:
//organize code
//unequipped weapons cause problems?
//delete all at once, bad?
//clear weaponCycles when needed

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
        PluginConfig.InitVanillaWeaponCycles();
        PluginConfig.WeaponVariantBindsConfig();
        wcArray[0] = PluginConfig.vanillaWeaponCycles[1];
        wcArray[1] = PluginConfig.vanillaWeaponCycles[0];
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
        if(MonoSingleton<OptionsManager>.Instance != null && !MonoSingleton<OptionsManager>.Instance.paused && !MonoSingleton<FistControl>.Instance.shopping && GameStateManager.Instance != null && !GameStateManager.Instance.PlayerInputLocked)
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
        if(wcTemp == PluginConfig.vanillaWeaponCycles[0]) {wcArray[0] = wcArray[1]; wcArray[1] = PluginConfig.vanillaWeaponCycles[1];}
        else if(wcTemp == PluginConfig.vanillaWeaponCycles[1]) {wcArray[0] = wcArray[1]; wcArray[1] = PluginConfig.vanillaWeaponCycles[2];}
        else if(wcTemp == PluginConfig.vanillaWeaponCycles[2]) {wcArray[0] = wcArray[1]; wcArray[1] = PluginConfig.vanillaWeaponCycles[3];}
        else if(wcTemp == PluginConfig.vanillaWeaponCycles[3]) {wcArray[0] = wcArray[1]; wcArray[1] = PluginConfig.vanillaWeaponCycles[4];}
        else
        {
            for(int i = 0; i < PluginConfig.numWeaponCyclesActive; i++)
            {
                if(wcTemp == PluginConfig.weaponCycles[i])
                {
                    index1 = i + 1;
                    if(index1 >= PluginConfig.numWeaponCyclesActive - 1)
                    {
                        wcArray[0] = wcArray[1]; wcArray[1] = PluginConfig.vanillaWeaponCycles[0];
                        goto end;
                    }
                }
            }
            for(int i = 0; i < PluginConfig.numWeaponCyclesActive; i++)
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
                    if(index1 >= PluginConfig.numWeaponCyclesActive)
                    {
                        index1 = 0;
                    }
                }
                else {break;}
            }
            wcArray[0] = wcArray[1];
            wcArray[1] = PluginConfig.weaponCycles[index1];
        }
        end:;

        WeaponCycle wc = wcArray[1];
        if(wc.rememberVariation == false) {wc.currentIndex = 0;}
        for (int j = 0; j < wc.weaponEnums.Length; j++)
        {
            if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None || wc.ignoreInCycle[wc.currentIndex] == true) {wc.currentIndex += 1;}
            if(wc.currentIndex >= wc.weaponEnums.Length) {wc.currentIndex = 0;}
        }
        int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
        newSlot = arr[0]; newVariation = arr[1];
    }
    public void SwitchToPrevCustomSlot() 
    {
        WeaponCycle wcTemp = wcArray[1];
        int index1 = PluginConfig.numWeaponCyclesActive - 1;
        if(wcTemp == PluginConfig.vanillaWeaponCycles[4]) {wcArray[0] = wcArray[1]; wcArray[1] = PluginConfig.vanillaWeaponCycles[3];}
        else if(wcTemp == PluginConfig.vanillaWeaponCycles[3]) {wcArray[0] = wcArray[1]; wcArray[1] = PluginConfig.vanillaWeaponCycles[2];}
        else if(wcTemp == PluginConfig.vanillaWeaponCycles[2]) {wcArray[0] = wcArray[1]; wcArray[1] = PluginConfig.vanillaWeaponCycles[1];}
        else if(wcTemp == PluginConfig.vanillaWeaponCycles[1]) {wcArray[0] = wcArray[1]; wcArray[1] = PluginConfig.vanillaWeaponCycles[0];}
        else 
        {
            for(int i = PluginConfig.weaponCycles.Length - 1; i >= 0 ; i += -1)
            {
                if(wcTemp == PluginConfig.weaponCycles[i])
                {
                    index1 = i - 1;
                    if(index1 < 0)
                    {
                        wcArray[0] = wcArray[1]; wcArray[1] = PluginConfig.vanillaWeaponCycles[4];
                        goto end;
                    }
                }
            }
            for(int i = PluginConfig.weaponCycles.Length - 1; i >= 0 ; i += -1)
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
                        index1 = PluginConfig.numWeaponCyclesActive - 1;
                    }
                }
                else {break;}
            }
            wcArray[0] = wcArray[1];
            wcArray[1] = PluginConfig.weaponCycles[index1];
        }
        end:;

        WeaponCycle wc = wcArray[1];
        if(wc.rememberVariation == false) {wc.currentIndex = 0;}
        for (int j = 0; j < wc.weaponEnums.Length; j++)
        {
            if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None || wc.ignoreInCycle[wc.currentIndex] == true) {wc.currentIndex += 1;}
            if(wc.currentIndex >= wc.weaponEnums.Length) {wc.currentIndex = 0;}
        }
        int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
        newSlot = arr[0]; newVariation = arr[1];
    }
    public void VariationBindLogic()
    {
        WeaponCycle wc = wcArray[1];
        //if(wc != null && wc.specificVariationThroughWeaponCycle == false) {return;}
        if(wc != null)
        {
            if(MonoSingleton<InputManager>.Instance.InputSource.Actions.Weapon.VariationSlot1.WasPressedThisFrame()) 
            {
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
                wc.currentIndex = 2;
                for (int j = 0; j < wc.weaponEnums.Length; j++)
                {
                    if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None) {wc.currentIndex += 1;}
                    if(wc.currentIndex >= wc.weaponEnums.Length) {wc.currentIndex = 0;}
                }
                int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
                newSlot = arr[0]; newVariation = arr[1];
            }
            for(int i = 3; i < 100; i++)
            {
                if(Input.GetKeyDown(PluginConfig.variationBinds[i]))
                {
                    wc.currentIndex = i;
                    for (int j = 0; j < wc.weaponEnums.Length; j++)
                    {
                        if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None) {wc.currentIndex += 1;}
                        if(wc.currentIndex >= wc.weaponEnums.Length) {wc.currentIndex = 0;}
                    }
                    int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
                    newSlot = arr[0]; newVariation = arr[1];
                }
            }
        }
    }
    public void ScrollLogic() //bugs out if you do it too fast
    {
        WeaponCycle wc = wcArray[1];
        //if(wc != null && wc.scrollThroughWeaponCycle == false) {return;}
        if(wcArray[1] != null) //slightly buggy?
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
                        if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None || wc.ignoreInCycle[wc.currentIndex] == true) {wc.currentIndex += 1;}
                    }
                    int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
                    newSlot = arr[0]; newVariation = arr[1];
                }
                if(value <= -0.1f)
                {
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
                        if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None || wc.ignoreInCycle[wc.currentIndex] == true) {wc.currentIndex += -1;}
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
        WeaponCycle wc = wcArray[1];
        //if(wc != null && wc.swapThroughWeaponCycle == false) {return;}
        if(MonoSingleton<InputManager>.Instance.InputSource.NextVariation.WasPerformedThisFrame)
        {
            wc.currentIndex += 1;
            for (int i = 0; i < wc.weaponEnums.Length; i++)
            {
                if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None || wc.ignoreInCycle[wc.currentIndex] == true) {wc.currentIndex += 1;}
                if(wc.currentIndex >= wc.weaponEnums.Length) {wc.currentIndex = 0;}
            }
            int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
            newSlot = arr[0]; newVariation = arr[1];
        }
        if(MonoSingleton<InputManager>.Instance.InputSource.PreviousVariation.WasCanceledThisFrame)
        {
            wc.currentIndex += -1;
            for (int i = 0; i < wc.weaponEnums.Length; i++)
            {
                if(wc.currentIndex < 0) {wc.currentIndex = wc.weaponEnums.Length - 1;}
                if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None || wc.ignoreInCycle[wc.currentIndex] == true) {wc.currentIndex += -1;}
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
    public void SwitchToWeapon(WeaponCycle wc)
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
            if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None || wc.ignoreInCycle[wc.currentIndex] == true) {wc.currentIndex += 1;}
            if(wc.currentIndex >= wc.weaponEnums.Length) {wc.currentIndex = 0;}
        }
        int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
        newSlot = arr[0]; newVariation = arr[1];
    }
    public void SwitchWeaponLogic()
    {
        if(MonoSingleton<InputManager>.Instance.InputSource.Slot1.WasPerformedThisFrame)
        {
            WeaponCycle wc = PluginConfig.vanillaWeaponCycles[0];
            SwitchToWeapon(wc);
            return;
        }
        if(MonoSingleton<InputManager>.Instance.InputSource.Slot2.WasPerformedThisFrame)
        {
            WeaponCycle wc = PluginConfig.vanillaWeaponCycles[1];
            SwitchToWeapon(wc);
            return;
        }
        if(MonoSingleton<InputManager>.Instance.InputSource.Slot3.WasPerformedThisFrame)
        {
            WeaponCycle wc = PluginConfig.vanillaWeaponCycles[2];
            SwitchToWeapon(wc);
            return;
        }
        if(MonoSingleton<InputManager>.Instance.InputSource.Slot4.WasPerformedThisFrame)
        {
            WeaponCycle wc = PluginConfig.vanillaWeaponCycles[3];
            SwitchToWeapon(wc);
            return;
        }
        if(MonoSingleton<InputManager>.Instance.InputSource.Slot5.WasPerformedThisFrame)
        {
            WeaponCycle wc = PluginConfig.vanillaWeaponCycles[4];
            SwitchToWeapon(wc);
            return;
        }
        for (int i = 0; i < PluginConfig.weaponCycles.Length; i++)
        {
            WeaponCycle wc = PluginConfig.weaponCycles[i];
            if(wc != null && Input.GetKeyDown(wc.useCode))
            {
                SwitchToWeapon(wc);
                return;
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

        if(newVariation > 100 || newSlot > 100)
        {
            Debug.Log("WeaponVariantBinds - Tried to switch to an empty weapon.");
        }
        else if(newVariation != -1 && newSlot != -1)
        {
            if     (newSlot == 0) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot1[newVariation], true);}
            else if(newSlot == 1) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot2[newVariation], true);}
            else if(newSlot == 2) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot3[newVariation], true);}
            else if(newSlot == 3) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot4[newVariation], true);}
            else if(newSlot == 4) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot5[newVariation], true);}
            weapon = MonoSingleton<GunControl>.Instance.currentWeapon;
            return;
        }
        if(weapon != MonoSingleton<GunControl>.Instance.currentWeapon) //if a weapon slot change not made by us is detected
        {
            Debug.Log("WeaponVariantBinds - Unaccounted weapon change detected.");
            weapon = MonoSingleton<GunControl>.Instance.currentWeapon;
            //this should never be ran
            /*PluginConfig.DetermineVanillaWeaponCycles();
            GameObject oldWeapon = weapon;
            WeaponCycle wcEmpty = new WeaponCycle();
            PluginConfig.WeaponEnum[] emptyWeaponEnums = {PluginConfig.WeaponEnum.None, PluginConfig.WeaponEnum.None, PluginConfig.WeaponEnum.None};
            wcEmpty.weaponEnums = emptyWeaponEnums;
            wcEmpty.useCode = KeyCode.None;
            wcArray[1] = wcEmpty; //overriden later, hopefully
            int currentSlot = MonoSingleton<GunControl>.Instance.currentSlotIndex;
            int currentVariation = MonoSingleton<GunControl>.Instance.currentVariationIndex;
            PluginConfig.WeaponEnum currentWeaponEnum = PluginConfig.convertWeaponToWeaponEnum(weapon);

            //alters vanilla behavior probably, but so be it.
            bool variationMemory = false;
            wcArray[0] = wcArray[1];
            if(MonoSingleton<PrefsManager>.Instance.prefMap.ContainsKey("variationMemory")) {variationMemory = (bool)MonoSingleton<PrefsManager>.Instance.prefMap["variationMemory"];}
            MonoSingleton<GunControl>.Instance.currentVariationIndex = 0;
            if     (currentSlot == 1 && variationMemory == false) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot1[0], true);}
            else if(currentSlot == 2 && variationMemory == false) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot2[0], true);}
            else if(currentSlot == 3 && variationMemory == false) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot3[0], true);}
            else if(currentSlot == 4 && variationMemory == false) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot4[0], true);}
            else if(currentSlot == 5 && variationMemory == false) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot5[0], true);}
            //endFindWeapon:;*/
        }
    }

    public static int tickCount = 0;
    public static int tickCountUpdateVanillaWeaponCycles = 0;
    public void Update()
    {
        tickCount++;
        if(tickCount == PluginConfig.tickUpdateSettings + 2) //yet more jank just for this stupid config... we need to wait so that the config fields update values because they DO NOT do it during the tick.
        {
            PluginConfig.UpdateSettings();
        }
        if(tickCount > tickCountUpdateVanillaWeaponCycles + 100)
        {
            PluginConfig.DetermineVanillaWeaponCycles(); //this is bad
            tickCountUpdateVanillaWeaponCycles = tickCount;
        }
        if(!IsGameplayScene() || !modEnabled || IsMenu()) {return;}
        InputLogic();
    }
}
