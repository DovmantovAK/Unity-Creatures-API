using System;
using UnityEngine;

namespace Unity.Creatures
{
    [Serializable]
    public struct Creature
    {
        public string Id;
        public string CreatureName;
        public float Health;
        public bool IsImmortal;
        public Sprite Portrait; 
        public RuntimeAnimatorController AnimatorController;
        public float BaseSpeed;
        public float SprintMultiplier;

        public Creature(string id, 
            string creatureName, 
            float health, 
            float baseSpeed, 
            float sprintMultiplier) : this()
        {
            Id = id;
            CreatureName = creatureName;
            Health = health;
            BaseSpeed = baseSpeed;
            SprintMultiplier = sprintMultiplier;
        }
    }
}