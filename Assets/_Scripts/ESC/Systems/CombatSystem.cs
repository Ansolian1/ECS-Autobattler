using Scellecs.Morpeh;
using UnityEngine;

public class CombatSystem : ISystem
{
    public World World { get; set; }

    private Filter _combatFilter;
    private Stash<PositionComponent> _posStash;
    private Stash<AttackStatsComponent> _attackStash;
    private Stash<AttackCooldownComponent> _cdStash;
    private Stash<HealthComponent> _healthStash;
    private Stash<TeamComponent> _teamStash;
    private Stash<ViewComponent> _viewStash;
    private Stash<UnitAudioComponent> _audioStash;
    private Stash<DeadComponent> _deadStash;
    public void OnAwake()
    {
        _posStash = World.GetStash<PositionComponent>();
        _attackStash = World.GetStash<AttackStatsComponent>();
        _cdStash = World.GetStash<AttackCooldownComponent>();
        _healthStash = World.GetStash<HealthComponent>();
        _teamStash = World.GetStash<TeamComponent>();
        _viewStash = World.GetStash<ViewComponent>();
        _audioStash = World.GetStash<UnitAudioComponent>();
        _deadStash = World.GetStash<DeadComponent>();

        _combatFilter = World.Filter
            .With<UnitComponent>()
            .With<PositionComponent>()
            .With<AttackStatsComponent>()
            .With<AttackCooldownComponent>()
            .Without<DeadComponent>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _combatFilter)
        {
            ref var cd = ref _cdStash.Get(entity);
            if (cd.timer > 0)
                cd.timer -= deltaTime;

            ref var pos = ref _posStash.Get(entity);
            ref var team = ref _teamStash.Get(entity);
            ref var attack = ref _attackStash.Get(entity);
            ref var view = ref _viewStash.Get(entity);
            ref var audio = ref _audioStash.Get(entity);

            Entity target = default;
            float minSqrDist = attack.range * attack.range;
            bool targetFound = false;

            foreach (var other in _combatFilter)
            {
                ref var otherTeam = ref _teamStash.Get(other);
                if (otherTeam.id == team.id) continue;

                ref var otherPos = ref _posStash.Get(other);
                Vector3 delta = otherPos.value - pos.value;
                delta.y = 0;
                float sqrDist = delta.sqrMagnitude;

                if (sqrDist <= minSqrDist)
                {
                    minSqrDist = sqrDist;
                    target = other;
                    targetFound = true;
                }
            }

            if (targetFound && cd.timer <= 0)
            {
                cd.timer = attack.cooldown;

                ref var targetHealth = ref _healthStash.Get(target);
                ref var targetView = ref _viewStash.Get(target);

                targetHealth.current = Mathf.Max(0, targetHealth.current - attack.damage);

                view.view.PlayAttack(audio.attackSound);
                targetView.view.UpdateHealth(targetHealth.current, targetHealth.max);

                if (targetHealth.current <= 0)
                {
                    _deadStash.Set(target, new DeadComponent());
                    targetView.view.Die();
                }
            }
        }
    }

    public void Dispose() { }
}