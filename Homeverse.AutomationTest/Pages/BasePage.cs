using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Homeverse.AutomationTest.Pages;

public class BasePage
{
    private readonly IWebDriver _driver;
    private readonly By _submitButton = By.Name("submit");
    private readonly By _alertifyMessage = By.CssSelector(".ajs-message");

    protected BasePage(IWebDriver driver)
    {
        _driver = driver;
    }

    protected void WaitUntilElementVisible(By by)
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(60));
        wait.Until(ExpectedConditions.ElementToBeClickable(by));
    }

    protected IWebElement GetElement(By by)
    {
        WaitUntilElementVisible(by);
        return _driver.FindElement(by);
    }

    protected void Click(By by)
    {
        WaitUntilElementVisible(by);
        _driver.FindElement(by).Click();
    }

    protected void SendKeys(By by, string text)
    {
        WaitUntilElementVisible(by);
        _driver.FindElement(by).SendKeys(text);
    }

    public string GetAlertMessage()
    {
        return GetElement(_alertifyMessage).Text;
    }

    public void Submit()
    {
        Click(_submitButton);
    }
}
