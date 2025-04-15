using BidMasterOnline.Application.DTO;

namespace BidMasterOnline.Application.ServiceContracts
{
    /// <summary>
    /// Service for managing the complaints.
    /// </summary>
    public interface IComplaintsService
    {
        /// <summary>
        /// Gets complaints list with applyed specifications.
        /// </summary>
        /// <param name="specifications">Specifications of complaint type, date, sorting and pagination.</param>
        /// <returns>List of complaints with applyed specifications.</returns>
        Task<ListModel<ComplaintDTO>> GetComplaintsListAsync(ComplaintSpecificationsDTO specifications);

        /// <summary>
        /// Gets the specified complaint by it`s identifier.
        /// </summary>
        /// <param name="id">Identifier of complaint to get.</param>
        /// <returns>Complaint with passed identifier.</returns>
        Task<ComplaintDTO> GetComplaintByIdAsync(Guid id);

        /// <summary>
        /// Sets new complaint of any type.
        /// </summary>
        /// <param name="complaint">Complaint to set.</param>
        Task SetNewComplaintAsync(SetComplaintDTO complaint);

        /// <summary>
        /// Set the status to the complaint as handled.
        /// </summary>
        /// <param name="id">Identifier of complaint to handle.</param>
        Task HandleComplaintAsync(Guid id);
    }
}
