using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueBoat : MonoBehaviour
{
    GameHandler gameHandler;
    [SerializeField] Seat[] seats;

    [SerializeField] GameObject poofParticle;
    [SerializeField] float particleDuration = 1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SeatTarget(Collider col)
    {
        GameObject target = col.gameObject;

        for(int i = 0; i < seats.Length; i++)
        {
            Seat seat = seats[i];

            if (seat.seatOwner == null)
            {
                Entity entity = target.GetComponent<Entity>();
                Passenger passenger = target.GetComponent<Passenger>();

                if (passenger != null && passenger.ownerPlayer == null || entity != null && entity.disables > 0)
                {
                    return;
                }

                GameObject newSeatPoof = Instantiate(poofParticle, seat.seatLocation.position, Quaternion.identity);
                GameObject newPlayerPoof = Instantiate(poofParticle, target.transform.position, Quaternion.identity);

                Destroy(newSeatPoof, particleDuration);
                Destroy(newPlayerPoof, particleDuration);

                if (entity != null)
                {
                    entity.Disable(true);
                    entity.Seat(true);
                }

                seat.seatOwner = target;

                target.transform.position = seat.seatLocation.position;
                target.transform.parent = seat.seatLocation;

                if (gameHandler == null)
                {
                    gameHandler = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameHandler>();
                }

                if (gameHandler != null)
                {
                    gameHandler.PassengerSaved();
                    gameHandler.npcSpawner.availableNPCs.Remove(entity.gameObject);
                    gameHandler.npcSpawner.CheckNPCCount();
                }

                if (passenger != null)
                {
                    passenger.currentState = Passenger.AIState.Rescued;
                }

                if (i >= seats.Length - 1)
                {
                    for(int q = seats.Length - 1; q >= 0; q--)
                    {
                        Seat thisSeat = seats[q];
                        if (thisSeat.seatOwner != null)
                        {
                            GameObject destroyPoof = Instantiate(poofParticle, thisSeat.seatLocation.position, Quaternion.identity);
                            Destroy(destroyPoof, particleDuration);
                            Destroy(thisSeat.seatOwner);
                            thisSeat.seatOwner = null;
                        }
                    }
                }


                break;
            }
        }
    }


    [System.Serializable]
    public class Seat
    {
        public Transform seatLocation;
        public GameObject seatOwner;
    }
}
