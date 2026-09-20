using PurrNet;
using UnityEngine;

public class PlayerMovment : NetworkIdentity
{
	public float speed = 5f;

	protected override void OnSpawned()
	{
		base.OnSpawned();
		enabled = isOwner;
	}

	void Update()
	{
		if (!isOwner) return;
		float moveZ = -Input.GetAxisRaw("Horizontal");
		float moveX = Input.GetAxisRaw("Vertical");

		Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized;

		transform.Translate(movement * speed * Time.deltaTime, Space.World);
	}
}