using UnityEngine;
using UnityEngine.UI;

public class MobileInput : MonoBehaviour
{
    public static MobileInput Instance;
    
    public Joystick movementJoystick;
    public Button jumpButton;
    public Button punchButton;
    public Button kickButton;
    public Button specialButton;
    
    public Vector2 MovementDirection => new Vector2(
        movementJoystick?.Horizontal ?? 0,
        movementJoystick?.Vertical ?? 0
    );
    
    public bool JumpPressed => jumpButton != null && jumpButton.IsPressed();
    public bool PunchPressed => punchButton != null && punchButton.IsPressed();
    public bool KickPressed => kickButton != null && kickButton.IsPressed();
    public bool SpecialPressed => specialButton != null && specialButton.IsPressed();
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}