using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;



public class GameManager : MonoBehaviour
{

    /// <summary>inputsystemの移動</summary>
    private PUZZLE _gameInputs;

    /// <summary>色の定義</summary>
    public enum COLOR { none, cyan, magenta, yellow, red, green, blue }

    private int[,] _waku = new int[5, 5];
    [SerializeField] private GameObject _circleCyan;
    [SerializeField] private GameObject _circleYellow;
    [SerializeField] private GameObject _circleMagenta;
    /// <summary>生成した色のインスタンス</summary>
    private GameObject _generateColorCircle;
    /// <summary>生成した色のインスタンス</summary>
    private COLOR _generateColor;
    /// <summary>生成した色の生成座標</summary>
    private Vector3 _generatePosition;

    /// <summary>色を盤面に生成する：true　まだ生成した色を盤面に置いていないので生成しなくていい：false</summary>
    private bool _colorGenerateFlag;

    /// <summary>レイでヒットした盤面の位置のオブジェクトを保持する</summary>
    private GameObject _mousePositionObject;

    /// <summary>色が突然表示移動されるとわかりにくいので移動させるフラグ
    /// マウスが盤面上にない：0
    /// マウスが盤面上にあるとき最初の一回だけtrueになり移動させる：1　
    /// 盤面上に移動したので移動しない：2</summary>
    private int _moveColorCursorInt;

    /// <summary>レイキャスト</summary>
    private Ray ray;
    private RaycastHit2D hit;

    /// <summary>レイキャストのレイヤーの指定</summary>
    private int layerMask;
    /// <summary>レイキャストの長さ</summary>
    float _maxDistance;


    //色を置いた配列
    public static COLOR[,] _colorPutArray = new COLOR[4, 4];

    //盤面をListに入れる
    [SerializeField] public List<GameObject> _boardList;

    // Start is called before the first frame update
    void Start()
    {
        //色の生成座標
        _generatePosition.x = 0.0f;
        _generatePosition.y = 4.3f;

        //最初は色を生成する為
        _colorGenerateFlag = true;

        //レイキャストの設定
        layerMask = LayerMask.GetMask("Board");
        _maxDistance = 2.0f;

        // マウスが盤面上にない：0
        // マウスが盤面上にあるとき最初の一回だけtrueになり移動させる：1　
        // 盤面上に移動したので移動しない：2
        _moveColorCursorInt = 0;

        //盤面の配列の初期化
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
        //色を盤面に生成するか？
        if (_colorGenerateFlag)
        {
            _colorGenerateFlag = false;

            //色を生成
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
            //一番上に表示する
            _generateColorCircle.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }

        ////マウス処理
        Vector3 pos = _gameInputs.Player.Mouse.ReadValue<Vector2>();

        //RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(pos);
        hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, _maxDistance, layerMask);

        //生成した色の移動処理
        //
        if (hit.collider)
        {
            //枠の上にカーソルがあるとき
            //Debug.Log("hit.collider.gameObject = " + hit.collider.gameObject.tag);
            if (hit.collider.gameObject.CompareTag("Square"))
            {
                //色が突然表示移動されるとわかりにくいので盤面に移動させる
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

                //盤面の色取得
                COLOR col = hit.collider.gameObject.GetComponent<IPutColor>().GetColor();
                if (col == COLOR.none)
                {
                    //マウスが盤面上なら盤面のマウスの座標に移動
                    Vector3 pos2 = hit.collider.gameObject.transform.position;
                    _generateColorCircle.transform.position = pos2;
                    //マウスの座標にある盤面のオブジェクトを保持
                    //_mousePositionObject = hit.collider.gameObject;
                }
                if (col != COLOR.none)
                {
                    //マウスが盤面上に色があれば斜め上に移動
                    Vector3 pos2 = hit.collider.gameObject.transform.position;
                    pos2.x += 0.5f;
                    pos2.y += 0.5f;
                    _generateColorCircle.transform.position = pos2;
                }
            }

        }
    }

    /// <summary>
    /// 左クリックの処理
    /// </summary>
    /// <param name="context"></param>
    public void OnPut(InputAction.CallbackContext context)
    {
        ////マウス処理
        Vector3 pos = _gameInputs.Player.Mouse.ReadValue<Vector2>();

        //Rayの長さ
        ray = Camera.main.ScreenPointToRay(pos);
        hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, _maxDistance, layerMask);

        //盤面の色の表示処理
        //レイヤーの指定で盤面しかレイを当ててない（Squareタグしか処理しない）
        if (hit.collider)
        {
            //枠の上にカーソルがあるとき(色を置いていないとき)
            if (hit.collider.gameObject.CompareTag("Square"))
            {
                //盤面の色取得
                COLOR col = hit.collider.gameObject.GetComponent<IPutColor>().GetColor();
                
                //色を置いてないとき
                if (col == COLOR.none)
                {
                    //盤面のマウスの座標に色を置く
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

                //混ざった色がすでにあるとき
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

                //シアンが置いてあるとき
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
                //マゼンタが置いてあるとき
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
                //イエローが置いてあるとき
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
                //盤面に色を表示したので、カーソルの色をデストロイ
                Destroy(_generateColorCircle);
                //次のカーソルの色を生成する
                _colorGenerateFlag = true;
                //色が突然表示移動されるとわかりにくいので移動させるフラグ
                _moveColorCursorInt = 0;
            }
        }
    }

    /// <summary>
    /// Input System初期化
    /// </summary>
    private void OnEnable()
    {
        //Actionスクリプトのインスタンス生成
        _gameInputs = new PUZZLE();
        //左クリック
        _gameInputs.Player.Put.canceled += OnPut;

        //有効化
        _gameInputs.Enable();

    }
    /// <summary>
    /// Input System無効化
    /// </summary>
    private void OnDisable()
    {
        //左クリック
        _gameInputs.Player.Put.canceled -= OnPut;

        //無効化
        _gameInputs?.Dispose();
    }
}

