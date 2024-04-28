using FYP;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour {
    int damage;
    Vector3 direction;
    new Rigidbody rigidbody;

    private void Update() {
        RaycastHit hit;
        LayerMask groundLayer = 1 << 6;
        if(Physics.Raycast(transform.position, -Vector3.up, out hit, 0.1f, groundLayer)){
            rigidbody.velocity = Vector3.zero;
        }
    }

    public void Throw(Vector3 direction, int damage){
        transform.AddComponent<BoxCollider>().isTrigger = true;
        rigidbody = transform.AddComponent<Rigidbody>();
        Debug.Log(direction);
        this.direction = new Vector3(direction.x,0.7f,direction.z);
        rigidbody.velocity = this.direction * damage;
        this.damage = damage;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.transform.TryGetComponent(out CharacterStats stats)){
            stats.TakeDamage(damage, null);
        }
        if(other.gameObject.layer == 6){
            rigidbody.velocity = Vector3.zero;
        }
        Destroy(gameObject,5);
        
    }


}