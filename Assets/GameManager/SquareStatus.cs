using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareStatus : MonoBehaviour, IPutColor
{
    /// <summary>色を置いたか</summary>
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


    //色を消す配列　何もない：0　消せるか？フラグ：1　消す：2
    public  int[,] _colorEraseArray = new int[4, 4];

    void Start()
    {
        _colorStatus = GameManager.COLOR.none;
        
        //非表示
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

        //色を置いた配列を更新する
        GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y] = _colorStatus;

        //Debug.Log("" + GameManager._colorPutArray[0, 3] + " " + GameManager._colorPutArray[1, 3] + " " + GameManager._colorPutArray[2, 3] + " " + GameManager._colorPutArray[3, 3]);
        //Debug.Log("" + GameManager._colorPutArray[0, 2] + " " + GameManager._colorPutArray[1, 2] + " " + GameManager._colorPutArray[2, 2] + " " + GameManager._colorPutArray[3, 2]);
        //Debug.Log("" + GameManager._colorPutArray[0, 1] + " " + GameManager._colorPutArray[1, 1] + " " + GameManager._colorPutArray[2, 1] + " " + GameManager._colorPutArray[3, 1]);
        //Debug.Log("" + GameManager._colorPutArray[0, 0] + " " + GameManager._colorPutArray[1, 0] + " " + GameManager._colorPutArray[2, 0] + " " + GameManager._colorPutArray[3, 0]);

        //消す色があるか判定
        EraseProcess();

        //基本色の判定
        BasicColorProcess();

    }


    /// <summary>
    /// 基本色の判定
    /// </summary>
    private void BasicColorProcess()
    {

    }


    /// <summary>
    /// 消す色があるかの判定
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
    /// 混ぜた色が3列縦か横に並ぶと消える処理
    /// </summary>
    private void EraseProcess()
    {
        //色を置いた作業用の配列
        int[,] ColorWorkArray = new int[4, 4];
        //盤面の配列の初期化
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                //作業用の配列
                ColorWorkArray[x, y] = 0;
                //消す用の配列
                _colorEraseArray[x, y] = 0;
            }
        }


        GameManager.COLOR ColorJudgment = GameManager._colorPutArray[0, 0];
        int ColorCount = 0;

        //縦三色そらったときの処理
        for (int row = 0; row < 2; row++)
        {
            for (int xx = 0; xx < 4; xx++)
            {
                ColorJudgment = GameManager._colorPutArray[xx, row];
                //判定するの盤面に混ざった色があるか？(赤か青か緑か)
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
                    //連続で3つある
                    if (ColorCount >= 3)
                    {
                        //消す盤面は2を入れる
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
                //連続カウントの初期化
                ColorCount = 0;
            }
        }

        //作業用の配列の初期化
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                //作業用の配列
                ColorWorkArray[x, y] = 0;
            }
        }

        //横三色そらったときの処理
        for (int column = 0; column < 2; column++)
        {
            for (int yy = 0; yy < 4; yy++)
            {
                ColorJudgment = GameManager._colorPutArray[column, yy];
                //判定するの盤面に混ざった色があるか？(赤か青か緑か)
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
                    //連続で3つある
                    if (ColorCount >= 3)
                    {
                        //消す盤面は2を入れる
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
                //連続カウントの初期化
                ColorCount = 0;
            }
        }


        //消す処理
        ArrayUpdate(_colorEraseArray);

    }

    private void ArrayUpdate(int[,] ColorEraseArray)
    {
        //配列を更新する
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


        //盤面を更新する
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
