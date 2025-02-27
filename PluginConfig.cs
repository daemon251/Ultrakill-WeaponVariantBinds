using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;
using PluginConfig.API;
using PluginConfig.API.Decorators;
using PluginConfig.API.Fields;
using PluginConfig.API.Functionals;
using UnityEngine;
using UnityEngine.UIElements;

namespace WeaponVariantBinds;

public enum SwapBehaviorEnum{NextVariation, SameVariation, FirstVariation}

public class WeaponCycle
{   
    //enabled field used to be here but pointless because you can just set the use code to none
    public PluginConfig.WeaponEnum[] weaponEnums = new PluginConfig.WeaponEnum[10];
    public int currentIndex = 0;
    public KeyCode useCode = KeyCode.None;
    public bool scrollThroughWeaponCycle = true;
    public bool swapThroughWeaponCycle = true;
    public bool specificVariationThroughWeaponCycle = true;
    public bool rememberVariation = false;
    public SwapBehaviorEnum swapBehavior = SwapBehaviorEnum.NextVariation;
    public WeaponCycle() {}
}

public class PluginConfig
{
    public enum WeaponEnum {None,
    Piercer_Revolver, Marksman_Revolver, Sharpshooter_Revolver, 
    Core_Eject_Shotgun, Pump_Charge_Shotgun, Sawed_On_Shotgun, 
    Attractor_Nailgun, Overheat_Nailgun, Jumpstart_Nailgun,
    Electric_Railcannon, Screwdriver, Malicious_Railcannon, 
    Freezeframe_Rocket_Launcher, SRS_Rocket_Launcher, Firestarter_Rocket_Launcher}
    public static void setWeaponEnumDisplayNames(EnumField<WeaponEnum> field)
    {
        field.SetEnumDisplayName(WeaponEnum.Piercer_Revolver, "Piercer Revolver");
        field.SetEnumDisplayName(WeaponEnum.Marksman_Revolver, "Marksman Revolver");
        field.SetEnumDisplayName(WeaponEnum.Sharpshooter_Revolver, "Sharpshooter Revolver");
        field.SetEnumDisplayName(WeaponEnum.Core_Eject_Shotgun, "Core Eject Shotgun");
        field.SetEnumDisplayName(WeaponEnum.Pump_Charge_Shotgun, "Pump Charge Shotgun");
        field.SetEnumDisplayName(WeaponEnum.Sawed_On_Shotgun, "Sawed-On Shotgun");
        field.SetEnumDisplayName(WeaponEnum.Attractor_Nailgun, "Attractor Nailgun");
        field.SetEnumDisplayName(WeaponEnum.Overheat_Nailgun, "Overheat Nailgun");
        field.SetEnumDisplayName(WeaponEnum.Jumpstart_Nailgun, "Jumpstart Nailgun");
        field.SetEnumDisplayName(WeaponEnum.Electric_Railcannon, "Electric Railcannon");
        field.SetEnumDisplayName(WeaponEnum.Screwdriver, "Screwdriver");
        field.SetEnumDisplayName(WeaponEnum.Malicious_Railcannon, "Malicious Railcannon");
        field.SetEnumDisplayName(WeaponEnum.Freezeframe_Rocket_Launcher, "Freezeframe Rocket Launcher");
        field.SetEnumDisplayName(WeaponEnum.SRS_Rocket_Launcher, "S.R.S. Rocket Launcher");
        field.SetEnumDisplayName(WeaponEnum.Firestarter_Rocket_Launcher, "Firestarter Rocket Launcher");
    }

    public static int[] convertWeaponEnumToSlotVariation(WeaponEnum code)
    {
        //arr[0] is slot, arr[1] is weapon variation
        int[] arr = {50,50}; //causes errors if this value isnt ever overwritten
        arr[0] = (int)(code - 1) / 3;
        for(int i = 0; i < 3; i++)
        {
            //REVOLVER
            if(MonoSingleton<GunControl>.Instance.slot1[i].GetComponent<Revolver>().gunVariation == 0 && code == WeaponEnum.Piercer_Revolver) {arr[1] = i;}
            if(MonoSingleton<GunControl>.Instance.slot1[i].GetComponent<Revolver>().gunVariation == 1 && code == WeaponEnum.Marksman_Revolver) {arr[1] = i;}
            if(MonoSingleton<GunControl>.Instance.slot1[i].GetComponent<Revolver>().gunVariation == 2 && code == WeaponEnum.Sharpshooter_Revolver) {arr[1] = i;}
            //SHOTGUN
            if(MonoSingleton<GunControl>.Instance.slot2[i].GetComponent<Shotgun>() != null) 
            {
                if(MonoSingleton<GunControl>.Instance.slot2[i].GetComponent<Shotgun>().variation == 0 && code == WeaponEnum.Core_Eject_Shotgun) {arr[1] = i;}
            }
            else if(MonoSingleton<GunControl>.Instance.slot2[i].GetComponent<ShotgunHammer>() != null) 
            {
                if(MonoSingleton<GunControl>.Instance.slot2[i].GetComponent<ShotgunHammer>().variation == 0 && code == WeaponEnum.Core_Eject_Shotgun) {arr[1] = i;}
            }
            if(MonoSingleton<GunControl>.Instance.slot2[i].GetComponent<Shotgun>() != null) 
            {
                if(MonoSingleton<GunControl>.Instance.slot2[i].GetComponent<Shotgun>().variation == 1 && code == WeaponEnum.Pump_Charge_Shotgun) {arr[1] = i;}
            }
            else if(MonoSingleton<GunControl>.Instance.slot2[i].GetComponent<ShotgunHammer>() != null) 
            {
                if(MonoSingleton<GunControl>.Instance.slot2[i].GetComponent<ShotgunHammer>().variation == 1 && code == WeaponEnum.Pump_Charge_Shotgun) {arr[1] = i;}
            }
            if(MonoSingleton<GunControl>.Instance.slot2[i].GetComponent<Shotgun>() != null) 
            {
                if(MonoSingleton<GunControl>.Instance.slot2[i].GetComponent<Shotgun>().variation == 2 && code == WeaponEnum.Sawed_On_Shotgun) {arr[1] = i;}
            }
            else if(MonoSingleton<GunControl>.Instance.slot2[i].GetComponent<ShotgunHammer>() != null) 
            {
                if(MonoSingleton<GunControl>.Instance.slot2[i].GetComponent<ShotgunHammer>().variation == 2 && code == WeaponEnum.Sawed_On_Shotgun) {arr[1] = i;}
            }

            //NAILGUN
            //hakita didnt order these the way I expected
            if(MonoSingleton<GunControl>.Instance.slot3[i].GetComponent<Nailgun>().variation == 1 && code == WeaponEnum.Attractor_Nailgun) {arr[1] = i;}
            if(MonoSingleton<GunControl>.Instance.slot3[i].GetComponent<Nailgun>().variation == 0 && code == WeaponEnum.Overheat_Nailgun) {arr[1] = i;}
            if(MonoSingleton<GunControl>.Instance.slot3[i].GetComponent<Nailgun>().variation == 2 && code == WeaponEnum.Jumpstart_Nailgun) {arr[1] = i;}

            //RAILCANNON
            if(MonoSingleton<GunControl>.Instance.slot4[i].GetComponent<Railcannon>().variation == 0 && code == WeaponEnum.Electric_Railcannon) {arr[1] = i;}
            if(MonoSingleton<GunControl>.Instance.slot4[i].GetComponent<Railcannon>().variation == 1 && code == WeaponEnum.Screwdriver) {arr[1] = i;}
            if(MonoSingleton<GunControl>.Instance.slot4[i].GetComponent<Railcannon>().variation == 2 && code == WeaponEnum.Malicious_Railcannon) {arr[1] = i;}

            //ROCKETLAUNCHER
            if(MonoSingleton<GunControl>.Instance.slot5[i].GetComponent<RocketLauncher>().variation == 0 && code == WeaponEnum.Freezeframe_Rocket_Launcher) {arr[1] = i;}
            if(MonoSingleton<GunControl>.Instance.slot5[i].GetComponent<RocketLauncher>().variation == 1 && code == WeaponEnum.SRS_Rocket_Launcher) {arr[1] = i;}
            if(MonoSingleton<GunControl>.Instance.slot5[i].GetComponent<RocketLauncher>().variation == 2 && code == WeaponEnum.Firestarter_Rocket_Launcher) {arr[1] = i;}
        }
        
        return arr;
    }

    public enum KeyEnum
    {
        None, Backspace, Tab, Escape, Space, UpArrow, DownArrow, RightArrow, LeftArrow, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, 
        Alpha1, Alpha2, Alpha3, Alpha4, Alpha5, Alpha6, Alpha7, Alpha8, Alpha9, Alpha0, CapsLock,
        RightShift, LeftShift, RightControl, LeftControl, RightAlt, LeftAlt, Mouse1, Mouse2, Mouse3, Mouse4, Mouse5, Mouse6, Mouse7, //following is courtesy of tetriscat
        BackQuote, EqualsSign, Minus, LeftBracket, RightBracket, Semicolon, Quote, Comma, Period, Slash, Backslash, 
		Numlock, KeypadDivide, KeypadMultiply, KeypadMinus, KeypadPlus, KeypadEnter, KeypadPeriod, 
		Keypad0, Keypad1, Keypad2, Keypad3, Keypad4, Keypad5, Keypad6, Keypad7, Keypad8, Keypad9, 
		Home, End, PageUp, PageDown, Enter, 
		F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, F13, F14, F15
    }
    public static KeyCode convertKeyEnumToKeyCode(KeyEnum value)
    {
        KeyCode code = KeyCode.None;
        if (value.Equals(KeyEnum.Backspace)) {code = KeyCode.Backspace;}
        else if (value.Equals(KeyEnum.Tab)) {code = KeyCode.Tab;}
        else if (value.Equals(KeyEnum.Escape)) {code = KeyCode.Escape;}
        else if (value.Equals(KeyEnum.Space)) {code = KeyCode.Space;}
        else if (value.Equals(KeyEnum.UpArrow)) {code = KeyCode.UpArrow;}
        else if (value.Equals(KeyEnum.DownArrow)) {code = KeyCode.DownArrow;}
        else if (value.Equals(KeyEnum.RightArrow)) {code = KeyCode.RightArrow;}
        else if (value.Equals(KeyEnum.LeftArrow)) {code = KeyCode.LeftArrow;}
        else if (value.Equals(KeyEnum.A)) {code = KeyCode.A;}
        else if (value.Equals(KeyEnum.B)) {code = KeyCode.B;}
        else if (value.Equals(KeyEnum.C)) {code = KeyCode.C;}
        else if (value.Equals(KeyEnum.D)) {code = KeyCode.D;}
        else if (value.Equals(KeyEnum.E)) {code = KeyCode.E;}
        else if (value.Equals(KeyEnum.F)) {code = KeyCode.F;}
        else if (value.Equals(KeyEnum.G)) {code = KeyCode.G;}
        else if (value.Equals(KeyEnum.H)) {code = KeyCode.H;}
        else if (value.Equals(KeyEnum.I)) {code = KeyCode.I;}
        else if (value.Equals(KeyEnum.J)) {code = KeyCode.J;}
        else if (value.Equals(KeyEnum.K)) {code = KeyCode.K;}
        else if (value.Equals(KeyEnum.L)) {code = KeyCode.L;}
        else if (value.Equals(KeyEnum.M)) {code = KeyCode.M;}
        else if (value.Equals(KeyEnum.N)) {code = KeyCode.N;}
        else if (value.Equals(KeyEnum.O)) {code = KeyCode.O;}
        else if (value.Equals(KeyEnum.P)) {code = KeyCode.P;}
        else if (value.Equals(KeyEnum.Q)) {code = KeyCode.Q;}
        else if (value.Equals(KeyEnum.R)) {code = KeyCode.R;}
        else if (value.Equals(KeyEnum.S)) {code = KeyCode.S;}
        else if (value.Equals(KeyEnum.T)) {code = KeyCode.T;}
        else if (value.Equals(KeyEnum.U)) {code = KeyCode.U;}
        else if (value.Equals(KeyEnum.V)) {code = KeyCode.V;}
        else if (value.Equals(KeyEnum.W)) {code = KeyCode.W;}
        else if (value.Equals(KeyEnum.X)) {code = KeyCode.X;}
        else if (value.Equals(KeyEnum.Y)) {code = KeyCode.Y;}
        else if (value.Equals(KeyEnum.Z)) {code = KeyCode.Z;}
        else if (value.Equals(KeyEnum.Alpha1)) {code = KeyCode.Alpha1;}
        else if (value.Equals(KeyEnum.Alpha2)) {code = KeyCode.Alpha2;}
        else if (value.Equals(KeyEnum.Alpha3)) {code = KeyCode.Alpha3;}
        else if (value.Equals(KeyEnum.Alpha4)) {code = KeyCode.Alpha4;}
        else if (value.Equals(KeyEnum.Alpha5)) {code = KeyCode.Alpha5;}
        else if (value.Equals(KeyEnum.Alpha6)) {code = KeyCode.Alpha6;}
        else if (value.Equals(KeyEnum.Alpha7)) {code = KeyCode.Alpha7;}
        else if (value.Equals(KeyEnum.Alpha8)) {code = KeyCode.Alpha8;}
        else if (value.Equals(KeyEnum.Alpha9)) {code = KeyCode.Alpha9;}
        else if (value.Equals(KeyEnum.Alpha0)) {code = KeyCode.Alpha0;}
        else if (value.Equals(KeyEnum.CapsLock)) {code = KeyCode.CapsLock;}
        else if (value.Equals(KeyEnum.RightShift)) {code = KeyCode.RightShift;}
        else if (value.Equals(KeyEnum.LeftShift)) {code = KeyCode.LeftShift;}
        else if (value.Equals(KeyEnum.RightControl)) {code = KeyCode.RightControl;}
        else if (value.Equals(KeyEnum.LeftControl)) {code = KeyCode.LeftControl;}
        else if (value.Equals(KeyEnum.RightAlt)) {code = KeyCode.RightAlt;}
        else if (value.Equals(KeyEnum.LeftAlt)) {code = KeyCode.LeftAlt;}
        else if (value.Equals(KeyEnum.Mouse1)) {code = KeyCode.Mouse0;} //these dont line up
        else if (value.Equals(KeyEnum.Mouse2)) {code = KeyCode.Mouse1;}
        else if (value.Equals(KeyEnum.Mouse3)) {code = KeyCode.Mouse2;}
        else if (value.Equals(KeyEnum.Mouse4)) {code = KeyCode.Mouse3;}
        else if (value.Equals(KeyEnum.Mouse5)) {code = KeyCode.Mouse4;}
        else if (value.Equals(KeyEnum.Mouse6)) {code = KeyCode.Mouse5;}
        else if (value.Equals(KeyEnum.Mouse7)) {code = KeyCode.Mouse6;}
        
        else if(value.Equals(KeyEnum.BackQuote)) {return KeyCode.BackQuote;} 
        else if(value.Equals(KeyEnum.EqualsSign)) {return KeyCode.Equals;} 
        else if(value.Equals(KeyEnum.Minus)) {return KeyCode.Minus;} 
        else if(value.Equals(KeyEnum.LeftBracket)) {return KeyCode.LeftBracket;} 
        else if(value.Equals(KeyEnum.RightBracket)) {return KeyCode.RightBracket;} 
        else if(value.Equals(KeyEnum.Semicolon)) {return KeyCode.Semicolon;} 
        else if(value.Equals(KeyEnum.Quote)) {return KeyCode.Quote;} 
        else if(value.Equals(KeyEnum.Comma)) {return KeyCode.Comma;} 
        else if(value.Equals(KeyEnum.Period)) {return KeyCode.Period;} 
        else if(value.Equals(KeyEnum.Slash)) {return KeyCode.Slash;} 
        else if(value.Equals(KeyEnum.Backslash)) {return KeyCode.Backslash;} 
        else if(value.Equals(KeyEnum.Numlock)) {return KeyCode.Numlock;} 
        else if(value.Equals(KeyEnum.KeypadDivide)) {return KeyCode.KeypadDivide;} 
        else if(value.Equals(KeyEnum.KeypadMultiply)) {return KeyCode.KeypadMultiply;} 
        else if(value.Equals(KeyEnum.KeypadMinus)) {return KeyCode.KeypadMinus;} 
        else if(value.Equals(KeyEnum.KeypadPlus)) {return KeyCode.KeypadPlus;} 
        else if(value.Equals(KeyEnum.KeypadEnter)) {return KeyCode.KeypadEnter;} 
        else if(value.Equals(KeyEnum.KeypadPeriod)) {return KeyCode.KeypadPeriod;} 
        else if(value.Equals(KeyEnum.Keypad0)) {return KeyCode.Keypad0;} 
        else if(value.Equals(KeyEnum.Keypad1)) {return KeyCode.Keypad1;} 
        else if(value.Equals(KeyEnum.Keypad2)) {return KeyCode.Keypad2;} 
        else if(value.Equals(KeyEnum.Keypad3)) {return KeyCode.Keypad3;} 
        else if(value.Equals(KeyEnum.Keypad4)) {return KeyCode.Keypad4;} 
        else if(value.Equals(KeyEnum.Keypad5)) {return KeyCode.Keypad5;} 
        else if(value.Equals(KeyEnum.Keypad6)) {return KeyCode.Keypad6;} 
        else if(value.Equals(KeyEnum.Keypad7)) {return KeyCode.Keypad7;} 
        else if(value.Equals(KeyEnum.Keypad8)) {return KeyCode.Keypad8;} 
        else if(value.Equals(KeyEnum.Keypad9)) {return KeyCode.Keypad9;} 
        else if(value.Equals(KeyEnum.Home)) {return KeyCode.Home;} 
        else if(value.Equals(KeyEnum.End)) {return KeyCode.End;} 
        else if(value.Equals(KeyEnum.PageUp)) {return KeyCode.PageUp;} 
        else if(value.Equals(KeyEnum.PageDown)) {return KeyCode.PageDown;} 
        else if(value.Equals(KeyEnum.Enter)) {return KeyCode.Return;} 
        else if(value.Equals(KeyEnum.F1)) {return KeyCode.F1;} 
        else if(value.Equals(KeyEnum.F2)) {return KeyCode.F2;}
        else if(value.Equals(KeyEnum.F3)) {return KeyCode.F3;} 
        else if(value.Equals(KeyEnum.F4)) {return KeyCode.F4;} 
        else if(value.Equals(KeyEnum.F5)) {return KeyCode.F5;} 
        else if(value.Equals(KeyEnum.F6)) {return KeyCode.F6;} 
        else if(value.Equals(KeyEnum.F7)) {return KeyCode.F7;} 
        else if(value.Equals(KeyEnum.F8)) {return KeyCode.F8;} 
        else if(value.Equals(KeyEnum.F9)) {return KeyCode.F9;} 
        else if(value.Equals(KeyEnum.F10)) {return KeyCode.F10;} 
        else if(value.Equals(KeyEnum.F11)) {return KeyCode.F11;} 
        else if(value.Equals(KeyEnum.F12)) {return KeyCode.F12;} 
        else if(value.Equals(KeyEnum.F13)) {return KeyCode.F13;} 
        else if(value.Equals(KeyEnum.F14)) {return KeyCode.F14;}
        else if(value.Equals(KeyEnum.F15)) {return KeyCode.F15;}
        return code;
    }

    // call for each EnumField (formats display names for some enums) 
    //courtesy of tetriscat
	private static void SetDisplayNames(EnumField<KeyEnum> field) 
    {
		field.SetEnumDisplayName(KeyEnum.Minus, "-");
		field.SetEnumDisplayName(KeyEnum.EqualsSign, "=");
		field.SetEnumDisplayName(KeyEnum.LeftBracket, "[");
		field.SetEnumDisplayName(KeyEnum.RightBracket, "]");
		field.SetEnumDisplayName(KeyEnum.CapsLock, "Caps Lock");
		field.SetEnumDisplayName(KeyEnum.LeftShift, "Left Shift");
		field.SetEnumDisplayName(KeyEnum.RightShift, "Right Shift");
		field.SetEnumDisplayName(KeyEnum.LeftControl, "Left Control");
		field.SetEnumDisplayName(KeyEnum.RightControl, "Right Control");
		field.SetEnumDisplayName(KeyEnum.LeftAlt, "Left Alt");
		field.SetEnumDisplayName(KeyEnum.RightAlt, "Right Alt");
		field.SetEnumDisplayName(KeyEnum.BackQuote, "`");
		field.SetEnumDisplayName(KeyEnum.Quote, "'");
		field.SetEnumDisplayName(KeyEnum.Semicolon, ";");
		field.SetEnumDisplayName(KeyEnum.Slash, "/");
		field.SetEnumDisplayName(KeyEnum.Backslash, "\\");
		field.SetEnumDisplayName(KeyEnum.Keypad0, "Keypad 0");
		field.SetEnumDisplayName(KeyEnum.Keypad1, "Keypad 1");
		field.SetEnumDisplayName(KeyEnum.Keypad2, "Keypad 2");
		field.SetEnumDisplayName(KeyEnum.Keypad3, "Keypad 3");
		field.SetEnumDisplayName(KeyEnum.Keypad4, "Keypad 4");
		field.SetEnumDisplayName(KeyEnum.Keypad5, "Keypad 5");
		field.SetEnumDisplayName(KeyEnum.Keypad6, "Keypad 6");
		field.SetEnumDisplayName(KeyEnum.Keypad7, "Keypad 7");
		field.SetEnumDisplayName(KeyEnum.Keypad8, "Keypad 8");
		field.SetEnumDisplayName(KeyEnum.Keypad9, "Keypad 9");
		field.SetEnumDisplayName(KeyEnum.KeypadDivide, "Keypad /");
		field.SetEnumDisplayName(KeyEnum.KeypadMultiply, "Keypad *");
		field.SetEnumDisplayName(KeyEnum.KeypadPlus, "Keypad Plus");
		field.SetEnumDisplayName(KeyEnum.KeypadMinus, "Keypad Minus");
		field.SetEnumDisplayName(KeyEnum.KeypadEnter, "Keypad Enter");
		field.SetEnumDisplayName(KeyEnum.KeypadPeriod, "Keypad .");
		field.SetEnumDisplayName(KeyEnum.Period, ".");
		field.SetEnumDisplayName(KeyEnum.Comma, ",");
		field.SetEnumDisplayName(KeyEnum.PageUp, "Page Up");
		field.SetEnumDisplayName(KeyEnum.PageDown, "Page Down");
		field.SetEnumDisplayName(KeyEnum.UpArrow, "Up Arrow");
		field.SetEnumDisplayName(KeyEnum.DownArrow, "Down Arrow");
		field.SetEnumDisplayName(KeyEnum.LeftArrow, "Left Arrow");
		field.SetEnumDisplayName(KeyEnum.RightArrow, "Right Arrow");
		field.SetEnumDisplayName(KeyEnum.Alpha0, "0");
		field.SetEnumDisplayName(KeyEnum.Alpha1, "1");
		field.SetEnumDisplayName(KeyEnum.Alpha2, "2");
		field.SetEnumDisplayName(KeyEnum.Alpha3, "3");
		field.SetEnumDisplayName(KeyEnum.Alpha4, "4");
		field.SetEnumDisplayName(KeyEnum.Alpha5, "5");
		field.SetEnumDisplayName(KeyEnum.Alpha6, "6");
		field.SetEnumDisplayName(KeyEnum.Alpha7, "7");
		field.SetEnumDisplayName(KeyEnum.Alpha8, "8");
		field.SetEnumDisplayName(KeyEnum.Alpha9, "9");
		field.SetEnumDisplayName(KeyEnum.Mouse1, "Mouse 1");
		field.SetEnumDisplayName(KeyEnum.Mouse2, "Mouse 2");
		field.SetEnumDisplayName(KeyEnum.Mouse3, "Mouse 3");
		field.SetEnumDisplayName(KeyEnum.Mouse4, "Mouse 4");
		field.SetEnumDisplayName(KeyEnum.Mouse5, "Mouse 5");
		field.SetEnumDisplayName(KeyEnum.Mouse6, "Mouse 6");
		field.SetEnumDisplayName(KeyEnum.Mouse7, "Mouse 7");
	}
    public static string DefaultParentFolder = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}";
    public static WeaponCycle[] weaponCycles = new WeaponCycle[15];

    public static void WeaponVariantBindsConfig()
    {
        var config = PluginConfigurator.Create("WeaponVariantBinds", "WeaponVariantBinds");
        config.SetIconWithURL($"{Path.Combine(DefaultParentFolder!, "icon.png")}");

        BoolField enabledField = new BoolField(config.rootPanel, "Mod Enabled", "modEnabled", true);

        ConfigDivision division = new ConfigDivision(config.rootPanel, "division");
        enabledField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.modEnabled = e.value; division.interactable = e.value;};
        Plugin.modEnabled = enabledField.value; division.interactable = enabledField.value; 

        //+----------------------+\\
        //| CUSTOM WEAPON CYCLES |\\
        //+----------------------+\\

        //ConfigPanel customWeaponCyclePanel = new ConfigPanel(division, "Custom Weapon Cycles", "customWeaponCyclePanel");
        ConfigHeader customWeaponCycleHeader = new ConfigHeader(division, "Weapon cycles probably work how you expect, but consult the readme if you need help. Mixing vanilla weapon switch binds and these weapon cycles may not work perfectly.");
        customWeaponCycleHeader.textSize = 14;

        for(int i = 1; i <= weaponCycles.Length; i++)
        {
            weaponCycles[i - 1] = new WeaponCycle();
            WeaponCycle weaponCycle = weaponCycles[i - 1];
            for(int j = 0; j < weaponCycle.weaponEnums.Length; j++)
            {
                weaponCycle.weaponEnums[j] = WeaponEnum.None;
            }

            ConfigPanel newWeaponCyclePanel = new ConfigPanel(division, "Weapon Cycle " + i, "customWeaponCyclePanel" + i);
            if(i % 2 == 1) {newWeaponCyclePanel.fieldColor = new Color(0.06f, 0.06f, 0.06f);}

            EnumField<KeyEnum> useKeyField = new EnumField<KeyEnum>(newWeaponCyclePanel, "Weapon Cycle Key", "customWeaponCycleKey" + i, KeyEnum.None);
            useKeyField.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {weaponCycle.useCode = convertKeyEnumToKeyCode(e.value);};
            weaponCycle.useCode = convertKeyEnumToKeyCode(useKeyField.value);
            SetDisplayNames(useKeyField);

            BoolField scrollThroughWeaponCycleField = new BoolField(newWeaponCyclePanel, "Scroll Through Cycle", "scrollThroughWeaponCycle" + i, true);
            scrollThroughWeaponCycleField.onValueChange += (BoolField.BoolValueChangeEvent e) => {weaponCycle.scrollThroughWeaponCycle = e.value;};
            weaponCycle.scrollThroughWeaponCycle = scrollThroughWeaponCycleField.value;

            BoolField swapThroughWeaponCycleField = new BoolField(newWeaponCyclePanel, "Swap Variant Through Cycle", "swapThroughWeaponCycle" + i, true);
            swapThroughWeaponCycleField.onValueChange += (BoolField.BoolValueChangeEvent e) => {weaponCycle.swapThroughWeaponCycle = e.value;};
            weaponCycle.swapThroughWeaponCycle = swapThroughWeaponCycleField.value;

            BoolField specificVariationThroughWeaponCycleField = new BoolField(newWeaponCyclePanel, "Specific Variant Bind In Cycle", "specificVariationThroughWeaponCycle" + i, true);
            specificVariationThroughWeaponCycleField.onValueChange += (BoolField.BoolValueChangeEvent e) => {weaponCycle.specificVariationThroughWeaponCycle = e.value;};
            weaponCycle.specificVariationThroughWeaponCycle = specificVariationThroughWeaponCycleField.value;

            BoolField rememberCycleVariationField = new BoolField(newWeaponCyclePanel, "Remember Cycle Variation", "rememberCycleVariation" + i, false);
            rememberCycleVariationField.onValueChange += (BoolField.BoolValueChangeEvent e) => {weaponCycle.rememberVariation = e.value;};
            weaponCycle.rememberVariation = rememberCycleVariationField.value;

            EnumField<SwapBehaviorEnum> swapBehaviorField = new EnumField<SwapBehaviorEnum>(newWeaponCyclePanel, "On Swap to Already Drawn Weapon", "swapBehavior" + i, SwapBehaviorEnum.NextVariation);
            swapBehaviorField.onValueChange += (EnumField<SwapBehaviorEnum>.EnumValueChangeEvent e) => {weaponCycle.swapBehavior = e.value;};
            weaponCycle.swapBehavior = swapBehaviorField.value;
            swapBehaviorField.SetEnumDisplayName(SwapBehaviorEnum.FirstVariation, "First Variation");
            swapBehaviorField.SetEnumDisplayName(SwapBehaviorEnum.NextVariation, "Next Variation");
            swapBehaviorField.SetEnumDisplayName(SwapBehaviorEnum.SameVariation, "Same Variation");

            ConfigHeader customWeaponCycleWeaponsHeader = new ConfigHeader(newWeaponCyclePanel, "Weapons");

            EnumField<WeaponEnum> weaponField1 = new EnumField<WeaponEnum>(newWeaponCyclePanel, "Weapon " + 1, "customWeaponCycle" + i + "weapon" + 1, WeaponEnum.None);
            weaponField1.onValueChange += (EnumField<WeaponEnum>.EnumValueChangeEvent e) => {weaponCycle.weaponEnums[0] = e.value;};
            weaponCycle.weaponEnums[0] =  weaponField1.value;
            setWeaponEnumDisplayNames(weaponField1);
            weaponField1.fieldColor = new Color(0.06f,0.06f,0.06f);

            EnumField<WeaponEnum> weaponField2 = new EnumField<WeaponEnum>(newWeaponCyclePanel, "Weapon " + 2, "customWeaponCycle" + i + "weapon" + 2, WeaponEnum.None);
            weaponField2.onValueChange += (EnumField<WeaponEnum>.EnumValueChangeEvent e) => {weaponCycle.weaponEnums[1] = e.value;};
            weaponCycle.weaponEnums[1] =  weaponField2.value;
            setWeaponEnumDisplayNames(weaponField2);

            EnumField<WeaponEnum> weaponField3 = new EnumField<WeaponEnum>(newWeaponCyclePanel, "Weapon " + 3, "customWeaponCycle" + i + "weapon" + 3, WeaponEnum.None);
            weaponField3.onValueChange += (EnumField<WeaponEnum>.EnumValueChangeEvent e) => {weaponCycle.weaponEnums[2] = e.value;};
            weaponCycle.weaponEnums[2] =  weaponField3.value;
            setWeaponEnumDisplayNames(weaponField3);
            weaponField3.fieldColor = new Color(0.06f,0.06f,0.06f);

            EnumField<WeaponEnum> weaponField4 = new EnumField<WeaponEnum>(newWeaponCyclePanel, "Weapon " + 4, "customWeaponCycle" + i + "weapon" + 4, WeaponEnum.None);
            weaponField4.onValueChange += (EnumField<WeaponEnum>.EnumValueChangeEvent e) => {weaponCycle.weaponEnums[3] = e.value;};
            weaponCycle.weaponEnums[3] =  weaponField4.value;
            setWeaponEnumDisplayNames(weaponField4);

            EnumField<WeaponEnum> weaponField5 = new EnumField<WeaponEnum>(newWeaponCyclePanel, "Weapon " + 5, "customWeaponCycle" + i + "weapon" + 5, WeaponEnum.None);
            weaponField5.onValueChange += (EnumField<WeaponEnum>.EnumValueChangeEvent e) => {weaponCycle.weaponEnums[4] = e.value;};
            weaponCycle.weaponEnums[4] =  weaponField5.value;
            setWeaponEnumDisplayNames(weaponField5);
            weaponField5.fieldColor = new Color(0.06f,0.06f,0.06f);

            EnumField<WeaponEnum> weaponField6 = new EnumField<WeaponEnum>(newWeaponCyclePanel, "Weapon " + 6, "customWeaponCycle" + i + "weapon" + 6, WeaponEnum.None);
            weaponField6.onValueChange += (EnumField<WeaponEnum>.EnumValueChangeEvent e) => {weaponCycle.weaponEnums[5] = e.value;};
            weaponCycle.weaponEnums[5] =  weaponField6.value;
            setWeaponEnumDisplayNames(weaponField6);

            EnumField<WeaponEnum> weaponField7 = new EnumField<WeaponEnum>(newWeaponCyclePanel, "Weapon " + 7, "customWeaponCycle" + i + "weapon" + 7, WeaponEnum.None);
            weaponField7.onValueChange += (EnumField<WeaponEnum>.EnumValueChangeEvent e) => {weaponCycle.weaponEnums[6] = e.value;};
            weaponCycle.weaponEnums[6] =  weaponField7.value;
            setWeaponEnumDisplayNames(weaponField7);
            weaponField7.fieldColor = new Color(0.06f,0.06f,0.06f);

            EnumField<WeaponEnum> weaponField8 = new EnumField<WeaponEnum>(newWeaponCyclePanel, "Weapon " + 8, "customWeaponCycle" + i + "weapon" + 8, WeaponEnum.None);
            weaponField8.onValueChange += (EnumField<WeaponEnum>.EnumValueChangeEvent e) => {weaponCycle.weaponEnums[7] = e.value;};
            weaponCycle.weaponEnums[7] =  weaponField8.value;
            setWeaponEnumDisplayNames(weaponField8);

            EnumField<WeaponEnum> weaponField9 = new EnumField<WeaponEnum>(newWeaponCyclePanel, "Weapon " + 9, "customWeaponCycle" + i + "weapon" + 9, WeaponEnum.None);
            weaponField9.onValueChange += (EnumField<WeaponEnum>.EnumValueChangeEvent e) => {weaponCycle.weaponEnums[8] = e.value;};
            weaponCycle.weaponEnums[8] =  weaponField9.value;
            setWeaponEnumDisplayNames(weaponField9);
            weaponField9.fieldColor = new Color(0.06f,0.06f,0.06f);

            EnumField<WeaponEnum> weaponField10 = new EnumField<WeaponEnum>(newWeaponCyclePanel, "Weapon " + 10, "customWeaponCycle" + i + "weapon" + 10, WeaponEnum.None);
            weaponField10.onValueChange += (EnumField<WeaponEnum>.EnumValueChangeEvent e) => {weaponCycle.weaponEnums[9] = e.value;};
            weaponCycle.weaponEnums[9] =  weaponField10.value;
            setWeaponEnumDisplayNames(weaponField10);
        }
    }
}