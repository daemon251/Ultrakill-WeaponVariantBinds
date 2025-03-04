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
using UnityEngine.Animations;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UIElements;

namespace WeaponVariantBinds;

public enum SwapBehaviorEnum{NextVariation, SameVariation, FirstVariation}

public class WeaponCycle
{   
    //enabled field used to be here but pointless because you can just set the use code to none
    public PluginConfig.WeaponEnum[] weaponEnums = new PluginConfig.WeaponEnum[100];
    public bool[] ignoreInCycle = new bool[100];
    public int currentIndex = 0;
    public KeyCode useCode = KeyCode.None;
    public bool rememberVariation = false;
    public SwapBehaviorEnum swapBehavior = SwapBehaviorEnum.NextVariation;
    public WeaponCycle() {}
}

public class PluginConfig
{
    public static KeyCode[] variationBinds = new KeyCode[100]; //index 0, 1, 2 will be empty (these are the vanilla binds so)
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

    public static WeaponEnum convertWeaponToWeaponEnum(GameObject weapon)
    {
        WeaponEnum weaponEnum = WeaponEnum.None;
        if(weapon.GetComponent<Revolver>() != null)
        {
            if(weapon.GetComponent<Revolver>().gunVariation == 0) {weaponEnum = WeaponEnum.Piercer_Revolver;}
            if(weapon.GetComponent<Revolver>().gunVariation == 1) {weaponEnum = WeaponEnum.Marksman_Revolver;}
            if(weapon.GetComponent<Revolver>().gunVariation == 2) {weaponEnum = WeaponEnum.Sharpshooter_Revolver;}
        }
        if(weapon.GetComponent<Shotgun>() != null)
        {
            if(weapon.GetComponent<Shotgun>().variation == 0) {weaponEnum = WeaponEnum.Core_Eject_Shotgun;}
            if(weapon.GetComponent<Shotgun>().variation == 1) {weaponEnum = WeaponEnum.Pump_Charge_Shotgun;}
            if(weapon.GetComponent<Shotgun>().variation == 2) {weaponEnum = WeaponEnum.Sawed_On_Shotgun;}
        }
        if(weapon.GetComponent<ShotgunHammer>() != null)
        {
            if(weapon.GetComponent<ShotgunHammer>().variation == 0) {weaponEnum = WeaponEnum.Core_Eject_Shotgun;}
            if(weapon.GetComponent<ShotgunHammer>().variation == 1) {weaponEnum = WeaponEnum.Pump_Charge_Shotgun;}
            if(weapon.GetComponent<ShotgunHammer>().variation == 2) {weaponEnum = WeaponEnum.Sawed_On_Shotgun;}
        }
        if(weapon.GetComponent<Nailgun>() != null)
        {
            if(weapon.GetComponent<Nailgun>().variation == 1) {weaponEnum = WeaponEnum.Attractor_Nailgun;}
            if(weapon.GetComponent<Nailgun>().variation == 0) {weaponEnum = WeaponEnum.Overheat_Nailgun;}
            if(weapon.GetComponent<Nailgun>().variation == 2) {weaponEnum = WeaponEnum.Jumpstart_Nailgun;}
        }
        if(weapon.GetComponent<Railcannon>() != null)
        {
            if(weapon.GetComponent<Railcannon>().variation == 0) {weaponEnum = WeaponEnum.Electric_Railcannon;}
            if(weapon.GetComponent<Railcannon>().variation == 1) {weaponEnum = WeaponEnum.Screwdriver;}
            if(weapon.GetComponent<Railcannon>().variation == 2) {weaponEnum = WeaponEnum.Malicious_Railcannon;}
        }
        if(weapon.GetComponent<RocketLauncher>() != null)
        {
            if(weapon.GetComponent<RocketLauncher>().variation == 0) {weaponEnum = WeaponEnum.Freezeframe_Rocket_Launcher;}
            if(weapon.GetComponent<RocketLauncher>().variation == 1) {weaponEnum = WeaponEnum.SRS_Rocket_Launcher;}
            if(weapon.GetComponent<RocketLauncher>().variation == 2) {weaponEnum = WeaponEnum.Firestarter_Rocket_Launcher;}
        }
        return weaponEnum;
    }
    public static WeaponEnum convertWeaponToWeaponEnum(int slot, int variation)
    {
        WeaponEnum weaponEnum = WeaponEnum.None;
        if(slot == 1)
        {
            if(variation == 0) {weaponEnum = WeaponEnum.Piercer_Revolver;}
            else if(variation == 1) {weaponEnum = WeaponEnum.Marksman_Revolver;}
            else if(variation == 2) {weaponEnum = WeaponEnum.Sharpshooter_Revolver;}
        }
        else if(slot == 2)
        {
            if(variation == 0) {weaponEnum = WeaponEnum.Core_Eject_Shotgun;}
            else if(variation == 1) {weaponEnum = WeaponEnum.Pump_Charge_Shotgun;}
            else if(variation == 2) {weaponEnum = WeaponEnum.Sawed_On_Shotgun;}
        }
        else if(slot == 3)
        {
            if(variation == 0) {weaponEnum = WeaponEnum.Overheat_Nailgun;}
            else if(variation == 1) {weaponEnum = WeaponEnum.Attractor_Nailgun;}
            else if(variation == 2) {weaponEnum = WeaponEnum.Jumpstart_Nailgun;}
        }
        else if(slot == 4)
        {
            if(variation == 0) {weaponEnum = WeaponEnum.Electric_Railcannon;}
            else if(variation == 1) {weaponEnum = WeaponEnum.Malicious_Railcannon;}
            else if(variation == 2) {weaponEnum = WeaponEnum.Screwdriver;}            
        }
        else if(slot == 5)
        {
            if(variation == 0) {weaponEnum = WeaponEnum.Freezeframe_Rocket_Launcher;}
            else if(variation == 1) {weaponEnum = WeaponEnum.SRS_Rocket_Launcher;}
            else if(variation == 2) {weaponEnum = WeaponEnum.Firestarter_Rocket_Launcher;}            
        }
        return weaponEnum;
    }
    public static int[] convertWeaponEnumToSlotVariation(WeaponEnum code)
    {
        //arr[0] is slot, arr[1] is weapon variation
        int[] arr = {500,500}; //causes errors if this value isnt ever overwritten
        arr[0] = (int)(code - 1) / 3;
        arr[1] = -1;
        for(int i = 0; i < 3; i++)
        {
            //REVOLVER
            if(i < MonoSingleton<GunControl>.Instance.slot1.Count)
            {
                if(MonoSingleton<GunControl>.Instance.slot1[i].GetComponent<Revolver>().gunVariation == 0 && code == WeaponEnum.Piercer_Revolver) {arr[1] = i;}
                if(MonoSingleton<GunControl>.Instance.slot1[i].GetComponent<Revolver>().gunVariation == 1 && code == WeaponEnum.Marksman_Revolver) {arr[1] = i;}
                if(MonoSingleton<GunControl>.Instance.slot1[i].GetComponent<Revolver>().gunVariation == 2 && code == WeaponEnum.Sharpshooter_Revolver) {arr[1] = i;}
            }
            //SHOTGUN
            if(i < MonoSingleton<GunControl>.Instance.slot2.Count)
            {
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
            }

            //NAILGUN
            //hakita didnt order these the way I expected
            if(i < MonoSingleton<GunControl>.Instance.slot3.Count)
            {
                if(MonoSingleton<GunControl>.Instance.slot3[i].GetComponent<Nailgun>().variation == 1 && code == WeaponEnum.Attractor_Nailgun) {arr[1] = i;}
                if(MonoSingleton<GunControl>.Instance.slot3[i].GetComponent<Nailgun>().variation == 0 && code == WeaponEnum.Overheat_Nailgun) {arr[1] = i;}
                if(MonoSingleton<GunControl>.Instance.slot3[i].GetComponent<Nailgun>().variation == 2 && code == WeaponEnum.Jumpstart_Nailgun) {arr[1] = i;}
            }

            //RAILCANNON
            if(i < MonoSingleton<GunControl>.Instance.slot4.Count)
            {
                if(MonoSingleton<GunControl>.Instance.slot4[i].GetComponent<Railcannon>().variation == 0 && code == WeaponEnum.Electric_Railcannon) {arr[1] = i;}
                if(MonoSingleton<GunControl>.Instance.slot4[i].GetComponent<Railcannon>().variation == 1 && code == WeaponEnum.Screwdriver) {arr[1] = i;}
                if(MonoSingleton<GunControl>.Instance.slot4[i].GetComponent<Railcannon>().variation == 2 && code == WeaponEnum.Malicious_Railcannon) {arr[1] = i;}
            }

            //ROCKETLAUNCHER
            if(i < MonoSingleton<GunControl>.Instance.slot5.Count)
            {
                if(MonoSingleton<GunControl>.Instance.slot5[i].GetComponent<RocketLauncher>().variation == 0 && code == WeaponEnum.Freezeframe_Rocket_Launcher) {arr[1] = i;}
                if(MonoSingleton<GunControl>.Instance.slot5[i].GetComponent<RocketLauncher>().variation == 1 && code == WeaponEnum.SRS_Rocket_Launcher) {arr[1] = i;}
                if(MonoSingleton<GunControl>.Instance.slot5[i].GetComponent<RocketLauncher>().variation == 2 && code == WeaponEnum.Firestarter_Rocket_Launcher) {arr[1] = i;}
            }
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
		F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, F13, F14, F15,
        WheelUp, WheelDown        
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

    //call for each EnumField (formats display names for some enums) 
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
    public static WeaponCycle[] weaponCycles = new WeaponCycle[100];
    public static WeaponCycle[] vanillaWeaponCycles = new WeaponCycle[5]; //0 indexed
    public static void InitVanillaWeaponCycles()
    {
        for(int i = 0; i < 5; i++)
        {
            WeaponCycle wc = new WeaponCycle();
            wc.weaponEnums = new PluginConfig.WeaponEnum[100];
            for(int j = 0; j < 100; j++)
            {
                wc.weaponEnums[i] = WeaponEnum.None;
            }
            wc.currentIndex = 0;
            wc.useCode = KeyCode.None;
            wc.rememberVariation = false; 
            wc.swapBehavior = SwapBehaviorEnum.NextVariation;
            vanillaWeaponCycles[i] = wc;
        }
    }
    public static void DetermineVanillaWeaponCycles()
    {   
        if(MonoSingleton<GunControl>.Instance != null) 
        {
            if(MonoSingleton<GunControl>.Instance.slot1.Count > 0) {vanillaWeaponCycles[0].weaponEnums[0] = convertWeaponToWeaponEnum(MonoSingleton<GunControl>.Instance.slot1[0]);}
            if(MonoSingleton<GunControl>.Instance.slot1.Count > 1) {vanillaWeaponCycles[0].weaponEnums[1] = convertWeaponToWeaponEnum(MonoSingleton<GunControl>.Instance.slot1[1]);}
            if(MonoSingleton<GunControl>.Instance.slot1.Count > 2) {vanillaWeaponCycles[0].weaponEnums[2] = convertWeaponToWeaponEnum(MonoSingleton<GunControl>.Instance.slot1[2]);}

            if(MonoSingleton<GunControl>.Instance.slot2.Count > 0) {vanillaWeaponCycles[1].weaponEnums[0] = convertWeaponToWeaponEnum(MonoSingleton<GunControl>.Instance.slot2[0]);}
            if(MonoSingleton<GunControl>.Instance.slot2.Count > 1) {vanillaWeaponCycles[1].weaponEnums[1] = convertWeaponToWeaponEnum(MonoSingleton<GunControl>.Instance.slot2[1]);}
            if(MonoSingleton<GunControl>.Instance.slot2.Count > 2) {vanillaWeaponCycles[1].weaponEnums[2] = convertWeaponToWeaponEnum(MonoSingleton<GunControl>.Instance.slot2[2]);}

            if(MonoSingleton<GunControl>.Instance.slot3.Count > 0) {vanillaWeaponCycles[2].weaponEnums[0] = convertWeaponToWeaponEnum(MonoSingleton<GunControl>.Instance.slot3[0]);}
            if(MonoSingleton<GunControl>.Instance.slot3.Count > 1) {vanillaWeaponCycles[2].weaponEnums[1] = convertWeaponToWeaponEnum(MonoSingleton<GunControl>.Instance.slot3[1]);}
            if(MonoSingleton<GunControl>.Instance.slot3.Count > 2) {vanillaWeaponCycles[2].weaponEnums[2] = convertWeaponToWeaponEnum(MonoSingleton<GunControl>.Instance.slot3[2]);}

            if(MonoSingleton<GunControl>.Instance.slot4.Count > 0) {vanillaWeaponCycles[3].weaponEnums[0] = convertWeaponToWeaponEnum(MonoSingleton<GunControl>.Instance.slot4[0]);}
            if(MonoSingleton<GunControl>.Instance.slot4.Count > 1) {vanillaWeaponCycles[3].weaponEnums[1] = convertWeaponToWeaponEnum(MonoSingleton<GunControl>.Instance.slot4[1]);}
            if(MonoSingleton<GunControl>.Instance.slot4.Count > 2) {vanillaWeaponCycles[3].weaponEnums[2] = convertWeaponToWeaponEnum(MonoSingleton<GunControl>.Instance.slot4[2]);}

            if(MonoSingleton<GunControl>.Instance.slot5.Count > 0) {vanillaWeaponCycles[4].weaponEnums[0] = convertWeaponToWeaponEnum(MonoSingleton<GunControl>.Instance.slot5[0]);}
            if(MonoSingleton<GunControl>.Instance.slot5.Count > 1) {vanillaWeaponCycles[4].weaponEnums[1] = convertWeaponToWeaponEnum(MonoSingleton<GunControl>.Instance.slot5[1]);}
            if(MonoSingleton<GunControl>.Instance.slot5.Count > 2) {vanillaWeaponCycles[4].weaponEnums[2] = convertWeaponToWeaponEnum(MonoSingleton<GunControl>.Instance.slot5[2]);}
        }

        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                vanillaWeaponFields[i,j].value = vanillaWeaponCycles[i].weaponEnums[j];
            }
        }
    }

    public static EnumField<WeaponEnum>[,] allWeaponFields = new EnumField<WeaponEnum>[100, 100]; //these are 1-indexed, 0 is empty.
    public static BoolField[,] weaponSkipCycleFields = new BoolField[100, 100]; //1 indexed
    public static EnumField<SwapBehaviorEnum>[] swapBehaviorFields = new EnumField<SwapBehaviorEnum>[100];
    public static ConfigPanel[] configPanels = new ConfigPanel[100]; //1 indexed
    public static ConfigPanel[] advancedWeaponOptionsFields = new ConfigPanel[100]; //1 indexed

    //this isnt super efficent 
    public static void UpdateSettings() //this is required because of jank with OnValueChange
    {
        updateVariantBinds();
        updateRememberVariation();
        updateSkipCycles();
        UpdateUseKeySettings();
        updateSwapBehavior();
        //adds to weaponCycle if needed
        for(int i = 1; i < 100; i++) 
        {
            configPanels[i].interactable = true;
            if(allWeaponFields[i, 1] == null || allWeaponFields[i, 1].value == WeaponEnum.None)
            {
                break;
            }
            bool WeaponEnumNoneFound = false;
            //weaponCycles[i - 1].weaponEnums[0] = WeaponEnum.None; //this
            for(int j = 1; j < 100; j++)
            {
                if(allWeaponFields[i, j] != null)
                {
                    weaponCycles[i - 1].weaponEnums[j - 1] = allWeaponFields[i,j].value;
                    allWeaponFields[i, j].interactable = true;
                    weaponSkipCycleFields[i, j].interactable = true;
                    if(allWeaponFields[i, j].value == WeaponEnum.None){WeaponEnumNoneFound = true;}
                }
                else 
                {
                    WeaponCycle weaponCycle = weaponCycles[i - 1];
                    //prevent accessing array with j = 100
                    if(j < 99 && WeaponEnumNoneFound == false && allWeaponFields[i, j] == null && allWeaponFields[i, j + 1] == null) {CreateWeaponField(configPanels[i], weaponCycle, i, j); goto end; }
                    break;
                }
            }
        }
        end:;
        //uninteractables extraneous items from any weaponCycle
        for(int i = 1; i < 100; i++)
        {
            if(allWeaponFields[i, 1] == null)
            {
                break;
            }
            int numEnumNone = 0;
            for(int j = 1; j < 100; j++)
            {
                if(allWeaponFields[i, j] != null)
                {
                    bool weaponAfterNone = false;
                    
                    if(allWeaponFields[i, j].value == WeaponEnum.None)
                    {
                        numEnumNone += 1;
                    }
                    else if(numEnumNone > 0) {weaponAfterNone = true; numEnumNone += 1;}
                    if(weaponAfterNone)
                    {
                        allWeaponFields[i, j].value = WeaponEnum.None; //done to prevent players from creating a long list of enums then setting one to none, which would mess up the cycle next reboot
                    }
                    if(numEnumNone > 1)
                    {
                        weaponSkipCycleFields[i, j].interactable = false;
                        allWeaponFields[i, j].interactable = false;
                    }
                }
                else {break;}
            }
        }
        //adds a weapon cycle if needed
        bool emptyWeaponCycleExists = true;
        for (int j = 1; j < 100; j++)
        {
            if(allWeaponFields[numWeaponCyclesActive, j] != null)
            {
                if(allWeaponFields[numWeaponCyclesActive, j].value != WeaponEnum.None)
                {
                    emptyWeaponCycleExists = false;
                    break;
                }
            }
        }
        if(emptyWeaponCycleExists == false)
        {
            CreateWeaponCyclePanel(numWeaponCyclesActive + 1);
        }
        //uninteractables extraneous weapon cycles if needed
        int numEmptyWeaponCycles = 0;
        for(int i = 1; i < 100; i++)
        {
            if(allWeaponFields[i, 1] == null)
            {
                break;
            }
            bool emptyWeaponCycle = false;
            for(int j = 1; j < 100; j++)
            {
                if(allWeaponFields[i, j] != null)
                {
                    emptyWeaponCycle = true;
                    if(allWeaponFields[i, j].value != WeaponEnum.None)
                    {
                        emptyWeaponCycle = false;
                        break;
                    }
                }
            }
            if(emptyWeaponCycle) {numEmptyWeaponCycles += 1;}
            if(numEmptyWeaponCycles > 0)
            {
                if(i < numWeaponCyclesActive) {HideWeaponCyclePanel(i + 1);}
            }
        }
    }

    public static void StartSettingsUpdate()
    {
        tickUpdateSettings = Plugin.tickCount; //sets countdown to updatesettings
    }

    public static int tickUpdateSettings = 0;
    public static int numWeaponCyclesActive = 0;
    public static EnumField<KeyEnum>[] useKeyFields = new EnumField<KeyEnum>[100];
    public static void UpdateUseKeySettings()
    {
        for(int i = 1; i < 100; i++)
        {
            if(useKeyFields[i] != null)
            {
                WeaponCycle wc = weaponCycles[i - 1];
                wc.useCode = convertKeyEnumToKeyCode(useKeyFields[i].value);
            }
            else {break;}
        }
    }
    public static ConfigPanel CreateWeaponCyclePanel(int i)
    {
        weaponCycles[i - 1] = new WeaponCycle();
        WeaponCycle weaponCycle = weaponCycles[i - 1];
        weaponCycle.ignoreInCycle = new bool[100];
        numWeaponCyclesActive += 1;
        for(int j = 0; j < weaponCycle.weaponEnums.Length; j++)
        {
            weaponCycle.weaponEnums[j] = WeaponEnum.None;
        }

        ConfigPanel newWeaponCyclePanel = new ConfigPanel(division, "Weapon Cycle " + i, "customWeaponCyclePanel" + i);
        configPanels[i] = newWeaponCyclePanel;
        if(i % 2 == 1) {newWeaponCyclePanel.fieldColor = new Color(0.06f, 0.06f, 0.06f);}

        ConfigHeader inputHeader = new ConfigHeader(newWeaponCyclePanel, "Only the first weapon with this bind in the panel will be switched to. Default weapon binds will also be used before this.");
        inputHeader.textSize = 12;
        EnumField<KeyEnum> useKeyField = new EnumField<KeyEnum>(newWeaponCyclePanel, "Weapon Cycle Key", "customWeaponCycleKey" + i, KeyEnum.None);
        useKeyField.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {StartSettingsUpdate();};
        weaponCycle.useCode = convertKeyEnumToKeyCode(useKeyField.value);
        SetDisplayNames(useKeyField);
        useKeyFields[i] = useKeyField;

        BoolField rememberCycleVariationField = new BoolField(newWeaponCyclePanel, "Remember Cycle Variation", "rememberCycleVariation" + i, false);
        rememberCycleVariationField.onValueChange += (BoolField.BoolValueChangeEvent e) => {StartSettingsUpdate();};
        weaponCycle.rememberVariation = rememberCycleVariationField.value;
        rememberVariationFields[i] = rememberCycleVariationField;

        EnumField<SwapBehaviorEnum> swapBehaviorField = new EnumField<SwapBehaviorEnum>(newWeaponCyclePanel, "On Swap to Already Drawn Weapon", "swapBehavior" + i, SwapBehaviorEnum.NextVariation);
        swapBehaviorField.onValueChange += (EnumField<SwapBehaviorEnum>.EnumValueChangeEvent e) => {StartSettingsUpdate();};
        weaponCycle.swapBehavior = swapBehaviorField.value;
        swapBehaviorField.SetEnumDisplayName(SwapBehaviorEnum.FirstVariation, "First Variation");
        swapBehaviorField.SetEnumDisplayName(SwapBehaviorEnum.NextVariation, "Next Variation");
        swapBehaviorField.SetEnumDisplayName(SwapBehaviorEnum.SameVariation, "Same Variation");
        swapBehaviorFields[i] = swapBehaviorField;

        ConfigPanel advancedWeaponOptionsField = new ConfigPanel(newWeaponCyclePanel, "Advanced Options", "advancedVanillaWeaponOptionsField" + i);
        ConfigHeader infoHeader = new ConfigHeader(advancedWeaponOptionsField, "Ignored weapons in cycle can only be switched to using specific variant binds.");
        infoHeader.textSize = 12;

        advancedWeaponOptionsFields[i] = advancedWeaponOptionsField;

        ConfigHeader customWeaponCycleWeaponsHeader = new ConfigHeader(newWeaponCyclePanel, "Weapons");

        for(int j = 1; j < 100; j++)
        {
            if(CreateWeaponField(newWeaponCyclePanel, weaponCycle, i, j).value == WeaponEnum.None) {break;}
        }
        return newWeaponCyclePanel;
    }
    public static void HideWeaponCyclePanel(int i)
    {
        configPanels[i].interactable = false;
        for(int j = 1; j < 100; j++)
        {
            if(allWeaponFields[i, j] != null)
            {
                allWeaponFields[i, j].value = WeaponEnum.None;
            }
            else {return;}
        }
    }
    public static EnumField<WeaponEnum> CreateWeaponField(ConfigPanel newWeaponCyclePanel, WeaponCycle weaponCycle, int i, int j)
    {
        EnumField<WeaponEnum> weaponField = new EnumField<WeaponEnum>(newWeaponCyclePanel, "Weapon " + j, "customWeaponCycle" + i + "weapon" + j, WeaponEnum.None);
        allWeaponFields[i,j] = weaponField;
        weaponField.onValueChange += (EnumField<WeaponEnum>.EnumValueChangeEvent e) => {StartSettingsUpdate();};

        weaponCycle.weaponEnums[j - 1] = weaponField.value;
        setWeaponEnumDisplayNames(weaponField);
        if(j % 2 == 0) {weaponField.fieldColor = new Color(0.06f,0.06f,0.06f);}

        ConfigPanel advancedWeaponOptionsField = advancedWeaponOptionsFields[i];
        BoolField ignoreWeaponInCycleField = new BoolField(advancedWeaponOptionsField, "Ignore Weapon " + j + " in cycle", "ignoreWeaponInCycle" + i + "_" +  j, false);
        ignoreWeaponInCycleField.onValueChange += (BoolField.BoolValueChangeEvent e) => {StartSettingsUpdate();};
        weaponCycles[i - 1].ignoreInCycle[j] = ignoreWeaponInCycleField.value;
        weaponSkipCycleFields[i, j] = ignoreWeaponInCycleField;

        return weaponField;
    }
    public static void HideWeaponField(int i, int j)
    {
        allWeaponFields[i, j].interactable = false;
        weaponSkipCycleFields[i, j].interactable = false;
    }

    public static ConfigDivision division = null;
    public static EnumField<WeaponEnum>[,] vanillaWeaponFields = new EnumField<WeaponEnum>[5, 3];
    public static BoolField[,] vanillaWeaponSkipCycleFields = new BoolField[5, 3];
    public static EnumField<KeyEnum>[] weaponVariantBindFields = new EnumField<KeyEnum>[100];
    public static EnumField<SwapBehaviorEnum>[] vanillaSwapBehaviorFields = new EnumField<SwapBehaviorEnum>[5];
    public static BoolField[] rememberVariationFields = new BoolField[100];
    public static BoolField[] rememberVanillaVariationFields = new BoolField[5];
    public static void updateVariantBinds()
    {
        for(int i = 3; i < 100; i++)
        {
            variationBinds[i] = convertKeyEnumToKeyCode(weaponVariantBindFields[i].value);
        }
    }
    public static void updateSkipCycles()
    {
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                vanillaWeaponCycles[i].ignoreInCycle[j] = vanillaWeaponSkipCycleFields[i,j].value;
            }
        }
        for(int i = 1; i < 100; i++)
        {
            if(weaponSkipCycleFields[i,1] == null) {break;}
            for(int j = 1; j < 100; j++)
            {
                if(weaponSkipCycleFields[i,j] != null)
                {
                    weaponCycles[i - 1].ignoreInCycle[j] = weaponSkipCycleFields[i,j].value;
                }
                else {break;}
            }
        }
    }
    public static void updateSwapBehavior()
    {
        for(int i = 0; i < 5; i++)
        {
            vanillaWeaponCycles[i].swapBehavior = vanillaSwapBehaviorFields[i].value;
        }
        for(int i = 1; i < 100; i++)
        {
            if(swapBehaviorFields[i] != null)
            {
                weaponCycles[i - 1].swapBehavior = swapBehaviorFields[i].value;
            }
            else {break;}
        }
    }
    public static void updateRememberVariation()
    {
        for(int i = 0; i < 5; i++)
        {
            vanillaWeaponCycles[i].rememberVariation = rememberVanillaVariationFields[i].value;
        }
        for(int i = 1; i < 100; i++)
        {
            if(rememberVariationFields[i] != null)
            {
                weaponCycles[i - 1].rememberVariation = rememberVariationFields[i].value;
            }
            else {break;}
        }
    }
    public static void WeaponVariantBindsConfig()
    {
        var config = PluginConfigurator.Create("WeaponVariantBinds", "WeaponVariantBinds");
        config.SetIconWithURL($"{Path.Combine(DefaultParentFolder!, "icon.png")}");

        ConfigHeader warningHeader = new ConfigHeader(config.rootPanel, "This mod overrides vanilla weapon switching, incompatibilies with other mods may arise.");
        warningHeader.textSize = 14;
        warningHeader.textColor = Color.red;

        BoolField enabledField = new BoolField(config.rootPanel, "Mod Enabled", "modEnabled", true);

        ConfigPanel advancedOptionsField = new ConfigPanel(config.rootPanel, "Advanced Options", "advancedOptionsPanel");
        ConfigHeader infoHeaderVariantBinds = new ConfigHeader(advancedOptionsField, "Variant Binds 1-3 are in Ultrakill's main controls settings.");
        infoHeaderVariantBinds.textSize = 12;
        for(int i = 3; i < 100; i++)
        {
            EnumField<KeyEnum> variantBindField = new EnumField<KeyEnum>(advancedOptionsField, "Weapon Variant Bind " + (i + 1), "variantBind" + i, KeyEnum.None);
            variantBindField.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {StartSettingsUpdate();};
            variationBinds[i] = convertKeyEnumToKeyCode(variantBindField.value);
            weaponVariantBindFields[i] = variantBindField;
            SetDisplayNames(variantBindField);
        }

        division = new ConfigDivision(config.rootPanel, "division");
        enabledField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.modEnabled = e.value; division.interactable = e.value;};
        Plugin.modEnabled = enabledField.value; division.interactable = enabledField.value; 

        //+-----------------------+\\
        //| VANILLA WEAPON CYCLES |\\
        //+-----------------------+\\
        //vanilla weapon cycles are special custom weapon cycles that follow the vanilla weapon ordering

        //not tested
        String[] names = {"Revolver", "Shotgun", "Nailgun", "Railcannon", "Rocket Launcher"};
        Color[] colors = {new Color(0.0f, 0.0f, 0.2f), new Color(0.2f, 0.0f, 0.0f), new Color(0.0f, 0.2f, 0.0f), new Color(0.10f, 0.00f, 0.10f), new Color(0.10f, 0.10f, 0.00f)};
        for(int i = 0; i < 5; i++)
        {
            String name = names[i];
            WeaponCycle weaponCycle = vanillaWeaponCycles[i];

            ConfigPanel newWeaponCyclePanel = new ConfigPanel(division, name + " Weapon Cycle", "customWeaponCyclePanel" + name);
            newWeaponCyclePanel.fieldColor = colors[i];

            ConfigHeader inputHeader = new ConfigHeader(newWeaponCyclePanel, "The use key for this weapon is whatever it is set to in Ultrakill main settings.");
            inputHeader.textSize = 12;

            BoolField rememberCycleVariationField = new BoolField(newWeaponCyclePanel, "Remember Cycle Variation", "rememberCycleVariation" + name, false);
            rememberCycleVariationField.onValueChange += (BoolField.BoolValueChangeEvent e) => {StartSettingsUpdate();};
            weaponCycle.rememberVariation = rememberCycleVariationField.value;
            rememberVanillaVariationFields[i] = rememberCycleVariationField;

            EnumField<SwapBehaviorEnum> swapBehaviorField = new EnumField<SwapBehaviorEnum>(newWeaponCyclePanel, "On Swap to Already Drawn Weapon", "swapBehavior" + name, SwapBehaviorEnum.NextVariation);
            swapBehaviorField.onValueChange += (EnumField<SwapBehaviorEnum>.EnumValueChangeEvent e) => {StartSettingsUpdate();};
            weaponCycle.swapBehavior = swapBehaviorField.value;
            swapBehaviorField.SetEnumDisplayName(SwapBehaviorEnum.FirstVariation, "First Variation");
            swapBehaviorField.SetEnumDisplayName(SwapBehaviorEnum.NextVariation, "Next Variation");
            swapBehaviorField.SetEnumDisplayName(SwapBehaviorEnum.SameVariation, "Same Variation");
            vanillaSwapBehaviorFields[i] = swapBehaviorField;

            ConfigPanel advancedWeaponOptionsField = new ConfigPanel(newWeaponCyclePanel, "Advanced Options", "advancedWeaponOptionsField" + i);
            ConfigHeader infoHeader = new ConfigHeader(advancedWeaponOptionsField, "Ignored weapons in cycle can only be switched to using specific variant binds.");
            infoHeader.textSize = 12;

            ConfigHeader customWeaponCycleWeaponsHeader = new ConfigHeader(newWeaponCyclePanel, "Weapons");

            for(int j = 0; j < 3; j++)
            {
                EnumField<WeaponEnum> weaponField = new EnumField<WeaponEnum>(newWeaponCyclePanel, "Weapon " + (j + 1), "vanillaWeaponCycle" + (i + 1) + "weapon" + (j + 1), vanillaWeaponCycles[i].weaponEnums[j]);
                weaponField.interactable = false;

                setWeaponEnumDisplayNames(weaponField);
                if(j % 2 == 1) {weaponField.fieldColor = new Color(0.06f,0.06f,0.06f);}
                vanillaWeaponFields[i,j] = weaponField;

                BoolField ignoreWeaponInCycleField = new BoolField(advancedWeaponOptionsField, "Ignore Weapon " + (j + 1) + " in cycle", "ignoreVanillaWeaponInCycle" + (i + 1) + "_" +  (j + 1), false);
                ignoreWeaponInCycleField.onValueChange += (BoolField.BoolValueChangeEvent e) => {StartSettingsUpdate();};
                vanillaWeaponCycles[i].ignoreInCycle[j] = ignoreWeaponInCycleField.value;
                vanillaWeaponSkipCycleFields[i, j] = ignoreWeaponInCycleField;
            }
        }

        //+----------------------+\\
        //| CUSTOM WEAPON CYCLES |\\
        //+----------------------+\\

        ConfigHeader customWeaponCycleHeader = new ConfigHeader(division, "Specific weapon variant binds can be made down here with a weapon cycle. Consult the readme on the mod page if you need help.");
        customWeaponCycleHeader.textSize = 14;

        for(int i = 1; i <= weaponCycles.Length; i++)
        {
            CreateWeaponCyclePanel(i);
            if(allWeaponFields[i, 1] == null || allWeaponFields[i, 1].value == WeaponEnum.None)
            {
                break;
            }
        }
    }
}