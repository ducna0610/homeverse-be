using OpenQA.Selenium;

namespace Homeverse.AutomationTest.Pages;

public class LoginPage : BasePage
{
    private readonly By _email = By.Name("email");
    private readonly By _password = By.Name("password");

    public LoginPage(IWebDriver driver) : base(driver)
    {
    }

    public void SetEmail(string email)
    {
        SendKeys(_email, email);
    }

    public void SetPassword(string password)
    {
        SendKeys(_password, password);
    }
}
