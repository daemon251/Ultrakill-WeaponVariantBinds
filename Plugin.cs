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
//names
//back weapon fix
//weapon variants buttons? I guess?
//colors
//disable default buttons when enabled option

//unequipped weapons cause problems?

[BepInPlugin("WeaponVariantBinds", "WeaponVariantBinds", "0.01")]
public class Plugin : BaseUnityPlugin
{
    public static bool modEnabled = true;
    public static bool legacyModDisabled = true;
    public static GameObject weapon = null;

    public static bool scrollEnabled = false;
    public static bool scrollVariation = false;
    public static bool scrollReversed = false;
    
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

    int lastWeaponSwitchDueToCycleNumber = -1;

    public bool findNextRealWeaponInCycle(WeaponCycle wc) //returns false if there is no next
    {
        wc.currentIndex += 1;
        if(wc.currentIndex >= wc.weaponEnums.Length) {wc.currentIndex = 0;}
        for(int j = 0; j < wc.weaponEnums.Length; j++)
        {
            if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None)
            {
                wc.currentIndex += 1;
                if(wc.currentIndex >= wc.weaponEnums.Length) {wc.currentIndex = 0;}
                if(j == wc.weaponEnums.Length - 1) {return false;} //Debug.Log("WeaponVariantBinds - Custom Weapon Cycle Full of Nones"); 
            }
        }
        
        return true;
    }
    public bool findLastRealWeaponInCycle(WeaponCycle wc)
    {
        wc.currentIndex += -1;
        if(wc.currentIndex < 0) {wc.currentIndex = wc.weaponEnums.Length - 1;}
        for(int j = 0; j < wc.weaponEnums.Length; j++)
        {
            if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None)
            {
                wc.currentIndex += -1;
                if(wc.currentIndex < 0) {wc.currentIndex = wc.weaponEnums.Length - 1;}
                if(j == wc.weaponEnums.Length - 1) {return false;} //Debug.Log("WeaponVariantBinds - Custom Weapon Cycle Full of Nones"); 
            }
        }
        return true;
    }

    public void switchInWeaponCycle(WeaponCycle wc, int i)
    {
        if(!(lastWeaponSwitchDueToCycleNumber == -1 || lastWeaponSwitchDueToCycleNumber == i)) {wc.currentIndex = 0;}

        int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
        int newSlot = arr[0];
        int newVariation = arr[1];

        Debug.Log("WeaponVariantBinds - Forcing via customweaponcycle to " + newSlot + " " + newVariation);

        MonoSingleton<GunControl>.Instance.lastUsedSlot = MonoSingleton<GunControl>.Instance.currentSlot;
        MonoSingleton<GunControl>.Instance.lastUsedVariation = MonoSingleton<GunControl>.Instance.currentVariation;

        if     (newSlot == 0) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot1[newVariation], true);}
        else if(newSlot == 1) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot2[newVariation], true);}
        else if(newSlot == 2) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot3[newVariation], true);}
        else if(newSlot == 3) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot4[newVariation], true);}
        else if(newSlot == 4) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot5[newVariation], true);}

        MonoSingleton<GunControl>.Instance.lastUsedSlot = MonoSingleton<GunControl>.Instance.currentSlot;
        MonoSingleton<GunControl>.Instance.lastUsedVariation = MonoSingleton<GunControl>.Instance.currentVariation;
        weapon = MonoSingleton<GunControl>.Instance.currentWeapon;
        PluginOld.setLastUsedWeapon();
        PluginOld.tempDisableAutoSwitch = true;

        /*for(int j = 0; j < PluginConfig.weaponCycles.Length; j++)
        {
            if(j != i)
            {
                PluginConfig.weaponCycles[j].currentIndex = -1; //set evrything but active to negative one, this gets added to again when we pull it out again, back to zero (what we want)
            }
        }*/
        lastWeaponSwitchDueToCycleNumber = i;
    }

    public bool checkForCustomWeaponCycleInput()
    {
        for (int i = 0; i < PluginConfig.weaponCycles.Length; i++)
        {
            WeaponCycle wc = PluginConfig.weaponCycles[i];
            if(lastWeaponSwitchDueToCycleNumber == i && wc.swapThroughWeaponCycle == true)
            {
                if(lastWeaponSwitchDueToCycleNumber >= 0 && MonoSingleton<InputManager>.Instance.InputSource.NextVariation.WasPerformedThisFrame)
                {
                    if(findNextRealWeaponInCycle(wc) == false) {return false;}
                    switchInWeaponCycle(wc, i);
                    return true;
                }
                if(lastWeaponSwitchDueToCycleNumber >= 0 && MonoSingleton<InputManager>.Instance.InputSource.PreviousVariation.WasCanceledThisFrame)
                {
                    if(findLastRealWeaponInCycle(wc) == false) {return false;}
                    switchInWeaponCycle(wc, i);
                    return true;
                }
            }
            if(lastWeaponSwitchDueToCycleNumber == i && wc.scrollThroughWeaponCycle == true)
            {
                if(MonoSingleton<PrefsManager>.Instance.prefMap.ContainsKey("scrollEnabled")) {scrollEnabled =   (bool)MonoSingleton<PrefsManager>.Instance.prefMap["scrollEnabled"];}
                if(MonoSingleton<PrefsManager>.Instance.prefMap.ContainsKey("scrollVariations")) {scrollVariation = (bool)MonoSingleton<PrefsManager>.Instance.prefMap["scrollVariations"];}
                if(MonoSingleton<PrefsManager>.Instance.prefMap.ContainsKey("scrollReversed")) {scrollReversed =  (bool)MonoSingleton<PrefsManager>.Instance.prefMap["scrollReversed"];}

                float mult = 1f;
                if(scrollReversed){mult = -1f;}
                float value = Input.GetAxis("Mouse ScrollWheel") * mult; 
                if(scrollEnabled && scrollVariation)
                {
                    if(value >= 0.1f)
                    {
                        if(findNextRealWeaponInCycle(wc) == false) {return false;}
                        switchInWeaponCycle(wc, i);
                        return true;
                    }
                    if(value <= -0.1f)
                    {
                        if(findLastRealWeaponInCycle(wc) == false) {return false;}
                        switchInWeaponCycle(wc, i);
                        return true;
                    }
                }
            }
            if(Input.GetKeyDown(wc.useCode))
            {
                if(lastWeaponSwitchDueToCycleNumber == -1)
                {
                    wc.currentIndex += -1; //if this is our first switch, then offset the effect of going to the next weapon (we want to start on weapon index zero not one)
                }
                if(findNextRealWeaponInCycle(wc) == false) {return false;} //return because there are no weapons to do operations with.
                switchInWeaponCycle(wc, i);
                return true;
            }
        }
        return false; //return true only if we did something
    }

    public void Update()
    {
        if(!IsGameplayScene() || !modEnabled || IsMenu()) {return;}

        PluginOld.slot = MonoSingleton<GunControl>.Instance.currentSlot;  PluginOld.lastSlot = MonoSingleton<GunControl>.Instance.lastUsedSlot;
        PluginOld.variation = MonoSingleton<GunControl>.Instance.currentVariation;  PluginOld.lastVariation = MonoSingleton<GunControl>.Instance.lastUsedVariation;

        //if we successfully do a custom weapon cycle weapon switch
        if(checkForCustomWeaponCycleInput() == true) {goto skipOtherBehavior;}
        if(legacyModDisabled == false)
        {
            PluginOld.LegacyCode();
        }

        //enable auto switch if we find a change in weapon (done by the player)
        //done after autoswitch attempted, disabled early in the tick if something happens.
        if(weapon != MonoSingleton<GunControl>.Instance.currentWeapon) 
        {
            PluginOld.tempDisableAutoSwitch = false; //lets allow force switching a weapon
            lastWeaponSwitchDueToCycleNumber = -1; //-1 for no cycle involved
            for(int i = 0; i < PluginConfig.weaponCycles.Length; i++) //set to zero because we are no longer in the cycle
            {
                PluginConfig.weaponCycles[i].currentIndex = 0;
            }
            Debug.Log("WeaponVariantBinds - old weapon: " + weapon + " new: " + MonoSingleton<GunControl>.Instance.currentWeapon);
        }
        skipOtherBehavior:

        weapon = MonoSingleton<GunControl>.Instance.currentWeapon;
    }
}
