using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class raycastController : MonoBehaviour
{
    public CharacterController playerController;
    public Transform gizmoPoint;
    public float pickUpRange;
    public GameObject holdingArea, pickedUpItem;
    public bool holdingItem;

    private GameObject hitGO;
    private Vector3 hitPoint = Vector3.zero;

    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            if (holdingItem)
            {
                pickUpItem();
                return;
            }

            var ray = new Ray(gizmoPoint.transform.position, gizmoPoint.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                hitPoint = hit.point;
                hitGO = hit.transform.gameObject;

                float distance = Vector3.Distance(gameObject.transform.position, hit.point);

                if (distance <= pickUpRange)
                {
                    if (hitGO.layer == LayerMask.NameToLayer("Pickup"))
                    {
                        pickedUpItem = hitGO;
                        pickUpItem();
                    }
                }

            }

        }
    }
    
    public void pickUpItem()
    {
        if (!holdingItem)
        {
            pickedUpItem.GetComponent<Rigidbody>().useGravity = false;
            pickedUpItem.transform.SetParent(holdingArea.transform, false);
            pickedUpItem.transform.localPosition = Vector3.zero;
            pickedUpItem.transform.localRotation = Quaternion.identity;
            pickedUpItem.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            pickedUpItem.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            holdingItem = true;
        }
        else
        {
            pickedUpItem.GetComponent<Rigidbody>().useGravity = true;
            pickedUpItem.transform.SetParent(null);
            pickedUpItem.GetComponent<Rigidbody>().linearVelocity = playerController.velocity;
            holdingItem = false;
            pickedUpItem = null;
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitPoint, 0.2f);
    }
}
