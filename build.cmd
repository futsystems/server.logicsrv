%SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319\msbuild /target:PublishClean  build.proj
%SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319\msbuild /target:Clean  build.proj

%SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319\msbuild /target:Base  build.proj
%SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319\msbuild /target:Server  build.proj

%SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319\msbuild /target:Connector  build.proj
%SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319\msbuild /target:RuleSet  build.proj
%SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319\msbuild /target:ExServer  build.proj

%SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319\msbuild /target:Publish  build.proj



