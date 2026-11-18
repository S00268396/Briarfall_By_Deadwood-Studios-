using UnityEngine;

public class SkinwalkerMimic : MonoBehaviour
{
    //Pitch value that will be used to modify the audio source pitch based on the distance between the player and the audio source.
    public float pitch = 1.0f;
    public AudioClip audioClip; //The audio clip that will be played by the audio source.

    private AudioSource audioSource; //The audio source component that will be used to play the audio clip.
    private float low = 0.75f; //The minimum pitch value that will be used when the player is far away from the audio source.
    private float high = 1.25f; //The maximum pitch value that will be used when the player is close to the audio source.

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); //Get the audio source component attached to the game object.
        audioSource.clip = audioClip; //Set the audio clip to be played by the audio source.
        audioSource.loop = true; //Set the audio source to loop the audio clip.

        audioSource.Play(); //Start playing the audio clip.
    }

    private void Update()
    {
        //Calculate the distance between the player and the audio source.
        float PlayerDistance = Vector3.Distance(transform.position, Camera.main.transform.position);

        //Use Mathf.InverseLerp to get a value between 0 and 1 based on the distance between the player and the audio source.
        float pitchValue = Mathf.InverseLerp(10, 0, PlayerDistance);

        //Set the pitch of the audio source based on the distance between the player and the audio source.
        audioSource.pitch = Mathf.Lerp(low, high, pitchValue); 
    }


    //I used this code that I find during my research to get an base idea on what to do and then I modified it to fit my needs.
    //public float pitchValue = 1.0f;
    //public AudioClip mySong;

    //private AudioSource audioSource;
    //private float low = 0.75f;
    //private float high = 1.25f;

    //void Awake()
    //{
    //    audioSource = GetComponent<AudioSource>();
    //    audioSource.clip = mySong;
    //    audioSource.loop = true;
    //}

    //void Update()
    //{
    //    float angle = Quaternion.Angle(Quaternion.Euler(new Vector3(0, 0, 0)), transform.rotation);
    //    float pitchValue = (angle / 180f) * (high - low) + low;
    //    audioSource.pitch = pitchValue;
    //}
}
