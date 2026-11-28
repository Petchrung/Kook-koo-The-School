
using System.Collections;
using UnityEngine;

public class JumpScare : MonoBehaviour
{
    public GameObject JumpScareimg;
    public AudioSource audioSource;

    private bool isPlayed = false;

    void Start()
    {
        JumpScareimg.SetActive(false);
        Debug.Log("JumpScare Ready");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPlayed)
        {
            isPlayed = true;

            JumpScareimg.SetActive(true);

            if (audioSource != null)
                audioSource.Play();

            StartCoroutine(DisableImg());
        }
    }

    IEnumerator DisableImg()
    {
        Debug.Log("Start Timer");

        yield return new WaitForSecondsRealtime(2f);

        JumpScareimg.SetActive(false);

        Debug.Log("Image Hidden");
    }
}
