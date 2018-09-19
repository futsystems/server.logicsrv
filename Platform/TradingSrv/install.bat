%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe LogicService.exe
Net Start ServiceBC
sc config ServiceBC start= auto