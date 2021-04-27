using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoicelineHandler : MonoBehaviour
{
    bool isActive = true;

    AudioClip lastAudioClip;
    [SerializeField] AudioClip[] generalAudio;
    [SerializeField] AudioClip[] playerHasPassengersAudio;
    public int playerHasPassengerChance = 20;
    [SerializeField] AudioClip[] passengerDiedAudio;
    public int passengerDiedChance = 20;
    [SerializeField] AudioSource announcerSource;

    bool canPlayAudio = true;
    [SerializeField] float minDelayBetweenAnnouncer, maxDelayBetweenAnnouncer;
    [SerializeField] float minGeneralAudioDelay, maxGeneralAudioDelay;

    Coroutine announcerRoutine;


    [SerializeField] PlayerSpawner playerHandler;
    [SerializeField] NPCSpawner npcHandler;
    [SerializeField] MinigameHandler minigameHandler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitVoicelineHandler()
    {
        foreach(GameObject npc in npcHandler.allSpawnedNPCS)
        {
            Passenger thisPassenger = npc.GetComponent<Passenger>();

            if(thisPassenger != null)
            {
                thisPassenger.onDeath += OnPassengerDied;
                thisPassenger.onFollowPlayer += OnPassengerFollowed;
            }
        }
    }

    public void StartStopAnnouncer(bool start)
    {
        if (start)
        {
            isActive = true;
            if(announcerRoutine == null)
            {
                announcerRoutine = StartCoroutine(AnnouncerRoutine());
            }
        }
        else
        {
            isActive = false;
            if (announcerRoutine != null)
            {
                StopCoroutine(announcerRoutine);
                announcerRoutine = null;
            }
        }
    }

    IEnumerator AnnouncerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minGeneralAudioDelay, maxGeneralAudioDelay));
            if (canPlayAudio)
            {
                SelectAnnouncerAudio();
            }
        }
    }

    IEnumerator AudioCooldown()
    {
        yield return new WaitForSeconds(Random.Range(minDelayBetweenAnnouncer, maxDelayBetweenAnnouncer));
        canPlayAudio = true;
    }

    public void OnPassengerDied()
    {
        if (canPlayAudio && isActive)
        {
            int random = Random.Range(1, 101);
            if(random <= passengerDiedChance)
            {
                List<AudioClip> possibleClips = new List<AudioClip>(passengerDiedAudio);
                if (possibleClips.Contains(lastAudioClip))
                {
                    possibleClips.Remove(lastAudioClip);
                }

                if(possibleClips.Count > 0)
                {
                    PlayAnnouncerAudio(possibleClips[Random.Range(0, possibleClips.Count)]);
                }
            }
        }
    }

    public void OnPassengerFollowed()
    {
        if (canPlayAudio && isActive)
        {
            int random = Random.Range(1, 101);
            if (random <= playerHasPassengerChance)
            {
                List<AudioClip> possibleClips = new List<AudioClip>(playerHasPassengersAudio);
                if (possibleClips.Contains(lastAudioClip))
                {
                    possibleClips.Remove(lastAudioClip);
                }

                if (possibleClips.Count > 0)
                {
                    PlayAnnouncerAudio(possibleClips[Random.Range(0, possibleClips.Count)]);
                }
            }
        }
    }

    void PlayAnnouncerAudio(AudioClip clip)
    {
        lastAudioClip = clip;
        canPlayAudio = false;
        StartCoroutine(AudioCooldown());
        announcerSource.clip = clip;
        announcerSource.Play();
    }

    void SelectAnnouncerAudio()
    {
        if (canPlayAudio)
        {
            List<AudioClip> possibleClips = new List<AudioClip>();

            int i = Random.Range(0, 3);

            switch (i)
            {
                case 0:
                    possibleClips = new List<AudioClip>(generalAudio);
                    if (possibleClips.Contains(lastAudioClip))
                    {
                        possibleClips.Remove(lastAudioClip);
                    }
                    break;

                case 1:

                    if(playerHandler != null)
                    {
                        foreach(Player player in playerHandler.localPlayers)
                        {
                            if(player.followingPassengers.Count > 0)
                            {
                                possibleClips = new List<AudioClip>(playerHasPassengersAudio);
                                if (possibleClips.Contains(lastAudioClip))
                                {
                                    possibleClips.Remove(lastAudioClip);
                                }
                                break;
                            }
                        }
                    }
                    break;

                case 2:
                    if (minigameHandler != null && minigameHandler.activeMinigames.Count > 0)
                    {
                        int selectedMinigame = Random.Range(0, minigameHandler.activeMinigames.Count);

                        Minigame selected = minigameHandler.activeMinigames[selectedMinigame];

                        if(selected.location.goToThisLocationAudio != lastAudioClip)
                        {
                            possibleClips = new List<AudioClip>();
                            possibleClips.Add(selected.location.goToThisLocationAudio);
                        }
                    }
                    break;
            }

            if (possibleClips.Count > 0)
            {
                PlayAnnouncerAudio(possibleClips[Random.Range(0, possibleClips.Count)]);
            }
        }
    }
}
