# Msh.ParsGreen.Sms
A tiny library based on [ParsGreen](https://www.parsgreen.ir) RestAPI to send SMS for .NET core web application.

### Installation
`
Nuget
`

### Usage
- Application `startup.cs`
``` C#
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.Configure<ParsGreenConfiguration>(opt => Configuration.GetSection("SmsConfig").Bind(opt));
        services.AddParsGreen();
        ...
    }
```

- Application settings `appsettings.json`
``` json
  "SmsConfig": {
    "Token": "enter your token here",
    "MaxLenght": 60,
    "MciCoefficent": 44,
    "IrancellCoefficent": 55,
    "RightelCoefficent": 55,
    "DefaultNumber": "enter your default number" 
  }
```

- Controller `yourController.cs`
``` C#
    private readonly ISmsProvider _smsProvider;

    public HomeController(ISmsProvider smsProvider)
    {
        _smsProvider = smsProvider;
    }

    public async Task<IActionResult> SendSms()
    {
        var t = await _smsProvider.InvokeSendSmsAsync(new SendSmsModel
        {
            Body = "My API",
            ToNumber = "09017206191"
        });
        if (t.Succeeded)
        {
            //todo: some database jobs...
            return View(t.Result)
        };
        return BadRequest(t.Errors);
    }
```