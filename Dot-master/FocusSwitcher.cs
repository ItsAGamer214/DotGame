using UnityEngine;

public class FocusSwitcher : MonoBehaviour
{
    public string FocusedLayer = "Focused";

    private GameObject currentlyFocused;
    private int previousLayer;

    public void SetFocused(GameObject obj)
    {
        // enables this camera and the postProcessingVolume which is the child
        gameObject.SetActive(true);

        // if something else was focused before reset it
        if (currentlyFocused) currentlyFocused.layer = previousLayer;

        // store and focus the new object
        currentlyFocused = obj;

        if (currentlyFocused)
        {
            previousLayer = currentlyFocused.layer;
            currentlyFocused.layer = LayerMask.NameToLayer(FocusedLayer);
        }
        else
        {
            Debug.Log("No object to focus");
            // if no object is focused disable the FocusCamera
            // and PostProcessingVolume for not wasting rendering resources
            gameObject.SetActive(false);
        }
    }

    // On disable make sure to reset the current object
    private void OnDisable()
    {
        if (currentlyFocused) currentlyFocused.layer =previousLayer;

        currentlyFocused = null;
    }
}