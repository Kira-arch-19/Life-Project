using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class FloatingMovementStick : OnScreenControl,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    // L'immagine della base del joystick.
    [SerializeField] private RectTransform joystickBase;

    // La levetta, figlia della base.
    [SerializeField] private RectTransform handle;

    // Distanza massima percorribile dalla levetta,
    // espressa nelle unità del Canvas.
    [SerializeField, Min(1f)] private float movementRange = 80f;

    // Invia l'input allo stick virtuale già collegato all'azione Move.
    [InputControl(layout = "Vector2")]
    [SerializeField] private string inputPath = "<Gamepad>/leftStick";

    protected override string controlPathInternal
    {
        get => inputPath;
        set => inputPath = value;
    }

    private RectTransform touchArea;
    private Vector3 initialPosition;
    private Vector2 touchOrigin;

    private bool isDragging;
    private int activePointerId;

    private void Start()
    {
        touchArea = GetComponent<RectTransform>();

        // Memorizza dove il joystick deve tornare quando rilasci il dito.
        initialPosition = joystickBase.localPosition;
        handle.anchoredPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Accetta un solo dito alla volta e il pulsante sinistro del mouse.
        if (isDragging || eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                touchArea,
                eventData.position,
                eventData.pressEventCamera,
                out touchOrigin))
            return;

        isDragging = true;
        activePointerId = eventData.pointerId;
// Mostra il joystick quando inizia un nuovo tocco.
joystickBase.gameObject.SetActive(true);
        // La base compare nel punto toccato.
        // Deve essere figlia diretta di TouchArea.
        joystickBase.localPosition = new Vector3(
            touchOrigin.x,
            touchOrigin.y,
            initialPosition.z);

        handle.anchoredPosition = Vector2.zero;
        SendValueToControl(Vector2.zero);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Ignora altri tocchi, per esempio quelli sui pulsanti di attacco.
        if (!isDragging || eventData.pointerId != activePointerId)
            return;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                touchArea,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 currentPosition))
            return;

        // Calcola lo spostamento rispetto al punto iniziale.
        Vector2 offset = currentPosition - touchOrigin;

        // Mantiene la levetta entro il raggio massimo.
        Vector2 limitedOffset = Vector2.ClampMagnitude(
            offset,
            movementRange);

        handle.anchoredPosition = limitedOffset;

        // Converte lo spostamento in un input con intensità da 0 a 1.
        SendValueToControl(limitedOffset / movementRange);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging || eventData.pointerId != activePointerId)
            return;

        ResetStick();
    }

    private void ResetStick()
{
    isDragging = false;

    // Ferma il personaggio.
    SendValueToControl(Vector2.zero);

    // Ripristina la posizione della levetta e della base.
    handle.anchoredPosition = Vector2.zero;
    joystickBase.localPosition = initialPosition;

    // Nasconde la grafica fino al prossimo tocco.
    joystickBase.gameObject.SetActive(false);
}

    private void OnApplicationFocus(bool hasFocus)
    {
        // Evita un movimento bloccato quando cambi finestra.
        if (!hasFocus && isDragging)
            ResetStick();
    }

    private void OnApplicationPause(bool isPaused)
    {
        // Ferma il movimento quando il telefono mette il gioco in pausa.
        if (isPaused && isDragging)
            ResetStick();
    }
}