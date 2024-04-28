using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Data.Common;

public class PlayerMinimapIconController : NetworkBehaviour
{
    [SerializeField] SpriteRenderer icon;
    [SerializeField] Sprite[] iconSprites;

    NetworkVariable<byte> m_iconID = new NetworkVariable<byte>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        m_iconID.OnValueChanged += OnIconIDChanged;
    }

    public void ResetID()
    {
        if(IsOwner && m_iconID.Value != 0)
        {
            m_iconID.Value = 0;
        }
    }

    public void SetID(byte id)
    {
        if(IsOwner && id != m_iconID.Value)
        {
            m_iconID.Value = id;
        }
    }

    private void OnIconIDChanged(byte previousValue, byte newValue)
    {
        icon.sprite = iconSprites[newValue];
    }
}
