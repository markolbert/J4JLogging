# J4JLogging

I'm a huge fan of [Serilog](https://serilog.net/) and use it in all of my C# work. But I wanted to be able to annotate the log messages with source code information because that's useful during debugging and I wanted to be able to send SMS messages about some log events.

**J4JLogging** is my approach to doing both those things. It's a simple wrapper for Serilog which makes it easy to include caller information, source code information and sending text messages via Twilio for selected log events.
