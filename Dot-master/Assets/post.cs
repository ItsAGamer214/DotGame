using UnityEngine;

public class post : MonoBehaviour
{
    [SerializeField] private FocusSwitcher  focus;

    void Start()
    {
        focus.SetFocused(gameObject);
    }


}