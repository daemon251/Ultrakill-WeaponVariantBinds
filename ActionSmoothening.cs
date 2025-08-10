using BepInEx;
using UnityEngine;
using HarmonyLib;
using System;
using BepInEx.Logging;
using WeaponVariantBinds;
using Mono.Collections.Generic;
using UnityEngine.PlayerLoop;
using System.Collections.Generic;
using UnityEngine.InputSystem.Interactions;

namespace WeaponVariantBinds;

public class ActionSmoothening
{   
    public static bool actionSmootheningEnabled = false;

    public static GameObject weaponToSwitchTo;
    public static WeaponCycle wcOld; //when preventing wc cycling, we need this later
    public static WeaponCycle wcToSwitchTo;
    public static float switchTime = -1f; //-1f when disabled
    public static float timeUntilCurrentWeaponDeploy = 0f;

    public static bool lastFireStatePrimary = false;
    public static bool lastFireStateSecondary = false;
    public static bool AttemptRevolverShoot(GameObject weapon, ActionSmootheningConfig config1) //works
    {
        bool fired = false;
        Revolver revolver = weapon.GetComponent<Revolver>();
        //from decompiler
        if (revolver.gunReady)
        {
            if ((revolver.inman.InputSource.Fire2.WasCanceledThisFrame || !revolver.inman.PerformingCheatMenuCombo() && !GameStateManager.Instance.PlayerInputLocked && revolver.inman.InputSource.Fire1.IsPressed) && revolver.shootReady && (revolver.gunVariation == 0 ? ((double) revolver.pierceShotCharge == 100.0 ? 1 : 0) : ((double) revolver.pierceShotCharge >= 25.0 ? 1 : 0)) != 0)
            {
                //dont need
            }
            else if (!revolver.inman.PerformingCheatMenuCombo() && revolver.inman.InputSource.Fire1.IsPressed && revolver.shootReady && !revolver.chargingPierce)
            {
                if (!(bool) (UnityEngine.Object) revolver.wid || (double) revolver.wid.delay == 0.0)
                {
                    revolver.Shoot();
                }
                else
                {
                    revolver.shootReady = false;
                    revolver.shootCharge = 0.0f;
                    revolver.Invoke("DelayedShoot", revolver.wid.delay);
                }
            }
        }
        if(revolver.shootReady == false) {fired = true;}
        return fired;
    }
    public static bool AttemptShotgunShoot(GameObject weapon, ActionSmootheningConfig config1) //works
    {
        bool fired = false;
        if     (weapon.TryGetComponent<Shotgun>(out Shotgun shotgun)) 
        {
            //from decompiler
            if (!MonoSingleton<InputManager>.Instance.PerformingCheatMenuCombo() && MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed && shotgun.gunReady && shotgun.gc.activated && !GameStateManager.Instance.PlayerInputLocked && !shotgun.charging)
            {
                if (!(bool) (UnityEngine.Object) shotgun.wid || (double) shotgun.wid.delay == 0.0)
                {
                    shotgun.Shoot();
                }
                else
                {
                    shotgun.gunReady = false;
                    shotgun.Invoke("Shoot", shotgun.wid.delay);
                }
            }
            if(shotgun.gunReady == false && shotgun.meterOverride == false) {fired = true;} //shotgun.meterOverride also false, make sure not recovering from core eject
        }
        else if(weapon.TryGetComponent<ShotgunHammer>(out ShotgunHammer shotgunhammer)) 
        {
            MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed = false; //in this case, we want to shoot it by letting go of primary fire if user already had it held
            //from decompiler
            if (!MonoSingleton<InputManager>.Instance.PerformingCheatMenuCombo() && !MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed && (double) shotgunhammer.swingCharge == 1.0 && shotgunhammer.gunReady && shotgunhammer.gc.activated && !GameStateManager.Instance.PlayerInputLocked)
            {
                shotgunhammer.chargingSwing = false;
                shotgunhammer.swingCharge = 0.0f;
                if (!(bool) (UnityEngine.Object) shotgunhammer.wid || (double) shotgunhammer.wid.delay == 0.0)
                {
                    shotgunhammer.Impact();
                }
                else
                {
                    shotgunhammer.gunReady = false;
                    shotgunhammer.Invoke("Impact", shotgunhammer.wid.delay);
                }
            }
            if(shotgunhammer.gunReady == false) {fired = true;}
        }
        return fired;
    }
    public static bool AttemptNailgunShoot(GameObject weapon, ActionSmootheningConfig config1) //works
    {
        bool fired = false;
        Nailgun nailgun = weapon.GetComponent<Nailgun>();
        //from decompiler
        if (nailgun.fireCooldown <= 0f)
        {
            nailgun.Shoot();
        }
        else
        {
            nailgun.fireCooldown = nailgun.currentFireRate;
            nailgun.Invoke("Shoot", nailgun.wid.delay / 10f);
        }
        if(nailgun.fireCooldown != 0) {fired = true;} //good enough?
        return fired;
    }
    public static bool AttemptRailcannonShoot(GameObject weapon, ActionSmootheningConfig config1) //works
    {
        bool fired = false;
        Railcannon railcannon = weapon.GetComponent<Railcannon>();
        //from decompiler
        if (!MonoSingleton<InputManager>.Instance.PerformingCheatMenuCombo() && MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed && railcannon.gc.activated && !GameStateManager.Instance.PlayerInputLocked)
        {
            if(MonoSingleton<WeaponCharges>.Instance.raicharge >= 4.0f)
            {
                if (!(bool) (UnityEngine.Object) railcannon.wid || (double) railcannon.wid.delay == 0.0)
                    railcannon.Shoot();
                else
                    railcannon.Invoke("Shoot", railcannon.wid.delay);
                //this nessecary?
                railcannon.fullCharge.SetActive(false);
                railcannon.transform.localPosition = railcannon.wpos.currentDefault;
                railcannon.wc.raicharge = 0.0f;
                railcannon.wc.railChargePlayed = false;
                railcannon.altCharge = 0.0f;
            }
        }
        if(MonoSingleton<WeaponCharges>.Instance.raicharge == 0f) {fired = true;} //might be broken?
        return fired;
    }
    public static bool AttemptRocketLauncherShoot(GameObject weapon, ActionSmootheningConfig config1) //works
    {
        bool fired = false;
        RocketLauncher rocketLauncher = weapon.GetComponent<RocketLauncher>();
        if (MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed && MonoSingleton<GunControl>.Instance.activated && !GameStateManager.Instance.PlayerInputLocked && rocketLauncher.cooldown == 0f)
        {
            if (!(bool) (UnityEngine.Object) rocketLauncher.wid || rocketLauncher.wid.delay == 0.0) {rocketLauncher.Shoot();}
            else {rocketLauncher.Invoke("Shoot", rocketLauncher.wid.delay);}
            fired = true;
            rocketLauncher.cooldown = 1f; 
        }
        return fired;
    }
    public static bool AttemptRevolverAlt1Shoot(GameObject weapon, ActionSmootheningConfig config1) //works
    {
        bool fired = false;
        Revolver revolver = weapon.GetComponent<Revolver>();
        if(revolver.pierceShotCharge >= 100f)
        {
            MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed = true;
            //from decompiler
            if ((revolver.inman.InputSource.Fire2.WasCanceledThisFrame || !revolver.inman.PerformingCheatMenuCombo() && !GameStateManager.Instance.PlayerInputLocked && revolver.inman.InputSource.Fire1.IsPressed) && revolver.shootReady && (revolver.gunVariation == 0 ? ((double) revolver.pierceShotCharge == 100.0 ? 1 : 0) : ((double) revolver.pierceShotCharge >= 25.0 ? 1 : 0)) != 0)
            {
                fired = true;
                if (!(bool) (UnityEngine.Object) revolver.wid || (double) revolver.wid.delay == 0.0)
                {
                    revolver.Shoot(2);
                }
                else
                {
                    revolver.shootReady = false;
                    revolver.shootCharge = 0.0f;
                    revolver.Invoke("DelayedShoot2", revolver.wid.delay);
                }
            }
        }
        return fired;
    }
    public static bool AttemptRevolverAlt2Shoot(GameObject weapon, ActionSmootheningConfig config1) //weird
    {
        bool fired = false;
        Revolver revolver = weapon.GetComponent<Revolver>();
        MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed = true;
        if (revolver.pierceReady && (double) revolver.coinCharge >= 100.0)
        {
            if(revolver.cc != null) //needed?
                revolver.cc.StopShake();
            if (!(bool) (UnityEngine.Object) revolver.wid || (double) revolver.wid.delay == 0.0)
                revolver.wc.rev1charge -= 100f;
            if (!(bool) (UnityEngine.Object) revolver.wid || (double) revolver.wid.delay == 0.0)
            {
                fired = true;
                revolver.ThrowCoin();
            }
            else
            {
                fired = true;
                revolver.Invoke("ThrowCoin", revolver.wid.delay);
                revolver.pierceReady = false;
                revolver.pierceCharge = 0.0f;
            }
        }
        return fired;
    }
    public static bool AttemptRevolverAlt3Shoot(GameObject weapon, ActionSmootheningConfig config1) //work
    {
        bool fired = false;
        Revolver revolver = weapon.GetComponent<Revolver>();
        //if(revolver.pierceCharge >= 100f) {MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed = true;}
        if ((true || !revolver.inman.PerformingCheatMenuCombo() && !GameStateManager.Instance.PlayerInputLocked) && revolver.shootReady && (revolver.gunVariation == 0 ? ((double) revolver.pierceShotCharge == 100.0 ? 1 : 0) : ((double) revolver.pierceShotCharge >= 25.0 ? 1 : 0)) != 0)
        {
            if(revolver.twirlLevel >= 4f)
            {
                fired = true;
                if (!(bool) (UnityEngine.Object) revolver.wid || (double) revolver.wid.delay == 0.0)
                {
                    revolver.Shoot(2);
                }
                else
                {
                    revolver.shootReady = false;
                    revolver.shootCharge = 0.0f;
                    revolver.Invoke("DelayedShoot2", revolver.wid.delay);
                }
            }
        }
        return fired;
    }
    public static bool AttemptShotgunAlt1Shoot(GameObject weapon, ActionSmootheningConfig config1) //works, but patch for OnSwitch aswell
    {
        bool fired = false;
        weapon.TryGetComponent<Shotgun      >(out Shotgun shotgun);
        weapon.TryGetComponent<ShotgunHammer>(out ShotgunHammer shotgunHammer);
        if(shotgun != null)             
        {
            MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed = false;
            if (!MonoSingleton<InputManager>.Instance.PerformingCheatMenuCombo() && !GameStateManager.Instance.PlayerInputLocked && shotgun.variation != 1 && shotgun.gunReady && shotgun.gc.activated && shotgun.charging && (shotgun.grenadeForce >= 60f  || config1.actionSmootheningTypeSecondary == ActionSmootheningTypeSecondary.FireOnSwitch))
            {
                shotgun.charging = false;
                fired = true;
                if (!(bool) (UnityEngine.Object) shotgun.wid || (double) shotgun.wid.delay == 0.0)
                {
                    shotgun.ShootSinks();
                }
                else
                {
                    shotgun.gunReady = false;
                    shotgun.Invoke(shotgun.variation == 0 ? "ShootSinks" : "ShootSaw", shotgun.wid.delay);
                }
                UnityEngine.Object.Destroy((UnityEngine.Object) shotgun.tempChargeSound.gameObject);
            }
        }
        else if(shotgunHammer != null)  
        {
            MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed = false;
            MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed = true;
            if (MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed && shotgunHammer.variation != 2 && (shotgunHammer.variation == 1 || (double) MonoSingleton<WeaponCharges>.Instance.shoAltNadeCharge >= 1.0) && !shotgunHammer.aboutToSecondary && shotgunHammer.gunReady && shotgunHammer.gc.activated && !GameStateManager.Instance.PlayerInputLocked)
            {
                shotgunHammer.gunReady = false;
                if (!(bool) (UnityEngine.Object) shotgunHammer.wid || (double) shotgunHammer.wid.delay == 0.0)
                {
                    fired = true;
                    if (shotgunHammer.variation == 0)
                        shotgunHammer.ThrowNade();
                    else
                        shotgunHammer.Pump();
                }
                else
                {
                    shotgunHammer.aboutToSecondary = true;
                    shotgunHammer.Invoke(shotgunHammer.variation == 0 ? "ThrowNade" : "Pump", shotgunHammer.wid.delay);
                }
            }
        }
        return fired;
    }
    public static bool AttemptShotgunAlt2Shoot(GameObject weapon, ActionSmootheningConfig config1) //works, but maybe stop at full pump?
    {
        bool fired = false;
        weapon.TryGetComponent<Shotgun      >(out Shotgun shotgun);
        weapon.TryGetComponent<ShotgunHammer>(out ShotgunHammer shotgunHammer);
        if(shotgun != null)             
        {
            MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed = true;
            //from decompiler
            if (MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed && shotgun.variation == 1 && shotgun.gunReady && shotgun.gc.activated && !GameStateManager.Instance.PlayerInputLocked && shotgun.primaryCharge != 3)
            {
                shotgun.gunReady = false;
                fired = true;
                if (!(bool) (UnityEngine.Object) shotgun.wid || (double) shotgun.wid.delay == 0.0) {shotgun.Pump();}
                else {shotgun.Invoke("Pump", shotgun.wid.delay);}
            }
        }
        else if(shotgunHammer != null)  
        {
            MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed = true;
            if (MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed && shotgunHammer.variation != 2 && (shotgunHammer.variation == 1 || (double) MonoSingleton<WeaponCharges>.Instance.shoAltNadeCharge >= 1.0) && !shotgunHammer.aboutToSecondary && shotgunHammer.gunReady && shotgunHammer.gc.activated && !GameStateManager.Instance.PlayerInputLocked && shotgunHammer.primaryCharge != 3)
            {
                shotgunHammer.gunReady = false;
                if (!(bool) (UnityEngine.Object) shotgunHammer.wid || (double) shotgunHammer.wid.delay == 0.0)
                {
                    fired = true;
                    shotgunHammer.Pump();
                }
                else
                {
                    shotgunHammer.aboutToSecondary = true;
                    shotgunHammer.Invoke(shotgunHammer.variation == 0 ? "ThrowNade" : "Pump", shotgunHammer.wid.delay);
                }
            }
        }
        return fired;
    }
    public static bool AttemptShotgunAlt3Shoot(GameObject weapon, ActionSmootheningConfig config1) //work?
    {
        bool fired = false;
        weapon.TryGetComponent<Shotgun      >(out Shotgun shotgun);
        weapon.TryGetComponent<ShotgunHammer>(out ShotgunHammer shotgunHammer);
        if(shotgun != null) 
        {
            MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed = false;
            if ((true || !MonoSingleton<InputManager>.Instance.PerformingCheatMenuCombo() && !GameStateManager.Instance.PlayerInputLocked) && shotgun.variation != 1 && shotgun.gunReady && shotgun.gc.activated && shotgun.charging)
            {
                shotgun.charging = false;
                if (shotgun.variation == 2)
                    MonoSingleton<WeaponCharges>.Instance.shoSawCharge = 0.0f;
                if (!(bool) (UnityEngine.Object) shotgun.wid || (double) shotgun.wid.delay == 0.0)
                {
                    fired = true;
                    if (shotgun.variation == 0)
                        shotgun.ShootSinks();
                    else
                        shotgun.ShootSaw();
                }
                else
                {
                    shotgun.gunReady = false;
                    shotgun.Invoke(shotgun.variation == 0 ? "ShootSinks" : "ShootSaw", shotgun.wid.delay);
                }
                UnityEngine.Object.Destroy((UnityEngine.Object) shotgun.tempChargeSound.gameObject);
            }
        }
        else if(shotgunHammer != null) 
        {
            MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed = false;
            if (!MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed && shotgunHammer.variation == 2 && shotgunHammer.gunReady && shotgunHammer.gc.activated && shotgunHammer.charging)
            {
                shotgunHammer.charging = false;
                MonoSingleton<WeaponCharges>.Instance.shoSawCharge = 0.0f;
                if (!(bool) (UnityEngine.Object) shotgunHammer.wid || (double) shotgunHammer.wid.delay == 0.0)
                {
                    fired = true;
                    shotgunHammer.ShootSaw();
                }
                else
                {
                    shotgunHammer.gunReady = false;
                    shotgunHammer.Invoke("ShootSaw", shotgunHammer.wid.delay);
                }
                if ((bool) (UnityEngine.Object) shotgunHammer.tempChargeSound)
                    UnityEngine.Object.Destroy((UnityEngine.Object) shotgunHammer.tempChargeSound.gameObject);
            }
        }
        return fired;
    }
    public static bool AttemptNailgunAlt1Shoot(GameObject weapon, ActionSmootheningConfig config1) //weird
    {
        bool fired = false;
        Nailgun nailgun = weapon.GetComponent<Nailgun>();
        MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed = true;
        if (true && nailgun.variation != 0)
        {
            if (nailgun.variation == 1 && (!(bool) (UnityEngine.Object) nailgun.wid || (double) nailgun.wid.delay == 0.0))
            {
                if ((double) nailgun.wc.naiMagnetCharge >= 1.0) {nailgun.ShootMagnet(); fired = true;}
                //else
                //    UnityEngine.Object.Instantiate<GameObject>(nailgun.noAmmoSound);
            }
        }
        return fired;
    }
    public static bool AttemptNailgunAlt2Shoot(GameObject weapon, ActionSmootheningConfig config1) //work
    {
        bool fired = false;
        Nailgun nailgun = weapon.GetComponent<Nailgun>();
        MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed = true;
        //from decompiler
        if (nailgun.canShoot && !nailgun.burnOut && (double) nailgun.heatSinks >= 1.0 && (double) nailgun.heatUp >= 0.10000000149011612 && nailgun.variation == 0 && true && nailgun.gc.activated && !GameStateManager.Instance.PlayerInputLocked)
        {
            fired = true;
            if (nailgun.altVersion)
            {
                nailgun.SuperSaw();
            }
            else
            {
                nailgun.burnOut = true;
                nailgun.fireCooldown = 0.0f;
                --nailgun.heatSinks;
                nailgun.heatSteam?.Play();
                nailgun.heatSteamAud?.Play();
                nailgun.currentFireRate = nailgun.fireRate - 2.5f;
            }
        }
        return fired;
    }
    public static bool AttemptNailgunAlt3Shoot(GameObject weapon, ActionSmootheningConfig config1) //work
    {
        bool fired = false;
        Nailgun nailgun = weapon.GetComponent<Nailgun>();
        MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed = true;
        //from decompiler
        if ((double) MonoSingleton<WeaponCharges>.Instance.naiZapperRecharge >= 5.0 && nailgun.currentZapper == null)
        {
            fired = true;
            if (!(bool) (UnityEngine.Object) nailgun.wid || (double) nailgun.wid.delay == 0.0) {nailgun.ShootZapper();}
            else
                nailgun.Invoke("ShootZapper", nailgun.wid.delay);
        }
        else if(nailgun.currentZapper == null && nailgun.noAmmoSound != null)
            UnityEngine.Object.Instantiate<GameObject>(nailgun.noAmmoSound);
        return fired;
    }
    public static bool forceFirceZoom = false;
    public static bool AttemptRailcannonAltShoot(GameObject weapon, ActionSmootheningConfig config1) //work good enough
    {
        bool fired = true;
        forceFirceZoom = true;
        return fired;
    }
    public static bool AttemptRocketLauncherAlt1Shoot(GameObject weapon, ActionSmootheningConfig config1) //work
    {
        bool fired = false;
        RocketLauncher rocketLauncher = weapon.GetComponent<RocketLauncher>();
        //MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed = true; //redundant
        //from decompiler
        if ((double) MonoSingleton<WeaponCharges>.Instance.rocketFreezeTime > 0.0 && !GameStateManager.Instance.PlayerInputLocked && (!(bool) (UnityEngine.Object) rocketLauncher.wid || !rocketLauncher.wid.duplicate))
        {
            fired = true;
            if (MonoSingleton<WeaponCharges>.Instance.rocketFrozen)
                rocketLauncher.UnfreezeRockets();
            else
                rocketLauncher.FreezeRockets();
        }
        return fired;
    }
    public static bool AttemptRocketLauncherAlt2Shoot(GameObject weapon, ActionSmootheningConfig config1) //work but improve for OnSwitch
    {
        bool fired = false;
        RocketLauncher rocketLauncher = weapon.GetComponent<RocketLauncher>();
        //MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed = false;
        //from decompiler
        if ((double) rocketLauncher.cbCharge > 0.0 && !rocketLauncher.firingCannonball)
        {
            if(rocketLauncher.cbCharge >= 1.0f || config1.actionSmootheningTypeSecondary == ActionSmootheningTypeSecondary.FireOnSwitch) //if player is not holding right click or if fully charged    lastFireStateSecondary == false || 
            {
                rocketLauncher.chargeSound.Stop();
                fired = true;
                if (!(bool) (UnityEngine.Object) rocketLauncher.wid || (double) rocketLauncher.wid.delay == 0.0)
                {
                    rocketLauncher.ShootCannonball();
                }
                else
                {
                    rocketLauncher.Invoke("ShootCannonball", rocketLauncher.wid.delay);
                    rocketLauncher.firingCannonball = true;
                }
                MonoSingleton<WeaponCharges>.Instance.rocketCannonballCharge = 0.0f;
            }
        }
        return fired;
    }
    public static bool forceFirceNapalm = false;
    public static bool AttemptRocketLauncherAlt3Shoot(GameObject weapon, ActionSmootheningConfig config1) //work
    {
        bool fired = true; //this is meh
        RocketLauncher rocketLauncher = weapon.GetComponent<RocketLauncher>();
        MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed = true; //uhhh sure?
        forceFirceNapalm = true;
        
        return fired;
    }

    public static bool AttemptPrimaryShoot(ActionSmootheningConfig config1)
    {
        //return true if fired

        bool fired = false;
        lastFireStatePrimary = MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed;
        lastFireStateSecondary = MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed;
        int[] temp = PluginConfig.getWeaponSlotVariation(MonoSingleton<GunControl>.Instance.currentWeapon);
        int slot = temp[0];
        int variation = temp[1];
        List<GameObject> weaponList = new List<GameObject>();
        weaponList.Add(MonoSingleton<GunControl>.Instance.currentWeapon);
        DualWield[] dualwields = MonoSingleton<GunControl>.Instance.GetComponentsInChildren<DualWield>();
        for(int i = 0; i < dualwields.Length; i++)
        {
            weaponList.Add(dualwields[i].currentWeapon);
        }
        for(int i = 0; i < weaponList.Count; i++)
        {
            GameObject NthWeapon = weaponList[i];
            MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed = true; //this is very strange... but it works!
            if(slot == 1 && NthWeapon.GetComponent<Revolver> != null) 
            {
                if(AttemptRevolverShoot(NthWeapon, config1)) {fired = true;}
            }
            else if(slot == 2 && (NthWeapon.GetComponent<Shotgun> != null || NthWeapon.GetComponent<ShotgunHammer> != null))
            {
                if(AttemptShotgunShoot(NthWeapon, config1)) {fired = true;}
            }
            else if(slot == 3 && NthWeapon.GetComponent<Nailgun> != null)
            {
                if(AttemptNailgunShoot(NthWeapon, config1)) {fired = true;}
            }
            else if(slot == 4 && NthWeapon.GetComponent<Railcannon> != null)
            {
                if(AttemptRailcannonShoot(NthWeapon, config1)) {fired = true;}
            }
            else if(slot == 5 && NthWeapon.GetComponent<RocketLauncher> != null)
            {
                if(AttemptRocketLauncherShoot(NthWeapon, config1)) {fired = true;}
            }
        }
        MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed = lastFireStatePrimary;
        MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed = lastFireStateSecondary;
        return fired;
    }
    public static bool AttemptSecondaryShoot(ActionSmootheningConfig config1)
    {
        //return true if fired
        bool fired = false;
        lastFireStatePrimary = MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed;
        lastFireStateSecondary = MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed;
        int[] temp = PluginConfig.getWeaponSlotVariation(MonoSingleton<GunControl>.Instance.currentWeapon);
        int slot = temp[0];
        int variation = temp[1];
        List<GameObject> weaponList = new List<GameObject>();
        weaponList.Add(MonoSingleton<GunControl>.Instance.currentWeapon);
        DualWield[] dualwields = MonoSingleton<GunControl>.Instance.GetComponentsInChildren<DualWield>();
        for(int i = 0; i < dualwields.Length; i++)
        {
            weaponList.Add(dualwields[i].currentWeapon);
        }
        for(int i = 0; i < weaponList.Count; i++)
        {
            GameObject NthWeapon = weaponList[i];
            if(slot == 1 && NthWeapon.GetComponent<Revolver> != null) 
            {
                if(variation == 0) {AttemptRevolverAlt1Shoot(NthWeapon, config1);}
                else if(variation == 1) {AttemptRevolverAlt2Shoot(NthWeapon, config1);}
                else if(variation == 2) {AttemptRevolverAlt3Shoot(NthWeapon, config1);}
            }
            else if(slot == 2 && (NthWeapon.GetComponent<Shotgun> != null || NthWeapon.GetComponent<ShotgunHammer> != null))
            {
                if(variation == 0) {AttemptShotgunAlt1Shoot(NthWeapon, config1);}
                else if(variation == 1) {AttemptShotgunAlt2Shoot(NthWeapon, config1);}
                else if(variation == 2) {AttemptShotgunAlt3Shoot(NthWeapon, config1);}
            }
            else if(slot == 3 && NthWeapon.GetComponent<Nailgun> != null)
            {
                if(variation == 0) {AttemptNailgunAlt1Shoot(NthWeapon, config1);}
                else if(variation == 1) {AttemptNailgunAlt2Shoot(NthWeapon, config1);}
                else if(variation == 2) {AttemptNailgunAlt3Shoot(NthWeapon, config1);}
            }
            else if(slot == 4 && NthWeapon.GetComponent<Railcannon> != null)
            {
                //sure???
                MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed = true; 
                AttemptRailcannonAltShoot(NthWeapon, config1);
            }
            else if(slot == 5 && NthWeapon.GetComponent<RocketLauncher> != null)
            {
                if(variation == 0) {AttemptRocketLauncherAlt1Shoot(NthWeapon, config1);}
                else if(variation == 1) {AttemptRocketLauncherAlt2Shoot(NthWeapon, config1);}
                else if(variation == 2) {AttemptRocketLauncherAlt3Shoot(NthWeapon, config1);}
            }
        }
        MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed = lastFireStatePrimary;
        MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed = lastFireStateSecondary;
        return fired;
    }

    public static float lastTimeSwitchStarted = 0;
    public static float GetCurrentTimeUntilWeaponDeploy()
    {
        //negative for duration it has been out since deploy

        int[] temp = PluginConfig.getWeaponSlotVariation(MonoSingleton<GunControl>.Instance.currentWeapon);
        int slot = temp[0];
        int variation = temp[1];

        float timeDeploy = 0.25f;

        if(slot == 1) 
        {
            if(MonoSingleton<GunControl>.Instance.currentWeapon.GetComponent<Revolver>().altVersion) {timeDeploy = 0.40f;}
            else {timeDeploy = 0.36f;}
        }
        else if(slot == 2)
        {
            if(MonoSingleton<GunControl>.Instance.currentWeapon.GetComponent<ShotgunHammer>() != null) {timeDeploy = 0f;}
            else {timeDeploy = 0.40f;}
        }
        else if(slot == 3)
        {
            timeDeploy = 0.31f;
        }
        else if(slot == 4)
        {
            timeDeploy = 0.00f; //close enough?
        }
        else if(slot == 5)
        {
            timeDeploy = 0.25f;
        }
        return lastTimeSwitchStarted - Time.time + timeDeploy; //assume switch time is 0.25s for all weps... seems right?
    }
    public static void ActionSmootheningLogicMAIN()
    {
        if(Plugin.modEnabled == false || actionSmootheningEnabled == false) {return;}
        //PluginConfig.actionSmootheningConfigs
        timeUntilCurrentWeaponDeploy = GetCurrentTimeUntilWeaponDeploy();
        CheckForFireBeforeDeploy();
        AutoShootLogic();
        AutoSwitchLogic();
    }
    public static void AutoSwitchLogic()
    {
        if(Time.time > switchTime && switchTime >= 0f)
        {
            //order matters
            MonoSingleton<GunControl>.Instance.ForceWeapon(weaponToSwitchTo, true);

            //below should make these weapon switches work with the rest of the mod!
            //swithing WC is important!
            Plugin.wcArray[0] = wcOld;
            Plugin.wcArray[1] = wcToSwitchTo;
            Plugin.weapon = MonoSingleton<GunControl>.Instance.currentWeapon;
                
            DualWield[] dualwields = MonoSingleton<GunControl>.Instance.GetComponentsInChildren<DualWield>();
            for(int i = 0; i < dualwields.Length; i++)
            {
                //dualwields[i].currentWeapon = weapon;
                dualwields[i].UpdateWeapon(Plugin.weapon);
                //logger.LogInfo(i + " " + dualwields[i].currentWeapon);
            }

            switchTime = -1f;
            weaponToSwitchTo = null;
            wcToSwitchTo = null;
        }
    }
    public static void AutoShootLogic()
    {
        int[] temp = PluginConfig.getWeaponSlotVariation(MonoSingleton<GunControl>.Instance.currentWeapon);
        int slot = temp[0];
        int variation = temp[1];

        if(slot < 1 || variation < 0) {return;}
        ActionSmootheningConfig config1 = PluginConfig.actionSmootheningConfigs[slot - 1, variation];

        if(timeUntilCurrentWeaponDeploy <= 0f && shootWhenReady != 0)
        {
            if(shootWhenReady == 1)
            {
                //primary fire current weapon
                if(AttemptPrimaryShoot(config1)) {shootWhenReady = 0;}
            }
            else if(shootWhenReady == 2)
            {
                //secondary fire current weapon
                if(AttemptSecondaryShoot(config1)) {shootWhenReady = 0;}
            }
        }

        if(config1.enabled && config1.actionSmootheningTypePrimary == ActionSmootheningTypePrimary.FireOnReady) 
        {
            //attempt primary fire current weapon
            AttemptPrimaryShoot(config1);
        }
        else if(config1.enabled && config1.actionSmootheningTypeSecondary == ActionSmootheningTypeSecondary.FireOnReady) 
        {
            //attempt secondary fire current weapon
            AttemptSecondaryShoot(config1);
        }
    }
    public static int shootWhenReady = 0; //0 is nothing, 1 is primary fire, 2 is secondary fire
    public static void CheckForFireBeforeDeploy()
    {
        if(timeUntilCurrentWeaponDeploy > 0f)
        {
            int[] temp = PluginConfig.getWeaponSlotVariation(MonoSingleton<GunControl>.Instance.currentWeapon);
            int slot = temp[0];
            int variation = temp[1];

            if(slot < 1 || variation < 0) {return;}
            ActionSmootheningConfig config1 = PluginConfig.actionSmootheningConfigs[slot - 1, variation];

            //Plugin.logger.LogInfo(timeUntilCurrentWeaponDeploy + " " + config1.rememberInputBeforeFullSwitchTime);
            if(config1.enabled && config1.rememberInputBeforeFullSwitch && timeUntilCurrentWeaponDeploy <= config1.rememberInputBeforeFullSwitchTime)
            {
                if(MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed)
                {
                    shootWhenReady = 1;
                }

                //most alt fires wont work for this I guess but its here?
                else if(MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed && config1.rememberPrimaryInputOnly == false)
                {
                    //rocket launcher alts work instantly so we don't even want to do this
                    if(slot != 5) {shootWhenReady = 2;}
                }
            }
            //shootWhenReady reset to 0 upon shot or weapon switch
        }
    }

    [HarmonyPatch(typeof(GunControl), "SwitchWeapon")]
    public class SwitchWeaponPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(int targetSlotIndex, int? targetVariationIndex = null, bool useRetainedVariation = false, bool cycleSlot = false, bool cycleVariation = false)
        {
            //if(!actionSmootheningEnabled || !Plugin.modEnabled) {return true;}
            //return false;
            if(targetSlotIndex > 5) {return true;} //remove base game weapon switching except for slot 6
            else {return false;}
        }
    }

    //prevents current wc being pushed to previous wc in case of WaitOnSwitch while we are waiting. 
    public static bool PreventWCOrderCycling = false;
    
    [HarmonyPatch(typeof(GunControl), "ForceWeapon")]
    public class ForceWeaponPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(GameObject weapon, bool setActive)
        {
            if(!actionSmootheningEnabled || !Plugin.modEnabled) {return true;}

            shootWhenReady = 0; 
            int[] temp = PluginConfig.getWeaponSlotVariation(MonoSingleton<GunControl>.Instance.currentWeapon);
            int slot = temp[0];
            int variation = temp[1];

            if(slot < 1 || variation < 0) {return true;}
            ActionSmootheningConfig config1 = PluginConfig.actionSmootheningConfigs[slot - 1, variation];

            if(switchTime >= 0f) // >= 0 if we are waiting / about to switch
            {
                if(Time.time > switchTime) 
                {
                    //we're switching NOW
                    PreventWCOrderCycling = false;
                    return true;
                }
                else
                {
                    //change weapon target now while waiting, but dont switch yet
                    if(weaponToSwitchTo == weapon) {switchTime = Time.time;} //user tried to switch to same weapon twice, so give it to them... may be problematic if the weapon occurs in multiple weapon cycles?
                    weaponToSwitchTo = weapon; 

                    return false;
                }
            } 

            if(config1.enabled == false) {return true;}
            else
            {
                if(config1.actionSmootheningTypeSecondary == ActionSmootheningTypeSecondary.FireOnSwitch)
                {
                    if(timeUntilCurrentWeaponDeploy > -config1.maxTime2)
                    {
                        //fire
                        if(AttemptSecondaryShoot(config1)) {return true;}
                    }
                }
                if(config1.actionSmootheningTypePrimary == ActionSmootheningTypePrimary.FireOnSwitch)
                {
                    if(timeUntilCurrentWeaponDeploy > -config1.maxTime)
                    {
                        //fire
                        if(AttemptPrimaryShoot(config1)) {return true;}
                    }
                }
                if(config1.actionSmootheningTypePrimary == ActionSmootheningTypePrimary.WaitOnSwitch)
                {
                    switchTime = Time.time + config1.maxTime;
                    weaponToSwitchTo = weapon;
                    //wcToSwitchTo = Plugin.wcArray[1];

                    if(PreventWCOrderCycling == false) {wcOld = Plugin.newWCThisTick;} //PreventWCOrderCyclingStartThisTick
                    PreventWCOrderCycling = true;
                    return false;
                    //deny switching
                }
                else if(config1.actionSmootheningTypeSecondary == ActionSmootheningTypeSecondary.None) 
                {
                    //nothing
                    return true;
                }
                else if(config1.actionSmootheningTypeSecondary == ActionSmootheningTypeSecondary.FireOnReady)
                {
                    //nothing here
                    return true;
                }
                else if(config1.actionSmootheningTypePrimary == ActionSmootheningTypePrimary.None) 
                {
                    //nothing
                    return true;
                }
                else if(config1.actionSmootheningTypePrimary == ActionSmootheningTypePrimary.FireOnReady)
                {
                    //nothing here
                    return true;
                }
            }
        
            return false;
        }
        
        [HarmonyPostfix]
        private static void Postfix(GameObject weapon, bool setActive)
        {
            lastTimeSwitchStarted = Time.time;
        }
    }

    //firestarter is such a mess that this is required
    [HarmonyPatch(typeof(RocketLauncher), "Update")]
    public class RocketLauncherUpdatePatch
    {
        [HarmonyPrefix]
        private static bool Prefix(RocketLauncher __instance)
        {
            if(forceFirceNapalm && __instance.GetComponent<RocketLauncher>().variation == 2) 
            {
                if (!(bool) (UnityEngine.Object) MonoSingleton<ColorBlindSettings>.Instance)
                    return false;
                Color color = MonoSingleton<ColorBlindSettings>.Instance.variationColors[__instance.variation];
                float num1 = 1f;
                if (MonoSingleton<WeaponCharges>.Instance.rocketset && __instance.lookingForValue)
                {
                    MonoSingleton<WeaponCharges>.Instance.rocketset = false;
                    __instance.lookingForValue = false;
                    __instance.cooldown = (double) MonoSingleton<WeaponCharges>.Instance.rocketcharge >= 0.25 ? MonoSingleton<WeaponCharges>.Instance.rocketcharge : 0.25f;
                }
                else if (__instance.variation == 2)
                {
                    if (!MonoSingleton<InputManager>.Instance.PerformingCheatMenuCombo() && !GameStateManager.Instance.PlayerInputLocked && true && !MonoSingleton<InputManager>.Instance.InputSource.Fire1.WasPerformedThisFrame && (double) MonoSingleton<WeaponCharges>.Instance.rocketNapalmFuel > 0.0 && (__instance.firingNapalm || (double) MonoSingleton<WeaponCharges>.Instance.rocketNapalmFuel >= 0.25))
                    {
                        if ((double) __instance.cooldown < 0.5)
                        {
                            if (!__instance.firingNapalm)
                            {
                                __instance.napalmMuzzleFlashTransform.localScale = Vector3.one * 3f;
                                __instance.napalmMuzzleFlashParticles.Play();
                                foreach (AudioSource muzzleFlashSound in __instance.napalmMuzzleFlashSounds)
                                {
                                muzzleFlashSound.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                                muzzleFlashSound.Play();
                                }
                            }
                            __instance.firingNapalm = true;
                        }
                    }
                    else if (__instance.firingNapalm)
                    {
                        __instance.firingNapalm = false;
                        __instance.napalmMuzzleFlashParticles.Stop();
                        foreach (AudioSource muzzleFlashSound in __instance.napalmMuzzleFlashSounds)
                        {
                        if (muzzleFlashSound.loop)
                            muzzleFlashSound.Stop();
                        }
                        __instance.napalmStopSound.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                        __instance.napalmStopSound.Play();
                    }
                    else if (false && (double) MonoSingleton<WeaponCharges>.Instance.rocketNapalmFuel < 0.25)
                        __instance.napalmNoAmmoSound.Play();
                    if (!__instance.firingNapalm && __instance.napalmMuzzleFlashTransform.localScale != Vector3.zero)
                        __instance.napalmMuzzleFlashTransform.localScale = Vector3.MoveTowards(__instance.napalmMuzzleFlashTransform.localScale, Vector3.zero, Time.deltaTime * 9f);
                    if ((bool) (UnityEngine.Object) __instance.timerArm)
                        __instance.timerArm.localRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Lerp(360f, 0.0f, MonoSingleton<WeaponCharges>.Instance.rocketNapalmFuel));
                    if ((bool) (UnityEngine.Object) __instance.timerMeter)
                        __instance.timerMeter.fillAmount = MonoSingleton<WeaponCharges>.Instance.rocketNapalmFuel;
                    if ((double) __instance.lastKnownTimerAmount != (double) MonoSingleton<WeaponCharges>.Instance.rocketNapalmFuel && (!(bool) (UnityEngine.Object) __instance.wid || (double) __instance.wid.delay == 0.0))
                    {
                        if ((double) MonoSingleton<WeaponCharges>.Instance.rocketNapalmFuel >= 0.25 && (double) __instance.lastKnownTimerAmount < 0.25)
                        UnityEngine.Object.Instantiate<AudioSource>(__instance.timerWindupSound);
                        __instance.lastKnownTimerAmount = MonoSingleton<WeaponCharges>.Instance.rocketNapalmFuel;
                    }
                    if (!__instance.firingNapalm && (double) MonoSingleton<WeaponCharges>.Instance.rocketNapalmFuel < 0.25)
                        color = Color.grey;
                }
                if ((double) __instance.cooldown > 0.0)
                    __instance.cooldown = Mathf.MoveTowards(__instance.cooldown, 0.0f, Time.deltaTime);
                else if (MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed && MonoSingleton<GunControl>.Instance.activated && !GameStateManager.Instance.PlayerInputLocked)
                {
                    if (!(bool) (UnityEngine.Object) __instance.wid || (double) __instance.wid.delay == 0.0)
                    {
                        __instance.Shoot();
                    }
                    else
                    {
                        __instance.Invoke("Shoot", __instance.wid.delay);
                        __instance.cooldown = 1f;
                    }
                }
                for (int index = 0; index < __instance.variationColorables.Length; ++index)
                    __instance.variationColorables[index].color = new Color(color.r, color.g, color.b, __instance.colorablesTransparencies[index] * num1);

                forceFirceNapalm = false; //is set to true again if needed for next tick
                return false;
            }

            else {forceFirceNapalm = false; return true;}
        }

        [HarmonyPostfix]
        private static void Postfix(RocketLauncher __instance)
        {
            
        }
    }

    [HarmonyPatch(typeof(Railcannon), "Update")]
    public class RailcannonUpdatePatch
    {
        [HarmonyPrefix]
        private static bool Prefix(Railcannon __instance)
        {
            if(forceFirceZoom)
            {
                if ((double) __instance.wid.delay > 0.0 && (double) __instance.altCharge < (double) __instance.wc.raicharge)
                    __instance.altCharge = __instance.wc.raicharge;
                float newIntensity = __instance.wc.raicharge;
                if ((double) __instance.wid.delay > 0.0)
                newIntensity = __instance.altCharge;
                if ((double) newIntensity < 5.0) //tf is this??? && !NoWeaponCooldown.NoCooldown)
                {
                    __instance.SetMaterialIntensity(newIntensity, true);
                }
                else
                {
                    MonoSingleton<RumbleManager>.Instance.SetVibrationTracked(RumbleProperties.RailcannonIdle, __instance.fullCharge);
                    if (!__instance.fullCharge.activeSelf)
                    {
                        __instance.fullCharge.SetActive(true);
                        if (__instance.variation == 2)
                        {
                            __instance.pitchRise = true;
                            __instance.fullAud.pitch = 0.0f;
                        }
                    }
                    if (!__instance.wc.railChargePlayed)
                        __instance.wc.PlayRailCharge();
                    __instance.transform.localPosition = new Vector3(__instance.wpos.currentDefault.x + UnityEngine.Random.Range(-0.005f, 0.005f), __instance.wpos.currentDefault.y + UnityEngine.Random.Range(-0.005f, 0.005f), __instance.wpos.currentDefault.z + UnityEngine.Random.Range(-0.005f, 0.005f));
                    __instance.SetMaterialIntensity(1f, false);
                    Color variationColor = MonoSingleton<ColorBlindSettings>.Instance.variationColors[__instance.variation];
                    __instance.fullChargeLight.color = variationColor;
                    //__instance.fullChargeParticles.main.startColor = (ParticleSystem.MinMaxGradient) variationColor; //errors out? tf?
                    if (!MonoSingleton<InputManager>.Instance.PerformingCheatMenuCombo() && MonoSingleton<InputManager>.Instance.InputSource.Fire1.WasPerformedThisFrame && __instance.gc.activated && !GameStateManager.Instance.PlayerInputLocked)
                    {
                        __instance.fullCharge.SetActive(false);
                        __instance.transform.localPosition = __instance.wpos.currentDefault;
                        __instance.wc.raicharge = 0.0f;
                        __instance.wc.railChargePlayed = false;
                        __instance.altCharge = 0.0f;
                        if (!(bool) (UnityEngine.Object) __instance.wid || (double) __instance.wid.delay == 0.0)
                            __instance.Shoot();
                        else
                            __instance.Invoke("Shoot", __instance.wid.delay);
                    }
                }
                if (!(bool) (UnityEngine.Object) __instance.wid || (double) __instance.wid.delay == 0.0)
                {
                    if (true && __instance.gc.activated && !GameStateManager.Instance.PlayerInputLocked)
                    {
                        __instance.zooming = true;
                        __instance.cc.Zoom(__instance.cc.defaultFov / 2f);
                    }
                    else if (__instance.zooming)
                    {
                        __instance.zooming = false;
                        __instance.cc.StopZoom();
                    }
                }
                if ((double) __instance.wid.delay != (double) __instance.gotWidDelay)
                {
                    __instance.gotWidDelay = __instance.wid.delay;
                    if ((bool) (UnityEngine.Object) __instance.wid && (double) __instance.wid.delay != 0.0)
                    {
                        __instance.fullAud.volume -= __instance.wid.delay * 2f;
                        if ((double) __instance.fullAud.volume < 0.0)
                        __instance.fullAud.volume = 0.0f;
                    }
                }
                if (!__instance.pitchRise)
                    return false;
                __instance.fullAud.pitch = Mathf.MoveTowards(__instance.fullAud.pitch, 2f, Time.deltaTime * 4f);
                if ((double) __instance.fullAud.pitch != 2.0)
                    return false;
                __instance.pitchRise = false;

                forceFirceZoom = false;
                return false;
            }
            else {return true;}
        }
    }
}