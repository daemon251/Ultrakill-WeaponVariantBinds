using System.IO;
using System.Reflection;
using PluginConfig.API;
using PluginConfig.API.Decorators;
using PluginConfig.API.Fields;
using PluginConfig.API.Functionals;
using UnityEngine;

namespace WeaponVariantBinds;

public class PluginConfig
{

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
    public static void WeaponVariantBindsConfig()
    {
        var config = PluginConfigurator.Create("WeaponVariantBinds", "WeaponVariantBinds");
        config.SetIconWithURL($"{Path.Combine(DefaultParentFolder!, "icon.png")}");

        //ConfigHeader warningHeader = new ConfigHeader(config.rootPanel, "");
        //warningHeader.textSize = 20;
        //warningHeader.textColor = Color.red;

        BoolField enabledField = new BoolField(config.rootPanel, "Mod Enabled", "modEnabled", true);

        ConfigDivision division = new ConfigDivision(config.rootPanel, "division");
        enabledField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.modEnabled = e.value; division.interactable = e.value;};
        Plugin.modEnabled = enabledField.value; division.interactable = enabledField.value; 

        BoolField SwapVariationIgnoreModField = new BoolField(division, "Swap variation ignores autoswitch?", "swapVariationIgnoreMod", true);
        SwapVariationIgnoreModField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.SwapVariationIgnoreMod = e.value;};
        Plugin.SwapVariationIgnoreMod = SwapVariationIgnoreModField.value; 

        BoolField ScrollVariationIgnoreModField = new BoolField(division, "Scroll variation ignores autoswitch?", "scrollVariationIgnoreMod", true);
        ScrollVariationIgnoreModField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.ScrollVariationIgnoreMod = e.value;};
        Plugin.ScrollVariationIgnoreMod = ScrollVariationIgnoreModField.value; 

        //+----------+\\
        //| REVOLVER |\\
        //+----------+\\

        ConfigPanel revolverPanel = new ConfigPanel(division, "Revolver", "revolverPanel");

        ConfigHeader revolverHeader = new ConfigHeader(revolverPanel, "These settings are based off of the weapon order that you have set in the weapon terminal for this weapon.");
        revolverHeader.textSize = 16;
        revolverHeader.textColor = Color.white;

        EnumField<KeyEnum> revolverVariant1Field = new EnumField<KeyEnum>(revolverPanel, "Revolver Variant 1 Bind", "revolverVariant1Bind", KeyEnum.None);
        revolverVariant1Field.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {Plugin.weaponKeyCodes[0,0] = convertKeyEnumToKeyCode(e.value);};
        Plugin.weaponKeyCodes[0,0] = convertKeyEnumToKeyCode(revolverVariant1Field.value);

        EnumField<KeyEnum> revolverVariant2Field = new EnumField<KeyEnum>(revolverPanel, "Revolver Variant 2 Bind", "revolverVariant2Bind", KeyEnum.None);
        revolverVariant2Field.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {Plugin.weaponKeyCodes[0,1] = convertKeyEnumToKeyCode(e.value);};
        Plugin.weaponKeyCodes[0,1] = convertKeyEnumToKeyCode(revolverVariant2Field.value);

        EnumField<KeyEnum> revolverVariant3Field = new EnumField<KeyEnum>(revolverPanel, "Revolver Variant 3 Bind", "revolverVariant3Bind", KeyEnum.None);
        revolverVariant3Field.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {Plugin.weaponKeyCodes[0,2] = convertKeyEnumToKeyCode(e.value);};
        Plugin.weaponKeyCodes[0,2] = convertKeyEnumToKeyCode(revolverVariant3Field.value);

        BoolField revolver1CycleIgnoreField = new BoolField(revolverPanel, "Autoswitch Variant 1 in Cycle", "ignoreRevolver1InCycle", false);
        revolver1CycleIgnoreField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.ignoreWeaponInCycle[0,0] = e.value;};
        Plugin.ignoreWeaponInCycle[0,0] = revolver1CycleIgnoreField.value;

        BoolField revolver2CycleIgnoreField = new BoolField(revolverPanel, "Autoswitch Variant 2 in Cycle", "ignoreRevolver2InCycle", false);
        revolver2CycleIgnoreField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.ignoreWeaponInCycle[0,1] = e.value;};
        Plugin.ignoreWeaponInCycle[0,1] = revolver2CycleIgnoreField.value;

        BoolField revolver3CycleIgnoreField = new BoolField(revolverPanel, "Autoswitch Variant 3 in Cycle", "ignoreRevolver3InCycle", false);
        revolver3CycleIgnoreField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.ignoreWeaponInCycle[0,2] = e.value;};
        Plugin.ignoreWeaponInCycle[0,2] = revolver3CycleIgnoreField.value;

        SetDisplayNames(revolverVariant1Field);
        SetDisplayNames(revolverVariant2Field);
        SetDisplayNames(revolverVariant3Field);

        //+---------+\\
        //| SHOTGUN |\\
        //+---------+\\

        ConfigPanel shotgunPanel = new ConfigPanel(division, "Shotgun", "shotgunPanel");

        ConfigHeader shotgunHeader = new ConfigHeader(shotgunPanel, "These settings are based off of the weapon order that you have set in the weapon terminal for this weapon.");
        shotgunHeader.textSize = 16;
        shotgunHeader.textColor = Color.white;

        EnumField<KeyEnum> shotgunVariant1Field = new EnumField<KeyEnum>(shotgunPanel, "Shotgun Variant 1 Bind", "shotgunVariant1Bind", KeyEnum.None);
        shotgunVariant1Field.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {Plugin.weaponKeyCodes[1,0] = convertKeyEnumToKeyCode(e.value);};
        Plugin.weaponKeyCodes[1,0] = convertKeyEnumToKeyCode(shotgunVariant1Field.value);

        EnumField<KeyEnum> shotgunVariant2Field = new EnumField<KeyEnum>(shotgunPanel, "Shotgun Variant 2 Bind", "shotgunVariant2Bind", KeyEnum.None);
        shotgunVariant2Field.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {Plugin.weaponKeyCodes[1,1] = convertKeyEnumToKeyCode(e.value);};
        Plugin.weaponKeyCodes[1,1] = convertKeyEnumToKeyCode(shotgunVariant2Field.value);

        EnumField<KeyEnum> shotgunVariant3Field = new EnumField<KeyEnum>(shotgunPanel, "Shotgun Variant 3 Bind", "shotgunVariant3Bind", KeyEnum.None);
        shotgunVariant3Field.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {Plugin.weaponKeyCodes[1,2] = convertKeyEnumToKeyCode(e.value);};
        Plugin.weaponKeyCodes[1,2] = convertKeyEnumToKeyCode(shotgunVariant3Field.value);

        BoolField shotgun1CycleIgnoreField = new BoolField(shotgunPanel, "Autoswitch Variant 1 in Cycle", "ignoreShotgun1InCycle", false);
        shotgun1CycleIgnoreField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.ignoreWeaponInCycle[1,0] = e.value;};
        Plugin.ignoreWeaponInCycle[1,0] = shotgun1CycleIgnoreField.value;

        BoolField shotgun2CycleIgnoreField = new BoolField(shotgunPanel, "Autoswitch Variant 2 in Cycle", "ignoreShotgun2InCycle", false);
        shotgun2CycleIgnoreField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.ignoreWeaponInCycle[1,1] = e.value;};
        Plugin.ignoreWeaponInCycle[1,1] = shotgun2CycleIgnoreField.value;

        BoolField shotgun3CycleIgnoreField = new BoolField(shotgunPanel, "Autoswitch Variant 3 in Cycle", "ignoreShotgun3InCycle", false);
        shotgun3CycleIgnoreField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.ignoreWeaponInCycle[1,2] = e.value;};
        Plugin.ignoreWeaponInCycle[1,2] = shotgun3CycleIgnoreField.value;

        SetDisplayNames(shotgunVariant1Field);
        SetDisplayNames(shotgunVariant2Field);
        SetDisplayNames(shotgunVariant3Field);

        //+---------+\\
        //| NAILGUN |\\
        //+---------+\\

        ConfigPanel nailgunPanel = new ConfigPanel(division, "Nailgun", "nailgunPanel");

        ConfigHeader nailgunHeader = new ConfigHeader(nailgunPanel, "These settings are based off of the weapon order that you have set in the weapon terminal for this weapon.");
        nailgunHeader.textSize = 16;
        nailgunHeader.textColor = Color.white;

        EnumField<KeyEnum> nailgunVariant1Field = new EnumField<KeyEnum>(nailgunPanel, "Nailgun Variant 1 Bind", "nailgunVariant1Bind", KeyEnum.None);
        nailgunVariant1Field.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {Plugin.weaponKeyCodes[2,0] = convertKeyEnumToKeyCode(e.value);};
        Plugin.weaponKeyCodes[2,0] = convertKeyEnumToKeyCode(nailgunVariant1Field.value);

        EnumField<KeyEnum> nailgunVariant2Field = new EnumField<KeyEnum>(nailgunPanel, "Nailgun Variant 2 Bind", "nailgunVariant2Bind", KeyEnum.None);
        nailgunVariant2Field.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {Plugin.weaponKeyCodes[2,1] = convertKeyEnumToKeyCode(e.value);};
        Plugin.weaponKeyCodes[2,1] = convertKeyEnumToKeyCode(nailgunVariant2Field.value);

        EnumField<KeyEnum> nailgunVariant3Field = new EnumField<KeyEnum>(nailgunPanel, "Nailgun Variant 3 Bind", "nailgunVariant3Bind", KeyEnum.None);
        nailgunVariant3Field.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {Plugin.weaponKeyCodes[2,2] = convertKeyEnumToKeyCode(e.value);};
        Plugin.weaponKeyCodes[2,2] = convertKeyEnumToKeyCode(nailgunVariant3Field.value);

        BoolField nailgun1CycleIgnoreField = new BoolField(nailgunPanel, "Autoswitch Variant 1 in Cycle", "ignoreNailgun1InCycle", false);
        nailgun1CycleIgnoreField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.ignoreWeaponInCycle[2,0] = e.value;};
        Plugin.ignoreWeaponInCycle[2,0] = nailgun1CycleIgnoreField.value;

        BoolField nailgun2CycleIgnoreField = new BoolField(nailgunPanel, "Autoswitch Variant 2 in Cycle", "ignoreNailgun2InCycle", false);
        nailgun2CycleIgnoreField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.ignoreWeaponInCycle[2,1] = e.value;};
        Plugin.ignoreWeaponInCycle[2,1] = nailgun2CycleIgnoreField.value;

        BoolField nailgun3CycleIgnoreField = new BoolField(nailgunPanel, "Autoswitch Variant 3 in Cycle", "ignoreNailgun3InCycle", false);
        nailgun3CycleIgnoreField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.ignoreWeaponInCycle[2,2] = e.value;};
        Plugin.ignoreWeaponInCycle[2,2] = nailgun3CycleIgnoreField.value;

        SetDisplayNames(nailgunVariant1Field);
        SetDisplayNames(nailgunVariant2Field);
        SetDisplayNames(nailgunVariant3Field);

        //+------------+\\
        //| RAILCANNON |\\
        //+------------+\\

        ConfigPanel railcannonPanel = new ConfigPanel(division, "Railcannon", "railcannonPanel");

        ConfigHeader railcannonHeader = new ConfigHeader(railcannonPanel, "These settings are based off of the weapon order that you have set in the weapon terminal for this weapon.");
        railcannonHeader.textSize = 16;
        railcannonHeader.textColor = Color.white;

        EnumField<KeyEnum> railcannonVariant1Field = new EnumField<KeyEnum>(railcannonPanel, "Railcannon Variant 1 Bind", "railcannonVariant1Bind", KeyEnum.None);
        railcannonVariant1Field.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {Plugin.weaponKeyCodes[3,0] = convertKeyEnumToKeyCode(e.value);};
        Plugin.weaponKeyCodes[3,0] = convertKeyEnumToKeyCode(railcannonVariant1Field.value);

        EnumField<KeyEnum> railcannonVariant2Field = new EnumField<KeyEnum>(railcannonPanel, "Railcannon Variant 2 Bind", "railcannonVariant2Bind", KeyEnum.None);
        railcannonVariant2Field.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {Plugin.weaponKeyCodes[3,1] = convertKeyEnumToKeyCode(e.value);};
        Plugin.weaponKeyCodes[3,1] = convertKeyEnumToKeyCode(railcannonVariant2Field.value);

        EnumField<KeyEnum> railcannonVariant3Field = new EnumField<KeyEnum>(railcannonPanel, "Railcannon Variant 3 Bind", "railcannonVariant3Bind", KeyEnum.None);
        railcannonVariant3Field.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {Plugin.weaponKeyCodes[3,2] = convertKeyEnumToKeyCode(e.value);};
        Plugin.weaponKeyCodes[3,2] = convertKeyEnumToKeyCode(railcannonVariant3Field.value);

        BoolField railcannon1CycleIgnoreField = new BoolField(railcannonPanel, "Autoswitch Variant 1 in Cycle", "ignoreRailcannon1InCycle", false);
        railcannon1CycleIgnoreField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.ignoreWeaponInCycle[3,0] = e.value;};
        Plugin.ignoreWeaponInCycle[3,0] = railcannon1CycleIgnoreField.value;

        BoolField railcannon2CycleIgnoreField = new BoolField(railcannonPanel, "Autoswitch Variant 2 in Cycle", "ignoreRailcannon2InCycle", false);
        railcannon2CycleIgnoreField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.ignoreWeaponInCycle[3,1] = e.value;};
        Plugin.ignoreWeaponInCycle[3,1] = railcannon2CycleIgnoreField.value;

        BoolField railcannon3CycleIgnoreField = new BoolField(railcannonPanel, "Autoswitch Variant 3 in Cycle", "ignoreRailcannon3InCycle", false);
        railcannon3CycleIgnoreField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.ignoreWeaponInCycle[3,2] = e.value;};
        Plugin.ignoreWeaponInCycle[3,2] = railcannon3CycleIgnoreField.value;

        SetDisplayNames(railcannonVariant1Field);
        SetDisplayNames(railcannonVariant2Field);
        SetDisplayNames(railcannonVariant3Field);

        //+-----------------+\\
        //| ROCKET LAUNCHER |\\
        //+-----------------+\\

        ConfigPanel rocket_launcherPanel = new ConfigPanel(division, "Rocket Launcher", "rocket_launcherPanel");

        ConfigHeader rocket_launcherHeader = new ConfigHeader(rocket_launcherPanel, "These settings are based off of the weapon order that you have set in the weapon terminal for this weapon.");
        rocket_launcherHeader.textSize = 16;
        rocket_launcherHeader.textColor = Color.white;

        EnumField<KeyEnum> rocket_launcherVariant1Field = new EnumField<KeyEnum>(rocket_launcherPanel, "Rocket Launcher Variant 1 Bind", "rocket_launcherVariant1Bind", KeyEnum.None);
        rocket_launcherVariant1Field.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {Plugin.weaponKeyCodes[4,0] = convertKeyEnumToKeyCode(e.value);};
        Plugin.weaponKeyCodes[4,0] = convertKeyEnumToKeyCode(rocket_launcherVariant1Field.value);

        EnumField<KeyEnum> rocket_launcherVariant2Field = new EnumField<KeyEnum>(rocket_launcherPanel, "Rocket Launcher Variant 2 Bind", "rocket_launcherVariant2Bind", KeyEnum.None);
        rocket_launcherVariant2Field.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {Plugin.weaponKeyCodes[4,1] = convertKeyEnumToKeyCode(e.value);};
        Plugin.weaponKeyCodes[4,1] = convertKeyEnumToKeyCode(rocket_launcherVariant2Field.value);

        EnumField<KeyEnum> rocket_launcherVariant3Field = new EnumField<KeyEnum>(rocket_launcherPanel, "Rocket Launcher Variant 3 Bind", "rocket_launcherVariant3Bind", KeyEnum.None);
        rocket_launcherVariant3Field.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {Plugin.weaponKeyCodes[4,2] = convertKeyEnumToKeyCode(e.value);};
        Plugin.weaponKeyCodes[4,2] = convertKeyEnumToKeyCode(rocket_launcherVariant3Field.value);

        BoolField rocket_launcher1CycleIgnoreField = new BoolField(rocket_launcherPanel, "Autoswitch Variant 1 in Cycle", "ignoreRocket_launcher1InCycle", false);
        rocket_launcher1CycleIgnoreField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.ignoreWeaponInCycle[4,0] = e.value;};
        Plugin.ignoreWeaponInCycle[4,0] = rocket_launcher1CycleIgnoreField.value;

        BoolField rocket_launcher2CycleIgnoreField = new BoolField(rocket_launcherPanel, "Autoswitch Variant 2 in Cycle", "ignoreRocket_launcher2InCycle", false);
        rocket_launcher2CycleIgnoreField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.ignoreWeaponInCycle[4,1] = e.value;};
        Plugin.ignoreWeaponInCycle[4,1] = rocket_launcher2CycleIgnoreField.value;

        BoolField rocket_launcher3CycleIgnoreField = new BoolField(rocket_launcherPanel, "Autoswitch Variant 3 in Cycle", "ignoreRocket_launcher3InCycle", false);
        rocket_launcher3CycleIgnoreField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.ignoreWeaponInCycle[4,2] = e.value;};
        Plugin.ignoreWeaponInCycle[4,2] = rocket_launcher3CycleIgnoreField.value;

        SetDisplayNames(rocket_launcherVariant1Field);
        SetDisplayNames(rocket_launcherVariant2Field);
        SetDisplayNames(rocket_launcherVariant3Field);
    }
}