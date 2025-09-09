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
        Vector3 correctionValue = playerCamera.forward.normalized;
        Vector3 clientUIPosition = playerCamera.position + new Vector3(correctionValue.x, 0, correctionValue.z);
        Quaternion clientUIRotation = Quaternion.Euler(new Vector3(0, playerCamera.eulerAngles.y, 0));
        return (clientUIPosition, clientUIRotation);
    }
}
