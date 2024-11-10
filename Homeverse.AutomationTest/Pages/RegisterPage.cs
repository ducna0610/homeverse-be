using OpenQA.Selenium;

namespace Homeverse.AutomationTest.Pages;

public class RegisterPage : BasePage
{
    private readonly By _userName = By.Name("userName");
    private readonly By _email = By.Name("email");
    private readonly By _phone = By.Name("phone");
    private readonly By _password = By.Name("password");
    private readonly By _confirmPassword = By.Name("confirmPassword");

    public RegisterPage(IWebDriver driver) : base(driver)
    {
    }

    public void SetUserName(string userName)
    {
        SendKeys(_userName, userName);
    }

    public void SetEmail(string email)
    {
        SendKeys(_email, email);
    }

    public void SetPhone(string phone)
    {
        SendKeys(_phone, phone);
    }

    public void SetPassword(string password)
    {
        SendKeys(_password, password);
    }

    public void SetConfirmtPassword(string confirmPassword)
    {
        SendKeys(_confirmPassword, confirmPassword);
    }
}
