using System;
using System.Collections.Generic;

public interface IAdService
{
    /// <summary>
    /// It stores all ad tokens with their corresponding names
    /// </summary>
    Dictionary<string, string> tokens { get; set; }

    /// <summary>
    /// You should fill this method when requesting to load, and call it when ad loaded
    /// </summary>
    Action onLoaded { get; set; }

    /// <summary>
    /// You should fill this method when requesting to load, and call it when ad failed to load
    /// </summary>
    Action onFailed { get; set; }

    /// <summary>
    /// You should fill this method when requesting to Show, and call it when ad finished showing
    /// </summary>
    Action onFinished { get; set; }

    /// <summary>
    /// You should add tokens inside this method using AddToken 
    /// </summary>
    void Init();

    /// <summary>
    /// You should add new Tokens to tokens dictionary here with additional implementation of desired ad platform
    /// </summary>
    void AddToken(string name, string token);

    /// <summary>
    /// You should put your ad platform specific load implementation here
    /// </summary>
    void Load(string name, Action onLoaded, Action onFailed);

    /// <summary>
    /// You should put your ad platform specific Readiness implementation here
    /// </summary>
    bool Ready(string name);

    /// <summary>
    /// You should put your ad platform specific Show implementation here
    /// </summary>
    void Show(string name, Action onFinished);

    /// <summary>
    /// You should put your ad platform specific Destroy implementation here
    /// </summary>
    void Destroy(string name);
}