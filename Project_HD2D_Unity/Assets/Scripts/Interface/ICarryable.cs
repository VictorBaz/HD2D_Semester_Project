using UnityEngine;

namespace Interface
{
    public interface ICarryable
    {
        void Carry(Transform playerHead);
        
        bool IsCarryable();
        
        void Eject(Vector3 force ,bool isEscaping = false);

        bool IsCarry();
    }
}