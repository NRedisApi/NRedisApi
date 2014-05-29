NRedisApi
=========

Redis API built upon StackExchange.Redis to allow .Net style access to Redis data structures and the creation of &amp; searching against indicies

So far the solution includes some initial experiments with StackExchange.Redis and accompanying Unit Tests. 

It also includes a Windows Service Project that will allow the running of the MS OpenTech build of Redis (also provided as a ZIP within a Solution Folder.)

There is a setting in the app.config of the windows svc that specifies the folder where the redis exe lives – change that to the right location for your exe (it is currently c:\redis server – if you unpack that ZIP on C:\ you won’t need to change any settings) then build the windows service in release and install it using VS command line tool (google install Windows Service if you haven’t done it before.)

It will then be installed, set to start automatically. Redis will be on 127.0.0.1/localhost and port 6379. In the redis server folder there is a little console exe called redis-cli.exe that you can use to connect to your Redis instance and directly issue redis commands (first type “ping” and expect to see “PONG” returned, then try “set urn:hello hi” then “get urn:hello” – it should return “hi”.)

Also included is the ServiceStack (browser-based) RedisAdminUI as a ZIP in a Solution Folder.

To install the web admin UI just unzip the folder into c:\inetpub\wwwroot and add it as a web application from IIS. Remember to change the app pool from the default to a .Net 2.0 app pool, otherwise it won’t work – IIS should have one available or just set one up. Give it alias “RedisAdminUI” and access it on http://localhost/ RedisAdminUI then you can look at whatever keys/values are in your Redis instance.

Do that and you will have Redis running as a Windows Svc on Windows, with a (basic) Admin UI - an ideal Database Sandbox for testing.

