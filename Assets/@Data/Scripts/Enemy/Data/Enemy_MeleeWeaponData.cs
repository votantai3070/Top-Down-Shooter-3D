using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New enemy melee weapon", menuName = "Enemy data/Melee weapon data")]
public class Enemy_MeleeWeaponData : ScriptableObject
{
    public List<AttackData_EnemyMelee> attackData;
    public float turnSpeed = 10;
}
