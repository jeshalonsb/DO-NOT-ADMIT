using UnityEngine;
using UnityEngine.InputSystem;

public class SubmissionInfoManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject namePanel;
    [SerializeField] private GameObject objectivesPanel;

    private void Start()
    {
        if (namePanel != null)
            namePanel.SetActive(false);

        if (objectivesPanel != null)
            objectivesPanel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            ToggleNamePanel();
        }

        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            ToggleObjectivesPanel();
        }
    }

    private void ToggleNamePanel()
    {
        if (namePanel == null)
            return;

        namePanel.SetActive(!namePanel.activeSelf);
    }

    private void ToggleObjectivesPanel()
    {
        if (objectivesPanel == null)
            return;

        objectivesPanel.SetActive(!objectivesPanel.activeSelf);
    }
}