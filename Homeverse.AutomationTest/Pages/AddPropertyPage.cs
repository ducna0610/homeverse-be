using OpenQA.Selenium;

namespace Homeverse.AutomationTest.Pages;

public class AddPropertyPage : BasePage
{
    private readonly By _title = By.Name("title");
    private readonly By _price = By.Name("price");
    private readonly By _area = By.Name("area");
    private readonly By _category = By.Name("category");
    private readonly By _furnish = By.Name("furnish");
    private readonly By _city = By.Name("city");
    private readonly By _isActive = By.Name("isActive");
    private readonly By _address = By.Name("address");
    private readonly By _description = By.Name("description");

    public AddPropertyPage(IWebDriver driver) : base(driver)
    {
    }

    public void SetTitle(string title)
    {
        SendKeys(_title, title);
    }

    public void SetPrice(string price)
    {
        SendKeys(_price, price);
    }

    public void SetArea(string area)
    {
        SendKeys(_area, area);
    }

    public void SetCategory(string category)
    {
        SendKeys(_category, category);
    }

    public void SetFurnish(string furnish)
    {
        SendKeys(_furnish, furnish);
    }

    public void SetCity(string city)
    {
        SendKeys(_city, city);
    }

    public void SetIsActive(string isActive)
    {
        SendKeys(_isActive, isActive);
    }

    public void SetAddress(string address)
    {
        SendKeys(_address, address);
    }

    public void SetDescription(string description)
    {
        SendKeys(_description, description);
    }
}
