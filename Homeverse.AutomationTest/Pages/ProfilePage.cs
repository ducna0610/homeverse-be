using OpenQA.Selenium;

namespace Homeverse.AutomationTest.Pages;

public class ProfilePage : BasePage
{
    private readonly By _userName = By.Name("userName");
    private readonly By _phone = By.Name("phone");
    private readonly By _password = By.Name("password");
    private readonly By _newPassword = By.Name("newPassword");
    private readonly By _confirmNewPassword = By.Name("confirmNewPassword");

    public ProfilePage(IWebDriver driver) : base(driver)
    {
    }

    public void SetUserName(string userName)
    {
        SendKeys(_userName, userName);
    }

    public void SetPhone(string phone)
    {
        SendKeys(_userName, phone);
    }

    public void SetPassword(string password)
    {
        SendKeys(_password, password);
    }

    public void SetNewPassword(string newPassword)
    {
        SendKeys(_newPassword, newPassword);
    }

    public void SetConfirmNewPassword(string confirmNewPassword)
    {
        SendKeys(_confirmNewPassword, confirmNewPassword);
    }
}
