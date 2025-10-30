using UnityEngine;
using VContainer;

public class ClientUIModel
{
    private ClientUIModelUseCase _useCase;
    private Transform _leftHandTransform;
    [Inject]
    public ClientUIModel(ClientUIModelUseCase useCase)
    {
        _useCase = useCase;
    }

    public void Initialize(PlayerReferences playerReferences)
    {
        _leftHandTransform = playerReferences.LeftHand;
    }

    public (Vector3, Quaternion) Respawn()
    {
        return _useCase.Respawn();
    }

    public (Vector3, Quaternion) CorrectionClientUITransform()
    {
        Vector3 correctionValue = new Vector3(0f, 0.3f, 0.15f);
        Vector3 clientUIPosition = _leftHandTransform.position + new Vector3(0, correctionValue.y, 0);
        Quaternion clientUIRotation = Quaternion.Euler(new Vector3(0, _leftHandTransform.eulerAngles.y, 0));
        return (clientUIPosition, clientUIRotation);
    }
}
