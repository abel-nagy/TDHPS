using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {

    // Weapons
    public Guns gunSlot1 = Guns.AssaultRifle;
    public Guns gunSlot2 = Guns.None;
    public Throwables throwableSlot1 = Throwables.None;
    public Throwables throwableSlot2 = Throwables.None;

    public int[] gunAmmo = new int[3];
    


    public Inventory() {
        gunAmmo[(int)Guns.AssaultRifle] = 50;
    }


}

public enum Guns {
    None, 
    AssaultRifle, 
    SniperRifle, 
    Pistol
}

public enum Throwables {
    None, 
    Grenade, 
    Flashbang, 
    Molotov, 
    Smoke, 
    Rock
}