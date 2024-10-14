using System;
using System.Collections;
using CoolTools.Actors;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DamageableTest
{
    private Damageable InitializeDamageable(int maxHealth)
    {
        var go = new GameObject("Test Damagebale");
        
        var actor = go.AddComponent<Actor>();
        actor.InitMode = Actor.ActorInitMode.Script;

        var damageable = go.AddComponent<Damageable>();
        damageable.Owner = actor;

        // damageable.MaxHealth = new IntFormulaEval()
        // {
        //     BaseValue = maxHealth,
        // };
        // damageable.HealthRegeneration = new FloatFormulaEval()
        // {
        //     BaseValue = 0,
        // };
        damageable.Multipliers = Array.Empty<Damageable.DamageMultipliers>();

        actor.Initialize();
        damageable.EvaluateAll();
        damageable.RestoreHealth();

        return damageable;
    }
    
    [Test]
    public void InitTest()
    {
        var damageable = InitializeDamageable(100);
        
        // Test to see if we initialized properly.
        Assert.AreEqual(100, damageable.MaxHealth);
        Assert.AreEqual(100, damageable.Health);
    }
    
    [Test]
    public void DamageTest()
    {
        var damageable = InitializeDamageable(100);

        damageable.DealDamage(10, DamageType.None);

        Assert.AreEqual(90, damageable.Health);
    }
    
    [Test]
    public void HealingTest()
    {
        var damageable = InitializeDamageable(100);

        damageable.DealDamage(10, DamageType.None);
        damageable.DealDamage(-5, DamageType.None);

        Assert.AreEqual(95, damageable.Health);
    }
    
    [Test]
    public void OverhealingTest()
    {
        var damageable = InitializeDamageable(100);

        damageable.DealDamage(10, DamageType.None);
        damageable.DealDamage(-15, DamageType.None);

        Assert.AreEqual(damageable.Health, damageable.MaxHealth);
    }
    
    [Test]
    public void InvincibilityTest()
    {
        var damageable = InitializeDamageable(100);

        damageable.Invincible = true;
        damageable.DealDamage(10, DamageType.None);

        Assert.AreEqual(damageable.Health, damageable.MaxHealth);
    }
    
    [Test]
    public void DeathTest()
    {
        var damageable = InitializeDamageable(100);

        damageable.DealDamage(100, DamageType.None);

        Assert.IsFalse(damageable.IsAlive);
    }

}
