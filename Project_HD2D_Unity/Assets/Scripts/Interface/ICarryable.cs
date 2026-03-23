using UnityEngine;

namespace Interface
{
    public interface ICarryable
    {
        void Carry(Transform playerHead);
        
        bool IsCarryable();
        
        void Eject();
    }
}