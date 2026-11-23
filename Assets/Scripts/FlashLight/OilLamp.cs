using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class OilLamp : MonoBehaviour
{

    [Header("Oil Lamp Settings")]
    [SerializeField] private Light Oillight;
    [SerializeField] private InputActionReference Action;

    [Header("Energy Settings")]
    [SerializeField] private float MaxOil = 100f;
    [SerializeField] private float OilUsageRate = 1f;
    [SerializeField] private float LowOil = 20f;
    [SerializeField] private float BackOil = 5f;
    [SerializeField] private bool FullOil = true;
    [SerializeField] private bool UnlimitedOil = false;


    [Header("Flicker Settings")]
    [SerializeField] private float MinDelay = 0.05f;
    [SerializeField] private float MaxDelay = 0.2f;
    [SerializeField] private float MinBrightness = 0.1f;
    [SerializeField] private float FlickerTime = 1.5f;
    [SerializeField] private float CurrentOil;
    private bool IsOn;
    private bool IsFlickering;
    private float NormalBrightness;
    private Coroutine FlickerCoroutine; //Coroutine is a method that can suspend execution and resume at a later time

    private void Awake()
    {
        if (Oillight == null)
        {
            Oillight = GetComponentInChildren<Light>();
        }

        if (Oillight != null)
        {
            NormalBrightness = Oillight.intensity;
        }

        if (FullOil == true)
        {
            CurrentOil = MaxOil;
        }

        SetOilLamp(false);
    }

    private void OnEnable()
    {
        if (Action != null)
        {
            Action.action.Enable();
            Action.action.performed += OilLamptInput;
        }
    }

    private void OnDisable()
    {
        if (Action != null)
        {
            Action.action.performed -= OilLamptInput;
            Action.action.Disable();
        }
    }

    private void OilLamptInput(InputAction.CallbackContext obj)
    {
        OilLampSwitch();
    }

    private void Update()
    {
        if (IsOn && !UnlimitedOil)
        {
            DrainOil();
        }
    }

    public void OilLampSwitch()
    {
        if (CurrentOil <= 0 && !IsOn)
        {
            return;
        }

        IsOn = !IsOn;

        if (IsOn)
        {
            SetOilLamp(true);
        }
        else
        {
            SetOilLamp(false);
            StopFlicker();
        }
    }

    private void SetOilLamp(bool state)
    {
        if (Oillight != null)
        {
            Oillight.enabled = state;
            Oillight.intensity = NormalBrightness;
        }
    }

    private void DrainOil()
    {
        CurrentOil -= OilUsageRate * Time.deltaTime;
        CurrentOil = Mathf.Clamp(CurrentOil, 0f, MaxOil);

        if (CurrentOil <= LowOil && Oillight != null)
        {
            float OilPercent = CurrentOil / LowOil;
            Oillight.intensity = Mathf.Lerp(NormalBrightness * 0.3f, NormalBrightness, OilPercent);
        }

        if (CurrentOil <= 0)
        {
            TurnOff();
        }
    }

    private void StartFlicker()
    {
        if (FlickerCoroutine == null)
        {
            FlickerCoroutine = StartCoroutine(FlameFlicker());
        }
    }
    
    private void StopFlicker()
    {
        if (FlickerCoroutine != null)
        {
            StopCoroutine(FlickerCoroutine);
            FlickerCoroutine = null;
        }

        if (Oillight != null)
        {
            Oillight.intensity = NormalBrightness;
        }
    }

    private IEnumerator FlameFlicker()
    {
        while (IsOn && CurrentOil > 0f)
        {
            float FlickerAmount = Random.Range(MinBrightness, NormalBrightness);

            if (CurrentOil <= BackOil)
            {
                FlickerAmount = LowOil * Random.Range(0.1f, 0.3f);
            }

            float OilPercent = CurrentOil / MaxOil;

            float Brightness = NormalBrightness * OilPercent;

            float RandomBrightness = Random.Range(MinBrightness, Brightness);

            Oillight.intensity = Mathf.Max(MinBrightness, Brightness + RandomBrightness);
        }

        yield return new WaitForSeconds(Random.Range(MinDelay, MaxDelay));

        FlickerCoroutine = null;

    }

    private void TurnOff()
    {
        IsOn = false;
        SetOilLamp(false);
        StopFlicker();
    }

    public void RefillOil(float amount)
    {
        CurrentOil = Mathf.Clamp(CurrentOil + amount, 0f, MaxOil);
    }

    public float GetCurrentOil()
    {
        return CurrentOil;
    }

    public float GetMaxOil()
    {
        return MaxOil;
    }

    public float GetPercentOil()
    {
        return CurrentOil / MaxOil;
    }
}
