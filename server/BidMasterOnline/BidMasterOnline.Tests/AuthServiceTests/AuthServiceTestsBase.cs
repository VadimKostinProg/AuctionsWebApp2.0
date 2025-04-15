using BidMasterOnline.Application.ServiceContracts;
using BidMasterOnline.Application.Services;
using BidMasterOnline.Application.Sessions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BidMasterOnline.Tests.AuthServiceTests
{
    public class AuthServiceTestsBase : ServiceTestsBase
    {
        protected readonly IAuthService service;
        protected readonly Mock<IConfiguration> configurationMock;
        protected readonly SessionContext sessionContext;

        public AuthServiceTestsBase()
        {
            configurationMock= new Mock<IConfiguration>();
            sessionContext= new SessionContext();

            service = new AuthService(this.repositoryMock.Object, this.configurationMock.Object, 
                this.sessionContext);
        }
    }
}
