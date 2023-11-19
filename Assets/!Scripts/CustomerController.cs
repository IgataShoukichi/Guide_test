using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerController : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    //�ǂ�������v���C���[
    private PlayerController followPlayer;
    private float plTimer = 0, myTimer = 0;
    //�ǂ�������z��(���W�Ɗp�x)�̔ԍ�
    private int currentFollowNum = 0;
    [SerializeField] Transform myTran;
    [Header("1�b�Ԃɉ�]���鑬�x")]
    [SerializeField] float rotationSpeed = 360f;
    private Quaternion targetRotation;
    //�ē��҂Ƃ͂��̋������ێ����悤�Ƃ���
    [SerializeField] public int cusNum;
    private float limitDistance;

    [Header("���W��ۑ��������")]
    [SerializeField] float span = 0.1f;
    [Header("�q���ǂ������n�߂�܂łɂ����鎞��")]
    [SerializeField] float followDelay = 0.2f;

    private List<Vector3> playerPos = new List<Vector3>();
    [SerializeField] LineRenderer lineRenderer;

    public bool isTable_Customer;
    public Vector3 sitPos;

    float eatDesire = 0;
    public bool eatNow = false;
    [Header("�q���H���\�Ȏ��ɕ\������UI")]
    [SerializeField] CanvasGroup eatCanvas;
    [Header("�q���H�׏I���܂ł̎���")]
    public float eatTime;

    [Header("�q�����W�܂ōs���̂ɂ����鎞��")]
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

    //������ԁG
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
        //�w�肵��Path��exitsTime�b�Ői�s�����������Ȃ���ʂ�B
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
            Debug.Log("cusNum����������");
        }
    }
    private void Start()
    {
        //updateSpan�b�҂�����1�ڂ̍��W�Ɗp�x��ۑ����邽�߂ɁAupdateSpan�b�������Ă���
        plTimer = span;
        //������
        targetRotation = myTran.rotation;

        eatCanvas.alpha = 0;

    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case CustomerState.WAIT: //WAIT���̏���
                #region
                if (stateEnter)
                {
                    stateEnter = false;
                    Debug.Log("�ҋ@���");
                }

                if (followPlayer != null)
                {
                    ChangeState(CustomerState.FOLLOW);
                    return;
                }
                break;
            #endregion

            case CustomerState.FOLLOW:�@//FOLLOW���̏���
                #region
                if (stateEnter)
                {
                    stateEnter = false;
                    Debug.Log("�Ǐ]���");
                }

                if (followPlayer != null)
                {
                    //�v���C���[�̈ړ����͂��Ȃ��Ƃ��͍��W�̕ۑ����s��Ȃ�
                    if (followPlayer.input.sqrMagnitude != 0.0f)
                    {
                        plTimer += Time.deltaTime;
                        if (plTimer >= span)
                        {
                            plTimer = 0;

                            //�v���C���[�̍��W�����X�g�ɕۑ�
                            playerPos.Add(followPlayer.myTran.position);

                            lineRenderer.positionCount = playerPos.Count;
                            lineRenderer.SetPosition(playerPos.Count - 1, playerPos[playerPos.Count - 1]);
                        }
                    }
                    else
                    {
                        //�v���C���[�̑��삪�Ȃ��Ƃ��ɋq�p�̃^�C�}�[�����Z�b�g
                        myTimer = 0;
                    }

                    float distance = (followPlayer.myTran.position - myTran.position).sqrMagnitude;
                    if (playerPos.Count > 0 && distance > limitDistance)
                    {
                        myTimer += Time.deltaTime;
                        //�v���C���[���������Ƃ��ɂ����ɓ����ƕs���R�Ȃ̂ŁA�����x�点��
                        if (myTimer >= followDelay)
                        {
                            //�ړI�n���߂����Ȃ��Ƃ��́A���̖ړI�n�������p�x���v�Z����
                            if ((playerPos[currentFollowNum] - myTran.position).sqrMagnitude > 0.01f)
                                targetRotation = Quaternion.LookRotation(playerPos[currentFollowNum] - myTran.position);
                            //�w�肵����]���x�ŏ��X�Ɍv�Z�����p�x�ɕω����Ă���
                            myTran.rotation = Quaternion.RotateTowards(myTran.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                            //�߂�����Ƃ��͌������A�����Ƃ��͉�������σX�s�[�h(��̓v���C���[�̑��x)
                            var speed = followPlayer.speed * distance / limitDistance;
                            //�ړI�n�ɂ͉σX�s�[�h���g���Ĉړ�����
                            myTran.position = Vector3.MoveTowards(myTran.position, playerPos[currentFollowNum], Time.deltaTime * followPlayer.speed);

                            //�ړI�n���߂��Ȃ����玟�̖ړI�n�ɐ؂�ւ���
                            //�����������鋗���̐ݒ肪�߂�����ƁA�v���C���[�����̏�ł��邮��ړ������������̒ʂ蓮���Ă��܂��̂ŕs���R�ɂȂ�
                            //���̂��߂�����x����Ă��Ă��ړI�n�̐؂�ւ����s���A���R�Ȉړ���\������
                            if ((playerPos[currentFollowNum] - myTran.position).sqrMagnitude <= Mathf.Pow(0.5f, 2) && playerPos.Count > currentFollowNum + 1)
                                currentFollowNum++;
                        }
                    }
                    else
                    {
                        //�����Ń^�C�}�[�����Z�b�g����Ƌq�̓������J�N�J�N���邱�Ƃ�����̂ł����ł͂��Ȃ�
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
                            Debug.Log("myPos����������");
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

            case CustomerState.EAT: //EAT���̏���
                #region

                if (stateEnter)
                {
                    Debug.Log("����");
                    stateEnter = false;
                    eatDesire = 0;
                    eatCanvas.alpha = 1;
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    eatNow = true;
                    Debug.Log("�H�����");
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

            case CustomerState.EXITS: //EXITS���̏���
                #region

                if (stateEnter)
                {
                    StartCoroutine("DoPath");
                    stateEnter = false;
                    Debug.Log("�A����");
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
                Debug.Log("myPos����������");
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