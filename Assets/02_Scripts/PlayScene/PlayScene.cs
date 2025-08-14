using UnityEngine;

public class PlayScene : MonoBehaviour
{
    [SerializeField] Hero _hero;
    [SerializeField] InputHandler _inputHandler;
    [SerializeField] DialogueSystem _dialogueSystem;

    //[SerializeField] CameraController _cameraController;

    private void Start()
    {
        _inputHandler.OnMoveInput += OnMoveInput;
        _inputHandler.OnAttackInput += OnAttackInput;
        _inputHandler.OnInteractInput += OnInteractInput;

        //if (_cameraController == null)
        //{
        //    Debug.LogError("VCameraController is not assigned in PlayScene.");
        //    return;
        //}
        //_inputHandler.OnCameraRotInput += _cameraController.Rotate;

        _hero.Initialize();
        _dialogueSystem.Initialize();
    }

    void OnMoveInput(Vector3 inputVector)
    {
        // 카메라의 전방 방향
        Vector3 camForward = Camera.main.transform.forward;
        // 카메라의 우측 방향
        Vector3 camRight = Camera.main.transform.right;

        // y축 방향 제거
        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        // 실제 이동 방향(카메라 기준으로 변환된 방향)

        Vector3 direction = camForward * inputVector.z + camRight * inputVector.x;

        _hero.Move(direction);
    }

    void OnAttackInput()
    {
        _hero.Attack();
    }

    void OnInteractInput()
    {
        _hero.ExecuteInteraction();
        Debug.Log("상호작용 입력받음");
    }
}
