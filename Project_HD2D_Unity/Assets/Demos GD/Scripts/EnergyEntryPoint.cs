using UnityEngine;
using UnityEngine.EventSystems;

namespace Demos_GD.Scripts
{
    public class EnergyEntryPoint : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private RootLinker rootLinker;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("FAAAH");
            
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    rootLinker.ChangeEnergyLevel(1);
                    break;
                
                case PointerEventData.InputButton.Right:
                    rootLinker.ChangeEnergyLevel(-1);
                    break;
            }
        }
    }
}
