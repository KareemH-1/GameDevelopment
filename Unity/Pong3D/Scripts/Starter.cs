using UnityEngine;

public class Starter : MonoBehaviour
{
    private GameController gameController;
    private Animator animator;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        animator = GetComponent<Animator>();

        ResetAnimatorState();
    }

    public void StartCountdown()
    {
        animator.SetTrigger("StartCountdown");
    }

    public void StartGame()
    {
        gameController.StartGame();
    }

    public void ResetAnimatorState()
    {
        if (animator != null)
        {
            animator.ResetTrigger("StartCountdown");
        }
    }
}