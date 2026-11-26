using UnityEngine;
using Scellecs.Morpeh;

public class BattleBootstrap : MonoBehaviour
{
    [SerializeField] private UnitView unitPrefab; // Твой префаб
    [SerializeField] private Sprite undeadSprite;
    [SerializeField] private Sprite humanSprite;
    [SerializeField] private AudioClip undeadSound;
    [SerializeField] private AudioClip humanSound;

    private World _world;
    private SystemsGroup _systemsGroup;

    private void Start()
    {
        _world = World.Default;

        _systemsGroup = _world.CreateSystemsGroup();
        _systemsGroup.AddSystem(new TargetAndMoveSystem());
        _systemsGroup.AddSystem(new MovementSystem());
        _systemsGroup.AddSystem(new CombatSystem());
        _systemsGroup.AddSystem(new ViewSyncSystem());
        _systemsGroup.Initialize();

        SpawnArmy(teamId: 0, count: 10, startX: -6f, undeadSprite, undeadSound);
        SpawnArmy(teamId: 1, count: 10, startX: 6f, humanSprite, humanSound);
    }

    private void Update()
    {
        _systemsGroup.Update(Time.deltaTime);
    }

    private void SpawnArmy(int teamId, int count, float startX, Sprite sprite, AudioClip sound)
    {
        var unitStash = _world.GetStash<UnitComponent>();
        var teamStash = _world.GetStash<TeamComponent>();
        var posStash = _world.GetStash<PositionComponent>();
        var velStash = _world.GetStash<VelocityComponent>();
        var radiusStash = _world.GetStash<RadiusComponent>();
        var speedStash = _world.GetStash<MoveSpeedComponent>();
        var attackStash = _world.GetStash<AttackStatsComponent>();
        var cdStash = _world.GetStash<AttackCooldownComponent>();
        var healthStash = _world.GetStash<HealthComponent>();
        var viewStash = _world.GetStash<ViewComponent>();
        var audioStash = _world.GetStash<UnitAudioComponent>();

        for (int i = 0; i < count; i++)
        {
            var entity = _world.CreateEntity();

            Vector3 spawnPos = new Vector3(
                startX + Random.Range(-0.5f, 0.5f),
                0,
                Random.Range(-3f, 3f)
            );

            UnitView view = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
            view.SetSprite(sprite);
            view.SetFacing(teamId == 1);

            unitStash.Set(entity, new UnitComponent());
            teamStash.Set(entity, new TeamComponent { id = teamId });
            posStash.Set(entity, new PositionComponent { value = spawnPos });
            velStash.Set(entity, new VelocityComponent { value = Vector3.zero });
            radiusStash.Set(entity, new RadiusComponent { value = 0.4f });
            speedStash.Set(entity, new MoveSpeedComponent { value = Random.Range(2f, 3f) });
            attackStash.Set(entity, new AttackStatsComponent { range = 0.9f, damage = 20f, cooldown = 1f });
            cdStash.Set(entity, new AttackCooldownComponent { timer = Random.Range(0f, 0.5f) });
            healthStash.Set(entity, new HealthComponent { current = 100f, max = 100f });
            viewStash.Set(entity, new ViewComponent { view = view });
            audioStash.Set(entity, new UnitAudioComponent { attackSound = sound });
        }
    }

    private void OnDestroy()
    {
        _systemsGroup?.Dispose();
    }
}