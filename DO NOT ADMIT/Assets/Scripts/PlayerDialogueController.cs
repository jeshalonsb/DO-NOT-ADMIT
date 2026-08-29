using System.Collections;
using UnityEngine;

public class PlayerDialogueController : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private VisitorDialogueUI dialogueUI;

    [Header("Opening")]
    [SerializeField] private float openingDelay = 1.5f;

    private void Start()
    {
        StartCoroutine(
            OpeningDialogueRoutine()
        );
    }

    private IEnumerator OpeningDialogueRoutine()
    {
        yield return new WaitForSeconds(
            openingDelay
        );

        Say(
            "First day... hope everything goes well."
        );
    }

    public void SayPowerOut()
    {
        Say(
            "Shit... the power's out."
        );
    }

    public void SayPartialPower()
    {
        Say(
            "Power's back... but something's still wrong."
        );
    }

    public void SayJumpscare()
    {
        Say(
            "AHH!"
        );
    }

    public void Say(
        string line)
    {
        if (dialogueUI == null)
            return;

        dialogueUI.ShowDialogue(
            "YOU",
            line
        );
    }
}