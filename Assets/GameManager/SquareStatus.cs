using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareStatus : MonoBehaviour, IPutColor
{
    /// <summary>�F��u������</summary>
    [SerializeField] private GameManager.COLOR _colorStatus;
    [SerializeField] public Vector2 _bordPosition;

    [SerializeField] private SpriteRenderer _cyanSP;
    [SerializeField] private SpriteRenderer _magentaSP;
    [SerializeField] private SpriteRenderer _yellowSP;
    [SerializeField] private SpriteRenderer _redSP;
    [SerializeField] private SpriteRenderer _blueSP;
    [SerializeField] private SpriteRenderer _greenSP;

    [SerializeField] private GameObject GameManagerObject;
    private GameManager GameManagerScript;


    //�F�������z��@�����Ȃ��F0�@�����邩�H�t���O�F1�@�����F2
    public int[,] _colorEraseArray = new int[4, 4];

    void Start()
    {
        _colorStatus = GameManager.COLOR.none;

        //��\��
        _cyanSP.enabled = false;
        _magentaSP.enabled = false;
        _yellowSP.enabled = false;
        _redSP.enabled = false;
        _blueSP.enabled = false;
        _greenSP.enabled = false;

        GameManagerScript = GameManagerObject.GetComponent<GameManager>();
    }
    public void PutColor(GameManager.COLOR CircleColor)
    {
        _cyanSP.enabled = false;
        _magentaSP.enabled = false;
        _yellowSP.enabled = false;
        _redSP.enabled = false;
        _blueSP.enabled = false;
        _greenSP.enabled = false;

        _colorStatus = CircleColor;
        switch (_colorStatus)
        {
            case GameManager.COLOR.none:
                break;
            case GameManager.COLOR.cyan:
                _cyanSP.enabled = true;
                break;
            case GameManager.COLOR.magenta:
                _magentaSP.enabled = true;
                break;
            case GameManager.COLOR.yellow:
                _yellowSP.enabled = true;
                break;
            case GameManager.COLOR.red:
                _redSP.enabled = true;
                break;
            case GameManager.COLOR.blue:
                _blueSP.enabled = true;
                break;
            case GameManager.COLOR.green:
                _greenSP.enabled = true;
                break;
        }

        //�F��u�����z����X�V����
        GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] = _colorStatus;

        //Debug.Log("" + GameManager._colorPutArray[0, 3] + " " + GameManager._colorPutArray[1, 3] + " " + GameManager._colorPutArray[2, 3] + " " + GameManager._colorPutArray[3, 3]);
        //Debug.Log("" + GameManager._colorPutArray[0, 2] + " " + GameManager._colorPutArray[1, 2] + " " + GameManager._colorPutArray[2, 2] + " " + GameManager._colorPutArray[3, 2]);
        //Debug.Log("" + GameManager._colorPutArray[0, 1] + " " + GameManager._colorPutArray[1, 1] + " " + GameManager._colorPutArray[2, 1] + " " + GameManager._colorPutArray[3, 1]);
        //Debug.Log("" + GameManager._colorPutArray[0, 0] + " " + GameManager._colorPutArray[1, 0] + " " + GameManager._colorPutArray[2, 0] + " " + GameManager._colorPutArray[3, 0]);

        //�����F�����邩����
        EraseProcess();

        //��{�F�̔���
        BasicColorProcess();

    }


    /// <summary>
    /// ��{�F�̔���
    /// </summary>
    private void BasicColorProcess()
    {

    }


    /// <summary>
    /// �����F�����邩�̔���
    /// </summary>
    public void EraseColor()
    {
        _cyanSP.enabled = false;
        _magentaSP.enabled = false;
        _yellowSP.enabled = false;
        _redSP.enabled = false;
        _blueSP.enabled = false;
        _greenSP.enabled = false;

        _colorStatus = GameManager.COLOR.none;
    }

    public GameManager.COLOR GetColor()
    {
        return _colorStatus;
    }

    /// <summary>
    /// �������F��3��c�����ɕ��ԂƏ����鏈��
    /// </summary>
    private void EraseProcess()
    {

        //�Ֆʂ̔z��̏�����
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                //�Ō�ɏ����p�̔z��
                _colorEraseArray[x, y] = 0;
            }
        }

        //������{�F���R���񂾎�
        SameColor3piecesProcess();
        //�Ⴄ��{�F���R���񂾎�
        DifferentColors3piecesProcess();

        //��������
        ArrayUpdate(_colorEraseArray);

    }

    private void ArrayUpdate(int[,] ColorEraseArray)
    {
        //�z����X�V����
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (ColorEraseArray[x, y] == 2)
                {
                    GameManager._colorPutArray[x, y] = GameManager.COLOR.none;
                }
            }
        }


        //�Ֆʂ��X�V����
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (ColorEraseArray[x, y] == 2)
                {
                    foreach (GameObject Square in GameManagerScript._boardList)
                    {
                        Vector2 pos = Square.GetComponent<SquareStatus>()._bordPosition;

                        if (((int)pos.x == x) && ((int)pos.y == y))
                        {
                            Square.GetComponent<IPutColor>().EraseColor();
                        }
                    }
                }
            }
        }


    }

    /// <summary>
    /// �Ⴄ�F���R���񂾂��𒲂ׂ鏈��
    /// </summary>
    private void DifferentColors3piecesProcess()
    {
        //�Ⴄ�F���c�ɂR���񂾂��𒲂ׂ鏈��
        DifferentColorsRowProcess();

        //�Ⴄ�F�����ɂR���񂾂��𒲂ׂ鏈��
        DifferentColorsColumnProcess();
    }

    /// <summary>
    /// �Ⴄ�F�����ɂR���񂾂��𒲂ׂ鏈��
    /// </summary>
    private void DifferentColorsColumnProcess()
    {
        //�Ֆʂɒu�����F���擾
        GameManager.COLOR ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y];
        //�Q�C�R�s�ڂ̂Ƃ��ɐ^�񒆂ɒu�������[�ɒu�������𔻒肷��t���O �����ɒu���Ă���Ftrue�@�[�ɒu���Ă���Ffalse
        bool CenterFlag = true;


        //�Ֆʂɒu�����F�����������F���H(�Ԃ����΂�)
        if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
        {
            return;
        }

        //�������𒲂ׂ�
        //�P��ڂ̎��͏�ɂQ�����ׂ�
        if ((int)_bordPosition.x == 0)
        {
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y];
            //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x + 2, (int)_bordPosition.y];
            //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
        }
        //�Q��ڂ̎��͍��E�ɂP�����ׂ�
        if ((int)_bordPosition.x == 1)
        {
            //�����������ꍇ�̏���������������������������������������������������������������������������������������������������������������������
            //�����������ꍇ
            CenterFlag = true;

            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y];
            //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y];
            //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                CenterFlag = false;
            }
            //���E�̐F�������F�Ȃ璆������Ȃ��Ē[����
            if (GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y] == GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y])
            {
                CenterFlag = false;
            }
            //�����܂ŗ����璆���Ȃ̂ō��E�ƍ��킹�ĂR����

            //�[�������ꍇ�̏�����������������������������������������������������������������������������������������������������������������������
            if (!CenterFlag)
            {
                ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y];
                //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
                if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
                {
                    return;
                }
                ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x + 2, (int)_bordPosition.y];
                //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
                if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
                {
                    return;
                }
                //���Ƃ��̎��̐F�������F�Ȃ�����Ȃ�
                if (GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y] == GameManager._colorPutArray[(int)_bordPosition.x + 2, (int)_bordPosition.y])
                {
                    return;
                }
            }
            //�����܂ŗ�����[����E�ɂR����

        }

        //�R��ڂ̎��͍��E�ɂP�����ׂ�
        if ((int)_bordPosition.x == 2)
        {
            //�����������ꍇ�̏���������������������������������������������������������������������������������������������������������������������
            //�����������ꍇ
            CenterFlag = true;

            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y];
            //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                CenterFlag = false;
            }
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y];
            //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
            //���E�̐F�������F�Ȃ璆������Ȃ��Ē[����
            if (GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y] == GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y])
            {
                CenterFlag = false;
            }
            //�����܂ŗ����璆���Ȃ̂ō��E�ƍ��킹�ĂR����

            //�[�������ꍇ�̏�����������������������������������������������������������������������������������������������������������������������
            if (!CenterFlag)
            {
                ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y];
                //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
                if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
                {
                    return;
                }
                ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x - 2, (int)_bordPosition.y];
                //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
                if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
                {
                    return;
                }
                //���Ƃ��̎��̐F�������F�Ȃ�����Ȃ�
                if (GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y] == GameManager._colorPutArray[(int)_bordPosition.x - 2, (int)_bordPosition.y])
                {
                    return;
                }
            }
            //�����܂ŗ�����[����E�ɂR����
        }

        //�S��ڂ̎��͉��ɂQ�����ׂ�
        if ((int)_bordPosition.x == 3)
        {
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y];
            //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x - 2, (int)_bordPosition.y];
            //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
        }



        //����������̏���
        //�P��ڂ̎��͏�ɂQ�����ׂ�
        if ((int)_bordPosition.x == 0)
        {
            if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y])
            {
                if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x + 2, (int)_bordPosition.y])
                {
                    if (GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x + 2, (int)_bordPosition.y])
                    {
                        _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y] = 2;
                        _colorEraseArray[(int)_bordPosition.x + 1, (int)_bordPosition.y] = 2;
                        _colorEraseArray[(int)_bordPosition.x + 2, (int)_bordPosition.y] = 2;
                    }
                }
            }
        }

        //�Q��ڂ̎��͍��E�ɂP�����E�Q�������ׂ�
        if ((int)_bordPosition.x == 1)
        {
            //�����̏ꍇ
            if (CenterFlag)
            {
                if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y])
                {
                    if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y])
                    {
                        if (GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y])
                        {
                            _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y] = 2;
                            _colorEraseArray[(int)_bordPosition.x + 1, (int)_bordPosition.y] = 2;
                            _colorEraseArray[(int)_bordPosition.x - 1, (int)_bordPosition.y] = 2;
                        }
                    }
                }
            }
            //�[�̏ꍇ
            else
            {
                if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y])
                {
                    if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x + 2, (int)_bordPosition.y])
                    {
                        if (GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x + 2, (int)_bordPosition.y])
                        {
                            _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y] = 2;
                            _colorEraseArray[(int)_bordPosition.x + 1, (int)_bordPosition.y] = 2;
                            _colorEraseArray[(int)_bordPosition.x + 2, (int)_bordPosition.y] = 2;
                        }
                    }
                }

            }
        }
        //�R��ڂ̎��͍��E�ɂP�������Q�������ׂ�
        if ((int)_bordPosition.x == 2)
        {
            //�����̏ꍇ
            if (CenterFlag)
            {
                if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y])
                {
                    if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y])
                    {
                        if (GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y])
                        {
                            _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y] = 2;
                            _colorEraseArray[(int)_bordPosition.x + 1, (int)_bordPosition.y] = 2;
                            _colorEraseArray[(int)_bordPosition.x - 1, (int)_bordPosition.y] = 2;
                        }
                    }
                }
            }
            //�[�̏ꍇ
            else
            {
                if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y])
                {
                    if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x - 2, (int)_bordPosition.y])
                    {
                        if (GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x - 2, (int)_bordPosition.y])
                        {
                            _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y] = 2;
                            _colorEraseArray[(int)_bordPosition.x - 1, (int)_bordPosition.y] = 2;
                            _colorEraseArray[(int)_bordPosition.x - 2, (int)_bordPosition.y] = 2;
                        }
                    }
                }

            }
        }

        //�S��ڂ̎��͉��ɂQ�����ׂ�
        if ((int)_bordPosition.x == 3)
        {
            if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y])
            {
                if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x - 2, (int)_bordPosition.y])
                {
                    if (GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x - 2, (int)_bordPosition.y])
                    {
                        _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y] = 2;
                        _colorEraseArray[(int)_bordPosition.x - 1, (int)_bordPosition.y] = 2;
                        _colorEraseArray[(int)_bordPosition.x - 2, (int)_bordPosition.y] = 2;
                    }
                }
            }
        }
    }
    /// <summary>
    /// �Ⴄ�F���c�ɂR���񂾂��𒲂ׂ鏈��
    /// </summary>
    private void DifferentColorsRowProcess()
    {
        //�Ֆʂɒu�����F���擾
        GameManager.COLOR ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y];
        //�Q�C�R�s�ڂ̂Ƃ��ɐ^�񒆂ɒu�������[�ɒu�������𔻒肷��t���O �����ɒu���Ă���Ftrue�@�[�ɒu���Ă���Ffalse
        bool CenterFlag = true;

        //�Ֆʂɒu�����F�����������F���H(�Ԃ����΂�)
        if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
        {
            return;
        }

        //�c�����𒲂ׂ�
        //�P�s�ڂ̎��͏�ɂQ�����ׂ�
        if ((int)_bordPosition.y == 0)
        {
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1];
            //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 2];
            //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
        }
        //�Q�s�ڂ̎��͏㉺�ɂP�����ׂ�
        if ((int)_bordPosition.y == 1)
        {
            //�����������ꍇ�̏���������������������������������������������������������������������������������������������������������������������
            //�����������ꍇ
            CenterFlag = true;

            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1];
            //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1];
            //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                CenterFlag = false;
            }
            //�㉺�̐F�������F�Ȃ璆������Ȃ��Ē[����
            if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1] == GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1])
            {
                CenterFlag = false;
            }
            //�����܂ŗ����璆���Ȃ̂ŏ㉺�ƍ��킹�ĂR����

            //�[�������ꍇ�̏�����������������������������������������������������������������������������������������������������������������������
            if (!CenterFlag)
            {
                ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1];
                //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
                if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
                {
                    return;
                }
                ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 2];
                //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
                if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
                {
                    return;
                }
                //���Ƃ��̎��̐F�������F�Ȃ�����Ȃ�
                if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1] == GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 2])
                {
                    return;
                }
            }
            //�����܂ŗ�����[���牺�ɂR����
        }

        //�R�s�ڂ̎��͏㉺�ɂP�����ׂ�
        if ((int)_bordPosition.y == 2)
        {
            //�����������ꍇ�̏���������������������������������������������������������������������������������������������������������������������
            //�����������ꍇ
            CenterFlag = true;

            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1];
            //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                CenterFlag = false;
            }
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1];
            //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
            //�㉺�̐F�������F�Ȃ璆������Ȃ��Ē[����
            if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1] == GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1])
            {
                CenterFlag = false;
            }
            //�����܂ŗ����璆���Ȃ̂ŏ㉺�ƍ��킹�ĂR����

            //�[�������ꍇ�̏�����������������������������������������������������������������������������������������������������������������������
            if (!CenterFlag)
            {
                ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1];
                //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
                if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
                {
                    return;
                }
                ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 2];
                //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
                if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
                {
                    return;
                }
                //���Ƃ��̎��̐F�������F�Ȃ�����Ȃ�
                if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1] == GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 2])
                {
                    return;
                }
            }
            //�����܂ŗ�����[�����ɂR����
        }

        //�S�s�ڂ̎��͉��ɂQ�����ׂ�
        if ((int)_bordPosition.y == 3)
        {
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1];
            //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 2];
            //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
        }

        //�c��������̏���
        //�P�s�ڂ̎��͏�ɂQ�����ׂ�
        if ((int)_bordPosition.y == 0)
        {
            if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1])
            {
                if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 2])
                {
                    if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1] != GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 2])
                    {
                        _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y] = 2;
                        _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y + 1] = 2;
                        _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y + 2] = 2;
                    }
                }
            }
        }

        //�Q�s�ڂ̎��͏㉺�ɂP�����ׂ�
        if ((int)_bordPosition.y == 1)
        {
            //�����̏ꍇ
            if (CenterFlag)
            {
                if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1])
                {
                    if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1])
                    {
                        if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1] != GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1])
                        {
                            _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y] = 2;
                            _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y + 1] = 2;
                            _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y - 1] = 2;
                        }
                    }
                }
            }
            //�[�̏ꍇ
            else
            {
                if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1])
                {
                    if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 2])
                    {
                        if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1] != GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 2])
                        {
                            _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y] = 2;
                            _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y + 1] = 2;
                            _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y + 2] = 2;
                        }
                    }
                }
            }
        }

        //�R�s�ڂ̎��͏㉺�ɂP�����ׂ�
        if ((int)_bordPosition.y == 2)
        {
            //�����̏ꍇ
            if (CenterFlag)
            {
                if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1])
                {
                    if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1])
                    {
                        if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1] != GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1])
                        {
                            _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y] = 2;
                            _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y + 1] = 2;
                            _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y - 1] = 2;
                        }
                    }
                }
            }
            //�[�̏ꍇ
            else
            {
                if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1])
                {
                    if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 2])
                    {
                        if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1] != GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 2])
                        {

                            _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y] = 2;
                            _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y - 1] = 2;
                            _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y - 2] = 2;
                        }
                    }
                }

            }
        }
        //�S�s�ڂ̎��͉��ɂQ�����ׂ�
        if ((int)_bordPosition.y == 3)
        {
            if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1])
            {
                if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] != GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 2])
                {
                    if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1] != GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 2])
                    {
                        _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y] = 2;
                        _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y - 1] = 2;
                        _colorEraseArray[(int)_bordPosition.x, (int)_bordPosition.y - 2] = 2;
                    }
                }
            }
        }

    }

    /// <summary>
    /// �����F���R�ȏ�S�܂ŕ��񂾂��𒲂ׂ鏈��
    /// </summary>
    private void SameColor3piecesProcess()
    {
        //�F��u������Ɨp�̔z��
        int[,] ColorWorkArray = new int[4, 4];
        //�Ֆʂ̔z��̏�����
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                //��Ɨp�̔z��
                ColorWorkArray[x, y] = 0;
                //�����p�̔z��
                //_colorEraseArray[x, y] = 0;
            }
        }


        GameManager.COLOR ColorJudgment;// = GameManager._colorPutArray[0, 0];
        int ColorCount = 0;

        //���P�s�ڂƂQ�s�ڂ��c�O�F��������Ƃ��̏���(�R�C�S�s�ڂ̏����͂��Ȃ�)
        for (int row = 0; row < 2; row++)
        {
            for (int xx = 0; xx < 4; xx++)
            {
                ColorJudgment = GameManager._colorPutArray[xx, row];
                //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
                if ((ColorJudgment == GameManager.COLOR.red) || (ColorJudgment == GameManager.COLOR.blue) || (ColorJudgment == GameManager.COLOR.green))
                {
                    ColorCount = 1;
                    ColorWorkArray[xx, row] = 1;

                    for (int yy = (row + 1); yy < 4; yy++)
                    {
                        if (ColorJudgment == GameManager._colorPutArray[xx, yy])
                        {
                            ColorWorkArray[xx, yy] = 1;
                            ColorCount++;
                        }
                        else
                        {

                            break;
                        }
                    }
                    //�A����3����
                    if (ColorCount >= 3)
                    {
                        //�����Ֆʂ�2������
                        for (int i = 0; i < ColorCount; i++)
                        {
                            if (ColorWorkArray[xx, row + i] == 1)
                            {
                                ColorWorkArray[xx, row + i] = 2;
                                _colorEraseArray[xx, row + i] = 2;
                            }
                        }


                    }
                }
                //�A���J�E���g�̏�����
                ColorCount = 0;
            }
        }

        //��Ɨp�̔z��̏�����
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                //��Ɨp�̔z��
                ColorWorkArray[x, y] = 0;
            }
        }

        //���P��ڂƂQ��ڂ����O�F��������Ƃ��̏���(�R�C�S��ڂ͏������Ȃ�)
        for (int column = 0; column < 2; column++)
        {
            for (int yy = 0; yy < 4; yy++)
            {
                ColorJudgment = GameManager._colorPutArray[column, yy];
                //���肷��̔Ֆʂɍ��������F�����邩�H(�Ԃ����΂�)
                if ((ColorJudgment == GameManager.COLOR.red) || (ColorJudgment == GameManager.COLOR.blue) || (ColorJudgment == GameManager.COLOR.green))
                {
                    ColorCount = 1;
                    ColorWorkArray[column, yy] = 1;

                    for (int xx = (column + 1); xx < 4; xx++)
                    {
                        if (ColorJudgment == GameManager._colorPutArray[xx, yy])
                        {
                            ColorWorkArray[xx, yy] = 1;
                            ColorCount++;
                        }
                        else
                        {

                            break;
                        }
                    }
                    //�A����3����
                    if (ColorCount >= 3)
                    {
                        //�����Ֆʂ�2������
                        for (int i = 0; i < ColorCount; i++)
                        {
                            if (ColorWorkArray[column + i, yy] == 1)
                            {
                                ColorWorkArray[column + i, yy] = 2;
                                _colorEraseArray[column + i, yy] = 2;
                            }
                        }


                    }
                }
                //�A���J�E���g�̏�����
                ColorCount = 0;
            }
        }

    }

}
