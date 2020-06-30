using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 抽象类无法被实例化，只能当成更多类的父类
/// </summary>
public abstract class IUserInput : MonoBehaviour
{
    [Header("===== Output Signals =====")]
    public float Dup;
    public float Dright;
    public float Dmag;
    public Vector3 Dvec;//人物朝向
    public float Jup;//cameras使用
    public float Jright;
    //pressing signal
    public bool run;
    public bool defense;
    //trigger signal
    public bool jump;//按下
    public bool attack;
    //double trigger
    public bool roll;
    public bool lockon;

    [Header("===== Others =====")]
    [HideInInspector]
    public bool inputEnabled = true;//Flag
    protected float targetDup;
    protected float targetDright;
    protected float velocityDup;//中间值不用管是多少
    protected float velocityDright;

    /// <summary>
    /// 修正函数，斜向也是速度为固定
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);
        return output;
    }
}
