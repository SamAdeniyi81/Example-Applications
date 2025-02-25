using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class LaserPointerHandler : MonoBehaviour
{
    private SteamVR_LaserPointer laserPointer;
    private Button lastHoveredButton;
    private Button lastSelectedButton;


    void Start()
    {
        laserPointer = GetComponent<SteamVR_LaserPointer>();

        if (laserPointer == null)
        {
            //Debug.LogError("SteamVR_LaserPointer component is missing!");
            return;
        }

        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += OnPointerClick;
    }

    private void OnPointerClick(object sender, PointerEventArgs e)
    {
        Button button = e.target.GetComponent<Button>();

        if (button != null)
        {
            button.onClick.Invoke();
            //Debug.Log("Button clicked: " + button.name);

            // Ensure only the selected button changes color
            if (lastSelectedButton != null && lastSelectedButton != button)
            {
                ResetButtonColor(lastSelectedButton);
            }

            HighlightButton(button, "click");
            lastSelectedButton = button;
        }
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        Button button = e.target.GetComponent<Button>();
        if (button != null && button != lastHoveredButton)
        {
            HighlightButton(button, "hover");

            // Reset the last hovered button if it exists and is different
            if (lastHoveredButton != null && lastHoveredButton != button)
            {
                ResetButtonColor(lastHoveredButton);
            }

            lastHoveredButton = button;
        }
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        Button button = e.target.GetComponent<Button>();
        if (button != null && button == lastHoveredButton)
        {
            ResetButtonColor(button);
            lastHoveredButton = null;
        }
    }

    private void HighlightButton(Button button, string state)
    {
        if (button == null) return;

        Color targetColor = state == "hover" ? Color.green : Color.blue;
        Debug.Log(state == "hover" ? "Hovering over button: " + button.name : "Button selected: " + button.name);

        ColorBlock colors = button.colors;
        colors.normalColor = targetColor;
        button.colors = colors;
    }

    private void ResetButtonColor(Button button)
    {
        if (button == null) return;

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        button.colors = colors;
    }

    private void OnDestroy()
    {
        if (laserPointer != null)
        {
            laserPointer.PointerClick -= OnPointerClick;
            laserPointer.PointerIn -= PointerInside;
            laserPointer.PointerOut -= PointerOutside;
        }
    }
}
