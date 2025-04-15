namespace BidMasterOnline.Application.ServiceContracts
{
    /// <summary>
    /// Service that provides method for execution in background periodic format.
    /// </summary>
    public interface IPeriodicTaskService
    {
        /// <summary>
        /// Perfoms the periodic task.
        /// </summary>
        Task PerformPeriodicTaskAsync();
    }
}
