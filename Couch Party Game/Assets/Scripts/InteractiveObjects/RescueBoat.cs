using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueBoat : MonoBehaviour
{
    [SerializeField] Seat[] seats;
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
        foreach(Seat seat in seats)
        {
            if(seat.seatOwner == null)
            {
                Entity entity = target.GetComponent<Entity>();
                if (entity != null)
                {
                    entity.Disable(true);
                    entity.Seat(true);
                }

                seat.seatOwner = target;
                target.transform.position = seat.seatLocation.position;
                target.transform.parent = seat.seatLocation;
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
