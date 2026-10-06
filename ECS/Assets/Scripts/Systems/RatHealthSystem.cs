using Unity.Entities;

public partial class RatHealthSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (health, entity) in SystemAPI.Query<RefRO<RatHealth>>().WithEntityAccess())
        {
            if (health.ValueRO.CurrentHealth <= 0)
            {
                ecb.DestroyEntity(entity);
            }
        }
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}