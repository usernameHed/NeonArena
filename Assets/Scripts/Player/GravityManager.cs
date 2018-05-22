using UnityEngine;
using Sirenix.OdinInspector;

public class GravityManager : MonoBehaviour
{
	// Parameters
    [FoldoutGroup("Gameplay")] [Tooltip("Gravite Fin de Saut")]
	public float afterGravityForce = 2.0f;

   

	[FoldoutGroup("Jump")] [Tooltip("saut")]
	public float jumpVelocity = 5f;
	[FoldoutGroup("WallJump")] [Tooltip("Ajoute une force Vertical au walljump ?")]
	public float forceWallVerticalJump = 2f;
	[FoldoutGroup("WallJump")] [Tooltip("Ajoute une force Horizontal au walljump ?")]
	public float forceWallHorizontalJump = 2f;
	[FoldoutGroup("WallJump")] [Tooltip("Force Slide down sur les murs")]
	public float slideDownForce = 2f;

	// Components
    private Rigidbody playerBody;
    private PlayerController playerController;

	// Private variables
	private float yPos;
	private float yLast;

	private bool onGround = false;
	public bool OnGround
	{
		get { return onGround; }
	}

	private int wallDirection = 1;


    private void Awake()
    {
		playerBody = gameObject.GetComponent<Rigidbody>();
		playerController = gameObject.GetComponent<PlayerController>();

        yPos = transform.position.y;
        yLast = yPos;
    }

    // Update wall jump status
    private void UpdateWallJumpState()
    {
		RaycastHit wallHit;
		Physics.Raycast (transform.position, Vector3.right * playerController.Direction, out wallHit, 0.5f);

		if (playerController.IsWalking && playerController.Direction == 1 && wallHit.collider != null && wallHit.collider.CompareTag("Tiles"))
		{
			wallDirection = 1;
            playerController.animatorPlayer.SetBool("OnFloor", true);
            

            if (playerBody.velocity.x > 0)
			{
				playerBody.velocity = new Vector3 (0.0F, slideDownForce, 0.0F);
                
            }
		}
		else if (playerController.IsWalking && playerController.Direction == -1 && wallHit.collider != null && wallHit.collider.CompareTag("Tiles"))
		{
			wallDirection = -1;
            playerController.animatorPlayer.SetBool("OnFloor", true);

            if (playerBody.velocity.x < 0)
			{
				playerBody.velocity = new Vector3 (0.0F, slideDownForce, 0.0F);
                
            }
		}
		else
		{
			wallDirection = 0;
		}
			
		//Debug.Log (wallDirection + " " + playerController.IsWalking + " " + onGround);
    }

	private void Jump()
	{
		bool jump = PlayerConnected.getSingularity().getPlayer(playerController.getPlayerID()).GetButtonDown("FireA");

		// Is jumping ?
		if (jump)
		{
			// Wall Jump
			if (wallDirection != 0)
			{
				SoundManager.Instance.PlaySound (SoundManager.Instance.JumpSound);

				Vector3 wallJumpDirection = (Vector3.right * -wallDirection * forceWallHorizontalJump) + (Vector3.up * forceWallVerticalJump);
				playerBody.AddForce (new Vector3(wallJumpDirection.x, 0.0F, 0.0F), ForceMode.Impulse);
				playerBody.velocity = new Vector3 (playerBody.velocity.x, wallJumpDirection.y, 0.0F);

                Vector3 posBump = new Vector3(transform.position.x, transform.position.y, 0);
                GameObject tmpParticle = Instantiate(playerController.prefabsNormalBump, posBump, Quaternion.Euler(0, 0, 90 * wallDirection));

				float offset = wallDirection == -1 ? 0.7F : -0.7F;
				Vector3 offV = new Vector3 (offset, 0.0F, 0.0F);
				tmpParticle.transform.position = tmpParticle.transform.position + offV; //marche dans un sens

                playerController.animatorPlayer.SetBool("OnWall", true);
            }
			// Simple jump
			else if(onGround)
			{

				SoundManager.Instance.PlaySound (SoundManager.Instance.WallJumpSound);

                Vector3 posBump = new Vector3(transform.position.x, transform.position.y + 0.7f, 0);
                Instantiate(playerController.prefabsNormalBump, posBump, Quaternion.identity);

                playerController.animatorPlayer.SetBool("Jump", true);
                
				playerBody.AddForce (new Vector3(0.0F, jumpVelocity, 0.0F), ForceMode.Impulse);
			}

			//PC.setVibrationPlayer(idPlayer);
			
		}
	}

    private void Update()
    {
        yPos = transform.position.y;

        if (!playerController.stopMove)
        {
            UpdateWallJumpState();
            Jump();
        }

		// Update last pos
        yLast = yPos;
    }

    private void FixedUpdate()
    {
		
		// Is falling ?
		if (yPos < yLast && !onGround)
		{
			playerBody.AddForce (-Vector3.up * afterGravityForce, ForceMode.Acceleration);
		}
    }

    void OnTriggerStay(Collider collider)
	{
		onGround = true;
        playerController.animatorPlayer.SetBool("OnFloor", true);
    }

	void OnTriggerExit(Collider collider)
	{
		onGround = false;
        playerController.animatorPlayer.SetBool("OnFloor", false);
    }
}
