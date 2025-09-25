using UnityEngine;

public class BlinkEvent : MonoBehaviour
{
    public enum BlinkEvents { None, Die, HalfBrightness, Disconnect }

    public void Blink()
    {
        BlinkEvents evt = (BlinkEvents)Random.Range(0, 3); // pick a random event
        switch (evt)
        {
            case BlinkEvents.Die:
                //StartCoroutine(FadeAndDestroy());
                break;

            case BlinkEvents.HalfBrightness:
                //brightness *= 0.5f;
                break;

            case BlinkEvents.Disconnect:
                // clear its connected stars
                // connectedStars.Clear();
                break;
        }
    }
}
