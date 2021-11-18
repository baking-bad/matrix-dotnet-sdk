namespace Matrix.Sdk.Sample.Console
{
    using System;
    using System.Collections.Generic;
    using Serilog.Sinks.SystemConsole.Themes;

    internal static class LoggerSetup
    {
        public static SystemConsoleTheme SetupTheme()
        {
            var customThemeStyles =
                new Dictionary<ConsoleThemeStyle, SystemConsoleThemeStyle>
                {
                    {
                        ConsoleThemeStyle.Text, new SystemConsoleThemeStyle
                        {
                            Foreground = ConsoleColor.Green
                        }
                    },
                    {
                        ConsoleThemeStyle.String, new SystemConsoleThemeStyle
                        {
                            Foreground = ConsoleColor.Blue
                        }
                    }
                };

            return new SystemConsoleTheme(customThemeStyles);
        }
    }
}