using BepInEx;
using UnityEngine;
using HarmonyLib;
using System;
using System.Linq;
using BepInEx.Logging;

namespace WeaponVariantBinds;

//TO DO:
//organize code
//unequipped weapons cause problems?
//delete all at once, bad?
//weapon slot 6
//dual wield

[BepInPlugin("WeaponVariantBinds", "WeaponVariantBinds", "0.01")]
public class Plugin : BaseUnityPlugin
{
    public static bool modEnabled = true;
    public static GameObject weapon = null;
    public static bool scrollEnabled = false;
    public static bool scrollVariation = false;
    public static bool scrollReversed = false;
    public static bool scrollWeapons = false;
    public static ManualLogSource logger;
    private void Awake()
    {
        logger = new ManualLogSource("WeaponVariantBinds"); BepInEx.Logging.Logger.Sources.Add(logger);
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
    public void IfSlotIsIgnoredLogic(bool goingToNext)
    {
        if(goingToNext == true) //go next
        {
            for(int i = 0; i < 105; i++)
            {
                WeaponCycle wc = wcArray[1];
                if(wc.skipOverThisCycle == true)
                {
                    if((PluginConfig.vanillaWeaponCycles.Contains(wc) && wc != PluginConfig.vanillaWeaponCycles[4]) || wc == PluginConfig.weaponCycles[PluginConfig.numWeaponCyclesActive - 2]) //switch to next vanilla wc
                    {
                        int index = 0;
                        if(wc == PluginConfig.vanillaWeaponCycles[0]) {index = 1;}
                        if(wc == PluginConfig.vanillaWeaponCycles[1]) {index = 2;}
                        if(wc == PluginConfig.vanillaWeaponCycles[2]) {index = 3;}
                        if(wc == PluginConfig.vanillaWeaponCycles[3]) {index = 4;}
                        wcArray[1] = PluginConfig.vanillaWeaponCycles[index];
                    }
                    else //switch to next custom wc
                    {
                        int index = 0;
                        for(int j = 0; j < 99; j++)
                        {
                            if(wc == PluginConfig.weaponCycles[j])
                            {
                                index = j + 1;
                            }
                        }
                        wcArray[1] = PluginConfig.weaponCycles[index];
                    }
                }
                else {break;}
            }
        }
        else if(goingToNext == false) //go prev
        {
            for(int i = 0; i < 105; i++)
            {
                WeaponCycle wc = wcArray[1];
                if(wc.skipOverThisCycle == true)
                {
                    if((PluginConfig.vanillaWeaponCycles.Contains(wc) && wc != PluginConfig.vanillaWeaponCycles[0]) || wc == PluginConfig.weaponCycles[0]) //switch to prev vanilla wc
                    {
                        int index = 4;
                        if(wc == PluginConfig.vanillaWeaponCycles[1]) {index = 0;}
                        if(wc == PluginConfig.vanillaWeaponCycles[2]) {index = 1;}
                        if(wc == PluginConfig.vanillaWeaponCycles[3]) {index = 2;}
                        if(wc == PluginConfig.vanillaWeaponCycles[4]) {index = 3;}
                        wcArray[1] = PluginConfig.vanillaWeaponCycles[index];
                    }
                    else
                    {
                        int index = 0;
                        for(int j = 1; j < 100; j++)
                        {
                            if(wc == PluginConfig.weaponCycles[j])
                            {
                                index = j - 1;
                            }
                        }
                        wcArray[1] = PluginConfig.weaponCycles[index];
                    }
                }
                else {break;}
            }
        }
    }
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

        IfSlotIsIgnoredLogic(true);

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

        IfSlotIsIgnoredLogic(false);

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
        //this is slighlty buggy because we aren't preventing base game scrolling, and my version and the game's version of scrolling definitions are slightly different
        WeaponCycle wc = wcArray[1];
        if(wcArray[1] != null) 
        {
            scrollEnabled = MonoSingleton<PrefsManager>.Instance.GetBool("scrollEnabled");
            scrollVariation = MonoSingleton<PrefsManager>.Instance.GetBool("scrollVariations");
            scrollReversed = MonoSingleton<PrefsManager>.Instance.GetBool("scrollReversed");
            scrollWeapons = MonoSingleton<PrefsManager>.Instance.GetBool("scrollWeapons");

            float mult = 1f;
            if(scrollReversed){mult = -1f;}
            float value = Input.GetAxis("Mouse ScrollWheel") * mult; 
            //float value = Mouse.current.scroll.ReadValue().y * mult; //copied from guncontrol, wish I could use this but Mouse is some mystery object there I cant use

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
        bool newCycleBool = false;
        if(wc != wcArray[1] && wc.rememberVariation) {newCycleBool = true;}
        if(wc.rememberVariation == true || wc == wcArray[1]) //if not switching to the weapon we had out last
        {
            resetWCIndex = false;
        }
        if(wc != wcArray[1])
        {
            wcArray[0] = wcArray[1];
            wcArray[1] = wc;
        }

        if(wc.swapBehavior == SwapBehaviorEnum.NextVariation && (newCycleBool == false))
        {
            wc.currentIndex += 1;
            if(resetWCIndex == true) {wc.currentIndex = 0;}
        }
        else if(wc.swapBehavior == SwapBehaviorEnum.SameVariation && newCycleBool == false)
        {
            wc.currentIndex += 0;
            if(resetWCIndex == true) {wc.currentIndex = 0;}
        }
        else if(wc.swapBehavior == SwapBehaviorEnum.FirstVariation && newCycleBool == false)
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
        if(MonoSingleton<InputManager>.Instance.InputSource.Slot6.WasPerformedThisFrame)
        {
            WeaponCycle wc = PluginConfig.miscVanillaWeaponCycle;
            if(wc != wcArray[1])
            {
                wcArray[0] = wcArray[1];
                wcArray[1] = wc;
            }
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
    public void SpecificCycleWeaponLogic()
    {
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                if(Input.GetKeyDown(PluginConfig.vanillaWeaponCycles[i].weaponVariantBinds[j]))
                {
                    WeaponCycle wc = PluginConfig.vanillaWeaponCycles[i];
                    if(wc != wcArray[1])
                    {
                        wcArray[0] = wcArray[1];
                        wcArray[1] = wc;
                    }

                    wc.currentIndex = j;

                    for (int k = 0; k < wc.weaponEnums.Length; k++)
                    {
                        if(wc.weaponEnums[wc.currentIndex] == PluginConfig.WeaponEnum.None) {wc.currentIndex += 1;}
                        if(wc.currentIndex >= wc.weaponEnums.Length) {wc.currentIndex = 0;}
                    }
                    int[] arr = PluginConfig.convertWeaponEnumToSlotVariation(wc.weaponEnums[wc.currentIndex]);
                    newSlot = arr[0]; newVariation = arr[1];
                }
            }
        }
        for(int i = 0; i < 99; i++)
        {
            if(PluginConfig.weaponCycles[i].weaponVariantBinds[0] == KeyCode.None) {break;}
            for(int j = 0; j < 99; j++)
            {
                if(PluginConfig.weaponCycles[i].weaponVariantBinds[j] == KeyCode.None) {break;}
                if(Input.GetKeyDown(PluginConfig.weaponCycles[i].weaponVariantBinds[j]))
                {
                    WeaponCycle wc = PluginConfig.weaponCycles[i];
                    if(wc != wcArray[1])
                    {
                        wcArray[0] = wcArray[1];
                        wcArray[1] = wc;
                    }

                    wc.currentIndex = j;

                    for (int k = 0; k < wc.weaponEnums.Length; k++)
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
    public void InputLogicMAIN()
    {
        newSlot = -1;
        newVariation = -1;

        VariationBindLogic();
        ScrollLogic();
        AlterVariationLogic();
        AlterWeaponLogic();
        LastWeaponLogic();
        SwitchWeaponLogic();
        SpecificCycleWeaponLogic();

        if(newVariation > 100 || newSlot > 100)
        {
            Logger.LogWarning("Tried to switch to an empty weapon.");
        }
        else if(newVariation != -1 && newSlot != -1)
        {
            if     (newSlot == 0) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot1[newVariation], true);}
            else if(newSlot == 1) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot2[newVariation], true);}
            else if(newSlot == 2) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot3[newVariation], true);}
            else if(newSlot == 3) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot4[newVariation], true);}
            else if(newSlot == 4) {MonoSingleton<GunControl>.Instance.ForceWeapon(MonoSingleton<GunControl>.Instance.slot5[newVariation], true);}
            weapon = MonoSingleton<GunControl>.Instance.currentWeapon;
                
            DualWield[] dualwields = MonoSingleton<GunControl>.Instance.GetComponentsInChildren<DualWield>();
            for(int i = 0; i < dualwields.Length; i++)
            {
                //dualwields[i].currentWeapon = weapon;
                dualwields[i].UpdateWeapon(weapon);
                //logger.LogInfo(i + " " + dualwields[i].currentWeapon);
            }
            return;
        }
        if(weapon != MonoSingleton<GunControl>.Instance.currentWeapon) //if a weapon slot change not made by us is detected
        {
            Logger.LogWarning("Unaccounted weapon change detected.");
            weapon = MonoSingleton<GunControl>.Instance.currentWeapon;
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
        if(tickCount > tickCountUpdateVanillaWeaponCycles + 100) //this is bad
        {
            PluginConfig.DetermineVanillaWeaponCycles();
            tickCountUpdateVanillaWeaponCycles = tickCount;
        }
        if(!IsGameplayScene() || !modEnabled || IsMenu()) {return;}
        InputLogicMAIN();
    }
}
