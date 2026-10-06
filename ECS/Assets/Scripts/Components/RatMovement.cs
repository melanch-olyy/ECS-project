using Unity.Entities;
using Unity.Mathematics;

public struct RatMovement : IComponentData 
{
    public float MoveSpeed;
    public float Stamina; 
    public float MaxStamina;
    public float StaminaRegen; 
    public float3 TargetPosition; 
}