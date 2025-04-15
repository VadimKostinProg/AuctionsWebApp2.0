using BidMasterOnline.Application.DTO;

namespace BidMasterOnline.Application.ServiceContracts
{
    /// <summary>
    /// Service for managing technical support requests.
    /// </summary>
    public interface ITechnicalSupportRequestsService
    {
        /// <summary>
        /// Gets the list of the technical support request.
        /// </summary>
        /// <param name="specifications">Specifications for filtering and pagination technical requests.</param>
        /// <returns></returns>
        Task<ListModel<TechnicalSupportRequestDTO>> GetTechnicalSupportRequestsListAsync(TechnicalSupportRequestSpecificationsDTO specifications);

        /// <summary>
        /// Gets the specified technical support request by it`s identifier.
        /// </summary>
        /// <param name="id">Identifier of the technical support request.</param>
        /// <returns>Technical support request with the passed id.</returns>
        Task<TechnicalSupportRequestDTO> GetTechnicalSupportRequestByIdAsync(Guid id);

        /// <summary>
        /// Sets new technical request.
        /// </summary>
        /// <param name="request">Technical support request to set.</param>
        Task SetTechnicalSupportRequestAsync(SetTechnicalSupportRequestDTO request);

        /// <summary>
        /// Sets the specified technical request as handled.
        /// </summary>
        /// <param name="id">Identifier of the technical support request to handle.</param>
        Task HandleTechnicalSupportRequestAsync(Guid id);
    }
}
