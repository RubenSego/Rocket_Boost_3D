using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class EggUI : MonoBehaviour
{

    [SerializeField] private string idleStateName = "Egg_Idle";
    
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
        animator.ResetTrigger(CrackTrigger);

        if (eggImage != null)
        {
            eggImage.enabled = true;
        }

        if (animator != null && animator.HasState(0, Animator.StringToHash(idleStateName)))
        {
            animator.Play(idleStateName, 0, 0f);
            animator.Update(0f);
        }
        else if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
    }
}
