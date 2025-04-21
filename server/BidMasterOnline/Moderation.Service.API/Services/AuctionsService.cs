using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage;
using Moderation.Service.API.DTO;
using Moderation.Service.API.ServiceContracts;

namespace Moderation.Service.API.Services
{
    public class AuctionsService : IAuctionsService
    {
        private readonly IModerationLogger _moderationLogger;
        private readonly IAuctionsClient _client;
        private readonly ITransactionsService _transactionsService;
        private readonly ILogger<AuctionsService> _logger;

        public AuctionsService(IModerationLogger moderationLogger,
            IAuctionsClient client,
            ITransactionsService transactionsService,
            ILogger<AuctionsService> logger)
        {
            _moderationLogger = moderationLogger;
            _client = client;
            _transactionsService = transactionsService;
            _logger = logger;
        }

        public async Task<ServiceResult> ApproveAuctionRequestAsync(ApproveAuctionRequestDTO requestDTO)
        {
            ServiceResult result = new();

            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                ServiceResult logResult = await _moderationLogger.LogAction(ModerationAction.ApprovingAuctionRequest,
                    auctionRequestId: requestDTO.AuctionRequestId);

                if (!logResult.IsSuccessfull)
                {
                    await transaction.RollbackAsync();
                    result.IsSuccessfull = false;
                    result.StatusCode = logResult.StatusCode;
                    result.Errors = logResult.Errors;
                    return logResult;
                }

                bool auctionActionResult = await _client.ApproveAuctionRequestAsync(requestDTO.AuctionRequestId);

                if (!auctionActionResult)
                {
                    await transaction.RollbackAsync();
                    result.IsSuccessfull = false;
                    result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    result.Errors.Add("Could not approve this auction request.");
                    return result;
                }

                await transaction.CommitAsync();

                result.Message = "Auction request has been successfully approved!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred during processing a request.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("Something went wrong during processing your request. Please, try again!");
            }

            return result;
        }

        public async Task<ServiceResult> DeclineAuctionRequestAsync(DeclineAuctionRequestDTO requestDTO)
        {
            ServiceResult result = new();

            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                ServiceResult logResult = await _moderationLogger.LogAction(ModerationAction.DecliningAuctionRequest,
                    auctionRequestId: requestDTO.AuctionRequestId);

                if (!logResult.IsSuccessfull)
                {
                    await transaction.RollbackAsync();
                    result.IsSuccessfull = false;
                    result.StatusCode = logResult.StatusCode;
                    result.Errors = logResult.Errors;
                    return logResult;
                }

                bool auctionActionResult = await _client.DeclineAuctionRequestAsync(requestDTO.AuctionRequestId, 
                    requestDTO.Reason);

                if (!auctionActionResult)
                {
                    await transaction.RollbackAsync();
                    result.IsSuccessfull = false;
                    result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    result.Errors.Add("Could not decline this auction request.");
                    return result;
                }

                await transaction.CommitAsync();

                result.Message = "Auction request has been successfully declined!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred during processing a request.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("Something went wrong during processing your request. Please, try again!");
            }

            return result;
        }

        public async Task<ServiceResult> CancelAuctionAsync(CancelAuctionDTO requestDTO)
        {
            ServiceResult result = new();

            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                ServiceResult logResult = await _moderationLogger.LogAction(ModerationAction.CancelingAuction,
                    auctionId: requestDTO.AuctionId);

                if (!logResult.IsSuccessfull)
                {
                    await transaction.RollbackAsync();
                    result.IsSuccessfull = false;
                    result.StatusCode = logResult.StatusCode;
                    result.Errors = logResult.Errors;
                    return logResult;
                }

                bool auctionActionResult = await _client.CancelAuctionAsync(requestDTO.AuctionId, requestDTO.Reason);

                if (!auctionActionResult)
                {
                    await transaction.RollbackAsync();
                    result.IsSuccessfull = false;
                    result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    result.Errors.Add("Could not cancel this auction.");
                    return result;
                }

                await transaction.CommitAsync();

                result.Message = "Auction has been successfully cancelled!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred during processing a request.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("Something went wrong during processing your request. Please, try again!");
            }

            return result;
        }

        public async Task<ServiceResult> RecoverAuctionAsync(RecoverAuctionDTO requestDTO)
        {
            ServiceResult result = new();

            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                ServiceResult logResult = await _moderationLogger.LogAction(ModerationAction.RecoveringAuction,
                    auctionId: requestDTO.AuctionId);

                if (!logResult.IsSuccessfull)
                {
                    await transaction.RollbackAsync();
                    result.IsSuccessfull = false;
                    result.StatusCode = logResult.StatusCode;
                    result.Errors = logResult.Errors;
                    return logResult;
                }

                bool auctionActionResult = await _client.RecoverAuctionAsync(requestDTO.AuctionId);

                if (!auctionActionResult)
                {
                    await transaction.RollbackAsync();
                    result.IsSuccessfull = false;
                    result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    result.Errors.Add("Could not recover this auction.");
                    return result;
                }

                await transaction.CommitAsync();

                result.Message = "Auction has been successfully recovered!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred during processing a request.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("Something went wrong during processing your request. Please, try again!");
            }

            return result;
        }
    }
}
