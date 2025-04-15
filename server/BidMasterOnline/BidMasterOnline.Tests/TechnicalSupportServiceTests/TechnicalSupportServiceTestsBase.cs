using BidMasterOnline.Application.ServiceContracts;
using BidMasterOnline.Application.Services;
using Moq;

namespace BidMasterOnline.Tests.TechnicalSupportRequestsServiceTests
{
    /// <summary>
    /// Base test class for TechnicalSupportRequestsService unit tests.
    /// </summary>
    public abstract class TechnicalSupportRequestsServiceTestsBase : ServiceTestsBase
    {
        protected readonly ITechnicalSupportRequestsService service;
        protected readonly Mock<IAuthService> authServiceMock;

        public TechnicalSupportRequestsServiceTestsBase()
        {
            authServiceMock = new Mock<IAuthService>();

            service = new TechnicalSupportRequestsService(repositoryMock.Object, authServiceMock.Object);
        }
    }
}
