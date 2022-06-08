using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class CardInDeck : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    [FormerlySerializedAs("yourScrollRect")] public ScrollRect scrollRect;
    private bool passingEvent = false;

    public Image backImage;
    public Rarity rarity;
    public CardCharacter CardCharacter { get; set; }

    public Color Color
    {
        set => backImage.color = value;
    }

    public TMP_Text nameField;

    public string Name
    {
        get => nameField.text;
        set => nameField.text = value;
    }

    public void Click()
    {
        AudioStatic.PlayAudio("Sounds/card");
        switch (Account.CurrentScene)
        {
            case Scenes.Collection:
                var collection = FindObjectOfType<Collection>();
                collection.MoveCardToCollection(this);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ChangeSize(bool enlarging) =>
        transform.localScale = enlarging
            ? new Vector3(1, 1, 1) * 1.25f
            : new Vector3(1, 1, 1);

    public void OnBeginDrag(PointerEventData eventData)
    {
        ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.beginDragHandler);
        passingEvent = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (
            passingEvent) // Don't send dragHandler before beginDragHandler has been called. It gives unwanted results...
        {
            print("dragging");
            ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.dragHandler);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.endDragHandler);
        passingEvent = false;
    }

    public void OnScroll(PointerEventData eventData)
    {
        ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.scrollHandler);
    }
}