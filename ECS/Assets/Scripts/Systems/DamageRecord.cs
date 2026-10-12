using Unity.Entities;

public struct DamageRecord : IBufferElementData
{
    public float DamageAmount;
    public float TimeReceived; //время получения урона
}