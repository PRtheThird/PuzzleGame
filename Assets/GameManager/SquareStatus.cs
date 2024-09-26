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
    public  int[,] _colorEraseArray = new int[4, 4];

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
                _colorEraseArray[x, y] = 0;
            }
        }


        GameManager.COLOR ColorJudgment = GameManager._colorPutArray[0, 0];
        int ColorCount = 0;

        //�c�O�F��������Ƃ��̏���
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

        //���O�F��������Ƃ��̏���
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
                if (ColorEraseArray[x,y] == 2)
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

                        if(((int)pos.x ==  x) && ((int)pos.y == y))
                        {
                            Square.GetComponent<IPutColor>().EraseColor();
                        }
                    }
                }
            }
        }


    }

}
