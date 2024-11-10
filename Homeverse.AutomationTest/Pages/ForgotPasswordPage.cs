using OpenQA.Selenium;

namespace Homeverse.AutomationTest.Pages;

public class ForgotPasswordPage : BasePage
{
    private readonly By _email = By.Name("email");

    public ForgotPasswordPage(IWebDriver driver) : base(driver)
    {
    }

    public void SetEmail(string email)
    {
        SendKeys(_email, email);
    }
}
