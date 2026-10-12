using Unity.Entities;
using Unity.Mathematics;

public class RatBaker : Baker<RatAuthoring>  
{
    public override void Bake(RatAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic); 

        AddComponent(entity, new RatHealth 
        { 
            CurrentHealth = authoring.StartHealth, 
            MaxHealth = authoring.StartHealth 
        });

        AddComponent(entity, new RatMovement 
        { 
            MoveSpeed = authoring.StartSpeed,
            Stamina = authoring.StartStamina,
            MaxStamina = authoring.StartStamina,
            StaminaRegen = authoring.StartStaminaRegen,
            TargetPosition = new float3(10f, 0f, 10f)
        });

        AddBuffer<DamageRecord>(entity);
    }
}