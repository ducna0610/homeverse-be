using Homeverse.AutomationTest.Pages;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace Homeverse.AutomationTest;

public class LoginTests : IAsyncLifetime
{
    private IWebDriver _driver;
    private LoginPage _loginPage;

    public async Task InitializeAsync()
    {
        _driver = new ChromeDriver();
        _loginPage = new LoginPage(_driver);
        _driver.Manage().Window.Maximize();
        _driver.Navigate().GoToUrl("http://localhost:4200/login");
    }

    [Fact]
    public void Login_WhenFail_ShouldAlertFailMessage()
    {
        // Arrange
        _loginPage.SetEmail("ducna0610@gmail.com");
        _loginPage.SetPassword("1234");

        // Act
        _loginPage.Submit();

        // Assert
        var alertifyMessage = _loginPage.GetAlertMessage();
        var expectedResult = "Invalid user name or password";
        Assert.Equal(alertifyMessage, expectedResult);
    }

    [Fact]
    public void Login_WhenSuccessful_ShouldAlertSuccessMessage()
    {
        // Arrange
        _loginPage.SetEmail("ducna0610@gmail.com");
        _loginPage.SetPassword("12345678");

        // Act
        _loginPage.Submit();

        // Assert
        var alertifyMessage = _loginPage.GetAlertMessage();
        var expectedResult = "Đăng nhập thành công";
        Assert.Equal(alertifyMessage, expectedResult);
    }

    public async Task DisposeAsync()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}
