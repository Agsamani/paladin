using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class UndeadSoul : MonoBehaviour, IInteractable
{
    public float RiseDuration = 1.0f;
    public float Height = 1.0f;
    public AnimationCurve Curve;

    public Material OutlineMat;

    void Start()
    {
        StartCoroutine(RiseCoroutine());
        
    }

    private IEnumerator RiseCoroutine()
    {
        float startY = transform.position.y;
        float endY = startY + Height;
        float time = 0f;

        while (time < RiseDuration)
        {
            time += Time.deltaTime;
            float u = Mathf.Clamp01(time / RiseDuration);
            float a = Curve.Evaluate(u); 
            Vector3 p = transform.position;
            p.y = Mathf.LerpUnclamped(startY, endY, a);
            transform.position = p;
            yield return null;
        }

        Vector3 final = transform.position;
        final.y = endY;
        transform.position = final;
    }

    void Update()
    {
        
    }

    public void Interact(InteractionInfo intrc)
    {
        Debug.Log("Hehe");
    }

    public void OnRayHit(RaycastHit info)
    {
    }
    public void OnRayExit()
    {

    }

    public GameObject ReturnSelf()
    {
        return this.gameObject;
    }
}
