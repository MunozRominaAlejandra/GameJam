using UnityEngine;

public class ReceptorEventosAnimacion : MonoBehaviour
{
    // El Animator del Starter Assets llama a estas funciones. 
    // Al dejarlas vacías, atrapamos el evento y silenciamos el error sin gastar recursos.

    public void OnFootstep(AnimationEvent animationEvent) { }

    public void OnLand(AnimationEvent animationEvent) { }
}
