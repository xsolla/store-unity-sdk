using System;
using UnityEngine.EventSystems;

public interface ISimpleButton :
	IPointerDownHandler,
	IPointerEnterHandler,
	IPointerExitHandler,
	IPointerUpHandler,
	IDragHandler
{
}
