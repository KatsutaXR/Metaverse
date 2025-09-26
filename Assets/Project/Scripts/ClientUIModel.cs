using UnityEngine;
using VContainer;

public class ClientUIModel
{
    private ClientUIModelUseCase _useCase;
    [Inject]
    public ClientUIModel(ClientUIModelUseCase useCase)
    {
        _useCase = useCase;
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
