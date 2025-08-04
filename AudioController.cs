using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace HyperTools
{
    public class AudioController : MonoBehaviour
    {
        [FormerlySerializedAs("MusicAudioSource")] [SerializeField] private AudioSource musicAudioSource;
        [FormerlySerializedAs("SfxAudioSource")] [SerializeField] private AudioSource sfxAudioSource;

        private bool _isApplicationPaused;

        private void OnApplicationPause(bool pauseStatus)
        {
            _isApplicationPaused = pauseStatus;

            if (pauseStatus)
            {
                musicAudioSource.Pause();
            }
            else
            {
                musicAudioSource.UnPause();
            }
        }

        public void PlayMusic(AudioClip clip, float volume = 1.0f, bool loop = true)
        {
            if (!musicAudioSource.isPlaying)
            {
                musicAudioSource.clip = clip;
                musicAudioSource.loop = loop;
                musicAudioSource.volume = volume;
                musicAudioSource.Play();
            }
            else
            {
                StartCoroutine(CrossfadeRoutine(clip, volume, 0.5f));
            }
        }

        private IEnumerator CrossfadeRoutine(AudioClip newClip, float newClipVolume, float crossfadeTime)
        {
            float time = 0;

            var oldAudioSource = musicAudioSource;
            var oldAudioVolume = oldAudioSource.volume;
            var newAudioSource = gameObject.AddComponent<AudioSource>();
            newAudioSource.playOnAwake = false;
            newAudioSource.clip = newClip;
            newAudioSource.loop = true;
            newAudioSource.volume = 0;
            newAudioSource.Play();
            musicAudioSource = newAudioSource;

            yield return null;

            while (time < crossfadeTime)
            {
                if (_isApplicationPaused)
                {
                    oldAudioSource.Stop();
                    yield return null;
                    continue;
                }

                time += Time.deltaTime;

                float normalizedTime = time / crossfadeTime;

                if (oldAudioSource != null)
                {
                    oldAudioSource.volume = Mathf.Lerp(oldAudioVolume, 0.0f, normalizedTime);
                }
                if (newAudioSource != null)
                {
                    newAudioSource.volume = Mathf.Lerp(0.0f, newClipVolume, normalizedTime);
                }

                yield return null;
            }

            newAudioSource.volume = newClipVolume;

            oldAudioSource.volume = 0.0f;
            oldAudioSource.Stop();

            yield return null;

            Destroy(oldAudioSource);
        }

        public void PlaySfx(AudioClip clip, float volume = 1.0f, float pitch = 1.0f, float pitchVariation = 0.0f)
        {
            if (pitchVariation > 0)
            {
                sfxAudioSource.pitch = Utilities.GetNormalRandom(pitch, pitchVariation);
            }
            else
            {
                sfxAudioSource.pitch = pitch;
            }

            sfxAudioSource.PlayOneShot(clip, volume);
        }
    }
}
