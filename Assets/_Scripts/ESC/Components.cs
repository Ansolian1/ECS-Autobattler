using Scellecs.Morpeh;
using UnityEngine;

public struct UnitComponent : IComponent { }
public struct TeamComponent : IComponent
{
    public int id; // 0 = Нежить (Скелеты), 1 = Люди
}

public struct PositionComponent : IComponent
{
    public Vector3 value;
}

public struct VelocityComponent : IComponent
{
    public Vector3 value;
}

public struct MoveSpeedComponent : IComponent
{
    public float value;
}

public struct RadiusComponent : IComponent
{
    public float value;
}

public struct HealthComponent : IComponent
{
    public float current;
    public float max;
}

public struct AttackStatsComponent : IComponent
{
    public float range;
    public float cooldown;
    public float damage;
}

public struct AttackCooldownComponent : IComponent
{
    public float timer;
}

public struct TargetComponent : IComponent
{
    public Entity targetEntity;
    public bool hasTarget;
}

public struct ViewComponent : IComponent
{
    public UnitView view;
}
public struct UnitAudioComponent : IComponent
{
    public AudioClip attackSound;
    public AudioClip abilitySound;
}
public struct DeadComponent : IComponent { }