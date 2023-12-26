using System.Collections.Generic;
public class Score
{
    /// <summary>
    /// 测试账号
    /// </summary>
    public string nickName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int score { get; set; }
}


public class BestScoreInfo
{
    /// <summary>
    /// 
    /// </summary>
    public string status { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int code { get; set; }
    /// <summary>
    /// 请求成功
    /// </summary>
    public string info { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int data { get; set; }
}
 
public class ScoreInfo
{
    /// <summary>
    /// 
    /// </summary>
    public string status { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int code { get; set; }
    /// <summary>
    /// 请求成功
    /// </summary>
    public string info { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<Score> data { get; set; }
}
