using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SLGUnit : MonoBehaviour
{
    enum StateEnum
    {
        ����,
        Ԥѡ,
        ѡ��
    }



    public SLGCell StandingCell { get; private set; }
    Outline _selfOutLine;
    StateEnum _currentState;
    Animator _selfAnim;
    AudioSource _as;


    // Start is called before the first frame update
    void Start()
    {
        _as = GetComponent<AudioSource>();
        _selfOutLine = GetComponent<Outline>();
        _selfAnim = GetComponent<Animator>();
    }


    public void MoveTo(List<SLGCell> path)
    {
        StartCoroutine(MoveCor(path));
        
    }

    public void SetCell(SLGCell desCell)
    {
        transform.position = desCell.transform.position;
        StandingCell = desCell;
        StandingCell.SetUnit(this);
        StandingCell.BackToNormal();
    }


    IEnumerator MoveCor(List<SLGCell> path)
    {
        _selfAnim.SetBool("Walk", true);
        while (path.Count > 0)
        {
            var cell = path[0];
            Vector3 originPos = transform.position;
            Quaternion originRot = transform.rotation;
            Vector3 lookPos = cell.transform.position;
            lookPos.y = transform.position.y;
            GameObject tmp = new GameObject();
            tmp.transform.position = transform.position;
            tmp.transform.LookAt(lookPos);
            Quaternion desRot = tmp.transform.rotation;
            Destroy(tmp);

            float workTime = 0;
            while (true)
            {
                workTime += Time.deltaTime;
                transform.position = Vector3.Lerp(originPos, cell.transform.position, workTime);
                transform.rotation = Quaternion.Lerp(originRot, desRot, workTime * 2);
                if (workTime >= 1)
                {
                    break;
                }
                yield return null;
            }


            if (StandingCell!=cell)
            {
                StandingCell.Clean();
            }

            StandingCell = cell;

            StandingCell.SetUnit(this);

            path.RemoveAt(0);


        }
        _selfAnim.SetBool("Walk", false);
        SLGMouse.Instance.SwitchState(StateSortEnum.ѡȡ��ɫ);
        SLGMap.Instance.CleamPath();

    }



    public void PreSelect()
    {
        if (_currentState != StateEnum.ѡ��)
        {
            _currentState = StateEnum.Ԥѡ;
        }
    }

    public void Select()
    {
        _as.Play();
        _currentState = StateEnum.ѡ��;
    }

    public void Idle(bool force=false)
    {
        if (_currentState != StateEnum.ѡ�� || force)
        {
            _currentState = StateEnum.����;
        }
    }


    // Update is called once per frame
    void Update()
    {
        switch (_currentState)
        {
            case StateEnum.����:
                Idling();
                break;
            case StateEnum.Ԥѡ:
                Preselecting();
                break;
            case StateEnum.ѡ��:
                Selecting();
                break;

        }
    }

    private void Selecting()
    {
        _selfOutLine.enabled = true;
        _selfOutLine.OutlineColor = Color.green;
    }

    private void Preselecting()
    {
        _selfOutLine.enabled = true;
        _selfOutLine.OutlineColor = Color.white;

    }

    private void Idling()
    {
        _selfOutLine.enabled = false;
    }
}
