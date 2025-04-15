using BidMasterOnline.Application.ServiceContracts;
using BidMasterOnline.Application.Services;
using Moq;

namespace BidMasterOnline.Tests.UserManagerTests
{
    /// <summary>
    /// Base test class for UserManager unit tests.
    /// </summary>
    public class UserManagerTestsBase : ServiceTestsBase
    {
        protected readonly IUserManager service;
        protected readonly Mock<IImagesService> imagesServiceMock;
        protected readonly Mock<IAuthService> authServiceMock;

        public UserManagerTestsBase()
        {
            imagesServiceMock = new Mock<IImagesService>();
            authServiceMock = new Mock<IAuthService>();

            service = new UserManager(repositoryMock.Object, imagesServiceMock.Object, authServiceMock.Object);
        }
    }
}
