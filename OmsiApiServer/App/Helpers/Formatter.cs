namespace OmsiApiServer.App.Helpers;

public static class Formatter
{
    public static string FormatUptime(double uptime)
    {
        TimeSpan t = TimeSpan.FromMilliseconds(uptime);

        if (t.Days > 0)
        {
            return $"{t.Days}d  {t.Hours}h {t.Minutes}m {t.Seconds}s";
        }
        else
        {
            return $"{t.Hours}h {t.Minutes}m {t.Seconds}s";
        }
    }
    
    public static string FormatUptime(TimeSpan t)
    {
        if (t.Days > 0)
        {
            return $"{t.Days}d  {t.Hours}h {t.Minutes}m {t.Seconds}s";
        }
        else
        {
            return $"{t.Hours}h {t.Minutes}m {t.Seconds}s";
        }
    }

    private static double Round(this double d, int decimals)
    {
        return Math.Round(d, decimals);
    }

    public static string FormatSize(long bytes)
    {
        var i = Math.Abs(bytes) / 1024D;
        if (i < 1)
        {
            return bytes + " B";
        }
        else if (i / 1024D < 1)
        {
            return i.Round(2) + " KB";
        }
        else if (i / (1024D * 1024D) < 1)
        {
            return (i / 1024D).Round(2) + " MB";
        }
        else
        {
            return (i / (1024D * 1024D)).Round(2) + " GB";
        }
    }

    public static string FormatDate(DateTime e)
    {
        string i2s(int i)
        {
            if (i.ToString().Length < 2)
                return "0" + i;
            return i.ToString();
        }
        
        return $"{i2s(e.Day)}.{i2s(e.Month)}.{e.Year} {i2s(e.Hour)}:{i2s(e.Minute)}";
    }
    
    public static string FormatDateOnly(DateTime e)
    {
        string i2s(int i)
        {
            if (i.ToString().Length < 2)
                return "0" + i;
            return i.ToString();
        }
        
        return $"{i2s(e.Day)}.{i2s(e.Month)}.{e.Year}";
    }

    public static string FormatSize(double bytes)
    {
        var i = Math.Abs(bytes) / 1024D;
        if (i < 1)
        {
            return bytes + " B";
        }
        else if (i / 1024D < 1)
        {
            return i.Round(2) + " KB";
        }
        else if (i / (1024D * 1024D) < 1)
        {
            return (i / 1024D).Round(2) + " MB";
        }
        else
        {
            return (i / (1024D * 1024D)).Round(2) + " GB";
        }
    }
    
    public static double BytesToGb(long bytes)
    {
        const double gbMultiplier = 1024 * 1024 * 1024; // 1 GB = 1024 MB * 1024 KB * 1024 B

        double gigabytes = (double)bytes / gbMultiplier;
        return gigabytes;
    }
}