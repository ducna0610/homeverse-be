using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Homeverse.SeleniumTest;

[TestFixture]
public class UserE2ETests
{
    private IWebDriver driver;
    private string frontendUrl;

    [SetUp]
    public void Setup()
    {
        frontendUrl = "http://localhost:4200";

        ChromeOptions options = new ChromeOptions();
        options.AddArgument("start-maximized");
        driver = new ChromeDriver(options);
    }

    [Test]
    public void RegisterUser_ShouldSucceed_WithValidData()
    {
        // Navigate to the URL
        driver.Navigate().GoToUrl($"{frontendUrl}/register");

        // Find elements
        driver.FindElement(By.Name("userName")).SendKeys("testuser");
        driver.FindElement(By.Name("email")).SendKeys("testuser@example.com");
        driver.FindElement(By.Name("phone")).SendKeys("0123456789");
        driver.FindElement(By.Name("password")).SendKeys("Test@1234");
        driver.FindElement(By.Name("confirmPassword")).SendKeys("Test@1234");

        // Click the registration button
        driver.FindElement(By.Name("submit")).Click();

        // Create a wait instance for explicit wait
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

        // Wait for the alertify success message
        IWebElement? successMessage = wait.Until(d =>
        {
            try
            {
                // Locate the success message by text
                return d.FindElement(By.XPath("//*[contains(text(), 'Chúc mừng bạn đã tạo tài khoản thành công!')]"));
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        });

        // Assert that the success message is displayed
        Assert.IsTrue(successMessage?.Displayed, "Success message was not displayed.");
    }

    [TearDown]
    public void TearDown()
    {
        driver.Dispose();
    }
}
