using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class EggUI : MonoBehaviour
{
    private Animator animator;
    private Image eggImage;
    private static readonly int CrackTrigger = Animator.StringToHash("Crack");

    void Awake()
    {
        animator = GetComponent<Animator>();
        eggImage = GetComponent<Image>();
    }

    public void BreakEgg()
    {
        animator.SetTrigger(CrackTrigger);
        StartCoroutine(HideAfterAnimation());
    }

    private IEnumerator HideAfterAnimation()
    {
        // Espera un frame para asegurar que el Animator entra al estado de rotura
        yield return null; 
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);

        eggImage.enabled = false;
    }

    public void RestoreEgg()
    {
        StopAllCoroutines();
        eggImage.enabled = true;
        animator.Play("Egg_Idle", 0, 0f);
    }
}
