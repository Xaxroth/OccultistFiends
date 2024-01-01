using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullPillarScript : MonoBehaviour
{
    public bool Destroyed = false;

    public List<SkullScript> allSkulls = new List<SkullScript>();

    void Start()
    {
        Destroyed = false;

        for (int i = 0; i < allSkulls.Count; i++)
        {
            allSkulls[i].gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public void DestroyPillar()
    {
        AudioManager.Instance.PlaySound("DestroyPillar", 0, 1, 0);

        for (int i = 0; i < allSkulls.Count; i++)
        {
            allSkulls[i].gameObject.GetComponent<Rigidbody>().isKinematic = false;
            allSkulls[i]._skullTrail.Stop();
        }
    }

    public void RestorePillar()
    {
        AudioManager.Instance.PlaySound("RestorePillar", 0, 1, 0);

        for (int i = 0; i < allSkulls.Count; i++)
        {
            allSkulls[i]._skullTrail.Play();
            allSkulls[i].gameObject.GetComponent<Rigidbody>().isKinematic = true;
            allSkulls[i].transform.rotation = allSkulls[i].StartRotation;
            StartCoroutine(LerpSkull(allSkulls[i], 0.3f));
        }
    }

    private IEnumerator LerpSkull(SkullScript skull, float acceleration)
    {
        float currentSpeed = 0;

        Vector3 currentPosition = skull.transform.position;
        Vector3 targetPosition = skull.StartPosition;

        while (currentPosition != targetPosition)
        {
            currentPosition = Vector3.MoveTowards(currentPosition, targetPosition, currentSpeed);
            skull.transform.position = currentPosition;
            currentSpeed += Time.deltaTime * acceleration;
            yield return null;
        }

    }
}
