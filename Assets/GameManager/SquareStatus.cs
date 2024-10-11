using System;
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
    public int[,] _colorEraseArray = new int[4, 4];

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

        //盤面の配列の初期化
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                //最後に消す用の配列
                _colorEraseArray[x, y] = 0;
            }
        }

        //同じ基本色が３個並んだ時
        SameColor3piecesProcess();
        //違う基本色が３個並んだ時
        DifferentColors3piecesProcess();

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
                if (ColorEraseArray[x, y] == 2)
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
    /// 違う色が３個並んだかを調べる処理
    /// </summary>
    private void DifferentColors3piecesProcess()
    {
        //違う色が縦に３個並んだかを調べる処理
        DifferentColorsRowProcess();

        //違う色が横に３個並んだかを調べる処理
        DifferentColorsColumnProcess();
    }

    /// <summary>
    /// 違う色が横に３個並んだかを調べる処理
    /// </summary>
    private void DifferentColorsColumnProcess()
    {
        //盤面に置いた色を取得
        GameManager.COLOR ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y];
        //２，３行目のときに真ん中に置いたか端に置いたかを判定するフラグ 中央に置いている：true　端に置いている：false
        bool CenterFlag = true;


        //盤面に置いた色が混ざった色か？(赤か青か緑か)
        if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
        {
            return;
        }

        //横方向を調べる
        //１列目の時は上に２つ分調べる
        if ((int)_bordPosition.x == 0)
        {
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y];
            //判定するの盤面に混ざった色があるか？(赤か青か緑か)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x + 2, (int)_bordPosition.y];
            //判定するの盤面に混ざった色があるか？(赤か青か緑か)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
        }
        //２列目の時は左右に１つ分調べる
        if ((int)_bordPosition.x == 1)
        {
            //中央だった場合の処理＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
            //中央だった場合
            CenterFlag = true;

            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y];
            //判定するの盤面に混ざった色があるか？(赤か青か緑か)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y];
            //判定するの盤面に混ざった色があるか？(赤か青か緑か)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                CenterFlag = false;
            }
            //左右の色が同じ色なら中央じゃなくて端かも
            if (GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y] == GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y])
            {
                CenterFlag = false;
            }
            //ここまで来たら中央なので左右と合わせて３つ消す

            //端だった場合の処理＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
            if (!CenterFlag)
            {
                ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y];
                //判定するの盤面に混ざった色があるか？(赤か青か緑か)
                if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
                {
                    return;
                }
                ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x + 2, (int)_bordPosition.y];
                //判定するの盤面に混ざった色があるか？(赤か青か緑か)
                if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
                {
                    return;
                }
                //次とその次の色が同じ色なら消せない
                if (GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y] == GameManager._colorPutArray[(int)_bordPosition.x + 2, (int)_bordPosition.y])
                {
                    return;
                }
            }
            //ここまで来たら端から右に３つ消す

        }

        //３列目の時は左右に１つ分調べる
        if ((int)_bordPosition.x == 2)
        {
            //中央だった場合の処理＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
            //中央だった場合
            CenterFlag = true;

            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y];
            //判定するの盤面に混ざった色があるか？(赤か青か緑か)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                CenterFlag = false;
            }
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y];
            //判定するの盤面に混ざった色があるか？(赤か青か緑か)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
            //左右の色が同じ色なら中央じゃなくて端かも
            if (GameManager._colorPutArray[(int)_bordPosition.x + 1, (int)_bordPosition.y] == GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y])
            {
                CenterFlag = false;
            }
            //ここまで来たら中央なので左右と合わせて３つ消す

            //端だった場合の処理＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
            if (!CenterFlag)
            {
                ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y];
                //判定するの盤面に混ざった色があるか？(赤か青か緑か)
                if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
                {
                    return;
                }
                ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x - 2, (int)_bordPosition.y];
                //判定するの盤面に混ざった色があるか？(赤か青か緑か)
                if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
                {
                    return;
                }
                //次とその次の色が同じ色なら消せない
                if (GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y] == GameManager._colorPutArray[(int)_bordPosition.x - 2, (int)_bordPosition.y])
                {
                    return;
                }
            }
            //ここまで来たら端から右に３つ消す
        }

        //４列目の時は下に２つ分調べる
        if ((int)_bordPosition.x == 3)
        {
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x - 1, (int)_bordPosition.y];
            //判定するの盤面に混ざった色があるか？(赤か青か緑か)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x - 2, (int)_bordPosition.y];
            //判定するの盤面に混ざった色があるか？(赤か青か緑か)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
        }



        //横方向判定の処理
        //１列目の時は上に２つ分調べる
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

        //２列目の時は左右に１つ分か右２つ分か調べる
        if ((int)_bordPosition.x == 1)
        {
            //中央の場合
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
            //端の場合
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
        //３列目の時は左右に１つ分か左２つ分か調べる
        if ((int)_bordPosition.x == 2)
        {
            //中央の場合
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
            //端の場合
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

        //４列目の時は下に２つ分調べる
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
    /// 違う色が縦に３個並んだかを調べる処理
    /// </summary>
    private void DifferentColorsRowProcess()
    {
        //盤面に置いた色を取得
        GameManager.COLOR ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y];
        //２，３行目のときに真ん中に置いたか端に置いたかを判定するフラグ 中央に置いている：true　端に置いている：false
        bool CenterFlag = true;

        //盤面に置いた色が混ざった色か？(赤か青か緑か)
        if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
        {
            return;
        }

        //縦方向を調べる
        //１行目の時は上に２つ分調べる
        if ((int)_bordPosition.y == 0)
        {
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1];
            //判定するの盤面に混ざった色があるか？(赤か青か緑か)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 2];
            //判定するの盤面に混ざった色があるか？(赤か青か緑か)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
        }
        //２行目の時は上下に１つ分調べる
        if ((int)_bordPosition.y == 1)
        {
            //中央だった場合の処理＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
            //中央だった場合
            CenterFlag = true;

            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1];
            //判定するの盤面に混ざった色があるか？(赤か青か緑か)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1];
            //判定するの盤面に混ざった色があるか？(赤か青か緑か)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                CenterFlag = false;
            }
            //上下の色が同じ色なら中央じゃなくて端かも
            if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1] == GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1])
            {
                CenterFlag = false;
            }
            //ここまで来たら中央なので上下と合わせて３つ消す

            //端だった場合の処理＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
            if (!CenterFlag)
            {
                ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1];
                //判定するの盤面に混ざった色があるか？(赤か青か緑か)
                if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
                {
                    return;
                }
                ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 2];
                //判定するの盤面に混ざった色があるか？(赤か青か緑か)
                if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
                {
                    return;
                }
                //次とその次の色が同じ色なら消せない
                if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1] == GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 2])
                {
                    return;
                }
            }
            //ここまで来たら端から下に３つ消す
        }

        //３行目の時は上下に１つ分調べる
        if ((int)_bordPosition.y == 2)
        {
            //中央だった場合の処理＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
            //中央だった場合
            CenterFlag = true;

            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1];
            //判定するの盤面に混ざった色があるか？(赤か青か緑か)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                CenterFlag = false;
            }
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1];
            //判定するの盤面に混ざった色があるか？(赤か青か緑か)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
            //上下の色が同じ色なら中央じゃなくて端かも
            if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y + 1] == GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1])
            {
                CenterFlag = false;
            }
            //ここまで来たら中央なので上下と合わせて３つ消す

            //端だった場合の処理＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
            if (!CenterFlag)
            {
                ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1];
                //判定するの盤面に混ざった色があるか？(赤か青か緑か)
                if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
                {
                    return;
                }
                ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 2];
                //判定するの盤面に混ざった色があるか？(赤か青か緑か)
                if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
                {
                    return;
                }
                //次とその次の色が同じ色なら消せない
                if (GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1] == GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 2])
                {
                    return;
                }
            }
            //ここまで来たら端から上に３つ消す
        }

        //４行目の時は下に２つ分調べる
        if ((int)_bordPosition.y == 3)
        {
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 1];
            //判定するの盤面に混ざった色があるか？(赤か青か緑か)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
            ColorJudgment = GameManager._colorPutArray[(int)_bordPosition.x, (int)_bordPosition.y - 2];
            //判定するの盤面に混ざった色があるか？(赤か青か緑か)
            if ((ColorJudgment != GameManager.COLOR.red) && (ColorJudgment != GameManager.COLOR.blue) && (ColorJudgment != GameManager.COLOR.green))
            {
                return;
            }
        }

        //縦方向判定の処理
        //１行目の時は上に２つ分調べる
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

        //２行目の時は上下に１つ分調べる
        if ((int)_bordPosition.y == 1)
        {
            //中央の場合
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
            //端の場合
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

        //３行目の時は上下に１つ分調べる
        if ((int)_bordPosition.y == 2)
        {
            //中央の場合
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
            //端の場合
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
        //４行目の時は下に２つ分調べる
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
    /// 同じ色が３個以上４個まで並んだかを調べる処理
    /// </summary>
    private void SameColor3piecesProcess()
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
                //_colorEraseArray[x, y] = 0;
            }
        }


        GameManager.COLOR ColorJudgment;// = GameManager._colorPutArray[0, 0];
        int ColorCount = 0;

        //下１行目と２行目が縦三色そらったときの処理(３，４行目の処理はしない)
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

        //左１列目と２列目が横三色そらったときの処理(３，４列目は処理しない)
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

    }

}
