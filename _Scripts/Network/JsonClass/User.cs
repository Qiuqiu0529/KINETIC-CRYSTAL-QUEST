public class UserData
{
        /// <summary>
    /// 
    /// </summary>
    public string nickName { get; set; }
    /// <summary>
    /// 未知
    /// </summary>
    public string province { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string userId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int sex { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int height { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int weight { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int experience { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string admin { get; set; }
}

public class LoginInfo
{
    public string status { get; set; }
    public int code { get; set; }
    public string info { get; set; }
    public UserData data { get; set; }
}

public class Data
{
}

public class NullDatadInfo
{
    public string status { get; set; }
    public int code { get; set; }
    public string info { get; set; }
    public Data data { get; set; }
}

