using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebCalculator.Models;
using NCalc;

namespace WebCalculator.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public ActionResult Index(string display)
    {
        if (display == null)
        {
            display = "";
        }

        ViewBag.Display = display;

        return View();
    }

    [HttpPost]
    public ActionResult AddToDisplay(string display, string buttonValue, string operation)
    {
        if (!string.IsNullOrEmpty(buttonValue))
        {
            display += buttonValue;
        }
        else
        {
            switch (operation)
            {
                case "addValue":
                    display += "+";
                    break;
                case "subValue":
                    display += "-";
                    break;
                case "multValue":
                    display += "*";
                    break;
                case "divValue":
                    display += "/";
                    break;
            }
        }
        return RedirectToAction("Index", new { display = display });
    }

    [HttpPost]
    public ActionResult Calculate(string display)
    {
        double sum = CalculateResult(display);

        return Json(new { result = sum });
    }

    private double CalculateResult(string display)
    {
        string strDisplay = display;

        // Check if the string contains letters or is empty
        if (string.IsNullOrWhiteSpace(strDisplay) || !IsValidExpression(strDisplay))
        {
            // If the string contains letters or is empty, reload the index page
            return double.NaN;
        }

        // Check if the expression is valid
        try
        {
            Expression e = new Expression(strDisplay);
            object result = e.Evaluate();

            double parsedResult;
            if (result != null && double.TryParse(result.ToString(), out parsedResult))
            {
                return parsedResult;
            }
            else
            {
                return double.NaN;
            }
        }
        catch (Exception)
        {
            // If an error occurs while evaluating the expression, reload the index page
            return double.NaN;
        }
    }

    private bool IsValidExpression(string expression)
    {
        for (int i = 0; i < expression.Length - 1; i++)
        {
            if (IsOperator(expression[i]) && IsOperator(expression[i + 1]))
            {
                return false;
            }
        }
        return true;
    }

    private bool IsOperator(char c)
    {
        return c == '+' || c == '-' || c == '*' || c == '/';
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}