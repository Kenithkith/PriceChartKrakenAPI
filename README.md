# Price Chart Displayer based on Kraken API
A Windows app to display cryptocurrency/coin price chart, using KraKen API.
![image](https://github.com/StevenNLWu/PriceChartKrakenAPI/blob/master/screenCap.png)
***
## Introduction
This is a Windows WPF app to display coin price chart.  <br/> 
<br/>
For the price chart, the app provides both linear diagram and logarithmic diagram. <br/>
For the logarithmic diagram, user can enter the log-base number according to his/er needs. <br/> 
<br/>
All tradable assets and price info are based on KraKen data; the app calls KraKen API, receiving its data and finally generates and displays the corresponding chart.
## Pre-Requirements
To run the app, one needs: <br/>
* .NET Framework 4.6.1 <br/>
* A KraKen API Key with *'Export Data'* permission <br/>

To run the source, it has been recommended to use *Microsoft Visual Studio 2017*.<br/>
## Quick Start
Make sure you have a KraKen API key firstly. <br/>
The key should, at least, has the *'Export Data'* permission. <br/>
<br/>
edit the file of *`build\App.exe`*, and then import the key value to the file, i.e. <br/>
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.6.1" />
    </startup>

  <appSettings>
    <add key="KrakenBaseAddress" value="https://api.kraken.com"/>
    <add key="KrakenApiVersion" value="0"/>
    <add key="KrakenKey" value= "<API Key>"/>       <!-- replace the value by yourself -->
    <add key="KrakenSecret" value="<Private Key>"/> <!-- replace the value by yourself -->
  </appSettings>

</configuration>
```
To run the app, just mouse-clicking the following file: <br/>
* build\app 

To run the source, just  mouse-clicking the following file using *Microsoft Visual Studio 2017*: <br/>
* src\src

And remember to add the key value in source, by creating the file of  *`src\App.config`*.

## Folder structure
- `build` - quick start app <br/>
- `src` - source code <br/>
