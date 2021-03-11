using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueBoat : MonoBehaviour
{
    [SerializeField] Seat[] seats;
    [SerializeField] TriggerEffect entranceTrigger;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        entranceTrigger.onTriggerEnterCollider += (col) => SeatTarget(col.gameObject);
    }

    private void OnDisable()
    {
        entranceTrigger.onTriggerEnterCollider -= (col) => SeatTarget(col.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SeatTarget(GameObject target)
    {
        foreach(Seat seat in seats)
        {
            if(seat.seatOwner == null)
            {
                seat.seatOwner = target;
                target.transform.position = seat.seatLocation.position;
                target.transform.parent = seat.seatLocation;

                Entity entity = target.GetComponent<Entity>();
                if (entity != null)
                {
                    entity.Seat(true);
                    entity.Disable(true);
                }
            }
            else
            {
                Debug.Log("BOAT IS FULL");
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
