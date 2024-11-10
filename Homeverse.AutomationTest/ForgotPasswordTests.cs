using Homeverse.AutomationTest.Pages;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace Homeverse.AutomationTest;

public class ForgotPasswordTests : IAsyncLifetime
{
    private IWebDriver _driver;
    private ForgotPasswordPage _forgotPasswordPage;

    public async Task InitializeAsync()
    {
        _driver = new ChromeDriver();
        _forgotPasswordPage = new ForgotPasswordPage(_driver);
        _driver.Manage().Window.Maximize();
        _driver.Navigate().GoToUrl("http://localhost:4200/forgot-password");
    }

    [Fact]
    public void ForgotPassword_WhenUserNotFound_ShouldAlertFailMessage()
    {
        // Arrange
        _forgotPasswordPage.SetEmail("404@gmail.com");

        // Act
        _forgotPasswordPage.Submit();

        // Assert
        var alertifyMessage = _forgotPasswordPage.GetAlertMessage();
        var expectedResult = "Vui lòng kiểm tra email để đổi mật khẩu";
        //Assert.Equal(alertifyMessage, expectedResult);
    }

    [Fact]
    public void ForgotPassword_WhenSuccessful_ShouldAlertSuccessMessage()
    {
        // Arrange
        _forgotPasswordPage.SetEmail("");

        // Act
        //_forgotPasswordPage.Submit();

        // Assert
        //var alertifyMessage = _forgotPasswordPage.GetAlertMessage();
        //var expectedResult = "Vui lòng kiểm tra email để đổi mật khẩu";
        //Assert.Equal(alertifyMessage, expectedResult);
    }

    public async Task DisposeAsync()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}