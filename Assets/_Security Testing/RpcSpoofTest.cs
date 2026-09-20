using System.Linq;
using UnityEngine;

public class RpcSpoofTest : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
			AttackOtherPlayer();
		if (Input.GetKeyDown(KeyCode.R))
			AttackSelf();
	}

	private void AttackSelf()
	{
		var target = FindObjectsByType<Player>(FindObjectsSortMode.None).FirstOrDefault(p => p.owner == Player.LocalInstance.owner);
		int val = Random.Range(0, 100);
		Debug.Log($"[RpcSpoofTest - SELF] Sending RPC to: {target.owner} | Local identity: {Player.LocalInstance.owner}");
		target.ModifyHealthServer(val);
	}

	private void AttackOtherPlayer()
	{
		var target = FindObjectsByType<Player>(FindObjectsSortMode.None).FirstOrDefault(p => p.owner != Player.LocalInstance.owner);

		if (target == null)
		{
			Debug.LogWarning("[RpcSpoofTest] No other player found");
			return;
		}

		int val = Random.Range(0, 100);

		Debug.Log($"[RpcSpoofTest] Sending RPC to: {target.owner} | Local identity: {Player.LocalInstance.owner}");

		target.ModifyHealthServer(val);
	}
}