using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;

public class raycastController : MonoBehaviour
{
    public CharacterController playerController;
    public Transform gizmoPoint;
    public float pickUpRange, interactRange = 6f;
    public GameObject holdingArea, pickedUpItem;
    public bool holdingItem;

    private GameObject hitGO;
    private Vector3 hitPoint = Vector3.zero;

    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            var ray = new Ray(gizmoPoint.transform.position, gizmoPoint.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                hitPoint = hit.point;
                hitGO = hit.transform.gameObject;
                Debug.Log(hitGO);
                float distance = Vector3.Distance(gameObject.transform.position, hit.point);

                if (hitGO.GetComponent<Lock>() && distance <= interactRange)
                {
                    if (pickedUpItem.CompareTag("key"))
                    {
                        hitGO.GetComponent<Lock>().unLock(pickedUpItem);
                        Destroy(pickedUpItem);
                        pickedUpItem = null;
                    }
                }

                if (distance <= pickUpRange)
                {
                    if (hitGO.layer == LayerMask.NameToLayer("Pickup"))
                    {
                        pickedUpItem = hitGO;
                        pickUpItem();
                    }
                }

                if(distance <= interactRange)
                {
                    if (hitGO.CompareTag("activateAbility"))
                    {
                        gameObject.GetComponent<PlayerController>().enableAbility();
                        Destroy(hitGO);
                    }
                }

            }
            else
            {
                if (holdingItem)
                {
                    pickUpItem();
                    return;
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
