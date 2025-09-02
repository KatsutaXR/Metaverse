using UnityEngine;
using VContainer;

public class ClientUIModel
{
    private IClientUIModelUseCase _useCase;
    private PlayerData _playerData;
    [Inject]
    public ClientUIModel(IClientUIModelUseCase useCase, PlayerData playerData)
    {
        _useCase = useCase;
        _playerData = playerData;
    }

    public Vector3 Respawn()
    {
        return _useCase.Respawn();
    }

    public (Vector3, Quaternion) CorrectionClientUITransform()
    {
        Transform playerCamera = GameObject.FindWithTag("MainCamera").transform;
        return (playerCamera.position + playerCamera.forward.normalized * 1f, playerCamera.transform.rotation);
    }
}
