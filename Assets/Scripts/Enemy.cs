using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public enum EnemyType
    {
        AxeBandit, EarthWisp, EyeDemon, FireWisp, Goblin, Kobold, Mimic, Minataur, Oculothorax, Ogre, Slime, Sorcerer, WaterWisp, WindWisp 
    }
    public EnemyType type;
    public int maxHealth, currentHealth, projSpeed = 15;
    public float damage, moveSpeed;
}
