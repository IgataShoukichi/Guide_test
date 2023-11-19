using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerController : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    //追いかけるプレイヤー
    private PlayerController followPlayer;
    private float plTimer = 0, myTimer = 0;
    //追いかける配列(座標と角度)の番号
    private int currentFollowNum = 0;
    [SerializeField] Transform myTran;
    [Header("1秒間に回転する速度")]
    [SerializeField] float rotationSpeed = 360f;
    private Quaternion targetRotation;
    //案内者とはこの距離を維持しようとする
    [SerializeField] public int cusNum;
    private float limitDistance;

    [Header("座標を保存する周期")]
    [SerializeField] float span = 0.1f;
    [Header("客が追いかけ始めるまでにかかる時間")]
    [SerializeField] float followDelay = 0.2f;

    private List<Vector3> playerPos = new List<Vector3>();
    [SerializeField] LineRenderer lineRenderer;

    public bool isTable_Customer;
    public Vector3 sitPos;

    float eatDesire = 0;
    public bool eatNow = false;
    [Header("客が食事可能な時に表示するUI")]
    [SerializeField] CanvasGroup eatCanvas;
    [Header("客が食べ終わるまでの時間")]
    public float eatTime;

    [Header("客がレジまで行くのにかかる時間")]
    public float exitsTime;
    private Table thisTable;
    public static int waitCus;

    private Family fam;

    enum CustomerState
    {
        WAIT,
        FOLLOW,
        EAT,
        EXITS
    }

    public void DoFollow(PlayerController player)
    {
        if (followPlayer != null)
            return;

        followPlayer = player;
    }
    public void StopFollow()
    {
        followPlayer = null;
    }

    //初期状態；
    CustomerState currentState = CustomerState.WAIT;
    bool stateEnter = true;

    void ChangeState(CustomerState newState)
    {
        currentState = newState;
        stateEnter = true;
    }

    void DoPath()
    {
        thisTable = fam.table.GetComponent<Table>();
        //指定したPathをexitsTime秒で進行方向を向きながら通る。
        exitsTime += thisTable.count;
        waitCus++;
        transform.DOPath(thisTable.exitsPos, exitsTime).SetLookAt(0.1f).SetEase(Ease.Linear);
    }

    private void Awake()
    {
        fam = GetComponentInParent<Family>();

        float[] limit =
        {
            Mathf.Pow(1.5f, 2),
            Mathf.Pow(2.5f, 2),
            Mathf.Pow(3.5f, 2),
            Mathf.Pow(4.5f, 2)
        };

        if (cusNum >= 0 && cusNum < limit.Length)
        {
            limitDistance = limit[cusNum];
        }
        else
        {
            Debug.Log("cusNumがおかしい");
        }
    }
    private void Start()
    {
        //updateSpan秒待たずに1個目の座標と角度を保存するために、updateSpan秒を代入しておく
        plTimer = span;
        //初期化
        targetRotation = myTran.rotation;

        eatCanvas.alpha = 0;

    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case CustomerState.WAIT: //WAIT時の処理
                #region
                if (stateEnter)
                {
                    stateEnter = false;
                    Debug.Log("待機状態");
                }

                if (followPlayer != null)
                {
                    ChangeState(CustomerState.FOLLOW);
                    return;
                }
                break;
            #endregion

            case CustomerState.FOLLOW:　//FOLLOW時の処理
                #region
                if (stateEnter)
                {
                    stateEnter = false;
                    Debug.Log("追従状態");
                }

                if (followPlayer != null)
                {
                    //プレイヤーの移動入力がないときは座標の保存を行わない
                    if (followPlayer.input.sqrMagnitude != 0.0f)
                    {
                        plTimer += Time.deltaTime;
                        if (plTimer >= span)
                        {
                            plTimer = 0;

                            //プレイヤーの座標をリストに保存
                            playerPos.Add(followPlayer.myTran.position);

                            lineRenderer.positionCount = playerPos.Count;
                            lineRenderer.SetPosition(playerPos.Count - 1, playerPos[playerPos.Count - 1]);
                        }
                    }
                    else
                    {
                        //プレイヤーの操作がないときに客用のタイマーをリセット
                        myTimer = 0;
                    }

                    float distance = (followPlayer.myTran.position - myTran.position).sqrMagnitude;
                    if (playerPos.Count > 0 && distance > limitDistance)
                    {
                        myTimer += Time.deltaTime;
                        //プレイヤーが動いたときにすぐに動くと不自然なので、少し遅らせる
                        if (myTimer >= followDelay)
                        {
                            //目的地が近すぎないときは、その目的地を向く角度を計算する
                            if ((playerPos[currentFollowNum] - myTran.position).sqrMagnitude > 0.01f)
                                targetRotation = Quaternion.LookRotation(playerPos[currentFollowNum] - myTran.position);
                            //指定した回転速度で徐々に計算した角度に変化していく
                            myTran.rotation = Quaternion.RotateTowards(myTran.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                            //近すぎるときは減速し、遠いときは加速する可変スピード(基準はプレイヤーの速度)
                            var speed = followPlayer.speed * distance / limitDistance;
                            //目的地には可変スピードを使って移動する
                            myTran.position = Vector3.MoveTowards(myTran.position, playerPos[currentFollowNum], Time.deltaTime * followPlayer.speed);

                            //目的地が近くなったら次の目的地に切り替える
                            //↑が発生する距離の設定が近すぎると、プレイヤーがその場でぐるぐる移動した時もその通り動いてしまうので不自然になる
                            //そのためある程度離れていても目的地の切り替えを行い、自然な移動を表現する
                            if ((playerPos[currentFollowNum] - myTran.position).sqrMagnitude <= Mathf.Pow(0.5f, 2) && playerPos.Count > currentFollowNum + 1)
                                currentFollowNum++;
                        }
                    }
                    else
                    {
                        //ここでタイマーをリセットすると客の動きがカクカクすることがあるのでここではやらない
                    }
                }
                else if (followPlayer == null)
                {
                    playerPos.Clear();
                    currentFollowNum = 0;

                    if (fam != null)
                    {
                        if (cusNum >= 0 && cusNum < fam.myPos.Length)
                        {
                            sitPos = fam.myPos[cusNum];
                        }
                        else
                        {
                            Debug.Log("myPosがおかしい");
                        }

                        if (sitPos != null)
                        {
                            gameObject.transform.position = sitPos;
                            ChangeState(CustomerState.EAT);
                        }
                    }

                    else
                    {
                        gameObject.transform.position = this.transform.position;
                        ChangeState(CustomerState.WAIT);
                    }
                    return;
                }
                break;
            #endregion

            case CustomerState.EAT: //EAT時の処理
                #region

                if (stateEnter)
                {
                    Debug.Log("着席");
                    stateEnter = false;
                    eatDesire = 0;
                    eatCanvas.alpha = 1;
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    eatNow = true;
                    Debug.Log("食事状態");
                    eatCanvas.alpha = 0;
                }

                if (eatNow)
                {
                    eatDesire += Time.deltaTime / eatTime;

                    if (eatDesire >= 1 && waitCus <= 5)
                    {
                        ChangeState(CustomerState.EXITS);
                        eatDesire = 0;
                    }
                }

                break;
            #endregion

            case CustomerState.EXITS: //EXITS時の処理
                #region

                if (stateEnter)
                {
                    StartCoroutine("DoPath");
                    stateEnter = false;
                    Debug.Log("帰宅状態");
                }

                ChangeState(CustomerState.WAIT);

                break;
                #endregion

        }
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Table"))
        {
            isTable_Customer = true;
            setPos = other.gameObject.transform.position;
            Vector3[] myPos =
            {
                setPos + new Vector3(0.5f, 0, 0.5f),
                setPos + new Vector3(0.5f, 0, -0.5f),
                setPos + new Vector3(-0.5f, 0, -0.5f),
                setPos + new Vector3(-0.5f, 0, 0.5f)
            };

            if (cusNum >= 0 && cusNum < myPos.Length)
            {
                sitPos = myPos[cusNum];
            }
            else
            {
                Debug.Log("myPosがおかしい");
            }
            table = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Table"))
        {
            isTable_Customer = false;
        }
    }
    */
}