using UnityEngine;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using System.Threading;

/// <summary>
/// �v���C���[�̊e�t���[���̍s�����L�^����f�[�^�N���X
/// �X�[�p�[�^�C���t�H�[�X�E���g���̂悤�ȃN���[���Đ��V�X�e���Ŏg�p
/// </summary>
[System.Serializable]
public class PlayerAction
{
    public float time;              // �L�^�J�n����̌o�ߎ���
    public Vector2 position;        // ���̎��_�ł̃v���C���[�̈ʒu
    public Vector2 velocity;        // ���̎��_�ł̃v���C���[�̑��x�i�������Z�p�j
    public bool jumpInput;          // �W�����v�{�^���������ꂽ���ǂ���
    public float horizontalInput;   // ���E�̓��͒l�i-1.0 �` 1.0�j
}

/// <summary>
/// �v���C���[�L�����N�^�[�̈ړ��E�W�����v�E�s���L�^���Ǘ�����X�N���v�g
/// �v���C���[�̑S�Ă̍s�����L�^���A�w��^�C�~���O�ŃN���[���𐶐�����
/// </summary>
public class PlayerScript : MonoBehaviour
{
    // ========== �ړ��֘A�̃p�����[�^ ==========
    [Header("�ړ��ݒ�")]
    [Tooltip("���E�̈ړ����x")]
    public float moveSpeed = 5f;

    [Tooltip("�W�����v�̋����i������̏����x�j")]
    public float jumpForce = 7f;

    // ========== �R���|�[�l���g�Q�� ==========
    private Rigidbody2D rb;          // �������Z�p��Rigidbody2D
    public bool isGrounded;          // �n�ʂɐڒn���Ă��邩�ǂ���

    // ========== �L�^�V�X�e���֘A ==========
    [Header("�L�^�ݒ�")]
    [Tooltip("�L�^���ꂽ�S�Ă̍s���f�[�^")]
    private List<PlayerAction> recordedActions = new List<PlayerAction>();

    [Tooltip("���݋L�^�����ǂ���")]
    private bool isRecording = true;

    [Tooltip("�L�^�J�n����̌o�ߎ���")]
    private float recordingTime = 0f;

    // ========== �N���[�������p ==========
    [Header("�N���[���ݒ�")]
    [Tooltip("��������N���[���̃v���n�u�iInspector�Őݒ�K�{�j")]
    public GameObject clonePrefab;

    // ========== 弾生成用 =============
    [Header("弾生成用のプレハブ")]
    public GameObject Bullet;
    [Header("���ˈʒu�iShotPoint�j")]
    public Transform shotPoint;

    /// <summary>
    /// ����������
    /// Rigidbody2D�R���|�[�l���g���擾
    /// </summary>
    void Start()
    {
        // Rigidbody2D�R���|�[�l���g���擾�i�������Z�ɕK�v�j
        rb = GetComponent<Rigidbody2D>();

        // �N���[���v���n�u���ݒ肳��Ă��Ȃ��ꍇ�͌x�����o��
        if (clonePrefab == null)
        {
            Debug.LogWarning("ClonePrefab���ݒ肳��Ă��܂���IInspector�Őݒ肵�Ă��������B");
        }
        isRecording = false;
    }

    /// <summary>
    /// ���t���[���Ă΂��X�V����
    /// �L�^���͓��͂��L�^���AR�L�[�ŃN���[������
    /// </summary>
    void Update()
    {
        // ���E�̓��͂��擾�i-1.0 �` 1.0 �͈̔́j
        float horizontal = Input.GetAxis("Horizontal");
        // �W�����v�{�^���i�X�y�[�X�L�[�j�������ꂽ�����擾
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        // �L�^���J�n���邽�߂̏�����
        if (Mathf.Abs(horizontal) >= 0.01f || jumpPressed)
        {
            isRecording = true;
        }

        // �L�^���̏ꍇ�A�v���C���[�̓��͂Ə�Ԃ��L�^
        if (isRecording)
        {
            RecordPlayerInput();
        }

        // R�L�[�������ꂽ��N���[���𐶐�
        // �������͍D���ȃ^�C�~���O�ɕύX�\�i���S���A�^�C���A�E�g���Ȃǁj
        if (rb.linearVelocity == Vector2.zero && Mathf.Abs(horizontal) == 0.0f)
        {
            if (recordedActions.Count != 0)
            {
                Debug.Log("Counton");
                StopRecordingAndSpawnClone();
                if (isRecording)
                {
                    isRecording = false;
                }
            }
            
        }
        //���݂̃V�[�����Z�b�g
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
        // 右クリックが押されたらクローンを生成
        if (Input.GetMouseButtonDown(1))
        {
            Shot();
        }
    }

    /// <summary>
    /// �v���C���[�̓��͂Ə�Ԃ��L�^���A���ۂ̈ړ��������s��
    /// </summary>
    void RecordPlayerInput()
    {
        // ���E�̓��͂��擾�i-1.0 �` 1.0 �͈̔́j
        float horizontal = Input.GetAxis("Horizontal");

        // �W�����v�{�^���i�X�y�[�X�L�[�j�������ꂽ�����擾
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);

        // ========== ���݂̏�Ԃ��L�^ ==========
        PlayerAction action = new PlayerAction
        {
            time = recordingTime,                    // ���݂̋L�^����
            position = transform.position,           // ���݂̈ʒu
            velocity = rb.linearVelocity,            // ���݂̑��x�i�������Z�̑��x�j
            jumpInput = jumpPressed,                 // �W�����v�{�^���̓���
            horizontalInput = horizontal             // ���E�̓��͒l
        };
        recordedActions.Add(action);  // �L�^���X�g�ɒǉ�

        // �L�^���Ԃ�i�߂�
        recordingTime += Time.deltaTime;

        // ========== ���ۂ̈ړ����� ==========
        // ���E�̓��͂�����ꍇ�A�������̑��x��ݒ�
        if (Mathf.Abs(horizontal) >= 0.01f)
        {
                       
            rb.linearVelocityX = horizontal * moveSpeed;
        }
        else
        {
            // ���͂��Ȃ��ꍇ�͉������̑��x��0�ɂ���i���葱���Ȃ��悤�Ɂj
            rb.linearVelocityX = 0f;
        }

        // �n�ʂɐڒn���Ă��ăW�����v�{�^���������ꂽ�ꍇ
        if (isGrounded && jumpPressed)
        {
            if (!isRecording)
            {
                isRecording = true;
            }
            // Y�����ɗ͂������ăW�����v�iX�����̑��x�͈ێ��j
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    /// <summary>
    /// �L�^���~���ăN���[���𐶐����A�V�����L�^���J�n����
    /// </summary>
    void StopRecordingAndSpawnClone()
    {
        // �L�^�f�[�^���Ȃ��ꍇ�͉������Ȃ�
        if (recordedActions.Count == 0)
        {
            Debug.LogWarning("�L�^�f�[�^������܂���B�N���[���𐶐��ł��܂���B");
            return;
        }

        // �N���[���v���n�u���ݒ肳��Ă��Ȃ��ꍇ�͐����ł��Ȃ�
        if (clonePrefab == null)
        {
            Debug.LogError("ClonePrefab���ݒ肳��Ă��܂���I");
            return;
        }

        // �L�^���ꎞ��~
        isRecording = false;

        // ========== �N���[���̐��� ==========
        // �ŏ��̋L�^�ʒu�ɃN���[���𐶐�
        GameObject clone = Instantiate(clonePrefab, recordedActions[0].position, Quaternion.identity);

        // �N���[���̃R���g���[���[���擾
        CloneController cloneController = clone.GetComponent<CloneController>();

        if (cloneController != null)
        {
            // �N���[���ɋL�^�f�[�^��n���i�V����List���쐬���ăR�s�[�j
            cloneController.SetRecordedActions(new List<PlayerAction>(recordedActions));
        }
        else
        {
            Debug.LogError("ClonePrefab��CloneController���A�^�b�`����Ă��܂���I");
        }

        // ========== �V�����L�^���J�n ==========
        recordedActions.Clear();  // �L�^�f�[�^���N���A
        recordingTime = 0f;       // �L�^���Ԃ����Z�b�g
        isRecording = true;       // �L�^���ĊJ

        Debug.Log("�N���[���𐶐����܂����I�V�����L�^���J�n���܂��B");
    }

    void Shot()
    {
        // 弾を生成
        GameObject bullet = Instantiate(Bullet, shotPoint.position, shotPoint.rotation);
    }

    /// <summary>
    /// ���̃R���C�_�[�ƏՓ˂����u�ԂɌĂ΂��
    /// �n�ʂƂ̐ڐG�����m���Đڒn��Ԃ��X�V
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �Փ˂����I�u�W�F�N�g��"Ground"�^�O�������Ă���ꍇ
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;  // �ڒn��Ԃ�true��
        }
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // �Փ˂����I�u�W�F�N�g��"Ground"�^�O�������Ă���ꍇ
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;  // �ڒn��Ԃ�true��
        }        
    }

    /// <summary>
    /// ���̃R���C�_�[���痣�ꂽ�u�ԂɌĂ΂��
    /// �n�ʂ��痣�ꂽ���Ƃ����m���Đڒn��Ԃ��X�V
    /// </summary>
    private void OnCollisionExit2D(Collision2D collision)
    {
        // ���ꂽ�I�u�W�F�N�g��"Ground"�^�O�������Ă���ꍇ
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;  // �ڒn��Ԃ�false��
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Clone"))
        {
            isGrounded = true;  // �ڒn��Ԃ�true��
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Clone"))
        {
            isGrounded = true;  // �ڒn��Ԃ�true��
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Clone"))
        {
            isGrounded = false;  // �ڒn��Ԃ�true��
        }
    }

   
}