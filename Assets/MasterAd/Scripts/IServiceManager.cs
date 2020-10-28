public interface IServiceManager
{
    /// <summary>
    /// Adds an Ad Service for further use
    /// </summary>
    /// <param name="adService"></param>
    void AddService(IAdService adService);

    /// <summary>
    /// Whenever an Ad Service fails to load ad, this method switches the service
    /// </summary>
    void SwitchService();

    /// <summary>
    /// It returns the current using service
    /// </summary>
    /// <returns></returns>
    IAdService GetService();
}