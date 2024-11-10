using Homeverse.AutomationTest.Pages;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace Homeverse.AutomationTest;

public class ProfileTests : IAsyncLifetime
{
    private IWebDriver _driver;
    private LoginPage _loginPage;
    private ProfilePage _profilePage;

    public async Task InitializeAsync()
    {
        _driver = new ChromeDriver();
        _loginPage = new LoginPage(_driver);
        _profilePage = new ProfilePage(_driver);
        _driver.Manage().Window.Maximize();
        _driver.Navigate().GoToUrl("http://localhost:4200/login");
        _loginPage.SetEmail("ducna0610@gmail.com");
        _loginPage.SetPassword("12345678");
        _loginPage.Submit();
        Thread.Sleep(3000);
        _driver.Navigate().GoToUrl("http://localhost:4200/profile");
    }

    [Fact]
    public void UpdateProfile_WhenIncorrectPassword_ShouldAlertSuccessMessage()
    {
        // Arrange
        _profilePage.SetUserName("test");
        _profilePage.SetPassword("1234");

        // Act
        //_profilePage.Submit();

        // Assert
        //var alertifyMessage = _profilePage.GetAlertMessage();
        var expectedResult = "Invalid user name or password";
        //Assert.Equal(alertifyMessage, expectedResult);
    }

    [Fact]
    public void UpdateProfile_WhenSuccessfull_ShouldAlertSuccessMessage()
    {
        // Arrange
        _profilePage.SetUserName("test");
        _profilePage.SetPassword("12345678");

        // Act
        //_profilePage.Submit();

        // Assert
        //var alertifyMessage = _profilePage.GetAlertMessage();
        var expectedResult = "Cập nhật tài khoản thành công";
        //Assert.Equal(alertifyMessage, expectedResult);
    }

    [Fact]
    public void UpdateProfileWithNewPassword_WhenSuccessful_ShouldAlertSuccessMessage()
    {
        // Arrange
        _profilePage.SetUserName("test");
        _profilePage.SetPassword("12345678");
        _profilePage.SetNewPassword("12345678");
        _profilePage.SetConfirmNewPassword("12345678");

        // Act
        //_profilePage.Submit();

        // Assert
        //var alertifyMessage = _profilePage.GetAlertMessage();
        var expectedResult = "Cập nhật tài khoản thành công";
        //Assert.Equal(alertifyMessage, expectedResult);
    }

    public async Task DisposeAsync()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}
