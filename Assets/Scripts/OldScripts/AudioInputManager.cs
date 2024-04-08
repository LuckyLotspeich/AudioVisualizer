using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class AudioInputManager : MonoBehaviour
{
    [Header("Core Audio Variables")]
    AudioSource audioSource;
    public static int fftSize = 512;
    public int numBands = 8;
    public static float[] samples;
    public static float[] freqBand;
    public static float[] bandBuffer;
    private float[] bufferDecrease;
    
    // public float minFrequency = 20f;
    // public float maxFrequency = 20000f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        samples = new float[fftSize];
        freqBand = new float[numBands];
        bandBuffer = new float[numBands];
        bufferDecrease = new float[numBands];
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
    }

    void GetSpectrumAudioSource() {
        // Can use Blackman, Hanning, Hamming, Triangle, Rectangular, BlackmanHarris
        audioSource.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);
    }

    void MakeFrequencyBands()
        {
            int count = 0;
            for (int i = 0; i < numBands; i++)
            {
                int sampleCount = (int)Mathf.Pow(2, i) * 2;
                if (i == numBands - 1)
                {
                    sampleCount += 2;
                }

            float average = 0;
            for (int j = 0; j < sampleCount; j++)
            {
                average += samples[count] * (count + 1); // Adjust for proper indexing
                count++;
            }

            average /= count;
            freqBand[i] = average * 10; // Scale the value if needed
        }
    }

    void BandBuffer() {
        for (int i = 0; i < numBands; i++) {
            // if (freqBand[i] > bandBuffer[i]) {
            //     bandBuffer[i] = freqBand[i];
            //     bufferDecrease[i] = .005f * Time.deltaTime;
            // }
            // else if (freqBand[i] < bandBuffer[i]) {
            //     bandBuffer[i] -= bufferDecrease[i];
            //     bufferDecrease[i] *= 1.2f;

                // bufferDecrease[i] = (bandBuffer[i] - freqBand[i]) / numBands;
                // bandBuffer[i] -= bufferDecrease[i];
            // }

            if (freqBand[i] > bandBuffer[i]) {
                bandBuffer[i] = freqBand[i];
                bufferDecrease[i] = Mathf.Abs(bandBuffer[i] - freqBand[i]) * Time.deltaTime;
            } 
            else {
                bufferDecrease[i] = Mathf.Abs(bandBuffer[i] - freqBand[i]) * Time.deltaTime;
                bandBuffer[i] -= bufferDecrease[i];
            }
        }
    }         
} 


