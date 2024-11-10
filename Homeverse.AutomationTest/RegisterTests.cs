using Homeverse.AutomationTest.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Homeverse.AutomationTest;

public class RegisterTests : IAsyncLifetime
{
    private IWebDriver _driver;
    private RegisterPage _registerPage;

    public async Task InitializeAsync()
    {
        _driver = new ChromeDriver();
        _registerPage = new RegisterPage(_driver);
        _driver.Manage().Window.Maximize();
        _driver.Navigate().GoToUrl("http://localhost:4200/register");
    }

    [Fact]
    public void Register_WhenUserAlreadyExist_ShouldAlertFailMessage()
    {
        // Arrange
        _registerPage.SetUserName("test");
        _registerPage.SetEmail("ducna0610@gmail.com");
        _registerPage.SetPhone("0123456789");
        _registerPage.SetPassword("password");
        _registerPage.SetConfirmtPassword("password");

        // Act
        _registerPage.Submit();

        // Assert
        var alertifyMessage = _registerPage.GetAlertMessage();
        var expectedResult = "Email already exists, please try something else";
        Assert.Equal(alertifyMessage, expectedResult);
    }

    [Fact]
    public void Register_WhenSuccessful_ShouldAlertSuccessMessage()
    {
        // Arrange
        _registerPage.SetUserName("test");
        _registerPage.SetEmail("test@gmail.com");
        _registerPage.SetPhone("0123456789");
        _registerPage.SetPassword("password");
        _registerPage.SetConfirmtPassword("password");

        // Act
        _registerPage.Submit();

        // Assert
        var alertifyMessage = _registerPage.GetAlertMessage();
        var expectedResult = "Chúc mừng bạn đã tạo tài khoản thành công!";
        Assert.Equal(alertifyMessage, expectedResult);
    }

    public async Task DisposeAsync()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}