using UnityEngine;

namespace Interface
{
    public interface ICarryable
    {
        void Carry(Transform playerHead);
        
        bool CanCarry();
        
        void Eject();
    }
}