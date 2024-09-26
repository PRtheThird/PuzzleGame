using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;



public class GameManager : MonoBehaviour
{

    /// <summary>inputsystem�̈ړ�</summary>
    private PUZZLE _gameInputs;

    /// <summary>�F�̒�`</summary>
    public enum COLOR { none, cyan, magenta, yellow, red, green, blue }

    private int[,] _waku = new int[5, 5];
    [SerializeField] private GameObject _circleCyan;
    [SerializeField] private GameObject _circleYellow;
    [SerializeField] private GameObject _circleMagenta;
    /// <summary>���������F�̃C���X�^���X</summary>
    private GameObject _generateColorCircle;
    /// <summary>���������F�̃C���X�^���X</summary>
    private COLOR _generateColor;
    /// <summary>���������F�̐������W</summary>
    private Vector3 _generatePosition;

    /// <summary>�F��Ֆʂɐ�������Ftrue�@�܂����������F��Ֆʂɒu���Ă��Ȃ��̂Ő������Ȃ��Ă����Ffalse</summary>
    private bool _colorGenerateFlag;

    /// <summary>���C�Ńq�b�g�����Ֆʂ̈ʒu�̃I�u�W�F�N�g��ێ�����</summary>
    private GameObject _mousePositionObject;

    /// <summary>�F���ˑR�\���ړ������Ƃ킩��ɂ����̂ňړ�������t���O
    /// �}�E�X���Ֆʏ�ɂȂ��F0
    /// �}�E�X���Ֆʏ�ɂ���Ƃ��ŏ��̈�񂾂�true�ɂȂ�ړ�������F1�@
    /// �Ֆʏ�Ɉړ������̂ňړ����Ȃ��F2</summary>
    private int _moveColorCursorInt;

    /// <summary>���C�L���X�g</summary>
    private Ray ray;
    private RaycastHit2D hit;

    /// <summary>���C�L���X�g�̃��C���[�̎w��</summary>
    private int layerMask;
    /// <summary>���C�L���X�g�̒���</summary>
    float _maxDistance;


    //�F��u�����z��
    public static COLOR[,] _colorPutArray = new COLOR[4, 4];

    //�Ֆʂ�List�ɓ����
    [SerializeField] public List<GameObject> _boardList;

    // Start is called before the first frame update
    void Start()
    {
        //�F�̐������W
        _generatePosition.x = 0.0f;
        _generatePosition.y = 4.3f;

        //�ŏ��͐F�𐶐������
        _colorGenerateFlag = true;

        //���C�L���X�g�̐ݒ�
        layerMask = LayerMask.GetMask("Board");
        _maxDistance = 2.0f;

        // �}�E�X���Ֆʏ�ɂȂ��F0
        // �}�E�X���Ֆʏ�ɂ���Ƃ��ŏ��̈�񂾂�true�ɂȂ�ړ�������F1�@
        // �Ֆʏ�Ɉړ������̂ňړ����Ȃ��F2
        _moveColorCursorInt = 0;

        //�Ֆʂ̔z��̏�����
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                _colorPutArray[x, y] = COLOR.none;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //�F��Ֆʂɐ������邩�H
        if (_colorGenerateFlag)
        {
            _colorGenerateFlag = false;

            //�F�𐶐�
            switch (Random.Range(0, 3))
            {
                case 0:
                    _generateColorCircle = Instantiate(_circleCyan, _generatePosition, Quaternion.identity);
                    _generateColor = COLOR.cyan;
                    break;
                case 1:
                    _generateColorCircle = Instantiate(_circleYellow, _generatePosition, Quaternion.identity);
                    _generateColor = COLOR.yellow;
                    break;
                case 2:
                    _generateColorCircle = Instantiate(_circleMagenta, _generatePosition, Quaternion.identity);
                    _generateColor = COLOR.magenta;
                    break;
            }
            //��ԏ�ɕ\������
            _generateColorCircle.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }

        ////�}�E�X����
        Vector3 pos = _gameInputs.Player.Mouse.ReadValue<Vector2>();

        //RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(pos);
        hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, _maxDistance, layerMask);

        //���������F�̈ړ�����
        //
        if (hit.collider)
        {
            //�g�̏�ɃJ�[�\��������Ƃ�
            //Debug.Log("hit.collider.gameObject = " + hit.collider.gameObject.tag);
            if (hit.collider.gameObject.CompareTag("Square"))
            {
                //�F���ˑR�\���ړ������Ƃ킩��ɂ����̂ŔՖʂɈړ�������
                if (_moveColorCursorInt <= 1)
                {
                    _moveColorCursorInt = 1;
                    _generateColorCircle.transform.position = Vector3.MoveTowards(_generateColorCircle.transform.position, hit.collider.gameObject.transform.position, 5 * Time.deltaTime);
                    if(_generateColorCircle.transform.position == hit.collider.gameObject.transform.position)
                    {
                        _moveColorCursorInt = 2;
                    }
                    return;
                }

                //�Ֆʂ̐F�擾
                COLOR col = hit.collider.gameObject.GetComponent<IPutColor>().GetColor();
                if (col == COLOR.none)
                {
                    //�}�E�X���Ֆʏ�Ȃ�Ֆʂ̃}�E�X�̍��W�Ɉړ�
                    Vector3 pos2 = hit.collider.gameObject.transform.position;
                    _generateColorCircle.transform.position = pos2;
                    //�}�E�X�̍��W�ɂ���Ֆʂ̃I�u�W�F�N�g��ێ�
                    //_mousePositionObject = hit.collider.gameObject;
                }
                if (col != COLOR.none)
                {
                    //�}�E�X���Ֆʏ�ɐF������Ύ΂ߏ�Ɉړ�
                    Vector3 pos2 = hit.collider.gameObject.transform.position;
                    pos2.x += 0.5f;
                    pos2.y += 0.5f;
                    _generateColorCircle.transform.position = pos2;
                }
            }

        }
    }

    /// <summary>
    /// ���N���b�N�̏���
    /// </summary>
    /// <param name="context"></param>
    public void OnPut(InputAction.CallbackContext context)
    {
        ////�}�E�X����
        Vector3 pos = _gameInputs.Player.Mouse.ReadValue<Vector2>();

        //Ray�̒���
        ray = Camera.main.ScreenPointToRay(pos);
        hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, _maxDistance, layerMask);

        //�Ֆʂ̐F�̕\������
        //���C���[�̎w��ŔՖʂ������C�𓖂ĂĂȂ��iSquare�^�O�����������Ȃ��j
        if (hit.collider)
        {
            //�g�̏�ɃJ�[�\��������Ƃ�(�F��u���Ă��Ȃ��Ƃ�)
            if (hit.collider.gameObject.CompareTag("Square"))
            {
                //�Ֆʂ̐F�擾
                COLOR col = hit.collider.gameObject.GetComponent<IPutColor>().GetColor();
                
                //�F��u���ĂȂ��Ƃ�
                if (col == COLOR.none)
                {
                    //�Ֆʂ̃}�E�X�̍��W�ɐF��u��
                    switch (_generateColor)
                    {
                        case COLOR.cyan:
                            hit.collider.gameObject.GetComponent<IPutColor>().PutColor(COLOR.cyan);
                            break;
                        case COLOR.yellow:
                            hit.collider.gameObject.GetComponent<IPutColor>().PutColor(COLOR.yellow);
                            break;
                        case COLOR.magenta:
                            hit.collider.gameObject.GetComponent<IPutColor>().PutColor(COLOR.magenta);
                            break;
                    }
                }

                //���������F�����łɂ���Ƃ�
                if (col == COLOR.red)
                {
                    return;
                }
                if (col == COLOR.blue)
                {
                    return;
                }
                if (col == COLOR.green)
                {
                    return;
                }

                //�V�A�����u���Ă���Ƃ�
                if (col == COLOR.cyan)
                {
                    if(_generateColor == COLOR.cyan)
                    {
                        return;
                    }
                    if (_generateColor == COLOR.magenta)
                    {
                        hit.collider.gameObject.GetComponent<IPutColor>().PutColor(COLOR.blue);
                    }
                    if (_generateColor == COLOR.yellow)
                    {
                        hit.collider.gameObject.GetComponent<IPutColor>().PutColor(COLOR.green);
                    }
                }
                //�}�[���^���u���Ă���Ƃ�
                if (col == COLOR.magenta)
                {
                    if (_generateColor == COLOR.cyan)
                    {
                        hit.collider.gameObject.GetComponent<IPutColor>().PutColor(COLOR.blue);
                    }
                    if (_generateColor == COLOR.magenta)
                    {
                        return;
                    }
                    if (_generateColor == COLOR.yellow)
                    {
                        hit.collider.gameObject.GetComponent<IPutColor>().PutColor(COLOR.red);
                    }
                }
                //�C�G���[���u���Ă���Ƃ�
                if (col == COLOR.yellow)
                {
                    if (_generateColor == COLOR.cyan)
                    {
                        hit.collider.gameObject.GetComponent<IPutColor>().PutColor(COLOR.green);
                    }
                    if (_generateColor == COLOR.magenta)
                    {
                        hit.collider.gameObject.GetComponent<IPutColor>().PutColor(COLOR.red);
                    }
                    if (_generateColor == COLOR.yellow)
                    {
                        return;
                    }
                }
                //�ՖʂɐF��\�������̂ŁA�J�[�\���̐F���f�X�g���C
                Destroy(_generateColorCircle);
                //���̃J�[�\���̐F�𐶐�����
                _colorGenerateFlag = true;
                //�F���ˑR�\���ړ������Ƃ킩��ɂ����̂ňړ�������t���O
                _moveColorCursorInt = 0;
            }
        }
    }

    /// <summary>
    /// Input System������
    /// </summary>
    private void OnEnable()
    {
        //Action�X�N���v�g�̃C���X�^���X����
        _gameInputs = new PUZZLE();
        //���N���b�N
        _gameInputs.Player.Put.canceled += OnPut;

        //�L����
        _gameInputs.Enable();

    }
    /// <summary>
    /// Input System������
    /// </summary>
    private void OnDisable()
    {
        //���N���b�N
        _gameInputs.Player.Put.canceled -= OnPut;

        //������
        _gameInputs?.Dispose();
    }
}

