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

namespace WeaponVariantBinds;

//TO DO:
//organize code
//mouse 5 dont work probably

//unequipped weapons cause problems

[BepInPlugin("WeaponVariantBinds", "WeaponVariantBinds", "0.01")]
public class Plugin : BaseUnityPlugin
{
    public static bool tempDisableAutoSwitch = false;
    public static bool modEnabled = true;
    public static bool SwapVariationIgnoreMod = false;
    public static bool ScrollVariationIgnoreMod = false;
    
    //I am not a C# developer but what is this syntax [,]???
    public static KeyCode[,] weaponKeyCodes = new KeyCode[5, 3];
    public static bool[,] ignoreWeaponInCycle = new bool[5, 3];
    
    private void Awake()
    {
        PluginConfig.WeaponVariantBindsConfig();
        Harmony harmony = new Harmony("WeaponVariantBinds");
        harmony.PatchAll();
        Logger.LogInfo("Plugin WeaponVariantBinds is loaded!");
    }

    int slot = 0; int lastSlot = 0;
    int variation = 0; int lastVariation = 0;

    public static bool IsGameplayScene() //copied from UltraTweaker
    {
        string[] NonGameplay = {"Intro","Bootstrap","Main Menu","Level 2-S","Intermission1","Intermission2"};
        if(SceneHelper.CurrentScene == null) {return false;}
        return !NonGameplay.Contains(SceneHelper.CurrentScene);
    }
    void setLastUsedWeapon()
    {
        MonoSingleton<GunControl>.Instance.lastUsedSlot = lastSlot;
        MonoSingleton<GunControl>.Instance.lastUsedVariation = lastVariation;
    }

    bool switchToNextWeaponWithSameKeyCode(KeyCode code, int i, int j)
    {
        int j_startIndex = j;
        for(int i2 = i; i2 < weaponKeyCodes.GetLength(0); i2++)
        {
            for(int j2 = j_startIndex; j2 < weaponKeyCodes.GetLength(1); j2++)
            {
                //PROBABLY DOES NOT SUPPORT MODDED EXTRA WEAPONS
                if(i == i2 & j == j2) {j2++;}
                if(j2 > 2) {j2 = 0; i2++;}
                if(j2 == 0 & i2 == 5) {goto CheckAtStartNow;} //if we reached the last weapon and variant
                if(weaponKeyCodes[i2, j2] == code)
                {
                    if     (i2 == 0) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot1[j2], true);}
                    else if(i2 == 1) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot2[j2], true);}
                    else if(i2 == 2) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot3[j2], true);}
                    else if(i2 == 3) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot4[j2], true);}
                    else if(i2 == 4) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot5[j2], true);}
                    //temp disable auto switch now
                    tempDisableAutoSwitch = true;
                    weapon = MonoSingleton<GunControl>.Instance.currentWeapon;
                    return true;
                }
            }
            j_startIndex = 0;
        }
        CheckAtStartNow:
        for(int i2 = 0; i2 < weaponKeyCodes.GetLength(0); i2++)
        {
            for(int j2 = 0; j2 < weaponKeyCodes.GetLength(1); j2++)
            {
                if(i == i2 & j == j2) {return false;}
                if(weaponKeyCodes[i2, j2] == code)
                {
                    if     (i2 == 0) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot1[j2], true);}
                    else if(i2 == 1) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot2[j2], true);}
                    else if(i2 == 2) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot3[j2], true);}
                    else if(i2 == 3) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot4[j2], true);}
                    else if(i2 == 4) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot5[j2], true);}
                    //temp disable auto switch now
                    tempDisableAutoSwitch = true;
                    weapon = MonoSingleton<GunControl>.Instance.currentWeapon;
                    return true;
                }
            }
        }
        Debug.Log("WeaponVariantBinds - Should have never gotten here.");
        return false;
    }

    GameObject weapon = null;
    public void Update() //haha spaghetti spaghetii
    {
        if(!IsGameplayScene() || !modEnabled) {return;}

        for (int i = 0; i < weaponKeyCodes.GetLength(0); i++) // j < 5
        {
            for (int j = 0; j < weaponKeyCodes.GetLength(1); j++) // j < 3
            {
                if(Input.GetKeyDown(weaponKeyCodes[i, j]))
                {
                    MonoSingleton<GunControl>.Instance.lastUsedSlot = MonoSingleton<GunControl>.Instance.currentSlot; MonoSingleton<GunControl>.Instance.lastUsedVariation = MonoSingleton<GunControl>.Instance.currentVariation;
                    //now check for if another weapon shares this bind AND if we are already on one of them. If we are, then switch to the next one on it (first by j, then by i). Else, switch to the weapon we intended.
                    if(MonoSingleton<GunControl>.Instance.currentSlot == i + 1 && MonoSingleton<GunControl>.Instance.currentVariation == j)
                    {
                        Debug.Log("WeaponVariantBinds - try to switch to next wep with same keycode");
                        if(switchToNextWeaponWithSameKeyCode(weaponKeyCodes[i, j], i, j))
                        {
                            Debug.Log("WeaponVariantBinds - switched to next wep with same keycode");
                            goto WeaponSwitched;
                        } 
                    }
                }
            }
        }

        //Ignore previous behavior normally. inefficent. 
        for (int i = 0; i < weaponKeyCodes.GetLength(0); i++) // j < 5 
        {
            for (int j = 0; j < weaponKeyCodes.GetLength(1); j++) // j < 3
            {
                if(Input.GetKeyDown(weaponKeyCodes[i, j]))
                {
                    //is this REALLY the best way to do this?
                    MonoSingleton<GunControl>.Instance.lastUsedSlot = MonoSingleton<GunControl>.Instance.currentSlot; MonoSingleton<GunControl>.Instance.lastUsedVariation = MonoSingleton<GunControl>.Instance.currentVariation;
                    if     (i == 0) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot1[j], true);}
                    else if(i == 1) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot2[j], true);}
                    else if(i == 2) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot3[j], true);}
                    else if(i == 3) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot4[j], true);}
                    else if(i == 4) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot5[j], true);}
                    //temp disable auto switch now
                    tempDisableAutoSwitch = true;
                    weapon = MonoSingleton<GunControl>.Instance.currentWeapon;

                    Debug.Log("WeaponVariantBinds - key pressed");

                    goto WeaponSwitched;
                }
            }
        }

        WeaponSwitched:

        //int onSwapBehavior = MonoSingleton<PrefsManager>.Instance.GetInt("WeaponRedrawBehaviour"); //0 is default, 1 is first weapon, 2 is same weapon

        slot = MonoSingleton<GunControl>.Instance.currentSlot; lastSlot = MonoSingleton<GunControl>.Instance.lastUsedSlot;
        variation = MonoSingleton<GunControl>.Instance.currentVariation; lastVariation = MonoSingleton<GunControl>.Instance.lastUsedVariation;

        List<GameObject> slotList = null;
        if     (slot == 1){slotList = MonoSingleton<GunControl>.Instance.slot1;}
        else if(slot == 2){slotList = MonoSingleton<GunControl>.Instance.slot2;}
        else if(slot == 3){slotList = MonoSingleton<GunControl>.Instance.slot3;}
        else if(slot == 4){slotList = MonoSingleton<GunControl>.Instance.slot4;}
        else if(slot == 5){slotList = MonoSingleton<GunControl>.Instance.slot5;}

        //  MonoSingleton<InputManager>.Instance.InputSource.Actions.Weapon.VariationSlot3 just doesnt exist if you dont use the ultrakill c# dll. I guess something I have is outdated and doesnt include the variation slots.
        if (MonoSingleton<InputManager>.Instance.InputSource.Actions.Weapon.VariationSlot1.WasPressedThisFrame()   ||  
            MonoSingleton<InputManager>.Instance.InputSource.Actions.Weapon.VariationSlot2.WasPressedThisFrame()   ||
            MonoSingleton<InputManager>.Instance.InputSource.Actions.Weapon.VariationSlot3.WasPressedThisFrame()    ) 
        {tempDisableAutoSwitch = true; weapon = MonoSingleton<GunControl>.Instance.currentWeapon;}

        if(MonoSingleton<InputManager>.Instance.InputSource.LastWeapon.WasCanceledThisFrame) {tempDisableAutoSwitch = true; weapon = MonoSingleton<GunControl>.Instance.currentWeapon;}

        if((MonoSingleton<InputManager>.Instance.InputSource.NextVariation.WasPerformedThisFrame    ||
            MonoSingleton<InputManager>.Instance.InputSource.PreviousVariation.WasCanceledThisFrame  )  &&
            SwapVariationIgnoreMod == true)
        {tempDisableAutoSwitch = true; weapon = MonoSingleton<GunControl>.Instance.currentWeapon;}

        //you have to do it like this, doing it the normal way just doesnt work for some reason... Causes KeyNotFoundException I think for some reason for some people unless we check if the key exists?
        //If this fixes the issue of the mod not working, this will still be concerning and probably means for that same group of people that scrolling will not work properly.
        bool scrollEnabled = false;
        bool scrollVariation = false;
        bool scrollReversed = false;

        //Debug.Log("WeaponVariantBinds - attempting to load extra prefs");
        if(MonoSingleton<PrefsManager>.Instance.prefMap.ContainsKey("scrollEnabled")) {scrollEnabled =   (bool)MonoSingleton<PrefsManager>.Instance.prefMap["scrollEnabled"];}
        if(MonoSingleton<PrefsManager>.Instance.prefMap.ContainsKey("scrollVariations")) {scrollVariation = (bool)MonoSingleton<PrefsManager>.Instance.prefMap["scrollVariations"];}
        if(MonoSingleton<PrefsManager>.Instance.prefMap.ContainsKey("scrollReversed")) {scrollReversed =  (bool)MonoSingleton<PrefsManager>.Instance.prefMap["scrollReversed"];}

        if(Math.Abs(Input.GetAxis("Mouse ScrollWheel")) >= 0.1 &&
           scrollEnabled            == true &&
           scrollVariation          == true &&
           ScrollVariationIgnoreMod == true  )
        {tempDisableAutoSwitch = true; weapon = MonoSingleton<GunControl>.Instance.currentWeapon;}

        //first two conditions should prevent OutOfBounds exceptions in case of modded weapons
        if(slot <= 5 && MonoSingleton<GunControl>.Instance.currentVariation <= 2 && tempDisableAutoSwitch == false && slotList != null && ignoreWeaponInCycle[slot - 1, MonoSingleton<GunControl>.Instance.currentVariation] == true) //this is a mess, fix later
        { 
            //first we have to check if ALL variants are ignored. (unless if there are more than 3)

            int ignoreCount = 0;
            for (int i = 0; i < slotList.Count; i++)
            {
                if(slotList.Count > 3) { } //to avoid array out of bounds exception in case modders add extra variants
                else if(ignoreWeaponInCycle[slot - 1, i] == true) {ignoreCount++;}
            }
            bool tooManyIgnores = false;
            if(ignoreCount == slotList.Count) {tooManyIgnores = true;}

            if(tooManyIgnores == false)
            {
                bool weaponChanged = false;
                bool reversedDirection = false;

                if(scrollEnabled && scrollVariation)
                {
                    float value = Input.GetAxis("Mouse ScrollWheel");
                    if(scrollReversed) {reversedDirection = Math.Abs(value) >= 0.1f && value > 0; }
                    else{reversedDirection = Math.Abs(value) >= 0.1f && value < 0;}
                }
                if(MonoSingleton<InputManager>.Instance.InputSource.PreviousVariation.WasCanceledThisFrame == true) {reversedDirection = true;}

                Debug.Log("WeaponVariantBinds - current: " + MonoSingleton<GunControl>.Instance.currentSlot + " " + MonoSingleton<GunControl>.Instance.currentVariation);
                Debug.Log("WeaponVariantBinds - weapon about to be forced");
                if(reversedDirection == false)
                {
                    if(slotList.Count > 1 && MonoSingleton<GunControl>.Instance.currentVariation == 0 && ignoreWeaponInCycle[slot - 1, MonoSingleton<GunControl>.Instance.currentVariation] == true) {MonoSingleton<GunControl>.Instance.ForceWeapon(slotList[1], true); weaponChanged = true;}
                    if(slotList.Count > 2 && MonoSingleton<GunControl>.Instance.currentVariation == 1 && ignoreWeaponInCycle[slot - 1, MonoSingleton<GunControl>.Instance.currentVariation] == true) {MonoSingleton<GunControl>.Instance.ForceWeapon(slotList[2], true); weaponChanged = true;}
                    if(slotList.Count > 3 && MonoSingleton<GunControl>.Instance.currentVariation == 2 && ignoreWeaponInCycle[slot - 1, MonoSingleton<GunControl>.Instance.currentVariation] == true) {MonoSingleton<GunControl>.Instance.ForceWeapon(slotList[3], true); weaponChanged = true;}
                    else if(                 MonoSingleton<GunControl>.Instance.currentVariation == 2 && ignoreWeaponInCycle[slot - 1, MonoSingleton<GunControl>.Instance.currentVariation] == true) {MonoSingleton<GunControl>.Instance.ForceWeapon(slotList[0], true); weaponChanged = true;}
                    if(weaponChanged) {setLastUsedWeapon();}
                }
                else
                {
                    if(MonoSingleton<GunControl>.Instance.currentVariation == 2 && ignoreWeaponInCycle[slot - 1, MonoSingleton<GunControl>.Instance.currentVariation] == true) {MonoSingleton<GunControl>.Instance.ForceWeapon(slotList[1], true); weaponChanged = true;}
                    if(MonoSingleton<GunControl>.Instance.currentVariation == 1 && ignoreWeaponInCycle[slot - 1, MonoSingleton<GunControl>.Instance.currentVariation] == true) {MonoSingleton<GunControl>.Instance.ForceWeapon(slotList[0], true); weaponChanged = true;}
                    if(MonoSingleton<GunControl>.Instance.currentVariation == 0 && ignoreWeaponInCycle[slot - 1, MonoSingleton<GunControl>.Instance.currentVariation] == true) {MonoSingleton<GunControl>.Instance.ForceWeapon(slotList[slotList.Count - 1], true); weaponChanged = true;}
                    if(weaponChanged) {setLastUsedWeapon();}
                }
                Debug.Log("WeaponVariantBinds - after: " + MonoSingleton<GunControl>.Instance.currentSlot + " " + MonoSingleton<GunControl>.Instance.currentVariation);
            }
        }

        //enable auto switch if we find a change in weapon (done by the player)
        if(weapon != MonoSingleton<GunControl>.Instance.currentWeapon) 
        {
            tempDisableAutoSwitch = false; //release the flood gates !!! (lets allow force switching a weapon)
        }
        weapon = MonoSingleton<GunControl>.Instance.currentWeapon;
    }
}
