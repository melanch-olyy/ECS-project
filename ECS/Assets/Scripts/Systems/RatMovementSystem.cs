using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class RatMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (movement, transform) in SystemAPI.Query<RefRW<RatMovement>, RefRW<LocalTransform>>())
        {
            float3 toTarget = movement.ValueRO.TargetPosition - transform.ValueRO.Position;
            float distance = math.length(toTarget);

            if (distance < 0.5f)
            {
                continue; 
            }

            if (movement.ValueRO.Stamina > 0)
            {
                float3 direction = math.normalize(toTarget);
                
                //заставляем локальную ось Z смотреть на цель, а Y наверх
                quaternion targetRotation = quaternion.LookRotationSafe(direction, math.up());
                
                //домножаем кватернион на поворот вокруг локальной оси X на -90 градусов
                //math.radians переводит градусы в радианы
                targetRotation = math.mul(targetRotation, quaternion.Euler(math.radians(-90f), 0, 90f));
                
                transform.ValueRW.Rotation = targetRotation;
                
                transform.ValueRW.Position += direction * movement.ValueRO.MoveSpeed * deltaTime;
                movement.ValueRW.Stamina -= 10f * deltaTime;
            }
            else
            {
                movement.ValueRW.Stamina += movement.ValueRO.StaminaRegen * deltaTime;
            }
        }
    }
}