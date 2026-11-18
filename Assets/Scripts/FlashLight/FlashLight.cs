using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class FlashLight : MonoBehaviour
{
   [Header("FlashLight Settings")]
   [SerializeField] private Light Flashlight;
   [SerializeField] private InputActionReference Action;

    [Header("Energy Settings")]
    [SerializeField] private float MaxPower = 100f;
    [SerializeField] private float PowerUsageRate = 1f;
    [SerializeField] private float LowPower = 20f;
    [SerializeField] private float BackPower = 5f;
    [SerializeField] private bool FullPower = true;
    [SerializeField] private bool UnlimitedPower = false;


    [Header("Flicker Settings")]
    [SerializeField] private float MinDelay = 0.05f;
    [SerializeField] private float MaxDelay = 0.2f;
    [SerializeField] private float MinBrightness = 0.1f;
    [SerializeField] private float FlickerTime = 1.5f;
    [SerializeField] private float CurrentPower;

    private bool IsOn;
    private bool IsFlickering;
    private float NormalBrightness;
    private Coroutine FlickerCoroutine; //Coroutine is a method that can suspend execution and resume at a later time

    private void Awake()
    {
        if (Flashlight == null)
        {
            Flashlight = GetComponentInChildren<Light>();
        }

        if (Flashlight != null)
        {
            NormalBrightness = Flashlight.intensity;
        }

        if (FullPower == true)
        {
            CurrentPower = MaxPower;
        }

        SetFlashLight(false);
    }

    private void OnEnable()
    {
        if (Action != null)
        {
            Action.action.Enable();
            Action.action.performed += FlashlightInput;
        }
    }

    private void OnDisable()
    {
        if (Action != null)
        {
            Action.action.performed -= FlashlightInput;
            Action.action.Disable();
        }
    }

    private void FlashlightInput(InputAction.CallbackContext obj)
    {
       FlashLightSwitch();
    }

    private void Update()
    {
        if (IsOn && !UnlimitedPower)
        {
            DrainEnergy();
            CheckFlicker();
        }
    }

    public void FlashLightSwitch()
    {
        if (CurrentPower <= 0 && !IsOn)
        {
            return;
        }

        IsOn = !IsOn;
        SetFlashLight(IsOn);

        if (!IsOn)
        {
            StopFlicker();
        }
    }

    private void SetFlashLight(bool state)
    {
        if (Flashlight != null)
        {
            Flashlight.enabled = state;
            Flashlight.intensity = NormalBrightness;
        }
    }

    private void DrainEnergy()
    {
        CurrentPower -= PowerUsageRate * Time.deltaTime;
        CurrentPower = Mathf.Clamp(CurrentPower, 0f, MaxPower);

        if (CurrentPower <= LowPower && Flashlight != null)
        {
            float PowerPercent = CurrentPower / LowPower;

            Flashlight.intensity = Mathf.Lerp(NormalBrightness * 0.3f, NormalBrightness, PowerPercent);
        }

        if (CurrentPower <= 0)
        {
            TurnOff();
        }
    }

    private void CheckFlicker()
    {
        if (IsFlickering || Flashlight == null)
        {
            return;
        }

        if (CurrentPower <= BackPower)
        {
            StartFlicking(AlmostEmptyFlicker());
        }
        else if (CurrentPower <= LowPower && Random.value < 0.01f)
        {
            StartFlicking(LowEnergyFlicker());
        }
    }

    private void StartFlicking(IEnumerator flicker)
    {
        if (FlickerCoroutine != null)
        {
            StopCoroutine(FlickerCoroutine);
        }

        FlickerCoroutine = StartCoroutine(flicker);
    }

    private void StopFlicker()
    {
        if (FlickerCoroutine != null)
        {
            StopCoroutine(FlickerCoroutine);
            FlickerCoroutine = null;
        }

        IsFlickering = false;
    }

    private IEnumerator LowEnergyFlicker()
    {
        IsFlickering = true;
        float Timer = 0f;

        while (Timer < FlickerTime / 2f)
        {
            Flashlight.enabled = !Flashlight.enabled;

            float Delay = Random.Range(MinDelay, MaxDelay);
            yield return new WaitForSeconds(Delay);

            Timer += Delay;
        }

        SetFlashLight(IsOn);
        IsFlickering = false;
        FlickerCoroutine = null;
    }

    private IEnumerator AlmostEmptyFlicker()
    {
        IsFlickering = true;
        float Timer = 0f;

        while (Timer < FlickerTime && CurrentPower > 0f)
        {
            Flashlight.enabled = !Flashlight.enabled;

            if (Flashlight.enabled)
            {
                Flashlight.intensity = Random.Range(MinBrightness, NormalBrightness * 0.5f);
            }

            float Delay = Random.Range(MinDelay / 2f, MaxDelay / 2f);
            yield return new WaitForSeconds(Delay);

            Timer += Delay;
        }

       SetFlashLight(CurrentPower > 0f && IsOn);
        IsFlickering = false;
        FlickerCoroutine = null;

    }

    private void TurnOff()
    {
        IsOn = false;
        SetFlashLight(false);
        StopFlicker();
    }

    public void AddEnergy(float Amount)
    {
        CurrentPower = Mathf.Clamp(CurrentPower + Amount, 0f, MaxPower);
    }

    public float GetMaxPower()
    {
        return MaxPower;
    }

    public float GetCurrentPower()
    {
        return CurrentPower;
    }

    public float GetPercentage()
    {
        return MaxPower > 0f ? CurrentPower / MaxPower: 0f;
    }

}
