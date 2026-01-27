using UnityEngine;

public class Scene2PauseLockout : MonoBehaviour
{
    [Tooltip("ÍÏ PauseMenuController ½øÀ´")]
    public PauseMenuController pauseMenu;

    [Min(0f)] public float lockSeconds = 15f;

    private void Start()
    {
        if (pauseMenu == null) return;
        StartCoroutine(LockRoutine());
    }

    private System.Collections.IEnumerator LockRoutine()
    {
        bool prev = pauseMenu.enable;
        pauseMenu.enable = false;

        yield return new WaitForSeconds(lockSeconds);

        pauseMenu.enable = prev;
    }
}
