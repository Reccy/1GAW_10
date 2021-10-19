using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stove : MonoBehaviour
{
    private int m_stage = 0;

    private void Log(string s)
    {
        LevelManager.Instance.TextLog.AddLine(s);
    }

    public void OnUsed(Interactable interactable)
    {
        if (m_stage == 0)
        {
            Log("You turn on the stove");
            interactable.Finish();
        }
        else if (m_stage == 1)
        {
            Log("You cook some lobster");
            interactable.Finish();
        }
        else if (m_stage == 2)
        {
            Log("Yummy");
            interactable.Finish();
        }
        else if (m_stage == 3)
        {
            Log("Itchy. Tasty.");
            interactable.Finish();
        }
        else
        {
            Log("You decide you've had enough of the stove...");
            StartCoroutine(FinishMeCoroutine(interactable));
        }

        m_stage++;
    }

    private IEnumerator FinishMeCoroutine(Interactable interactable)
    {
        interactable.CanCancel = false;

        while (transform.localScale.x > 0.05f)
        {
            transform.localScale = new Vector3(transform.localScale.x - 0.02f * Time.deltaTime, transform.localScale.y - 0.02f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        Log("Goodbye, Stove......");

        interactable.Finish();
        Destroy(gameObject);
    }
}
