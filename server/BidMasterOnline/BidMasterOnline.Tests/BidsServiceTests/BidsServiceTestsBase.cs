using BidMasterOnline.Application.ServiceContracts;
using BidMasterOnline.Application.Services;
using Moq;

namespace BidMasterOnline.Tests.BidsServiceTests
{
    /// <summary>
    /// Base test class for BidsService unit tests.
    /// </summary>
    public abstract class BidsServiceTestsBase : ServiceTestsBase
    {
        protected readonly IBidsService service;
        protected readonly Mock<IAuthService> authServiceMock;

        public BidsServiceTestsBase()
        {
            this.authServiceMock = new Mock<IAuthService>();

            service = new BidsService(this.repositoryMock.Object, authServiceMock.Object);
        }
    }
}
