using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class TargetAndMoveSystem : ISystem
{
    public World World { get; set; }

    private Filter _unitsFilter;

    private Stash<PositionComponent> _posStash;
    private Stash<TeamComponent> _teamStash;
    private Stash<MoveSpeedComponent> _speedStash;
    private Stash<VelocityComponent> _velStash;
    private Stash<AttackStatsComponent> _attackStash;

    public void OnAwake()
    {
        _posStash = World.GetStash<PositionComponent>();
        _teamStash = World.GetStash<TeamComponent>();
        _speedStash = World.GetStash<MoveSpeedComponent>();
        _velStash = World.GetStash<VelocityComponent>();
        _attackStash = World.GetStash<AttackStatsComponent>();

        _unitsFilter = World.Filter
            .With<UnitComponent>()
            .With<PositionComponent>()
            .With<TeamComponent>()
            .With<MoveSpeedComponent>()
            .Without<DeadComponent>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _unitsFilter)
        {
            ref var pos = ref _posStash.Get(entity);
            ref var team = ref _teamStash.Get(entity);
            ref var speed = ref _speedStash.Get(entity);
            ref var vel = ref _velStash.Get(entity);
            ref var attack = ref _attackStash.Get(entity);

            Entity closestEnemy = default;
            float minSqrDist = float.MaxValue;
            bool enemyFound = false;

            foreach (var otherEntity in _unitsFilter)
            {
                ref var otherTeam = ref _teamStash.Get(otherEntity);
                if (otherTeam.id == team.id) continue;

                ref var otherPos = ref _posStash.Get(otherEntity);
                float sqrDist = (otherPos.value - pos.value).sqrMagnitude;

                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    closestEnemy = otherEntity;
                    enemyFound = true;
                }
            }

            if (enemyFound)
            {
                ref var enemyPos = ref _posStash.Get(closestEnemy);
                Vector3 toEnemy = enemyPos.value - pos.value;
                toEnemy.y = 0;
                float dist = toEnemy.magnitude;

                if (dist > attack.range)
                {
                    Vector3 dir = toEnemy / dist;
                    vel.value = dir * speed.value;
                }
                else
                {
                    vel.value = Vector3.zero;
                }
            }
            else
            {
                vel.value = Vector3.zero;
            }
        }
    }

    public void Dispose() { }
}

public class MovementSystem : ISystem
{
    public World World { get; set; }

    private Filter _movableFilter;

    private Stash<PositionComponent> _posStash;
    private Stash<VelocityComponent> _velStash;
    private Stash<RadiusComponent> _radiusStash;

    private readonly List<Entity> _entitiesCache = new List<Entity>(128);

    public void OnAwake()
    {
        _posStash = World.GetStash<PositionComponent>();
        _velStash = World.GetStash<VelocityComponent>();
        _radiusStash = World.GetStash<RadiusComponent>();

        _movableFilter = World.Filter
            .With<PositionComponent>()
            .With<VelocityComponent>()
            .With<RadiusComponent>()
            .Without<DeadComponent>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        _entitiesCache.Clear();

        foreach (var entity in _movableFilter)
        {
            ref var pos = ref _posStash.Get(entity);
            ref var vel = ref _velStash.Get(entity);

            if (vel.value.sqrMagnitude > 0.0001f)
                pos.value += vel.value * deltaTime;

            _entitiesCache.Add(entity);
        }

        int count = _entitiesCache.Count;

        for (int i = 0; i < count; i++)
        {
            var a = _entitiesCache[i];
            ref var posA = ref _posStash.Get(a);
            ref var radA = ref _radiusStash.Get(a);

            for (int j = i + 1; j < count; j++)
            {
                var b = _entitiesCache[j];
                ref var posB = ref _posStash.Get(b);
                ref var radB = ref _radiusStash.Get(b);

                Vector3 delta = posA.value - posB.value;
                delta.y = 0;
                float minDist = radA.value + radB.value;
                float sqrDist = delta.sqrMagnitude;

                if (sqrDist < minDist * minDist && sqrDist > 0.0001f)
                {
                    float dist = Mathf.Sqrt(sqrDist);
                    Vector3 dir = delta / dist;
                    float push = (minDist - dist) * 0.5f;

                    posA.value += dir * push;
                    posB.value -= dir * push;
                }
            }
        }
    }

    public void Dispose() { }
}

public class ViewSyncSystem : ISystem
{
    public World World { get; set; }
    private Filter _viewFilter;

    private Stash<PositionComponent> _posStash;
    private Stash<VelocityComponent> _velStash;
    private Stash<ViewComponent> _viewStash;

    public void OnAwake()
    {
        _posStash = World.GetStash<PositionComponent>();
        _velStash = World.GetStash<VelocityComponent>();
        _viewStash = World.GetStash<ViewComponent>();

        _viewFilter = World.Filter
            .With<PositionComponent>()
            .With<ViewComponent>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _viewFilter)
        {
            ref var pos = ref _posStash.Get(entity);
            ref var view = ref _viewStash.Get(entity);
            ref var vel = ref _velStash.Get(entity);

            view.view.SetPosition(pos.value);
            view.view.SetFacing(vel.value.x > 0);
            view.view.ResetVisualScale();
        }
    }

    public void Dispose() { }
}