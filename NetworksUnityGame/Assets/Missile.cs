using Photon.Pun;
using UnityEngine;

public class Missile : MonoBehaviourPun
{
    Rigidbody rb;
    GameObject explosionParticles;
    public int bounces = 2;
    public int damage = 10;

    float timeAlive = 0;

    public static float maxTimeAlive = 6;

    // Audio 
    private AudioSource audiosource; 
   
    void Awake()
    {
        explosionParticles = (GameObject)Resources.Load("PhotonPrefabs/Tanks/Missiles/Explosion");
        rb = GetComponent<Rigidbody>();

        audiosource = GetComponent<AudioSource>();

        int num = gameObject.GetPhotonView().OwnerActorNr;
        GameObject tank = PhotonNetwork.CurrentRoom.GetPlayer(num).TagObject as GameObject;
        tank.GetComponent<PlayerController>().ShootSound();
    }

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);

        if (timeAlive > maxTimeAlive)
            PhotonNetwork.Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (bounces > 0 && collision.collider.gameObject.tag != "Tank" && collision.collider.gameObject.tag != "Box")
        {
            if((bounces > 1 && !photonView.IsMine) || (bounces > 0 && photonView.IsMine))
            {
                audiosource.PlayOneShot(audiosource.clip);
            }



            bounces--;
            return;
        }

        // TODO: change to PhotonNetwork.Instantiate 
        Instantiate(explosionParticles, transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(gameObject);
    }
}
