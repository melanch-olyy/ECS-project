using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class RatDamageSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        float currentTime = (float)SystemAPI.Time.ElapsedTime; 

        foreach (var (health, movement, transform, damageBuffer) in 
                 SystemAPI.Query<RefRW<RatHealth>, RefRW<RatMovement>, RefRO<LocalTransform>, DynamicBuffer<DamageRecord>>())
        {
            float3 toTarget = movement.ValueRO.TargetPosition - transform.ValueRO.Position;
            float distance = math.length(toTarget);
            
            if (distance < 0.5f)
            {
                float damage = 25f * deltaTime;
                health.ValueRW.CurrentHealth -= damage;
                
                damageBuffer.Add(new DamageRecord
                {
                    DamageAmount = damage,
                    TimeReceived = currentTime
                });
            }
        }
    }
}