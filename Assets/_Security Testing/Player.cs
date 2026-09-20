using PurrNet;
using TMPro;
using UnityEngine;

public class Player : NetworkIdentity
{
	[SerializeField] private TextMeshProUGUI _text;
	[SerializeField] private int _health = 100;

	public static Player LocalInstance { get; private set; }

	protected override void OnSpawned()
	{
		base.OnSpawned();
		UpdateLocalText();
		enabled = isOwner;

		if (isOwner)
			LocalInstance = this;
	}

	protected override void OnDespawned()
	{
		base.OnDespawned();
		if (LocalInstance == this)
			LocalInstance = null;
	}

	private void Update()
	{
		UpdateLocalText();
	}

	private void UpdateLocalText()
	{
		if (_text == null) return;
		string str = owner.HasValue ? owner.Value.ToString() : "None";
		_text.text = $"Health: {_health}\nOwner: {str}";
	}

	[ServerRpc(requireOwnership: true)]
	public void ModifyHealthServer(int newHealth, RPCInfo info = default)
	{
		Debug.Log($"[Server] RPC Recived. Claimed sender: {info.sender} | Target owner: {owner}");
		SetHealthObservers(newHealth);
	}

	[ObserversRpc]
	private void SetHealthObservers(int newHealth, RPCInfo info = default)
	{
		_health = newHealth;
		UpdateLocalText();
	}
}