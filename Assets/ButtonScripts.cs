using UnityEngine.EventSystems;
using UnityEngine;

public class ButtonScripts : MonoBehaviour, IPointerEnterHandler
{
     public void OnPointerEnter(PointerEventData eventData)
     {
         AudioManager.GetInstance().PlaySound("UIClick");
     }

     public void PlayButtonPressedSound()
     {
        AudioManager.GetInstance().PlaySound("ButtonClicked");
     }
}
