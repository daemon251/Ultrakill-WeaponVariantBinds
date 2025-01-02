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

//cant wait for yet ANOTHER Fucking Edge Case to come screw me in the behind

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
        //harmony.PatchAll(typeof(GunControl));
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

    GameObject weapon = null;
    public void Update() //haha spaghetti spaghetii
    {
        if(!IsGameplayScene() || !modEnabled) {return;}

        for (int i = 0; i < weaponKeyCodes.GetLength(0); i++)
        {
            for (int j = 0; j < weaponKeyCodes.GetLength(1); j++)
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
                }
            }
        }

        int onSwapBehavior = MonoSingleton<PrefsManager>.Instance.GetInt("WeaponRedrawBehaviour"); //0 is default, 1 is first weapon, 2 is same weapon
        //if(onSwapBehavior == 1) {tempDisableAutoSwitch = true;} //basically disable auto switching if this is enabled.
        //if(onSwapBehavior == 2) //same variation; go through switch the first time but after that dont do it.
        //{
        //    tempDisableAutoSwitch = true; 
        //    if(slot != MonoSingleton<GunControl>.Instance.currentSlot && forceSwitched == false) //upon weapon switch, go ahead and auto switch
        //    {
        //        tempDisableAutoSwitch = false;
        //    }
        //} 

        slot = MonoSingleton<GunControl>.Instance.currentSlot;  lastSlot = MonoSingleton<GunControl>.Instance.lastUsedSlot;
        variation = MonoSingleton<GunControl>.Instance.currentVariation; lastVariation = MonoSingleton<GunControl>.Instance.lastUsedVariation;

        List<GameObject> slotList = null;
        if     (slot == 1){slotList = MonoSingleton<GunControl>.Instance.slot1;}
        else if(slot == 2){slotList = MonoSingleton<GunControl>.Instance.slot2;}
        else if(slot == 3){slotList = MonoSingleton<GunControl>.Instance.slot3;}
        else if(slot == 4){slotList = MonoSingleton<GunControl>.Instance.slot4;}
        else if(slot == 5){slotList = MonoSingleton<GunControl>.Instance.slot5;}

        //  MonoSingleton<InputManager>.Instance.InputSource.Actions.Weapon.VariationSlot3 just doesnt exist if you dont use the ultrakill c# dll. I guess something I have is outdated and doesnt include the variation slots.
        if (MonoSingleton<InputManager>.Instance.InputSource.Actions.Weapon.VariationSlot1.WasPressedThisFrame()   ||  //so god damn LOOONG
            MonoSingleton<InputManager>.Instance.InputSource.Actions.Weapon.VariationSlot2.WasPressedThisFrame()   ||
            MonoSingleton<InputManager>.Instance.InputSource.Actions.Weapon.VariationSlot3.WasPressedThisFrame()    ) 
        {tempDisableAutoSwitch = true; weapon = MonoSingleton<GunControl>.Instance.currentWeapon;}

        if(MonoSingleton<InputManager>.Instance.InputSource.LastWeapon.WasCanceledThisFrame) {tempDisableAutoSwitch = true; weapon = MonoSingleton<GunControl>.Instance.currentWeapon;}

        if((MonoSingleton<InputManager>.Instance.InputSource.NextVariation.WasPerformedThisFrame    ||
            MonoSingleton<InputManager>.Instance.InputSource.PreviousVariation.WasCanceledThisFrame  )  &&
            SwapVariationIgnoreMod == true)
        {tempDisableAutoSwitch = true; weapon = MonoSingleton<GunControl>.Instance.currentWeapon;}

        if(Math.Abs(Input.GetAxis("Mouse ScrollWheel")) >= 0.1 &&
           (bool)MonoSingleton<PrefsManager>.Instance.prefMap["scrollEnabled"]    == true &&
           (bool)MonoSingleton<PrefsManager>.Instance.prefMap["scrollVariations"] == true &&
           ScrollVariationIgnoreMod == true)
        {tempDisableAutoSwitch = true; weapon = MonoSingleton<GunControl>.Instance.currentWeapon;}

        //first two conditions should prevent OutOfBounds exceptions in case of modded weapons
        if(slot <= 5 && MonoSingleton<GunControl>.Instance.currentVariation <= 2 && tempDisableAutoSwitch == false && slotList != null && ignoreWeaponInCycle[slot - 1, MonoSingleton<GunControl>.Instance.currentVariation] == true) //this is a mess, fix later
        { 
            //first we have to check if ALL variants are ignored. (unless if there are more than 3)
            int ignoreCount = 0;
            for (int i = 0; i < slotList.Count; i++)
            {
                if(slotList.Count > 3) { } //short circuit the following if to avoid array out of bounds exception in case modders add extra variants
                else if(ignoreWeaponInCycle[slot - 1, i] == true) {ignoreCount++;}
            }
            bool tooManyIgnores = false;
            if(ignoreCount == slotList.Count) {tooManyIgnores = true;}

            if(tooManyIgnores == false)
            {
                bool weaponChanged = false;
                bool reversedDirection = false; //cancelled because it actually brings up a menu
                //MonoSingleton<PrefsManager>.Instance.GetBool ScrollReversed ScrollEnabled ScrollVariations

                bool scrollEnabled = (bool)MonoSingleton<PrefsManager>.Instance.prefMap["scrollEnabled"]; //you have to do it like this, doing it the normal way just doesnt work for some god damn reason
                bool scrollVariation = (bool)MonoSingleton<PrefsManager>.Instance.prefMap["scrollVariations"];
                if(scrollEnabled && scrollVariation)
                {
                    float value = Input.GetAxis("Mouse ScrollWheel");
                    if((bool)MonoSingleton<PrefsManager>.Instance.prefMap["scrollReversed"]) {reversedDirection = Math.Abs(value) >= 0.1f && value > 0; }
                    else{reversedDirection = Math.Abs(value) >= 0.1f && value < 0;}
                }
                if(MonoSingleton<InputManager>.Instance.InputSource.PreviousVariation.WasCanceledThisFrame == true) {reversedDirection = true;}

                if(reversedDirection == false)
                {
                    if(slotList.Count > 1 && MonoSingleton<GunControl>.Instance.currentVariation == 0 && ignoreWeaponInCycle[slot - 1, MonoSingleton<GunControl>.Instance.currentVariation] == true) {MonoSingleton<GunControl>.Instance.ForceWeapon(slotList[1], true); weaponChanged = true;}
                    if(slotList.Count > 2 && MonoSingleton<GunControl>.Instance.currentVariation == 1 && ignoreWeaponInCycle[slot - 1, MonoSingleton<GunControl>.Instance.currentVariation] == true) {MonoSingleton<GunControl>.Instance.ForceWeapon(slotList[2], true); weaponChanged = true;}
                    if(slotList.Count > 3 && MonoSingleton<GunControl>.Instance.currentVariation == 2 && ignoreWeaponInCycle[slot - 1, MonoSingleton<GunControl>.Instance.currentVariation] == true) {MonoSingleton<GunControl>.Instance.ForceWeapon(slotList[3], true); weaponChanged = true;}
                    else if (                MonoSingleton<GunControl>.Instance.currentVariation == 2 && ignoreWeaponInCycle[slot - 1, MonoSingleton<GunControl>.Instance.currentVariation] == true) {MonoSingleton<GunControl>.Instance.ForceWeapon(slotList[0], true); weaponChanged = true;}
                    if(weaponChanged) {setLastUsedWeapon();}
                }
                else
                {
                    if(MonoSingleton<GunControl>.Instance.currentVariation == 2 && ignoreWeaponInCycle[slot - 1, MonoSingleton<GunControl>.Instance.currentVariation] == true) {MonoSingleton<GunControl>.Instance.ForceWeapon(slotList[1], true); weaponChanged = true;}
                    if(MonoSingleton<GunControl>.Instance.currentVariation == 1 && ignoreWeaponInCycle[slot - 1, MonoSingleton<GunControl>.Instance.currentVariation] == true) {MonoSingleton<GunControl>.Instance.ForceWeapon(slotList[0], true); weaponChanged = true;}
                    if(MonoSingleton<GunControl>.Instance.currentVariation == 0 && ignoreWeaponInCycle[slot - 1, MonoSingleton<GunControl>.Instance.currentVariation] == true) {MonoSingleton<GunControl>.Instance.ForceWeapon(slotList[slotList.Count - 1], true); weaponChanged = true;}
                    if(weaponChanged) {setLastUsedWeapon();}
                }
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
